using Avalonia.Controls;
using Avalonia.Platform;

namespace AvaloniaApplication.Controls;

public abstract class CrossPlatformNativeControl : NativeControlHost
{

    private IPlatformHandle _handle = null!;

    protected virtual void CreateWindows(IPlatformHandle parent, Size size)
    {
        throw new ApplicationException("CrossPlatformNativeControl was instantiated on Windows, but CreateWindows has not been overriden");
    }

    protected virtual void CreateLinux(IPlatformHandle parent, Size size)
    {
        throw new ApplicationException("CrossPlatformNativeControl was instantiated on Linux, but CreateLinux has not been overriden");
    }

    protected virtual void CreateOsx(IPlatformHandle parent, Size size)
    {
        throw new ApplicationException("CrossPlatformNativeControl was instantiated on OSX, but CreateOsx has not been overriden");
    }

    protected virtual void PostCreate() {}

    protected virtual void DestroyWindows()
    {
        throw new ApplicationException("CrossPlatformNativeControl was destroyed on Windows, but DestroyWindows has not been overriden");
    }

    protected virtual void DestroyLinux()
    {
        throw new ApplicationException("CrossPlatformNativeControl was destroyed on Linux, but DestroyLinux has not been overriden");
    }

    protected virtual void DestroyOsx()
    {
        throw new ApplicationException("CrossPlatformNativeControl was destroyed on OSX, but DestroyOsx has not been overriden");
    }
    
    protected virtual void PostDestroy() {}

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _handle = parent;
        return base.CreateNativeControlCore(parent);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (OperatingSystem.IsWindows()) CreateWindows(_handle, finalSize);
        if (OperatingSystem.IsLinux()) CreateLinux(_handle, finalSize);
        if (OperatingSystem.IsMacOS()) CreateOsx(_handle, finalSize);
        
        PostCreate();
        return base.ArrangeOverride(finalSize);
    }

    protected override void OnInitialized()
    {
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (OperatingSystem.IsWindows()) DestroyWindows();
        if (OperatingSystem.IsLinux()) DestroyLinux();
        if (OperatingSystem.IsMacOS()) DestroyOsx();
        
        PostDestroy();
    }

}