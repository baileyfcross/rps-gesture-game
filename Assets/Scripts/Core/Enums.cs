/// <summary>
/// Shared enumerations used across the RPS Gesture Game.
/// Defines states, shapes, finger types, positions, and round results.
/// </summary>
namespace RPSGame.Core
{
    /// <summary>Represents the high-level state of the game.</summary>
    public enum GameState
    {
        /// <summary>The main menu screen is displayed.</summary>
        MainMenu,

        /// <summary>The player is selecting Rock, Paper, or Scissors.</summary>
        ShapeSelection,

        /// <summary>The player is forming the chosen gesture with their fingers.</summary>
        GestureFormation,

        /// <summary>The round outcome is being displayed.</summary>
        Resolution,

        /// <summary>The game has ended (player won).</summary>
        GameOver
    }

    /// <summary>The three possible shapes in Rock, Paper, Scissors.</summary>
    public enum Shape
    {
        /// <summary>All fingers curled into a fist.</summary>
        Rock,

        /// <summary>All fingers extended flat.</summary>
        Paper,

        /// <summary>Index and Middle fingers extended, rest curled.</summary>
        Scissors
    }

    /// <summary>Identifies each of the five fingers on one hand.</summary>
    public enum FingerType
    {
        /// <summary>The thumb.</summary>
        Thumb,

        /// <summary>The index finger.</summary>
        Index,

        /// <summary>The middle finger.</summary>
        Middle,

        /// <summary>The ring finger.</summary>
        Ring,

        /// <summary>The pinky finger.</summary>
        Pinky
    }

    /// <summary>The two possible positions a finger can occupy.</summary>
    public enum FingerPosition
    {
        /// <summary>The finger is curled inward.</summary>
        Curled,

        /// <summary>The finger is stretched outward.</summary>
        Extended
    }

    /// <summary>The result of a single round of Rock, Paper, Scissors.</summary>
    public enum RoundResult
    {
        /// <summary>The player won this round.</summary>
        Win,

        /// <summary>The player lost this round.</summary>
        Lose,

        /// <summary>Both players chose the same shape.</summary>
        Draw
    }
}
