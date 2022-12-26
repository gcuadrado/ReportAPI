using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharp.Charting;
using System;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text.Unicode;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using static System.Net.Mime.MediaTypeNames;

namespace ReportAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IConverter converter;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConverter converter, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            this.converter = converter;
            _hostingEnvironment = hostEnvironment;
        }

        [HttpGet(Name = "GetWeatherForecast")]
public FileContentResult Get()
{
            var pathTemplate=Path.Combine(_hostingEnvironment.ContentRootPath, "template.hbs");
            var source=System.IO.File.ReadAllText(pathTemplate);
            var template = Handlebars.Compile(source);

            var data = new
            {
                title = "My new post 😂😂",
                body = "This is my first post!",
                Buyers = new [] {
                new{
                    Firstname="Gonzalo"
                }
                }.ToList()
            };

            for (int i = 0; i < 5000; i++)
            {
                data.Buyers.Add(new { Firstname = i.ToString() });
            }
            var result = template(data);
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Portrait,
        PaperSize = PaperKind.A4,
    },
                Objects = {
        new ObjectSettings() {
            PagesCount = true,
            HtmlContent = result,
            WebSettings = { DefaultEncoding = "utf-8" },
            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
        }
    }
            };

            var pdf = converter.Convert(doc);

            return File(pdf, "application/pdf");
        }
    }
}