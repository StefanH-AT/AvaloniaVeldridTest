using Graphics.Core;
using Veldrid;
using Veldrid.SPIRV;

namespace Graphics.TestApp;

public class TestAppGraphicsManager : EmbeddedGraphicsManager
{
    
    protected override IEnumerable<Shader> CreateShaders(ResourceFactory factory)
    {
        yield return factory.CreateFromSpirv(new ShaderDescription(ShaderStages.Vertex,
                                                                LoadEmbeddedAsset("Shaders/Vertex.glsl"),
                                                                "main"),
                                             new ShaderDescription(ShaderStages.Fragment,
                                                 LoadEmbeddedAsset("Shaders/Fragment.glsl"),
                                                 "main"));
    }

    protected override IEnumerable<VertexLayoutDescription> CreateVertexLayouts()
    {
        yield return new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));
    }

    protected override Pipeline CreatePipeline(ResourceFactory factory, IEnumerable<Shader> shaders, IEnumerable<VertexLayoutDescription> vertexLayouts)
    {
        var pipelineDescription = new GraphicsPipelineDescription
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = new DepthStencilStateDescription(
                true,
                true,
                ComparisonKind.LessEqual),
            RasterizerState = new RasterizerStateDescription(
                FaceCullMode.Back,
                PolygonFillMode.Solid,
                FrontFace.Clockwise,
                true,
                false),
            PrimitiveTopology = PrimitiveTopology.TriangleStrip,
            ResourceLayouts = Array.Empty<ResourceLayout>(),
            ShaderSet = new ShaderSetDescription(vertexLayouts.ToArray(), shaders.ToArray()),
            Outputs = Swapchain.Framebuffer.OutputDescription
        };
        return factory.CreateGraphicsPipeline(pipelineDescription);
    }

    protected override CommandList CreateCommandList(ResourceFactory factory)
    {
        return factory.CreateCommandList();
    }

    protected override void Setup()
    {
    }

    protected override void Draw(CommandList commandList, Pipeline pipeline)
    {
        commandList.Begin();
        commandList.SetFramebuffer(Swapchain.Framebuffer);
        commandList.ClearColorTarget(0, RgbaFloat.Black);
        
        commandList.SetVertexBuffer(0, _vertexBuffer);
        commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        commandList.SetPipeline(pipeline);
        commandList.DrawIndexed(4, 1, 0, 0, 0);
        commandList.End();
        
        GraphicsDevice.SubmitCommands(commandList);
        GraphicsDevice.SwapBuffers(Swapchain);
    }

    public TestAppGraphicsManager(IEmbeddedGraphicsManagerOptions options) : base(options) { }
}