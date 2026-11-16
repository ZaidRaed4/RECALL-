using Godot;
using System;

public partial class Enemy : Area2D
{
	[Export] private Player _player;
	[Export] private float _speed = 40f;
	[Export] private float _chaseSpeed = 70f;
	[Export] private float _patrolDistance = 60f;
	[Export] private float _chaseRange = 80f;
	[Export] private float _attackRange = 20f;
	[Export] private int _hitFrame = 5;

	private AnimatedSprite2D _sprite;
	private float _startX;
	private float _direction = 1f;
	private bool _isAttacking;
	private bool _isDead;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_startX = Position.X;

		_sprite.FrameChanged += OnSpriteFrameChanged;
		_sprite.AnimationFinished += OnSpriteAnimationFinished;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead || _isAttacking)
		{
			return;
		}

		float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

		if (distance <= _attackRange)
		{
			StartAttack();
		}

		else if (distance <= _chaseRange)
		{
			Chase((float)delta);
		}

		else
		{
			Patrol((float)delta);
		}

		_sprite.FlipH = _direction < 0f;
	}

	private void Patrol(float delta)
	{
		Position += new Vector2(_speed * _direction * (float)delta, 0f);

		if (Position.X >= _startX + _patrolDistance)
		{
			_direction = -1f;
		}
		else if (Position.X <= _startX)
		{
			_direction = 1f;
		}
		_sprite.Play("walk");
	}


	private void Chase(float delta)
	{
		_direction = DirectionToPlayer();
		Position += new Vector2(_chaseSpeed * _direction * (float)delta, 0f);
		float x = Mathf.Clamp(Position.X, _startX, _startX + _patrolDistance);
		Position = new Vector2(x, Position.Y);

		_sprite.Play("walk");
	}

	private void StartAttack()
	{
		_isAttacking = true;
		_direction = DirectionToPlayer();
		_sprite.Play("attack");
	}

	private float DirectionToPlayer()
	{
		if (_player.GlobalPosition.X < GlobalPosition.X)
		{
			return -1f;
		}
		return 1f;
	}

	private void OnSpriteFrameChanged()
	{
		if (_isAttacking && _sprite.Frame == _hitFrame)
		{
			if (GlobalPosition.DistanceTo(_player.GlobalPosition) <= _attackRange)
			{
				_player.Die();
			}
		}
	}

	public void Die()
	{
		if (_isDead)
		{
			return;
		}

		_isDead = true;
		_isAttacking = false;
		_sprite.Play("die");
	}

	private void OnSpriteAnimationFinished()
	{
		if (_isDead)
		{
			QueueFree();
			return;
		}

		_isAttacking = false;
	}

}
