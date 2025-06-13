using Godot;
using System;

public partial class Board : Node2D
{
    public int rows = 5;
    public int columns = 5;
    private Node2D[,] grid;

    
    [Export] public PackedScene cellScene;
    public override void _Ready()
    {
        grid = new Node2D[rows, columns];
        //Pegar a position de target para que assim eu consiga instanciar através dela
    }

    public void GenerateGrid()
    {
        var temp = cellScene.Instantiate() as Node2D;

        var sprite = temp.GetNode<Sprite2D>("Sprite2D");
        var size = sprite.Texture.GetSize();
        var scale = temp.Scale;
        temp.QueueFree();

        float gridWidth = columns * size.X * scale.X;
        float gridHeight = rows * size.Y * scale.Y;
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        Vector2 offset = (screenSize - new Vector2(gridWidth, gridHeight + 40)) / 2.65f;
        
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector2 pos =  offset + new Vector2(x * size.X * scale.X, y * size.Y * scale.Y);
                var cell = cellScene.Instantiate<Node2D>();
                cell.Position = pos;
                AddChild(cell);
                grid[y, x] = cell;
            }
        }
    }
}
