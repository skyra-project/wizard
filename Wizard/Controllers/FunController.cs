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
	public class FunController : ControllerBase
	{

		//TODO: make ImageControllerBase class, that has these fields.
		private readonly PngEncoder _encoder = new PngEncoder();
		private readonly MediaTypeHeaderValue _header = MediaTypeHeaderValue.Parse("image/png");

		private readonly Dictionary<float, int> _winningRotations = new Dictionary<float, int>
		{
			{2.4f, 0},
			{0.3f, 1},
			{1.5f, 2},
			{1.2f, 3},
			{0.5f, 4},
			{1.7f, 5},
			{0.2f, 6},
			{0.1f, 7}
		};
		[HttpGet("wof")]
		public async Task<IActionResult> GetWofAsync([FromQuery] float winningAmount)
		{
			using var image = new Image<Rgb24>(500, 800);
			using var wheel = await Image.LoadAsync(FileSystem.OpenRead("./Assets/images/fun/wheel.png"));

			var random = new Random();
			var rnd = random.NextDouble();

			var rotationAmount = (44f * _winningRotations[winningAmount]) + (float)(44f * rnd) + 0.5f;

			wheel.Mutate(context =>
			{
				context.Rotate(-rotationAmount);
				// var (width, height) = context.GetCurrentSize();
				// var middleX = width / 2;
				// var middleY = height / 2;
				// context.Crop(new Rectangle(middleX - 205, middleY - 205, 410, 410));
			});

			var segments = new LinearLineSegment(
				new PointF(250, 250 - 155),
				new PointF(240, 250 - 165),
			new PointF(260, 250 - 165));

			image.Mutate(context =>
			{
				context.DrawImage(wheel, new Point(250 - (wheel.Width / 2), 250 - (wheel.Height / 2)), 1f);
				context.Draw(new SolidBrush(Color.Brown), 2f, new EllipsePolygon(250f, 250f, 5f));
				context.Draw(new SolidBrush(Color.Yellow), 2f, new Polygon(segments));
			});

			var stream = new MemoryStream();

			await image.SaveAsync(stream, _encoder);

			stream.Seek(0, SeekOrigin.Begin);

			return new FileStreamResult(stream, _header);

		}
	}
}
