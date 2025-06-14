using Godot;
using System;

public partial class Peca : Node2D
{
    public Vector2I IndiceAtual { get; set; }
    public Board.Ocupacao Tipo { get; set; }
    protected Board board;
    protected GameManager gameManager;

    public override void _Ready()
    {
        // Gridmanager é o pai, e tem o script Board
        board = GetParent() as Board;

        // Sobe para o Node2D e procura o GameManager
        gameManager = GetTree().Root.GetNode<GameManager>("Node2D/GameManager");
    }
}
