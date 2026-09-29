using System.Collections.Generic;
using Game.Autoload;
using Game.UI.Tutorial;
using Godot;

public partial class FragmentAnalysisUI
{
	private const ulong Level3TutorialManualSeed = 273060694UL;
	private const ulong Level3TutorialAutonomousSeed = 2504846179UL;
	private const ulong Level7PresentationManualSeed = 881840862UL;
	private const ulong Level7PresentationAutonomousSeed = 231429598UL;

	private readonly List<TutorialTargetRegistration> tutorialTargets = new();

	public void RegisterTutorialTargets(TutorialTargetRegistry registry)
	{
		ClearTutorialTargets();
		if (registry == null) return;

		Button manualButton = GetNodeOrNull<Button>("%InitialManualButton");
		Button supportButton = GetNodeOrNull<Button>("%InitialSupportButton");
		Button autonomousButton = GetNodeOrNull<Button>("%InitialAutonomousButton");
		Control processingToggles = GetNodeOrNull<Control>("%ProcessingToggles");
		Control processingSliders = GetNodeOrNull<Control>("%ProcessingSliders");
		// DirectionInset belongs to the instantiated FragmentAutonomyPanel scene, so a % lookup
		// from this outer CanvasLayer cannot resolve it. InitializeAutonomyNodes already stores
		// the live Arrow & Direction submenu control in directionInset.
		Control worldBearing = directionInset;

		if (IsInstanceValid(manualButton))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentManualButton,
				manualButton));
		if (IsInstanceValid(supportButton))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentSupportButton,
				supportButton));
		if (IsInstanceValid(reloadButton))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.ReloadFragmentButton,
				reloadButton));
		if (IsInstanceValid(quitButton))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentExitButton,
				quitButton));
		if (IsInstanceValid(autonomousButton))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentAutonomousButton,
				autonomousButton));
		if (IsInstanceValid(autonomyPerformerButton))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentAutonomyPerformerButton,
				autonomyPerformerButton));
		if (IsInstanceValid(autonomyAdvancedButton))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentTaskAllocationButton,
				autonomyAdvancedButton));
		if (IsInstanceValid(capabilityOverridesScroll))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentCapabilityOverrides,
				capabilityOverridesScroll));
		if (IsInstanceValid(fragmentCanvas))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentCanvas,
				fragmentCanvas));
		if (IsInstanceValid(worldBearing))
		{
			ScrollContainer bearingScroll = FindAncestorScrollContainer(worldBearing);
			tutorialTargets.Add(registry.RegisterRectProvider(
				TutorialTargetIds.FragmentWorldBearing,
				worldBearing,
				() =>
				{
					if (!IsInstanceValid(worldBearing) || !worldBearing.IsVisibleInTree())
						return null;
					bearingScroll?.EnsureControlVisible(worldBearing);
					return worldBearing.GetGlobalRect();
				},
				worldBearing));
		}
		if (IsInstanceValid(processingToggles) && IsInstanceValid(processingSliders))
			tutorialTargets.Add(registry.RegisterRectProvider(
				TutorialTargetIds.FragmentProcessingControls,
				this,
				() => GetCombinedVisibleRect(processingToggles, processingSliders)));
		if (IsInstanceValid(orientationRotationRow))
			tutorialTargets.Add(registry.RegisterControl(
				TutorialTargetIds.FragmentRotationControls,
				orientationRotationRow));
	}

	private void ClearTutorialTargets()
	{
		foreach (TutorialTargetRegistration registration in tutorialTargets)
			registration.Dispose();
		tutorialTargets.Clear();
	}

	private static Rect2? GetCombinedVisibleRect(Control first, Control second)
	{
		if (!IsInstanceValid(first) || !IsInstanceValid(second) ||
			!first.IsVisibleInTree() || !second.IsVisibleInTree())
			return null;
		return first.GetGlobalRect().Merge(second.GetGlobalRect());
	}

	private static ScrollContainer FindAncestorScrollContainer(Node node)
	{
		for (Node current = node?.GetParent(); current != null; current = current.GetParent())
			if (current is ScrollContainer scrollContainer) return scrollContainer;
		return null;
	}

	private void GenerateFragmentForAnalysisPass(
		string analysisPass,
		ulong? level3TutorialSeed,
		ulong? level7PresentationSeed)
	{
		bool isLevel3Tutorial = IsLevel3TutorialAnalysis();
		bool isLevel7Presentation = IsLevel7PresentationAnalysis();
		ulong? fixedTutorialSeed = isLevel3Tutorial
			? level3TutorialSeed
			: isLevel7Presentation ? level7PresentationSeed : null;
		bool useFixedTutorialSeed = fixedTutorialSeed.HasValue;
		if (useFixedTutorialSeed)
			fragmentCanvas.GenerateFragmentFromSeed(fixedTutorialSeed.Value);
		else
			fragmentCanvas.GenerateFragment();

		if ((!isLevel3Tutorial && !isLevel7Presentation) || fragmentCanvas?.Puzzle == null)
			return;

		string tutorialName = isLevel3Tutorial
			? "LEVEL 3 TUTORIAL"
			: "LEVEL 7 PRESENTATION";
		GD.Print(
			$"[{tutorialName} FRAGMENT — " +
			$"{(useFixedTutorialSeed ? "FIXED" : "RANDOM CANDIDATE")}] {analysisPass} | " +
			$"SEED={fragmentCanvas.Puzzle.Seed} | " +
			$"GLYPH={fragmentCanvas.Puzzle.GlyphType}");
	}

	private bool IsLevel3TutorialAnalysis() =>
		LevelManager.IsTutorialModeActive &&
		GetParent() is Game.BaseLevel level &&
		level.LevelId == TutorialCatalog.Level3Id;

	private bool IsLevel7PresentationAnalysis() =>
		LevelManager.IsTutorialModeActive &&
		GetParent() is Game.BaseLevel level &&
		level.LevelId == TutorialCatalog.Level7PresentationId;
}
