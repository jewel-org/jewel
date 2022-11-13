using Silk.NET.Input;

internal static class Input
{
    public static IInputContext Context { get; private set; }

    public static IKeyboard Keyboard { get; private set; }
    public static IMouse Mouse { get; private set; }

    static Input()
    {
        Context = Program.Window.CreateInput();

        Keyboard = Context.Keyboards[0];
        Mouse = Context.Mice[0];
    }

    public static void Init()
    { }
}