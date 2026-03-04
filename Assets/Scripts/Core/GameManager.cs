/// <summary>
/// Singleton MonoBehaviour that orchestrates the full game flow for the RPS Gesture Game.
/// Coordinates ShapeSelection → GestureFormation → Resolution → GameOver states.
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPSGame.Core
{
    using RPSGame.AI;
    using RPSGame.Hand;

    /// <summary>
    /// Central controller for the RPS Gesture Game.  Implemented as a persistent
    /// singleton (<see cref="DontDestroyOnLoad"/>) so it survives scene transitions.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>Gets the single active instance of <see cref="GameManager"/>.</summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Enforces the singleton pattern and marks this object persistent across scenes.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Serialized Fields

        /// <summary>Reference to the hand controller that tracks finger positions.</summary>
        [Tooltip("The HandController managing all five FingerControllers.")]
        [SerializeField] private HandController handController;

        /// <summary>Reference to the AI opponent that picks a shape each round.</summary>
        [Tooltip("The AIOpponent component.")]
        [SerializeField] private AIOpponent aiOpponent;

        /// <summary>Reference to the round countdown timer.</summary>
        [Tooltip("The RoundTimer component used during GestureFormation.")]
        [SerializeField] private RoundTimer roundTimer;

        /// <summary>Database of all shape definitions used to validate gestures.</summary>
        [Tooltip("ScriptableObject containing all ShapeDefinition assets.")]
        [SerializeField] private ShapeDatabase shapeDatabase;

        #endregion

        #region Private Fields

        private GameState _currentState;
        private Shape     _playerShape;
        private int       _roundCount;
        private readonly List<RoundHistoryEntry> _roundHistory = new List<RoundHistoryEntry>();

        /// <summary>Set to <c>true</c> when the gesture was completed before the timer expired.</summary>
        private bool _gestureCompletedInTime;

        #endregion

        #region Properties

        /// <summary>Gets the current <see cref="GameState"/> of the game.</summary>
        public GameState CurrentState => _currentState;

        /// <summary>Gets the complete history of resolved rounds in the current game session.</summary>
        public IReadOnlyList<RoundHistoryEntry> RoundHistory => _roundHistory.AsReadOnly();

        /// <summary>Gets the number of rounds completed so far.</summary>
        public int RoundCount => _roundCount;

        #endregion

        #region Events

        /// <summary>
        /// Raised whenever the <see cref="CurrentState"/> changes.
        /// The parameter is the new <see cref="GameState"/>.
        /// </summary>
        public event Action<GameState> OnGameStateChanged;

        /// <summary>
        /// Raised after a round has been resolved.
        /// Parameters: the <see cref="RoundResult"/>, the player's shape, and the AI's shape.
        /// </summary>
        public event Action<RoundResult, Shape, Shape> OnRoundResolved;

        /// <summary>Raised when the player wins a round, ending the game.</summary>
        public event Action OnGameOver;

        #endregion

        #region Unity Lifecycle

        /// <summary>Subscribes to hand and timer events once the component is enabled.</summary>
        private void OnEnable()
        {
            if (handController != null)
                handController.OnAllFingersPlaced += HandleAllFingersPlaced;

            if (roundTimer != null)
            {
                roundTimer.OnTimerExpired += HandleTimerExpired;
            }
        }

        /// <summary>Unsubscribes from hand and timer events when the component is disabled.</summary>
        private void OnDisable()
        {
            if (handController != null)
                handController.OnAllFingersPlaced -= HandleAllFingersPlaced;

            if (roundTimer != null)
            {
                roundTimer.OnTimerExpired -= HandleTimerExpired;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Begins a new game session, resetting state and transitioning to
        /// <see cref="GameState.ShapeSelection"/>.
        /// </summary>
        public void StartGame()
        {
            _roundCount   = 0;
            _roundHistory.Clear();
            SetState(GameState.ShapeSelection);
        }

        /// <summary>
        /// Called when the player selects a shape in the UI.
        /// Stores the chosen shape, has the AI pick its shape, starts the timer,
        /// resets the hand, and transitions to <see cref="GameState.GestureFormation"/>.
        /// </summary>
        /// <param name="shape">The shape the player selected.</param>
        public void OnShapeSelected(Shape shape)
        {
            _playerShape            = shape;
            _gestureCompletedInTime = false;

            aiOpponent?.ChooseShape();

            handController?.ResetAllFingers();
            roundTimer?.ResetTimer();
            roundTimer?.StartTimer();

            SetState(GameState.GestureFormation);
        }

        /// <summary>
        /// Resets hand and timer state then transitions back to <see cref="GameState.ShapeSelection"/>
        /// for the next round.
        /// </summary>
        public void StartNextRound()
        {
            handController?.ResetAllFingers();
            roundTimer?.ResetTimer();
            SetState(GameState.ShapeSelection);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates <see cref="CurrentState"/>, raises <see cref="OnGameStateChanged"/>,
        /// and stores the new state.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        private void SetState(GameState newState)
        {
            _currentState = newState;
            OnGameStateChanged?.Invoke(_currentState);
        }

        /// <summary>
        /// Invoked by <see cref="HandController.OnAllFingersPlaced"/>.
        /// Validates the formed gesture and resolves the round if the gesture is correct.
        /// </summary>
        private void HandleAllFingersPlaced()
        {
            roundTimer?.StopTimer();
            _gestureCompletedInTime = true;

            ShapeDefinition targetDefinition = shapeDatabase?.GetDefinition(_playerShape);
            Dictionary<FingerType, FingerPosition> positions = handController?.GetCurrentPositions();

            bool gestureValid = GestureValidator.Validate(targetDefinition, positions);

            if (gestureValid)
                ResolveRound();
            else
                Debug.Log("[GameManager] Gesture did not match the selected shape.");
        }

        /// <summary>
        /// Invoked by <see cref="RoundTimer.OnTimerExpired"/>.
        /// Treats the round as a loss due to time-out.
        /// </summary>
        private void HandleTimerExpired()
        {
            _gestureCompletedInTime = false;
            Shape aiShape = aiOpponent != null ? aiOpponent.LastChosenShape : Shape.Rock;
            RecordAndBroadcastResult(RoundResult.Lose, _playerShape, aiShape);
            SetState(GameState.Resolution);
        }

        /// <summary>
        /// Compares the player's shape against the AI's shape, records the result,
        /// fires <see cref="OnRoundResolved"/>, and transitions to the appropriate next state.
        /// </summary>
        private void ResolveRound()
        {
            Shape aiShape = aiOpponent != null ? aiOpponent.LastChosenShape : Shape.Rock;
            RoundResult result = RoundResolver.Resolve(_playerShape, aiShape);

            RecordAndBroadcastResult(result, _playerShape, aiShape);

            if (result == RoundResult.Win)
            {
                SetState(GameState.GameOver);
                OnGameOver?.Invoke();
            }
            else
            {
                SetState(GameState.Resolution);
            }
        }

        /// <summary>
        /// Increments the round counter, appends a <see cref="RoundHistoryEntry"/> to
        /// <see cref="_roundHistory"/>, and raises <see cref="OnRoundResolved"/>.
        /// </summary>
        /// <param name="result">Outcome of the round from the player's perspective.</param>
        /// <param name="playerShape">Shape chosen by the player.</param>
        /// <param name="aiShape">Shape chosen by the AI.</param>
        private void RecordAndBroadcastResult(RoundResult result, Shape playerShape, Shape aiShape)
        {
            _roundCount++;
            _roundHistory.Add(new RoundHistoryEntry(
                _roundCount, playerShape, aiShape, result, _gestureCompletedInTime));

            OnRoundResolved?.Invoke(result, playerShape, aiShape);
        }

        #endregion
    }
}
