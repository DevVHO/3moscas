using Godot;

public partial class Peca : Node2D
{
    public Vector2I IndiceAtual { get; set; }
    public Board.Ocupacao Tipo { get; set; }
    protected Board board;
    protected GameManager gameManager;

    public override void _Ready()
    {
        // Opção 2: Se precisar buscar pelo caminho (mais seguro)

        board = GetNode<Board>("/root/Node2D/Gridmanager");

        // GameManager é irmão do Gridmanager
        gameManager = GetNode<GameManager>("/root/Node2D/GameManager");
    }
    public void UpdateVisualPosition()
    {
        if (board != null)
            Position = board.IndiceParaPosicao(IndiceAtual);
    }
}