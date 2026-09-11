namespace Game.UI.Tutorial.Scripts;

/// <summary>
/// Conference presentation hosted inside Level 7. Every slide advances manually, so the
/// presenter controls the pacing and can demonstrate the live game between slides.
/// </summary>
public sealed class Level7Presentation : TutorialScript
{
	public override bool UsesPresentationLayout => true;

	public override void Build(TutorialBuilder presentation)
	{
		presentation.Step("demo.introduction").When(TutorialEvent.LevelReady)
			.Say("EXOPLANET EXPLORER",
				"An open-source educational video game for teaching Human–Autonomy Teaming")
			.WithImage("res://assets/HAT_game_logo_cropped.png")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.context")
			.Say("THE CONTEXT",
				"Autonomous agents are entering increasingly complex sociotechnical systems.\n\nHuman–Autonomy Teaming asks us to design these agents as collaborative teammates—not merely as passive tools.\n\nCoactive design and Interdependence Analysis help translate joint activity into concrete teaming requirements.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.problem")
			.Say("THE PROBLEM",
				"Interdependence Analysis remains difficult to teach.\n\nStudents may have little direct experience working with autonomous teammates. Abstract, text-based cases make it difficult to connect joint activity, agent capabilities, and interface requirements.\n\nThe missing ingredient is an environment where assumptions can be tested through experience.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.existing-solutions")
			.Say("OTHER SOLUTIONS—AND THE GAP",
				"HATTB and NeoCITIES support controlled human-in-the-loop research. BW4T, MATRX, and CHAOPT provide reusable coordination testbeds.\n\nThese systems are valuable research platforms, but they are not primarily designed for higher-education pedagogy. Common barriers include installation, accessibility, engagement, and limited alignment with explicit HAT learning objectives.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.primer")
			.Say("OUR APPROACH: FROM PRIMER TO GAME",
				"Exoplanet Explorer began as a primer for a JPL laboratory HSI simulation.\n\nIt became a playable planetary exploration mission in which one human operator collaborates with multiple autonomous rovers and drones in a partially observable environment.")
			.WithImage("res://assets/monolith_fragment_v3.png")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.open-source")
			.Say("OPEN-SOURCE",
				"The game is an openly accessible educational resource that instructors, students, and researchers can inspect, adapt, and extend.\n\nSource and release archive: Zenodo DOI 10.5281/zenodo.18651448")
			.WithImage("res://assets/HAT_game_logo_cropped.png")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.multi-platform")
			.Say("MULTI-PLATFORM",
				"A desktop game that can be exported for Windows, Linux, and macOS.\n\nThe same interactive scenario can support classroom instruction, individual exploration, or a live demonstration like this one.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.ia-designed")
			.Say("DESIGNED THROUGH IA—FOR TEACHING IA",
				"Interdependence Analysis shaped the game itself. The environment makes three teaming requirements observable:\n\n• Observability—perceiving teammates’ status and actions\n• Predictability—anticipating teammates’ actions\n• Directability—directing the behavior of others\n\nStudents can observe a failure, revise their model, and test it again.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.dedicated-level")
			.Say("NOW: THE LIVE GAME",
				"Dedicated tutorial levels introduce the mechanics progressively. For this demonstration, we can follow those same steps while stopping anywhere to discuss task allocation, robot capabilities, coordination constraints, information visibility, and levels of autonomy.")
			.PointTo(TutorialTargetIds.DeploymentPanel)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilContinue();
	}
}
