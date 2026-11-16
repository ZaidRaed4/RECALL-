using Godot;
using System;

public partial class SoulOrb : Area2D
{
	[Signal] public delegate void CollectedEventHandler();

    public override void _Ready()
    {
        AddToGroup("soul_orb");
		BodyEntered += OnBodyEntered;
    }

	private void OnBodyEntered(Node2D body)
	{
		if(body is Player)
		{
			EmitSignal(SignalName.Collected);
			QueueFree();
		}
	}

}
