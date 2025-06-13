using Godot;

public partial class GameManager : Node2D
{
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

                char currentChar = boardMatrixChars[y, x];
                Ocupacao ocupacao = CharParaOcupacao(currentChar);

                if (ocupacao == Ocupacao.Guarda)
                {
                    Node2D guardaInst = Guarda.Instantiate<Node2D>();
                    guardaInst.Position = pos;
                    AddChild(guardaInst);
                }
                else if (ocupacao == Ocupacao.Mosca)
                {
                    Node2D moscaInst = Mosca.Instantiate<Node2D>();
                    moscaInst.Position = pos;
                    AddChild(moscaInst);
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
        var board = GetParent().GetNode<Board>("Gridmanager");
        board.GenerateGrid();
    }
    
}
