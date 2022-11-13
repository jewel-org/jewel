using Silk.NET.Windowing;

internal static class Program
{
    public static IWindow Window { get; private set; }

    static Program()
    {
        var options = WindowOptions.Default;
        options.WindowState = WindowState.Fullscreen;
        Window = Silk.NET.Windowing.Window.Create(options);

        Window.Load += OnLoad;
        Window.Render += OnRender;
    }

    private static void Main() => Window.Run();

    private static void OnLoad()
    {
        Input.Init();
        Renderer.Init();
    }

    private static void OnRender(double delta)
    {
        //ImGui.ShowDemoWindow();
    }

    public static byte[] GetRes(string path)
    {
        using var s = typeof(Program).Assembly.GetManifestResourceStream($"jewel.{path}")!;
        using var ms = new MemoryStream();
        s.CopyTo(ms);
        return ms.ToArray();
    }
}