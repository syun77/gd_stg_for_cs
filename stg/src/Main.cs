using Godot;
using System;

public partial class Main : Node2D
{
	/// <summary>
    /// 開始.
    /// </summary>
	public override void _Ready()
    {
		// 各種レイヤーを登録.
		Common.Instance.AddLayer("shot", GetNode<CanvasLayer>("ShotLayer"));
		Common.Instance.AddLayer("enemy", GetNode<CanvasLayer>("EnemyLayer"));
		Common.Instance.AddLayer("bullet", GetNode<CanvasLayer>("BulletLayer"));
    }

	public override void _Process(double delta)
	{
	}
}
