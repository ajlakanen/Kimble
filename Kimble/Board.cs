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

    public List<Piece> Pieces { get; private set; }

    public Position[] Positions { get; private set; }

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
                SafeOrBase @base = new SafeOrBase(players[i]);
                this[j] = @base;
            }

            // Basic positions
            for (int j = startPos + 4; j < startPos + 4 + 7; j++)
            {
                Position position = new Position();
                this[j] = position;
            }

            // Safe 
            int safeStartsFrom = (startPos + 4 + 7) % Board.TotalNumberOfPositions;
            for (int j = safeStartsFrom; j < safeStartsFrom + 4; j++)
            {
                SafeOrBase safe = new SafeOrBase(players[i]);
                this[j] = safe;
            }
        }
    }

    internal Position GiveNextPosition(Player player, Position candidate)
    {
        throw new NotImplementedException();
    }

    public void MovePieceToBase(Position position)
    {
        if (position.PieceInPosition == null) return;
        Piece piece = position.PieceInPosition;
        int firstBasePos = position.PieceInPosition.Owner.StartingPosition;
        do
        {
            if (!this[firstBasePos].IsVacant().isVacant)
            {
                firstBasePos++;
            }
            else
            {
                position.RemovePiece();
                this[firstBasePos].InsertPiece(piece);
            }
        } while (firstBasePos < firstBasePos + 4);
    }
}
