using Godot;
using System;

public partial class Cell : Node2D
{
    public int Row;
    public int Column;
    public Node2D Unit; // Guarda ou Mosqueteiro

    public bool IsOccupied => Unit != null;

    public void SetUnit(Node2D unit)
    {
        Unit = unit;
    }

    public void ClearUnit()
    {
        Unit = null;
    }
}

