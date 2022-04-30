﻿using System;
using System.Collections.Generic;
using System.Linq;
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

    // Dictionary<Color, List<(Position, GameObject)>> pieces;
    readonly Dictionary<Player, List<(Position, GameObject)>> pieces;
    double uiInitialX;
    double uiInitialY;

    public UI(Game game, Kimble kimble)
    {
        this.game = game;
        this.kimble = kimble;
        pieces = new();
    }


    private Vector BoardToUIPosition2(SafeOrBase position)
    {
        Player player = position.OwnedBy;
        int baseStartsFrom = player.StartingPosition;
        double startAngle;
        double angle;
        var safesOrBases = kimble.Board.Positions.Select<Position, SafeOrBase>(x => x as SafeOrBase).Where(x => x is SafeOrBase s && s.OwnedBy == player).ToList();
        int index = safesOrBases.IndexOf(position);
        double distance;
        if (position is Safe)
        {
            startAngle = StartAngle + ((baseStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI);
            angle = startAngle;
            distance = SafeDistanceStart - index * 40;
        }
        else
        {
            startAngle = StartAngle + ((baseStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI) + 3 * BasicAngleAdd;
            distance = BaseDistance;
            angle = startAngle - index * BasicAngleAdd;
        }
        Vector posUI = Vector.FromLengthAndAngle(distance, Angle.FromRadians(angle));
        return posUI;
    }

    private Vector BoardToUIPosition(Base @base)
    {
        Player player = @base.OwnedBy;
        int baseStartsFrom = player.StartingPosition;
        double baseStartAngle = StartAngle - ((baseStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI) + 3 * BasicAngleAdd;
        var bases = kimble.Board.Positions.Select(x => x).Where(x => x is Base b && b.OwnedBy == @base.OwnedBy).ToList();
        int index = bases.IndexOf(@base);
        Vector posUI = Vector.FromLengthAndAngle(BaseDistance, Angle.FromRadians(baseStartAngle - index * BasicAngleAdd));
        return posUI;
    }

    private Vector BoardToUIPosition(Safe safe)
    {
        Player player = safe.OwnedBy;
        int baseStartsFrom = player.StartingPosition;
        double safeStartAngle = StartAngle - ((baseStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI);
        var safes = kimble.Board.Positions.Select(x => x).Where(x => x is Safe s && s.OwnedBy == safe.OwnedBy).ToList();
        int index = safes.IndexOf(safe);
        Vector posUI = Vector.FromLengthAndAngle(SafeDistanceStart - index * 40, Angle.FromRadians(safeStartAngle));
        return posUI;
    }

    internal void MovePiece(Player player, Position oldPosition, Position newPosition)
    {
        var list = pieces[player];
        (Position position, GameObject piece) = pieces[player].Select(x => x).Where(x => x.Item1 == oldPosition).First();
        int index = list.IndexOf((position, piece));
        if (newPosition is Safe) piece.Position = BoardToUIPosition(newPosition as Safe);
        else if (newPosition is Base) piece.Position = BoardToUIPosition(newPosition as Base);
        else piece.Position = BoardToUIPosition(newPosition);
        position = newPosition;
        list[index] = (position, piece);
    }

    private Vector BoardToUIPosition(Position position)
    {
        var basicPositions = kimble.Board.Positions.Select(pos => pos).Where(pos => pos is not Base && pos is not Safe).ToList();
        int index = basicPositions.IndexOf(position);
        Vector posUI = Vector.FromLengthAndAngle(BasicDistance, Angle.FromRadians(StartAngle - index * BasicAngleAdd));
        return posUI;
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
            Vector position = BoardToUIPosition(now);
            DrawPosition(-1, i, 0, position);
        }

        // Bases
        for (int i = 0; i < kimble.Players.Length; i++)
        {
            int baseStartsFrom = kimble.Players[i].StartingPosition;

            for (int j = 0; j < 4; j++)
            {
                Player p = kimble.Players[i];
                Base baseNow = kimble.Board.Positions[baseStartsFrom + j] as Base;
                Vector position = BoardToUIPosition(baseNow);
                DrawPosition(i, baseStartsFrom, j, position);
                GameObject piece = new(20, 20, Shape.Circle)
                {
                    Color = p.Color.ToJypeliColor(),
                    Position = position
                };
                game.Add(piece);
                if (pieces.ContainsKey(p))
                {
                    // pieces[p.Color].Add((baseNow, piece));
                    pieces[p].Add((baseNow, piece));
                }
                else
                {
                    pieces.Add(p, new() { (baseNow, piece) });
                }
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

    private void DrawPosition(int playerIndex, int boardIndex, int add, Vector position)
    {
        GameObject g = new(20, 20, Shape.Circle)
        {
            Position = position
        };
        GameObject background = new(30, 30, Shape.Circle);
        if (playerIndex >= 0)
        {
            background.Color = kimble.Players[playerIndex].Color.ToJypeliColor();
        }
        else
        {
            background.Color = Jypeli.Color.White;
        }

        //Label l = new(boardIndex + add + "");
        //l.Position = position;
        background.Position = position;
        //game.Add(l);
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
        pointer = new GameObject(20, 80, Shape.Triangle)
        {
            Angle = Angle.FromRadians(-Math.PI / 2)
        };
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
                Text = $"{player.Color.Stringify()} ({player.LastSafePosition}): {kimble.Board.PrintPositions(player)}",
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
            label.Value.Text = $"{label.Key.Color.Stringify()} ({label.Key.LastSafePosition}): {kimble.Board.PrintPositions(label.Key)}";
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
