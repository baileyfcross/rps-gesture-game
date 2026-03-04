/// <summary>
/// MonoBehaviour that manages all UI panels and text elements for the RPS Gesture Game.
/// Subscribes to <see cref="GameManager"/> events to show/hide panels and update display text.
/// </summary>
using UnityEngine;
using UnityEngine.UI;

namespace RPSGame.UI
{
    using RPSGame.Core;

    /// <summary>
    /// Handles panel visibility transitions driven by <see cref="GameManager.OnGameStateChanged"/>,
    /// updates the on-screen timer during the Gesture Formation phase, displays round results,
    /// and routes button-click callbacks to the <see cref="GameManager"/> singleton.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Serialized Fields — Panels

        /// <summary>Panel shown while the game is in the <see cref="GameState.MainMenu"/> state.</summary>
        [Tooltip("Root GameObject of the Main Menu panel.")]
        [SerializeField] private GameObject mainMenuPanel;

        /// <summary>Panel shown while the player is choosing a shape.</summary>
        [Tooltip("Root GameObject of the Shape Selection panel.")]
        [SerializeField] private GameObject shapeSelectionPanel;

        /// <summary>Panel shown while the player is forming a gesture.</summary>
        [Tooltip("Root GameObject of the Gesture Formation panel.")]
        [SerializeField] private GameObject gestureFormationPanel;

        /// <summary>Panel shown after a round resolves.</summary>
        [Tooltip("Root GameObject of the Resolution panel.")]
        [SerializeField] private GameObject resolutionPanel;

        /// <summary>Panel shown when the game is over.</summary>
        [Tooltip("Root GameObject of the Game Over panel.")]
        [SerializeField] private GameObject gameOverPanel;

        #endregion

        #region Serialized Fields — Text

        /// <summary>Text element that displays the countdown timer during gesture formation.</summary>
        [Tooltip("Text element that shows the remaining time during the Gesture Formation phase.")]
        [SerializeField] private Text timerText;

        /// <summary>Text element that shows the round result (Win / Lose / Draw).</summary>
        [Tooltip("Text element that displays the round result on the Resolution panel.")]
        [SerializeField] private Text resultText;

        /// <summary>Text element that shows which shape the AI chose.</summary>
        [Tooltip("Text element that shows the AI's chosen shape on the Resolution panel.")]
        [SerializeField] private Text aiChoiceText;

        /// <summary>Text element that shows which shape the player chose.</summary>
        [Tooltip("Text element that shows the player's chosen shape on the Resolution panel.")]
        [SerializeField] private Text playerChoiceText;

        /// <summary>Reference to the scene's <see cref="RoundTimer"/> for live timer display.</summary>
        [Tooltip("The RoundTimer component used to read TimeRemaining for the on-screen display.")]
        [SerializeField] private RoundTimer roundTimer;

        #endregion

        #region Private Fields

        private bool _showTimer;

        #endregion

        #region Unity Lifecycle

        /// <summary>Subscribes to <see cref="GameManager"/> events.</summary>
        private void OnEnable()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            GameManager.Instance.OnRoundResolved    += HandleRoundResolved;
        }

        /// <summary>Unsubscribes from <see cref="GameManager"/> events.</summary>
        private void OnDisable()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            GameManager.Instance.OnRoundResolved    -= HandleRoundResolved;
        }

        /// <summary>
        /// Updates the timer display each frame while in the Gesture Formation phase.
        /// </summary>
        private void Update()
        {
            if (!_showTimer || timerText == null || roundTimer == null) return;

            timerText.text = roundTimer.TimeRemaining.ToString("F1");
        }

        #endregion

        #region Public Button Callbacks

        /// <summary>
        /// Called when the player presses the Rock button.
        /// Forwards the selection to <see cref="GameManager.OnShapeSelected"/>.
        /// </summary>
        public void OnRockSelected()
        {
            GameManager.Instance?.OnShapeSelected(Shape.Rock);
        }

        /// <summary>
        /// Called when the player presses the Paper button.
        /// Forwards the selection to <see cref="GameManager.OnShapeSelected"/>.
        /// </summary>
        public void OnPaperSelected()
        {
            GameManager.Instance?.OnShapeSelected(Shape.Paper);
        }

        /// <summary>
        /// Called when the player presses the Scissors button.
        /// Forwards the selection to <see cref="GameManager.OnShapeSelected"/>.
        /// </summary>
        public void OnScissorsSelected()
        {
            GameManager.Instance?.OnShapeSelected(Shape.Scissors);
        }

        /// <summary>
        /// Called when the player presses the Next Round button on the Resolution panel.
        /// </summary>
        public void OnNextRoundClicked()
        {
            GameManager.Instance?.StartNextRound();
        }

        /// <summary>
        /// Called when the player presses the Play Again button on the Game Over panel.
        /// </summary>
        public void OnPlayAgainClicked()
        {
            GameManager.Instance?.StartGame();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Activates the appropriate panel for <paramref name="newState"/> and hides all others.
        /// </summary>
        /// <param name="newState">The game state that was just entered.</param>
        private void HandleGameStateChanged(GameState newState)
        {
            SetPanelActive(mainMenuPanel,         newState == GameState.MainMenu);
            SetPanelActive(shapeSelectionPanel,   newState == GameState.ShapeSelection);
            SetPanelActive(gestureFormationPanel, newState == GameState.GestureFormation);
            SetPanelActive(resolutionPanel,       newState == GameState.Resolution);
            SetPanelActive(gameOverPanel,         newState == GameState.GameOver);

            _showTimer = newState == GameState.GestureFormation;

            if (!_showTimer && timerText != null)
                timerText.text = string.Empty;
        }

        /// <summary>
        /// Updates the result, player choice, and AI choice text fields with round outcome data.
        /// </summary>
        /// <param name="result">The round outcome.</param>
        /// <param name="playerShape">The shape the player formed.</param>
        /// <param name="aiShape">The shape the AI chose.</param>
        private void HandleRoundResolved(RoundResult result, Shape playerShape, Shape aiShape)
        {
            if (resultText      != null) resultText.text      = result.ToString();
            if (playerChoiceText != null) playerChoiceText.text = $"You: {playerShape}";
            if (aiChoiceText    != null) aiChoiceText.text    = $"AI: {aiShape}";
        }

        /// <summary>
        /// Safely sets a panel's active state, guarding against null references.
        /// </summary>
        /// <param name="panel">The panel <see cref="GameObject"/> to toggle.</param>
        /// <param name="active">Whether the panel should be active.</param>
        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
                panel.SetActive(active);
        }

        #endregion
    }
}
