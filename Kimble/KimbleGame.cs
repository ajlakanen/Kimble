using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

namespace Kimble;

public class KimbleGame : Game
{
    public override void Begin()
    {
        Kimble kimble = new Kimble();
        Position[] movablePositions;
        do
        {
            (int diceNumber, movablePositions) = kimble.ThrowDice();
            MessageDisplay.Add($"{kimble.playerInTurn.PlayerColor} heitti: {diceNumber}");
            if (movablePositions.Length==0) MessageDisplay.Add($"Vuoro vaihtuu. Vuorossa nyt: {kimble.playerInTurn.PlayerColor}");
        } while (movablePositions.Length == 0);
        string s = "";
        movablePositions.ForEach(p => s += kimble.Board.GetIndexOf(p) + ", ");
        InputWindow iw = new InputWindow(s);
        Add(iw);
        string selected = "";
        iw.TextEntered += delegate (InputWindow iw)
        {
            selected = iw.InputBox.Text;
        };

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
}

