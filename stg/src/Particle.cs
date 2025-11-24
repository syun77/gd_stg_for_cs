using Godot;
using System;

/// <summary>
/// パーティクル.
/// </summary>
public partial class Particle : Sprite2D
{
	private static readonly PackedScene PARTICLE_SCENE = GD.Load<PackedScene>("res://src/Particle.tscn");

	public static Particle Add(Vector2 pos, float deg, float speed, Godot.Color c)
    {
        var p = PARTICLE_SCENE.Instantiate<Particle>();
		p.Position = pos;
		p.SetVelocity(deg, speed);
		p.Modulate = c;
		Common.Instance.AddLayerChild("particle", p);
		return p;
    }

	enum eState
    {
		Main,
        Blink,
    }
	private eState _state = eState.Main;
	public float lifetime = 1f;
	private float _timer = 0f;
	private int _cnt = 0;
	public Vector2 velocity = Vector2.Zero;
	public void SetVelocity(float deg, float speed)
    {
        velocity = Common.AngleToVector2(deg, speed);
    }
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
    {
		velocity *= 0.97f;
		Position += velocity * (float)delta;

        switch(_state)
        {
        case eState.Main:
			lifetime -= (float)delta;
			if(lifetime <= 0)
            {
				_state = eState.Blink;
            }
			break;
		case eState.Blink:
			_cnt++;
			Visible = true;
			if(_cnt%4 < 2)
            {
                Visible = false;
            }
			_timer += (float)delta;
			if(_timer > 1f)
            {
                    QueueFree();
            }
			break;
        }
    }
}
