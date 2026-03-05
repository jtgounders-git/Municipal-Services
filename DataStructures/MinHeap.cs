using System;
using System.Collections.Generic;

namespace Prog7312PoePart1.DataStructures
{
    // A simple Min-Heap (priority queue) where smaller priority numbers come first.
    public class MinHeap<T>
    {
        // Stores items with their priority values
        private readonly List<(int Priority, T Item)> _data = new();

        // Number of items in the heap
        public int Count => _data.Count;

        // Adds a new item with a given priority (lower number = higher priority)
        public void Insert(int priority, T item)
        {
            _data.Add((priority, item));
            HeapifyUp(_data.Count - 1);
        }

        // Removes and returns the item with the lowest priority value
        public (int Priority, T Item) Pop()
        {
            if (_data.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            var root = _data[0]; // Get smallest element
            _data[0] = _data[^1]; // Move last element to top
            _data.RemoveAt(_data.Count - 1);

            // Restore heap order if there are still elements
            if (_data.Count > 0)
                HeapifyDown(0);

            return root;
        }

        // Moves an item up until the heap order is correct
        private void HeapifyUp(int idx)
        {
            while (idx > 0)
            {
                int parent = (idx - 1) / 2;

                // Stop if parent is smaller (order is correct)
                if (_data[parent].Priority <= _data[idx].Priority)
                    break;

                // Swap with parent
                (_data[parent], _data[idx]) = (_data[idx], _data[parent]);
                idx = parent;
            }
        }

        // Moves an item down until the heap order is correct
        private void HeapifyDown(int idx)
        {
            while (true)
            {
                int left = 2 * idx + 1;
                int right = 2 * idx + 2;
                int smallest = idx;

                // Compare with left child
                if (left < _data.Count && _data[left].Priority < _data[smallest].Priority)
                    smallest = left;

                // Compare with right child
                if (right < _data.Count && _data[right].Priority < _data[smallest].Priority)
                    smallest = right;

                // Stop if already in correct place
                if (smallest == idx)
                    break;

                // Swap with smaller child
                (_data[idx], _data[smallest]) = (_data[smallest], _data[idx]);
                idx = smallest;
            }
        }
    }
}
