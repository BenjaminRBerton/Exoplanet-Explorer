[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.18651448.svg)](https://doi.org/10.5281/zenodo.18651448)

# Exoplanet Explorer

An open-source educational game for teaching and studying human–autonomy teaming (HAT).

As the supervisor of a remote planetary mission, the player coordinates ground rovers and aerial drones to explore an unknown environment, manage energy and communications, gather resources, and locate a mysterious monolith. The game makes task allocation, interdependence, coordination breakdowns, and interface transparency directly observable through play.

## Download and play

Download the archive for your platform from the [GitHub Releases](https://github.com/Bertonlome/Human-Autonomy-Teaming-Game/releases) page, extract the complete archive, and launch Exoplanet Explorer. Keep the executable and its `.pck` file together.

The main menu includes guided tutorials, game rules, and a complete controls reference. A keyboard and mouse are recommended. The game supports Windows, macOS, and Linux desktop builds.

## Educational use

Exoplanet Explorer can support activities in:

- coactive design and interdependence analysis;
- human–robot task allocation;
- coordination and communication constraints;
- adaptable autonomy; and
- interface design for human–autonomy teams.

Typical classroom work asks learners to play a representative mission, assess the robots' capabilities, identify coordination requirements, and propose interface improvements grounded in HAT principles.

## Build from source

Prerequisites:

- [Godot Engine 4.3 with .NET support](https://godotengine.org/download/archive/4.3-stable/); and
- the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

Clone and restore the project:

```bash
git clone https://github.com/Bertonlome/Human-Autonomy-Teaming-Game.git
cd Human-Autonomy-Teaming-Game
dotnet restore HAT_Game.sln
```

Open `project.godot` in the Godot editor and run the project. A command-line build can be checked with:

```bash
dotnet build HAT_Game.sln
```

## Optional Gemini integration

The custom-path interface can request strategic path suggestions from the Google Gemini API. This feature is optional; the rest of the game does not require an API key.

Create a key in [Google AI Studio](https://aistudio.google.com/app/apikey), then enter it through the in-game API-key dialog or set the `GEMINI_API_KEY` environment variable before starting the game. Requests include the painted route, map coordinates, nearby terrain context, robot names, and player annotations. The key is stored locally when entered in the game; do not commit it to this repository or share it in bug reports.

## Research and citation

Archived releases are available from [Zenodo](https://doi.org/10.5281/zenodo.18651448).

If you use the software for research or teaching, cite:

> Berton, B., & Doyon-Poulin, P. (2026). *Exoplanet Explorer (Version 1.0.1)* [Computer software]. Zenodo. https://doi.org/10.5281/zenodo.18651448

Citation metadata is also provided in [`CITATION.cff`](CITATION.cff).

## Contributing

Bug reports, pedagogical feedback, and focused pull requests are welcome through [GitHub Issues](https://github.com/Bertonlome/Human-Autonomy-Teaming-Game/issues). Please open an issue before beginning a major feature change.

## License

Original project code is released under the [MIT License](LICENSE). Third-party artwork, fonts, sound effects, and music remain subject to their respective authors' terms. See [Third-Party Notices](THIRD_PARTY_NOTICES.md)

## Authors

Benjamin Berton and Philippe Doyon-Poulin, Polytechnique Montréal
