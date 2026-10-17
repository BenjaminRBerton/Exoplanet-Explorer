using Game.Autoload;
using Godot;

namespace Game.UI;

public partial class CreditsMenu : CanvasLayer
{
	private const string VideoUrl = "https://youtu.be/nd30w3JyyPI?si=pzO4lo-kPKgDkCjh";
	private const string TeachingMaterialUrl = "https://benjaminrberton.github.io/";

	[Signal]
	public delegate void DonePressedEventHandler();

	public override void _Ready()
	{
		Button videoButton = GetNode<Button>("%VideoButton");
		Button teachingMaterialButton = GetNode<Button>("%TeachingMaterialButton");
		Button doneButton = GetNode<Button>("%DoneButton");

		AudioHelpers.RegisterButtons(new[] { videoButton, teachingMaterialButton, doneButton });
		videoButton.Pressed += () => OS.ShellOpen(VideoUrl);
		teachingMaterialButton.Pressed += () => OS.ShellOpen(TeachingMaterialUrl);
		doneButton.Pressed += () => EmitSignal(SignalName.DonePressed);
	}
}
