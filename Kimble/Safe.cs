using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Safe : Position, HomeOrSafe
{
    public Player OwnedBy { get; init; }

    public Safe(Player player)
    {
        OwnedBy = player;
    }
}
