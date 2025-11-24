using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Bullet : Area2D
{
	public Vector2 velocity = Vector2.Zero;
	public Vector2 accel = Vector2.Zero;
	public override void _Ready()
	{
	}

	/// <summary>
	/// 弾の速度と加速度を設定します.
	/// </summary>
	/// <param name="deg">弾の進行方向の角度（度）</param>
	/// <param name="speed">弾の速度</param>
	/// <param name="ax">弾の加速度のX成分</param>
	/// <param name="ay">弾の加速度のY成分</param>
	public void SetVelocity(float deg, float speed, float ax=0, float ay=0)
    {
        velocity = Common.AngleToVector2(deg, speed);
        accel = new Vector2(ax, ay);
    }

	public override void _Process(double delta)
	{
		// 移動.
        velocity += accel;
        Position += velocity * (float)delta;

		if(Common.Instance.IsInScreen(this.Position, 4) == false)
        {
			// 画面外に出たので消える.
            QueueFree();
        }
	}
}
