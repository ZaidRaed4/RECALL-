using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private float _gravity = 980f;
	[Export] private float _moveSpeed = 120f;
	[Export] private float _acceleration = 900f;
	[Export] private float _friction = 1200f;
	[Export] private float _jumpSpeed =300f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y += _gravity * (float)delta;
		}

		float direction = Input.GetAxis("move_left", "move_right");

		if(direction != 0)
		{
			velocity.X = Mathf.MoveToward(velocity.X, direction * _moveSpeed, _acceleration * (float)delta);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, _friction * (float)delta);
		}

		if(Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = -_jumpSpeed;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
