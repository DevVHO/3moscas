using Godot;

public partial class Guarda : Peca
{

    public override void _Input(InputEvent @event)
    {
        if (gameManager.TurnoAtual != GameManager.QuemJoga.Guarda)
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
                    board.pecaSelecionada = this;
                    board.RealcarCasasVizinhas(PosicaoLogica, true);
                    GD.Print($"[SELECIONADO] Guarda na posição {PosicaoLogica}");
                }
                else if (estaSelecionado)
                {
                    // Segundo clique: desmarca e remove realce
                    estaSelecionado = false;
                    board.pecaSelecionada = null;
                    board.RealcarCasasVizinhas(PosicaoLogica, false);
                    GD.Print($"[DESELECIONADO] Guarda na posição {PosicaoLogica}");      
                }

                ZIndex = estaSelecionado ? 2 : 1;
            }
        }
    }
}
