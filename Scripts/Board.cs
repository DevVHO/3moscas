using Godot;
using System;
using System.Runtime;

public partial class Board : Node2D
{
    public int rows = 5;
    public int columns = 5;
    private int cellWidth;
    private int cellHeight;
    private Vector2 offset;
    private Node2D[,] grid;
    private Ocupacao[,] estadoLogico;
    private Node2D[,] pecasVisuais;


    [Export] public PackedScene Cellscene;
    [Export] public PackedScene Guarda;
    [Export] public PackedScene Mosca;
    public override void _Ready()
    {
        //O grid serve como um atributo interno dentro dessa classe
        grid = new Node2D[rows, columns];
        //Pegar a position de target para que assim eu consiga instanciar através dela

    }


    public void GenerateGrid()
    {
        var target = GetNode<Node2D>("Target");
        Node2D cellTemp = Cellscene.Instantiate<Node2D>();
        Sprite2D spritecell = cellTemp.GetNode<Sprite2D>("Sprite2D");
        Vector2 CellSize = spritecell.Texture.GetSize() * cellTemp.Scale;
        var CellScale = cellTemp.Scale;
        GD.Print(CellScale);
        cellTemp.QueueFree();


        //Criei 2 arrays para conseguir separar o visual para o Lógico
        estadoLogico = new Ocupacao[rows, columns];
        pecasVisuais = new Node2D[rows, columns];
        cellWidth = (int)(CellSize.X);
        cellHeight = (int)(CellSize.Y);


        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                //criação da grid de celulas 
                Vector2 pos = new Vector2(x * cellWidth, y * cellHeight);
                Node2D cellInstance = Cellscene.Instantiate<Node2D>();
                cellInstance.Position = pos;
                target.AddChild(cellInstance);
                grid[y, x] = cellInstance;

                //utilização
                char currentChar = boardMatrixChars[y, x];
                Ocupacao ocupacao = CharParaOcupacao(currentChar);
                estadoLogico[y, x] = ocupacao;
                if (ocupacao == Ocupacao.Mosca)
                {
                    Peca Moscains = Mosca.Instantiate<Peca>();
                    Moscains.PosicaoLogica = new Vector2I(x, y);
                    Moscains.AtualizarPosicaoVisual(cellWidth, cellHeight);
                    target.AddChild(Moscains);
                    pecasVisuais[y, x] = Moscains;
                    Moscains.ZIndex = 2;
                }
                if (ocupacao == Ocupacao.Guarda)
                {
                    Peca Guardains = Guarda.Instantiate<Peca>();
                    Guardains.PosicaoLogica = new Vector2I(x, y);
                    Guardains.AtualizarPosicaoVisual(cellWidth, cellHeight);
                    target.AddChild(Guardains);
                    pecasVisuais[y, x] = Guardains;
                    Guardains.ZIndex = 2;
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