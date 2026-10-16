using System.Collections.Generic;
using Game.Autoload;
using Game.Resources.Level;
using Game.UI.Tutorial;
using Godot;

namespace Game.UI;

public partial class LevelSelectScreen : MarginContainer
{
	private const int LevelsPerPage = 8;

	[Signal]
	public delegate void BackPressedEventHandler();

	[Export]
	private PackedScene levelSelectSectionScene;

	private GridContainer gridContainer;
	private Button backButton;
	private Button previousPageButton;
	private Button nextPageButton;
	private Label pageLabel;
	private LevelDefinitionResource[] levelDefinitions;
	private readonly List<LevelSelectSection> levelSections = new();
	private int currentPage;
	private ConfirmationDialog tutorialChoiceDialog;
	private Button regularModeButton;
	private int pendingLevelIndex = -1;

	public override void _Ready()
	{
		gridContainer = GetNode<GridContainer>("%GridContainer");
		backButton = GetNode<Button>("BackButton");
		previousPageButton = GetNode<Button>("%PreviousPageButton");
		nextPageButton = GetNode<Button>("%NextPageButton");
		pageLabel = GetNode<Label>("%PageLabel");
		AudioHelpers.RegisterButtons(new Button[] { backButton, previousPageButton, nextPageButton });

		CreateTutorialChoiceDialog();
		levelDefinitions = LevelManager.GetLevelDefinitions();
		for (var i = 0; i < levelDefinitions.Length; i++)
		{
			var levelDefinition = levelDefinitions[i];
			var levelSelectSection = levelSelectSectionScene.Instantiate<LevelSelectSection>();
			gridContainer.AddChild(levelSelectSection);
			levelSections.Add(levelSelectSection);

			levelSelectSection.SetLevelDefinition(levelDefinition);
			levelSelectSection.SetLevelIndex(i);
			levelSelectSection.SetTutorialAvailability(
				TutorialCatalog.HasTutorial(levelDefinition.Id),
				SaveManager.HasTutorialStarted(levelDefinition.Id),
				levelDefinition.IsPresentation);
			levelSelectSection.LevelSelected += OnLevelSelected;
		}

		backButton.Pressed += OnBackButtonPressed;
		previousPageButton.Pressed += OnPreviousPagePressed;
		nextPageButton.Pressed += OnNextPagePressed;
		GetViewport().SizeChanged += UpdateGridColumns;
		UpdateGridColumns();
		UpdatePage();
	}

	private void OnLevelSelected(int levelIndex)
	{
		if (levelIndex < 0 || levelIndex >= levelDefinitions.Length)
		{
			return;
		}

		LevelDefinitionResource levelDefinition = levelDefinitions[levelIndex];
		if (!TutorialCatalog.HasTutorial(levelDefinition.Id))
		{
			LevelManager.ChangeToLevel(levelIndex, tutorialMode: false);
			return;
		}
		if (levelDefinition.IsPresentation)
		{
			StartTutorial(levelIndex);
			return;
		}

		if (!SaveManager.HasTutorialStarted(levelDefinition.Id))
		{
			StartTutorial(levelIndex);
			return;
		}

		pendingLevelIndex = levelIndex;
		tutorialChoiceDialog.Title = $"Level {levelIndex + 1} Tutorial";
		tutorialChoiceDialog.DialogText =
			"You have already started this tutorial. Replay it, or enter the level without tutorial guidance?";
		SetPopupCursorOverride(true);
		tutorialChoiceDialog.PopupCentered();
	}

	private void OnPreviousPagePressed()
	{
		currentPage--;
		UpdatePage();
	}

	private void OnNextPagePressed()
	{
		currentPage++;
		UpdatePage();
	}

	private void UpdatePage()
	{
		int pageCount = Mathf.Max(1, Mathf.CeilToInt(levelSections.Count / (float)LevelsPerPage));
		currentPage = Mathf.Clamp(currentPage, 0, pageCount - 1);

		for (int i = 0; i < levelSections.Count; i++)
		{
			levelSections[i].Visible = i / LevelsPerPage == currentPage;
		}

		bool showsPresentation = false;
		int firstVisibleIndex = currentPage * LevelsPerPage;
		int lastVisibleIndex = Mathf.Min(firstVisibleIndex + LevelsPerPage, levelDefinitions.Length);
		for (int i = firstVisibleIndex; i < lastVisibleIndex; i++)
		{
			showsPresentation |= levelDefinitions[i].IsPresentation;
		}

		pageLabel.Text = showsPresentation
			? $"PAGE {currentPage + 1} / {pageCount} - SPECIAL"
			: $"PAGE {currentPage + 1} / {pageCount} - LEVELS 1-8";
		previousPageButton.Disabled = currentPage == 0;
		nextPageButton.Disabled = currentPage == pageCount - 1;
	}

	private void CreateTutorialChoiceDialog()
	{
		tutorialChoiceDialog = new ConfirmationDialog
		{
			Name = "TutorialChoiceDialog",
			Title = "Tutorial Mode",
			DialogText = "Choose how to start this level.",
			OkButtonText = "PLAY TUTORIAL",
			Exclusive = true,
		};
		AddChild(tutorialChoiceDialog);
		tutorialChoiceDialog.GetCancelButton().Text = "CANCEL";
		regularModeButton = tutorialChoiceDialog.AddButton("PLAY WITHOUT TUTORIAL");
		OrderTutorialChoiceButtons();
		AudioHelpers.RegisterButtons(new Button[]
		{
			tutorialChoiceDialog.GetOkButton(),
			tutorialChoiceDialog.GetCancelButton(),
			regularModeButton,
		});

		tutorialChoiceDialog.Confirmed += OnTutorialReplayConfirmed;
		tutorialChoiceDialog.Canceled += OnTutorialChoiceCanceled;
		regularModeButton.Pressed += OnRegularModePressed;
	}

	private void OrderTutorialChoiceButtons()
	{
		Button tutorialButton = tutorialChoiceDialog.GetOkButton();
		Button cancelButton = tutorialChoiceDialog.GetCancelButton();
		HBoxContainer buttonRow = tutorialButton.GetParent<HBoxContainer>();
		if (buttonRow == null)
		{
			return;
		}

		// Keep platform-independent ordering: Tutorial, Without Tutorial, Cancel (rightmost).
		buttonRow.MoveChild(tutorialButton, buttonRow.GetChildCount() - 1);
		buttonRow.MoveChild(regularModeButton, buttonRow.GetChildCount() - 1);
		buttonRow.MoveChild(cancelButton, buttonRow.GetChildCount() - 1);
	}

	private void UpdateGridColumns()
	{
		if (gridContainer == null || GetViewport() == null)
		{
			return;
		}

		float viewportWidth = GetViewport().GetVisibleRect().Size.X;
		gridContainer.Columns = viewportWidth switch
		{
			>= 1500f => 4,
			>= 1050f => 3,
			>= 720f => 2,
			_ => 1,
		};
	}

	private void OnTutorialReplayConfirmed()
	{
		int levelIndex = ConsumePendingLevelIndex();
		SetPopupCursorOverride(false);
		if (levelIndex >= 0)
		{
			StartTutorial(levelIndex);
		}
	}

	private void OnRegularModePressed()
	{
		int levelIndex = ConsumePendingLevelIndex();
		tutorialChoiceDialog.Hide();
		SetPopupCursorOverride(false);
		if (levelIndex >= 0)
		{
			LevelManager.ChangeToLevel(levelIndex, tutorialMode: false);
		}
	}

	private void OnTutorialChoiceCanceled()
	{
		pendingLevelIndex = -1;
		SetPopupCursorOverride(false);
	}

	private void StartTutorial(int levelIndex)
	{
		if (levelIndex < 0 || levelIndex >= levelDefinitions.Length)
		{
			return;
		}

		SaveManager.MarkTutorialStarted(levelDefinitions[levelIndex].Id);
		LevelManager.ChangeToLevel(levelIndex, tutorialMode: true);
	}

	private int ConsumePendingLevelIndex()
	{
		int levelIndex = pendingLevelIndex;
		pendingLevelIndex = -1;
		return levelIndex;
	}

	private void SetPopupCursorOverride(bool enabled)
	{
		GetNodeOrNull<Game.Autoload.Cursor>("/root/Cursor")?.SetPopupCursorOverride(enabled);
	}

	private void OnBackButtonPressed()
	{
		EmitSignal(SignalName.BackPressed);
	}

	public override void _ExitTree()
	{
		SetPopupCursorOverride(false);
		if (GetViewport() != null)
		{
			GetViewport().SizeChanged -= UpdateGridColumns;
		}
		if (tutorialChoiceDialog != null)
		{
			tutorialChoiceDialog.Confirmed -= OnTutorialReplayConfirmed;
			tutorialChoiceDialog.Canceled -= OnTutorialChoiceCanceled;
		}
		if (regularModeButton != null)
		{
			regularModeButton.Pressed -= OnRegularModePressed;
		}
		if (previousPageButton != null)
		{
			previousPageButton.Pressed -= OnPreviousPagePressed;
		}
		if (nextPageButton != null)
		{
			nextPageButton.Pressed -= OnNextPagePressed;
		}
	}
}
