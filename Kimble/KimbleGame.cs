using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

namespace Kimble;

public class KimbleGame : Game
{
    Kimble kimble;
    Dictionary<Player, string> playerPositions = new Dictionary<Player, string>();
    List<Label> playerPositionLabels = new List<Label>();


    public override void Begin()
    {
        kimble = new();

        CreateLabels(Rules.colorsAndStartingPositions);

        NewTurn(kimble);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    private void CreateLabels((Color color, int startingPosition)[] colorsAndStartingPositions)
    {
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
                TextColor = colors[player.PlayerColor],
                Text = $"{player.PlayerColor.ToString()}: {kimble.PrintPositions(player)}",
                Color = Jypeli.Color.Black
            };
            label.Left = x;
            label.Top = y;
            y -= (int)(label.Height * 1.5);
            Add(label);
        }

    }

    private void NewTurn(Kimble kimble)
    {
        int diceNumber;
        Position[] movablePositions;
        do
        {
            (diceNumber, movablePositions) = kimble.ThrowDice();
            MessageDisplay.Add($"{kimble.PlayerInTurn.PlayerColor} heitti: {diceNumber}");
            if (movablePositions.Length == 0) MessageDisplay.Add($"Vuoro vaihtuu. Vuorossa nyt: {kimble.PlayerInTurn.PlayerColor}");
        } while (movablePositions.Length == 0);
        string s = "";
        movablePositions.ForEach(p => s += kimble.Board.GetIndexOf(p) + ", ");
        InputWindow iw = new(s);
        Add(iw);
        string selected = "";
        iw.TextEntered += delegate (InputWindow iw)
        {
            int selected = int.Parse(iw.InputBox.Text);
            MessageDisplay.Add(selected + "");
            //kimble.Board.MovePieceToNewPosition(selected, selected + diceNumber);
            kimble.Move(selected, diceNumber);
            NewTurn(kimble);
        };
    }
}

