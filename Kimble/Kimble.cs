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



    public Piece[] PiecesToMove(Player player, int diceNumber)
    {
        var pieces = player.Pieces.Select(piece => piece).Where(piece => !piece.InSafe && !piece.InBase).ToList();
        int i = 0;
        while (i < pieces.Count())
        {

            for (int j = 0; j < diceNumber; j++)
            {
                
            }
            i++;
        }
        return pieces.ToArray();
    }

    public bool Turn()
    {
        Random random = new Random();
        // Throw dice
        int number = random.Next(7);
        // Check which pieces can move. Note: Player can not move piece if target position is occupied by his own piece. 

        // Move selected piece
        // If there was enemy, move enemy to base
        // If all pieces are in safe, player in turn wins
        // If dice showed 6, repeat Turn
        // Change player in turn to the next player that is still in the game. 
        return false;
    }
}
