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
    ZIndex = 1;

    var parent = GetParent();
    bool matado = false;

    foreach (Node node in parent.GetChildren())
    {
        if (node is Tile guard)
        {
            // Areas dos sprites
            var guardSprite = guard.GetNode<Sprite2D>("Sprite2D");
            var guardSize = guardSprite.Texture.GetSize();
            var guardArea = new Rect2(guard.GlobalPosition - guardSize / 2, guardSize);

            var thisSprite = GetNode<Sprite2D>("Guarda_S");
            var thisSize = thisSprite.Texture.GetSize();
            var thisArea = new Rect2(GlobalPosition - thisSize / 2, thisSize);

            // Se estiver em cima
            if (guardArea.Intersects(thisArea))
            {
                GlobalPosition = guard.GlobalPosition;
                guard.QueueFree(); // Deletar
                gameManager.PassarTurno();
                matado = true;
                break;
            }
        }
    }

    if (!matado)
    {
        // Return to where it started
        GlobalPosition = posicaoinicial;
    }
}
        }
    }
}
