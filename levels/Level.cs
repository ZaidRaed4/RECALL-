using Godot;
using System;

public partial class Level : Node2D
{
	[Export] private Label _orbLabel;
	[Export] private Label _timerLabel;

	private int _orbsCollected;
	private int _orbsTotal;
	private float _time;

	public override void _Ready()
	{
		foreach (SoulOrb orb in GetTree().GetNodesInGroup("soul_orb"))
		{
			orb.Collected += OnOrbCollected;
			_orbsTotal++;
		}

		UpdateOrbLabel();
	}

	public override void _Process(double delta)
	{
		_time += (float)delta;
		_timerLabel.Text = $"{_time:0.0}";
	}

	private void OnOrbCollected()
	{
		_orbsCollected++;
		UpdateOrbLabel();
	}

	private void UpdateOrbLabel()
	{
		_orbLabel.Text = $"{_orbsCollected}";
	}
}