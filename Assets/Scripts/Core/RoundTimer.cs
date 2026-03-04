/// <summary>
/// MonoBehaviour that manages a countdown timer for a gesture formation round.
/// Fires events when the timer ticks each frame and when it expires.
/// </summary>
using System;
using UnityEngine;

namespace RPSGame.Core
{
    /// <summary>
    /// Countdown timer used during the Gesture Formation phase.
    /// Attach to any persistent GameObject in the scene.
    /// </summary>
    public class RoundTimer : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>Total duration of the round in seconds.</summary>
        [Tooltip("Total duration of a gesture formation round in seconds.")]
        [SerializeField] private float roundDuration = 10f;

        #endregion

        #region Private Fields

        private float _timeRemaining;
        private bool  _isRunning;

        #endregion

        #region Properties

        /// <summary>Gets the time remaining in the current countdown (in seconds).</summary>
        public float TimeRemaining => _timeRemaining;

        /// <summary>Gets a value indicating whether the timer is currently counting down.</summary>
        public bool IsRunning => _isRunning;

        #endregion

        #region Events

        /// <summary>
        /// Raised when the countdown reaches zero.
        /// Subscribers should handle the time-out scenario (e.g. auto-fail the round).
        /// </summary>
        public event Action OnTimerExpired;

        /// <summary>
        /// Raised every frame while the timer is running.
        /// The <c>float</c> parameter carries the current <see cref="TimeRemaining"/> value.
        /// </summary>
        public event Action<float> OnTimerTick;

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// Counts down <see cref="TimeRemaining"/> and fires <see cref="OnTimerTick"/>
        /// and <see cref="OnTimerExpired"/> as appropriate.
        /// </summary>
        private void Update()
        {
            if (!_isRunning)
                return;

            _timeRemaining -= Time.deltaTime;
            OnTimerTick?.Invoke(_timeRemaining);

            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                _isRunning     = false;
                OnTimerExpired?.Invoke();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts (or resumes) the countdown. Resets the remaining time to
        /// <see cref="roundDuration"/> if the timer was not already running.
        /// </summary>
        public void StartTimer()
        {
            if (!_isRunning)
                _timeRemaining = roundDuration;

            _isRunning = true;
        }

        /// <summary>Pauses the countdown without resetting the remaining time.</summary>
        public void StopTimer()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Stops the countdown and resets the remaining time to <see cref="roundDuration"/>.
        /// </summary>
        public void ResetTimer()
        {
            _isRunning     = false;
            _timeRemaining = roundDuration;
        }

        #endregion
    }
}
