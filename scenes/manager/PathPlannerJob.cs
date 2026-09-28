using System;
using System.Collections.Generic;
using System.Linq;
using Game.Component;
using Godot;

namespace Game.Manager;

/// <summary>
/// A resumable A* planner job that can be advanced a few iterations per frame.
/// Contains no Godot-specific nodes beyond using GridManager for move checks.
/// </summary>
public class PathPlannerJob
{
    private readonly GridManager gridManager;
    private readonly BuildingComponent robot;
    private readonly bool allowBridges;
    private readonly bool? bridgeElevationIsElevated;
    private readonly HashSet<Vector2I> excludedPositions;
    private readonly IReadOnlySet<Vector2I> knownTiles;
    private readonly bool allowUnknownTiles;
    private readonly Vector2I start;
    private readonly Vector2I target;

    private readonly PriorityQueue<PathNode, int> open = new();
    private readonly Dictionary<Vector2I, int> bestCosts = new();
    private readonly HashSet<Vector2I> closed = new();

    public bool Completed { get; private set; } = false;
    public List<Vector2I> Result { get; private set; } = null;
    public Action<List<Vector2I>> OnComplete { get; }

    public PathPlannerJob(
        GridManager gridManager,
        BuildingComponent robot,
        Vector2I start,
        Vector2I target,
        bool allowBridges,
        bool? bridgeElevationIsElevated,
        HashSet<Vector2I> excludedPositions,
        IReadOnlySet<Vector2I> knownTiles,
        bool allowUnknownTiles,
        Action<List<Vector2I>> onComplete)
    {
        this.gridManager = gridManager ?? throw new ArgumentNullException(nameof(gridManager));
        this.robot = robot;
        this.start = start;
        this.target = target;
        this.allowBridges = allowBridges;
        this.bridgeElevationIsElevated = bridgeElevationIsElevated;
        this.excludedPositions = excludedPositions;
        this.knownTiles = knownTiles ?? throw new ArgumentNullException(nameof(knownTiles));
        this.allowUnknownTiles = allowUnknownTiles;
        this.OnComplete = onComplete;

        var startNode = new PathNode(start, null, 0, Heuristic(start, target));
        open.Enqueue(startNode, startNode.F);
        bestCosts[start] = 0;
    }

    public void Step(int maxIterations)
    {
        if (Completed) return;
		if (gridManager.IsKnownDestinationOnDifferentElevation(
			robot,
			start,
			target,
			knownTiles))
		{
			Completed = true;
			Result = null;
			OnComplete?.Invoke(null);
			return;
		}

        int iterations = 0;
        while (open.Count > 0 && iterations < maxIterations)
        {
            iterations++;

            var current = open.Dequeue();
            if (closed.Contains(current.Position) ||
                !bestCosts.TryGetValue(current.Position, out int bestCost) ||
                current.G != bestCost)
            {
                continue;
            }
            closed.Add(current.Position);

            if (current.Position == target)
            {
                // reconstruct
                var path = new List<Vector2I>();
                var node = current;
                while (node != null)
                {
                    path.Add(node.Position);
                    node = node.Parent;
                }
                path.Reverse();
                Result = path;
                Completed = true;
                OnComplete?.Invoke(Result);
                return;
            }

            var neighbors = new[]
            {
                new Vector2I(current.Position.X, current.Position.Y - 1), // Up
                new Vector2I(current.Position.X, current.Position.Y + 1), // Down
                new Vector2I(current.Position.X - 1, current.Position.Y), // Left
                new Vector2I(current.Position.X + 1, current.Position.Y)  // Right
            };

            foreach (var neighborPos in neighbors)
            {
                if (closed.Contains(neighborPos)) continue;
                if (excludedPositions != null && excludedPositions.Contains(neighborPos)) continue;

                Rect2I originArea = new Rect2I(current.Position, Vector2I.One);
                Rect2I destinationArea = new Rect2I(neighborPos, Vector2I.One);

                if (!gridManager.IsNavigationStepTraversable(
                    robot,
                    originArea,
                    destinationArea,
                    knownTiles,
                    allowBridges,
                    bridgeElevationIsElevated,
                    allowUnknownTiles))
                {
                    continue;
                }

                int movementCost = 1;
                bool stepIsKnown = originArea.ToTiles().All(knownTiles.Contains) &&
                    destinationArea.ToTiles().All(knownTiles.Contains);
                if (allowBridges && stepIsKnown &&
                    !gridManager.IsBuildingMovable(robot, originArea, destinationArea))
                {
                    // Prefer a longer dry route over consuming additional wood.
                    movementCost += 10;
                }

                int gCost = current.G + movementCost;
                int hCost = Heuristic(neighborPos, target);

                if (bestCosts.TryGetValue(neighborPos, out int existingCost) &&
                    gCost >= existingCost)
                {
                    continue;
                }

                bestCosts[neighborPos] = gCost;
                var neighbor = new PathNode(neighborPos, current, gCost, hCost);
                open.Enqueue(neighbor, neighbor.F);
            }
        }

        // If we exhausted open list without finding target, mark completed with null result
        if (open.Count == 0 && !Completed)
        {
            Completed = true;
            Result = null;
            OnComplete?.Invoke(null);
        }
    }

    private int Heuristic(Vector2I from, Vector2I to)
    {
        return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
    }

    private class PathNode
    {
        public Vector2I Position;
        public PathNode Parent;
        public int G;
        public int H;
        public int F => G + H;

        public PathNode(Vector2I pos, PathNode parent, int g, int h)
        {
            Position = pos;
            Parent = parent;
            G = g;
            H = h;
        }
    }
}
