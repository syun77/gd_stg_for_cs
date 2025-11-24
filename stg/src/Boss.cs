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
		if(_cnt == 10)
        {
            Add(1, 270-45, 300);
			Add(1, 270+45, 300);
        }
    }

	private void Add(int id, float deg, float speed)
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
