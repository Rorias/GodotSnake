using Godot;
using Godot.Collections;
using System;

public partial class PowerupManager : Node
{
	public bool noGrow = false;
	public bool invincible = false;
	
	private PlayerMovement partyManager;
	
	private PackedScene powerupPrefab = GD.Load<PackedScene>("res://powerup.tscn");
		
	public Array<Node2D> powerups = new Array<Node2D>();

	private float newPowerupSpawnTimer = 3.0f;

	private float duration = 5.0f;
	private float time = 0.0f;
	
	private RandomNumberGenerator random;

	public override void _Ready()
	{
		random = new RandomNumberGenerator();
		random.Randomize();
		
		partyManager = GetNode<PlayerMovement>("/root/Main/Player/PlayerScript");
	}

	public override void _Process(double delta)
	{
		var _delta = (float)delta;
				
		for (int i = 0; i < powerups.Count; i++)
		{
			if (powerups[i].GetNode<Powerup>("./PowerupScript").powerupDuration < 3)
			{
				GD.Print("slowly fading?");
				for (int c = 0; c < partyManager.partyMembers.Count; c++)
				{
					partyManager.partyMembers[c].GetNode<CanvasItem>("./TailPieceVisual").Modulate = partyManager.partyMembers[c].GetNode<CanvasItem>("./TailPieceVisual").Modulate.Lerp(new Color(1, 1, 1), _delta);
				}
				time += _delta / duration;
			}

			if (powerups[i].GetNode<Powerup>("./PowerupScript").powerupDuration <= 0 || powerups[i].GetNode<Powerup>("./PowerupScript").powerupDespawnTimer <= 0)
			{
				DestroyPowerup(powerups[i]);
				break;
			}
		}

		if (newPowerupSpawnTimer <= 0)
		{
			//GD.Print("creating new powerup!");
			int rnd = random.RandiRange(5, 21);
			newPowerupSpawnTimer = rnd;

			CreateNewPowerup();
		}
		
		if (powerups.Count == 0)
		{
			newPowerupSpawnTimer -= _delta;
		}
	}

	public void CreateNewPowerup()
	{
		int rnd;
		SelectNewPowerup(out rnd);

		var instance = powerupPrefab.Instantiate();
		AddChild(instance);
		instance.GetNode<Powerup>("./PowerupScript").SetPowerupType(rnd);
		
		powerups.Add(instance.GetNode<Node2D>("."));
	}

	private void SelectNewPowerup(out int rndOut)
	{
		rndOut = random.RandiRange(0, 1);

		if ((rndOut == 0 && noGrow) || (rndOut == 1 && invincible))
		{
			SelectNewPowerup(out rndOut);
		}
	}

	public void RemovePowerup(Node2D _powerup)
	{
		_powerup.Position = new Vector2(1000, 600);
	}

	public void DestroyPowerup(Node2D _powerup)
	{
		if (_powerup.GetNode<Powerup>("./PowerupScript").powerupActivated)
		{
			switch (_powerup.GetNode<Powerup>("./PowerupScript").type)
			{
				case 0:
					noGrow = false;
					break;
				case 1:
					invincible = false;
					break;
				default:
					break;
			}

			for (int c = 0; c < partyManager.partyMembers.Count; c++)
			{
				partyManager.partyMembers[c].GetNode<CanvasItem>("./TailPieceVisual").Modulate =  new Color(1, 1, 1);
			}
		}

		powerups.Remove(_powerup);
		_powerup.QueueFree();
	}
}
