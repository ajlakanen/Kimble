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
    public int PiecesInBase { get; set; }
    public int PiecesInSafe { get; set; }
    public bool IsPlaying { get; set; }

    public Piece[] Pieces { get; private set; }

    public Player()
    {
        Pieces = new Piece[]
        {
            new Piece(this),
            new Piece(this),
            new Piece(this),
            new Piece(this)
        };
    }
}
