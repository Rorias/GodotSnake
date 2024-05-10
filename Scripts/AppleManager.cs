using Godot;
using Godot.Collections;
using System;

public partial class AppleManager : Node
{
	private PackedScene applePrefab = GD.Load<PackedScene>("res://apple.tscn");
	
	public Array<Node2D> apples = new Array<Node2D>();

	public void CreateNewApple(bool respawnable = true)
	{
		var instance = applePrefab.Instantiate();
		AddChild(instance);
		apples.Add(instance.GetNode<Node2D>("."));
	}

	public void RemoveApple(Node2D _apple)
	{
		apples.Remove(_apple);
		_apple.QueueFree();
	}
}
