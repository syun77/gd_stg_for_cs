using Godot;
using System;

public partial class Main : Node2D
{
	private Label _text;

	/// <summary>
    /// 開始.
    /// </summary>
	public override void _Ready()
    {
		// ターゲットを登録.
		Common.Instance.SetTarget(GetNode<Player>("Player"));

		// 各種レイヤーを登録.
		Common.Instance.AddLayer("shot", GetNode<CanvasLayer>("ShotLayer"));
		Common.Instance.AddLayer("enemy", GetNode<CanvasLayer>("EnemyLayer"));
		Common.Instance.AddLayer("bullet", GetNode<CanvasLayer>("BulletLayer"));
		Common.Instance.AddLayer("particle", GetNode<CanvasLayer>("ParticleLayer"));

		_text = GetNode<Label>("CanvasUI/Label");
    }

	public override void _Process(double delta)
    {
		_text.Text = "Shot: " + Common.Instance.GetLayerChildCount("shot").ToString();
		_text.Text += "\nEnemy: " + Common.Instance.GetLayerChildCount("enemy").ToString();
		_text.Text += "\nBullet: " + Common.Instance.GetLayerChildCount("bullet").ToString();
		_text.Text += "\nParticle: " + Common.Instance.GetLayerChildCount("particle").ToString();
    }
}
