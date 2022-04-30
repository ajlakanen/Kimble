using System;
using System.Collections.Generic;
using System.Linq;

namespace Kimble;

/// <summary>
/// Kimble main class.
/// </summary>
internal class Kimble
{
    /// <summary>
    /// Players.
    /// </summary>
    public readonly Player[] Players = new Player[4];

    /// <summary>
    /// Board.
    /// </summary>
    public readonly Board Board;

    /// <summary>
    /// Player currently in turn. 
    /// </summary>
    public Player PlayerInTurn { get; private set; }

    /// <summary>
    /// Dice now. 
    /// </summary>
    public int DiceNow { get; set; }

    /// <summary>
    /// Pieces that are currently movable (oldPosition) and their new position counterparts. 
    /// </summary>
    public List<(Position oldPosition, Position newPosition)> PiecesThatCanMove { get; private set; }

    /// <summary>
    /// Game over. 
    /// </summary>
    public bool GameOver = false;

    /// <summary>
    /// Move handler. Function is passed by the UI and used to move the UI pieces. 
    /// </summary>
    /// <param name="player">Player</param>
    /// <param name="oldPos">Old position</param>
    /// <param name="newPos">New position.</param>
    public delegate void MoveHandler(Player player, Position oldPos, Position newPos);

    /// <summary>
    /// Initialize players and board.
    /// </summary>
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
                StartingAngle = Math.PI / 2 - (i * Math.PI / 2),
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
    public List<(Position, Position)> ThrowDice()
    {
        Random random = new();
        // Throw dice
        DiceNow = random.Next(1, 7);
        // Check which pieces can move. 
        PiecesThatCanMove = Board.MovablePositions(PlayerInTurn, DiceNow);
        return PiecesThatCanMove;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldPosition"></param>
    /// <param name="diceNumber"></param>
    /// <returns>Has the player won</returns>
    public bool Move(Position oldPosition, Position newPosition, MoveHandler handler)
    {
        // Move the selected piece
        var newPositionIndex = Board.GetIndexOf(newPosition);

        // If there was opponent's piece, move opponent to base
        if (!(newPosition.IsVacant()))
        {
            // Board.MovePieceToBase(newPosition);
            Base @base = Board.GetVacantBasePosition(newPosition.PlayerInPosition);
            Player playerToMoved = newPosition.PlayerInPosition;
            newPosition.MovePlayerTo(@base);
            handler(playerToMoved, newPosition, @base);
        }

        oldPosition.MovePlayerTo(newPosition); // TODO: Can this fail??
        handler(newPosition.PlayerInPosition, oldPosition, newPosition);

        // If all pieces are in safe, player in turn wins
        if (Board.Positions.Select(pos => pos).Where(pos => pos.PositionOccupiedBy(PlayerInTurn)).All(pos => pos is Safe))
        {
            GameOver = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Change player in turn to the next player that is still in the game. 
    /// </summary>
    public void NextPlayer()
    {
        PlayerInTurn = Players[(Array.IndexOf(Players, PlayerInTurn) + 1) % 4];
    }

}