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

        bool debug = false;

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

        if (debug)
        {
            foreach (KeyValuePair<Player, Dictionary<GameObject, Position>> kv in ui.pieces)
            {
                foreach (KeyValuePair<GameObject, Position> go_pos in kv.Value)
                {
                    Mouse.ListenOn(go_pos.Key, MouseButton.Left, ButtonState.Pressed, () =>
                    {
                        MessageDisplay.Add(kv.Key.Color.ToString() + ": " + go_pos.Value + " " + kimble.Board.GetIndexOf(go_pos.Value));
                        InputWindow iw = new("nopan luku");
                        Add(iw);
                        iw.TextEntered += delegate (InputWindow iw)
                        {
                            int noppa = int.Parse(iw.InputBox.Text);
                            kimble.DiceNow = noppa;
                            Position newPos = kimble.Board.CalculateNewPosition(kv.Key, noppa, go_pos.Value);
                            MovableSelected(go_pos.Key, go_pos.Value, newPos);
                        };
                    }, null);
                }
            }
        }
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
        }
        else
        {
            NewTurn();
        }

    }

    void MovableSelected(GameObject item, Position oldPos = null, Position newPos = null)
    {
        if (oldPos == null) oldPos = ui.GetPositionOf(item);
        if (newPos == null) newPos = kimble.PiecesThatCanMove.Find(x => x.oldPosition == oldPos).newPosition;
        PieceMoved += () => PieceMovedHandler();
        bool opponentFound = kimble.CheckForOpponent(newPos, out Player opponent, out Home opponentHome);
        if (opponentFound)
        {
            ui.MovePiece(oldPos.PlayerInPosition, oldPos, newPos, item, () => MoveOpponent());
            void MoveOpponent()
            {
                ui.MovePiece(opponent, newPos, opponentHome, ui.GetObjectAt(opponent, newPos), () =>
                {
                    kimble.Move(newPos, opponentHome);
                    PieceMovedHandler();
                });
            }
        }
        else
        {
            ui.MovePiece(oldPos.PlayerInPosition, oldPos, newPos, item, PieceMoved);
        }

        MoveComplete();

        void MoveComplete()
        {
        }

        void PieceMovedHandler()
        {
            kimble.Move(oldPos, newPos);
            bool isGameOver = kimble.IsGameOver();
            if (isGameOver)
            {
                MessageDisplay.Add("Game over");
                ClearControls();
                ClearGameObjects();
                ClearTimers();
                Label label = new Label($"Peli päättyy!\n{kimble.PlayerInTurn.Color.Stringify("fi")} voittaa!");
                return;
            }
            PieceMoved = null;
            if (kimble.DiceNow == 6)
            {
                kimble.GameState = GameState.ReadyToRollDice;
                MessageDisplay.Add("Heitä uudestaan");
                return;
            }
            else NewTurn();
        }
    }


    public Action PointerMovedToNewLocation;

    private void NewTurn()
    {
        // kimble.GameState = GameState.ReadyToRollDice;
        kimble.NextPlayer();
        Timer.SingleShot(0.75 / Kimble.Speed, () =>
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
        };
        return iw;
    }
}