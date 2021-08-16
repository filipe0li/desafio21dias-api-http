using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace desafio21dias_api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                    await context.Response.WriteAsync($"<h1>Valores do Cookie localhost<br></h1>");
                    string teste = context.Request.Headers["Cookie"];
                    string[] chaveValor = teste.Split(';');
                    await context.Response.WriteAsync("<ul>");
                    foreach (var item in chaveValor)
                    {
                        string[] cv = item.Split('=');
                        if (cv[0].Trim().ToLower() == "_test") // valor do cookie
                        {
                            await context.Response.WriteAsync("<li>");
                            await context.Response.WriteAsync($"<b>Chave</b> - {cv[0]}<br>");
                            await context.Response.WriteAsync($"<b>Valor</b> - {cv[1]}<br>");
                            await context.Response.WriteAsync("</li>");
                        }
                    }
                    await context.Response.WriteAsync("</li>");
                    await context.Response.WriteAsync($"Hello World! - {teste}");
                });
                endpoints.MapGet("/csv", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "text/csv; charset=utf-8");
                    await context.Response.WriteAsync("Chave; Valor;\n");
                    string teste = context.Request.Headers["Cookie"];
                    string[] chaveValor = teste.Split(';');
                    foreach (var item in chaveValor)
                    {
                        string[] cv = item.Split('=');
                        await context.Response.WriteAsync($"{cv[0]};{cv[1]}\n");
                    }
                });
                endpoints.MapGet("/pdf", async context =>
                {
                    context.Response.Headers.Add("Content-Type", "application/pdf; charset=utf-8");
                    var document = new PdfDocument();
                    document.Info.Title = "Created with PDFsharp";
                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    var font = new XFont("Verdana", 20.0, XFontStyle.BoldItalic);
                    gfx.DrawString("Hello, World!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
                    const string filename = "tmp/HelloWorld.pdf";
                    document.Save(filename);
                    string doc = File.ReadAllText(filename);
                    await context.Response.WriteAsync(doc);
                });
            });
        }
    }
}
