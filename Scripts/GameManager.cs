using Godot;
using System;

public partial class GameManager : Node2D
{
    public static GameManager Instance { get; private set; }

    public enum QuemJoga
    {
        Mosca,
        Guarda
    }

    private QuemJoga turnoAtual = QuemJoga.Guarda;
    public QuemJoga TurnoAtual
    {
        get => turnoAtual;
        private set
        {
            turnoAtual = value;
            OnTurnoMudou?.Invoke(turnoAtual);
        }
    }

    public event Action<QuemJoga> OnTurnoMudou;

    public override void _Ready()
    {
        Instance = this;

        var board = GetParent().GetNode<Board>("Gridmanager");
        board.GenerateGrid();

        TurnoAtual = QuemJoga.Mosca; // Define o primeiro turno
    }

    public void PassarTurno()
    {
        TurnoAtual = (TurnoAtual == QuemJoga.Guarda) ? QuemJoga.Mosca : QuemJoga.Guarda;
        GD.Print("Agora é o turno de: ", TurnoAtual);
    }
}
