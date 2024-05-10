using Godot;
using System;

public partial class ClassicManager : Node
{
	public int currentScore { get; private set; }
	public static int highscore { get; private set; } = 0;
	
	private AppleManager appleManager;

	//// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		appleManager = GetNode<AppleManager>("/root/Main/Apples");
		appleManager.CreateNewApple();
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
