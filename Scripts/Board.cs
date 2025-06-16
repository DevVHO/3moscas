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
    public Peca pecaSelecionada = null;
    public Peca PecaSelecionada => pecaSelecionada;


    [Export] public PackedScene Cellscene;
    [Export] public PackedScene Guarda;
    [Export] public PackedScene Mosca;
    public override void _Ready()
    {
        //O grid serve como um atributo interno dentro dessa classe
        grid = new Node2D[rows, columns];
        //Pegar a position de target para que assim eu consiga instanciar através dela
    }
    public void SelecionarPecaParaAtaque(Peca peca)
    {
        pecaSelecionada = peca;
        RealcarCasasVizinhas(peca.PosicaoLogica, true);
    }

    public void CancelarSelecao()
    {
        if (pecaSelecionada != null)
        {
            RealcarCasasVizinhas(pecaSelecionada.PosicaoLogica, false);
            pecaSelecionada = null;
        }
    }

    public bool TentarAtacar(Vector2I posAlvo)
    {
        if (pecaSelecionada == null)
            return false;

        // Verifica se posAlvo está na lista de casas vizinhas da peça selecionada
        int dx = Math.Abs(posAlvo.X - pecaSelecionada.PosicaoLogica.X);
        int dy = Math.Abs(posAlvo.Y - pecaSelecionada.PosicaoLogica.Y);

        if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1)) // vizinho direto
        {
            // Implementa a lógica de ataque, exemplo:
            GD.Print($"{pecaSelecionada.GetType().Name} ataca peça na posição {posAlvo}");

            // Depois de atacar, cancela seleção e realce
            CancelarSelecao();
            return true;
        }
        return false;
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
    public void RealcarCasasVizinhas(Vector2I pos, bool ativar)
    {
        int x = pos.X;
        int y = pos.Y;

        // Lista das posições vizinhas (sem incluir a própria)
        Vector2I[] vizinhos = new Vector2I[]
        {
            new Vector2I(x - 1, y), //esquerda
            new Vector2I(x + 1, y), // direita
            new Vector2I(x, y - 1), //baixo
            new Vector2I(x, y + 1), //cima
        };

        foreach (var vizinho in vizinhos)
        {
            if (vizinho.X >= 0 && vizinho.X < columns && vizinho.Y >= 0 && vizinho.Y < rows)
            {
                var cell = grid[vizinho.Y, vizinho.X] as Cell;
                if (cell != null)
                {
                    cell.Realcar(ativar);
                }
            }
        }
    }
}