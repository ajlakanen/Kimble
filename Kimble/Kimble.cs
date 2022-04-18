using System;
using System.Collections.Generic;
using System.Linq;

namespace Kimble;

internal class Kimble
{
    private readonly Player[] players = new Player[4];
    private readonly Board board;
    public Player playerInTurn { get; private set; }
    public bool GameOver = false;

    public Kimble()
    {
        players = MakePlayers();
        board = new Board(players);
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
                StartingPosition = startPos
            };
            players[i] = player;
        }
        return players;
    }

    public void Initialize()
    {


        // Red player starts.
        // TODO: Make better player selection procedure. 
        playerInTurn = players[0];
    }


    /// <summary>
    /// Find the player's pieces that can move
    /// to a particular position. 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="diceNumber"></param>
    /// <returns></returns>
    public Piece[] PiecesThatCanMove(Player player, int diceNumber)
    {
        List<Piece> pieces;
        if (diceNumber == 6) pieces = player.Pieces.ToList();
        else pieces = player.Pieces.Select(piece => piece).Where(piece => !piece.InBase).ToList();

        int i = 0;
        while (i < pieces.Count)
        {
            Position candidate = pieces[i].Position;
            // From base the piece can go only to the starting position
            if (pieces[i].InBase) candidate = board.Positions[player.StartingPosition];
            else // Other positions have to be calculated. 
            {
                candidate = CalculateNewPosition(player, diceNumber, candidate);
            }
            // Player can not go to others' safes.
            // TODO: if (!candidate.CanPlayerMove(player)) pieces.RemoveAt(i);
            // If player's own piece is in the position, player can not move there. 
            if (candidate.PieceInPosition != null && candidate.PieceInPosition.Owner == player) pieces.RemoveAt(i);
            // TODO: Player should be able to move forward in the safe.
            else i++;
        }
        return pieces.ToArray();
    }

    private Position CalculateNewPosition(Player player, int diceNumber, Position candidate)
    {
        for (int j = 0; j < diceNumber; j++)
            candidate = board.GiveNextPosition(player, candidate);
        return null;
    }

    /// <summary>
    /// Throw dice, return the pieces that can move. 
    /// </summary>
    /// <returns>Movable pieces.</returns>
    public (int diceNumber, Piece[]) ThrowDice()
    {
        Random random = new Random();
        // Throw dice
        int diceNumber = random.Next(7);
        // Check which pieces can move. Note: Player can not move piece if target position is occupied by his own piece. 
        var piecesThatCanMove = PiecesThatCanMove(playerInTurn, diceNumber);
        if (piecesThatCanMove.Length == 0)
        {
            NextPlayersTurn();
            return (diceNumber, null);
        }
        return (diceNumber, piecesThatCanMove);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pieceToMove"></param>
    /// <param name="diceNumber"></param>
    /// <returns>Has the player won</returns>
    public bool Move(Piece pieceToMove, int diceNumber)
    {
        // TODO: Interact with player
        // Move the selected piece
        // var pieceToMove = piecesThatCanMove.First();

        // If there was opponent's piece, move opponent to base
        var newPosition = CalculateNewPosition(playerInTurn, diceNumber, pieceToMove.Position);
        if (!(newPosition.IsVacant()).isVacant)
        {
            board.MovePieceToBase(newPosition);
            newPosition.InsertPiece(pieceToMove); // TODO: Can this fail??
        }

        // If all pieces are in safe, player in turn wins
        if (playerInTurn.Pieces.All(piece => piece.InSafe))
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
        playerInTurn = players[(Array.IndexOf(players, playerInTurn) + 1) % 4];
    }
}
