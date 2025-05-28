using Godot;
using System;
using System.Data;

public partial class GameManager : Node
{
    [Export] public PackedScene Cell;

    public int rows = 5;
    public int columns = 5;
    private float cellSize = 108f;

    private Node2D[,] grid;
    public override void _Ready()
    {
        //Busca o tamanho da grid


        grid = new Node2D[rows, columns];
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Node2D cellInstance = Cell.Instantiate<Node2D>();

                // Define a posição da célula na tela
                cellInstance.Position = new Vector2(x * cellSize, y * cellSize);

                // Adiciona à árvore como filha desse node
                AddChild(cellInstance);

                // Salva referência na matriz
                grid[y, x] = cellInstance;
            }
        }
    }
}
