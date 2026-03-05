using System;
using System.Collections.Generic;
using System.Linq;
using Prog7312PoePart1.DataStructures;
using Prog7312PoePart1.Models;

namespace Prog7312PoePart1.Services
{
    public class ServiceRequestManager
    {
        private readonly List<ServiceRequest> _requests;

        public ServiceRequestManager(List<ServiceRequest> requests)
        {
            _requests = requests ?? new List<ServiceRequest>();
        }

        // ---------------------------
        // Filter requests by search, status, category, priority
        // ---------------------------
        public IEnumerable<ServiceRequest> FilterRequests(string search, string status, string category, string priority)
        {
            var query = _requests.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(r =>
                    r.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    r.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<RequestStatus>(status, true, out var st))
                query = query.Where(r => r.Status == st);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(priority) && int.TryParse(priority, out var p))
                query = query.Where(r => r.Priority == p);

            return query;
        }

        // ---------------------------
        // Sort requests
        // ---------------------------
        public IEnumerable<ServiceRequest> SortRequests(IEnumerable<ServiceRequest> data, string sort)
        {
            var list = data.ToList();

            switch (sort)
            {
                case "createdAsc":
                    {
                        var avl = new AVLTree<DateTime, ServiceRequest>();
                        foreach (var r in list) avl.Insert(r.CreatedAt, r);
                        return avl.InOrder();
                    }
                case "createdDesc":
                    {
                        var avl = new AVLTree<DateTime, ServiceRequest>();
                        foreach (var r in list) avl.Insert(r.CreatedAt, r);
                        return avl.InOrder().Reverse();
                    }
                case "priorityAsc":
                    {
                        var heap = new MinHeap<ServiceRequest>();
                        foreach (var r in list) heap.Insert(r.Priority, r);
                        var result = new List<ServiceRequest>();
                        while (heap.Count > 0) result.Add(heap.Pop().Item);
                        return result;
                    }
                case "priorityDesc":
                    {
                        var heap = new MinHeap<ServiceRequest>();
                        foreach (var r in list) heap.Insert(r.Priority, r);
                        var result = new List<ServiceRequest>();
                        while (heap.Count > 0) result.Add(heap.Pop().Item);
                        return result.OrderByDescending(r => r.Priority).ToList();
                    }
                default:
                    return list;
            }
        }

        // ---------------------------
        // Build a graph based on category/location similarity
        // ---------------------------
        public Graph<Guid> BuildRequestGraph(IEnumerable<ServiceRequest> data)
        {
            var list = data.ToList();
            var graph = new Graph<Guid>();

            foreach (var r in list) graph.AddVertex(r.Id);

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    var a = list[i];
                    var b = list[j];
                    double weight = 1000;

                    if (string.Equals(a.Category, b.Category, StringComparison.OrdinalIgnoreCase))
                        weight -= 500;

                    if (!string.IsNullOrWhiteSpace(a.Location) && !string.IsNullOrWhiteSpace(b.Location))
                    {
                        var tokensA = a.Location.ToLower().Split(' ', ',', '-');
                        var tokensB = b.Location.ToLower().Split(' ', ',', '-');
                        weight -= tokensA.Intersect(tokensB).Count() * 10;
                    }

                    if (weight < 1000)
                        graph.AddEdge(a.Id, b.Id, Math.Max(1, weight));
                }
            }

            return graph;
        }

        // ---------------------------
        // Get related requests (same category)
        // ---------------------------
        public IEnumerable<ServiceRequest> GetDependencies(ServiceRequest request)
        {
            return _requests.Where(r => r.Category == request.Category && r.Id != request.Id);
        }

        public ServiceRequest? GetTopPriority() => _requests.OrderBy(r => r.Priority).FirstOrDefault();
    }
}
