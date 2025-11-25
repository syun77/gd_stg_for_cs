using Godot;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;

public partial class Player : Area2D
{
	[Export]
	public PackedScene ShotScene;
	private Sprite2D _spr;
	private int _shotCnt = 0;
	private const float MOVE_SPEED = 300f;
	private const float MOVE_SPEED_SLOW = 100f;
	public override void _Ready()
    {
		_spr = GetNode<Sprite2D>("Sprite");
    }
	public override void _Process(double delta)
    {
		// 移動.
		_UpdateMove(delta);

		var rotSpeed = 2f * (float)delta;

		// ショット.
		if(Input.IsActionPressed("ui_accept"))
        {
            var shot = ShotScene.Instantiate<Shot>();
			shot.Position = Position;
			if(_shotCnt%2 == 0) {
				shot.Position += new Vector2(0, -16);
			}
			var deg = 90+Common.RandFRange(-5, 5);
			shot.SetSpeed(deg, 1500);
			Common.Instance.AddLayerChild("shot", shot);
			_shotCnt++;

			rotSpeed *= 0.5f;
        }

        _spr.Rotation += rotSpeed;
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
		var speed = MOVE_SPEED;
		if(Input.IsActionPressed("ui_accept")) {
			// 長押しで移動速度低下.
			speed = MOVE_SPEED_SLOW;
		}
		Vector2 newPosition = Position + direction * speed * (float)delta;
		
		// Commonクラスを使用して画面内に座標を制限
		if (Common.Instance != null)
		{
			Vector2 spriteSize = _spr.Texture.GetSize();
			Position = Common.Instance.ClampToScreen(newPosition, spriteSize);
		}
		else
		{
			Position = newPosition;
		}
	}

	private void OnAreaEntered(Area2D target)
	{
		var obj = target as Bullet;
		if(obj != null) {
			var deg = Common.ToAngle(obj.velocity);
			deg -= 180 + Common.RandFRange(-30, 30);
			var speed = Common.ToSpeed(obj.velocity);
			Particle.Add(obj.Position, deg, speed, new Godot.Color(1, 0.5f, 0.5f, 1));

			// 敵弾のみ衝突処理.
			target.QueueFree();
		}
	}
}
