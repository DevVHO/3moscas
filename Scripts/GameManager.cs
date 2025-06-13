using Godot;

public partial class GameManager : Node2D
{
<<<<<<< HEAD
=======
    [Export] public PackedScene Cellscene;
    [Export] public PackedScene Guarda;
    [Export] public PackedScene Mosca;

    public enum EstadoJogo { JOGANDO = 0, GANHOU_G = 1, GANHOU_M = 2 };

    public Vector2 CellSize;
    private Vector2 CellScale;
    private Vector2 Offset;
    public int rows = 5;
    public int columns = 5;
    private Node2D[,] grid;
>>>>>>> bdcb8725d759995e90d2c08ef74b40d4859e4208
    public override void _Ready()
    {
        var board = GetParent().GetNode<Board>("Gridmanager");
        board.GenerateGrid();
    }
}
