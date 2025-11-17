using UnityEngine;

namespace PrisonIsland.Utilities
{
    /// <summary>
    /// Helper functions for Prison Island Manager
    /// </summary>
    public static class HelperFunctions
    {
        /// <summary>
        /// Snap position to grid
        /// </summary>
        public static Vector3 SnapToGrid(Vector3 position, float gridSize = 5f)
        {
            position.x = Mathf.Round(position.x / gridSize) * gridSize;
            position.z = Mathf.Round(position.z / gridSize) * gridSize;
            position.y = 0;
            return position;
        }

        /// <summary>
        /// Get random point on NavMesh
        /// </summary>
        public static bool GetRandomPointOnNavMesh(Vector3 center, float radius, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * radius;
                UnityEngine.AI.NavMeshHit hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = center;
            return false;
        }

        /// <summary>
        /// Format number with abbreviations (K, M)
        /// </summary>
        public static string FormatNumber(float number)
        {
            if (number >= 1000000)
                return (number / 1000000f).ToString("0.0") + "M";
            if (number >= 1000)
                return (number / 1000f).ToString("0.0") + "K";
            return number.ToString("0");
        }

        /// <summary>
        /// Get color based on percentage (red to green)
        /// </summary>
        public static Color GetPercentageColor(float percentage)
        {
            if (percentage >= 70)
                return Color.green;
            if (percentage >= 40)
                return Color.yellow;
            if (percentage >= 20)
                return new Color(1f, 0.5f, 0f); // Orange
            return Color.red;
        }

        /// <summary>
        /// Lerp float over time
        /// </summary>
        public static float SmoothLerp(float current, float target, float smoothTime)
        {
            return Mathf.Lerp(current, target, Time.deltaTime / smoothTime);
        }

        /// <summary>
        /// Check if position is within bounds
        /// </summary>
        public static bool IsWithinBounds(Vector3 position, Vector3 min, Vector3 max)
        {
            return position.x >= min.x && position.x <= max.x &&
                   position.z >= min.z && position.z <= max.z;
        }

        /// <summary>
        /// Get direction emoji based on danger level
        /// </summary>
        public static string GetDangerEmoji(PrisonIsland.Data.DangerLevel level)
        {
            switch (level)
            {
                case PrisonIsland.Data.DangerLevel.Low: return "🟢";
                case PrisonIsland.Data.DangerLevel.Medium: return "🟡";
                case PrisonIsland.Data.DangerLevel.High: return "🟠";
                case PrisonIsland.Data.DangerLevel.Maximum: return "🔴";
                default: return "⚪";
            }
        }

        /// <summary>
        /// Play haptic feedback on mobile
        /// </summary>
        public static void PlayHaptic()
        {
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif
        }

        /// <summary>
        /// Safe destroy with null check
        /// </summary>
        public static void SafeDestroy(GameObject obj)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }

        /// <summary>
        /// Get distance in 2D (ignoring Y axis)
        /// </summary>
        public static float Distance2D(Vector3 a, Vector3 b)
        {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }
    }
}
