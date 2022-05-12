using System;
using System.Collections.Generic;
using System.Linq;
using Jypeli;

namespace Kimble;

/// <summary>
/// User interface.
/// </summary>
internal class UI
{
    /// <summary>
    /// Dice
    /// </summary>
    public Dice Dice { get; set; }

    const int TotalPositions = Board.TotalNumberOfPositions;
    const int TotalBasicPositions = Board.TotalNumberOfPositions - 4 * (4 + 4);
    const double BasicAngleAdd = (Math.PI * 2) / (double)TotalBasicPositions;
    const double HomeDistance = 360.0;
    const double BasicDistance = 300.0;
    const double SafeDistanceStart = 260.0;
    const double StartAngle = Math.PI / 2;

    readonly Kimble kimble;
    readonly Game game;
    private GameObject pointer;
    //private Dictionary<Player, List<(Position position, GameObject gameObject)>> pieces;
    private Dictionary<Player, Dictionary<GameObject, Position>> pieces;

    public UI(Game game, Kimble kimble)
    {
        this.game = game;
        this.kimble = kimble;
        pieces = new();
        Dice = new Dice(100, 100);
        game.Add(Dice);
        CreatePointer(-800, 0);
        CreateBoardLayout();
    }

    private Vector BoardToUIPosition2(IHomeOrSafe position)
    {
        Player player = position.OwnedBy;
        int homeStartsFrom = player.HomeStartsFrom;
        double startAngle;
        double angle;
        var homesOrSafes = kimble.Board.Positions.Select<Position, IHomeOrSafe>(x => x as IHomeOrSafe).Where(x => x is IHomeOrSafe s && s.OwnedBy == player).ToList();
        int index = homesOrSafes.IndexOf(position);
        double distance;
        if (position is Safe)
        {
            startAngle = StartAngle + ((homeStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI);
            angle = startAngle;
            distance = SafeDistanceStart - index * 40;
        }
        else
        {
            startAngle = StartAngle + ((homeStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI) + 3 * BasicAngleAdd;
            distance = HomeDistance;
            angle = startAngle - index * BasicAngleAdd;
        }
        Vector posUI = Vector.FromLengthAndAngle(distance, Angle.FromRadians(angle));
        return posUI;
    }

    private Vector BoardToUIPosition(Home home)
    {
        Player player = home.OwnedBy;
        int homeStartsFrom = player.HomeStartsFrom;
        double homeStartAngle = StartAngle - ((homeStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI) + 3 * BasicAngleAdd;
        var homes = kimble.Board.Positions.Select(x => x).Where(x => x is Home b && b.OwnedBy == home.OwnedBy).ToList();
        int index = homes.IndexOf(home);
        Vector posUI = Vector.FromLengthAndAngle(HomeDistance, Angle.FromRadians(homeStartAngle - index * BasicAngleAdd));
        return posUI;
    }

    private Vector BoardToUIPosition(Safe safe)
    {
        Player player = safe.OwnedBy;
        int homeStartsFrom = player.HomeStartsFrom;
        double safeStartAngle = StartAngle - ((homeStartsFrom * 1.0 / TotalPositions) * 2 * Math.PI);
        var safes = kimble.Board.Positions.Select(x => x).Where(x => x is Safe s && s.OwnedBy == safe.OwnedBy).ToList();
        int index = safes.IndexOf(safe);
        Vector posUI = Vector.FromLengthAndAngle(SafeDistanceStart - index * 40, Angle.FromRadians(safeStartAngle));
        return posUI;
    }

    public Vector BoardToUIPosition(Position position)
    {
        var basicPositions = kimble.Board.Positions.Select(pos => pos).Where(pos => pos is not Home && pos is not Safe).ToList();
        int index = basicPositions.IndexOf(position);
        Vector posUI = Vector.FromLengthAndAngle(BasicDistance, Angle.FromRadians(StartAngle - index * BasicAngleAdd));
        return posUI;
    }

    internal void MovePieceController(Player player, Position oldPosition, Position newPosition, GameObject piece, Action pieceMovedHandler)
    {
        pieces[player][piece] = newPosition;
        if (newPosition is Safe) piece.MoveTo(BoardToUIPosition(newPosition as Safe), 1000);
        else if (oldPosition is Home) piece.MoveTo(BoardToUIPosition(newPosition), 1000);
        else if (newPosition is Home) piece.MoveTo(BoardToUIPosition(newPosition as Home), 1000);
        else
        {
            Timer.SingleShot(0.05, () => MoveAlongArc(piece, kimble.DiceNow, pieceMovedHandler));
            return;
        }
        pieceMovedHandler.Invoke();
    }

    /// <summary>
    /// Moves an object one "stop" around the arc.
    /// </summary>
    public void MoveAlongArc(GameObject g, int steps, Action moveCompletedHandler)
    {
        Timer moveTimer = new Timer();
        moveTimer.Interval = 0.50;
        moveTimer.Timeout += () => Move(g);
        moveTimer.Start(steps);
        Timer.SingleShot(steps * moveTimer.Interval + 0.1, () => moveCompletedHandler?.Invoke());
        static void Move(GameObject g)
        {
            double distance = Math.PI / 14;
            Timer t = new Timer();
            t.Interval = 0.01;
            int speed = 4;
            int times = (int)(1 / t.Interval) / speed;
            t.Timeout += () =>
            {
                double angle = Math.Atan2(g.Y, g.X);
                double angleToAdd = distance * t.Interval * speed;
                angle -= angleToAdd;
                g.Position = Vector.FromLengthAndAngle(BasicDistance, Angle.FromRadians(angle));
            };
            t.Start(times);
        }
    }

    private Position GetCurrentPosition(Player player, GameObject gameObject)
    {
        //(Position position, GameObject piece) = pieces[player].Select(go_pos => go_pos).Where(go_pos => go_pos.Key == gameObject).First();
        //return position;
        return pieces[player][gameObject];
    }



    internal void CreateBoardLayout()
    {
        // Angle from the top going clockwise.
        Position now;

        // Basic positions
        for (int i = 0; i < TotalPositions; i++)
        {
            now = kimble.Board.Positions[i];
            if (now is Home || now is Safe) continue;
            Vector position = BoardToUIPosition(now);
            DrawPosition(-1, i, 0, position);
        }

        // Homes
        for (int i = 0; i < kimble.Players.Length; i++)
        {
            int homeStartsFrom = kimble.Players[i].HomeStartsFrom;
            Label playerLabel = new(kimble.Players[i].Color + "");
            Vector v = BoardToUIPosition(kimble.Board.Positions[homeStartsFrom + 3] as Home);
            v = Vector.FromLengthAndAngle(v.Magnitude * 1.2, v.Angle);
            playerLabel.Position = v;
            game.Add(playerLabel);

            for (int j = 0; j < 4; j++)
            {
                Player player = kimble.Players[i];
                Home homeNow = kimble.Board.Positions[homeStartsFrom + j] as Home;
                Vector position = BoardToUIPosition(homeNow);
                DrawPosition(i, homeStartsFrom, j, position);
                GameObject piece = new(20, 20, Shape.Circle)
                {
                    Color = player.Color.ToJypeliColor(),
                    Position = position
                };
                /* DEBUG: 
                game.Mouse.ListenOn(piece, MouseButton.Left, ButtonState.Pressed, () =>
                {
                    Position piecePos = GetCurrentPosition(p, piece);
                    game.MessageDisplay.Add("" + Array.IndexOf(kimble.Board.Positions, piecePos));
                    InputWindow iw = new InputWindow("Give target position");
                    game.Add(iw);
                    iw.Closed += delegate
                    {
                        int selected = int.Parse(iw.InputBox.Text);
                        Position newPos = kimble.Board.Positions[selected];
                        kimble.Move(piecePos, newPos, MovePiece);
                        // MovePiece(p, piecePos, newPos);
                    };
                }, null);
                */
                game.Add(piece);
                if (pieces.ContainsKey(player))
                {
                    pieces[player].Add(piece, homeNow);
                }
                else
                {
                    pieces.Add(player, new() { { piece, homeNow } });
                }
            }
        }

        // Safes
        for (int i = 0; i < Rules.colorsAndStartingPositions.Length; i++)
        {
            int safeStartsFrom = kimble.Players[i].SafeEnd - 3;
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
        game.Add(g, -1);
        game.Add(background, -2);
    }

    private void CreatePointer(double x, double y)
    {
        Vector v = BoardToUIPosition(kimble.Board.Positions[kimble.PlayerInTurn.HomeStartsFrom + 3] as Home);
        v = Vector.FromLengthAndAngle(v.Magnitude * 1.2, v.Angle);
        pointer = new GameObject(30, 75, Shape.Rectangle)
        {
            Angle = Angle.FromRadians(-Math.PI / 2),
            Position = v,
            Color = kimble.Players[0].Color.ToJypeliColor()
        };
        game.Add(pointer);
    }

    public void UpdatePointer(Action pointerUpdatedHandler)
    {
        Vector v = BoardToUIPosition(kimble.Board.Positions[kimble.PlayerInTurn.HomeStartsFrom + 3] as Home);
        v = Vector.FromLengthAndAngle(v.Magnitude * 1.2, v.Angle);
        pointer.Color = Jypeli.Color.White;
        pointer.MoveTo(v, 1000, () =>
        {
            pointer.Color = kimble.PlayerInTurn.Color.ToJypeliColor();
            pointerUpdatedHandler();
        });
    }

    public GameObject[] GetPiecesThatCanMove()
    {
        List<Position> positionsToMove = kimble.PiecesThatCanMove.Select(x => x.oldPosition).ToList();
        GameObject[] piecesToMove = pieces[kimble.PlayerInTurn].Select(go_pos => go_pos).Where(go_pos => positionsToMove.Any(p => p == go_pos.Value)).Select(x => x.Key).ToArray();
        return piecesToMove;
    }

    public EventHandler AnimateMovables(EventHandler timerStopHandler)
    {
        GameObject[] piecesToMove = GetPiecesThatCanMove();

        foreach (var piece in piecesToMove)
        {
            Timer t = new Timer();
            t.Interval = 0.5;
            t.Timeout += () =>
            {
                if (piece.Color == Jypeli.Color.Black) piece.Color = kimble.PlayerInTurn.Color.ToJypeliColor();
                else piece.Color = Jypeli.Color.Black;
            };
            t.Start();

            timerStopHandler += (o, e) =>
            {
                t.Stop();
                piece.Color = kimble.PlayerInTurn.Color.ToJypeliColor();
            };
        }
        return timerStopHandler;
    }

    public Position GetPositionOf(GameObject g)
    {
        //return pieces[kimble.PlayerInTurn].Select(x => x).Where(x => x.gameObject == g).First().position;
        return GetCurrentPosition(kimble.PlayerInTurn, g);
    }
}