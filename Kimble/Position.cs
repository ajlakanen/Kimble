using System;

namespace Kimble;

/// <summary>
/// Position
/// </summary>
public class Position
{
    protected bool isVacant = true;

    /// <summary>
    /// Piece that occupies the position. 
    /// </summary>

    private Player playerInPosition;
    /// <summary>
    /// Player that is currently in the position.
    /// </summary>
    protected Player PlayerInPosition
    {
        get
        {
            return playerInPosition;
        }
        set
        {
            playerInPosition = value;
            if (value == null) isVacant = true;
            else isVacant = false;
        }
    }

    /// <summary>
    /// Position
    /// </summary>
    public Position()
    {
    }

    /// <summary>
    /// Is position vacant. If not vacant, return also the piece in the position. 
    /// </summary>
    /// <returns></returns>
    public (bool isVacant, Player player) IsVacant()
    {
        return isVacant ? (isVacant, null) : (isVacant, PlayerInPosition);
    }

    /// <summary>
    /// Is position occupied by a player.
    /// </summary>
    /// <param name="player">Player</param>
    /// <returns>Is occupied</returns>
    public bool PositionOccupiedBy(Player player)
    {
        if (PlayerInPosition == null) return false;
        else if (PlayerInPosition == player) return true;
        else return false;
    }

    /// <summary>
    /// Move player from old position to new position.
    /// </summary>
    /// <param name="newPosition"></param>
    /// <exception cref="Exception"></exception>
    public void MovePlayerTo(Position newPosition)
    {
        if (newPosition.PlayerInPosition != null) throw new Exception("Player already in new position");
        newPosition.PlayerInPosition = this.PlayerInPosition;
        this.PlayerInPosition = null;
    }
}
