using Game.Component;

namespace Game.UI.Tutorial.Scripts;

/// <summary>
/// ASPIRE 2026 conference presentation hosted inside Level 7. Every slide advances
/// manually so the presenter controls the pacing.
/// </summary>
public sealed class Level7Presentation : TutorialScript
{
	public override bool UsesPresentationLayout => true;

	private static bool IsRobot(TutorialEventContext context, string displayName) =>
		context.Subject is BuildingComponent building &&
		building.BuildingResource?.DisplayName == displayName;

	private static bool IsBase(TutorialEventContext context) => IsRobot(context, "Base");
	private static bool IsRover(TutorialEventContext context) => IsRobot(context, "Rover");
	private static bool IsDrone(TutorialEventContext context) => IsRobot(context, "Drone");

	public override void Build(TutorialBuilder presentation)
	{
		presentation.Step("demo.introduction").When(TutorialEvent.LevelReady)
			.Say("EXOPLANET EXPLORER",
				"An open-source educational video game for teaching human-autonomy teaming\n\n\n\nBenjamin Rémi Berton & Philippe Doyon-Poulin, Polytechnique Montréal\n\n\n\n\n\nASPIRE 2026 Demonstration")
			.WithImage("res://assets/HAT_game_logo_cropped.png")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.educational-challenge")
			.Say("WHY HAVE WE CREATED THIS GAME?",
				"Systems of greater and greater autonomy are being developed, especially now with the advent of agentic Artificial Intelligence.\n\n• Integrating autonomous agents into complex sociotechnical systems creates new design challenges for future engineers.\n\n• Interdependence Analysis helps model joint activity and derive human-autonomy teaming requirements.\n\n• Learners rarely have direct experience with autonomous agents, so the concepts remain abstract and text-based cases limit experimentation. Interdependence analysis is hard to learn, and even harder to teach.")
			.WithFootnote("Johnson et al., 2011 — Coactive Design: Designing Support for Interdependence in Joint Activity")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.educational-question")
			.Say("","How can students observe these interdependencies, test them, and derive interface requirements from them?")
			.EmphasizeBody(fontScale: 1.45f, bold: true)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.our-idea")
			.Say("OUR IDEA",
				"Exoplanet Explorer: a videogame designed to teach Interdependence Analysis and more broadly, Human-Autonomy Teaming.")
			.EmphasizeBody(fontScale: 1.45f, bold: true)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.testbed-hattb")
			.Say("RELATED WORK — HATTB",
				"NASA's Human-Autonomy Teaming Task Battery provides controlled, repeatable tasks for studying human interaction with autonomous teammates.\n\nSTRENGTH\nHigh experimental control across reusable human-in-the-loop tasks.\n\nPUBLIC AVAILABILITY\nAlthough described by NASA Langley Research Center, we could not identify a public release or download.")
			.WithFootnote("NASA Langley Research Center, 2024 — Human-Autonomy Teaming Task Battery (HATTB)")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/HATTB.png",
				TutorialImagePlacement.Right, gap: 16, widthPercent: 65f)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.testbed-neocities")
			.Say("RELATED WORK — NEOCITIES",
				"NeoCITIES simulates distributed command-and-control work in which team members manage unfolding emergency events.\n\nSTRENGTH\nA controlled setting for studying teamwork, communication, and shared situation awareness.\n\nPUBLIC AVAILABILITY\nNeoCITIES does not appear to be publicly available, and no open-source distribution has been identified.")
			.WithFootnote("Hellar & McNeese, 2010 — NeoCITIES")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/NeoCITIES.png",
				TutorialImagePlacement.Right, gap: 32, widthPercent: 65f)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.testbed-bw4t")
			.Say("RELATED WORK — BW4T",
				"In Blocks World 4 Teams, human and software teammates coordinate the collection and delivery of colored blocks.\n\nSTRENGTH\nOpen, accessible, and highly controllable for studying coordination strategies.\n\nEDUCATIONAL GAP\nIts abstract grid world lacks immersion.")
			.WithFootnote("Johnson et al., 2009 — Joint Activity Testbed: Blocks World for Teams (BW4T) · DOI: 10.1007/978-3-642-10203-5_26")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/BW4T.png",
				TutorialImagePlacement.Right, 40)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.testbed-matrx")
			.Say("RELATED WORK — MATRX",
				"MATRX—Human-Agent Teaming Rapid Experimentation software is an open Python framework for rapidly building configurable human-agent teamwork experiments.\n\nSTRENGTH\nReusable components, transparent implementation, and strong experimental flexibility.\n\nEDUCATIONAL GAP\nA general-purpose framework still requires instructors to create the scenario, scaffolding, and explicit method alignment.")
			.WithFootnote("van der Waa & Haije, 2023 — MATRX, version 2.3.2 · DOI: 10.5281/zenodo.8154912")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/matrx.png",
				TutorialImagePlacement.Right, 40)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.testbed-chaopt")
			.Say("RELATED WORK — CHAOPT / OVERCOOKED! 2",
				"Bishop and colleagues used Overcooked! 2 for studying Performance and Teaming using a commercial off-the-shelf video game as its HAT environment.\n\nSTRENGTH\nA polished and engaging setting for observing coordination under time pressure.\n\nLIMITATION\nThe \"autonomous\" teammate is an experimental confederate remotely controlling the second chef.\nOvercooked! 2 is not open source, and not configurable as a reusable HAT testbed.")
			.WithFootnote("Bishop et al., 2020 — CHAOPT: A Testbed for Evaluating Human-Autonomy Team Collaboration Using Overcooked! 2 · DOI: 10.1109/SIEDS49339.2020.9106686")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/chaopt.png",
				TutorialImagePlacement.Right, 40, widthPercent: 65f)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.testbed-exoplanet-explorer")
			.Say("POSITIONING EXOPLANET EXPLORER",
				"• Mechanics derived from an Interdependence Analysis of human, UGV, and UAV capabilities\n\n• Designed for learning HAT through a ready-to-use assignment\n\n• An immersive game environment rather than abstract symbol manipulation\n\n• Open source, downloadable, cross-platform, and reproducible")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/gameIllustration.png",
				TutorialImagePlacement.Bottom, 40, widthPercent: 100f)
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();


		presentation.Step("demo.game1")
			.Say("EXOPLANET EXPLORER — THE GAME",
				"A human operator on Earth collaborates with autonomous rovers and drones deployed on an exoplanet. The goal of the mission is to locate and analyze a mysterious monolith that may hold the secrets of the universe.")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/forestmonolith_inactive.png",
				TutorialImagePlacement.Right, gap: 20, widthPercent: 12f)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilContinue();

		presentation.Step("demo.game2-base")
			.Say("ESTABLISH THE MISSION BASE",
				"The first human action is to deploy the base which contains the autonomous robots.")
			.PointTo(TutorialTargetIds.BaseDeployButton)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilTargetPressed().OrContinue();

		presentation.Step("demo.game3-place-base")
			.Say("PLACE THE BASE",
				"Sensor data from the base indicates admissible locations. Helping the human to choose a valid place to deploy the base.")
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.BuildingPlaced, IsBase).OrContinue();

		presentation.Step("demo.game4-rover")
			.Say("DEPLOY A GROUND ROVER",
				"The player can deploy rovers or drones. The rover can gather resources, deploy antennas, and analyze the monolith.")
			.PointTo(TutorialTargetIds.RoverDeployButton)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilTargetPressed().OrContinue();

		presentation.Step("demo.game5-place-rover")
			.Say("ROVER DEPLOYMENT",
				"The player receive indication of admissible deployment location, then chooses where to deploy the rover.")
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.BuildingPlaced, IsRover).OrContinue();

		presentation.Step("demo.game6-select-rover")
			.Say("SELECT THE ROVER",
				"By selecting a deployed rover. We can have access to its control panel which exposes state, sensing, resources, and available actions.")
			.PointTo(TutorialTargetIds.DeployedRover)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.RobotSelected, IsRover).OrContinue();

		presentation.Step("demo.game8-directed-command")
			.Say("DIRECTABILITY — ISSUE A COMMAND",
				"Let's explore a joint task in which the human specifies the goal while the rover plans and executes the route. By right-clicking on the map, the human can direct the rover to move to a specific location. The rover will plan a path and execute it autonomously.")
			.PointTo(TutorialTargetIds.DeployedRover)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.DirectedMoveRequested, IsRover).OrContinue();

		presentation.Step("demo.game9-autonomy-mode")
			.Say("ANOTHER LEVEL OF AUTONOMY",
				"We can also direct the rover to engage an exploration mode. In that case, the rover will choose the destination and plan the path and execute the movement towards the destination. The human shifts to directing a broader intent.")
			.PointTo(TutorialTargetIds.ExplorationModeMenu)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.ExplorationModeSelected, IsRover).OrContinue();

		presentation.Step("demo.game10-start-autonomy")
			.Say("START AUTONOMOUS EXPLORATION",
				"Start the selected behavior. Watch how the rover takes initiative now.")
			.PointTo(TutorialTargetIds.StartExplorationButton)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.ExplorationStarted, IsRover).OrContinue();

		presentation.Step("demo.game11-rover-state")
			.Say("OBSERVABILITY — MONITOR THE ROVER",
				"The selected-robot panel reports battery, carried resources, current mode, and sensor information. Those information supports interdependence and thus teaming.")
			.PointTo(TutorialTargetIds.SelectedRoverBattery)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilContinue();

		presentation.Step("demo.game12-drone")
			.Say("DEPLOY AN AERIAL DRONE",
				"Let's see the Drone now. Adding a second kind of teammate changes the set of tasks the team can accomplish.")
			.PointTo(TutorialTargetIds.DroneDeployButton)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilTargetPressed().OrContinue();

		presentation.Step("demo.game13-place-drone")
			.Say("DRONE DEPLOYMENT",
				"This one can hover over water and elevation, scout quickly, and even transport a rover.")
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.BuildingPlaced, IsDrone).OrContinue();

		presentation.Step("demo.game14-capacities")
			.Say("ENOUGH TALKING — LET'S DO THE ANALYSIS",
				"Let us now take the student's—or analyst's—perspective.\n\nThe game is not the analysis itself. It is the environment in which we observe behavior, test assumptions, and gather evidence for an Interdependence Analysis.")
			.EmphasizeBody(fontScale: 1.2f, bold: true)
			.HardPause()
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.UntilContinue();

		presentation.Step("demo.game15-IA")
			.Say("1 · START WITH A PROCEDURE AND A CAPACITY",
				"Our worksheet decomposes the procedure MOVING TO POSITION into capacities that can be assessed for every teammate and team configuration.\n\nWe begin with SELECTING A DESTINATION. The cells are intentionally blank: the student must determine who can perform this capacity, who can support it, and who cannot contribute.")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/IA-1.png",
				TutorialImagePlacement.Bottom, gap: 28, widthPercent: 96f)
			.HardPause()
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.UntilContinue();

		presentation.Step("demo.game16-IA-observe-selection")
			.Say("TEST THE ASSUMPTION IN THE GAME",
				"In this mode, the HUMAN selects a destination for a robot. Demonstrate it now by selecting the rover and choosing a destination.\n\nWatch what the robots contribute: both independently check whether the selected tile is a valid destination. That support can prevent an invalid command and improve reliability.\n\nPress Next after the demonstration to record the assessment.")
			.PointTo(TutorialTargetIds.DeployedRover)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.UntilContinue();

		presentation.Step("demo.game17-IA-selection-result")
			.Say("2 · RECORD CAPACITY TO PERFORM AND SUPPORT",
				"For Team Alternative 1, the HUMAN is GREEN: they can perform destination selection.\n\nThe UGV and UAV are YELLOW: they support the task by independently validating the destination tile. This redundant check improves reliability.\n\nGREEN = capacity to perform\nYELLOW = capacity to support\nRED = no capacity in this configuration")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/IA-2.png",
				TutorialImagePlacement.Bottom, gap: 28, widthPercent: 96f)
			.HardPause()
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.UntilContinue();

		presentation.Step("demo.game18-IA-path")
			.Say("3 · ASSESS THE NEXT CAPACITY",
				"The selected destination is a goal—not a route. The robots can optimize and execute a path to that goal, so they are GREEN for OPTIMIZING PATH.\n\nThe human is YELLOW in Team Alternative 1: selecting the goal and monitoring execution supports the robots' planning, but the human does not compute the path.")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/IA-3.png",
				TutorialImagePlacement.Bottom, gap: 28, widthPercent: 96f)
			.HardPause()
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.UntilContinue();

		presentation.Step("demo.game19-IA-terrain-question")
			.Say("4 · DECOMPOSE UNTIL DIFFERENCES BECOME VISIBLE",
				"A generic capacity to MOVE would hide important differences between teammates. We therefore assess movement through grass, trees, cliffs, and mud separately.\n\nThese blank rows are hypotheses to test—not answers to guess from the robot names.")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/IA-4.png",
				TutorialImagePlacement.Bottom, gap: 28, widthPercent: 96f)
			.HardPause()
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.UntilContinue();

		presentation.Step("demo.game20-IA-observe-terrain")
			.Say("COMPARE THE TEAMMATES IN CONTEXT",
				"Return to the game and compare the UGV and UAV against the visible terrain.\n\n• Which robot can traverse grass and trees?\n• Which can cross cliffs or mud?\n• Can one teammate support another where it cannot perform alone?\n\nPress Next once the evidence is clear.")
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.UntilContinue();

		presentation.Step("demo.game21-IA-completed-example")
			.Say("5 · COMPLETE THE EVIDENCE-BASED ASSESSMENT",
				"The worked assessment now exposes complementary capabilities. In Team Alternative 2, the UGV can move through trees but not over cliffs; the UAV can cross cliffs and mud but not trees. The UGV can still support movement through mud.\n\nThe color pattern makes interdependence visible: team composition changes what the joint system can accomplish and where support is required.")
			.WithImage("res://scenes/ui/tutorial/scripts/imagesForPresentation/IA-5.png",
				TutorialImagePlacement.Bottom, gap: 28, widthPercent: 96f)
			.HardPause()
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.UntilContinue();

		presentation.Step("demo.game15-drone-autonomy")
			.Say("COMPLEMENTARY CAPABILITIES",
				"The drone's mobility allows for a team dynamic where the rover explore and identify targets of interest, then the rover can be deployed to analyze them.")
			.PointTo(TutorialTargetIds.ExplorationModeMenu)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.ExplorationModeSelected, IsDrone).OrContinue();

		presentation.Step("demo.game15-start-aerial-scouting")
			.Say("AERIAL SCOUTING",
				"Let's see if we can identify something interesting by launching the drone in random exploration. It uses a greedy search strategy to reveal map terrain.")
			.PointTo(TutorialTargetIds.StartExplorationButton)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.DroneScoutStarted, IsDrone).OrContinue();

		presentation.Step("demo.game16-team-configurations")
			.Say("CONFIGURABLE TEAMS",
				"Scenarios can vary the map and team composition: one human can coordinate multiple rovers and drones with different capability combinations.")
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilContinue();

		presentation.Step("demo.game17-partial-observability")
			.Say("THE ANOMALY SENSOR",
				"The monolith is known to produce disturbances in the planetary gravitational field. The robots carry an anomaly radar that can be used by the human to understand directions of interest.")
			.PointTo(TutorialTargetIds.MinimapContainer)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilContinue();

		presentation.Step("demo.game20-mission-monitoring")
			.Say("MONITOR THE MISSION",
				"The mission display brings together time, resources, and progress. The operator must monitor the team and mission as a whole, as well as individual robots when needed.")
			.PointTo(TutorialTargetIds.StatusPanel)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilContinue();

		presentation.Step("demo.game21-select-drone-teamwork")
			.Say("PREPARE A TEAMING ACTION",
				"Select the drone and position it above the rover. When the Lift Robot control becomes available, press Next. This creates a concrete interdependence: neither robot can complete the transport task alone.")
			.PointTo(TutorialTargetIds.DeployedDrone)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.UntilContinue();

		presentation.Step("demo.game22-lift-rover")
			.Say("ROVER–DRONE INTERDEPENDENCE",
				"When the drone is above the rover, use Lift Robot. The drone supplies mobility across elevation; the rover supplies the surface capabilities needed at the destination.")
			.PointTo(TutorialTargetIds.LiftRobotButton)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight)
			.Until(TutorialEvent.RobotLiftRequested, IsDrone).OrContinue();

		presentation.Step("demo.game23-hat-requirements")
			.Say("WHAT THE GAME MAKES VISIBLE",
				"OBSERVABILITY — perceive teammate state and knowledge\n\nPREDICTABILITY — anticipate actions and consequences\n\nDIRECTABILITY — influence behavior at the right time\n\nThese are the requirements students derive from the activity they have just observed.")
			.PointTo(TutorialTargetIds.MinimapContainer)
			.GuideAction().UndimBackground()
			.PlaceCallout(TutorialCalloutPlacement.TopRight).UntilContinue();

		presentation.Step("demo.design-chain")
			.Say("INTERDEPENDENCE ANALYSIS AS A DESIGN CHAIN",
				"1 · JOINT ACTIVITY\nBreak down tasks and their relationships\n\n→  2 · CAPABILITIES\nWho can do what, under which constraints?\n\n→  3 · INTERDEPENDENCIES\nWhere do actors constrain or complement one another?\n\n→  4 · HAT REQUIREMENTS\nObservability · Predictability · Directability\n\nEXPECTED OUTCOME\nMove from observed behavior to a traceable rationale for information, coordination, and control needs in the interface.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.course-implementation")
			.Say("COURSE IMPLEMENTATION",
				"• Graduate human factors course focused on HAT — Fall 2025\n• 24 students completed an Interdependence Analysis assignment using the game\n• Tasks: model the work, assess robot capabilities, identify teaming requirements, and propose interface improvements\n\nEXPLORATORY EVALUATION — 7 VOLUNTEERS (CER-2526-19-D)\n\nDuring the assignment — n = 6\nMetacognitive regulation, analysis strategies, and resources used\n\nAfter the assignment — n = 7\nSelf-reports, perceived learning, and areas for improvement\n\nLikert scales and open-ended responses were analyzed using inductive qualitative content analysis. Results concern reported perceptions and reasoning, not a causal measure of performance.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.results")
			.Say("RESULTS", "Questionnaires — Fall 2025")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.metacognitive-regulation")
			.Say("DURING THE ASSIGNMENT — METACOGNITIVE REGULATION",
				"CHECKING THE CONSISTENCY OF THE ANALYSIS\n\n• 4/6 often used in-game robot behavior to adjust their decisions.\n\nPRIMARY RESOURCE USED\n\n• 4/6 — direct exploration of the game\n• 2/6 — course notes\n• 1/6 — discussions with other students\n\nOpen-ended responses consistently describe an iterative cycle:\n\nHYPOTHESIS → IN-GAME TEST → DIAGRAM/TABLE REVISION\n\nP3: “Direct experience with the game helps to clear any uncertainty about task capabilities.”\nP5: “I can test scenarios to verify hypotheses.”")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.revisions")
			.Say("DURING THE ASSIGNMENT — EXAMPLES OF REVISIONS",
				"A recurring example among four participants (P1, P4, P5, P6):\n\n• Initial misunderstanding: can the human player execute robot movements, or only order them?\n• In-game test: only robots execute movements.\n• Result: correction of the task allocation.\n\nAnother participant tested the probability of getting stuck in mud and verified the one-in-ten chance in the game.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.perceived-learning")
			.Say("AFTER THE ASSIGNMENT — PERCEIVED LEARNING",
				"ALL 7 PARTICIPANTS AGREED OR STRONGLY AGREED THAT THE ACTIVITY HELPED THEM:\n\n• Better represent human–autonomy interactions — 4 agree, 3 strongly agree\n• Understand task interdependence — 3 agree, 4 strongly agree\n• Understand interface issues for teaming — 1 agree, 6 strongly agree\n• Analyze the reciprocal effects of human and agent actions — 3 agree, 4 strongly agree\n• Feel comfortable analyzing a complex situation involving an autonomous agent — 4 agree, 3 strongly agree\n\n6/7 identified iteration as essential; 6/7 described learning interdependence through direct involvement.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.conceptual-gains")
			.Say("CONCEPTUAL GAINS… AND PERSISTENT AMBIGUITY",
				"WHAT THE OPEN-ENDED RESPONSES SUGGEST\n\n• 6/7 described gaining an understanding of interdependence through direct involvement.\n• Definitions emphasized complementary capabilities and mutual constraints between teammates.\n• Distinctions between autonomy and automation generally drew on initiative and adaptability.\n• Some ambiguity remained around this distinction, indicating a need for explicit instructional scaffolding.\n\nCAUTIOUS INTERPRETATION\n\nReported verification behaviors converge with perceived learning, but there was no pretest, control group, or objective measure of transfer.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.student-voices")
			.Say("WHAT STUDENTS SAY",
				"“The game allowed me to become one of the entities that must collaborate. I was an active participant and could see that my actions had consequences [...] This immersion allowed me to experience interdependence, rather than merely analyze it as an observer.” — P1\n\n“For me, the only way to verify my answers was to play the game and test several use cases for the task being analyzed.” — P7\n\n“Using a video game is a very good idea because I find that it makes the interactions feel less artificial.” — P1")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.strengths-and-improvements")
			.Say("STRENGTHS & AREAS FOR IMPROVEMENT",
				"IDENTIFIED STRENGTHS\n\n• Well-received quality and retro atmosphere; strong educational potential\n• Balanced human/robot capabilities that create natural collaboration\n• Simple interface and progressively more difficult levels\n\nPRIORITY IMPROVEMENTS\n\n• Missing in-game tutorial → added to the current version\n• Unclear resource system → interface clarified\n• Poorly placed error messages → repositioned and made more visible\n• Unclear anomaly histogram and map → visual redesign\n\nEDUCATIONAL SUGGESTIONS\n\nPlay in class; use a two-stage familiarization-to-analysis assignment; clarify the player's role.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.contribution")
			.Say("CONTRIBUTION TO THE HAT COMMUNITY",
				"• AN EDUCATIONAL TOOL ALIGNED WITH A METHOD\nGame mechanics make Interdependence Analysis constructs observable and open to discussion.\n\n• A BRIDGE BETWEEN THEORY AND BEHAVIOR\nStudents test a hypothesis, observe actual agent capabilities, and revise their model.\n\n• A CONFIGURABLE SCENARIO\nMaps, team composition, and levels of autonomy vary the coordination problems.\n\n• AN OPEN, CROSS-PLATFORM RESOURCE\nReusable for teaching and potentially as a starting point for HAT research.\n\nCORE CONTRIBUTION\nSupporting the translation of observable system behavior into a reasoned human–autonomy teaming requirement.")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.conclusion")
			.Say("CONCLUSION",
				"A tangible environment that promotes reflective revision of a complex human–robot activity analysis.\n\n• Exploratory data suggest support for hypothesis testing, model revision, and perceived understanding of interdependence.\n• Limitations: small volunteer sample, one course, primarily self-reported measures, and no experimental comparison.\n• Next steps: larger cohorts, objective measures of transfer, and evaluation of changes made to the game.\n\nOpen-source game and repository\nZenodo DOI: 10.5281/zenodo.18651448\n\nWe welcome collaborations!")
			.WithImage("res://assets/HAT_game_logo_cropped.png")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

		presentation.Step("demo.references")
			.Say("KEY REFERENCES",
				"• Cooke et al. (2020) — Human–Autonomy Teaming\n• Johnson et al. (2011) — Coactive design and Interdependence Analysis\n• Johnson et al. (2009) — Joint Activity Testbed: Blocks World for Teams (BW4T). DOI: 10.1007/978-3-642-10203-5_26\n• Hellar & McNeese (2010) — NeoCITIES\n• Bishop et al. (2020) — CHAOPT: A Testbed for Evaluating Human–Autonomy Team Collaboration Using the Video Game Overcooked! 2. DOI: 10.1109/SIEDS49339.2020.9106686\n• van der Waa & Haije (2023) — MATRX: Human-Agent Teaming Rapid Experimentation software. DOI: 10.5281/zenodo.8154912\n• NASA Langley Research Center (2024) — HAT Task Battery\n• Smith et al. (2024) — Challenges of educational HAT platforms\n\nEXOPLANET EXPLORER\n10.5281/zenodo.18651448")
			.PlaceCallout(TutorialCalloutPlacement.FullScreen)
			.HardPause().UntilContinue();

	}
}
