/// <summary>
/// Static utility class that validates player gesture input against a target shape definition.
/// </summary>
using System.Collections.Generic;

namespace RPSGame.Core
{
    /// <summary>
    /// Provides methods to compare the player's current finger positions against the
    /// requirements specified in a <see cref="ShapeDefinition"/>.
    /// </summary>
    public static class GestureValidator
    {
        /// <summary>
        /// Determines whether the supplied finger positions exactly match the
        /// requirements of the given <paramref name="target"/> shape.
        /// </summary>
        /// <param name="target">The shape definition to validate against.</param>
        /// <param name="currentPositions">
        /// A mapping of each <see cref="FingerType"/> to its current <see cref="FingerPosition"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if all fingers satisfy the target shape requirements; otherwise <c>false</c>.
        /// </returns>
        public static bool Validate(ShapeDefinition target, Dictionary<FingerType, FingerPosition> currentPositions)
        {
            if (target == null || currentPositions == null)
                return false;

            return target.IsMatch(currentPositions);
        }

        /// <summary>
        /// Calculates the proportion of fingers that are already in their required positions.
        /// </summary>
        /// <param name="target">The shape definition to validate against.</param>
        /// <param name="currentPositions">
        /// A mapping of each <see cref="FingerType"/> to its current <see cref="FingerPosition"/>.
        /// </param>
        /// <returns>
        /// A value between <c>0</c> (no fingers correct) and <c>1</c> (all fingers correct).
        /// Returns <c>0</c> when <paramref name="target"/> or <paramref name="currentPositions"/>
        /// is <c>null</c> or empty.
        /// </returns>
        public static float GetMatchPercentage(ShapeDefinition target, Dictionary<FingerType, FingerPosition> currentPositions)
        {
            if (target == null || currentPositions == null || currentPositions.Count == 0)
                return 0f;

            int totalFingers = System.Enum.GetValues(typeof(FingerType)).Length;
            int matchCount = 0;

            foreach (FingerType fingerType in System.Enum.GetValues(typeof(FingerType)))
            {
                if (currentPositions.TryGetValue(fingerType, out FingerPosition actual))
                {
                    if (actual == target.GetRequiredPosition(fingerType))
                        matchCount++;
                }
            }

            return (float)matchCount / totalFingers;
        }
    }
}
