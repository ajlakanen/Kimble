using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Home : Position, HomeOrSafe
{
    public Player OwnedBy { get; init; }

    public Home(Player player)
    {
        OwnedBy = player;
        base.PlayerInPosition = player;
    }
}
