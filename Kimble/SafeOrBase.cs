using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class SafeOrBase : Position
{
    public Player OwnedBy { get; init; }

    public SafeOrBase(Player player)
    {
        OwnedBy = player;
    }
}
