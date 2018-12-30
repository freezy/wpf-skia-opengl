using System.ComponentModel;
using System.Windows;
using SkiaSharp;
using SkiaSharp.Tests;
using SkiaSharp.Views.Desktop;

namespace WpfSkiaOpenGL
{
	public partial class MainWindow : Window
	{
		private SKSurface _surface;
		private GRContext _grContext;
		private SKSize _screenCanvasSize;

		private readonly WglContext _glContext = new WglContext();

		public MainWindow()
		{
			InitializeComponent();
			
			_glContext.MakeCurrent();
		}

		private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
		{
			OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
		}

		private void OnPaintSurface(SKCanvas canvas, int width, int height)
		{
			var canvasSize = new SKSize(width, height);

			// check if we need to recreate the off-screen surface
			if (_screenCanvasSize != canvasSize) {
				_surface?.Dispose();
				_grContext?.Dispose();

				_grContext = GRContext.Create(GRBackend.OpenGL);
				_surface = SKSurface.Create(_grContext, true, new SKImageInfo(width, height));

				_screenCanvasSize = canvasSize;
			}

			// draw onto off-screen gl context
			DrawOffscreen(_surface.Canvas, width, height);

			// draw offscreen surface onto screen
			canvas.DrawSurface(_surface, new SKPoint(0f, 0f));
		}

		private void DrawOffscreen(SKCanvas canvas, int width, int height)
		{
			canvas.Clear(SKColors.Gray.WithAlpha(0x80));

			// will be more expensive in the real world
			using (var paint = new SKPaint()) {
				paint.TextSize = 64.0f;
				paint.IsAntialias = true;
				paint.Color = 0xFF4281A4;
				paint.IsStroke = false;
				canvas.DrawText("SkiaSharp", width / 2f, 64.0f, paint);
			}
		}

		private void OnWindowClosing(object sender, CancelEventArgs e)
		{
			_surface?.Dispose();
			_grContext?.Dispose();
			_glContext.Destroy();
		}
	}
}
