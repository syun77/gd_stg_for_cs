using Godot;
using System;

public partial class Boss : Area2D
{
	private int _cnt;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
    {
        _cnt++;
		switch(_cnt)
        {
        case 1 * 60:
            Spawn(1, 270-45, 300);
			Spawn(1, 270+45, 300);
			break;
		case 5 * 60:
            Spawn(2, 0, 600);
			Spawn(2, 180, 600);
			break;
		case 6 * 60:
			// 16方向に "3" を生成.
			for (int i = 0; i < 16; i++)
			{
				var dir = i * 360 / 16;
				Spawn(3, dir, 300);
			}
			break;
		case 9 * 60:
			// ワインダー.
			Spawn(4, 0,   500);
			Spawn(4, 180, 500);	
			break;
		case 13 * 60:
			Spawn(5, 45,  400);
			Spawn(5, 135, 400);
			break;
		case 14 * 60 + 30:
			Spawn(6, 0,   500);
			Spawn(6, 180, 500);
			break;
		case 19 * 60:
			_cnt = 0; // 最初に戻る.
			break;
        }
    }

	private void Spawn(int id, float deg, float speed)
    {
		Enemy.Add(Position, id, deg, speed);
    }

	public void OnAreaEntered(Area2D target)
    {
		if(target is Shot)
        {
			var obj = target as Shot;
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
