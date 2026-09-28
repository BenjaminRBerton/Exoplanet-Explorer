# Presentation images

Add Level 7 slide images to this folder and reference them from
`scenes/ui/tutorial/scripts/Level7Presentation.cs`, for example:

```csharp
.WithImage("res://assets/presentation/system-overview.png")

.WithImage(
	"res://assets/presentation/system-overview.png",
	TutorialImagePlacement.Right,
	gap: 32,
	widthPercent: 65f)

.WithImages(
	TutorialImagePlacement.Bottom,
	20,
	55f,
	"res://assets/presentation/overview-a.png",
	"res://assets/presentation/overview-b.png")
```

Images are scaled proportionally to fit the slide. A 16:9 image around 1280×720 is a useful
default for conference screens. Supported placements are `Top`, `Bottom`, `Left`, `Right`, and
`Center`; `gap` controls both the text-to-gallery spacing and spacing between gallery images.
`widthPercent` controls the gallery's share of the available slide width and is clamped from 5% to
95%. For `Left` and `Right`, the remaining width is assigned to the text.
