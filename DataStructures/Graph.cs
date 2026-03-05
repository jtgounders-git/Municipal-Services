using System;
using System.Collections.Generic;
using System.Linq;

namespace Prog7312PoePart1.DataStructures
{
    // Represents a generic, undirected, weighted graph.
    // Uses an adjacency list where each vertex maps to a list of its neighbors and edge weights.
    public class Graph<T>
    {
        // Internal adjacency list: each vertex maps to a list of (neighbor, weight) pairs.
        private readonly Dictionary<T, List<(T neighbor, double weight)>> _adj = new();

        // Adds a vertex to the graph if it does not already exist.
        public void AddVertex(T v)
        {
            if (!_adj.ContainsKey(v))
                _adj[v] = new List<(T, double)>();
        }

        // Adds an undirected weighted edge between vertices A and B.
        // Automatically adds the vertices if they don't exist.
        public void AddEdge(T a, T b, double weight = 1)
        {
            AddVertex(a);
            AddVertex(b);
            _adj[a].Add((b, weight));
            _adj[b].Add((a, weight)); // Since this is an undirected graph
        }

        // Performs a Breadth-First Search (BFS) starting from a given vertex.
        // Returns the vertices in the order they were visited.
        public IEnumerable<T> BFS(T start)
        {
            var visited = new HashSet<T>(); // Keeps track of visited nodes
            var queue = new Queue<T>();     // Queue to hold nodes for processing

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current; // Return the current node (lazy enumeration)

                // Visit all unvisited neighbors
                foreach (var (nb, _) in _adj[current])
                {
                    if (!visited.Contains(nb))
                    {
                        visited.Add(nb);
                        queue.Enqueue(nb);
                    }
                }
            }
        }

        // Computes the Minimum Spanning Tree (MST) using Prim's Algorithm.
        // Returns a list of edges (A, B, Weight) that make up the MST.
        public List<(T a, T b, double w)> PrimMST()
        {
            var result = new List<(T a, T b, double w)>(); // Stores MST edges
            if (_adj.Count == 0) return result;

            var start = _adj.Keys.First(); // Start from any vertex
            var visited = new HashSet<T> { start };

            // Priority queue (sorted set) for edges, sorted by smallest weight first
            var edgeHeap = new SortedSet<(double w, T a, T b)>(
                Comparer<(double w, T a, T b)>.Create((x, y) =>
                {
                    int cmp = x.w.CompareTo(y.w); // Compare weights first
                    if (cmp != 0) return cmp;

                    // Tie-breaker to ensure consistent ordering (avoid duplicates)
                    int hashX = x.a.GetHashCode() ^ x.b.GetHashCode();
                    int hashY = y.a.GetHashCode() ^ y.b.GetHashCode();
                    return hashX.CompareTo(hashY);
                })
            );

            // Add all edges from the start vertex to the priority queue
            foreach (var (nb, w) in _adj[start])
                edgeHeap.Add((w, start, nb));

            // Continue until all vertices are included or no edges remain
            while (edgeHeap.Count > 0)
            {
                var smallest = edgeHeap.First();
                edgeHeap.Remove(smallest);
                var (w, a, b) = smallest;

                // Skip edges leading to already visited vertices
                if (visited.Contains(b)) continue;

                // Add this edge to the MST
                visited.Add(b);
                result.Add((a, b, w));

                // Add new edges from the newly visited vertex
                foreach (var (nb, nw) in _adj[b])
                    if (!visited.Contains(nb))
                        edgeHeap.Add((nw, b, nb));
            }

            return result;
        }

        // Returns all vertices in the graph
        public IEnumerable<T> Vertices => _adj.Keys;

        // Returns all neighbors (and weights) of a given vertex
        public IEnumerable<(T neighbor, double w)> Neighbors(T v)
        {
            if (_adj.ContainsKey(v))
            {
                foreach (var (neighbor, weight) in _adj[v])
                    yield return (neighbor, weight);
            }
        }
    }
}
