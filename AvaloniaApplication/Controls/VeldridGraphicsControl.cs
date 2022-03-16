using System;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.VisualTree;
using Avalonia.Win32;
using Graphics.Core;
using Test3D;
using Veldrid;

namespace AvaloniaApplication.Controls;

public abstract class VeldridGraphicsControl : CrossPlatformNativeControl
{

    private EmbeddedGraphicsManager _graphicsManager;
    
    protected abstract EmbeddedGraphicsManager CreateManager(IEmbeddedGraphicsManagerOptions options);

    protected abstract GraphicsDeviceOptions CreateGraphicsDeviceOptions();
    
    protected override void CreateWindows(IPlatformHandle parent)
    {
        var topLevel = (TopLevel) this.GetVisualRoot();
        var windowImpl = (WindowImpl) topLevel.PlatformImpl;
        
        IntPtr hwnd = windowImpl.Handle.Handle;
        IntPtr hinstance = Marshal.GetHINSTANCE(typeof(Program).Module);

        var options = new EmbeddedGraphicsManagerOptionsWindows(CreateGraphicsDeviceOptions(), 
                                                                (uint) DesiredSize.Width, (uint) DesiredSize.Height, 
                                                                hwnd, hinstance);
        
        _graphicsManager = CreateManager(options);
    }

    protected override void CreateLinux(IPlatformHandle parent)
    {
        throw new System.NotImplementedException();
    }

    protected override void CreateOSX(IPlatformHandle parent)
    {
        throw new System.NotImplementedException();
    }

    protected override void DestroyWindows(IPlatformHandle control)
    {
        throw new System.NotImplementedException();
    }

    protected override void DestroyLinux(IPlatformHandle control)
    {
        throw new System.NotImplementedException();
    }

    protected override void DestroyOSX(IPlatformHandle control)
    {
        throw new System.NotImplementedException();
    }

}