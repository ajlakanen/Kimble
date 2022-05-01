namespace Kimble;

/// <summary>
/// Home or safe position.
/// </summary>
internal interface IHomeOrSafe
{
    /// <summary>
    /// Position owned by.
    /// </summary>
    public Player OwnedBy { get; init; }
}
