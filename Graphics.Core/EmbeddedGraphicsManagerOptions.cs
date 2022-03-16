using Veldrid;

namespace Graphics.Core;

public interface IEmbeddedGraphicsManagerOptions
{
    public GraphicsDeviceOptions GraphicsDeviceOptions { get; init; }
    public uint Width { get; init; }
    public uint Height { get; init; }
}

public record EmbeddedGraphicsManagerOptionsWindows(GraphicsDeviceOptions GraphicsDeviceOptions, 
                                                    uint Width, 
                                                    uint Height, 
                                                    IntPtr Hwnd, 
                                                    IntPtr Hinstance) : IEmbeddedGraphicsManagerOptions;