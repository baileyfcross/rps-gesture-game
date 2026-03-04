/// <summary>
/// Static utility class that resolves the outcome of a Rock, Paper, Scissors round.
/// </summary>
namespace RPSGame.Core
{
    /// <summary>
    /// Determines the <see cref="RoundResult"/> for a player given their chosen
    /// <see cref="Shape"/> and the AI's chosen <see cref="Shape"/>.
    /// </summary>
    public static class RoundResolver
    {
        /// <summary>
        /// Applies standard Rock-Paper-Scissors rules to determine the round outcome
        /// from the player's perspective.
        /// <list type="bullet">
        ///   <item><description>Rock beats Scissors</description></item>
        ///   <item><description>Scissors beats Paper</description></item>
        ///   <item><description>Paper beats Rock</description></item>
        ///   <item><description>Identical shapes result in a Draw</description></item>
        /// </list>
        /// </summary>
        /// <param name="playerShape">The shape chosen by the player.</param>
        /// <param name="aiShape">The shape chosen by the AI opponent.</param>
        /// <returns>
        /// <see cref="RoundResult.Win"/> if the player wins,
        /// <see cref="RoundResult.Lose"/> if the AI wins,
        /// or <see cref="RoundResult.Draw"/> if both chose the same shape.
        /// </returns>
        public static RoundResult Resolve(Shape playerShape, Shape aiShape)
        {
            if (playerShape == aiShape)
                return RoundResult.Draw;

            bool playerWins =
                (playerShape == Shape.Rock     && aiShape == Shape.Scissors) ||
                (playerShape == Shape.Scissors && aiShape == Shape.Paper)    ||
                (playerShape == Shape.Paper    && aiShape == Shape.Rock);

            return playerWins ? RoundResult.Win : RoundResult.Lose;
        }
    }
}
