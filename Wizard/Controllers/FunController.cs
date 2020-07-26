using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.Fonts;
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

		private readonly TextGraphicsOptions _options = new TextGraphicsOptions
		{
			TextOptions = new TextOptions
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			}
		};

		private readonly Polygon _trianglePolygon = new Polygon(new LinearLineSegment(
			new PointF(125, 25),
			new PointF(120, 35 / 2),
			new PointF(130, 35 / 2)));

		private readonly Color _triangleColour = Color.ParseHex("#FFCC33");

		private readonly FontFamily _textFont =
			new FontCollection().Install("./Assets/fonts/Roboto-Light.ttf");

		private readonly Random _random = new Random();
		private readonly Font _topFont;
		private readonly Font _bottomFont;
		private readonly PointF _topLocation = new PointF(300, 100);
		private readonly PointF _bottomLocation = new PointF(300, 150);
		private readonly Image _background = Image.Load(FileSystem.OpenRead("./Assets/images/fun/wof_background.bmp"));
		private readonly Image _wheel = Image.Load(FileSystem.OpenRead("./Assets/images/fun/wof.bmp"));
		private readonly Image _shiny = Image.Load(FileSystem.OpenRead("./Assets/images/social/shiny_small.png"));

		public FunController()
		{
			_topFont = _textFont.CreateFont(23f);
			_bottomFont = _textFont.CreateFont(16f);
		}

		[HttpGet("wof")]
		public async Task<IActionResult> GetWofAsync([FromQuery] float winningAmount, [FromQuery] int bet)
		{
			var rnd = _random.NextDouble();

			var offset = 45f * _winningRotations[winningAmount];


			var rotationAmount = offset + (43f * (float)rnd) + 1f;
			_wheel.Mutate(context => { context.Rotate(-rotationAmount); });

			var won = winningAmount > 1f;
			var text = won ? "Won!" : "Lost.";

			var updatedBalance = bet * winningAmount;

			var measure = TextMeasurer.MeasureBounds($"New Balance: {updatedBalance}", new RendererOptions(_textFont.CreateFont(16f)));

			_background.Mutate(context =>
			{
				context.DrawImage(_wheel, new Point(125 - (_wheel.Width / 2), 125 - (_wheel.Height / 2)), 1f);
				context.Fill(_triangleColour, _trianglePolygon);
				context.DrawText(_options, $"You {text}", _topFont, Color.White, _topLocation);
				context.DrawText(_options, $"New Balance: {updatedBalance}", _bottomFont, Color.LightGray, _bottomLocation);

				var shinyPoint = new Point(305 + (int) measure.Width / 2, 150 - 7);

				context.DrawImage(_shiny, shinyPoint, 1f);
			});



			var stream = new MemoryStream();

			await _background.SaveAsync(stream, Encoder);

			stream.Seek(0, SeekOrigin.Begin);

			return new FileStreamResult(stream, Header);
		}
	}
}
