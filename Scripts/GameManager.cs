using Godot;

public partial class GameManager : Node2D
{
    public override void _Ready()
    {
        var board = GetParent().GetNode<Board>("Gridmanager");
        board.GenerateGrid();
    }
}
