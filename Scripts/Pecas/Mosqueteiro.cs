using Godot;

public partial class Mosqueteiro : Peca
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent 
            && mouseEvent.ButtonIndex == MouseButton.Left 
            && mouseEvent.Pressed)
        {
            // Pega a posição do mouse em coordenadas locais da peça
            Vector2 localMouse = ToLocal(GetGlobalMousePosition());

            var sprite = GetNode<Sprite2D>("Mosqueteiro_S");

            // Retângulo do sprite centrado na origem (posição da peça)
            Rect2 spriteRect = new Rect2(
                -sprite.Texture.GetSize() / 2,
                sprite.Texture.GetSize()
            );

            if (spriteRect.HasPoint(localMouse))
            {
                GD.Print($"[CLICK] Tipo: Mosqueteiro | Posição Lógica: {PosicaoLogica}");
                ZIndex = 2;
            }
        }
    }
}
