using Silk.NET.Input;
using Silk.NET.Windowing;

internal static class Window
{
    private static IWindow window;

    public static void Init()
    {
        var options = WindowOptions.Default;
        options.WindowState = WindowState.Maximized;

        window = Silk.NET.Windowing.Window.Create(options);

        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;

        window.Run();
    }

    private static void OnLoad()
    {
        var input = window.CreateInput();
        foreach (var item in input.Keyboards)
            item.KeyDown += KeyDown;
    }

    private static void OnUpdate(double obj)
    { }

    private static void OnRender(double obj)
    { }

    private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
    {
        if (arg2 == Key.Escape)
            window.Close();
    }
}