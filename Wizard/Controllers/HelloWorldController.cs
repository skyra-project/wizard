using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Wizard.Controllers
{
	[ApiController]
	[Route("/api")]
	public class HelloWorldController : ControllerBase
	{
		[HttpGet("helloworld")]
		public IActionResult GetHelloWorld()
		{
			using var image = new Image<Rgba32>(500, 500);
			image.Mutate(context =>
			{
				var font = SystemFonts.CreateFont("Arial", 24);
				context.Fill(Color.Black);
				context.DrawText("Hello World", font, Color.White, new PointF(10, 250));
			});

			var stream = new MemoryStream();

			image.Save(stream, new PngEncoder());

			stream.Seek(0, SeekOrigin.Begin);

			return new FileStreamResult(stream, MediaTypeHeaderValue.Parse("image/png"));
		}
	}
}
