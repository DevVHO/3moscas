using Godot;

public partial class Mosqueteiro : Peca
{
    private bool carregando = false;
    private Vector2 posicaoinicial;
    private Vector2 mouseOffset;

    public override void _Ready()
    {
        base._Ready();
        posicaoinicial = GlobalPosition;
        Tipo = Board.Ocupacao.Mosca;
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
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed)
                {
                    if (gameManager.TurnoAtual != GameManager.QuemJoga.Mosca)
                        return;

                    var sprite = GetNode<Sprite2D>("Mosqueteiro_S");
                    Vector2 mousePos = GetGlobalMousePosition();
                    var spriteSize = sprite.Texture.GetSize();
                    var spriteRect = new Rect2(GlobalPosition - spriteSize / 2, spriteSize);

                    if (spriteRect.HasPoint(mousePos))
                    {
                        carregando = true;
                        mouseOffset = mousePos - GlobalPosition;
                        ZIndex = 2;
                    }
                }
                else if (carregando)
                {
                    carregando = false;
                    ZIndex = 1;

                    Vector2 mousePos = GetGlobalMousePosition();
                    Vector2I destino = board.PosicaoParaIndice(mousePos);

                    if (board.TentarMoverPeca(this, destino))
                    {
                        // Sucesso
                        IndiceAtual = destino;
                        GlobalPosition = board.IndiceParaPosicao(destino);
                        gameManager.PassarTurno();
                    }
                    else
                    {
                        // Falha → volta à posição original
                        GlobalPosition = board.IndiceParaPosicao(IndiceAtual);
                    }
                }
            }
        }
    }



}
