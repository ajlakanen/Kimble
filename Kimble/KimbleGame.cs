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
        Piece[] pieces;
        do
        {
            (int diceNumber, pieces) = kimble.ThrowDice();
            MessageDisplay.Add($"{kimble.playerInTurn.PlayerColor} heitti: {diceNumber}");
            if (pieces == null) MessageDisplay.Add($"Vuoro vaihtuu. Vuorossa nyt: {kimble.playerInTurn.PlayerColor}");
        } while (pieces == null);
        string s = "";
        pieces.ForEach(p => s += p.Position + ", ");
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

