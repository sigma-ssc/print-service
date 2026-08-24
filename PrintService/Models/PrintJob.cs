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

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace org.sigma.services.PrintService.Models;

public record PrintJob
{
    [Required]
    [BindProperty(Name = "base64String")]
    public required string Base64String { get; set; }

    [Required]
    [BindProperty(Name = "printerName")]
    public required string PrinterName { get; set; }
}
