using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 敵弾クラス.
/// </summary>
public partial class Enemy : Area2D
{
	class DelayedBatteryInfo
    {
		public float deg = 0; // 角度.
		public float speed = 0; // 速さ.
		public float delay = 0; // 遅延時間(秒).
		public float ax = 0; // 加速度(X)
		public float ay = 0; // 加速度(Y)
		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="_deg"></param>
		/// <param name="_speed"></param>
		/// <param name="_delay"></param>
		/// <param name="_ax"></param>
		/// <param name="_ay"></param>
		public DelayedBatteryInfo(float _deg, float _speed, float _delay, float _ax=0.0f, float _ay=0.0f) {
			deg = _deg;
			speed = _speed;
			delay = _delay;
			ax = _ax;
			ay = _ay;
		}
		public bool Elapse(float delta) {
			// 時間経過 trueで発射可能.
			delay -= delta;
			if (delay <= 0) {
				return true; // 発射できる.
			}
			return false;
		}
        
    }


	[Export]
	public PackedScene BulletScene;
	/// <summary>
	/// 弾のディレイ発射用配列.
	/// </summary>
	List<DelayedBatteryInfo> _batteries = new List<DelayedBatteryInfo>();

	private float _timer;
	public override void _Ready()
	{
	}

	/// <summary>
    /// 固定時間で更新.
    /// </summary>
    /// <param name="delta"></param>
	public override void _PhysicsProcess(double delta)
    {
        _timer += (float)delta;
		if(_timer > 3)
        {
            _timer -= 3;

			// 3回ループする
			for(int i = 0; i < 5; i++)
            {
				_NWay(3, _GetAim(), 30, 100 + (i * 20), i*0.2f);
            }
        }

		// 発射リストを更新する.
		_UpdateBatteries((float)delta);
    }

	private float _GetAim()
    {
        return Common.Instance.GetAim(this.Position);
    }

	private void _UpdateBatteries(float delta)
    {
        var tmp = new List<DelayedBatteryInfo>();
		foreach (var b in _batteries)
		{
			if (b.Elapse(delta))
            {
				// 発射する.
				_Bullet(b.deg, b.speed, b.ax, b.ay);                
            }
			else
            {
				// 次に使い回す.
                tmp.Add(b);
            }
		}
		_batteries = tmp;
    }

	private void _Bullet(float deg, float speed, float delay=0, float ax=0, float ay=0)
    {
        if(delay > 0)
        {
            // 遅延発射なのでリストに追加するだけ.
			_AddBattery(deg, speed, delay, ax, ay);
			return;
        }

		// 発射する.
		var b = BulletScene.Instantiate<Bullet>();
		b.Position = Position;
		b.SetVelocity(deg, speed, ax, ay);
		Common.Instance.AddLayerChild("bullet", b);
    }

	private void _AddBattery(float deg, float speed, float delay, float ax, float ay)
    {
        var b = new DelayedBatteryInfo(deg, speed, delay, ax, ay);
		_batteries.Add(b);
    }

	/// <summary>
	/// N-Wayを撃つ
	/// </summary>
	/// <param name="n">発射数.</param>
	/// <param name="center">中心角度.</param>
	/// <param name="wide">範囲.</param>
	/// <param name="speed">速度.</param>
	/// <param name="delay">発射遅延時間 (秒).</param>
	private void _NWay(int n, float center, float wide, float speed, float delay=0.0f)
    {        
		if (n < 1)
        {
			return; // 発射しない.            
        }
		
		var d = wide / n; // 弾の間隔
		var a = center - (d * 0.5f * (n - 1)); // 開始角度
		for (int i = 0; i < n; i++)
        {
			_Bullet(a, speed, delay);
			a += d;
        }
    }
}
