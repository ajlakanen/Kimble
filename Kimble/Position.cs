using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Position
{
    private bool isVacant = true;

    /// <summary>
    /// Piece that occupies the position. 
    /// </summary>
    public Player PlayerInPosition { get; private set; }

    /// <summary>
    /// Position
    /// </summary>
    public Position()
    {
    }

    /// <summary>
    /// Is position vacant. If not vacant, return also the piece in the position. 
    /// </summary>
    /// <returns></returns>
    public (bool isVacant, Player player) IsVacant()
    {
        if (isVacant) return (isVacant, null);
        else return (isVacant, PlayerInPosition);
    }

    /// <summary>
    /// Insert player into position.
    /// </summary>
    /// <param name="oldPosition">Player.</param>
    /// <returns>Is insertion successful.</returns>
    public void MovePieceToNewPosition(Position oldPosition)
    {
        PlayerInPosition = oldPosition.PlayerInPosition;
        RemovePiece(oldPosition);
        isVacant = false;
    }

    /// <summary>
    /// Remove piece from position.
    /// </summary>
    public static void RemovePiece(Position position)
    {
        position.PlayerInPosition = null;
        position.isVacant = true;
    }

    public bool PositionOccupiedBy(Player player)
    {
        if (PlayerInPosition == null) return false;
        else if (PlayerInPosition == player) return true;
        else return false;
    }
}
