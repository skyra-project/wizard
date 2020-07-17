using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using FileSystem = System.IO.File;
using System.Threading.Tasks;

namespace Wizard.Controllers
{
	[ApiController]
	[Route("fun")]
	public class FunController : ImageControllerBase
	{
		private readonly Dictionary<float, int> _winningRotations = new Dictionary<float, int>
		{
			{0.1f, 0},
			{2.4f, 1},
			{0.3f, 2},
			{1.5f, 3},
			{1.2f, 4},
			{1.7f, 5},
			{0.5f, 6},
			{0.2f, 7}
		};

		private readonly Polygon _trianglePolygon = new Polygon(new LinearLineSegment(
			new PointF(250, 50),
			new PointF(240, 35),
			new PointF(260, 35)));

		[HttpGet("wof")]
		public async Task<IActionResult> GetWofAsync([FromQuery] float winningAmount)
		{
			using var image = new Image<Rgb24>(800, 500);
			using var wheel = await Image.LoadAsync(FileSystem.OpenRead("./Assets/images/fun/wof.bmp"));

			var random = new Random();
			var rnd = random.NextDouble();

			var offset = (45f * _winningRotations[winningAmount]);


			var rotationAmount = offset + (43f * (float)rnd) + 1f;
			wheel.Mutate(context => { context.Rotate(-rotationAmount); });

			image.Mutate(context =>
			{
				context.DrawImage(wheel, new Point(250 - (wheel.Width / 2), 250 - (wheel.Height / 2)), 1f);
				context.Fill(Color.Gold, _trianglePolygon);
			});

			var stream = new MemoryStream();

			await image.SaveAsync(stream, Encoder);

			stream.Seek(0, SeekOrigin.Begin);

			return new FileStreamResult(stream, Header);
		}
	}
}
