using System;
using System.Collections.Generic;
using System.Linq;
using Jypeli;

namespace Kimble;

enum DicePart
{
    NE, E, SE, SW, W, NW, Center
}

internal class Dice : GameObject
{
    readonly Dictionary<int, DicePart[]> _figures;
    readonly Dictionary<DicePart, GameObject> _gameObjects;
    private readonly double _dotsize;

    public Dice(double width, double height) : base(width, height)
    {
        _dotsize = width / 5;
        _figures = new Dictionary<int, DicePart[]>()
        {
            { 1, new DicePart[] {DicePart.Center} },
            { 2, new DicePart[] {DicePart.NW, DicePart.SE} },
            { 3, new DicePart[] {DicePart.NW, DicePart.Center, DicePart.SE} },
            { 4, new DicePart[] {DicePart.NW, DicePart.NE, DicePart.SW, DicePart.SE} },
            { 5, new DicePart[] {DicePart.NW, DicePart.NE, DicePart.Center, DicePart.SW, DicePart.SE} },
            { 6, new DicePart[] {DicePart.NW, DicePart.NE, DicePart.SW, DicePart.SE, DicePart.W, DicePart.E } },
        };

        GameObject _baseObject = new(width, height);

        _gameObjects = new Dictionary<DicePart, GameObject>()
        {
            {DicePart.NW, NW()},
            {DicePart.NE, NE()},
            {DicePart.SW, SW()},
            {DicePart.SE, SE()},
            {DicePart.W, W()},
            {DicePart.E, E()},
            {DicePart.Center, Center()}
        };

        foreach (var item in _gameObjects.Values)
        {
            item.Color = Jypeli.Color.Black;
            this.Add(item);
        }
    }

    public void Hide()
    {
        foreach (var go in _gameObjects.Values)
        {
            go.IsVisible = false;
        }
    }

    /// <summary>
    /// Dice throwing animation is complete.
    /// </summary>
    public Action DiceAnimationComplete;


    public int Throw()
    {
        int animStepNow = 0;
        int[] animNumbers;

        int Next() { return animNumbers[animStepNow++]; }

        int value = new Random().Next(1, 7);
        const int AnimSteps = 5;
        animNumbers = new int[AnimSteps];
        for (int j = 0; j < animNumbers.Length; j++)
        {
            int n;
            do
            {
                n = new Random().Next(1, 7);
                animNumbers[j] = n;
            } while (!animNumbers.Take(j).All(x => x != n) || n == value);

        }

        for (int i = 1; i <= AnimSteps; i++)
        {
            Timer.SingleShot(0.1 * i, () =>
            {
                Hide();
                Show(Next());
            });
        }

        Timer.SingleShot(0.1 * (AnimSteps + 1), () =>
        {
            Hide();
            Show(value);
            DiceAnimationComplete?.Invoke();
        });
        return value;
    }

    public void Show(int value)
    {
        foreach (var go in _figures[value])
        {
            _gameObjects[go].IsVisible = true;
        }
    }

    private GameObject NE() => new(_dotsize, _dotsize, Shape.Circle)
    {
        Position = new Vector(Position.X + Width / 4, Position.Y + Width / 4)
    };

    private GameObject NW() => new(_dotsize, _dotsize, Shape.Circle)
    {
        Position = new Vector(Position.X - Width / 4, Position.Y + Width / 4)
    };

    private GameObject E() => new(_dotsize, _dotsize, Shape.Circle)
    {
        Position = new Vector(Position.X + Width / 4, Position.Y)
    };
    private GameObject W() => new(_dotsize, _dotsize, Shape.Circle)
    {
        Position = new Vector(Position.X - Width / 4, Position.Y)
    };

    private GameObject SE() => new(_dotsize, _dotsize, Shape.Circle)
    {
        Position = new Vector(Position.X + Width / 4, Position.Y - Width / 4)
    };
    private GameObject SW() => new(_dotsize, _dotsize, Shape.Circle)
    {
        Position = new Vector(Position.X - Width / 4, Position.Y - Width / 4)
    };
    private GameObject Center() => new(_dotsize, _dotsize, Shape.Circle)
    {
        Position = new Vector(Position.X, Position.Y)
    };
}