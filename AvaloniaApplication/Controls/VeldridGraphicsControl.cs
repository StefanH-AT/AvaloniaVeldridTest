using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Platform;
using Graphics.Core;
using Veldrid;

namespace AvaloniaApplication.Controls;

public abstract class VeldridGraphicsControl : CrossPlatformNativeControl
{

    private EmbeddedGraphicsManager _graphicsManager = default!;
    private Thread _graphicsLoopThread = default!;
    private CancellationTokenSource _loopThreadCancellationTokenSource;
    
    protected abstract EmbeddedGraphicsManager CreateManager(IEmbeddedGraphicsManagerOptions options);

    protected abstract GraphicsDeviceOptions CreateGraphicsDeviceOptions();
    
    protected override void CreateWindows(IPlatformHandle parent, Size size)
    {
        IntPtr hwnd = parent.Handle;
        IntPtr hinstance = Marshal.GetHINSTANCE(typeof(Program).Module);
        
        var options = new EmbeddedGraphicsManagerOptionsWindows(CreateGraphicsDeviceOptions(), 
                                                                (uint) size.Width, (uint) size.Height, 
                                                                hwnd, hinstance);
        
        _graphicsManager = CreateManager(options);

        
        WidthProperty.Changed.Subscribe(args =>
        {
            Console.WriteLine("Resizing");
            _graphicsManager.Resize((uint)args.NewValue.Value, (uint)Height);
        });
        //BoundsProperty.Changed.Subscribe(args =>
        //{
        //    _graphicsManager.Resize((uint)args.NewValue.Value.Width, (uint)args.NewValue.Value.Height);
        //});
    }

    protected override void PostCreate()
    {
        _loopThreadCancellationTokenSource = new CancellationTokenSource();
        _graphicsLoopThread = new Thread(() =>
        {
            _graphicsManager.Loop(_loopThreadCancellationTokenSource.Token).Wait();
        });
        _graphicsLoopThread.Start();
    }
    
    protected override void DestroyWindows()
    {
        _loopThreadCancellationTokenSource.Cancel();
    }

}