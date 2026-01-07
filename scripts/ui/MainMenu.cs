using Godot;
using Osiris.System;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        OsiSystem.LoadSession();
    }
}
