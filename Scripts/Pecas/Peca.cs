using Godot;

public partial class Peca : Node2D
{
	public Vector2I PosicaoLogica { get; set; }
	protected bool estaSelecionado = false;

	protected Board board;
	protected GameManager gameManager;
	public override void _Ready()
	{
		gameManager = GetTree().Root.GetNode<GameManager>("Node2D/GameManager");
		board = GetParent().GetParent<Board>(); // Supondo que Target é filho direto do Board
	}

	public void AtualizarPosicaoVisual(int cellWidth, int cellHeight)
	{
		Position = new Vector2(PosicaoLogica.X * cellWidth, PosicaoLogica.Y * cellHeight);
	}
	
}
