using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Test3D;

public class GraphicsService : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Sdl2Window _window;
    private readonly Swapchain _swapchain;
    
    private readonly CommandList _commandList;
    private readonly DeviceBuffer _vertexBuffer;
    private readonly DeviceBuffer _indexBuffer;
    private readonly Shader[] _shaders;
    private readonly Pipeline _pipeline;
    
    private const string VertexCode = @"
";

    private const string FragmentCode = @"
";
    
    public GraphicsService(IntPtr? controlHandle, IntPtr? instanceHandle)
    {
       
        ResourceFactory factory = _graphicsDevice.ResourceFactory;
        
        VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector2(-0.75f, 0.75f), RgbaFloat.Red),
            new VertexPositionColor(new Vector2(0.75f, 0.75f), RgbaFloat.Green),
            new VertexPositionColor(new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
            new VertexPositionColor(new Vector2(0.75f, -0.75f), RgbaFloat.Yellow)
        };

        ushort[] quadIndices = { 0, 1, 2, 3 };

        _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
        _indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

        _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);
        _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

        VertexLayoutDescription vertexLayout = 

        ShaderDescription vertexShaderDesc = new ShaderDescription(
            ShaderStages.Vertex,
            Encoding.UTF8.GetBytes(VertexCode),
            "main");
        ShaderDescription fragmentShaderDesc = new ShaderDescription(
            ShaderStages.Fragment,
            Encoding.UTF8.GetBytes(FragmentCode),
            "main");

        _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual);

        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: FaceCullMode.Back,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false);

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
        pipelineDescription.ResourceLayouts = Array.Empty<ResourceLayout>();

        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
            shaders: _shaders);

        pipelineDescription.Outputs = _swapchain.Framebuffer.OutputDescription;
        _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

        _commandList = factory.CreateCommandList();
    }

    public Task Loop()
    {
        while (true)
        {
            //_window.PumpEvents();
            Draw();
        }
        
        Dispose();

        return Task.CompletedTask;
    }

    public void Resize(uint w, uint h)
    {
        _swapchain.Resize(w, h);
    }

    private void Draw()
    {
        _commandList.Begin();
        _commandList.SetFramebuffer(_swapchain.Framebuffer);
        _commandList.ClearColorTarget(0, RgbaFloat.Black);
        
        _commandList.SetVertexBuffer(0, _vertexBuffer);
        _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        _commandList.SetPipeline(_pipeline);
        _commandList.DrawIndexed(4, 1, 0, 0, 0);
        _commandList.End();
        
        _graphicsDevice.SubmitCommands(_commandList);
        _graphicsDevice.SwapBuffers(_swapchain);
    }

    public void Dispose()
    {
        _graphicsDevice.Dispose();
        _commandList.Dispose();
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _pipeline.Dispose();
    }
}