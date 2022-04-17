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

    public Position[] Positions { get; private set; }

    public Board()
    {
        Positions = new Position[TotalNumberOfPositions];
    }

    public Position NextPosition(Position position)
    {
        int current = Array.IndexOf(Positions, position);
        return Positions[current + 1 % TotalNumberOfPositions];
    }

    /// <summary>
    /// Give the next position that is not one of other players safe positions. 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="currentPosition"></param>
    /// <returns></returns>
    public int GiveNextPosition(Player player, int currentPosition)
    {
        int candidatePosition = currentPosition;
        do
        {
            candidatePosition = currentPosition + 1 >= Positions.Length ? 0 : currentPosition + 1;
        } while (!Positions[candidatePosition].CanPlayerMove(player));
        return candidatePosition;
    }
}
