using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;

/// <summary>
/// どこからでもアクセスできる共通クラス
/// AutoLoadとして設定することでシングルトンとして使用可能
/// </summary>
public partial class Common : Node
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static Common Instance { get; private set; }

    /// <summary>
    /// 画面サイズ
    /// </summary>
    public Vector2 ScreenSize { get; private set; }

    /// <summary>
    /// スコア
    /// </summary>
    public int Score { get; set; } = 0;

    /// <summary>
    /// プレイヤーライフ
    /// </summary>
    public int Lives { get; set; } = 3;

    private Player _target;

    /// <summary>
    /// CanvasLayerの管理用ディクショナリ
    /// </summary>
    private Dictionary<string, CanvasLayer> _canvasLayers = new Dictionary<string, CanvasLayer>();

    public override void _Ready()
    {
        // シングルトンインスタンスを設定
        Instance = this;
        
        // 画面サイズを取得
        ScreenSize = GetViewport().GetVisibleRect().Size;
        
        _canvasLayers = new Dictionary<string, CanvasLayer>();
    }

    public void SetTarget(Player target)
    {
        _target = target;
    }
    public float GetAim(Vector2 pos)
    {
        var v = _target.Position - pos;
        var d = Math.Atan2(-v.Y, v.X);
        // ラジアンを角度に変換する.
        return (float)(d * 180.0 / Math.PI);
    }

    public void AddLayer(string name, CanvasLayer layer)
    {
        _canvasLayers.Add(name, layer);
    }
    public CanvasLayer GetLayer(string name)
    {
        return _canvasLayers[name];
    }
    public void AddLayerChild(string name, Node2D obj)
    {
        var layer = GetLayer(name);
        layer.AddChild(obj);
    }
    public int GetLayerChildCount(string name)
    {
        var layer = GetLayer(name);
        return layer.GetChildCount();
    }

    /// <summary>
    /// スコアを加算
    /// </summary>
    /// <param name="points">加算するポイント</param>
    public void AddScore(int points)
    {
        Score += points;
        GD.Print($"Score: {Score}");
    }

    /// <summary>
    /// ライフを減らす
    /// </summary>
    public void LoseLife()
    {
        Lives--;
        GD.Print($"Lives: {Lives}");
        
        if (Lives <= 0)
        {
            GD.Print("Game Over!");
        }
    }

    /// <summary>
    /// ゲームをリセット
    /// </summary>
    public void ResetGame()
    {
        Score = 0;
        Lives = 3;
        GD.Print("Game Reset");
    }

    /// <summary>
    /// 画面内に座標が収まっているかチェック
    /// </summary>
    /// <param name="position">チェックする座標</param>
    /// <param name="margin">マージン</param>
    /// <returns>画面内にある場合true</returns>
    public bool IsInScreen(Vector2 position, float margin = 0f)
    {
        return position.X >= -margin && position.X <= ScreenSize.X + margin &&
               position.Y >= -margin && position.Y <= ScreenSize.Y + margin;
    }

    /// <summary>
    /// 座標を画面内に制限
    /// </summary>
    /// <param name="position">制限する座標</param>
    /// <param name="size">オブジェクトのサイズ</param>
    /// <returns>制限された座標</returns>
    public Vector2 ClampToScreen(Vector2 position, Vector2 size)
    {
        var halfSize = size / 2f;
        return new Vector2(
            Mathf.Clamp(position.X, halfSize.X, ScreenSize.X - halfSize.X),
            Mathf.Clamp(position.Y, halfSize.Y, ScreenSize.Y - halfSize.Y)
        );
    }

    /// <summary>
    /// ランダムな画面座標を取得
    /// </summary>
    /// <param name="margin">画面端からのマージン</param>
    /// <returns>ランダムな座標</returns>
    public Vector2 GetRandomScreenPosition(float margin = 50f)
    {
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        
        return new Vector2(
            rng.RandfRange(margin, ScreenSize.X - margin),
            rng.RandfRange(margin, ScreenSize.Y - margin)
        );
    }

    /// <summary>
    /// CanvasLayerを登録
    /// </summary>
    /// <param name="name">CanvasLayerの識別名</param>
    /// <param name="canvasLayer">CanvasLayerのインスタンス</param>
    public void RegisterCanvasLayer(string name, CanvasLayer canvasLayer)
    {
        if (_canvasLayers.ContainsKey(name))
        {
            GD.PrintErr($"CanvasLayer '{name}' is already registered. Replacing existing layer.");
        }
        
        _canvasLayers[name] = canvasLayer;
        GD.Print($"CanvasLayer '{name}' registered");
    }

    /// <summary>
    /// CanvasLayerを取得
    /// </summary>
    /// <param name="name">CanvasLayerの識別名</param>
    /// <returns>CanvasLayerのインスタンス、見つからない場合はnull</returns>
    public CanvasLayer GetCanvasLayer(string name)
    {
        if (_canvasLayers.TryGetValue(name, out CanvasLayer canvasLayer))
        {
            return canvasLayer;
        }
        
        GD.PrintErr($"CanvasLayer '{name}' not found");
        return null;
    }

    /// <summary>
    /// CanvasLayerの登録を削除
    /// </summary>
    /// <param name="name">CanvasLayerの識別名</param>
    /// <returns>削除に成功した場合true</returns>
    public bool UnregisterCanvasLayer(string name)
    {
        bool removed = _canvasLayers.Remove(name);
        if (removed)
        {
            GD.Print($"CanvasLayer '{name}' unregistered");
        }
        else
        {
            GD.PrintErr($"CanvasLayer '{name}' not found for removal");
        }
        return removed;
    }

    /// <summary>
    /// すべてのCanvasLayerを削除
    /// </summary>
    public void ClearAllCanvasLayers()
    {
        _canvasLayers.Clear();
        GD.Print("All CanvasLayers cleared");
    }

    /// <summary>
    /// 登録されているCanvasLayerの数を取得
    /// </summary>
    /// <returns>登録されているCanvasLayerの数</returns>
    public int GetCanvasLayerCount()
    {
        return _canvasLayers.Count;
    }

    /// <summary>
    /// 登録されているCanvasLayerの名前一覧を取得
    /// </summary>
    /// <returns>CanvasLayer名の配列</returns>
    public string[] GetCanvasLayerNames()
    {
        var names = new string[_canvasLayers.Count];
        _canvasLayers.Keys.CopyTo(names, 0);
        return names;
    }

    /// <summary>
    /// 指定されたCanvasLayerが登録されているかチェック
    /// </summary>
    /// <param name="name">CanvasLayerの識別名</param>
    /// <returns>登録されている場合true</returns>
    public bool HasCanvasLayer(string name)
    {
        return _canvasLayers.ContainsKey(name);
    }

    /// <summary>
    /// デバッグ情報を表示
    /// </summary>
    public void PrintDebugInfo()
    {
        GD.Print($"=== Debug Info ===");
        GD.Print($"Score: {Score}");
        GD.Print($"Lives: {Lives}");
        GD.Print($"Screen Size: {ScreenSize}");
        GD.Print($"Registered CanvasLayers: {_canvasLayers.Count}");
        foreach (var kvp in _canvasLayers)
        {
            GD.Print($"  - {kvp.Key}: {kvp.Value.Name} (Layer: {kvp.Value.Layer})");
        }
        GD.Print($"================");
    }

    public static Vector2 AngleToVector2(float degree, float speed)
    {
        // 角度をラジアンに変換.
        var rad = degree * Math.PI / 180;
        return new Vector2(
            (float)Math.Cos(rad) * speed, 
            (float)Math.Sin(rad) * -speed
        );
    }

    // 範囲指定で乱数を返す (整数値).
    public static int RandInt(int aMin, int bMax)
    {
        return Random.Shared.Next(aMin, bMax);
    }
    // 範囲指定で乱数を返す (浮動小数点数).
    public static float RanfFloat(float aMin, float bMax)
    {
        return (float)(Random.Shared.NextDouble() * (bMax - aMin) + aMin);
    }
}