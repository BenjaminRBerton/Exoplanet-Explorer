using Godot;


namespace Game.UI;
public partial class MineralIndicator : Node2D
{

	private Sprite2D sprite2D;
	private Tween activeTween;
	[Export]
	private Texture2D redOreTexture;
	[Export]
	private Texture2D greenOreTexture;
	[Export]
	private Texture2D blueOreTexture;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		sprite2D = GetNode<Sprite2D>("%Sprite2D");

		var duration = GD.RandRange(.7, 1);

		activeTween = CreateTween();
		activeTween.SetLoops();
		activeTween.TweenProperty(sprite2D, "position", Vector2.Up * 4, duration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.InOut);
		activeTween.TweenProperty(sprite2D, "position", Vector2.Down * 4, duration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.InOut);

		var life = GD.RandRange(2, 3);
		await ToSignal(GetTree().CreateTimer(life), SceneTreeTimer.SignalName.Timeout);
		Destroy();
	}

	public void Destroy()
	{
		if (activeTween != null && activeTween.IsValid())
		{
			activeTween.Kill();
		}

		activeTween = CreateTween();
		activeTween.SetParallel();
		activeTween.TweenInterval(GD.RandRange(.1, .3));
		activeTween.Chain();
		activeTween.TweenProperty(sprite2D, "scale", Vector2.Zero, .3)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.In);
		activeTween.TweenProperty(sprite2D, "position", Vector2.Up * 32, .3)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);
		activeTween.Chain();
		activeTween.TweenCallback(Callable.From(() => QueueFree()));
	}

	public void SetOreType(string oreType)
	{
		if (sprite2D == null)
		{
			sprite2D = GetNode<Sprite2D>("%Sprite2D");
		}
		switch (oreType)
		{
			case "red_ore":
				sprite2D.Texture = redOreTexture;
				break;
			case "green_ore":
				sprite2D.Texture = greenOreTexture;
				break;
			case "blue_ore":
				sprite2D.Texture = blueOreTexture;
				break;
		}
	}
}
