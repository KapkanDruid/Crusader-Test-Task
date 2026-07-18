using System.Collections.Generic;

namespace Game.Runtime.Utils
{
    public static class QueueExtensions
    {
        public static T CycledDequeue<T>(this Queue<T> queue)
        {
            if (queue.Count == 0)
                throw new System.InvalidOperationException("Cannot dequeue from an empty queue.");

            T item = queue.Dequeue();
            queue.Enqueue(item);
            return item;
        }
    }
}
