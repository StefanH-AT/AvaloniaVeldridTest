using System.Reactive;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using Avalonia.Win32;
using AvaloniaApplication.ViewModels;
using ReactiveUI;
using Test3D;

namespace AvaloniaApplication.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{

    private GraphicsService _graphicsService;
    
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void EndInit()
    {
        var topLevel = (TopLevel) this.GetVisualRoot();
        var windowImpl = (WindowImpl) topLevel.PlatformImpl;
        
        var handle = windowImpl.Handle.Handle;
        var instance = Marshal.GetHINSTANCE(typeof(Program).Module);

        _graphicsService = new GraphicsService(handle, instance);
        _graphicsService.Resize((uint) ClientSize.Width, (uint) ClientSize.Height);

        Thread graphicsThread = new Thread(() =>
        {
            _graphicsService.Loop().Wait();
        });
        graphicsThread.Start();

        base.EndInit();
    }

}