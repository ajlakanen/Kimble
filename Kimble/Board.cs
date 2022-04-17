using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Board
{
    /// <summary>
    /// Basic positions + safe positions. 
    /// Bases are not included.
    /// </summary>
    public const int TotalNumberOfPositions = 44;

    public List<Piece> Pieces { get; private set; }

    public Position[] Positions { get; private set; }

    public Board()
    {
        Positions = new Position[TotalNumberOfPositions];
    }

    /// <summary>
    /// Give the next position that is not one of other players safe positions. 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="currentPosition"></param>
    /// <returns></returns>
    public Position GiveNextPosition(Player player, Position currentPosition)
    {
        int candidatePosition = Array.IndexOf(Positions, currentPosition);
        do
        {
            candidatePosition = candidatePosition + 1 >= Positions.Length ? 0 : candidatePosition + 1;
        } while (!Positions[candidatePosition].CanPlayerMove(player));
        return Positions[candidatePosition];
    }
}
