using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Position
{
    /// <summary>
    /// Safe is owned by a player. No other
    /// player can enter it.
    /// </summary>
    public Player PositionOwnedBy { get; init; }

    public Piece PieceInPosition { get; private set; }

    private bool isVacant = true;

    public Position()
    {
    }

    /// <summary>
    /// Safe position. 
    /// </summary>
    /// <param name="player">Owner.</param>
    public Position(Player player)
    {
        PositionOwnedBy = player;
    }

    public (bool isVacant, Piece piece) IsVacant()
    {
        if (isVacant) return (isVacant, null);
        else return (isVacant, PieceInPosition);
    }

    /// <summary>
    /// Insert player into position.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <returns>Is insertion successful.</returns>
    public bool InsertPiece(Player player, Piece piece)
    {
        if (PositionOwnedBy != player) return false;
        PieceInPosition = piece;
        isVacant = false;
        return true;
    }

    /// <summary>
    /// Remove piece from position.
    /// </summary>
    public void RemovePiece()
    {
        PieceInPosition = null;
        isVacant = true;
    }

    /// <summary>
    /// Can player move into this position.
    /// </summary>
    /// <param name="player">Player</param>
    /// <returns>Can player move</returns>
    public bool CanPlayerMove(Player player)
    {
        return PositionOwnedBy == null || PositionOwnedBy == player;
    }
}
