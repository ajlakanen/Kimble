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
    UI ui;
    readonly Dictionary<Player, string> playerPositions = new();

    public override void Begin()
    {
        kimble = new();
        ui = new(this, kimble);
        ui.CreateLabels(-200, -200);
        NewTurn(kimble);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    private void NewTurn(Kimble kimble)
    {
        string movablePositions = Throw(out int diceNumber);
        ui.UpdateLabels();
        ui.UpdateMovables(movablePositions);
        InputWindow iw = SelectAndMove(diceNumber, movablePositions);
        iw.Closed += delegate
        {
            ui.UpdateLabels();
            NewTurn(kimble);
        };
    }

    private InputWindow SelectAndMove(int diceNumber, string s)
    {
        InputWindow iw = new(s);
        Add(iw);
        iw.TextEntered += delegate (InputWindow iw)
        {
            int selected = int.Parse(iw.InputBox.Text);
            MessageDisplay.Add(selected + "");
            //kimble.Board.MovePieceToNewPosition(selected, selected + diceNumber);
            kimble.Move(selected, diceNumber);
        };
        return iw;
    }

    private string Throw(out int diceNumber)
    {
        Position[] movablePositions;
        do
        {
            (diceNumber, movablePositions) = kimble.ThrowDice();
            MessageDisplay.Add($"{kimble.PlayerInTurn.Color} heitti: {diceNumber}");
            if (movablePositions.Length == 0) MessageDisplay.Add($"Vuoro vaihtuu. Vuorossa nyt: {kimble.PlayerInTurn.Color}");
        } while (movablePositions.Length == 0);
        // return kimble.PrintPositions(kimble.PlayerInTurn);
        string s = "";
        movablePositions.ForEach(p => s += kimble.Board.GetIndexOf(p) + ", ");
        return s;
    }
}

