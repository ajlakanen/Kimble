using System.Collections.Generic;

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
            case Color.Red: return Jypeli.Color.Red;
            case Color.Green: return Jypeli.Color.Green;
            case Color.Yellow: return Jypeli.Color.Yellow;
            case Color.Blue: return Jypeli.Color.Blue;
            default: break;
        }
        return Jypeli.Color.LightBlue;
    }

    public static string Stringify(this Color color, string language)
    {
        Dictionary<string, Dictionary<Color, string>> strings = new()
        {
            {"fi", new()
                {
                    {Color.Red, "Punainen" },
                    {Color.Green, "Vihreä" },
                    {Color.Yellow, "Keltainen" },
                    {Color.Blue, "Sininen" }
                }
            },
            {"en", new()
                {
                    {Color.Red, "Red"},
                    {Color.Green, "Green" },
                    {Color.Yellow, "Yellow" },
                    {Color.Blue, "Blue" }
                }
            }
        };

        return strings[language][color];
    }
}