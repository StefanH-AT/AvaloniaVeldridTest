using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace Graphics.Core;

public abstract class EmbeddedGraphicsManager : IDisposable
{

    private int FpsMax = 60;
    private float MaxFrametime => 1.0f / FpsMax;

    protected Swapchain Swapchain { get; }
    protected GraphicsDevice GraphicsDevice { get; }
    
    protected Framebuffer Framebuffer { get; set; }
    
    protected bool Resizing;
    protected uint ResizeTargetWidth;
    protected uint ResizeTargetHeight;

    private IEnumerable<Pipeline> _pipelines = default!;
    private CommandList _commandList = default!;
    
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
            Swapchain.Name = "Embedded Swapchain";
            Framebuffer = Swapchain.Framebuffer;
            Start();
            return;
        }

        throw new ArgumentException("Options passed to EmbeddedGraphicsManager are not an instance of supported platform options");
    }

    public void Resize(uint width, uint height)
    {
        Resizing = true;
        ResizeTargetWidth = width;
        ResizeTargetHeight = height;
    }

    private void Start()
    {
        var factory = GraphicsDevice.ResourceFactory;
        _pipelines = CreatePipelines(factory);
        _commandList = CreateCommandList(factory, _pipelines);

        Setup(factory);
    }

    public Task Loop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _commandList.Begin();
            Draw(_commandList, _pipelines);
            _commandList.End();
            
            GraphicsDevice.SubmitCommands(_commandList);
            GraphicsDevice.SwapBuffers(Swapchain);
        }
        return Task.CompletedTask;
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

    protected Shader[] LoadSpirvShader(string name)
    {
        return GraphicsDevice.ResourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Vertex,
                File.ReadAllBytes($"Shaders/{name}_vertex.glsl"),
                "main"),
            new ShaderDescription(ShaderStages.Fragment,
                File.ReadAllBytes($"Shaders/{name}_fragment.glsl"),
                "main"));
    }

    protected ShaderSetDescription CreateShaderSet(string name, VertexLayoutDescription[] vertexLayout)
    {
        return new ShaderSetDescription(vertexLayout, LoadSpirvShader(name));
    }
    
    // Abstract implementation methods

    protected abstract IEnumerable<Pipeline> CreatePipelines(ResourceFactory factory);
    protected abstract CommandList CreateCommandList(ResourceFactory factory, IEnumerable<Pipeline> pipelines);
    protected abstract void Setup(ResourceFactory factory);

    /// <summary>
    /// Renders the command list. Begin, End and swapchain buffer swapping are done for you
    /// </summary>
    /// <param name="commandList"></param>
    /// <param name="pipelines">Pipelines in the order they were created in <see cref="CreatePipelines"/></param>
    protected abstract void Draw(CommandList commandList, IEnumerable<Pipeline> pipelines);

    public void Dispose()
    {
        _commandList.Dispose();
        Swapchain.Dispose();
        GraphicsDevice.Dispose();
    }
}