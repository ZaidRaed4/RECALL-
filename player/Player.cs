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
	[Export] private float _roomCheckInset = 1f;
	[Export] private float _recallCooldown = 0.4f;
	[Export] private float _blinkTime = 0.06f;
	[Export] private Label _statusLabel;


	private AnimatedSprite2D _sprite;
	private CollisionShape2D _collisionShape;
	private RectangleShape2D _roomCheckShape;
	private float _recallCooldownLeft;


	private Translocator _translocator;

	public override void _PhysicsProcess(double delta)
	{
		_recallCooldownLeft = Mathf.Max(_recallCooldownLeft - (float)delta, 0f);

		if (Input.IsActionJustPressed("translocator_drop"))
		{
			DropTranslocator();
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

		UpdateStatusLabel();
		UpdateAnimation(direction);
	}

	private void UpdateAnimation(float direction)
	{
		if (direction != 0)
		{
			Vector2 scale = _sprite.Scale;
			scale.X = Mathf.Abs(scale.X) * Mathf.Sign(direction);
			_sprite.Scale = scale;
		}

		if (!IsOnFloor())
		{
			if (Velocity.Y < 0)
			{
				_sprite.Play("jump");
			}
			else
				_sprite.Play("fall");
		}
		else if (direction != 0)
		{
			_sprite.Play("run");
		}
		else
			_sprite.Play("idle");
	}

	// Shows the translocator's state in the HUD.
	private void UpdateStatusLabel()
	{
		if (_statusLabel == null)
		{
			return;
		}

		if (!IsInstanceValid(_translocator))
		{
			_statusLabel.Text = "No translocator";
		}
		else if (_recallCooldownLeft > 0f)
		{
			_statusLabel.Text = "Cooling down";
		}
		else
		{
			_statusLabel.Text = "Recall ready";
		}
	}

	private void DropTranslocator()
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
		Blink(_translocator);
	}

	public override void _Ready()
	{
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		Vector2 bodysize = ((RectangleShape2D)_collisionShape.Shape).Size;
		_roomCheckShape = new RectangleShape2D { Size = bodysize - Vector2.One * 2f * _roomCheckInset };
	}

	private void RecallToTranslocator()
	{
		if (!IsInstanceValid(_translocator))
		{
			return;
		}

		if (_recallCooldownLeft > 0f)
		{
			return;
		}

		Vector2 target = _translocator.GlobalPosition;

		if (!HasRoomAt(target))
		{
			GD.Print("Recall blocked: no room to stand there.");
			return;
		}

		GlobalPosition = target;
		Velocity = Vector2.Zero;
		_recallCooldownLeft = _recallCooldown;
		Blink(this);
	}

	// Blinks something 3 times: fade out, fade back in, repeat.
	private void Blink(CanvasItem target)
	{
		Tween tween = target.CreateTween().SetLoops(3);
		tween.TweenProperty(target, "modulate:a", 0f, _blinkTime);
		tween.TweenProperty(target, "modulate:a", 1f, _blinkTime);
	}

	private bool HasRoomAt(Vector2 feetPosition)
	{
		var query = new PhysicsShapeQueryParameters2D
		{
			Shape = _roomCheckShape,
			Transform = new Transform2D(0f, feetPosition + _collisionShape.Position),
			CollisionMask = CollisionMask,
		};

		return GetWorld2D().DirectSpaceState.IntersectShape(query, 1).Count == 0;
	}

}
