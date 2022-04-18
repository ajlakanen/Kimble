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
    public Piece PieceInPosition { get; private set; }

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
    public void InsertPiece(Piece piece)
    {
        PieceInPosition = piece;
        isVacant = false;
    }

    /// <summary>
    /// Remove piece from position.
    /// </summary>
    public void RemovePiece()
    {
        PieceInPosition = null;
        isVacant = true;
    }
}
