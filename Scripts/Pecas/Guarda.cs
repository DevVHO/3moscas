using Godot;

public partial class Guarda : Peca
{
    public override void _Input(InputEvent @event)
    {
        if (gameManager.TurnoAtual != GameManager.QuemJoga.Guarda)
            return;

        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.Pressed)
        {
            Vector2 localMouse = ToLocal(GetGlobalMousePosition());
            var sprite = GetNode<Sprite2D>("Guarda_S");

            Rect2 spriteRect = new Rect2(
                -sprite.Texture.GetSize() / 2,
                sprite.Texture.GetSize()
            );

            if (spriteRect.HasPoint(localMouse))
            {
                if (!estaSelecionado && board.pecaSelecionada == null)
                {
                    // Seleciona e realça casas vizinhas
                    estaSelecionado = true;
                    board.RealcarCasasVizinhas(PosicaoLogica, true);
                    board.pecaSelecionada = this;
                    GD.Print($"[SELECIONADO] Guarda na posição {PosicaoLogica}");
                }
                else if (estaSelecionado)
                {
                    // Deseleciona se clicar novamente
                    estaSelecionado = false;
                    board.RealcarCasasVizinhas(PosicaoLogica, false);
                    board.pecaSelecionada = null;
                    GD.Print($"[DESELECIONADO] Guarda na posição {PosicaoLogica}");
                }
            }
            else if (estaSelecionado)
            {
                // Clicou fora do sprite: tentar ação
                Vector2 mouseNaBoard = GetGlobalMousePosition() - board.GlobalPosition;
                Vector2I destino = board.PosicaoLocalParaLogica(mouseNaBoard);

                if (board.PodeMoverOuAtacar(PosicaoLogica, destino))
                {
                    Vector2I origem = PosicaoLogica;

                    board.MoverOuAtacar(origem, destino);

                    PosicaoLogica = destino;
                    AtualizarPosicaoVisual(board.cellWidth, board.cellHeight);

                    estaSelecionado = false;
                    board.pecaSelecionada = null;
                    board.RealcarCasasVizinhas(origem, false);

                    gameManager.PassarTurno();

                    GD.Print($"[AÇÃO] Guarda movido/atacado de {origem} para {destino}");
                }
            }

            ZIndex = estaSelecionado ? 2 : 1;
        }
    }
}
