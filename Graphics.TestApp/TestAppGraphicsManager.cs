using System.Numerics;
using Graphics.Core;
using Test3D;
using Veldrid;

namespace Graphics.TestApp;

public class TestAppGraphicsManager : EmbeddedGraphicsManager
{

    private readonly VertexLayoutDescription _vertexLayoutDefault = new(
        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

    private VertexPositionColor[] _meshVertices = default!;
    private ushort[] _meshIndices = default!;
    private DeviceBuffer _vertexBuffer = default!;
    private DeviceBuffer _indexBuffer = default!;
    
    protected override IEnumerable<Pipeline> CreatePipelines(ResourceFactory factory)
    {
        yield return factory.CreateGraphicsPipeline(new GraphicsPipelineDescription
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
            ShaderSet = CreateShaderSet("Default", new[]{ _vertexLayoutDefault }),
            Outputs = Swapchain.Framebuffer.OutputDescription
        });
    }

    protected override CommandList CreateCommandList(ResourceFactory factory, IEnumerable<Pipeline> pipelines)
    {
        return factory.CreateCommandList();
    }

    protected override void Setup(ResourceFactory factory)
    {
        _meshVertices = new[]
        {
            new VertexPositionColor(new Vector2(-0.75f, 0.75f), RgbaFloat.Red),
            new VertexPositionColor(new Vector2(0.75f, 0.75f), RgbaFloat.Green),
            new VertexPositionColor(new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
            new VertexPositionColor(new Vector2(0.75f, -0.75f), RgbaFloat.Yellow)
        };
        _meshIndices = new ushort[] { 0, 1, 2, 3 };
        _vertexBuffer = factory.CreateBuffer(new BufferDescription((uint) _meshVertices.Length * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
        _indexBuffer = factory.CreateBuffer(new BufferDescription((uint) _meshIndices.Length * sizeof(ushort), BufferUsage.IndexBuffer));
        
        GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, _meshVertices);
        GraphicsDevice.UpdateBuffer(_indexBuffer, 0, _meshIndices);
    }

    protected override void Draw(CommandList commandList, IEnumerable<Pipeline> pipelines)
    {
        if (Resizing)
        {
            Swapchain.Resize(ResizeTargetWidth, ResizeTargetHeight);
            Resizing = false;
            Console.WriteLine($"Resizing to {ResizeTargetWidth} {ResizeTargetHeight}");
        }
        commandList.SetFramebuffer(Swapchain.Framebuffer);
        commandList.ClearColorTarget(0, RgbaFloat.Pink);
        
        commandList.SetVertexBuffer(0, _vertexBuffer);
        commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        commandList.SetPipeline(pipelines.First());

        commandList.DrawIndexed(4, 1, 0, 0, 0);
    }

    public TestAppGraphicsManager(IEmbeddedGraphicsManagerOptions options) : base(options) { }
}