namespace Kimble;

/// <summary>
/// Home.
/// </summary>
public class Home : Position, IHomeOrSafe
{
    /// <summary>
    /// Position owned by.
    /// </summary>
    public Player OwnedBy { get; init; }

    /// <summary>
    /// Home.
    /// </summary>
    /// <param name="player">Player.</param>
    public Home(Player player)
    {
        OwnedBy = player;
        base.PlayerInPosition = player;
    }
}
