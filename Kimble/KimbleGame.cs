using Jypeli;
using System;
using System.Collections.Generic;

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
        ui.Dice.DiceAnimationComplete += () =>
        {
            //Mouse.Enable(MouseButton.Left);
            kimble.GameState = GameState.ChoosingPieceToMove;
        };
        ui.Dice.DiceAnimationComplete += () => DiceThrown();
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Listener l = Mouse.ListenOn(ui.Dice, MouseButton.Left, ButtonState.Pressed, () =>
        {
            if (kimble.GameState != GameState.ReadyToRollDice) return;
            //kimble.DiceNow = ui.Dice.Throw();
            kimble.ThrowDice();
            ui.Dice.RollAnimation(kimble.DiceNow);
        }, null);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    public event EventHandler MovablePieceSelected;

    public Action PieceMoved;
    private void DiceThrown()
    {
        string movablesStr = "";
        var positionsThatCanMove = kimble.GetPositionsThatCanMove();
        MessageDisplay.Add($"{kimble.PlayerInTurn.Color} heitti: {kimble.DiceNow}");
        kimble.PiecesThatCanMove.ForEach(p => movablesStr += kimble.Board.GetIndexOf(p.oldPosition) + ", ");

        GameObject[] piecesThatCanMove_GO = ui.GetPiecesThatCanMove();
        MovablePieceSelected = ui.AnimateMovables(MovablePieceSelected);

        if (kimble.PiecesThatCanMove.Count != 0)
        {
            List<Listener> listeners = new();
            foreach (GameObject item in piecesThatCanMove_GO)
            {
                Listener l = Mouse.ListenOn(item, MouseButton.Left, ButtonState.Pressed, () =>
                {
                    MovablePieceSelected?.Invoke(this, EventArgs.Empty);
                    MovablePieceSelected = null;
                    MovableSelected(item);
                    listeners.ForEach(x => x.Destroy());
                }, null);
                listeners.Add(l);
            }

            // ui.MovePiece(kimble.PlayerInTurn, oldPos, newPos, ui.MovePiece);
        }
        else
        {
            NewTurn();
        }

        void MovableSelected(GameObject item)
        {
            Position oldPos = ui.GetPositionOf(item);
            Position newPos = kimble.PiecesThatCanMove.Find(x => x.oldPosition == oldPos).newPosition;
            PieceMoved += () => PieceMovedHandler();
            MoveComplete();
            /* 
            if (oldPos is not Home && newPos is not Safe)
            {
                Timer.SingleShot(0.01, () => ui.MoveAlongArc(item, kimble.DiceNow, PieceMoved));
            }
            else
            {
                item.MoveTo(ui.BoardToUIPosition(newPos), 1000, MoveComplete);
                // MoveComplete();
            }
            */

            void MoveComplete()
            {
                //kimble.Move(oldPos, newPos, ui.MovePiece);
                kimble.Move(oldPos, newPos, PieceMoved);
                if (kimble.DiceNow == 6)
                {
                    kimble.GameState = GameState.ReadyToRollDice;
                    MessageDisplay.Add("Heitä uudestaan");
                    return;
                }
                else NewTurn();
            }

            void PieceMovedHandler()
            {
                ui.MovePiece(newPos.PlayerInPosition, oldPos, newPos);
                PieceMoved = null;
            }
        }
    }

    public Action PointerMovedToNewLocation;

    private void NewTurn()
    {
        // kimble.GameState = GameState.ReadyToRollDice;
        kimble.NextPlayer();
        Timer.SingleShot(1.0, () =>
        {
            ui.UpdatePointer(() => kimble.GameState = GameState.ReadyToRollDice);
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
            //kimble.Move(oldPos, newPos);//, ui.MovePiece);
        };
        return iw;
    }
}