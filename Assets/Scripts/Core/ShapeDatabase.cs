/// <summary>
/// ScriptableObject that acts as a central registry of all <see cref="ShapeDefinition"/> assets.
/// Create an instance via Assets > Create > RPSGame > Shape Database.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

namespace RPSGame.Core
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> holding references to every <see cref="ShapeDefinition"/>
    /// in the game, allowing look-up by <see cref="Shape"/> type.
    /// </summary>
    [CreateAssetMenu(fileName = "ShapeDatabase", menuName = "RPSGame/Shape Database")]
    public class ShapeDatabase : ScriptableObject
    {
        #region Serialized Fields

        /// <summary>All shape definitions available in the game.</summary>
        [Tooltip("List of all ShapeDefinition assets. Assign Rock, Paper, and Scissors definitions here.")]
        [SerializeField] private List<ShapeDefinition> shapeDefinitions = new List<ShapeDefinition>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the <see cref="ShapeDefinition"/> corresponding to the given <paramref name="shape"/>.
        /// </summary>
        /// <param name="shape">The shape to look up.</param>
        /// <returns>
        /// The matching <see cref="ShapeDefinition"/>, or <c>null</c> if none is registered.
        /// </returns>
        public ShapeDefinition GetDefinition(Shape shape)
        {
            foreach (ShapeDefinition definition in shapeDefinitions)
            {
                if (definition != null && definition.ShapeType == shape)
                    return definition;
            }

            Debug.LogWarning($"[ShapeDatabase] No definition found for shape: {shape}");
            return null;
        }

        #endregion
    }
}
