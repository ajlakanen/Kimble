namespace Kimble;

/// <summary>
/// Player
/// </summary>
public class Player
{
    /// <summary>
    /// Color
    /// </summary>
    public Color Color { get; init; }
    
    /// <summary>
    /// First home position (board array index).
    /// </summary>
    public int HomeStartsFrom { get; init; }

    public double StartingAngle { get; init; }

    /// <summary>
    /// Last safe position (board array index).
    /// </summary>
    public int SafeEnd { get; init; }
    
    /// <summary>
    /// Is player still in the game.
    /// </summary>
    public bool InGame { get; protected set; }

    /// <summary>
    /// Player
    /// </summary>
    public Player()
    {
    }
}
