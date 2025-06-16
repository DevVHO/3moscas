using Godot;

public partial class Cell : Node2D
{
    public Vector2I indice;
    private Board board;

    public override void _Ready()
    {
        board = GetParent().GetParent<Board>();
        indice = board.PosicaoParaIndice(Position);

        GD.Print($"Célula criada em Índice={indice}, Posição={Position}");

    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            var localMouse = GetGlobalMousePosition();
            var rect = new Rect2(GlobalPosition - new Vector2(32, 32), new Vector2(64, 64)); // ajuste conforme seu sprite

            if (rect.HasPoint(localMouse))
            {
                board.TentarMoverPeca(board.pecaSelecionada, indice);
            }
        }
    }

    public void Destacar()
    {
        GetNode<Sprite2D>("Sprite2D").Modulate = new Color(1, 1, 0); // amarelo
    }

    public void Resetar()
    {
        GetNode<Sprite2D>("Sprite2D").Modulate = new Color(1, 1, 1); // branco
    }
    public Vector2 IndiceParaPosicao(Vector2I indice)
{
    int cellSize = 216; // DEVE SER O MESMO VALOR USADO NO GenerateGrid()!
    return new Vector2(
+        indice.X * cellSize + cellSize / 2, // Centraliza na célula
        indice.Y * cellSize + cellSize / 2
    );
}
}

