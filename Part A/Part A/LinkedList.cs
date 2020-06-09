using System;

namespace Puzzle1
{
    public class LinkedList<T>
    {
        public LinkedListNode<T> First { get; private set; } = null;

        public LinkedListNode<T> Last { get; private set; } = null;

        public int Count { get; private set; } = 0;
       
        public void Add(T value)
        {
            var newNode = new LinkedListNode<T>
            { 
                Value = value
            };

            if (First == null)
            {
                First = newNode;
            }
            else
            {
                Last.Next = newNode;
            }

            Last = newNode;
            Count++;
        }

        public LinkedListNode<T> GetByIndex(uint index, bool startFromHead = true)
        {
            var current = First;
            var currentIndex = 0;
            var count = Count;
            var searchIndex = startFromHead ? index: count - index - 1;

            if (index >= count)
            {
                throw new ArgumentOutOfRangeException($"Provided index value [{index}] is out of range.");
            }

            while (current != null && currentIndex < searchIndex)
            {
                current = current.Next;
                currentIndex++;
            }

            return current;
        }
    }
}
