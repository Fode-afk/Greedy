using Greedy.Architecture;
using System.Collections.Generic;
using System.Linq;
namespace Greedy;

public class DijkstraPathFinder
{
    class DijkstraData
    {
        public Point? Previous { get; set; }
        public int Price { get; set; }
    }

    public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
		IEnumerable<Point> targets)
	{
        var priorityQueue = new PriorityQueue<Point, int>();
        priorityQueue.Enqueue(start, 0);
    
        var track = new Dictionary<Point, DijkstraData>()
        {
            [start] = new DijkstraData { Price = 0, Previous = null }
        };       

        while (priorityQueue.Count > 0)
        {
            Point? toOpen = priorityQueue.Dequeue();

            if (targets.Any(target => target == toOpen.Value))
            {
                var result = new List<Point>();
                var end = toOpen.Value;
                var finalPrice = track[end].Price;
                
                while (end != start)
                {
                    result.Add(end);
                    end = track[end].Previous.Value;
                }
                result.Add(start);
                result.Reverse();
                yield return new PathWithCost(finalPrice, result.ToArray());
            }

            var neighbors = new List<Point>
            {
                new Point(toOpen.Value.X + 1, toOpen.Value.Y),
                new Point(toOpen.Value.X - 1, toOpen.Value.Y),
                new Point(toOpen.Value.X, toOpen.Value.Y + 1),
                new Point(toOpen.Value.X, toOpen.Value.Y - 1)
            };

            foreach (var neighbor in neighbors)
            {
                if (!state.InsideMap(neighbor) || state.IsWallAt(neighbor))
                    continue;

                int newPrice = track[toOpen.Value].Price + state.CellCost[neighbor.X, neighbor.Y];
                if (!track.ContainsKey(neighbor) || track[neighbor].Price > newPrice)
                { 
                    track[neighbor] = new DijkstraData { Previous = toOpen, Price = newPrice };
                    priorityQueue.Enqueue(neighbor, newPrice);
                }            
            }
        }
    }
}