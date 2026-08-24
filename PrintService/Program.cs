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

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Setup Serilog
builder.Logging.ClearProviders();
builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add configurations
builder.Services.Configure<GhostscriptConfig>(builder.Configuration.GetRequiredSection(
    GhostscriptConfig.ConfigSectionKey));

// build app
var app = builder.Build();

// map the "about" endpoint
app.MapGet("/about", () => $"""
    SIGMA Print Service v1.0.0 — Copyright (C) 2026 SIGMA SOFTWARE UND CONSULTING GMBH
    Licensed under the GNU AGPL v3. Source code for this running instance:
    https://github.com/sigma-ssc/print-service
    """);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
