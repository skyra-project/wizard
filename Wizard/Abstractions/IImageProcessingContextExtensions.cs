using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Wizard.Abstractions
{
	/// <summary>
	///     Used to crop square images into rounded corners.
	///     Taken with love from the ImageSharp repo:
	///     <see href="https://github.com/SixLabors/Samples/blob/master/ImageSharp/AvatarWithRoundedCorner/Program.cs" />
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public static class IImageProcessingContextExtensions
	{
		/// <summary>
		///     Implements a full image mutating pipeline operating on IImageProcessingContext
		/// </summary>
		/// <param name="processingContext">The processing context to operate upon.</param>
		/// <param name="size">The size for it to be resized too.</param>
		/// <param name="cornerRadius">The radius from the corner to the center.</param>
		/// <returns>A <see cref="IImageProcessingContext" /> to chain processing on.</returns>
		public static IImageProcessingContext ConvertToAvatar(this IImageProcessingContext processingContext, Size size,
			float cornerRadius)
		{
			return processingContext.Resize(new ResizeOptions
			{
				Size = size,
				Mode = ResizeMode.Crop
			}).ApplyRoundedCorners(cornerRadius);
		}

		/// <summary>
		///     This method can be seen as an inline implementation of an `IImageProcessor`:
		///     (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
		/// </summary>
		/// <param name="ctx"> The processing context to operate upon.</param>
		/// <param name="cornerRadius"> The radius from the corner to the center.</param>
		/// <returns> A <see cref="IImageProcessingContext" /> to chain processing on.</returns>
		private static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
		{
			var size = ctx.GetCurrentSize();
			var corners = BuildCorners(size.Width, size.Height, cornerRadius);

			ctx.SetGraphicsOptions(new GraphicsOptions
			{
				Antialias = true,
				AlphaCompositionMode =
					PixelAlphaCompositionMode
						.DestOut // enforces that any part of this shape that has color is punched out of the background
			});

			// mutating in here as we already have a cloned original
			// use any color (not Transparent), so the corners will be clipped
			foreach (var c in corners) ctx = ctx.Fill(Color.Red, c);
			return ctx;
		}

		/// <summary>
		///     Builds the corners of a <see cref="IPathCollection" /> to be used for cropping.
		/// </summary>
		/// <param name="imageWidth"> The width of the image.</param>
		/// <param name="imageHeight"> The height of the image.</param>
		/// <param name="cornerRadius"> The radius from the corner to the center.</param>
		/// <returns>A <see cref="IPathCollection" /> to use for cropping.</returns>
		private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
		{
			// first create a square
			var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

			// then cut out of the square a circle so we are left with a corner
			var cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

			// corner is now a corner shape positions top left
			//lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

			var rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
			var bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

			// move it across the width of the image - the width of the shape
			var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
			var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
			var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

			return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
		}
	}
}
