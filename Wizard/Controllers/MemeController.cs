using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Wizard.Abstractions;
using FileSystem = System.IO.File;

namespace Wizard.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MemeController : ControllerBase
	{
		private readonly Size _batmanCropSize = new Size(79 * 2);

		/*
		 warning: this might break. If so, move this down to where the images are overlaid, and change
		 new Size(191 / 2); to new Size(batman.Width / 2); for BOTH batman and robin.
		 */

		private readonly Point _batmanLocation = new Point(476, 173) - new Size(191 / 2);
		private readonly PngEncoder _encoder = new PngEncoder();
		private readonly MediaTypeHeaderValue _header = MediaTypeHeaderValue.Parse("image/png");
		private readonly Size _robinCropSize = new Size(93 * 2);
		private readonly Point _robinLocation = new Point(244, 265) - new Size(246 / 2);

		[HttpGet("slap")]
		public async Task<IActionResult> SlapAsync([FromQuery] Uri firstUrl, [FromQuery] Uri secondUrl)
		{
			using var client = new HttpClient();

			var first = await client.GetByteArrayAsync(firstUrl);

			var second = await client.GetByteArrayAsync(secondUrl);

			using var baseImage = await Image.LoadAsync(FileSystem.OpenRead("Assets/images/memes/slap.png"));

			using var batman = Image.Load(first);
			using var robin = Image.Load(second);

			// crop & mutate them both
			batman.Mutate(context =>
			{
				context.ConvertToAvatar(_batmanCropSize, 79);
				context.Rotate(-13.96f);
				context.Flip(FlipMode.Horizontal);
			});
			robin.Mutate(context =>
			{
				context.ConvertToAvatar(_robinCropSize, 93);
				context.Rotate(-24.53f);
			});

			// draw avatars on the base image.
			baseImage.Mutate(context =>
			{
				context.DrawImage(batman, _batmanLocation, 1f);
				context.DrawImage(robin, _robinLocation, 1f);
			});

			var stream = new MemoryStream();

			await baseImage.SaveAsync(stream, _encoder);

			stream.Seek(0, SeekOrigin.Begin);

			return new FileStreamResult(stream, _header);
		}
	}
}
