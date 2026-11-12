using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private float _gravity = 980f;
	[Export] private float _moveSpeed = 120f;
	[Export] private float _acceleration = 900f;
	[Export] private float _friction = 1200f;
	[Export] private float _jumpSpeed = 300f;
	[Export] private PackedScene _translocatorScene;
	private Translocator _translocator;

	public override void _PhysicsProcess(double delta)
	{

		if (Input.IsActionJustPressed("translocator_drop"))
		{
			dropTranslocator();
		}

		if (Input.IsActionJustPressed("translocator_recall"))
		{
			RecallToTranslocator();
		}
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y += _gravity * (float)delta;
		}

		float direction = Input.GetAxis("move_left", "move_right");

		if (direction != 0)
		{
			velocity.X = Mathf.MoveToward(velocity.X, direction * _moveSpeed, _acceleration * (float)delta);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, _friction * (float)delta);
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = -_jumpSpeed;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void dropTranslocator()
	{
		if (!IsInstanceValid(_translocator))
		{
			if (_translocatorScene == null)
			{
				GD.PushError("Player: drag translocator.tscn into the Translocator Scene slot in the Inspector.");

				return;
			}

			_translocator = _translocatorScene.Instantiate<Translocator>();
			GetParent().AddChild(_translocator);
		}
		_translocator.placeAt(GlobalPosition);

	}


	private void RecallToTranslocator()
	{
		if (!IsInstanceValid(_translocator))
		{
			return;
		}

		GlobalPosition = _translocator.GlobalPosition;
		Velocity = Vector2.Zero;
	}

}
