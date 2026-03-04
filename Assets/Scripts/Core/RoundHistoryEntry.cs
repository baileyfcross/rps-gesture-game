/// <summary>
/// Serializable data class that records the details of a single completed round.
/// </summary>
using System;

namespace RPSGame.Core
{
    /// <summary>
    /// Stores a snapshot of one round's outcome, used to build the round history log.
    /// This class is not a MonoBehaviour; it is a plain C# data container marked
    /// <see cref="SerializableAttribute"/> so Unity can display it in the Inspector.
    /// </summary>
    [Serializable]
    public class RoundHistoryEntry
    {
        /// <summary>The 1-based sequential number of this round within the current game.</summary>
        public int roundNumber;

        /// <summary>The shape the player attempted to form.</summary>
        public Shape playerShape;

        /// <summary>The shape the AI opponent chose.</summary>
        public Shape aiShape;

        /// <summary>The outcome of this round from the player's perspective.</summary>
        public RoundResult result;

        /// <summary>
        /// <c>true</c> if the player successfully placed all fingers before the timer expired;
        /// <c>false</c> if the round timed out.
        /// </summary>
        public bool gestureCompletedInTime;

        /// <summary>
        /// Initialises a new <see cref="RoundHistoryEntry"/> with all required fields.
        /// </summary>
        /// <param name="roundNumber">The sequential round number (1-based).</param>
        /// <param name="playerShape">The shape the player chose.</param>
        /// <param name="aiShape">The shape the AI chose.</param>
        /// <param name="result">The round outcome from the player's perspective.</param>
        /// <param name="gestureCompletedInTime">Whether the gesture was placed within the time limit.</param>
        public RoundHistoryEntry(int roundNumber, Shape playerShape, Shape aiShape,
                                 RoundResult result, bool gestureCompletedInTime)
        {
            this.roundNumber             = roundNumber;
            this.playerShape             = playerShape;
            this.aiShape                 = aiShape;
            this.result                  = result;
            this.gestureCompletedInTime  = gestureCompletedInTime;
        }
    }
}
