using System;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wizard.Structures;

namespace Wizard.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MemeController : ControllerBase
	{
		[HttpGet("test")]
		public IActionResult Get([FromBody] SlapData data)
		{
			throw new NotImplementedException();
		}
	}
}
