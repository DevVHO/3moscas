using System;
using System.Diagnostics;
using Godot;

public partial class Guarda : Peca
{
    private bool carregando = false;
    private Vector2 posicaoinicial;
    private Vector2 mouseOffset;

    public override void _Ready()
    {
        posicaoinicial = GlobalPosition;
        Tipo = Board.Ocupacao.Guarda;
        gameManager = GetTree().Root.GetNode<GameManager>("Node2D/GameManager");
    }

    public override void _Process(double delta)
    {
        if (carregando)
        {
            GlobalPosition = GetGlobalMousePosition() - mouseOffset;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                // Só deixa clicar se for a vez do Guarda
                if (gameManager.TurnoAtual != GameManager.QuemJoga.Guarda)
                    return ;
                Vector2 mousePos = GetGlobalMousePosition();
                var sprite = GetNode<Sprite2D>("Guarda_S");
                var spriteSize = sprite.Texture.GetSize();
                var spriteRect = new Rect2(GlobalPosition - spriteSize / 2, spriteSize);
                if (spriteRect.HasPoint(mousePos))
                {
                    carregando = true;
                    mouseOffset = mousePos - GlobalPosition;
                    ZIndex = 2;
                }
            }
            else if (!mouseEvent.Pressed && carregando)
            {
                carregando = false;
                ZIndex = 1; 

                
                Vector2 finalPos = GlobalPosition;
                Vector2I destinoLogico = board.PosicaoParaIndice(finalPos);

                Vector2I origemLogica = board.PosicaoParaIndice(posicaoinicial);
                IndiceAtual = origemLogica;
                bool movimentoValido = board.TentarMoverPeca(this, destinoLogico);

                if (movimentoValido)
                {
                    IndiceAtual = destinoLogico;
                    GlobalPosition = board.IndiceParaPosicao(destinoLogico);
                    gameManager.PassarTurno();
                }
                else
                {
                    // Volta pra posição inicial
                    GlobalPosition = posicaoinicial;
                }
            }
                
        }
    }

    
}
