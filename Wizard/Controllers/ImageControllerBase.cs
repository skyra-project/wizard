using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp.Formats.Png;

namespace Wizard.Controllers
{
	public class ImageControllerBase : ControllerBase
	{
		protected readonly PngEncoder Encoder = new PngEncoder();
		protected readonly MediaTypeHeaderValue Header = MediaTypeHeaderValue.Parse("image/png");
	}
}
