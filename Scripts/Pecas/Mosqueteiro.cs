using Godot;

public partial class Mosqueteiro : Peca
{
    public override void _Ready()
    {
        base._Ready();
        Tipo = Board.Ocupacao.Mosca;
    }
}
