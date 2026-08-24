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

namespace org.sigma.services.PrintService.Models;

public class GhostscriptConfig
{
    /// <summary>
    /// The name of the config section which is related to this configuration.
    /// </summary>
    public const string ConfigSectionKey = "Ghostscript";

    /// <summary>
    /// Specifies the list of parameters which are passed to the <see cref="Ghostscript"/>-library.
    /// </summary>
    public List<string> Parameter { get; set; } = [];
}
