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
    GameObject pointer;
    double uiInitialX;
    double uiInitialY;

    public UI(Game game, Kimble kimble)
    {
        this.game = game;
        this.kimble = kimble;
    }

    internal void CreateBoardLayout()
    {
        int total = Board.TotalNumberOfPositions;
        int totalBasicPositions = Board.TotalNumberOfPositions - 4 * (4 + 4);
        // Angle from the top going clockwise.
        const double StartAngle = Math.PI / 2;
        double angle = StartAngle;
        double basicAngleAdd = (Math.PI * 2) / (double)totalBasicPositions;
        double baseDistance = 360.0;
        double basicDistance = 300.0;
        double safeDistanceStart = 260.0;
        double distance = basicDistance;
        Position now;
        Position previous = kimble.Board.Positions[Board.TotalNumberOfPositions - 1];
        Jypeli.Color color;

        for (int i = 0; i < total; i++)
        {
            now = kimble.Board.Positions[i];

            if (now is Base || now is Safe) continue;
            Vector position = Vector.FromLengthAndAngle(basicDistance, Angle.FromRadians(angle));
            GameObject g = new GameObject(20, 20, Shape.Circle);
            Label l = new Label(i + "");
            game.Add(l);
            l.Position = position;
            g.Position = position;
            game.Add(g);
            angle -= basicAngleAdd;
        }

        // Bases
        for (int i = 0; i < kimble.Players.Length; i++)
        {
            int baseStartsFrom = kimble.Players[i].StartingPosition;
            double baseStartAngle = StartAngle + ((baseStartsFrom * 1.0 / total) * 2 * Math.PI) + 3 * basicAngleAdd ;

            for (int j = 0; j < 4; j++)
            {
                now = kimble.Board.Positions[baseStartsFrom + j];
                Vector position = Vector.FromLengthAndAngle(baseDistance, Angle.FromRadians(baseStartAngle - (j * basicAngleAdd) ));
                GameObject g = new GameObject(20, 20, Shape.Circle);
                Label l = new(baseStartsFrom + j + "");
                game.Add(l);
                l.Position = position;
                g.Position = position;
                game.Add(g);
            }
        }

        // Safes
        for (int i = 0; i < Rules.colorsAndStartingPositions.Length; i++)
        {
            int baseStartsFrom = kimble.Players[i].StartingPosition;
            int safeStartsFrom = (baseStartsFrom - 4) < 0 ? baseStartsFrom - 4 + Board.TotalNumberOfPositions : (baseStartsFrom - 4) % total;
            double safeStartAngle = StartAngle + ((baseStartsFrom * 1.0 / total) * 2 * Math.PI);
            for (int j = 0; j < 4; j++)
            {
                Vector position = Vector.FromLengthAndAngle(safeDistanceStart - j * 40, Angle.FromRadians(safeStartAngle));
                GameObject g = new GameObject(20, 20, Shape.Circle);
                Label l = new(safeStartsFrom + j + "");
                game.Add(l);
                l.Position = position;
                g.Position = position;
                game.Add(g);
            }
        }

    }

    public void CreateLabels(double x, double y)
    {
        uiInitialX = x;
        uiInitialY = y;
        Vector bottom = CreatePlayerLabels(x, y);
        CreateDiceLabel(bottom.X, bottom.Y);
        CreateYouCanMoveLabel(300, dice.Y);
        CreatePointer(x, y);
    }

    private void CreateYouCanMoveLabel(double x, double y)
    {
        youCanMove = new Label
        {
            Left = dice.Right + 150,
            Y = y,
            Color = Jypeli.Color.Black,
            TextColor = Jypeli.Color.White,
            Text = ""
        };
        game.Add(youCanMove);
    }

    private void CreatePointer(double x, double y)
    {
        pointer = new GameObject(20, 80, Shape.Triangle);
        pointer.Angle = Angle.FromRadians(-Math.PI / 2);
        pointer.X = x - pointer.Height;
        pointer.Y = y - 10;
        game.Add(pointer);
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
                Text = $"{player.Color.Stringify()} ({player.LastSafePosition}): {kimble.PrintPositions(player)}",
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
            label.Value.Text = $"{label.Key.Color.Stringify()} ({label.Key.LastSafePosition}): {kimble.PrintPositions(label.Key)}";
        }

        pointer.Y = uiInitialY - (Array.IndexOf(kimble.Players, kimble.PlayerInTurn) * dice.Height * 1.5) - 10;
        pointer.X = uiInitialX - pointer.Height;
    }

    public void UpdateDiceLabel(string s)
    {
        dice.Text = s;
    }

    internal void UpdateMovables(string s)
    {
        if (s.Length == 0) youCanMove.Text = "";
        else youCanMove.Text = s;
    }
}
