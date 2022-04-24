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
    public static Jypeli.Color ToJypeliColor(this Color color)
    {
        switch (color)
        {   
            case Color.Red:
                return Jypeli.Color.Red;
                break;
            case Color.Green:
                return Jypeli.Color.Green;
                break;
            case Color.Yellow:
                return Jypeli.Color.Yellow;
                break;
            case Color.Blue:
                return Jypeli.Color.Blue;
                break;
            default:
                break;
        }
        return Jypeli.Color.LightBlue;
    }

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
    /// <summary>
    /// Color
    /// </summary>
    public Color Color { get; init; }
    
    /// <summary>
    /// Starting position (board array index).
    /// </summary>
    public int StartingPosition { get; init; }

    public double StartingAngle { get; init; }

    /// <summary>
    /// Last safe position (board array index).
    /// </summary>
    public int LastSafePosition { get; init; }
    
    /// <summary>
    /// Is player still in the game.
    /// </summary>
    public bool InGame { get; protected set; }

    /// <summary>
    /// Player
    /// </summary>
    public Player()
    {
    }
}
