using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public static class Rules
{
    /// <summary>
    /// Do the pieces have to "come backwards" from the safe.
    /// true = Pieces stop in the last safe vacant position, even though the dice shows greater number.
    /// false = Pieces have to move exactly as the dice tells, even if they have to come out of the safe.
    /// </summary>
    public static bool PieceStopsInSafe = true;

    /// <summary>
    /// Do the pieces skip occupied safe positions.
    /// </summary>
    public static bool PiecesSkipOccupiedPositionsInSafe = false;

    /// <summary>
    /// Player colors and their correspondent starting positions (indices of the Positions table). 
    /// </summary>
    public static readonly (Color color, int startingPosition)[] colorsAndStartingPositions =
    {
        (Color.Red, 0),
        (Color.Blue, 10),
        (Color.Yellow, 20),
        (Color.Green, 30)
    };
}
