using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime;
using System.Threading;

/// <summary>
/// 敵弾クラス.
/// </summary>
public partial class Enemy : Area2D
{
	private static readonly PackedScene ENEMY_SCENE = GD.Load<PackedScene>("res://src/Enemy.tscn");

	public static void Add(Vector2 pos, int id, float deg, float speed)
    {
        var e = ENEMY_SCENE.Instantiate<Enemy>();
		e.Initilalize(pos, id, deg, speed);
		Common.Instance.AddLayerChild("enemy", e);        
    }


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
	private float _destroyTimer;
	private int _step;
	private int _cnt;
	private float _wait;
	private int _id;
	private int _hp;
	private float _decayVelocity = 0.95f;
	public Vector2 velocity = Vector2.Zero;
	public override void _Ready()
	{
	}

	public void Initilalize(Vector2 pos, int id, float deg, float speed)
    {
        Position = pos;
		_id = id;
		velocity = Common.AngleToVector2(deg, speed);

		var size_tbl = new float[] {0, 1,  1,  0.3f, 1,  1,  1}; // サイズ.
		var hp_tbl   = new int[]   {0, 8,  10, 5,    10, 10, 10}; // HP.
		var dst_tbl  = new int[]   {0, 5,  10, 8,    10, 9,  10}; // 自爆タイマー.

        _hp = hp_tbl[_id];
		var size = size_tbl[_id];
		Scale = new Vector2(size, size);
		_destroyTimer = dst_tbl[_id];
	}

	private void Wait(float t)
    {
        _wait = t;
    }

	/// <summary>
    /// 固定時間で更新.
    /// </summary>
    /// <param name="delta"></param>
	public override void _PhysicsProcess(double delta)
    {
		velocity *= _decayVelocity;
		Position += velocity * (float)delta;

		_timer += (float)delta;
		_wait -= (float)delta;
		if(_wait <= 0)
        {            
			switch(_id)
			{
			case 1:
				_ai_01();
				break;
			case 2:
				_ai_02();
				break;
			case 3:
				_ai_03();
				break;
			case 4:
				_ai_04();
				break;
			case 5:
				_ai_05();
				break;
			case 6:
				_ai_06();
				break;
			}
        }

		// 発射リストを更新する.
		_UpdateBatteries((float)delta);

		if(Common.Instance.IsInScreen(Position, 16) == false)
        {
			// 画面外.
            QueueFree();
        }
		else if(_timer >= _destroyTimer)
        {
            // 自爆.
			QueueFree();
        }	
    }

	private void _ai_01()
    {
		// 高速狙い撃ち弾を撃つ
		switch(_step)
        {
		case 0:
			Wait(1);
			_step++;
			break;
		case 1:
			for (int i = 0; i < 5; i++)
			{
				_Aim(700, i * 0.05f);
			}
			Wait(0.8f);
			break;
        }
    }
	private void _ai_02()
    {
		// 両サイドから攻撃する
		switch (_step)
        {
		case 0:
			Wait(1);
			_step++;
			break;
		case 1:
			_decayVelocity = 1.001f; // 速度減衰無効
			velocity = new Vector2(0, 100);
			_step++;
			break;
		case 2:
			var dir = 0-20;
			if (Position.X > Common.Instance.ScreenSize.X/2)
            {
				dir = 180+20;
            }
			for (int i = 0; i < 16; i++)
			{
				_Bullet(dir, 500, i * 0.03f);
			}
			Wait(1);
			break;
		}
    }
	private void _ai_03()
    {
		// 狙い撃ち弾
		switch (_step)
        {
		case 0:
			Wait(2);
			_step++;
			break;
		case 1:
			for (int i = 0; i < 5; i++)
			{
				_Aim(300);
			}
			Wait(1.5f);
			break;
        }
    }
	private void _ai_04()
    {
		// ワインダー
		switch (_step)
        {
		case 0:
			Wait(2);
			_step++;
			break;
		case 1:
			_cnt = 0;
			_step++;
			break;
		case 2:
			var dir = 270 + 20 * Mathf.Sin(Common.Deg2Rad(_cnt * 4));
			_Bullet(dir, 500);
			_cnt++;
			Wait(0.05f);
			break;
		}
    }
	private	void _ai_05()
    {
        // ウィップ弾.
		switch (_step)
		{
		case 0:
			Wait(2);
			_step++;
			break;
		case 1:
			_cnt = 3;
			_step++;
			break;
		case 2:
			var dir = _GetAim();
			for (int i = 0; i < _cnt + 2; i++)
			{
				_NWay(_cnt, dir, 60, 300 + (50 * i), 0.02f * i * _cnt);
			}
			Wait(2);
			_cnt++;
			break;
        }
    }
	private void _ai_06()
    {
		// 回転弾
		switch (_step)
		{
		case 0:
			Wait(2);
			_step++;
			break;
		case 1:
			_cnt = 0; // 横から開始
			_step++;
			break;
		case 2:
			Wait(0.05f);
			_step++;
			break;
		case 3:
			var d = 8; // 回転速度
			_Bullet(_cnt, 200);
			_Bullet(_cnt+180, 200);
			if (Position.X < Common.Instance.ScreenSize.X/2)
            {
				_cnt += d;
            }
			else
            {
				_cnt -= d;
            }
			_step = 2;
			break;
		}   
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

	private void _Aim(float speed, float delay=0, float ax=0, float ay=0)
    {
        _Bullet(_GetAim(), speed, delay, ax, ay);
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

	private void OnAreaEntered(Area2D target)
	{
		var obj = target as Shot;
		if(obj != null)
        {
			var deg = Common.ToAngle(obj.velocity);
			deg -= 180 + Common.RandFRange(-30, 30);
			var speed = Common.ToSpeed(obj.velocity);
			speed *= Common.RandFRange(0.2f, 0.5f);
			Particle.Add(obj.Position, deg, speed, new Godot.Color(0.5f, 0.5f, 1, 1));
			// ショットのみ衝突処理.
            target.QueueFree();
        }
	}

}
