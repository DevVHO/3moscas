using Godot;
using System;
using System.Runtime;

public partial class Board : Node2D
{
    public int rows = 5;
    public int columns = 5;
    private Node2D[,] grid;

    
    [Export] public PackedScene Cellscene;
    [Export] public PackedScene Guarda;
    [Export] public PackedScene Mosca;
    public override void _Ready()
    {
        grid = new Node2D[rows, columns];
        //Pegar a position de target para que assim eu consiga instanciar através dela
        var target = GetNode<Node2D>("Target");

    }

    public void GenerateGrid()
    {
        var target = GetNode<Node2D>("Target");
        Node2D cellTemp = Cellscene.Instantiate<Node2D>();
        Sprite2D spritecell = cellTemp.GetNode<Sprite2D>("Sprite2D");
        Vector2 CellSize = spritecell.Texture.GetSize();
        Vector2 CellScale = cellTemp.Scale;
        cellTemp.QueueFree();
        //Tamanho da grid
        float gridWidth = columns * CellSize.X * CellScale.X;
        float gridHeight = rows * CellSize.Y * CellScale.Y;
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        Vector2 Offset = (screenSize - new Vector2(gridWidth, gridHeight + 40)) / 2;


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
                target.AddChild(cellInstance);

                // Salva referência na matriz
                grid[y, x] = cellInstance;

                char currentChar = boardMatrixChars[y, x];
    Ocupacao ocupacao = CharParaOcupacao(currentChar);

    if (ocupacao == Ocupacao.Guarda)
    {
        Node2D guardaInst = Guarda.Instantiate<Node2D>();
        guardaInst.Position = pos;
        target.AddChild(guardaInst);
    }
    else if (ocupacao == Ocupacao.Mosca)
    {
        Node2D moscaInst = Mosca.Instantiate<Node2D>();
        moscaInst.Position = pos;
        target.AddChild(moscaInst);
    }
                
            }

        }
    }

    private char[,] boardMatrixChars = new char[5, 5]
  {
    { 'G', 'G', 'G', 'G', 'M' },
    { 'G', 'G', 'G', 'G', 'G' },
    { 'G', 'G', 'M', 'G', 'G' },
    { 'G', 'G', 'G', 'G', 'G' },
    { 'M', 'G', 'G', 'G', 'G' }
  };

    private Ocupacao CharParaOcupacao(char c)
    {
        return c switch
        {
            'M' => Ocupacao.Mosca,
            'G' => Ocupacao.Guarda,
            'V' => Ocupacao.Vazio,
            _ => throw new ArgumentException($"Caractere inválido: {c}")
        };
    }


    public enum Ocupacao
    {
        Vazio,
        Mosca,
        Guarda
    }
    
}
