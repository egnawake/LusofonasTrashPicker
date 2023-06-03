/// <summary>
/// Represents a high score.
/// </summary>
public struct HighScore
{
    /// <summary>
    /// Score obtained.
    /// </summary>
    public int Score { get; }

    /// <summary>
    /// Indicates whether this score was obtained by the AI.
    /// </summary>
    public bool PlayedByAI { get; }

    public HighScore(int score, bool playedByAI)
    {
        Score = score;
        PlayedByAI = playedByAI;
    }
}
