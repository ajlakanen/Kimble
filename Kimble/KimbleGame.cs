using Jypeli;

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
        ui.CreateLabels(-800, 0);
        ui.CreateBoardLayout();
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Space, ButtonState.Pressed, ThrowDice, null);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    private void ThrowDice()
    {
        string movablesStr = "";
        var piecesThatCanMove = kimble.ThrowDice();
        MessageDisplay.Add($"{kimble.PlayerInTurn.Color} heitti: {kimble.DiceNow}");
        kimble.PiecesThatCanMove.ForEach(p => movablesStr += kimble.Board.GetIndexOf(p.aboutToMove) + ", ");
        ui.UpdateDiceLabel($"Dice shows: {kimble.DiceNow}");
        ui.UpdateLabels();
        if (kimble.PiecesThatCanMove.Count != 0)
        {
            ui.UpdateMovables($"You can move: {movablesStr}");
            InputWindow iw = SelectAndMove(movablesStr);
            iw.Closed += delegate
            {
                ui.UpdateLabels();
                ui.UpdateMovables("");
                if (kimble.DiceNow == 6)
                {
                    MessageDisplay.Add("Heitä uudestaan");
                    ui.UpdateDiceLabel("");
                    //ThrowDice();
                    return;
                }
                else NewTurn();
            };
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
        MessageDisplay.Add($"Vuoro vaihtuu.");
        Timer.SingleShot(1.0, () =>
        {
            ui.UpdateLabels();
            MessageDisplay.Add($"Vuorossa nyt: {kimble.PlayerInTurn.Color}");
        });
        // return kimble.PrintPositions(kimble.PlayerInTurn);
    }

    private InputWindow SelectAndMove(string s)
    {
        InputWindow iw = new(s);
        Add(iw);
        iw.TextEntered += delegate (InputWindow iw)
        {
            int selected = int.Parse(iw.InputBox.Text);
            MessageDisplay.Add($"Moved piece from position {selected}");
            //kimble.Board.MovePieceToNewPosition(selected, selected + diceNumber);
            kimble.Move(selected);
            ui.MovePiece(selected, kimble.DiceNow);
        };
        return iw;
    }

}

