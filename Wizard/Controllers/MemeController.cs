using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Wizard.Abstractions;
using Wizard.Structures;
using FileSystem = System.IO.File;

namespace Wizard.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MemeController : ControllerBase
	{
		[HttpPost("batslap")]
		public async Task<IActionResult> SlapAsync(
			[FromBody] SlapData data)
		{
			using var client = new HttpClient();

			try
			{
				var first = await client.GetByteArrayAsync(data.FirstUserUrl);

				var second = await client.GetByteArrayAsync(data.SecondUserUrl);

				using var baseImage = await Image.LoadAsync(FileSystem.OpenRead("Assets/images/memes/slap.png"));

				using var batman = Image.Load(first);
				using var robin = Image.Load(second);

				// crop & mutate them both
				batman.Mutate(x =>
				{
					x.ConvertToAvatar(new Size(79 * 2), 79);
					x.Rotate(-13.96f);
					x.Flip(FlipMode.Horizontal);
				});
				robin.Mutate(x =>
				{
					x.ConvertToAvatar(new Size(93 * 2), 93);
					x.Rotate(-24.53f);
				});

				baseImage.Mutate(x =>
				{
					var batmanLocation = new Point(476, 173) - new Size(batman.Width / 2, batman.Height / 2);
					var robinLocation = new Point(244, 265) - new Size(robin.Width / 2, robin.Height / 2);

					x.DrawImage(batman, batmanLocation, 1f);
					x.DrawImage(robin, robinLocation, 1f);
				});

				var stream = new MemoryStream();

				await baseImage.SaveAsync(stream, new PngEncoder());

				stream.Seek(0, SeekOrigin.Begin);

				return new FileStreamResult(stream, MediaTypeHeaderValue.Parse("image/png"));

			}
			catch (HttpRequestException exception)
			{
				return BadRequest(exception);
			}
		}

		public struct SlapData
		{
			public string FirstUserUrl { get; set; }
			public string SecondUserUrl { get; set; }
		}
	}
}
