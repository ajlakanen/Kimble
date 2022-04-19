using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kimble;
using Jypeli;

namespace Kimble;

internal class UI
{
    Kimble kimble;
    Game game;

    public UI(Game game, Kimble kimble)
    {
        this.game = game;
        this.kimble = kimble;
    }

    public Dictionary<Player, Label> CreateLabels()
    {
        Dictionary<Player, Label> playerPositionLabels = new();
        int x = -200;
        int y = -200;

        Dictionary<Color, Jypeli.Color> colors = new()
        {
            { Color.Red, Jypeli.Color.Red },
            { Color.Green, Jypeli.Color.Green },
            { Color.Blue, Jypeli.Color.Blue },
            { Color.Yellow, Jypeli.Color.Yellow }
        };

        foreach (var player in kimble.Players)
        {
            Label label = new()
            {
                TextColor = colors[player.Color],
                Text = $"{player.Color.Stringify()}: {kimble.PrintPositions(player)}",
                Color = Jypeli.Color.Black
            };
            label.Left = x;
            label.Top = y;
            y -= (int)(label.Height * 1.5);
            playerPositionLabels.Add(player, label);
            game.Add(label);
        }
        return playerPositionLabels;
    }
}
