using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Utils
{
    public static class RectPointGenerator
    {
        public static List<Vector2> Generate(RectTransform rectTransform, int count, float padding = 0f,
            int candidateCount = 30, int? seed = null)
        {
            if (rectTransform == null)
                throw new ArgumentNullException(nameof(rectTransform));

            if (count <= 0)
                return new List<Vector2>();

            Rect rect = rectTransform.rect;
            float width = rect.width - padding * 2f;
            float height = rect.height - padding * 2f;

            if (width <= 0f || height <= 0f)
                throw new ArgumentException("Padding is too large for the specified RectTransform.");

            candidateCount = Mathf.Max(1, candidateCount);
            System.Random random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
            List<Vector2> points = new List<Vector2>(count);
            float minX = rect.xMin + padding;
            float minY = rect.yMin + padding;

            for (int i = 0; i < count; i++)
            {
                Vector2 bestCandidate = default;
                float bestDistance = -1f;

                for (int j = 0; j < candidateCount; j++)
                {
                    Vector2 candidate = new Vector2(
                        NextFloat(random, minX, minX + width),
                        NextFloat(random, minY, minY + height)
                    );
                    float distance = GetClosestDistanceSquared(candidate, points);

                    if (distance <= bestDistance)
                        continue;

                    bestCandidate = candidate;
                    bestDistance = distance;
                }

                points.Add(bestCandidate);
            }

            return points;
        }

        private static float NextFloat(System.Random random, float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private static float GetClosestDistanceSquared(Vector2 candidate, IList<Vector2> points)
        {
            float closestDistance = float.MaxValue;

            for (int i = 0; i < points.Count; i++)
            {
                float distance = (candidate - points[i]).sqrMagnitude;

                if (distance < closestDistance)
                    closestDistance = distance;
            }

            return closestDistance;
        }
    }
}
