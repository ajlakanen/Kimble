using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Piece
{
    public Player Owner { get; init; }
    public Position position { get; private set; } = new Position();
    public bool InBase = true;
    public bool InSafe = false;

    public Piece(Player owner)
    {
        Owner = owner;
    }
}
