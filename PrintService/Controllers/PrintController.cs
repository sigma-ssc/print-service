/*
Copyright (C) 2026 SIGMA SOFTWARE UND CONSULTING GMBH

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>. 
*/

using Microsoft.AspNetCore.Mvc;

namespace org.sigma.services.PrintService.Controllers;

[ApiController]
[Route("")]
public class PrintController : Controller
{
    #region Constants
    /// <summary>
    /// The name of the subDirectory where all printJobs are stored temporary.
    /// </summary>
    public const string PrintJobSubDirectoryName = "Printjobs";
    #endregion

    #region HTTP-Actions
    [HttpGet]
    public string Get() => "SIGMA-PrintService is up and running...";

    [HttpPost("print")]
    public ActionResult Print(PrintJob printerJob, IOptions<GhostscriptConfig> config, ILogger<PrintController> logger)
    {
        string jobGuid = Guid.NewGuid().ToString();
        using (logger.BeginScope(new Dictionary<string, object>()
        {
            ["JobGuid"] = jobGuid,
        }))
        {
            logger.LogInformation("Started processing...");

            try
            {
                // validate parameters
                string? printerName = printerJob.PrinterName;
                if (string.IsNullOrEmpty(printerName))
                    throw new ArgumentException("The printer name was NULL or empty! Printing was cancelled.");

                string? base64String = printerJob.Base64String;
                if (string.IsNullOrEmpty(base64String))
                    throw new ArgumentException($"The base64String was NULL or empty! Printing was cancelled.");

                logger.LogDebug("Trying to fetch list of installed printers...");

                var installedPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
                if (installedPrinters is null)
                    throw new InvalidOperationException("Failed to fetch installed printers!");

                List<string> printerNames = installedPrinters.Cast<string>().ToList();
                if (printerNames.Count == 0)
                    throw new InvalidOperationException("No installed printers found! Printing was cancelled.");

                logger.LogDebug("The following installed printers were found: '{InstalledPrintersList}'",
                        string.Join("'; '", printerNames));

                bool printerExists = printerNames.Exists(p => p.Equals(printerName));
                if (printerExists)
                {
                    logger.LogInformation("Specified Printer '{PrinterName}' was found sucessfully.", printerName);

                    byte[] pdfBytes = Convert.FromBase64String(base64String);

                    logger.LogDebug("Converted from base64String successfully. Saving to file...");

                    // build directory path and check it's existence
                    string fullDirectoryPath = $".\\{PrintJobSubDirectoryName}";
                    if (!Directory.Exists(fullDirectoryPath))
                    {
                        logger.LogDebug("PrintJob-Subdirectory does not exist. Trying to create...");
                        Directory.CreateDirectory(fullDirectoryPath);
                        logger.LogInformation("PrintJob-Subdirectory was created successfully.");
                    }

                    // build file name and attempt to create the local print file
                    // 2026-07-24, ASC: making the file name unique
                    // => avoid having the file overwritten when two print-actions are running at the same time
                    string localPrintFilePath = $"{fullDirectoryPath}\\Printjob_{jobGuid}.pdf";
                    System.IO.File.WriteAllBytes(localPrintFilePath, pdfBytes);

                    logger.LogDebug("Saved to file successfully. Preparing the parameter list...");

                    List<string> parameters = config.Value.Parameter;

                    logger.LogDebug("Applied parameters from configuration successfully. Adding mandatory parameters...");

                    parameters.AddRange([
                        $"-sOutputFile=%printer%{printerName}",
                        "-f",
                        localPrintFilePath
                    ]);

                    logger.LogInformation("Sending printJob to '{PrinterName}'...", printerName);

                    using (GhostscriptProcessor processor = new())
                        processor.StartProcessing(parameters.ToArray(), new SigmaGhostscriptIOLogger(logger));

                    logger.LogInformation("Cleaning up print file '{LocalPrinterFile}'...", localPrintFilePath);

                    System.IO.File.Delete(localPrintFilePath);

                    logger.LogInformation("Printjob was completed successfully.");
                }
                else
                    throw new InvalidOperationException($"Specified Printer '{printerName}' was not found in installed printers. " +
                        "Printing was cancelled.");

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
    #endregion

    #region Nested Type "SigmaGhostscriptIOLogger"
    private class SigmaGhostscriptIOLogger : GhostscriptStdIO
    {
        private ILogger _logger;

        public SigmaGhostscriptIOLogger(ILogger logger)
            : base(handleStdIn: false, handleStdErr: true, handleStdOut: true)
        {
            _logger = logger;
        }

        public override void StdError(string error) => _logger.LogError(error);

        public override void StdIn(out string input, int count)
            => throw new NotImplementedException(/* not handling stdIn currently -> see constructor */);

        public override void StdOut(string output) => _logger.LogTrace(output);
    }
    #endregion
}
