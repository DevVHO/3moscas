using Godot;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;

public partial class GameManager : Node
{
    [Export] public PackedScene Cellscene;
    [Export] public PackedScene Guarda;
    [Export] public PackedScene Mosca;

    public enum EstadoJogo { JOGANDO = 0, GANHOU_G = 1, GANHOU_M = 2 };

    public Vector2 CellSize;
    private Vector2 CellScale;
    private Vector2 Offset;
    public int rows = 5;
    public int columns = 5;
    private Node2D[,] grid;
    public override void _Ready()
    {
        //Instancia temporária para adquirir o tamanho do Sprite
        grid = new Node2D[rows, columns];
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        Node2D cellTemp = Cellscene.Instantiate<Node2D>();
        Sprite2D spritecell = cellTemp.GetNode<Sprite2D>("Sprite2D");
        Vector2 CellSize = spritecell.Texture.GetSize();
        Vector2 CellScale = spritecell.Scale;
        cellTemp.QueueFree();
        //Tamanho da grid
        float gridWidth = columns * CellSize.X * CellScale.X;
        float gridHeight = rows * CellSize.Y * CellScale.Y;
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        Vector2 Offset = (screenSize - new Vector2(gridWidth, gridHeight + 40)) / 2.65f;


        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Node2D cellInstance = Cellscene.Instantiate<Node2D>();
                // Define a posição da célula na tela
                Vector2 pos = Offset + new Vector2(x * CellSize.X * CellScale.X, y * CellSize.Y * CellScale.Y);
                cellInstance.Position = pos;
                GD.Print(screenSize);

                // Adiciona à árvore como filha desse node
                AddChild(cellInstance);

                // Salva referência na matriz
                grid[y, x] = cellInstance;

                // Cria  Guardas
                Node2D guardainst = Guarda.Instantiate<Node2D>();
                // Define a posição da célula na tela
                Vector2 pos1 = Offset + new Vector2(x * CellSize.X * CellScale.X, y * CellSize.Y * CellScale.Y);
                guardainst.Position = pos1;

                // Adiciona à árvore como filha desse node
                AddChild(guardainst);
                
            }

        }
    }
    
    public void GerarPeca()
    {
        
    }
}
    