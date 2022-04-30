namespace Kimble;

public static class Rules
{
    /// <summary>
    /// Do the pieces have to "come backwards" from the safe.
    /// true = Pieces stop in the last safe vacant position, even though the dice shows greater number.
    /// false = Pieces have to move exactly as the dice tells, even if they have to come out of the safe.
    /// </summary>
    public const bool PieceStopsInSafe = true;

    /// <summary>
    /// Do the pieces skip occupied safe positions.
    /// </summary>
    public const bool PiecesSkipOccupiedPositionsInSafe = false;

    /// <summary>
    /// Player colors and their correspondent starting positions (indices of the Positions table).
    /// First four indices are homes.
    /// </summary>
    public static readonly (Color color, int startingPosition)[] colorsAndStartingPositions =
    {
        (Color.Red, 0),
        (Color.Blue, 15),
        (Color.Yellow, 30),
        (Color.Green, 45)
    };
}
