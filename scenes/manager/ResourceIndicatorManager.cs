using Game.UI;
using Godot;

namespace Game.Manager;

public partial class ResourceIndicatorManager : Node
{

	[Export]
	private GridManager gridManager;
	[Export]
	private PackedScene resourceIndicatorScene;
	[Export]
	private PackedScene mineralIndicatorScene;

	private AudioStreamPlayer audioStreamPlayer;


	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;
		gridManager.MineralTilesUpdated += OnResourceTilesUpdated;
	}

	private void ShowIndicator(Vector2I tile, string resourceType)
	{
		Node2D indicator;
		switch (resourceType)
		{
			case "red_ore":
			case "green_ore":
			case "blue_ore":
				var mineralIndicator = mineralIndicatorScene.Instantiate<MineralIndicator>();
				mineralIndicator.SetOreType(resourceType);
				indicator = mineralIndicator;
				break;
			case "wood":
				indicator = resourceIndicatorScene.Instantiate<Resourceindicator>();
				break;
			default:
				return;
		}

		AddChild(indicator);
		indicator.GlobalPosition = tile * 64;
		audioStreamPlayer.Play();
	}

	private void OnResourceTilesUpdated(Vector2I tile, int _, string resourceType)
	{
		Callable.From(() => ShowIndicator(tile, resourceType)).CallDeferred();
	}

}
