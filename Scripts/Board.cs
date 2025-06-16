using Godot;
using System;
using System.Runtime;

public partial class Board : Node2D
{
    public int rows = 5;
    public int columns = 5;
    public int cellWidth;
    public int cellHeight;
    public Node2D[,] grid;
    public Ocupacao[,] estadoLogico;
    public Node2D[,] pecasVisuais;
    public Peca pecaSelecionada = null;
    public Peca PecaSelecionada => pecaSelecionada;
    public GameManager gameManager;


    [Export] public PackedScene Cellscene;
    [Export] public PackedScene Guarda;
    [Export] public PackedScene Mosca;
    public override void _Ready()
    {
        //O grid serve como um atributo interno dentro dessa classe
        grid = new Node2D[rows, columns];
        gameManager = GetTree().Root.GetNode<GameManager>("Node2D/GameManager");
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
    #region enumerados
    public char[,] boardMatrixChars = new char[5, 5]
    {
        { 'G', 'G', 'G', 'G', 'M' },
        { 'G', 'G', 'G', 'G', 'G' },
        { 'G', 'G', 'M', 'G', 'G' },
        { 'G', 'G', 'V', 'G', 'G' },
        { 'M', 'G', 'G', 'G', 'G' }
    };

    public Ocupacao CharParaOcupacao(char c)
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
    #endregion fimenumerados
    public void MoverPeca(Vector2I origem, Vector2I destino, Ocupacao novaOcupacao)
    {
        var peca = pecasVisuais[origem.Y, origem.X];
        pecasVisuais[origem.Y, origem.X] = null;
        pecasVisuais[destino.Y, destino.X] = peca;

        estadoLogico[origem.Y, origem.X] = Ocupacao.Vazio;
        estadoLogico[destino.Y, destino.X] = novaOcupacao;
    }
    public void MoverOuAtacar(Vector2I origem, Vector2I destino)
    {
        Ocupacao origemOcupacao = estadoLogico[origem.Y, origem.X];
        Ocupacao destinoOcupacao = estadoLogico[destino.Y, destino.X];

        if (origemOcupacao == Ocupacao.Guarda)
        {
            // Movimento normal, destino deve estar vazio
            MoverPeca(origem, destino, Ocupacao.Guarda);
        }
        else if (origemOcupacao == Ocupacao.Mosca && destinoOcupacao == Ocupacao.Guarda)
        {
            // Ataque do Mosqueteiro, remove Guarda e move Mosqueteiro
            // Remove visual do Guarda
            var pecaAtacada = pecasVisuais[destino.Y, destino.X];
            if (pecaAtacada != null)
                pecaAtacada.QueueFree();

            // Atualiza arrays e estado lógico
            pecasVisuais[destino.Y, destino.X] = pecasVisuais[origem.Y, origem.X];
            pecasVisuais[origem.Y, origem.X] = null;

            estadoLogico[destino.Y, destino.X] = Ocupacao.Mosca;
            estadoLogico[origem.Y, origem.X] = Ocupacao.Vazio;
        }
    }
    public bool EstaDentroDoTabuleiro(Vector2I pos)
    {
        return pos.X >= 0 && pos.X < columns && pos.Y >= 0 && pos.Y < rows;
    }

    public bool PodeMoverOuAtacar(Vector2I origem, Vector2I destino)
    {
        if (!EstaDentroDoTabuleiro(destino))
            return false;

        Ocupacao origemOcupacao = estadoLogico[origem.Y, origem.X];
        Ocupacao destinoOcupacao = estadoLogico[destino.Y, destino.X];

        if (origemOcupacao == Ocupacao.Guarda)
        {
            // Guarda só pode mover para casa vazia adjacente
            return destinoOcupacao == Ocupacao.Vazio && EstaCelulaRealcada(destino);
        }
        else if (origemOcupacao == Ocupacao.Mosca)
        {
            // Mosqueteiro só pode atacar Guarda adjacente
            return destinoOcupacao == Ocupacao.Guarda && EstaCelulaRealcada(destino);
        }

        return false;
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
                var peca = pecasVisuais[vizinho.Y, vizinho.X] as Peca;
                var cell = grid[vizinho.Y, vizinho.X] as Cell;
                if (cell != null)
                {
                    if (gameManager.TurnoAtual == GameManager.QuemJoga.Mosqueteiro)
                    {
                        // Mosqueteiro só pode atacar Guarda
                        if (peca is Guarda)
                        {
                            cell.Realcar(ativar);
                            cell.EstaRealcada = ativar;
                        }
                        else
                        {
                            cell.Realcar(false);
                            cell.EstaRealcada = false;
                        }
                    }
                    else if (gameManager.TurnoAtual == GameManager.QuemJoga.Guarda)
                    {
                        // Guarda só pode andar para casas vazias
                        if (estadoLogico[vizinho.Y, vizinho.X] == Ocupacao.Vazio)
                        {
                            cell.Realcar(ativar);
                            cell.EstaRealcada = ativar;
                        }
                        else
                        {
                            cell.Realcar(false);
                            cell.EstaRealcada = false;
                        }
                    }
                }
            }
        }
    }
    public bool EstaCelulaRealcada(Vector2I pos)
    {
        if (!EstaDentroDoTabuleiro(pos))
            return false;

        var cell = grid[pos.Y, pos.X] as Cell;
        if (cell != null)
            return cell.EstaRealcada;

        return false;
    }
    public Vector2I PosicaoLocalParaLogica(Vector2 posicaoLocal)
    {
        int x = (int)(posicaoLocal.X / cellWidth);
        int y = (int)(posicaoLocal.Y / cellHeight);
        return new Vector2I(x, y);
    }
    public Vector2I GetCelulaLogicaAPartirDoMouse(Vector2 globalMousePos)
{
    int col = (int)(globalMousePos.X / cellWidth);
    int row = (int)(globalMousePos.Y / cellHeight);
    return new Vector2I(col, row);
}



}
