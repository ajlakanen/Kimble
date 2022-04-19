using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimble;


/// <summary>
/// Color
/// </summary>
public enum Color
{
    Red, Green, Yellow, Blue
}

/// <summary>
/// Color extensions.
/// </summary>
public static class ColorExtensions
{
    public static string Stringify(this Color color)
    {
        switch (color)
        {
            case Color.Red:
                return "Red";
            case Color.Green:
                return "Green";
            case Color.Yellow:
                return "Yellow";
            case Color.Blue:
                return "Blue";
            default:
                break;
        }
        return color.ToString();
    }
}

/// <summary>
/// Player
/// </summary>
public class Player
{
    public Color Color { get; init; }
    public int StartingPosition { get; init; }
    public int LastSafePosition { get; init; }

    public Player()
    {
    }
}
