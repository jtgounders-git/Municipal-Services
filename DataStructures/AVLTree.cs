using System;
using System.Collections.Generic;

namespace Prog7312PoePart1.DataStructures
{
    // Represents a single node in the AVL tree.
    // Each node stores a key, a value, references to left and right children, and its height in the tree.
    public class AVLNode<TKey, TValue> where TKey : IComparable<TKey>
    {
        public TKey Key;                  // Unique key used for ordering nodes
        public TValue Value;              // Associated data value
        public AVLNode<TKey, TValue> Left, Right;  // Left and right child nodes
        public int Height;                // Height of the node (used for balancing)

        public AVLNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Height = 1; // New nodes are created with height 1
        }
    }

    // AVLTree class ensures that the binary search tree remains balanced after every insertion.
    // Balancing maintains O(log n) search and insert time complexity.
    public class AVLTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        public AVLNode<TKey, TValue> Root; // The root node of the AVL tree

        // Helper method to get the height of a node (returns 0 for null)
        private int Height(AVLNode<TKey, TValue> n) => n?.Height ?? 0;

        // Calculates the balance factor: height difference between left and right subtrees
        private int BalanceFactor(AVLNode<TKey, TValue> n) =>
            n == null ? 0 : Height(n.Left) - Height(n.Right);

        // Updates the node's height based on its children's heights
        private void FixHeight(AVLNode<TKey, TValue> n) =>
            n.Height = Math.Max(Height(n.Left), Height(n.Right)) + 1;

        // Performs a right rotation to fix left-heavy imbalance (LL case)
        private AVLNode<TKey, TValue> RightRotate(AVLNode<TKey, TValue> y)
        {
            var x = y.Left;  // Left child becomes the new root of this subtree
            var t = x.Right; // Temporarily store x’s right subtree

            // Perform rotation
            x.Right = y;
            y.Left = t;

            // Update heights
            FixHeight(y);
            FixHeight(x);

            return x; // Return new root
        }

        // Performs a left rotation to fix right-heavy imbalance (RR case)
        private AVLNode<TKey, TValue> LeftRotate(AVLNode<TKey, TValue> x)
        {
            var y = x.Right; // Right child becomes the new root of this subtree
            var t = y.Left;  // Temporarily store y’s left subtree

            // Perform rotation
            y.Left = x;
            x.Right = t;

            // Update heights
            FixHeight(x);
            FixHeight(y);

            return y; // Return new root
        }

        // Public method to insert a key-value pair into the tree
        public void Insert(TKey key, TValue value) => Root = Insert(Root, key, value);

        // Recursive insertion method that ensures balancing after every insert
        private AVLNode<TKey, TValue> Insert(AVLNode<TKey, TValue> node, TKey key, TValue value)
        {
            // Base case: reached a leaf position
            if (node == null) return new AVLNode<TKey, TValue>(key, value);

            // Determine where to insert based on key comparison
            int cmp = key.CompareTo(node.Key);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);      // Insert in left subtree
            else if (cmp > 0) node.Right = Insert(node.Right, key, value); // Insert in right subtree
            else node.Value = value; // Update existing key’s value

            // Update the height of the current node
            FixHeight(node);

            // Calculate balance factor to check for imbalance
            int balance = BalanceFactor(node);

            // === HANDLE FOUR ROTATION CASES ===
            // Left Left (LL) Case
            if (balance > 1 && key.CompareTo(node.Left.Key) < 0)
                return RightRotate(node);

            // Right Right (RR) Case
            if (balance < -1 && key.CompareTo(node.Right.Key) > 0)
                return LeftRotate(node);

            // Left Right (LR) Case
            if (balance > 1 && key.CompareTo(node.Left.Key) > 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left (RL) Case
            if (balance < -1 && key.CompareTo(node.Right.Key) < 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            // Return the (potentially rebalanced) node
            return node;
        }

        // Returns all values in ascending key order (in-order traversal)
        public IEnumerable<TValue> InOrder()
        {
            var list = new List<TValue>();
            InOrder(Root, list);
            return list;
        }

        // Helper recursive method for in-order traversal
        private void InOrder(AVLNode<TKey, TValue> node, List<TValue> list)
        {
            if (node == null) return;
            InOrder(node.Left, list);    // Visit left subtree
            list.Add(node.Value);        // Visit current node
            InOrder(node.Right, list);   // Visit right subtree
        }
    }
}
