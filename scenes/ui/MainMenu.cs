using Game.Autoload;
using Godot;

namespace Game.UI;

public partial class MainMenu : Node
{
	[Export]
	private PackedScene optionsMenuScene;

	[Export]
	private PackedScene rulesMenuScene;
	[Export]
	private PackedScene controlsMenuScene;
	[Export]
	private PackedScene creditsMenuScene;

	private Button playButton;
	private Control mainMenuContainer;
	private LevelSelectScreen levelSelectScreen;
	private Button quitButton;
	private Button optionsButton;
	private Button rulesButton;
	private Button controlsButton;
	private Button creditsButton;

	private Button watchIntroButton;

	public override void _Ready()
	{
		playButton = GetNode<Button>("%PlayButton");
		quitButton = GetNode<Button>("%QuitButton");
		optionsButton = GetNode<Button>("%OptionsButton");
		rulesButton = GetNode<Button>("%RulesButton");
		watchIntroButton = GetNode<Button>("%IntroButton");
		controlsButton = GetNode<Button>("%ControlsButton");
		creditsButton = GetNode<Button>("%CreditsButton");

		AudioHelpers.RegisterButtons(new Button[] { playButton, quitButton, optionsButton, rulesButton, watchIntroButton, controlsButton, creditsButton });

		mainMenuContainer = GetNode<Control>("%MainMenuContainer");
		levelSelectScreen = GetNode<LevelSelectScreen>("%LevelSelectScreen");

		levelSelectScreen.Visible = false;
		mainMenuContainer.Visible = true;

		playButton.Pressed += OnPlayButtonPressed;
		quitButton.Pressed += OnQuitButtonPressed;
		levelSelectScreen.BackPressed += OnLevelSelectBackPressed;
		optionsButton.Pressed += OnOptionsButtonPressed;
		rulesButton.Pressed += OnRulesButtonPressed;
		watchIntroButton.Pressed += OnWatchIntroButtonPressed;
		controlsButton.Pressed += OnControlsButtonPressed;
		creditsButton.Pressed += OnCreditsButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		mainMenuContainer.Visible = false;
		levelSelectScreen.Visible = true;
	}

	private void OnLevelSelectBackPressed()
	{
		mainMenuContainer.Visible = true;
		levelSelectScreen.Visible = false;
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnWatchIntroButtonPressed()
	{
		LevelManager.ChangeToIntroCutScene();
	}

	private void OnOptionsButtonPressed()
	{
		mainMenuContainer.Visible = false;
		var optionsMenu = optionsMenuScene.Instantiate<OptionsMenu>();
		AddChild(optionsMenu);
		optionsMenu.DonePressed += () =>
		{
			OnOptionsDonePressed(optionsMenu);
		};
	}

	private void OnRulesButtonPressed()
	{
		mainMenuContainer.Visible = false;
		var rulesMenu = rulesMenuScene.Instantiate<RulesMenu>();
		AddChild(rulesMenu);
		rulesMenu.DonePressed += () =>
		{
			OnRulesDonePressed(rulesMenu);
		};
	}

	private void OnOptionsDonePressed(OptionsMenu optionsMenu)
	{
		optionsMenu.QueueFree();
		mainMenuContainer.Visible = true;
	}

	private void OnRulesDonePressed(RulesMenu rulesMenu)
	{
		rulesMenu.QueueFree();
		mainMenuContainer.Visible = true;
	}

	private void OnControlsButtonPressed()
	{
		mainMenuContainer.Visible = false;
		var controlsMenu = controlsMenuScene.Instantiate<ControlsMenu>();
		AddChild(controlsMenu);
		controlsMenu.DonePressed += () =>
		{
			OnControlsDonePressed(controlsMenu);
		};
	}

	private void OnControlsDonePressed(ControlsMenu controlsMenu)
	{
		controlsMenu.QueueFree();
		mainMenuContainer.Visible = true;
	}

	private void OnCreditsButtonPressed()
	{
		mainMenuContainer.Visible = false;
		var creditsMenu = creditsMenuScene.Instantiate<CreditsMenu>();
		AddChild(creditsMenu);
		creditsMenu.DonePressed += () =>
		{
			creditsMenu.QueueFree();
			mainMenuContainer.Visible = true;
		};
	}
}
