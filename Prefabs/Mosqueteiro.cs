using Godot;

public partial class Mosqueteiro : Peca
{
    private bool carregando = false;
    private Vector2 mouseOffset;

    public override void _Ready()
    {
        base._Ready();
        Tipo = Board.Ocupacao.Mosca;
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
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseEvent.Pressed)
            {
                if (gameManager.TurnoAtual != GameManager.QuemJoga.Mosca)
                    return;

                var sprite = GetNode<Sprite2D>("Mosqueteiro_S");
                var spriteRect = new Rect2(
                    GlobalPosition - sprite.Texture.GetSize() / 2,
                    sprite.Texture.GetSize()
                );

                if (spriteRect.HasPoint(GetGlobalMousePosition()))
                {
                    carregando = true;
                    mouseOffset = GetGlobalMousePosition() - GlobalPosition;
                    ZIndex = 2;
                }
            }
            else if (carregando)
            {
                carregando = false;
                ZIndex = 1;

                Vector2I destino = board.PosicaoParaIndice(GetGlobalMousePosition());
                
                if (board.TentarMoverPeca(this, destino))
                {
                    UpdateVisualPosition();
                }
                else
                {
                    gameManager.PassarTurno();
                }
            }
        }
    }
}