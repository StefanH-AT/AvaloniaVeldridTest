using System;
using Avalonia.Controls;
using Avalonia.Platform;

namespace AvaloniaApplication.Controls;

public abstract class CrossPlatformNativeControl : NativeControlHost
{
    protected abstract void CreateWindows(IPlatformHandle parent);
    protected abstract void CreateLinux(IPlatformHandle parent);
    protected abstract void CreateOSX(IPlatformHandle parent);

    protected virtual void PostCreate() {}
    
    protected abstract void DestroyWindows(IPlatformHandle control);
    protected abstract void DestroyLinux(IPlatformHandle control);
    protected abstract void DestroyOSX(IPlatformHandle control);
    
    protected virtual void PostDestroy() {}

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (OperatingSystem.IsWindows()) CreateWindows(parent);
        if (OperatingSystem.IsLinux()) CreateLinux(parent);
        if (OperatingSystem.IsMacOS()) CreateOSX(parent);
        
        return base.CreateNativeControlCore(parent);
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        if (OperatingSystem.IsWindows()) DestroyWindows(control);
        if (OperatingSystem.IsLinux()) DestroyLinux(control);
        if (OperatingSystem.IsMacOS()) DestroyOSX(control);
        
        base.DestroyNativeControlCore(control);
    }
}