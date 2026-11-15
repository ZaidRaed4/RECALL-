using Godot;
using System;

public partial class Spell : Area2D
{
	[Export] private float _speed = 200f;
	[Export] private float _lifetime = 1.5f;

	private float _direction = 1f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public void Launch(Vector2 globalposition, float direction)
	{
		GlobalPosition = globalposition;
		_direction = direction;
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += new Vector2(_speed * _direction * (float)delta, 0f);

		_lifetime -= (float)delta;
		if(_lifetime <= 0f)
		{
			QueueFree();
		}
	}


	private void OnBodyEntered(Node2D body)
	{
		QueueFree();
	}

}
