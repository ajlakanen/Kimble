using System;
using System.Collections.Generic;
using System.Linq;

namespace Kimble;

internal class Kimble
{
    private readonly Player[] players = new Player[4];
    public readonly Board Board;
    public Player PlayerInTurn { get; private set; }
    public bool GameOver = false;

    public Kimble()
    {
        players = MakePlayers();
        PlayerInTurn = players[0];
        Board = new Board(players);
    }

    private Player[] MakePlayers()
    {
        for (int i = 0; i < Rules.colorsAndStartingPositions.Length; i++)
        {
            // Each player has his own color and starting position.
            int startPos = Rules.colorsAndStartingPositions[i].startingPosition;
            Player player = new()
            {
                PlayerColor = Rules.colorsAndStartingPositions[i].color,
                StartingPosition = startPos,
                LastSafePosition = startPos + 14
            };
            players[i] = player;
        }
        return players;
    }

    public void Initialize()
    {
        // Red player starts.
        // TODO: Make better player selection procedure. 
        PlayerInTurn = players[0];
    }

    

    

    /// <summary>
    /// Throw dice, return the pieces that can move. 
    /// </summary>
    /// <returns>Movable pieces.</returns>
    public (int diceNumber, Position[]) ThrowDice()
    {
        Random random = new();
        // Throw dice
        int diceNumber = random.Next(7);
        // Check which pieces can move. Note: Player can not move piece if target position is occupied by his own piece. 
        var piecesThatCanMove = Board.MovablePositions(PlayerInTurn, diceNumber);
        if (piecesThatCanMove.Length == 0)
        {
            NextPlayersTurn();
            return (diceNumber, piecesThatCanMove);
        }
        return (diceNumber, piecesThatCanMove);
    }

    public bool Move(int oldPosition, int diceNumber)
    {
        return Move(Board.Positions[oldPosition], diceNumber);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldPosition"></param>
    /// <param name="diceNumber"></param>
    /// <returns>Has the player won</returns>
    public bool Move(Position oldPosition, int diceNumber)
    {
        // Move the selected piece
        var newPosition = Board.CalculateNewPosition(PlayerInTurn, diceNumber, oldPosition);

        // If there was opponent's piece, move opponent to base
        if (!(newPosition.IsVacant()).isVacant)
        {
            Board.MovePieceToBase(newPosition);
            newPosition.MovePlayerTo(oldPosition); // TODO: Can this fail??
        }

        // If all pieces are in safe, player in turn wins
        if (PlayerInTurn.Pieces.All(piece => piece.InSafe))
        {
            GameOver = true;
            return true;
        }

        // If dice showed 6, repeat the Turn with the same player. 
        if (diceNumber == 6) ThrowDice();

        NextPlayersTurn();
        return false;
    }

    private void NextPlayersTurn()
    {
        // Change player in turn to the next player that is still in the game. 
        PlayerInTurn = players[(Array.IndexOf(players, PlayerInTurn) + 1) % 4];
    }
}
