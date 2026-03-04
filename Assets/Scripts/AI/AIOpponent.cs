/// <summary>
/// MonoBehaviour that represents the AI opponent in Rock, Paper, Scissors.
/// Picks a shape using configurable weighted-random selection.
/// </summary>
using UnityEngine;

namespace RPSGame.AI
{
    using RPSGame.Core;

    /// <summary>
    /// Simple AI opponent that selects a <see cref="Shape"/> each round using
    /// weighted-random logic.  Weights correspond to Rock, Paper, and Scissors
    /// in the order defined by the <see cref="Shape"/> enum.
    /// </summary>
    public class AIOpponent : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Relative weights for each shape.  Index 0 = Rock, 1 = Paper, 2 = Scissors.
        /// Higher values make that shape more likely to be chosen.
        /// Default equal weights produce a uniform random selection.
        /// </summary>
        [Tooltip("Relative weights for Rock (0), Paper (1), and Scissors (2). Higher = more likely.")]
        [SerializeField] private float[] shapeWeights = { 1f, 1f, 1f };

        #endregion

        #region Properties

        /// <summary>Gets the shape that was chosen in the most recent call to <see cref="ChooseShape"/>.</summary>
        public Shape LastChosenShape { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Selects and returns a <see cref="Shape"/> using weighted-random logic based
        /// on <see cref="shapeWeights"/>.  The result is also stored in <see cref="LastChosenShape"/>.
        /// </summary>
        /// <returns>The shape the AI has chosen for this round.</returns>
        public Shape ChooseShape()
        {
            float totalWeight = 0f;
            foreach (float weight in shapeWeights)
                totalWeight += Mathf.Max(0f, weight);

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < shapeWeights.Length; i++)
            {
                cumulative += Mathf.Max(0f, shapeWeights[i]);
                if (roll <= cumulative)
                {
                    LastChosenShape = (Shape)i;
                    return LastChosenShape;
                }
            }

            // Fallback — should not normally be reached
            LastChosenShape = Shape.Rock;
            return LastChosenShape;
        }

        #endregion
    }
}
