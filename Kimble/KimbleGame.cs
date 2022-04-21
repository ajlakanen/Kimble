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
        string movablesStr = Throw();
        ui.UpdateLabels();
        ui.UpdateMovables(movablesStr);
        InputWindow iw = SelectAndMove(movablesStr);
        iw.Closed += delegate
        {
            ui.UpdateLabels();
            NewTurn(kimble);
        };
    }

    private InputWindow SelectAndMove(string s)
    {
        InputWindow iw = new(s);
        Add(iw);
        iw.TextEntered += delegate (InputWindow iw)
        {
            int selected = int.Parse(iw.InputBox.Text);
            MessageDisplay.Add(selected + "");
            //kimble.Board.MovePieceToNewPosition(selected, selected + diceNumber);
            kimble.Move(selected);
        };
        return iw;
    }

    private string Throw()
    {
        do
        {
            kimble.ThrowDice();
            MessageDisplay.Add($"{kimble.PlayerInTurn.Color} heitti: {kimble.DiceNow}");
            if (kimble.PiecesThatCanMove.Count == 0) MessageDisplay.Add($"Vuoro vaihtuu. Vuorossa nyt: {kimble.PlayerInTurn.Color}");
        } while (kimble.PiecesThatCanMove.Count == 0);
        // return kimble.PrintPositions(kimble.PlayerInTurn);
        string s = "";
        kimble.PiecesThatCanMove.ForEach(p => s += kimble.Board.GetIndexOf(p.aboutToMove) + ", ");
        return s;
    }
}

