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
    Dictionary<Player, Label> labels;
    Label dice;
    Label youCanMove;

    public UI(Game game, Kimble kimble)
    {
        this.game = game;
        this.kimble = kimble;
    }

    public void CreateLabels(double x, double y)
    {
        Vector bottom = CreatePlayerLabels(x, y);
        CreateDiceLabel(bottom.X, bottom.Y);
    }

    void CreateDiceLabel(double x, double y)
    {
        dice = new Label
        {
            Left = x,
            Top = y,
            Color = Jypeli.Color.Black,
            TextColor = Jypeli.Color.White,
            Text = "Dice shows:  "
        };
        game.Add(dice);

        youCanMove = new Label
        {
            Left = dice.Right + 150,
            Top = y,
            Color = Jypeli.Color.Black,
            TextColor = Jypeli.Color.White,
            Text = "You can move: "
        };


        game.Add(youCanMove);
    }

    Vector CreatePlayerLabels(double x, double y)
    {
        Dictionary<Player, Label> playerPositionLabels = new();

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
        labels = playerPositionLabels;
        return new Vector(x, y);
    }

    internal void UpdateLabels()
    {
        foreach (var label in labels)
        {
            label.Value.Text = $"{label.Key.Color.Stringify()}: {kimble.PrintPositions(label.Key)}";
        }
        dice.Text = $"Dice shows: {kimble.DiceNow}";
    }

    internal void UpdateMovables(string s)
    {
        youCanMove.Text = $"You can move: {s}";
    }
}
