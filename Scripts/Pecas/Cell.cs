using Godot;
using System;

public partial class Cell : Node2D
{
	public Board.Ocupacao EstadoAtual { get; private set; }
	public bool EstaRealcada = false;
	public Board board;
	private Sprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		board = GetParent().GetParent<Board>(); // Supondo que Target é filho direto do Board
	}
	public void Realcar(bool ativo)
	{
		sprite.SelfModulate = ativo ? new Color(1f, 1f, 0f) : Colors.White;
	}
}
