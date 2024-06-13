using Godot;
using Godot.Collections;
using System;
using System.Collections;

public partial class PlayerMovement : Node
{
	private Node2D _this;

	private PackedScene snakePiecePrefab = GD.Load<PackedScene>("res://tailpiece.tscn");
	private Node2D lastPiece;
	
	private AppleManager appleManager;
	private PowerupManager powerupManager;
	private ClassicManager classicManager;
	private PlayerMovement playerMovement;

	public Array<Node2D> partyMembers = new Array<Node2D>();
	private Array<Vector2> previousMoves = new Array<Vector2>();

	private Vector2 lastDirection;
	public Vector2 currentDirection;
	private Vector2 inputDirection;

	private bool isMoving;
	public float fixedMoveDuration = 0.2f;
	public float moveDuration = 0.2f;

	private Vector2 bufferedDirection;
	public float invFramesDuration = 1.5f;
	private Array<bool> pressed = new Array<bool>();
	
	private Array<bool> closed = new Array<bool>();
	
	//// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_this = GetNode<Node2D>("..");
		
		partyMembers.Add(GetNode<Node2D>("/root/Main/TailPiece"));
		partyMembers.Add(GetNode<Node2D>("/root/Main/TailPiece2"));
		partyMembers.Add(GetNode<Node2D>("/root/Main/TailPiece3"));
		
		appleManager = GetNode<AppleManager>("/root/Main/Apples");
		powerupManager = GetNode<PowerupManager>("/root/Main/Powerups");
		classicManager = GetNode<ClassicManager>("/root/Main/ClassicManager");
		
		lastPiece = GetNode<Node2D>("/root/Main/TailPiece3");
	}
//
	//// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var _delta = (float)delta;
		
		EatApple();
		EatPowerup();

		if (currentDirection == lastDirection) { currentDirection = new Vector2(0,0); }

		if (currentDirection == new Vector2(0,0))
		{
			if (bufferedDirection == new Vector2(0,0))
			{
				if (inputDirection == new Vector2(0,0))
				{
					currentDirection = lastDirection;
				}
				else
				{
					currentDirection = inputDirection;
				}
			}
			else
			{
				currentDirection = bufferedDirection;
				bufferedDirection = new Vector2(0,0);
			}
		}
		else if (bufferedDirection == new Vector2(0,0) && inputDirection != new Vector2(0,0) && inputDirection != currentDirection && currentDirection.Dot(inputDirection) != -1)
		{
			bufferedDirection = inputDirection;
		}

		//GD.Print(partyMembers.Count + " outer");

		if (!isMoving)
		{
			if (invFramesDuration < 0 && !powerupManager.invincible)
			{
				for (int i = 0; i < partyMembers.Count; i++)
				{
					for (int j = 0; j < partyMembers.Count; j++)
					{
						if (partyMembers[i].Position == partyMembers[j].Position && partyMembers[i] != partyMembers[j])
						{
							classicManager.SetHighscore();
							GetTree().ReloadCurrentScene();
						}
					}
				}
			}

			if ((_this.Position.X > 768f || _this.Position.X < -768f || _this.Position.Y > 416f || _this.Position.Y < -416f))
			{
				classicManager.SetHighscore();
				GetTree().ReloadCurrentScene();
			}
			else
			{
				MoveCharacters(currentDirection*32f, _delta, moveDuration);
			}
		}

		if (previousMoves.Count > partyMembers.Count)
		{
			previousMoves.RemoveAt(previousMoves.Count - 1);
		}
		
		if (currentDirection != new Vector2(0,0))
		{
			invFramesDuration -= _delta;
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		inputDirection = new Vector2(0,0);
		
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
   			if (keyEvent.Keycode == Key.W)
			{
				inputDirection = new Vector2(0,-1);
			}
			else if (keyEvent.Keycode == Key.D)
			{
				inputDirection = new Vector2(1,0);
			}
			else if (keyEvent.Keycode == Key.S)
			{
				inputDirection = new Vector2(0,1);
			}
			else if (keyEvent.Keycode == Key.A)
			{
				inputDirection = new Vector2(-1,0);
			}
			
			if (currentDirection.Dot(inputDirection) == -1)
			{
				inputDirection = new Vector2(0,0);
			}
		}
	}
	
	private void EatApple()
	{
		for (int i = 0; i < appleManager.apples.Count; i++)
		{
			if (_this.Position == appleManager.apples[i].Position)
			{
				if (fixedMoveDuration >= 0.07f)
				{
					fixedMoveDuration -= 0.01f;
				}

				moveDuration = fixedMoveDuration;

				if (invFramesDuration > 0)
				{
					invFramesDuration += fixedMoveDuration * 4f;
				}
				else
				{
					invFramesDuration = fixedMoveDuration * 4f;
				}

				if (!powerupManager.noGrow)
				{
					var newPiece = snakePiecePrefab.Instantiate();
					AddChild(newPiece);
					newPiece.GetNode<Node2D>(".").Position = lastPiece.Position;
					newPiece.GetNode<CanvasItem>("./TailPieceVisual").Modulate = lastPiece.GetNode<CanvasItem>("./TailPieceVisual").Modulate;
					partyMembers.Add(newPiece.GetNode<Node2D>("."));
					lastPiece = newPiece.GetNode<Node2D>(".");
				}
				
				classicManager.AddScore(appleManager.apples[i].GetNode<Apple>("./AppleScript").appleValue);

				appleManager.CreateNewApple();

				appleManager.RemoveApple(appleManager.apples[i]);
			}
		}
	}
	
	private void EatPowerup()
	{
		for (int i = 0; i < powerupManager.powerups.Count; i++)
		{
			if (_this.Position == powerupManager.powerups[i].Position)
			{
				//GD.Print("is touching the powerup");
				switch (powerupManager.powerups[i].GetNode<Powerup>("./PowerupScript").type)
				{
					case 0:
						powerupManager.noGrow = true;
						break;
					case 1:
						powerupManager.invincible = true;
						break;
					default:
						break;
				}

				for (int c = 0; c < partyMembers.Count; c++)
				{
					partyMembers[c].GetNode<CanvasItem>("./TailPieceVisual").Modulate = powerupManager.powerups[i].GetNode<CanvasItem>("./PowerupVisual").Modulate;
				}

				powerupManager.powerups[i].GetNode<Powerup>("./PowerupScript").powerupActivated = true;
				powerupManager.RemovePowerup(powerupManager.powerups[i]);
			}
		}
	}

	private void MoveCharacters(Vector2 direction, float delta, float moveDuration = 0)
	{
		lastDirection = direction/32f;
		previousMoves.Insert(0, direction);
		MovePlayer(direction, moveDuration, delta);

		for (int i = 1; i < partyMembers.Count + 1 && i < previousMoves.Count; i++)
		{
			MovePartyMember(partyMembers[i - 1], previousMoves[i], moveDuration, delta);
		}
	}

	private async void MovePlayer(Vector2 direction, float timeToMove, float delta)
	{
		isMoving = true;
		float elapsedTime = 0f;
		Vector2 originalPosition = _this.Position;
		Vector2 targetPosition = originalPosition + direction;
		while (elapsedTime < timeToMove)
		{
			_this.Position = originalPosition.Lerp(targetPosition, elapsedTime / timeToMove);
			elapsedTime += delta;
			await ToSignal(GetTree().CreateTimer(delta), "timeout");
		}
		_this.Position = targetPosition;
		isMoving = false;
	}

	private async void MovePartyMember(Node2D member, Vector2 direction, float timeToMove, float delta)
	{
		float elapsedTime = 0f;
		int startCount = partyMembers.Count;
		Vector2 originalPosition = member.Position;
		Vector2 targetPosition = originalPosition + direction;
		
		while (elapsedTime < timeToMove)
		{
			member.Position = originalPosition.Lerp(targetPosition, elapsedTime / timeToMove);
			elapsedTime += delta;
			await ToSignal(GetTree().CreateTimer(delta), "timeout");
		}
			
		member.Position = targetPosition;
	}
}
