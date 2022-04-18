using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Board
{
    /// <summary>
    /// Basic positions + bases + safes.
    /// </summary>
    public const int TotalNumberOfPositions = 60;

    /// <summary>
    /// Board positions.
    /// </summary>
    public Position[] Positions { get; private set; }

    /// <summary>
    /// Board position.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Position this[int index]
    {
        get { return Positions[index]; }
        set { Positions[index] = value; }
    }

    public Board(Player[] players)
    {
        Positions = new Position[TotalNumberOfPositions];

        // Let's make the board
        for (int i = 0; i < players.Length; i++)
        {
            // Each player has his own color and starting position.
            int startPos = players[i].StartingPosition;

            // Base
            for (int j = startPos; j < startPos + 4; j++)
            {
                Base @base = new(players[i]);
                this[j] = @base;
            }

            // Basic positions
            for (int j = startPos + 4; j < startPos + 4 + 7; j++)
            {
                Position position = new();
                this[j] = position;
            }

            // Safe 
            int safeStartsFrom = (startPos + 4 + 7) % Board.TotalNumberOfPositions;
            for (int j = safeStartsFrom; j < safeStartsFrom + 4; j++)
            {
                Safe safe = new(players[i]);
                this[j] = safe;
            }
        }
    }

    /// <summary>
    /// Next position on board that is not a base or opponents safe.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="oldPosition"></param>
    /// <returns></returns>
    internal Position NextBoardPosition(Player player, Position oldPosition)
    {
        int index = Array.IndexOf(this.Positions, oldPosition);
        do
        {
            index = index + 1 >= this.Positions.Length ? 0 : index + 1;
            Position position = this.Positions[index];
            if (position is Base) continue;
            else if (position is Safe safe && safe.OwnedBy != player) continue;
            else break;
        } while (true);
        return Positions[index];
    }

    /// <summary>
    /// Move a piece from position to base.
    /// </summary>
    /// <param name="position">Current position.</param>
    public void MovePieceToBase(Position position)
    {
        if (position.PlayerInPosition == null) return;
        Player player = position.PlayerInPosition;
        int firstBasePos = player.StartingPosition;
        int basePos = firstBasePos;
        do
        {
            if (!this[basePos].IsVacant().isVacant)
            {
                basePos++;
            }
            else
            {
                this[basePos].MovePieceToNewPosition(position);
            }
        } while (basePos < firstBasePos + 4);
    }

    public int GetIndexOf(Position position)
    {
        return Array.IndexOf(this.Positions, position);
    }
}
