/// <summary>
/// ScriptableObject that defines which finger positions are required to form a specific RPS shape.
/// Create instances via Assets > Create > RPSGame > Shape Definition.
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPSGame.Core
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that stores the required <see cref="FingerPosition"/>
    /// for every <see cref="FingerType"/> to form a given <see cref="Shape"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "NewShapeDefinition", menuName = "RPSGame/Shape Definition")]
    public class ShapeDefinition : ScriptableObject
    {
        #region Serializable Inner Class

        /// <summary>
        /// Maps a single <see cref="FingerType"/> to its required <see cref="FingerPosition"/>.
        /// Used in a serialized list because Unity cannot serialize generic Dictionaries.
        /// </summary>
        [Serializable]
        public class FingerRequirement
        {
            /// <summary>The finger this requirement applies to.</summary>
            [Tooltip("The finger this requirement applies to.")]
            public FingerType fingerType;

            /// <summary>The position the finger must be in to satisfy this requirement.</summary>
            [Tooltip("The position (Extended or Curled) required for this finger.")]
            public FingerPosition requiredPosition;
        }

        #endregion

        #region Serialized Fields

        /// <summary>The shape this definition represents.</summary>
        [Tooltip("The RPS shape represented by this definition.")]
        [SerializeField] private Shape shapeType;

        /// <summary>
        /// The list of finger requirements that together describe the shape.
        /// One entry per finger is expected.
        /// </summary>
        [Tooltip("Required position for each finger to form this shape.")]
        [SerializeField] private List<FingerRequirement> fingerRequirements = new List<FingerRequirement>();

        #endregion

        #region Properties

        /// <summary>Gets the <see cref="Shape"/> this definition represents.</summary>
        public Shape ShapeType => shapeType;

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the required <see cref="FingerPosition"/> for the specified <paramref name="finger"/>.
        /// Defaults to <see cref="FingerPosition.Curled"/> if the finger is not in the list.
        /// </summary>
        /// <param name="finger">The finger to query.</param>
        /// <returns>The required position for the given finger.</returns>
        public FingerPosition GetRequiredPosition(FingerType finger)
        {
            foreach (FingerRequirement req in fingerRequirements)
            {
                if (req.fingerType == finger)
                    return req.requiredPosition;
            }
            return FingerPosition.Curled;
        }

        /// <summary>
        /// Checks whether the supplied finger positions satisfy all requirements of this shape.
        /// </summary>
        /// <param name="currentPositions">
        /// A mapping of each <see cref="FingerType"/> to its current <see cref="FingerPosition"/>.
        /// </param>
        /// <returns><c>true</c> if every finger matches its required position; otherwise <c>false</c>.</returns>
        public bool IsMatch(Dictionary<FingerType, FingerPosition> currentPositions)
        {
            if (currentPositions == null)
                return false;

            foreach (FingerRequirement req in fingerRequirements)
            {
                if (!currentPositions.TryGetValue(req.fingerType, out FingerPosition actual))
                    return false;

                if (actual != req.requiredPosition)
                    return false;
            }
            return true;
        }

        #endregion
    }
}
