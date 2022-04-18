using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Base : Position
{
    public Player OwnedBy { get; init; }

    public Base(Player player)
    {
        OwnedBy = player;
    }
}
