﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Position
{
    protected bool isVacant = true;

    /// <summary>
    /// Piece that occupies the position. 
    /// </summary>

    private Player playerInPosition;
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

    public bool PositionOccupiedBy(Player player)
    {
        if (PlayerInPosition == null) return false;
        else if (PlayerInPosition == player) return true;
        else return false;
    }

    public void MovePlayerTo(Position newPosition)
    {
        if (newPosition.PlayerInPosition != null) throw new Exception("Player already in new position");
        newPosition.PlayerInPosition = this.PlayerInPosition;
        this.PlayerInPosition = null;
    }
}
