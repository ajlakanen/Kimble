using System;
using System.Collections.Generic;
using System.Linq;

namespace Kimble;

internal class Kimble
{
    public readonly Player[] Players = new Player[4];
    public readonly Board Board;
    public Player PlayerInTurn { get; private set; }
    public int DiceNow { get; set; }
    public List<(Position aboutToMove, Position newPosition)> PiecesThatCanMove { get; private set; }

    public bool GameOver = false;

    public Kimble()
    {
        Players = MakePlayers();
        PlayerInTurn = Players[0];
        Board = new Board(Players);
    }

    private Player[] MakePlayers()
    {
        for (int i = 0; i < Rules.colorsAndStartingPositions.Length; i++)
        {
            // Each player has his own color and starting position.
            int startPos = Rules.colorsAndStartingPositions[i].startingPosition;
            Player player = new()
            {
                Color = Rules.colorsAndStartingPositions[i].color,
                StartingPosition = startPos,
                LastSafePosition = startPos - 1 < 0 ? startPos - 1 + Board.TotalNumberOfPositions : startPos - 1
            };
            Players[i] = player;
        }
        return Players;
    }

    /// <summary>
    /// Throw dice, return the pieces that can move. 
    /// </summary>
    /// <returns>Movable pieces.</returns>
    public void ThrowDice()
    {
        Random random = new();
        // Throw dice
        DiceNow = random.Next(1, 7);
        // Check which pieces can move. Note: Player can not move piece if target position is occupied by his own piece. 
        PiecesThatCanMove = Board.MovablePositions(PlayerInTurn, DiceNow);
        if (PiecesThatCanMove.Count == 0)
        {
            NextPlayersTurn();
        }
    }

    public bool Move(int oldPosition)
    {
        return Move(Board.Positions[oldPosition]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldPosition"></param>
    /// <param name="diceNumber"></param>
    /// <returns>Has the player won</returns>
    public bool Move(Position oldPosition)
    {
        // Move the selected piece
        var newPosition = Board.CalculateNewPosition(PlayerInTurn, DiceNow, oldPosition);
        var newPositionIndex = Board.GetIndexOf(newPosition);

        // If there was opponent's piece, move opponent to base
        if (!(newPosition.IsVacant().isVacant))
        {
            Board.MovePieceToBase(newPosition);
        }

        oldPosition.MovePlayerTo(newPosition); // TODO: Can this fail??

        // If all pieces are in safe, player in turn wins
        if (Board.Positions.Select(pos => pos).Where(pos => pos.PositionOccupiedBy(PlayerInTurn)).All(pos => pos is Safe))
        {
            GameOver = true;
            return true;
        }

        // If dice showed 6, repeat the Turn with the same player. 
        if (DiceNow == 6)
        {
            ThrowDice();
            return false;
        }

        NextPlayersTurn();
        return false;
    }

    private void NextPlayersTurn()
    {
        // Change player in turn to the next player that is still in the game. 
        PlayerInTurn = Players[(Array.IndexOf(Players, PlayerInTurn) + 1) % 4];
    }

    public string PrintPositions(Player player)
    {
        return string.Join(", ", Board.Positions.Select(pos => pos).Where(pos => pos.PositionOccupiedBy(player)).Select(pos => Board.GetIndexOf(pos)));
    }
}
