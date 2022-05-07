using Jypeli;
using System;

namespace Kimble;

public class KimbleGame : Game
{
    Kimble kimble;
    UI ui;

    public override void Begin()
    {
        SetWindowSize(1920, 1080);
        CenterWindow();
        kimble = new();
        ui = new(this, kimble);
        ui.Dice.DiceAnimationComplete += () => { Mouse.Enable(MouseButton.Left); };
        ui.Dice.DiceAnimationComplete += () => ThrowDice();
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Listener l = Mouse.ListenOn(ui.Dice, MouseButton.Left, ButtonState.Pressed, () =>
        {
            kimble.DiceNow = ui.Dice.Throw();
            Mouse.Disable(MouseButton.Left);
        }, null);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    public event EventHandler MovablePieceSelected;

    private void ThrowDice()
    {
        string movablesStr = "";
        var positionsThatCanMove = kimble.GetPositionsThatCanMove();
        MessageDisplay.Add($"{kimble.PlayerInTurn.Color} heitti: {kimble.DiceNow}");
        kimble.PiecesThatCanMove.ForEach(p => movablesStr += kimble.Board.GetIndexOf(p.oldPosition) + ", ");
        ui.UpdateDiceLabel($"Dice shows: {kimble.DiceNow}");
        ui.UpdateLabels();
        //GameObject[] movables = ui.FlashMovables(MovablePieceSelected);
        GameObject[] piecesThatCanMove = ui.GetPiecesThatCanMove();
        MovablePieceSelected = ui.FlashMovables(MovablePieceSelected);

        foreach (GameObject item in piecesThatCanMove)
        {
            Mouse.ListenOn(item, MouseButton.Left, ButtonState.Pressed, () =>
            {
                MessageDisplay.Add("Moi");
                MovablePieceSelected?.Invoke(this, EventArgs.Empty);
            }, null);
        }

        if (kimble.PiecesThatCanMove.Count != 0)
        {
            ui.UpdateMovables($"You can move: {movablesStr}");
            /*
            InputWindow iw = SelectAndMove(movablesStr);
            iw.Closed += delegate
            {
                ui.UpdateLabels();
                ui.UpdateMovables("");
                if (kimble.DiceNow == 6)
                {
                    MessageDisplay.Add("Heitä uudestaan");
                    ui.UpdateDiceLabel("");
                    return;
                }
                else NewTurn();
            };
            */
        }
        else
        {
            ui.UpdateMovables("You can not move.");
            NewTurn();
        }        
    }

    private void NewTurn()
    {
        kimble.NextPlayer();
        ui.UpdateMovables("");
        ui.UpdateDiceLabel("");
        Timer.SingleShot(1.0, () =>
        {       
            ui.UpdatePointer();

            ui.UpdateLabels();
            MessageDisplay.Add($"Vuorossa nyt: {kimble.PlayerInTurn.Color}");
        });
    }

    private InputWindow SelectAndMove(string s)
    {
        InputWindow iw = new(s);
        Add(iw);
        iw.TextEntered += delegate (InputWindow iw)
        {
            int selected = int.Parse(iw.InputBox.Text);
            Position oldPos = kimble.Board.Positions[selected];
            Position newPos = kimble.PiecesThatCanMove.Find(x => x.oldPosition == oldPos).newPosition;
            MessageDisplay.Add($"Moved piece from position {selected}");
            kimble.Move(oldPos, newPos, ui.MovePiece);
        };
        return iw;
    }
}