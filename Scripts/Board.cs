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
        var CellScale = cellTemp.Scale;
        GD.Print(CellScale);
        cellTemp.QueueFree();
        
        //Criei 2 arrays para conseguir separar o visual para o Lógico
        estadoLogico = new Ocupacao[rows, columns];
        pecasVisuais = new Node2D[rows, columns];
        cellWidth = (int)(CellSize.X * CellScale.X);
        cellHeight = (int)(CellSize.Y * CellScale.Y);
        offset = target.GlobalPosition; 


        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                //Posição Visual
                Vector2 pos = new Vector2(x * CellSize.X * CellScale.X, y * CellSize.Y * CellScale.Y);

                //Instaciar uma célula
                Node2D cellInstance = Cellscene.Instantiate<Node2D>();
                cellInstance.Position = pos;
                // Adiciona à árvore como filha desse node
                target.AddChild(cellInstance);
                // Salva referência na matriz
                grid[y, x] = cellInstance;


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

    public bool TentarMoverPeca(Peca peca, Vector2I destino)
    {
        // 1. Verificação básica do tabuleiro
        if (!DentroDoTabuleiro(destino))
        {
            GD.Print("Destino fora do tabuleiro");
            peca.Position = IndiceParaPosicao(peca.IndiceAtual); // Garante o retorno visual
            return false;
        }

        Vector2I origem = peca.IndiceAtual;
        
        // 2. Verifica se está tentando mover para a mesma posição
        if (destino == origem)
        {
            GD.Print("Movimento para mesma posição");
            peca.Position = IndiceParaPosicao(origem); // Garante o retorno visual
            return false;
        }

        // 3. Cálculo de distância - movimento deve ser exatamente 1 casa
        int dx = Mathf.Abs(destino.X - origem.X);
        int dy = Mathf.Abs(destino.Y - origem.Y);
        
        // Movimento válido deve ser adjacente (horizontal/vertical) e apenas 1 casa
        bool movimentoValido = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);

        if (!movimentoValido)
        {
            GD.Print("Movimento inválido: deve ser adjacente horizontal/vertical");
            peca.Position = IndiceParaPosicao(origem); // Garante o retorno visual
            return false;
        }

        // 4. Lógica específica para Mosqueteiros
        if (peca.Tipo == Ocupacao.Mosca)
        {
            // Mosqueteiro só pode mover para posições com guardas
            if (estadoLogico[destino.Y, destino.X] != Ocupacao.Guarda)
            {
                GD.Print("Mosqueteiro: Destino deve conter um guarda");
                peca.Position = IndiceParaPosicao(origem);
                return false;
            }

            // Executa a captura
            estadoLogico[origem.Y, origem.X] = Ocupacao.Vazio;
            estadoLogico[destino.Y, destino.X] = Ocupacao.Mosca;

            // Remove o guarda do destino
            pecasVisuais[destino.Y, destino.X]?.QueueFree();
            
            // Move o mosqueteiro
            pecasVisuais[destino.Y, destino.X] = pecasVisuais[origem.Y, origem.X];
            pecasVisuais[origem.Y, origem.X] = null;
            
            // Atualiza ambos: lógico e visual
            peca.IndiceAtual = destino;
            peca.Position = IndiceParaPosicao(destino);
            
            return true;
        }
        // 5. Lógica específica para Guardas
        else if (peca.Tipo == Ocupacao.Guarda)
        {
            // Guarda só pode mover para posições vazias
            if (estadoLogico[destino.Y, destino.X] != Ocupacao.Vazio)
            {
                GD.Print("Guarda: Destino deve estar vazio");
                peca.Position = IndiceParaPosicao(origem);
                return false;
            }

            // Executa o movimento
            estadoLogico[origem.Y, origem.X] = Ocupacao.Vazio;
            estadoLogico[destino.Y, destino.X] = Ocupacao.Guarda;

            // Move o guarda
            pecasVisuais[destino.Y, destino.X] = pecasVisuais[origem.Y, origem.X];
            pecasVisuais[origem.Y, origem.X] = null;
            
            // Atualiza ambos: lógico e visual
            peca.IndiceAtual = destino;
            peca.Position = IndiceParaPosicao(destino);
            
            return true;
        }

        GD.Print("Tipo de peça desconhecido");
        peca.Position = IndiceParaPosicao(origem);
        return false;
    }

    
}
