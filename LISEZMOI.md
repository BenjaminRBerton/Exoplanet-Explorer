# Exoplanet Explorer

Exoplanet Explorer est un jeu éducatif libre consacré à la collaboration humain–autonomie (*Human–Autonomy Teaming*, HAT).

Le joueur supervise une mission d'exploration sur une exoplanète inconnue. Il coordonne des rovers et des drones semi-autonomes afin de cartographier le terrain, maintenir le réseau de communication, gérer l'énergie, recueillir des ressources et retrouver un mystérieux monolithe.

## Jouer

Téléchargez l'archive correspondant à votre système sur la page [GitHub Releases](https://github.com/Bertonlome/Human-Autonomy-Teaming-Game/releases), puis extrayez-la au complet avant de lancer le jeu. L'exécutable et le fichier `.pck` doivent rester dans le même dossier.

Le menu principal donne accès aux tutoriels guidés, aux règles du jeu et à la liste complète des commandes. Un clavier et une souris sont recommandés.

## Objectifs pédagogiques

Le jeu peut notamment servir à étudier :

- l'analyse d'interdépendance et la conception coactive;
- la répartition des tâches entre l'humain et les robots;
- les contraintes de coordination et de communication;
- l'autonomie adaptable; et
- la conception d'interfaces pour les équipes humain–autonomie.

## Compiler le projet

Le projet nécessite [Godot 4.3 avec prise en charge de .NET](https://godotengine.org/download/archive/4.3-stable/) et le [SDK .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/Bertonlome/Human-Autonomy-Teaming-Game.git
cd Human-Autonomy-Teaming-Game
dotnet restore HAT_Game.sln
dotnet build HAT_Game.sln
```

Ouvrez ensuite `project.godot` dans l'éditeur Godot.

## Intégration Gemini facultative

L'interface de création de parcours peut demander des suggestions stratégiques à l'API Google Gemini. Cette fonction est facultative; le reste du jeu fonctionne sans clé API. Une requête transmet à Google le parcours dessiné, les coordonnées, le contexte du terrain, les noms des robots et les annotations saisies par le joueur.

## Citation et licence

Les versions archivées sont disponibles sur [Zenodo](https://doi.org/10.5281/zenodo.18651448). Les métadonnées de citation se trouvent dans [`CITATION.cff`](CITATION.cff).

Le code original est distribué sous [licence MIT](LICENSE). Les ressources provenant de tiers demeurent assujetties aux conditions de leurs auteurs respectifs.

La documentation principale et les consignes de contribution se trouvent dans le [README anglais](readme.md).
