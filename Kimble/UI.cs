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
    const int TotalPositions = Board.TotalNumberOfPositions;
    const int TotalBasicPositions = Board.TotalNumberOfPositions - 4 * (4 + 4);
    const double BasicAngleAdd = (Math.PI * 2) / (double)TotalBasicPositions;
    const double BaseDistance = 360.0;
    const double BasicDistance = 300.0;
    const double SafeDistanceStart = 260.0;
    const double StartAngle = Math.PI / 2;

    readonly Kimble kimble;
    readonly Game game;
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


    private Vector BoardToUIPosition(Safe safe)
    {
        Player player = safe.OwnedBy;
        int baseStartsFrom = player.StartingPosition;
        // int safeStartsFrom = (baseStartsFrom - 4) < 0 ? baseStartsFrom - 4 + Board.TotalNumberOfPositions : (baseStartsFrom - 4) % TotalPositions;
        int safeStartsFrom = player.LastSafePosition - 4;
        double safeStartAngle = StartAngle + ((baseStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI);
        var safes = kimble.Board.Positions.Select(x => x).Where(x => x is Safe s && s.OwnedBy == safe.OwnedBy).ToList();
        int index = safes.IndexOf(safe);
        Vector position = Vector.FromLengthAndAngle(SafeDistanceStart - index * 40, Angle.FromRadians(safeStartAngle));
        return position;
    }

    private Vector BoardToUIPosition(Position position)
    {
        throw new NotImplementedException();
    }

    private Vector BoardToUIPosition(Base @base)
    {
        Player player = @base.OwnedBy;
        int baseStartsFrom = player.StartingPosition;
        double baseStartAngle = StartAngle + ((baseStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI) + 3 * BasicAngleAdd;
        var bases = kimble.Board.Positions.Select(x => x).Where(x => x is Base b && b.OwnedBy == @base.OwnedBy).ToList();
        int index = bases.IndexOf(@base);
        Vector position = Vector.FromLengthAndAngle(BaseDistance, Angle.FromRadians(baseStartAngle - index * BasicAngleAdd));
        return position;
    }

    internal void CreateBoardLayout()
    {
        // Angle from the top going clockwise.
        Position now;

        // Basic positions
        for (int i = 0; i < TotalPositions; i++)
        {
            now = kimble.Board.Positions[i];
            if (now is Base || now is Safe) continue;
            var basicPositions = kimble.Board.Positions.Select(x => x).Where(x => x is not Base && x is not Safe).ToList();
            int index = basicPositions.IndexOf(now);
            Vector position = Vector.FromLengthAndAngle(BasicDistance, Angle.FromRadians(StartAngle - index * BasicAngleAdd));
            DrawPosition(-1, i, 0, position);
        }

        // Bases
        for (int i = 0; i < kimble.Players.Length; i++)
        {
            int baseStartsFrom = kimble.Players[i].StartingPosition;

            for (int j = 0; j < 4; j++)
            {
                Base baseNow = kimble.Board.Positions[baseStartsFrom + j] as Base;
                Vector position = BoardToUIPosition(baseNow);
                DrawPosition(i, baseStartsFrom, j, position);
            }
        }

        // Safes
        for (int i = 0; i < Rules.colorsAndStartingPositions.Length; i++)
        {
            int safeStartsFrom = kimble.Players[i].LastSafePosition - 3;
            for (int j = 0; j < 4; j++)
            {
                Vector position = BoardToUIPosition(kimble.Board.Positions[safeStartsFrom + j] as Safe);
                DrawPosition(i, safeStartsFrom, j, position);
            }
        }

    }

    private void DrawPosition(int i, int baseStartsFrom, int j, Vector position)
    {
        GameObject g = new(20, 20, Shape.Circle)
        {
            Position = position
        };
        GameObject background = new(30, 30, Shape.Circle);
        if (i >= 0)
        {
            background.Color = kimble.Players[i].Color.ToJypeliColor();
        }
        else
        {
            background.Color = Jypeli.Color.White;
        }
        Label l = new(baseStartsFrom + j + "");
        l.Position = position;
        background.Position = position;
        game.Add(l);
        game.Add(g);
        game.Add(background, -1);
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
