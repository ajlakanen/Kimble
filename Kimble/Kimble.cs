using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

internal class Kimble
{
    private Player[] players;
    private Board board;
    private Player playerInTurn;
    private readonly (Color color, int startingPosition)[] colorsAndStartingPositions =
    {
        (Color.Red, 0),
        (Color.Blue, 10),
        (Color.Yellow, 20),
        (Color.Green, 30)
    };


    public void Initialize()
    {
        players = new Player[4];
        board = new Board();

        // Let's make the board
        for (int i = 0; i < colorsAndStartingPositions.Length; i++)
        {
            // Each player has his own color and starting position.
            Player player = new()
            {
                PlayerColor = colorsAndStartingPositions[i].color,
                StartingPosition = colorsAndStartingPositions[i].startingPosition
            };
            players[i] = player;

            // Initialize basic positions for each player
            for (int j = colorsAndStartingPositions[i].startingPosition; j < colorsAndStartingPositions[i].startingPosition + 7; j++)
            {
                Position position = new Position();
                board.Positions[j] = position;
            }

            // Initialize safe positions for each player
            int safeStartsFrom = (players[i].StartingPosition + Board.TotalNumberOfPositions - 4) % Board.TotalNumberOfPositions;
            for (int j = safeStartsFrom; j < safeStartsFrom + 4; j++)
            {
                Position position = new Position(player);
                board.Positions[j] = position;
            }
        }

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
            if (!candidate.CanPlayerMove(player)) pieces.RemoveAt(i);
            if (candidate.PieceInPosition.Owner == player) pieces.RemoveAt(i);
            // TODO: Player should be able to move forward in the safe.
            else i++;
        }
        return pieces.ToArray();
    }

    private Position CalculateNewPosition(Player player, int diceNumber, Position candidate)
    {
        for (int j = 0; j < diceNumber; j++) candidate = board.GiveNextPosition(player, candidate);
        return candidate;
    }

    public bool Turn()
    {
        Random random = new Random();
        // Throw dice
        int diceNumber = random.Next(7);
        // Check which pieces can move. Note: Player can not move piece if target position is occupied by his own piece. 
        var piecesThatCanMove = PiecesThatCanMove(playerInTurn, diceNumber);

        // TODO: Interact with player
        // Move selected piece
        var pieceToMove = piecesThatCanMove.First();
        // If there was enemy, move enemy to base
        var newPosition = CalculateNewPosition(playerInTurn, diceNumber, pieceToMove.Position);
        if(!(newPosition.IsVacant()).isVacant)
        {
            newPosition.PieceInPosition.MoveToBase();
            newPosition.InsertPiece(pieceToMove); // TODO: Can this fail??
        }
        // If all pieces are in safe, player in turn wins
        // If dice showed 6, repeat Turn
        // Change player in turn to the next player that is still in the game. 
        return false;
    }
}
