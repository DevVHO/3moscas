using Godot;
using System;

public partial class GameManager : Node
{
    [Export] public PackedScene Mosqueteiro; 
    public override void _Ready()
    {
        //Instancia do mosqueteiro
        Node MosqueteiroIns = Mosqueteiro.Instantiate();
        AddChild(MosqueteiroIns);
    }

}
