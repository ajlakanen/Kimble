using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;

public class Player
{
    public Color PlayerColor { get; init; }
    public int StartingPosition { get; init; }
    public int LastSafePosition { get; init; }
    public bool IsPlaying { get; set; }

    public Player()
    {
    }
}
