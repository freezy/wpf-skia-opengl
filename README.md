# WPF + Skia + OpenGL

*A proof-of-concept of a GPU-accelerated surface without using `WindowsFormsHost`*

## Idea

Create a GL context manually, render all the compute-intensive stuff on an 
off-screen surface, and draw the result on a WPF-compatible control (in this 
case `SKElement`).

## Why

SkiaSharp's examples use a [WindowsFormsHost](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.integration.windowsformshost?view=netframework-4.7.2)
control for GPU-accelerated rendering, which uses [OpenTK](https://github.com/opentk/opentk).

There are however a few issues with `WindowsFormsHost`:

- A window with `AllowsTransparency` enabled shows no controls - [no transparency possible](https://social.msdn.microsoft.com/Forums/vstudio/en-US/6f9dd3b5-af92-4076-9b4e-1a770dd52f70/windowsformshost-and-allowstransparency-makes-all-win32-controls-transparent?forum=wpf)
- Given we can't do `AllowsTransparency`, setting `WindowStyle` to `None` still shows a border
- Airspace problems - things like context menus or `DragMove` don't work over a forms host

## How

Instead of using OpenTK, we're creating a GL context through native calls on
`opengl.dll`. This code has already been provided in SkiaSharp's test suite:

```c#
_glContext = new WglContext();
_glContext.MakeCurrent();
```

Then, we initialize an offscreen surface as usual:

```c#
_grContext = GRContext.Create(GRBackend.OpenGL);
_surface = SKSurface.Create(_grContext, true, new SKImageInfo(width, height));
```

After drawing on the surface's canvas, we draw the result back on 
`SKElement`' canvas:

```c#
canvas.DrawSurface(_surface, new SKPoint(0f, 0f));
```

The surface is disposed and recreated when the `SKElement`'s size changes.
