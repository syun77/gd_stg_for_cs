using Godot;
using System;

public partial class Shot : Area2D
{
	public Vector2 velocity = Vector2.Zero;
	public override void _Ready()
    {
        GD.Print("shot ready");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        Position += velocity * (float)delta;
		if(Common.Instance.IsInScreen(Position, 4) == false)
        {
			// 画面外に出たら消去.
            QueueFree();
        }
    }
}
