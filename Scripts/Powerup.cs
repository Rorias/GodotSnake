using Godot;
using System;

public partial class Powerup : Node
{
	public Node2D _this;
		
	public int type;
	
	public float powerupDuration = 15.0f;
	
	public float powerupDespawnTimer = 10.0f;
	public bool powerupActivated = false;
		
	private PlayerMovement partyManager;
	private Node2D player;
	private RandomNumberGenerator random;
	
	public override void _Ready()
	{
		_this = GetNode<Node2D>("..");
		player = GetNode<Node2D>("/root/Main/Player");
		
		random = new RandomNumberGenerator();
		random.Randomize();
		
		partyManager = GetNode<PlayerMovement>("/root/Main/Player/PlayerScript");
		
		SetPowerupPosition();
	}

	public override void _Process(double delta)
	{
		var _delta = (float)delta;
				
		if (powerupActivated)
		{
			powerupDuration -= _delta;
		}
		else
		{
			powerupDespawnTimer -= _delta;
		}
	}
	
	public void SetPowerupType(int _type)
	{
		type = _type;
		
		if(_type == 0)//noGrow
		{
			GetNode<CanvasItem>("../PowerupVisual").Modulate = new Color(1, 0, 1);
		}
		else if (_type == 1)//invincible
		{
			GetNode<CanvasItem>("../PowerupVisual").Modulate = new Color(1, 1, 0);
		}
	}

	private void SetPowerupPosition()
	{
		int newXpos = random.RandiRange(-24, 24);
		int newYpos = random.RandiRange(-13, 13);
		
		_this.Position = new Vector2(newXpos*32, newYpos*32);
		//GD.Print("powerup located at " + _this.Position);
		
		for (int i = 0; i < partyManager.partyMembers.Count; i++)
		{
			if (partyManager.partyMembers[i].Position == _this.Position)
			{
				SetPowerupPosition();
			}
		}
	}
}
