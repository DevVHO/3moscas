using Godot;

public partial class Guarda : Peca
{
    public override void _Ready()
    {
        base._Ready();
        Tipo = Board.Ocupacao.Guarda;
    }
}
