namespace Kimble;

/// <summary>
/// Safe
/// </summary>
public class Safe : Position, IHomeOrSafe
{
    /// <summary>
    /// Position owned by.
    /// </summary>
    public Player OwnedBy { get; init; }

    /// <summary>
    /// Safe.
    /// </summary>
    /// <param name="player">Player</param>
    public Safe(Player player)
    {
        OwnedBy = player;
    }
}
