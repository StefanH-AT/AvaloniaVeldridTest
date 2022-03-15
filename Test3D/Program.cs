namespace Test3D;

public static class Program
{
    public static void Main(string[] args)
    {
        GraphicsService graphicsService = new GraphicsService(null, null);

        graphicsService.Loop().Wait();
    }
}