using Godot;

public partial class Mosqueteiro : Peca
{
    public override void _Input(InputEvent @event)
    {
        if (gameManager.TurnoAtual != GameManager.QuemJoga.Mosqueteiro)
            return;

        if (@event is InputEventMouseButton mouseEvent
            && mouseEvent.ButtonIndex == MouseButton.Left
            && mouseEvent.Pressed)
        {
            Vector2 localMouse = ToLocal(GetGlobalMousePosition());
            var sprite = GetNode<Sprite2D>("Mosqueteiro_S");

            Rect2 spriteRect = new Rect2(
                -sprite.Texture.GetSize() / 2,
                sprite.Texture.GetSize()
            );

            if (spriteRect.HasPoint(localMouse))
            {
                
                if (!estaSelecionado && board.pecaSelecionada == null)
                {
                    // Primeiro clique: seleciona e realça
                    estaSelecionado = true;
                    board.RealcarCasasVizinhas(PosicaoLogica, true);
                    board.pecaSelecionada = this;
                    GD.Print($"[SELECIONADO] Mosqueteiro na posição {PosicaoLogica}");
                }
                else if (estaSelecionado)
                {
                    // Segundo clique: desmarca e remove realce
                    estaSelecionado = false;
                    board.pecaSelecionada = null;
                    board.RealcarCasasVizinhas(PosicaoLogica, false);
                    GD.Print($"[DESELECIONADO] Mosqueteiro na posição {PosicaoLogica}");
                }
                ZIndex = estaSelecionado ? 2 : 1;
            }
        }
    }
}
