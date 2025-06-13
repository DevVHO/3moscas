using Godot;
using System;

public partial class Mosqueteiro : Node2D
{
    private bool carregando = false;
    private Vector2 posicaoinicial;
    private Vector2 mouseOffset;

    public override void _Ready()
    {
        posicaoinicial = GlobalPosition;
    }

    public override void _Process(double delta)
    {
        if (carregando)
        {
            GlobalPosition = GetGlobalMousePosition() - mouseOffset;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Vector2 mousePos = GetGlobalMousePosition();
                var sprite = GetNode<Sprite2D>("Mosqueteiro_S");

                var spriteSize = sprite.Texture.GetSize();
                var spriteRect = new Rect2(GlobalPosition - spriteSize / 2, spriteSize);

                if (spriteRect.HasPoint(mousePos))
                {
                    carregando = true;
                    mouseOffset = mousePos - GlobalPosition;
                    ZIndex = 2; //Trazer para frente
                }
            }
            else if (!mouseEvent.Pressed && carregando)
            {
                carregando = false;
                GlobalPosition = posicaoinicial;
                ZIndex = 1; //Levar de volta para trás
            }
        }
    }
}
