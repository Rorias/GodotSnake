using Godot;
using System;

public partial class Apple : Node
{
	public Node2D _this;
	
	public int appleValue { get; private set; }
	
	private PlayerMovement partyManager;
	private Node2D player;
	
	private int appleRadius;
	private RandomNumberGenerator random;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_this = GetNode<Node2D>("..");
		player = GetNode<Node2D>("/root/Main/Player");
		
	 	random = new RandomNumberGenerator();
		random.Randomize();
		
		partyManager = GetNode<PlayerMovement>("/root/Main/Player/PlayerScript");

		SetAppleType();
		SetApplePosition();
	}

	private void SetAppleType()
	{
		appleValue = 1;
		GetNode<CanvasItem>("../AppleVisual").Modulate = new Color(0, 1, 0);
	}

	private void SetApplePosition()
	{
		appleRadius = (partyManager.partyMembers.Count / 2) + 1;
		//GD.Print(appleRadius + " apple radius");
		int newXpos = random.RandiRange(-24, 24);
		//GD.Print(newXpos + " initial x");
//
		if (newXpos < (player.Position.X/32f) - appleRadius)
		{
			//GD.Print((player.Position.X/32f) + " first");
			//GD.Print("Apple is closer on " + ((player.Position.X/32f) - appleRadius));
			newXpos = (int)(player.Position.X/32f) - appleRadius;
		}
		else if (newXpos > (player.Position.X/32f) + appleRadius)
		{
			//GD.Print((player.Position.X/32f) + " second");
			//GD.Print("Apple is closer on " + ((player.Position.X/32f) + appleRadius));
			newXpos = (int)(player.Position.X/32f) + appleRadius;
		}
		//GD.Print(newXpos + " new x");
		
		int newYpos = random.RandiRange(-13, 13);
		//GD.Print(newYpos + " initial y");
//
		if (newYpos < (player.Position.Y/32f) - appleRadius)
		{
			//GD.Print("Apple is closer on " + ((player.Position.Y/32f) - appleRadius));
			newYpos = (int)(player.Position.Y/32f) - appleRadius;
		}
		else if (newYpos > (player.Position.Y/32f) + appleRadius)
		{
			//GD.Print("Apple is closer on " + ((player.Position.Y/32f) + appleRadius));
			newYpos = (int)(player.Position.Y/32f) + appleRadius;
		}
		//GD.Print(newYpos + " new y");
		
		_this.Position = new Vector2(newXpos*32, newYpos*32);
		//GD.Print("apple located at " + _this.Position);
		for (int i = 0; i < partyManager.partyMembers.Count; i++)
		{
			if (partyManager.partyMembers[i].Position == _this.Position)
			{
				SetApplePosition();
			}
		}
	}
}
