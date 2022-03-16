using System.Text;
using Veldrid;

namespace Graphics.Core;

public abstract class EmbeddedGraphicsManager
{

    protected Swapchain Swapchain { get; }
    protected GraphicsDevice GraphicsDevice { get; }
    
    /// <summary>
    /// Instantiates the swapchain and graphics devices from the platform options, attaching itself to the passed rendering area.
    ///
    /// When implementing this class, leave this constructor alone. Put startup code in Setup instead
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ArgumentException"></exception>
    protected EmbeddedGraphicsManager(IEmbeddedGraphicsManagerOptions options)
    {
        if (options is EmbeddedGraphicsManagerOptionsWindows optionsWindows)
        {
            GraphicsDevice = GraphicsDevice.CreateD3D11(options.GraphicsDeviceOptions);
            var swapchainSource = SwapchainSource.CreateWin32(optionsWindows.Hwnd, optionsWindows.Hinstance);
            var swapchainDescription = new SwapchainDescription(swapchainSource, options.Width, options.Height, PixelFormat.R32_Float, true);

            Swapchain = GraphicsDevice.ResourceFactory.CreateSwapchain(swapchainDescription);
            Start();
            return;
        }

        throw new ArgumentException("Options passed to EmbeddedGraphicsManager are not an instance of supported platform options");
    }

    public void Resize(uint width, uint height)
    {
        Swapchain.Resize(width, height);
    }

    private void Start()
    {
        var factory = GraphicsDevice.ResourceFactory;
        var shaders = CreateShaders(factory);
        var vertexLayouts = CreateVertexLayouts();
        var pipeline = CreatePipeline(GraphicsDevice.ResourceFactory, shaders, vertexLayouts);
        
        Setup();
    }

    public Task Loop()
    {
        
    }

    protected byte[] LoadEmbeddedAsset(string assetName)
    {
        Stream? stream = GetType().Assembly.GetManifestResourceStream(assetName);
        if (stream is null)
        {
            throw new InvalidOperationException("Failed to load embedded asset '" + assetName + "'");
        }

        StreamReader reader = new StreamReader(stream);
        return Encoding.Unicode.GetBytes(reader.ReadToEnd());
    }
    
    // Abstract implementation methods

    protected abstract IEnumerable<Shader> CreateShaders(ResourceFactory factory);
    protected abstract IEnumerable<VertexLayoutDescription> CreateVertexLayouts();
    protected abstract Pipeline CreatePipeline(ResourceFactory factory, IEnumerable<Shader> shaders, IEnumerable<VertexLayoutDescription> vertexLayouts);
    protected abstract CommandList CreateCommandList(ResourceFactory factory);
    protected abstract void Setup();
    protected abstract void Draw(CommandList commandList, Pipeline pipeline);
}