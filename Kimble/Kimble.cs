using System;
using System.Collections.Generic;
using System.Linq;

namespace Kimble;

enum GameState
{
    ReadyToRollDice,
    DiceRolling,
    ChoosingPieceToMove,
    TurnChanging
}

/// <summary>
/// Kimble main logic class.
/// </summary>
internal class Kimble
{
    public GameState GameState { get; set; }

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
    public delegate void MoveHandler(Player player, Position oldPos, Position newPos);//, GameObject g);
 
    //public delegate void MoveHandler(GameObject g);

    /// <summary>
    /// Initialize players and board.
    /// </summary>
    public Kimble()
    {
        Players = CreatePlayers();
        PlayerInTurn = Players[0];
        Board = new Board(Players);
        GameState = GameState.ReadyToRollDice;
    }

    private Player[] CreatePlayers()
    {
        for (int i = 0; i < Rules.colorsAndStartingPositions.Length; i++)
        {
            // Each player has his own color and starting position.
            int startPos = Rules.colorsAndStartingPositions[i].startingPosition;
            Player player = new()
            {
                Color = Rules.colorsAndStartingPositions[i].color,
                HomeStartsFrom = startPos,
                StartingAngle = Math.PI / 2 - (i * Math.PI / 2),
                SafeEnd = startPos - 1 < 0 ? startPos - 1 + Board.TotalNumberOfPositions : startPos - 1
            };
            Players[i] = player;
        }
        return Players;
    }

    public int ThrowDice()
    {
        GameState = GameState.DiceRolling;
        // This is the actual returned number.
        int value = new Random().Next(5, 7);
        DiceNow = value;
        return value;
    }

    /// <summary>
    /// Throw dice, return the pieces that can move. 
    /// </summary>
    /// <returns>Movable pieces.</returns>
    public List<(Position, Position)> GetPositionsThatCanMove()
    {
        //Random random = new();
        //DiceNow = random.Next(1, 7);
        PiecesThatCanMove = Board.MovablePositions(PlayerInTurn, DiceNow);
        return PiecesThatCanMove;
    }

    /// <summary>
    /// Move piece
    /// </summary>
    /// <param name="oldPosition">Old position</param>
    /// <param name="newPosition">New position</param>
    /// <param name="moveHandler">UI move handler</param>
    /// <returns>Has the player won</returns>
    //public bool Move(Position oldPosition, Position newPosition, MoveHandler moveHandler)
    public bool Move(Position oldPosition, Position newPosition, Action moveHandler)
    {
        // If there was opponent's piece, move opponent to home
        if (!(newPosition.IsVacant))
        {
            Home home = Board.GetVacantHomePosition(newPosition.PlayerInPosition);
            Player playerToMove = newPosition.PlayerInPosition;
            newPosition.MovePlayerTo(home);
        }

        oldPosition.MovePlayerTo(newPosition);
        //moveHandler(newPosition.PlayerInPosition, oldPosition, newPosition);
        moveHandler?.Invoke();

        // If all pieces are in safe, player in turn wins
        if (Board.Positions.Select(pos => pos).Where(pos => pos.PlayerInPosition == PlayerInTurn).All(pos => pos is Safe))
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