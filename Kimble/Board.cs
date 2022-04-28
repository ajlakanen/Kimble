using System;
using System.Collections.Generic;
using System.Linq;

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
            int safeStartsFrom = (startPos - 4) < 0 ? startPos - 4 + Board.TotalNumberOfPositions : (startPos - 4) % Board.TotalNumberOfPositions;
            for (int j = safeStartsFrom; j < safeStartsFrom + 4; j++)
            {
                Safe safe = new(players[i]);
                this[j] = safe;
            }
        }
    }

    /// <summary>
    /// Gives a list of (Position, Position) tuples which holds those pieces that 
    /// player can move, and their new position counterparts. Pieces are
    /// selected with the following rules:
    ///  - Piece can move from the base to the starting position.
    ///  - If the piece is already in the last vacant Safe position, it cannot be moved. 
    ///  - If there's players own piece in the new position, it cannot be moved. 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="diceNumber"></param>
    /// <returns></returns>
    public List<(Position aboutToMove, Position newPosition)> MovablePositions(Player player, int diceNumber)
    {
        List<Position> movables = SelectMovables(player, diceNumber);
        List<(Position aboutToMove, Position newPosition)> movablesNewPositions = new();

        // Ignore those old positions where the player can not actually move to the new position. 
        for (int i = 0; i < movables.Count; i++)
        {
            Position movable = movables[i];
            Position newPosition;

            // Phase 1. Let's check where the piece is ABOUT to go.
            // a. From base the piece can go only to the starting position
            if (movable is Base) newPosition = Positions[player.StartingPosition + 4];

            // b. Is the piece already in last safe position, it cannot move and we don't add it to the list. 
            else if (movable is Safe safe && !LaterSafePositionExists(safe, player)) continue;
            
            // c. Other positions have to be calculated.             
            else newPosition = CalculateNewPosition(player, diceNumber, movable);

            // Phase 2. 
            // We'll check if player's own piece is in the position.
            // If it is, player can not move there and we don't add it to the list. 
            if (newPosition.PositionOccupiedBy(player)) continue;

            // After all this, we are ready to add a movable and its
            // new position to the tuples
            movablesNewPositions.Add((movable, newPosition));
        }

        return movablesNewPositions;
    }

    private List<Position> SelectMovables(Player player, int diceNumber)
    {
        List<Position> movables;
        if (diceNumber == 6)
        {
            movables = Positions
                .Select(pos => pos)
                .Where(pos => pos.PositionOccupiedBy(player))
                .ToList();
        }
        else
        {
            movables = Positions
                .Select(pos => pos)
                .Where(pos => pos is not Base && pos.PositionOccupiedBy(player))
                .ToList();
        }
        return movables;
    }

    /// <summary>
    /// Calculate new potential position.
    /// 1. From the base you go to starting position.
    /// 2. From the basic positions you can NOT go to others' safes or bases.
    /// 3. Counting stops when we reach last vacant safe position. 
    /// </summary>
    /// <param name="player">Player</param>
    /// <param name="oldPosition"></param>
    /// <returns></returns>
    internal Position CalculateNewPosition(Player player, int diceNumber, Position oldPosition)
    {
        if (oldPosition is Base) return Positions[player.StartingPosition + 4];
        Position newPosition = oldPosition;

        for (int j = 0; j < diceNumber; j++)
        {
            newPosition = NextBoardPosition(player, newPosition);
            if (newPosition is Safe)
            {
                // If the position is player's LAST vacant own safe, we stop there. 
                if (!LaterSafePositionExists(newPosition as Safe, player)) return newPosition;
            }
        }
        return newPosition;
    }

    /// <summary>
    /// Are there vacant positions more deeper in the safe?
    /// Note: if piece is in the last vacant safe position, 
    /// we should not be able to move it anymore. 
    /// </summary>
    /// <param name="safe">Position</param>
    /// <param name="player">Player</param>
    /// <returns>Is this the last vacant safe position</returns>
    private bool LaterSafePositionExists(Safe safe, Player player)
    {
        int end = player.LastSafePosition;
        int thisPos = Array.IndexOf(Positions, safe);
        while(end > thisPos)
        {
            if (Positions[end].IsVacant()) return true;
            end--;
        }
        return false;
    }

    /// <summary>
    /// Move a piece from position to base.
    /// </summary>
    /// <param name="oldPosition">Current position.</param>
    public void MovePieceToBase(Position oldPosition)
    {
        if (oldPosition.IsVacant()) return;
        Player player = oldPosition.PlayerInPosition;
        int firstBasePos = player.StartingPosition;
        int basePos = firstBasePos;
        do
        {
            if (!(this[basePos].IsVacant())) basePos++;
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

    public Position[] OccupiedPositions()
    {
        var occupied = Positions.Select(pos => pos).Where(pos => !pos.IsVacant());
        return occupied.ToArray();
    }
}
