using Godot;

public partial class GameManager : Node2D
{
    public enum QuemJoga
    {
        Mosca,
        Guarda
    }

    public QuemJoga TurnoAtual { get; private set; } = QuemJoga.Guarda;
    public override void _Ready()
    {
        var board = GetParent().GetNode<Board>("Gridmanager");
        board.GenerateGrid();

        TurnoAtual = QuemJoga.Mosca;
    }
    
     public void PassarTurno()
    {
        TurnoAtual = (TurnoAtual == QuemJoga.Guarda) ? QuemJoga.Mosca : QuemJoga.Guarda;
    }
}