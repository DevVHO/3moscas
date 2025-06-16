using Godot;
using System;

public partial class GameManager : Node2D
{
    public int numerodeselecionada;
    public enum QuemJoga
    {
        Mosqueteiro,
        Guarda
    }

    public QuemJoga TurnoAtual { get; private set; } = QuemJoga.Mosqueteiro;

    private Board board;

    public override void _Ready()
    {
        board = GetParent().GetNode<Board>("Gridmanager");
        board.GenerateGrid();
    }

    public void PassarTurno()
    {
        TurnoAtual = TurnoAtual == QuemJoga.Mosqueteiro ? QuemJoga.Guarda : QuemJoga.Mosqueteiro;
        GD.Print($"Turno agora é do: {TurnoAtual}");
    }
}