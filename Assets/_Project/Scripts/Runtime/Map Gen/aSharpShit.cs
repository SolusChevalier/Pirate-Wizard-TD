using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public static class aSharpShit
{
}

public interface WeightedGraph<Coord>
{
    double Cost(CoordStruct a, CoordStruct b);

    IEnumerable<CoordStruct> Neighbors(CoordStruct id);
}

public class SquareGrid : WeightedGraph<CoordStruct>
{
    public static readonly CoordStruct[] DIRS = new[]
        {
            new CoordStruct(1, 0),
            new CoordStruct(0, -1),
            new CoordStruct(-1, 0),
            new CoordStruct(0, 1)
        };

    public int width, height;
    public HashSet<CoordStruct> walls = new HashSet<CoordStruct>();
    public HashSet<CoordStruct> forests = new HashSet<CoordStruct>();

    public SquareGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public bool InBounds(CoordStruct id)
    {
        return 0 <= id.x && id.x < width
            && 0 <= id.y && id.y < height;
    }

    public bool Passable(CoordStruct id)
    {
        return !walls.Contains(id);
    }

    public double Cost(CoordStruct a, CoordStruct b)
    {
        return forests.Contains(b) ? 5 : 1;
    }

    public IEnumerable<CoordStruct> Neighbors(CoordStruct id)
    {
        foreach (var dir in DIRS)
        {
            CoordStruct next = new CoordStruct(id.x + dir.x, id.y + dir.y);
            if (InBounds(next) && Passable(next))
            {
                yield return next;
            }
        }
    }
}

public class PriorityQueue<TElement, TPriority>
{
    private List<Tuple<TElement, TPriority>> elements = new List<Tuple<TElement, TPriority>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(TElement item, TPriority priority)
    {
        elements.Add(Tuple.Create(item, priority));
    }

    public TElement Dequeue()
    {
        Comparer<TPriority> comparer = Comparer<TPriority>.Default;
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (comparer.Compare(elements[i].Item2, elements[bestIndex].Item2) < 0)
            {
                bestIndex = i;
            }
        }

        TElement bestItem = elements[bestIndex].Item1;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

public class AStarSearch
{
    public Dictionary<CoordStruct, CoordStruct> cameFrom
        = new Dictionary<CoordStruct, CoordStruct>();

    public Dictionary<CoordStruct, double> costSoFar
        = new Dictionary<CoordStruct, double>();

    public List<CoordStruct> path = new List<CoordStruct>();

    public static double Heuristic(CoordStruct a, CoordStruct b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public AStarSearch(WeightedGraph<CoordStruct> graph, CoordStruct start, CoordStruct goal)
    {
        var frontier = new PriorityQueue<CoordStruct, double>();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current.Equals(goal))
            {
                break;
            }

            foreach (var next in graph.Neighbors(current))
            {
                double newCost = costSoFar[current]
                    + graph.Cost(current, next);
                if (!costSoFar.ContainsKey(next)
                    || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    double priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
        //make path
        CoordStruct currentPath = goal;
        while (!currentPath.Equals(start))
        {
            path.Add(currentPath);
            currentPath = cameFrom[currentPath];
        }
        path.Add(start);
        path.Reverse();
    }
}