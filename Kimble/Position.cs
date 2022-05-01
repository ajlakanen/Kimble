using System;

namespace Kimble;

/// <summary>
/// Position
/// </summary>
public class Position
{
    /// <summary>
    /// Is position vacant
    /// </summary>
    public bool IsVacant { get; private set; } = true;

    private Player playerInPosition;

    /// <summary>
    /// Player that is currently in the position.
    /// </summary>
    public Player PlayerInPosition
    {
        get
        {
            return playerInPosition;
        }
        set
        {
            playerInPosition = value;
            if (value == null) IsVacant = true;
            else IsVacant = false;
        }
    }

    /// <summary>
    /// Position
    /// </summary>
    public Position()
    {
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
