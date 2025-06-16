using Godot;
using System;

public partial class Cell : Node2D
{
    public Vector2I GridPosition { get; set; } // Ex: (x, y)
    public Board.Ocupacao EstadoAtual { get; private set; }
    private Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public void DefinirEstado(Board.Ocupacao estado)
    {
        EstadoAtual = estado;

        // Aqui você pode mudar cor, brilho, highlight E >>>ETC<<<. :)
        switch (estado)
        {
            case Board.Ocupacao.Mosca:
                sprite.Modulate = new Color(1f, 1f, 1f); // branco
                break;
            case Board.Ocupacao.Guarda:
                sprite.Modulate = new Color(0.7f, 0.7f, 0.7f); // cinza
                break;
            case Board.Ocupacao.Vazio:
                sprite.Modulate = new Color(0.3f, 0.3f, 0.3f); // escuro
                break;
        }
    }

    public void Realcar(bool ativo)
    {
        sprite.SelfModulate = ativo ? new Color(1f, 1f, 0f) : Colors.White;
    }
}
