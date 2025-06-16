using Godot;

public partial class Guarda : Peca
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent 
            && mouseEvent.ButtonIndex == MouseButton.Left 
            && mouseEvent.Pressed)
        {
            // Converte mouse global para local (relativo à peça)
            Vector2 localMouse = ToLocal(GetGlobalMousePosition());

            var sprite = GetNode<Sprite2D>("Guarda_S");

            // Retângulo centrado na origem (posição da peça)
            Rect2 spriteRect = new Rect2(
                -sprite.Texture.GetSize() / 2,
                sprite.Texture.GetSize()
            );

            if (spriteRect.HasPoint(localMouse))
            {
                GD.Print($"[CLICK] Tipo: Guarda | Posição Lógica: {PosicaoLogica}");
                ZIndex = 2;
            }
        }
    }
}