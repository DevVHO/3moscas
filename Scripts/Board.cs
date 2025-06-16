using Godot;
using System;

public partial class Board : Node2D
{
    public int rows = 5;
    public int columns = 5;
    private int cellWidth;
    private int cellHeight;
    private Cell[,] grid;
    private Ocupacao[,] estadoLogico;
    private Node2D[,] pecasVisuais;


    public Peca pecaSelecionada = null;

    [Export] public PackedScene Cellscene;
    [Export] public PackedScene Guarda;
    [Export] public PackedScene Mosca;

    public override void _Ready()
    {
        grid = new Cell[rows, columns];
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        var target = GetNode<Node2D>("Target");
        Node2D cellTemp = Cellscene.Instantiate<Node2D>();
        Sprite2D spritecell = cellTemp.GetNode<Sprite2D>("Sprite2D");
        Vector2 cellSize = spritecell.Texture.GetSize() * cellTemp.Scale;
        cellTemp.QueueFree();

        estadoLogico = new Ocupacao[rows, columns];
        pecasVisuais = new Node2D[rows, columns];

        cellWidth = (int)cellSize.X;
        cellHeight = (int)cellSize.Y;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector2 pos = new Vector2(x * cellWidth, y * cellHeight);
                Cell cellInstance = Cellscene.Instantiate<Cell>();
                cellInstance.Position = pos;
                target.AddChild(cellInstance);
                grid[y, x] = cellInstance;

                char currentChar = boardMatrixChars[y, x];
                Ocupacao ocupacao = CharParaOcupacao(currentChar);
                estadoLogico[y, x] = ocupacao;

                if (ocupacao == Ocupacao.Guarda)
                {
                    Guarda guardaInst = Guarda.Instantiate<Guarda>();
                    guardaInst.Position = pos;
                    guardaInst.IndiceAtual = new Vector2I(x, y);
                    target.AddChild(guardaInst);
                    pecasVisuais[y, x] = guardaInst;
                }
                else if (ocupacao == Ocupacao.Mosca)
                {
                    Mosqueteiro moscaInst = Mosca.Instantiate<Mosqueteiro>();
                    moscaInst.Position = pos;
                    moscaInst.IndiceAtual = new Vector2I(x, y);
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
        return new Vector2(indice.X * cellWidth, indice.Y * cellHeight);
    }

    public Vector2I PosicaoParaIndice(Vector2 pos)
    {
        // Garante que a posição não seja negativa
        pos = new Vector2(
            Mathf.Max(0, pos.X),
            Mathf.Max(0, pos.Y)
        );

        int x = (int)(pos.X / cellWidth);
        int y = (int)(pos.Y / cellHeight);
        
        // Limita aos índices válidos
        x = Mathf.Clamp(x, 0, columns - 1);
        y = Mathf.Clamp(y, 0, rows - 1);
        
        return new Vector2I(x, y);
    }

    public bool DentroDoTabuleiro(Vector2I indice)
    {
        return indice.X >= 0 && indice.X < columns &&
               indice.Y >= 0 && indice.Y < rows;
    }

    public bool PodeMover(Vector2I de, Vector2I para)
    {
        if (!DentroDoTabuleiro(de) || !DentroDoTabuleiro(para))
        {
            GD.PrintErr("Índices fora do tabuleiro!");
            return false;
        }
        if (!DentroDoTabuleiro(para)) return false;

        Peca peca = EncontrarPecaEm(de) as Peca;
        if (peca == null) return false;

        // Verifica se é movimento ortogonal (horizontal/vertical)
        int dx = Math.Abs(para.X - de.X);
        int dy = Math.Abs(para.Y - de.Y);
        bool movimentoValido = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);

        if (!movimentoValido) return false;

        // Mosqueteiro só pode mover para cima de Guarda
        if (peca.Tipo == Board.Ocupacao.Mosca)
            return estadoLogico[para.Y, para.X] == Board.Ocupacao.Guarda;

        // Guarda só pode mover para vazio
        else if (peca.Tipo == Board.Ocupacao.Guarda)
            return estadoLogico[para.Y, para.X] == Board.Ocupacao.Vazio;

        return false;
    }

    public Node2D EncontrarPecaEm(Vector2I indice)
    {
        if (!DentroDoTabuleiro(indice))
            return null;

        return pecasVisuais[indice.Y, indice.X];
    }

    public bool TentarMoverPeca(Peca peca, Vector2I destino)
    {
        if (peca == null || !DentroDoTabuleiro(destino))
        {
            GD.PrintErr("Peça inválida ou destino fora do tabuleiro!");
            return false;
        }

        Vector2I origem = peca.IndiceAtual;

        // Verifica se o movimento é válido
        if (!PodeMover(origem, destino))
        {
            GD.Print("Movimento inválido!");
            return false;
        }

        // Se for Mosqueteiro (só pode comer Guarda)
        if (peca.Tipo == Board.Ocupacao.Mosca)
        {
            // Remove o Guarda da posição de destino
            var pecaComida = pecasVisuais[destino.Y, destino.X];
            if (pecaComida != null)
            {
                pecaComida.QueueFree();
            }

            // Atualiza estado lógico
            estadoLogico[origem.Y, origem.X] = Board.Ocupacao.Vazio;
            estadoLogico[destino.Y, destino.X] = Board.Ocupacao.Mosca;

            // Move o Mosqueteiro visualmente
            pecasVisuais[destino.Y, destino.X] = pecasVisuais[origem.Y, origem.X];
            pecasVisuais[origem.Y, origem.X] = null;

            // Atualiza posição da peça
            peca.IndiceAtual = destino;
            peca.Position = IndiceParaPosicao(destino);

            GD.Print("Mosqueteiro comeu o Guarda!");
            return true;
        }
        // Se for Guarda (só pode andar para vazio)
        else if (peca.Tipo == Board.Ocupacao.Guarda)
        {
            // (Mantém a lógica atual do Guarda)
            estadoLogico[origem.Y, origem.X] = Board.Ocupacao.Vazio;
            estadoLogico[destino.Y, destino.X] = Board.Ocupacao.Guarda;

            pecasVisuais[destino.Y, destino.X] = pecasVisuais[origem.Y, origem.X];
            pecasVisuais[origem.Y, origem.X] = null;

            peca.IndiceAtual = destino;
            peca.Position = IndiceParaPosicao(destino);

            GD.Print("Guarda moveu-se para vazio!");
            return true;
        }

        return false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            // Debug: mostra posição real do clique
            // Pega a posição GLOBAL do clique
            Vector2 mouseGlobal = GetGlobalMousePosition();

            // Converte para posição LOCAL no tabuleiro
            Vector2 mouseLocal = GetNode<Node2D>("Target").ToLocal(mouseGlobal);

            // Debug crucial (verifique no console)
            GD.Print($"Mouse: Global={mouseGlobal}, Local={mouseLocal}");

            // Converte para índice da matriz
            Vector2I indice = PosicaoParaIndice(mouseLocal);
            GD.Print($"Índice calculado: {indice}");

            if (!DentroDoTabuleiro(indice))
            {
                GD.PrintErr("Clique fora do tabuleiro!");
                return;
            }

            // Se já tem uma peça selecionada, tenta mover
            if (pecaSelecionada != null)
            {
                if (TentarMoverPeca(pecaSelecionada, indice))
                {
                    GameManager.Instance.PassarTurno();
                }
                pecaSelecionada = null;
                LimparDestacadas();
            }
            // Se não, verifica se clicou em uma peça do turno atual
            else
            {
                Node2D peca = EncontrarPecaEm(indice);
                if (peca is Peca pecaClicada &&
                    ((pecaClicada.Tipo == Board.Ocupacao.Mosca && GameManager.Instance.TurnoAtual == GameManager.QuemJoga.Mosca) ||
                    (pecaClicada.Tipo == Board.Ocupacao.Guarda && GameManager.Instance.TurnoAtual == GameManager.QuemJoga.Guarda)))
                {
                    pecaSelecionada = pecaClicada;
                    LimparDestacadas();
                    DestacarMovimentosPossiveis(pecaSelecionada);
                }
            }
        }
    }
    public void DestacarMovimentosPossiveis(Peca peca)
    {
        if (peca == null) return;

        Vector2I origem = peca.IndiceAtual;
        Vector2I[] direcoes = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

        foreach (var dir in direcoes)
        {
            Vector2I destino = origem + dir;
            if (DentroDoTabuleiro(destino) && PodeMover(origem, destino))
            {
                grid[destino.Y, destino.X].Destacar();
            }
        }
    }

    public void LimparDestacadas()
    {
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
                grid[y, x].Resetar();
    }
    public void VerificarSincronizacao()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Node2D pecaVisual = pecasVisuais[y, x];
                Ocupacao estado = estadoLogico[y, x];

                if (pecaVisual == null && estado != Ocupacao.Vazio)
                    GD.PrintErr($"ERRO: Posição ({x},{y}) - Visual=nulo mas Lógico={estado}");

                if (pecaVisual != null)
                {
                    Peca peca = pecaVisual as Peca;
                    if (peca?.Tipo != estado)
                        GD.PrintErr($"ERRO: Posição ({x},{y}) - Visual={peca?.Tipo} vs Lógico={estado}");
                }
            }
        }
        GD.Print("Verificação de sincronização concluída!");
    }
    

}
