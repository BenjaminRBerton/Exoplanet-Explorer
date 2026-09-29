using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.UI.Tutorial;

public abstract class TutorialScript
{
	public virtual bool UsesPresentationLayout => false;

	public abstract void Build(TutorialBuilder tutorial);

	public IReadOnlyList<TutorialStep> CreateSteps()
	{
		TutorialBuilder builder = new();
		Build(builder);
		return builder.Build();
	}
}

public sealed class TutorialBuilder
{
	private readonly List<TutorialStepBuilder> stepBuilders = new();

	public TutorialStepBuilder Step(string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			throw new ArgumentException("A tutorial step requires a non-empty ID.", nameof(id));
		}
		if (stepBuilders.Any(step => step.Id == id))
		{
			throw new InvalidOperationException($"Tutorial step ID '{id}' is duplicated.");
		}

		string previousId = stepBuilders.Count > 0 ? stepBuilders[^1].Id : null;
		TutorialStepBuilder stepBuilder = new(id, previousId);
		stepBuilders.Add(stepBuilder);
		return stepBuilder;
	}

	public IReadOnlyList<TutorialStep> Build()
	{
		if (stepBuilders.Count == 0)
		{
			throw new InvalidOperationException("A tutorial script must define at least one step.");
		}

		List<TutorialStep> steps = stepBuilders.Select(step => step.Build()).ToList();
		HashSet<string> earlierIds = new();
		foreach (TutorialStep step in steps)
		{
			if (step.Trigger.Kind == TutorialTriggerKind.AfterStep &&
				!earlierIds.Contains(step.Trigger.PreviousStepId))
			{
				throw new InvalidOperationException(
					$"Tutorial step '{step.Id}' must follow a step defined earlier, but " +
					$"'{step.Trigger.PreviousStepId}' is not earlier in the script.");
			}
			if (step.Completion.Kind == TutorialCompletionKind.TargetPressed &&
				string.IsNullOrWhiteSpace(step.TargetId))
			{
				throw new InvalidOperationException(
					$"Tutorial step '{step.Id}' waits for a target press but has no target ID.");
			}
			earlierIds.Add(step.Id);
		}

		return steps.AsReadOnly();
	}
}

public sealed class TutorialStepBuilder
{
	internal string Id { get; }

	private readonly string defaultPreviousStepId;
	private string title = "TUTORIAL";
	private string text = string.Empty;
	private string targetId;
	private TutorialOverlayMode mode = TutorialOverlayMode.HardPause;
	private TutorialTrigger trigger;
	private TutorialCompletion completion;
	private double missingTargetTimeoutSeconds = 1.0d;
	private bool skippable = true;
	private bool dimBackground = true;
	private TutorialCalloutPlacement calloutPlacement = TutorialCalloutPlacement.Auto;
	private readonly List<string> imagePaths = new();
	private TutorialImagePlacement imagePlacement = TutorialImagePlacement.Bottom;
	private int imageGap = 16;
	private float imageWidthPercent = 40f;
	private float bodyFontScale = 1f;
	private bool bodyBold;
	private string footnote = string.Empty;

	internal TutorialStepBuilder(string id, string defaultPreviousStepId)
	{
		Id = id;
		this.defaultPreviousStepId = defaultPreviousStepId;
	}

	public TutorialStepBuilder When(
		TutorialEvent tutorialEvent,
		Func<TutorialEventContext, bool> predicate = null)
	{
		trigger = TutorialTrigger.OnEvent(tutorialEvent, predicate);
		return this;
	}

	public TutorialStepBuilder After(string stepId)
	{
		trigger = TutorialTrigger.After(stepId);
		return this;
	}

	public TutorialStepBuilder Immediately()
	{
		trigger = TutorialTrigger.Immediate();
		return this;
	}

	public TutorialStepBuilder Say(string message)
	{
		text = message ?? string.Empty;
		return this;
	}

	public TutorialStepBuilder Say(string heading, string message)
	{
		title = heading ?? "TUTORIAL";
		text = message ?? string.Empty;
		return this;
	}

	public TutorialStepBuilder PointTo(string tutorialTargetId)
	{
		targetId = tutorialTargetId;
		return this;
	}

	public TutorialStepBuilder HardPause()
	{
		mode = TutorialOverlayMode.HardPause;
		return this;
	}

	public TutorialStepBuilder GuideAction()
	{
		mode = TutorialOverlayMode.GuidedAction;
		return this;
	}

	public TutorialStepBuilder UndimBackground()
	{
		dimBackground = false;
		return this;
	}

	public TutorialStepBuilder PlaceCallout(TutorialCalloutPlacement placement)
	{
		calloutPlacement = placement;
		return this;
	}

	/// <summary>Adds one project image at the requested position relative to the slide text.</summary>
	public TutorialStepBuilder WithImage(
		string resourcePath,
		TutorialImagePlacement placement = TutorialImagePlacement.Bottom,
		int gap = 16,
		float widthPercent = 40f)
	{
		imagePaths.Clear();
		AddImagePath(resourcePath);
		imagePlacement = placement;
		imageGap = Math.Max(0, gap);
		imageWidthPercent = Math.Clamp(widthPercent, 5f, 100f);
		return this;
	}

	/// <summary>Adds a centered row of project images below the slide text.</summary>
	public TutorialStepBuilder WithImages(params string[] resourcePaths)
	{
		return WithImages(resourcePaths, TutorialImagePlacement.Bottom, 16, 40f);
	}

	/// <summary>Adds project images using fluent placement-first arguments.</summary>
	public TutorialStepBuilder WithImages(
		TutorialImagePlacement placement,
		int gap,
		params string[] resourcePaths)
	{
		return WithImages(resourcePaths, placement, gap, 40f);
	}

	/// <summary>Adds project images with explicit placement, spacing, and gallery width.</summary>
	public TutorialStepBuilder WithImages(
		TutorialImagePlacement placement,
		int gap,
		float widthPercent,
		params string[] resourcePaths)
	{
		return WithImages(resourcePaths, placement, gap, widthPercent);
	}

	/// <summary>Adds project images at the requested position and spacing.</summary>
	public TutorialStepBuilder WithImages(
		IEnumerable<string> resourcePaths,
		TutorialImagePlacement placement = TutorialImagePlacement.Bottom,
		int gap = 16,
		float widthPercent = 40f)
	{
		imagePaths.Clear();
		if (resourcePaths != null)
		{
			foreach (string resourcePath in resourcePaths)
			{
				AddImagePath(resourcePath);
			}
		}
		imagePlacement = placement;
		imageGap = Math.Max(0, gap);
		imageWidthPercent = Math.Clamp(widthPercent, 5f, 100f);
		return this;
	}

	private void AddImagePath(string resourcePath)
	{
		if (!string.IsNullOrWhiteSpace(resourcePath)) imagePaths.Add(resourcePath);
	}

	/// <summary>Emphasizes this step's message without changing the presentation heading.</summary>
	public TutorialStepBuilder EmphasizeBody(float fontScale = 1.35f, bool bold = true)
	{
		bodyFontScale = Math.Clamp(fontScale, 0.75f, 2f);
		bodyBold = bold;
		return this;
	}

	/// <summary>Adds smaller supporting text at the bottom of the slide.</summary>
	public TutorialStepBuilder WithFootnote(string text)
	{
		footnote = text ?? string.Empty;
		return this;
	}

	public TutorialStepBuilder UntilContinue()
	{
		completion = TutorialCompletion.Continue();
		return this;
	}

	public TutorialStepBuilder UntilTargetPressed()
	{
		completion = TutorialCompletion.TargetPressed();
		return this;
	}

	public TutorialStepBuilder Until(
		TutorialEvent tutorialEvent,
		Func<TutorialEventContext, bool> predicate = null)
	{
		completion = TutorialCompletion.OnEvent(tutorialEvent, predicate);
		return this;
	}

	public TutorialStepBuilder UntilState(Func<bool> predicate)
	{
		completion = TutorialCompletion.State(predicate);
		return this;
	}

	/// <summary>Keeps CONTINUE available as an alternative to the configured action condition.</summary>
	public TutorialStepBuilder OrContinue()
	{
		completion = (completion ?? TutorialCompletion.Continue()).WithContinueAlternative();
		return this;
	}

	public TutorialStepBuilder FallbackAfter(double seconds)
	{
		missingTargetTimeoutSeconds = Math.Max(0d, seconds);
		return this;
	}

	public TutorialStepBuilder CannotSkip()
	{
		skippable = false;
		return this;
	}

	internal TutorialStep Build()
	{
		TutorialTrigger resolvedTrigger = trigger ?? (defaultPreviousStepId == null
			? TutorialTrigger.Immediate()
			: TutorialTrigger.After(defaultPreviousStepId));
		TutorialCompletion resolvedCompletion = completion ?? TutorialCompletion.Continue();
		return new TutorialStep(
			Id,
			title,
			text,
			targetId,
			mode,
			resolvedTrigger,
			resolvedCompletion,
			missingTargetTimeoutSeconds,
			skippable,
			dimBackground,
			calloutPlacement,
			imagePaths.AsReadOnly(),
			imagePlacement,
			imageGap,
			imageWidthPercent,
			bodyFontScale,
			bodyBold,
			footnote);
	}
}
