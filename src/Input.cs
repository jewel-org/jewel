using Silk.NET.Input;

internal static class Input
{
    public static IInputContext Context { get; } = Program.Window.CreateInput();

    public static IKeyboard Keyboard => Context.Keyboards[0];
    public static IMouse Mouse => Context.Mice[0];

    public static void Init()
    { }
}