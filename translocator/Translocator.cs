using Godot;
using System;

public partial class Translocator : CharacterBody2D
{
	[Export] private float _gravity = 980f;
    
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity.Y += _gravity * (float)delta;
        }
        
        Velocity = velocity;
        MoveAndSlide();
    }

    public void placeAt(Vector2 globalPosition)
    {
        GlobalPosition = globalPosition;
        Velocity = Vector2.Zero;
    }

}
