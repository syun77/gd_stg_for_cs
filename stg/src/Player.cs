using Godot;
using System;

public partial class Player : Area2D
{
	private Sprite2D _spr;
	private const float MOVE_SPEED = 500f;
	public override void _Ready()
    {
		_spr = GetNode<Sprite2D>("Sprite");
    }
	public override void _Process(double delta)
    {
        _spr.Rotation += 1f * (float)delta;
		// 移動.
		_UpdateMove(delta);
    }

	/// <summary>
	/// 移動処理.
	/// </summary>
	private void _UpdateMove(double delta)
	{
		Vector2 direction = Vector2.Zero;
		if (Input.IsActionPressed("ui_right"))
		{
			direction.X += 1;
		}
		if (Input.IsActionPressed("ui_left"))
		{
			direction.X -= 1;
		}
		if (Input.IsActionPressed("ui_down"))
		{
			direction.Y += 1;
		}
		if (Input.IsActionPressed("ui_up"))
		{
			direction.Y -= 1;
		}
		direction = direction.Normalized();
		Position += direction * MOVE_SPEED * (float)delta;
	}
}
