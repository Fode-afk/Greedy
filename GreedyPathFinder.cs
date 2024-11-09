using System.Collections.Generic;
using Greedy.Architecture;
using System.Linq;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
	public List<Point> FindPathToCompleteGoal(State state)
	{
		var pathFinder = new DijkstraPathFinder();
		if (state.Chests.Count < state.Goal || state.Chests.Count < state.Goal)
			return new List<Point>();

		var fromStartToChest = pathFinder
            .GetPathsByDijkstra(state, state.Position, state.Chests)
			.FirstOrDefault();

		if (fromStartToChest == null)
            return new List<Point>();

        var visitedChests = new HashSet<Point> { fromStartToChest.End };
        var shortestPaths = new List<PathWithCost> { fromStartToChest };

		if (state.Chests.Any(chest => chest == state.Position))
			visitedChests.Add(state.Position);

        fromStartToChest.Path.RemoveAt(0);

        for (int i = 0; i < state.Goal - 1; i++)
		{
			var fromChestToChest = pathFinder
                .GetPathsByDijkstra(state,
					shortestPaths[i].End,
					state.Chests.Where(chest => chest != shortestPaths[i].End && !visitedChests.Contains(chest)))				
				.FirstOrDefault();

			if (fromChestToChest == null)
				return new List<Point>();

			visitedChests.Add(fromChestToChest.End);
			fromChestToChest.Path.RemoveAt(0);
			shortestPaths.Add(fromChestToChest);
		}

		var finalCost = shortestPaths
			.Sum(path => path.Cost);

		if (finalCost > state.Energy)
			return new List<Point>();

        return shortestPaths
			.SelectMany(path => path.Path)
			.ToList();			
	}
}