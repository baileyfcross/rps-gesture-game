/// <summary>
/// MonoBehaviour that manages all five <see cref="FingerController"/> instances
/// that make up the player's hand, tracking when every finger has been placed.
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPSGame.Hand
{
    using RPSGame.Core;

    /// <summary>
    /// Aggregates finger-placement events from each <see cref="FingerController"/>,
    /// maintains a dictionary of the current finger positions, and raises
    /// <see cref="OnAllFingersPlaced"/> once all five fingers are locked.
    /// </summary>
    public class HandController : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>References to each of the five finger controllers.</summary>
        [Tooltip("Assign all five FingerController components here (Thumb, Index, Middle, Ring, Pinky).")]
        [SerializeField] private FingerController[] fingers;

        #endregion

        #region Private Fields

        private readonly Dictionary<FingerType, FingerPosition> _currentPositions =
            new Dictionary<FingerType, FingerPosition>();

        #endregion

        #region Events

        /// <summary>
        /// Raised when every finger has been locked into a snap zone.
        /// Subscribe to this event to trigger gesture validation.
        /// </summary>
        public event Action OnAllFingersPlaced;

        #endregion

        #region Unity Lifecycle

        /// <summary>Subscribes to each finger's <see cref="FingerController.OnFingerPlaced"/> event.</summary>
        private void OnEnable()
        {
            if (fingers == null) return;
            foreach (FingerController finger in fingers)
            {
                if (finger != null)
                    finger.OnFingerPlaced += HandleFingerPlaced;
            }
        }

        /// <summary>Unsubscribes from each finger's <see cref="FingerController.OnFingerPlaced"/> event.</summary>
        private void OnDisable()
        {
            if (fingers == null) return;
            foreach (FingerController finger in fingers)
            {
                if (finger != null)
                    finger.OnFingerPlaced -= HandleFingerPlaced;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a snapshot of every finger's current <see cref="FingerPosition"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> mapping each <see cref="FingerType"/>
        /// to its current <see cref="FingerPosition"/>.
        /// </returns>
        public Dictionary<FingerType, FingerPosition> GetCurrentPositions()
        {
            return new Dictionary<FingerType, FingerPosition>(_currentPositions);
        }

        /// <summary>Resets every finger to its default state and clears the position tracking.</summary>
        public void ResetAllFingers()
        {
            _currentPositions.Clear();

            if (fingers == null) return;
            foreach (FingerController finger in fingers)
            {
                if (finger != null)
                    finger.ResetFinger();
            }
        }

        /// <summary>
        /// Returns <c>true</c> if all five fingers have been locked into a snap zone.
        /// </summary>
        /// <returns><c>true</c> when all fingers are placed; otherwise <c>false</c>.</returns>
        public bool AreAllFingersPlaced()
        {
            if (fingers == null || fingers.Length == 0)
                return false;

            foreach (FingerController finger in fingers)
            {
                if (finger == null || !finger.IsLocked)
                    return false;
            }
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles a finger-placed notification from an individual <see cref="FingerController"/>.
        /// Updates the position dictionary and checks whether all fingers are now placed.
        /// </summary>
        /// <param name="fingerType">The finger that was placed.</param>
        /// <param name="position">The position the finger snapped to.</param>
        private void HandleFingerPlaced(FingerType fingerType, FingerPosition position)
        {
            _currentPositions[fingerType] = position;

            if (AreAllFingersPlaced())
                OnAllFingersPlaced?.Invoke();
        }

        #endregion
    }
}
