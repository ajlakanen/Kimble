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

    /// <summary>
    /// Board
    /// </summary>
    /// <param name="players">Players</param>
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

    public Position[] MovablePositions(Player player, int diceNumber)
    {
        List<Position> movables;
        if (diceNumber == 6) movables = Positions.Select(pos => pos).Where(pos => pos.PositionOccupiedBy(player)).ToList();
        else 
            movables = Positions.Select(pos => pos).Where(pos => pos is not Base && pos.PositionOccupiedBy(player)).ToList();

        int i = 0;
        while (i < movables.Count)
        {
            Position candidate = movables[i];
            // From base the piece can go only to the starting position
            if (movables[i] is Base) candidate = Positions[player.StartingPosition + 4];
            else // Other positions have to be calculated. 
            {
                candidate = CalculateNewPosition(player, diceNumber, candidate);
            }
            // If player's own piece is in the position, player can not move there. 
            if (candidate.PositionOccupiedBy(player)) movables.RemoveAt(i);
            // TODO: Player should be able to move forward in the safe.
            else i++;
        }

        return movables.ToArray();
    }

    /// <summary>
    /// Move a piece from position to base.
    /// </summary>
    /// <param name="oldPosition">Current position.</param>
    public void MovePieceToBase(Position oldPosition)
    {
        if (oldPosition.IsVacant().player == null) return;
        Player player = oldPosition.IsVacant().player;
        int firstBasePos = player.StartingPosition;
        int basePos = firstBasePos;
        do
        {
            if (!(this[basePos].IsVacant().isVacant))
            {
                basePos++;
            }
            else
            {
                MovePieceToNewPosition(oldPosition, this[basePos]);
                break;
            }
        } while (basePos < firstBasePos + 4);
    }

    /// <summary>
    /// Move player to a new position.
    /// </summary>
    /// <param name="oldPosition">Current position</param>
    /// <param name="newPosition">New position</param>
    public static void MovePieceToNewPosition(Position oldPosition, Position newPosition)
    {
        oldPosition.MovePlayerTo(newPosition);
    }

    /// <summary>
    /// Move player to a new position.
    /// </summary>
    /// <param name="oldPosition">Current position</param>
    /// <param name="newPosition">New position</param>
    public void MovePieceToNewPosition(int oldPosition, int newPosition)
    {
        MovePieceToNewPosition(Positions[oldPosition], Positions[newPosition]);
    }

    public int GetIndexOf(Position position)
    {
        return Array.IndexOf(this.Positions, position);
    }

    internal Position CalculateNewPosition(Player player, int diceNumber, Position oldPosition)
    {
        if (oldPosition is Base) return Positions[player.StartingPosition + 4];
        for (int j = 0; j < diceNumber; j++)
        {
            oldPosition = NextBoardPosition(player, oldPosition);
            // If the position is player's LAST own safe, we stop there. 
            if (oldPosition is Safe && GetIndexOf(oldPosition) == player.LastSafePosition) return oldPosition;
        }
        return oldPosition;
    }

    /// <summary>
    /// Next position on board that is not a base or opponents safe.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="oldPosition"></param>
    /// <returns></returns>
    private Position NextBoardPosition(Player player, Position oldPosition)
    {
        int index = Array.IndexOf(this.Positions, oldPosition);
        do
        {
            index = index + 1 >= this.Positions.Length ? 0 : index + 1;
            Position position = this.Positions[index];
            // Bases can not be re-entered from the base positions.
            if (position is Base) continue;

            // Player can not go to others' safes.
            else if (position is Safe safe && safe.OwnedBy != player) continue;
            else break;
        } while (true);
        return Positions[index];
    }
}