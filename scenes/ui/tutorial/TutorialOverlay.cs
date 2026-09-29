using System.Collections.Generic;
using Game.Autoload;
using Godot;

namespace Game.UI.Tutorial;

public enum TutorialOverlayMode
{
	HardPause,
	GuidedAction,
}

/// <summary>
/// Presents tutorial copy while dimming everything except an optional screen-space target.
/// Pause ownership deliberately remains outside this view so a future TutorialDirector can
/// distinguish hard-paused explanations from guided actions that must reach gameplay code.
/// </summary>
public partial class TutorialOverlay : CanvasLayer
{
	[Signal]
	public delegate void ContinueRequestedEventHandler();

	[Signal]
	public delegate void PreviousRequestedEventHandler();

	[Signal]
	public delegate void CloseWindowRequestedEventHandler();

	[Signal]
	public delegate void QuitTutorialRequestedEventHandler();

	private const float FocusMargin = 10f;
	private const float ViewportMargin = 24f;
	private const float CalloutGap = 42f;

	private Control overlayRoot;
	private ColorRect topBlocker;
	private ColorRect bottomBlocker;
	private ColorRect leftBlocker;
	private ColorRect rightBlocker;
	private Panel focusBorder;
	private Line2D arrowLine;
	private Polygon2D arrowHead;
	private PanelContainer callout;
	private Panel calloutAttentionBorder;
	private Label titleLabel;
	private RichTextLabel bodyLabel;
	private Label footnoteLabel;
	private VBoxContainer slideStage;
	private BoxContainer slideLayout;
	private GridContainer slideImages;
	private Control slideTopSpacer;
	private Control slideBottomSpacer;
	private Button continueButton;
	private Button previousButton;
	private Button closeWindowButton;
	private Button restorePresentationButton;
	private Button quitTutorialButton;
	private Label slideProgressLabel;
	private Cursor tutorialCursor;

	private Rect2? requestedFocusRect;
	private TutorialCalloutPlacement requestedCalloutPlacement;
	private TutorialImagePlacement requestedImagePlacement;
	private float requestedImageWidthPercent = 40f;
	private float requestedGalleryAspectRatio = 1f;
	private Rect2 visibleFocusRect;
	private bool stepVisible;
	private double pulseTime;
	private double stepVisibleSeconds;
	private bool presentationMode;
	private bool presentationMinimized;
	private bool requestedDimBackground = true;

	public bool IsPresentationMinimized => presentationMinimized;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		overlayRoot = GetNode<Control>("OverlayRoot");
		topBlocker = GetNode<ColorRect>("%TopBlocker");
		bottomBlocker = GetNode<ColorRect>("%BottomBlocker");
		leftBlocker = GetNode<ColorRect>("%LeftBlocker");
		rightBlocker = GetNode<ColorRect>("%RightBlocker");
		focusBorder = GetNode<Panel>("%FocusBorder");
		arrowLine = GetNode<Line2D>("%ArrowLine");
		arrowHead = GetNode<Polygon2D>("%ArrowHead");
		callout = GetNode<PanelContainer>("%Callout");
		calloutAttentionBorder = GetNode<Panel>("%CalloutAttentionBorder");
		titleLabel = GetNode<Label>("%TitleLabel");
		bodyLabel = GetNode<RichTextLabel>("%BodyLabel");
		footnoteLabel = GetNode<Label>("%FootnoteLabel");
		slideStage = GetNode<VBoxContainer>("%SlideStage");
		slideLayout = GetNode<BoxContainer>("%SlideLayout");
		slideImages = GetNode<GridContainer>("%SlideImages");
		slideTopSpacer = GetNode<Control>("%SlideTopSpacer");
		slideBottomSpacer = GetNode<Control>("%SlideBottomSpacer");
		continueButton = GetNode<Button>("%ContinueButton");
		previousButton = GetNode<Button>("%PreviousButton");
		closeWindowButton = GetNode<Button>("%CloseWindowButton");
		restorePresentationButton = GetNode<Button>("%RestorePresentationButton");
		quitTutorialButton = GetNode<Button>("%QuitTutorialButton");
		slideProgressLabel = GetNode<Label>("%SlideProgressLabel");
		tutorialCursor = GetNodeOrNull<Cursor>("/root/Cursor");

		AudioHelpers.RegisterButtons(new Button[] {
			continueButton,
			previousButton,
			closeWindowButton,
			restorePresentationButton,
			quitTutorialButton
		});
		continueButton.Pressed += OnContinuePressed;
		previousButton.Pressed += OnPreviousPressed;
		closeWindowButton.Pressed += OnCloseWindowPressed;
		restorePresentationButton.Pressed += OnCloseWindowPressed;
		quitTutorialButton.Pressed += OnQuitTutorialPressed;
		GetViewport().SizeChanged += RefreshLayout;
		HideStep();
	}

	public override void _ExitTree()
	{
		tutorialCursor?.SetPopupCursorOverride(false);
		if (continueButton != null)
		{
			continueButton.Pressed -= OnContinuePressed;
		}
		if (previousButton != null)
		{
			previousButton.Pressed -= OnPreviousPressed;
		}
		if (closeWindowButton != null)
		{
			closeWindowButton.Pressed -= OnCloseWindowPressed;
		}
		if (restorePresentationButton != null)
		{
			restorePresentationButton.Pressed -= OnCloseWindowPressed;
		}
		if (quitTutorialButton != null)
		{
			quitTutorialButton.Pressed -= OnQuitTutorialPressed;
		}
		if (GetViewport() != null)
		{
			GetViewport().SizeChanged -= RefreshLayout;
		}
	}

	public override void _Process(double delta)
	{
		if (!stepVisible || presentationMinimized)
		{
			return;
		}

		pulseTime += delta;
		// Use a deliberately broad range: the previous 0.72-1.0 pulse was imperceptible in play.
		float alpha = 0.35f + (0.65f * ((Mathf.Sin((float)pulseTime * 5f) + 1f) * 0.5f));
		focusBorder.Modulate = new Color(1f, 1f, 1f, alpha);

		stepVisibleSeconds += delta;
		bool showAttentionFlicker =
			requestedCalloutPlacement == TutorialCalloutPlacement.TopRight &&
			stepVisibleSeconds >= 5d;
		calloutAttentionBorder.Visible = showAttentionFlicker;
		if (showAttentionFlicker)
		{
			float time = (float)(stepVisibleSeconds - 5d);
			// Keep attention below two cycles per second. The wider contrast makes the flicker
			// legible, but it still affects only a transparent, shadowless border.
			float slowPulse = (Mathf.Sin((time * 5f) +
				(0.2f * Mathf.Sin(time * 2.3f))) + 1f) * 0.5f;
			float unevenPulse = (Mathf.Sin((time * 10.5f) + 1.1f) + 1f) * 0.5f;
			float borderAlpha = 0.24f + (slowPulse * 0.46f) + (unevenPulse * 0.16f);
			calloutAttentionBorder.Modulate = new Color(1f, 1f, 1f, borderAlpha);
		}
	}

	public void ShowStep(
		string title,
		string message,
		Rect2? targetScreenRect,
		TutorialOverlayMode mode,
		bool showContinue = true,
		bool showQuitTutorial = true,
		bool dimBackground = true,
		TutorialCalloutPlacement calloutPlacement = TutorialCalloutPlacement.Auto,
		IReadOnlyList<string> imagePaths = null,
		IReadOnlyList<string> imageCaptions = null,
		TutorialImagePlacement imagePlacement = TutorialImagePlacement.Bottom,
		int imageGap = 16,
		float imageWidthPercent = 40f,
		float bodyFontScale = 1f,
		bool bodyBold = false,
		string footnote = null,
		bool showPrevious = false,
		string progressText = null)
	{
		titleLabel.Text = title ?? string.Empty;
		bodyLabel.Text = message ?? string.Empty;
		footnoteLabel.Text = footnote ?? string.Empty;
		footnoteLabel.Visible = !string.IsNullOrWhiteSpace(footnote);
		requestedFocusRect = targetScreenRect;
		requestedCalloutPlacement = calloutPlacement;
		requestedDimBackground = dimBackground;
		SetPresentationMinimized(false);
		if (presentationMode)
		{
			bool fullScreenSlide = calloutPlacement == TutorialCalloutPlacement.FullScreen;
			float resolvedBodyScale = Mathf.Clamp(bodyFontScale, 0.75f, 2f);
			titleLabel.AddThemeFontSizeOverride("font_size", fullScreenSlide ? 42 : 28);
			int bodyFontSize = Mathf.RoundToInt(
				(fullScreenSlide ? 26f : 20f) * resolvedBodyScale);
			ApplyRichTextFontSize(bodyLabel, bodyFontSize);
			string[] bodyFontFamilies =
				{ "OCR B", "OCR-B", "OCRB", "IBM Plex Mono", "monospace" };
			int bodyFontWeight = bodyBold ? 700 : 400;
			ApplyPresentationRichTextFont(
				bodyLabel,
				CreatePresentationFont(bodyFontFamilies, bodyFontWeight),
				CreatePresentationFont(bodyFontFamilies, bodyFontWeight, true));
			footnoteLabel.AddThemeFontSizeOverride("font_size", fullScreenSlide ? 14 : 12);
		}
		continueButton.Visible = showContinue;
		previousButton.Visible = presentationMode && showPrevious;
		slideProgressLabel.Visible = presentationMode && !string.IsNullOrWhiteSpace(progressText);
		slideProgressLabel.Text = progressText ?? string.Empty;
		closeWindowButton.Visible = true;
		quitTutorialButton.Visible = showQuitTutorial;
		SetSlideImages(
			imagePaths,
			imageCaptions,
			imagePlacement,
			imageGap,
			imageWidthPercent);

		// A guided step with no resolved target is the safe text-only fallback: the dimmer remains
		// visible, but input passes through so a missing registration cannot trap the player.
		bool passThroughFallback = mode == TutorialOverlayMode.GuidedAction && !targetScreenRect.HasValue;
		SetBlockerMouseFilters(passThroughFallback
			? Control.MouseFilterEnum.Ignore
			: Control.MouseFilterEnum.Stop);
		SetBlockersVisible(dimBackground);
		overlayRoot.Visible = true;
		stepVisible = true;
		tutorialCursor?.SetPopupCursorOverride(true);
		pulseTime = 0d;
		stepVisibleSeconds = 0d;
		calloutAttentionBorder.Visible = false;
		RefreshLayout();
		Callable.From(RefreshLayout).CallDeferred();

		if (showContinue)
		{
			continueButton.GrabFocus();
		}
	}

	/// <summary>Switches the shared tutorial overlay into a conference-slide layout.</summary>
	public void ConfigurePresentationMode(bool enabled)
	{
		presentationMode = enabled;

		if (enabled)
		{
			slideTopSpacer.Visible = true;
			slideBottomSpacer.Visible = true;
			SystemFont bodyFont = CreatePresentationFont(
				new[] { "OCR B", "OCR-B", "OCRB", "IBM Plex Mono", "monospace" }, 400);
			SystemFont headingFont = CreatePresentationFont(
				new[] { "OCR A", "OCR-A", "OCR B", "IBM Plex Mono", "monospace" }, 700);

			ApplyPresentationFont(titleLabel, headingFont);
			ApplyPresentationRichTextFont(
				bodyLabel,
				bodyFont,
				CreatePresentationFont(
					new[] { "OCR B", "OCR-B", "OCRB", "IBM Plex Mono", "monospace" },
					400,
					true));
			ApplyPresentationFont(footnoteLabel, bodyFont);
			ApplyPresentationFont(continueButton, bodyFont);
			ApplyPresentationFont(previousButton, bodyFont);
			ApplyPresentationFont(closeWindowButton, bodyFont);
			ApplyPresentationFont(restorePresentationButton, bodyFont);
			ApplyPresentationFont(quitTutorialButton, bodyFont);
			ApplyPresentationFont(slideProgressLabel, bodyFont);
			ApplyPresentationColors();

			titleLabel.AddThemeFontSizeOverride("font_size", 34);
			ApplyRichTextFontSize(bodyLabel, 23);
			titleLabel.AddThemeConstantOverride("line_spacing", 4);
			bodyLabel.AddThemeConstantOverride("line_separation", 4);
			continueButton.Text = "NEXT";
			quitTutorialButton.Text = "QUIT";
			closeWindowButton.Text = "-";
			closeWindowButton.TooltipText = "Minimize slide";
			closeWindowButton.CustomMinimumSize = new Vector2(44f, 38f);
		}
		else
		{
			SetPresentationMinimized(false);
			slideTopSpacer.Visible = false;
			slideBottomSpacer.Visible = false;
			ApplyPresentationFont(titleLabel, null);
			ApplyPresentationRichTextFont(bodyLabel, null, null);
			ApplyPresentationFont(footnoteLabel, null);
			ApplyPresentationFont(continueButton, null);
			ApplyPresentationFont(previousButton, null);
			ApplyPresentationFont(closeWindowButton, null);
			ApplyPresentationFont(restorePresentationButton, null);
			ApplyPresentationFont(quitTutorialButton, null);
			ApplyPresentationFont(slideProgressLabel, null);
			RemovePresentationColors();

			titleLabel.AddThemeFontSizeOverride("font_size", 28);
			ApplyRichTextFontSize(bodyLabel, 19);
			titleLabel.RemoveThemeConstantOverride("line_spacing");
			bodyLabel.RemoveThemeConstantOverride("line_separation");
			continueButton.Text = "CONTINUE";
			quitTutorialButton.Text = "QUIT TUTORIAL";
			closeWindowButton.Text = "CLOSE WINDOW  X";
			closeWindowButton.TooltipText = "Close this tutorial window";
			closeWindowButton.CustomMinimumSize = new Vector2(165f, 38f);
		}
	}

	private static SystemFont CreatePresentationFont(
		string[] familyNames,
		int weight,
		bool italic = false)
	{
		return new SystemFont
		{
			FontNames = familyNames,
			FontWeight = weight,
			FontItalic = italic,
		};
	}

	private void ApplyPresentationColors()
	{
		callout.AddThemeStyleboxOverride("panel", CreatePresentationPanelStyle());
		titleLabel.AddThemeColorOverride("font_color", Colors.Black);
		bodyLabel.AddThemeColorOverride("default_color", Colors.Black);
		footnoteLabel.AddThemeColorOverride("font_color", new Color(0.32f, 0.32f, 0.32f));
		slideProgressLabel.AddThemeColorOverride("font_color", Colors.Black);

		ApplyPresentationButtonStyle(continueButton, false);
		ApplyPresentationButtonStyle(previousButton, false);
		ApplyPresentationButtonStyle(closeWindowButton, false);
		ApplyPresentationButtonStyle(restorePresentationButton, false);
		ApplyPresentationButtonStyle(quitTutorialButton, true);
	}

	private void RemovePresentationColors()
	{
		callout.RemoveThemeStyleboxOverride("panel");
		titleLabel.RemoveThemeColorOverride("font_color");
		bodyLabel.RemoveThemeColorOverride("default_color");
		footnoteLabel.RemoveThemeColorOverride("font_color");
		slideProgressLabel.RemoveThemeColorOverride("font_color");

		RemovePresentationButtonStyle(continueButton);
		RemovePresentationButtonStyle(previousButton);
		RemovePresentationButtonStyle(closeWindowButton);
		RemovePresentationButtonStyle(restorePresentationButton);
		RemovePresentationButtonStyle(quitTutorialButton);
	}

	private static StyleBoxFlat CreatePresentationPanelStyle()
	{
		return new StyleBoxFlat
		{
			BgColor = Colors.White,
			BorderWidthLeft = 2,
			BorderWidthTop = 2,
			BorderWidthRight = 2,
			BorderWidthBottom = 2,
			BorderColor = Colors.Black,
		};
	}

	private static void ApplyPresentationButtonStyle(Button button, bool destructive)
	{
		Color textColor = destructive ? new Color(0.55f, 0.05f, 0.05f) : Colors.Black;
		button.AddThemeColorOverride("font_color", textColor);
		button.AddThemeColorOverride("font_hover_color", Colors.Black);
		button.AddThemeColorOverride("font_pressed_color", Colors.White);
		button.AddThemeColorOverride("font_focus_color", Colors.Black);
		button.AddThemeStyleboxOverride("normal", CreatePresentationButtonBox(Colors.White));
		button.AddThemeStyleboxOverride("hover", CreatePresentationButtonBox(new Color(0.92f, 0.92f, 0.92f)));
		button.AddThemeStyleboxOverride("pressed", CreatePresentationButtonBox(Colors.Black));
		button.AddThemeStyleboxOverride("focus", CreatePresentationButtonBox(Colors.White, 3));
	}

	private static StyleBoxFlat CreatePresentationButtonBox(Color background, int borderWidth = 1)
	{
		return new StyleBoxFlat
		{
			BgColor = background,
			BorderWidthLeft = borderWidth,
			BorderWidthTop = borderWidth,
			BorderWidthRight = borderWidth,
			BorderWidthBottom = borderWidth,
			BorderColor = Colors.Black,
		};
	}

	private static void RemovePresentationButtonStyle(Button button)
	{
		button.RemoveThemeColorOverride("font_color");
		button.RemoveThemeColorOverride("font_hover_color");
		button.RemoveThemeColorOverride("font_pressed_color");
		button.RemoveThemeColorOverride("font_focus_color");
		button.RemoveThemeStyleboxOverride("normal");
		button.RemoveThemeStyleboxOverride("hover");
		button.RemoveThemeStyleboxOverride("pressed");
		button.RemoveThemeStyleboxOverride("focus");
	}

	private static void ApplyPresentationFont(Control control, Font font)
	{
		if (font == null)
		{
			control.RemoveThemeFontOverride("font");
			return;
		}
		control.AddThemeFontOverride("font", font);
	}

	private static void ApplyPresentationRichTextFont(
		RichTextLabel control,
		Font normalFont,
		Font italicFont)
	{
		if (normalFont == null)
		{
			control.RemoveThemeFontOverride("normal_font");
			control.RemoveThemeFontOverride("bold_font");
			control.RemoveThemeFontOverride("italics_font");
			control.RemoveThemeFontOverride("bold_italics_font");
			return;
		}

		control.AddThemeFontOverride("normal_font", normalFont);
		control.AddThemeFontOverride("bold_font", normalFont);
		control.AddThemeFontOverride("italics_font", italicFont ?? normalFont);
		control.AddThemeFontOverride("bold_italics_font", italicFont ?? normalFont);
	}

	private static void ApplyRichTextFontSize(RichTextLabel control, int fontSize)
	{
		control.AddThemeFontSizeOverride("normal_font_size", fontSize);
		control.AddThemeFontSizeOverride("bold_font_size", fontSize);
		control.AddThemeFontSizeOverride("italics_font_size", fontSize);
		control.AddThemeFontSizeOverride("bold_italics_font_size", fontSize);
	}

	private void SetSlideImages(
		IReadOnlyList<string> imagePaths,
		IReadOnlyList<string> imageCaptions,
		TutorialImagePlacement placement,
		int gap,
		float widthPercent)
	{
		requestedImagePlacement = placement;
		requestedImageWidthPercent = Mathf.Clamp(widthPercent, 5f, 100f);
		ClearSlideImages();
		// Do not let the dimensions calculated for the previous slide influence this slide's
		// initial minimum-size calculation.
		slideImages.CustomMinimumSize = Vector2.Zero;
		List<Texture2D> textures = new();
		if (imagePaths != null)
		{
			foreach (string imagePath in imagePaths)
			{
				if (string.IsNullOrWhiteSpace(imagePath)) continue;
				if (!ResourceLoader.Exists(imagePath))
				{
					GD.PushWarning($"Tutorial slide image does not exist: '{imagePath}'.");
					continue;
				}

				Texture2D texture = GD.Load<Texture2D>(imagePath);
				if (texture != null) textures.Add(texture);
			}
		}

		RebuildSlideLayout(
			placement,
			textures.Count,
			Mathf.Max(0, gap),
			requestedImageWidthPercent);
		requestedGalleryAspectRatio = 0f;
		foreach (Texture2D texture in textures)
		{
			if (texture.GetHeight() > 0)
			{
				requestedGalleryAspectRatio += (float)texture.GetWidth() / texture.GetHeight();
			}
		}
		requestedGalleryAspectRatio = Mathf.Max(0.01f, requestedGalleryAspectRatio);
		bool sidePlacement = placement is TutorialImagePlacement.Left or TutorialImagePlacement.Right;
		for (int index = 0; index < textures.Count; index++)
		{
			Texture2D texture = textures[index];
			VBoxContainer imageFrame = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			TextureRect image = new()
			{
				Texture = texture,
				// Side galleries receive their dimensions from widthPercent in ApplyImageWidth.
				// A fixed minimum here would override small percentages in compact callouts.
				CustomMinimumSize = Vector2.Zero,
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				TextureFilter = CanvasItem.TextureFilterEnum.Linear,
			};
			imageFrame.AddChild(image);

			string captionText = imageCaptions != null && index < imageCaptions.Count
				? imageCaptions[index]
				: string.Empty;
			if (imageCaptions != null && index < imageCaptions.Count)
			{
				Label caption = new()
				{
					Text = captionText,
					CustomMinimumSize = new Vector2(0f, presentationMode ? 28f : 22f),
					HorizontalAlignment = HorizontalAlignment.Center,
					AutowrapMode = TextServer.AutowrapMode.WordSmart,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};
				caption.AddThemeFontSizeOverride("font_size", presentationMode ? 18 : 14);
				if (presentationMode)
				{
					caption.AddThemeColorOverride("font_color", Colors.Black);
					ApplyPresentationFont(
						caption,
						CreatePresentationFont(
							new[] { "OCR B", "OCR-B", "OCRB", "IBM Plex Mono", "monospace" },
							700));
				}
				imageFrame.AddChild(caption);
			}

			slideImages.AddChild(imageFrame);
		}
		slideImages.Visible = textures.Count > 0;
	}

	private void RebuildSlideLayout(
		TutorialImagePlacement placement,
		int imageCount,
		int gap,
		float widthPercent)
	{
		bool horizontal = placement is TutorialImagePlacement.Left or TutorialImagePlacement.Right;
		BoxContainer replacement = horizontal ? new HBoxContainer() : new VBoxContainer();
		replacement.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		replacement.Alignment = BoxContainer.AlignmentMode.Center;
		replacement.AddThemeConstantOverride("separation", gap);
		slideStage.AddChild(replacement);
		slideStage.MoveChild(replacement, slideBottomSpacer.GetIndex());

		bool imagesFirst = placement is TutorialImagePlacement.Top or TutorialImagePlacement.Left;
		if (imagesFirst)
		{
			slideImages.Reparent(replacement);
			bodyLabel.Reparent(replacement);
		}
		else
		{
			bodyLabel.Reparent(replacement);
			slideImages.Reparent(replacement);
		}

		slideLayout.QueueFree();
		slideLayout = replacement;
		bodyLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		bodyLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		bodyLabel.SizeFlagsStretchRatio = horizontal ? 100f - widthPercent : 1f;
		slideImages.SizeFlagsHorizontal = horizontal
			? Control.SizeFlags.ExpandFill
			: Control.SizeFlags.ShrinkCenter;
		slideImages.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		slideImages.SizeFlagsStretchRatio = horizontal ? widthPercent : 1f;
		slideImages.Columns = horizontal ? 1 : Mathf.Max(1, imageCount);
		slideImages.AddThemeConstantOverride("h_separation", gap);
		slideImages.AddThemeConstantOverride("v_separation", gap);
	}

	private void ClearSlideImages()
	{
		if (slideImages == null) return;
		foreach (Node child in slideImages.GetChildren())
		{
			slideImages.RemoveChild(child);
			child.QueueFree();
		}
		slideImages.Visible = false;
	}

	public void SetFocusRect(Rect2? targetScreenRect)
	{
		requestedFocusRect = targetScreenRect;
		if (stepVisible)
		{
			RefreshLayout();
		}
	}

	public void HideStep()
	{
		SetPresentationMinimized(false);
		stepVisible = false;
		requestedFocusRect = null;
		requestedCalloutPlacement = TutorialCalloutPlacement.Auto;
		pulseTime = 0d;
		stepVisibleSeconds = 0d;
		if (calloutAttentionBorder != null)
		{
			calloutAttentionBorder.Visible = false;
		}
		if (overlayRoot != null)
		{
			overlayRoot.Visible = false;
		}
		ClearSlideImages();
		tutorialCursor?.SetPopupCursorOverride(false);
	}

	public void SetPresentationMinimized(bool minimized)
	{
		presentationMinimized = presentationMode && minimized;
		if (callout == null || restorePresentationButton == null)
		{
			return;
		}

		callout.Visible = !presentationMinimized;
		restorePresentationButton.Visible = presentationMinimized;
		if (presentationMinimized)
		{
			SetBlockersVisible(false);
			focusBorder.Visible = false;
			arrowLine.Visible = false;
			arrowHead.Visible = false;
			calloutAttentionBorder.Visible = false;
			tutorialCursor?.SetPopupCursorOverride(false);
			return;
		}

		SetBlockersVisible(requestedDimBackground);
		stepVisibleSeconds = 0d;
		calloutAttentionBorder.Visible = false;
		if (stepVisible)
		{
			tutorialCursor?.SetPopupCursorOverride(true);
			RefreshLayout();
		}
	}

	public void RefreshLayout()
	{
		if (!stepVisible || overlayRoot == null || presentationMinimized)
		{
			return;
		}

		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		overlayRoot.Size = viewportSize;
		visibleFocusRect = CalculateVisibleFocusRect(viewportSize);
		LayoutBlockers(viewportSize, visibleFocusRect);

		bool hasFocus = requestedCalloutPlacement != TutorialCalloutPlacement.FullScreen &&
			requestedFocusRect.HasValue && visibleFocusRect.Size.X > 0f &&
			visibleFocusRect.Size.Y > 0f;
		focusBorder.Visible = hasFocus;
		if (hasFocus)
		{
			ApplyRect(focusBorder, visibleFocusRect);
		}

		LayoutCallout(viewportSize, hasFocus);
		ApplyImageWidth();
		LayoutArrow(hasFocus);
	}

	private void ApplyImageWidth()
	{
		if (slideImages == null || !slideImages.Visible) return;
		float availableWidth = Mathf.Max(1f, callout.Size.X - 48f);
		float galleryWidth = availableWidth * requestedImageWidthPercent / 100f;
		bool sidePlacement = requestedImagePlacement is
			TutorialImagePlacement.Left or TutorialImagePlacement.Right;
		if (sidePlacement)
		{
			float availableHeight = Mathf.Max(
				120f,
				callout.Size.Y - (presentationMode ? 150f : 100f));
			slideImages.CustomMinimumSize = new Vector2(galleryWidth, availableHeight);
			return;
		}

		float reservedHeight = titleLabel.GetCombinedMinimumSize().Y +
			bodyLabel.GetCombinedMinimumSize().Y +
			(footnoteLabel.Visible ? footnoteLabel.GetCombinedMinimumSize().Y : 0f) +
			140f;
		float verticalAvailableHeight = Mathf.Max(120f, callout.Size.Y - reservedHeight);
		float galleryHeight = Mathf.Min(
			galleryWidth / requestedGalleryAspectRatio,
			verticalAvailableHeight);
		slideImages.CustomMinimumSize = new Vector2(galleryWidth, galleryHeight);
	}

	private Rect2 CalculateVisibleFocusRect(Vector2 viewportSize)
	{
		if (!requestedFocusRect.HasValue)
		{
			return new Rect2();
		}

		Rect2 expanded = requestedFocusRect.Value.Grow(FocusMargin);
		float left = Mathf.Clamp(expanded.Position.X, 0f, viewportSize.X);
		float top = Mathf.Clamp(expanded.Position.Y, 0f, viewportSize.Y);
		float right = Mathf.Clamp(expanded.End.X, left, viewportSize.X);
		float bottom = Mathf.Clamp(expanded.End.Y, top, viewportSize.Y);
		return new Rect2(left, top, right - left, bottom - top);
	}

	private void LayoutBlockers(Vector2 viewportSize, Rect2 focusRect)
	{
		if (!requestedFocusRect.HasValue || focusRect.Size.X <= 0f || focusRect.Size.Y <= 0f)
		{
			ApplyRect(topBlocker, new Rect2(Vector2.Zero, viewportSize));
			ApplyRect(bottomBlocker, new Rect2());
			ApplyRect(leftBlocker, new Rect2());
			ApplyRect(rightBlocker, new Rect2());
			return;
		}

		float left = focusRect.Position.X;
		float top = focusRect.Position.Y;
		float right = focusRect.End.X;
		float bottom = focusRect.End.Y;

		ApplyRect(topBlocker, new Rect2(0f, 0f, viewportSize.X, top));
		ApplyRect(bottomBlocker, new Rect2(0f, bottom, viewportSize.X, viewportSize.Y - bottom));
		ApplyRect(leftBlocker, new Rect2(0f, top, left, bottom - top));
		ApplyRect(rightBlocker, new Rect2(right, top, viewportSize.X - right, bottom - top));
	}

	private void LayoutCallout(Vector2 viewportSize, bool hasFocus)
	{
		if (requestedCalloutPlacement == TutorialCalloutPlacement.FullScreen)
		{
			Vector2 fullScreenSize = new(
				Mathf.Max(240f, viewportSize.X),
				Mathf.Max(160f, viewportSize.Y));
			callout.Position = Vector2.Zero;
			callout.Size = fullScreenSize;
			ApplyRect(calloutAttentionBorder, new Rect2(callout.Position, fullScreenSize));
			return;
		}

		Vector2 minimumSize = callout.GetCombinedMinimumSize();
		minimumSize.X = Mathf.Max(minimumSize.X, presentationMode ? 680f : 460f);
		minimumSize.Y = Mathf.Max(minimumSize.Y, presentationMode ? 280f : 190f);
		minimumSize.X = Mathf.Min(minimumSize.X, Mathf.Max(240f, viewportSize.X - (ViewportMargin * 2f)));
		minimumSize.Y = Mathf.Min(minimumSize.Y, Mathf.Max(160f, viewportSize.Y - (ViewportMargin * 2f)));
		callout.Size = minimumSize;

		Vector2 position;
		if (requestedCalloutPlacement == TutorialCalloutPlacement.TopLeft)
		{
			position = new Vector2(ViewportMargin, ViewportMargin);
		}
		else if (requestedCalloutPlacement == TutorialCalloutPlacement.TopRight)
		{
			position = new Vector2(
				viewportSize.X - minimumSize.X - ViewportMargin,
				ViewportMargin);
		}
		else if (!hasFocus)
		{
			position = (viewportSize - minimumSize) * 0.5f;
		}
		else
		{
			float rightRoom = viewportSize.X - visibleFocusRect.End.X;
			float leftRoom = visibleFocusRect.Position.X;
			float bottomRoom = viewportSize.Y - visibleFocusRect.End.Y;
			float topRoom = visibleFocusRect.Position.Y;
			float largestRoom = Mathf.Max(Mathf.Max(rightRoom, leftRoom), Mathf.Max(bottomRoom, topRoom));

			if (largestRoom == rightRoom)
			{
				position = new Vector2(
					visibleFocusRect.End.X + CalloutGap,
					visibleFocusRect.GetCenter().Y - (minimumSize.Y * 0.5f));
			}
			else if (largestRoom == leftRoom)
			{
				position = new Vector2(
					visibleFocusRect.Position.X - minimumSize.X - CalloutGap,
					visibleFocusRect.GetCenter().Y - (minimumSize.Y * 0.5f));
			}
			else if (largestRoom == bottomRoom)
			{
				position = new Vector2(
					visibleFocusRect.GetCenter().X - (minimumSize.X * 0.5f),
					visibleFocusRect.End.Y + CalloutGap);
			}
			else
			{
				position = new Vector2(
					visibleFocusRect.GetCenter().X - (minimumSize.X * 0.5f),
					visibleFocusRect.Position.Y - minimumSize.Y - CalloutGap);
			}
		}

		position.X = Mathf.Clamp(position.X, ViewportMargin, Mathf.Max(ViewportMargin, viewportSize.X - minimumSize.X - ViewportMargin));
		position.Y = Mathf.Clamp(position.Y, ViewportMargin, Mathf.Max(ViewportMargin, viewportSize.Y - minimumSize.Y - ViewportMargin));
		callout.Position = position;
		ApplyRect(calloutAttentionBorder, new Rect2(position, minimumSize));
	}

	private void LayoutArrow(bool hasFocus)
	{
		if (!hasFocus)
		{
			arrowLine.Visible = false;
			arrowHead.Visible = false;
			return;
		}

		Rect2 calloutRect = callout.GetGlobalRect();
		Vector2 start = ClosestPointOnRect(calloutRect, visibleFocusRect.GetCenter());
		Vector2 target = ClosestPointOnRect(visibleFocusRect, calloutRect.GetCenter());
		Vector2 direction = target - start;
		if (direction.LengthSquared() < 64f)
		{
			arrowLine.Visible = false;
			arrowHead.Visible = false;
			return;
		}

		Vector2 normalized = direction.Normalized();
		start += normalized * 8f;
		Vector2 lineEnd = target - (normalized * 18f);
		arrowLine.Points = new Vector2[] { start, lineEnd };
		arrowLine.Visible = true;
		arrowHead.Position = target;
		arrowHead.Rotation = normalized.Angle();
		arrowHead.Visible = true;
	}

	private static Vector2 ClosestPointOnRect(Rect2 rect, Vector2 point)
	{
		return new Vector2(
			Mathf.Clamp(point.X, rect.Position.X, rect.End.X),
			Mathf.Clamp(point.Y, rect.Position.Y, rect.End.Y));
	}

	private void SetBlockerMouseFilters(Control.MouseFilterEnum mouseFilter)
	{
		topBlocker.MouseFilter = mouseFilter;
		bottomBlocker.MouseFilter = mouseFilter;
		leftBlocker.MouseFilter = mouseFilter;
		rightBlocker.MouseFilter = mouseFilter;
	}

	private void SetBlockersVisible(bool visible)
	{
		topBlocker.Visible = visible;
		bottomBlocker.Visible = visible;
		leftBlocker.Visible = visible;
		rightBlocker.Visible = visible;
	}

	private static void ApplyRect(Control control, Rect2 rect)
	{
		control.Position = rect.Position;
		control.Size = new Vector2(Mathf.Max(0f, rect.Size.X), Mathf.Max(0f, rect.Size.Y));
	}

	private void OnContinuePressed()
	{
		EmitSignal(SignalName.ContinueRequested);
	}

	private void OnPreviousPressed()
	{
		EmitSignal(SignalName.PreviousRequested);
	}

	private void OnCloseWindowPressed()
	{
		EmitSignal(SignalName.CloseWindowRequested);
	}

	private void OnQuitTutorialPressed()
	{
		EmitSignal(SignalName.QuitTutorialRequested);
	}
}
