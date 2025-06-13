using Godot;

public partial class Guarda : Node2D
{
    private bool carregando = false;
    private Vector2 posicaoinicial;
    private Vector2 mouseOffset;
    private GameManager gameManager;

    public override void _Ready()
    {
        posicaoinicial = GlobalPosition;
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
                    return;

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
                GlobalPosition = posicaoinicial;
                ZIndex = 1;
            }
        }
    }
}
