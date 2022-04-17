﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Piece
{
    public Player Owner { get; init; }
    public Position Position { get; private set; } = new() { Index = -1};
    public bool InBase = true;
    public bool InSafe = false;

    public Piece(Player owner)
    {
        Owner = owner;
    }

    public void MoveToBase()
    {
        InBase = true;
        Position = null;
    }
}
