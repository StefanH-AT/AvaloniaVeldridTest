using Graphics.Core;
using Graphics.TestApp;
using Veldrid;

namespace AvaloniaApplication.Controls;

public class TestAppGraphicsControl : VeldridGraphicsControl
{
    protected override EmbeddedGraphicsManager CreateManager(IEmbeddedGraphicsManagerOptions options)
    {
        return new TestAppGraphicsManager(options);
    }

    protected override GraphicsDeviceOptions CreateGraphicsDeviceOptions()
    {
        return new GraphicsDeviceOptions
        {
            SyncToVerticalBlank = true
        };
    }
    
    
}