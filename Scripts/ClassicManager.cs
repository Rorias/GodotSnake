using Godot;
using System;

public partial class ClassicManager : Node
{
	public int currentScore { get; private set; }
	public static int highscore { get; private set; } = 0;
	
	private AppleManager appleManager;
	
	private bool paused = false;

	//// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		appleManager = GetNode<AppleManager>("/root/Main/Apples");
		appleManager.CreateNewApple();
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
   			if (keyEvent.Keycode == Key.Escape && !paused)
			{
				paused = true;
				GetTree().Paused = paused;
			}
			else if (keyEvent.Keycode == Key.Escape && paused)
			{
				paused = false;
				GetTree().Paused = paused;
			}
		}
	}

	public void AddScore(int score)
	{
		currentScore += score;
		GetNode<Label>("../Score").Text = "Score: " + currentScore + "\nHiScore: " + highscore;
	}
	
	public void SetHighscore()
	{
		highscore = currentScore;
		GetNode<Label>("../Score").Text = "Score: " + currentScore + "\nHiScore: " + highscore;
	}
}
