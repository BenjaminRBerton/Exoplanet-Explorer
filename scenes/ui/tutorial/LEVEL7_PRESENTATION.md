# Level 7 conference presentation

Level 7 uses the normal tutorial director as an in-game slide deck. Select **Level 7
(Presentation)** from the level-select screen. The presentation starts automatically and does
not alter the appearance of tutorials in Levels 1–3.

## Editing slides

Edit `scripts/Level7Presentation.cs`. Each block beginning with `presentation.Step(...)` is one
slide. Slides appear in source order and should have a unique ID.

```csharp
presentation.Step("demo.my-slide")
    .Say("SLIDE TITLE", "Short slide text for the audience.")
    .WithImage("res://assets/presentation/my-picture.png")
    .HardPause().UntilContinue();
```

`WithImage(...)` is optional. PNG, JPG, SVG, and other Godot-supported texture resources can be
used. The image is fitted inside the slide without changing its aspect ratio. Place conference
images under `assets/presentation/` (or any project folder) and reference them with a `res://`
path.

## Slide behavior

- `HardPause()` freezes the game while the slide is shown.
- `PlaceCallout(TutorialCalloutPlacement.FullScreen)` creates a full-screen slide.
- `GuideAction().UndimBackground()` leaves the game interactive behind a top-right slide.
- `PointTo(TutorialTargetIds.SomeTarget)` highlights a registered game control.
- `PlaceCallout(TutorialCalloutPlacement.TopLeft)` or `TopRight` fixes its screen position.
- **Back** and **Next** navigate the deck; **Exit presentation** ends it.

Presentation slides use the bundled Inter variable font for projector readability. Regular
tutorials keep the existing game font.
