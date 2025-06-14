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

        //Criei 2 arrays para conseguir separar o visual para o Lógico
        estadoLogico = new Ocupacao[rows, columns];
        pecasVisuais = new Node2D[rows, columns];


        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                //Posição Visual
                Vector2 pos = Offset + new Vector2(x * CellSize.X * CellScale.X, y * CellSize.Y * CellScale.Y);

                //Instaciar uma célula
                Node2D cellInstance = Cellscene.Instantiate<Node2D>();
                cellInstance.Position = pos;
                // Adiciona à árvore como filha desse node
                target.AddChild(cellInstance);
                // Salva referência na matriz
                grid[y, x] = cellInstance;

                //Estado Lógico


                char currentChar = boardMatrixChars[y, x];
                Ocupacao ocupacao = CharParaOcupacao(currentChar);
                estadoLogico[y, x] = ocupacao;

                if (ocupacao == Ocupacao.Guarda)
                {
                    Node2D guardaInst = Guarda.Instantiate<Node2D>();
                    guardaInst.Position = pos;
                    target.AddChild(guardaInst);
                    pecasVisuais[y, x] = guardaInst;
                }
                else if (ocupacao == Ocupacao.Mosca)
                {
                    Node2D moscaInst = Mosca.Instantiate<Node2D>();
                    moscaInst.Position = pos;
                    target.AddChild(moscaInst);
                    pecasVisuais[y, x] = moscaInst;
                }
                else
                {
                    pecasVisuais[y, x] = null;
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
    public Vector2 IndiceParaPosicao(Vector2I indice)
    {
        // Mesmo cálculo que você usa no GenerateGrid
        return offset + new Vector2(indice.X * cellWidth, indice.Y * cellHeight);
    }

    public Vector2I PosicaoParaIndice(Vector2 pos)
    {
        // Inverso do cálculo acima
        int x = Mathf.FloorToInt((pos.X - offset.X) / cellWidth);
        int y = Mathf.FloorToInt((pos.Y - offset.Y) / cellHeight);
        return new Vector2I(x, y);
    }
    public bool DentroDoTabuleiro(Vector2I indice)
    {
        return indice.X >= 0 && indice.X < columns &&
            indice.Y >= 0 && indice.Y < rows;
    }

    public bool PodeMover(Vector2I de, Vector2I For)
    {
        if (!DentroDoTabuleiro(For)) return false;
        return estadoLogico[For.Y, For.X] == Ocupacao.Guarda; // Só pode "comer" guardas
    }
    public Node2D EncontrarPecaEm(Vector2I indice)
    {
        if (!DentroDoTabuleiro(indice)) return null;
        return pecasVisuais[indice.Y, indice.X];
    }
    public bool TentarMoverPeca(Peca peca, Vector2I origem, Vector2I destino)
    {
        // Verifica se destino está dentro do tabuleiro
        if (!DentroDoTabuleiro(destino))
            return false;

        // Verifica se a movimentação é válida com base na peça
        Ocupacao alvo = estadoLogico[destino.Y, destino.X];

        switch (peca.Tipo)
        {
            case Ocupacao.Mosca:
                // Mosqueteiros só podem mover para uma célula com Guarda
                if (alvo != Ocupacao.Guarda)
                    return false;
                break;

            case Ocupacao.Guarda:
                // Guardas só podem mover para célula vazia
                if (alvo != Ocupacao.Vazio)
                    return false;
                break;

            default:
                return false;
        }

        // Movimento válido: atualiza estado lógico
        estadoLogico[origem.Y, origem.X] = Ocupacao.Vazio;
        estadoLogico[destino.Y, destino.X] = peca.Tipo;

        return true;
    }

    public void MoverPeca(Vector2I de, Vector2I For)
    {
        estadoLogico[For.Y, For.X] = Ocupacao.Mosca;
        estadoLogico[de.Y, de.X] = Ocupacao.Vazio;

        var peca = pecasVisuais[de.Y, de.X];
        var destino = pecasVisuais[For.Y, For.X];

        destino?.QueueFree(); // Mata a peça se houver
        pecasVisuais[For.Y, For.X] = peca;
        pecasVisuais[de.Y, de.X] = null;
    }
    
}
