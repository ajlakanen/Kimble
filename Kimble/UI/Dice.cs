using System;
using System.Collections.Generic;
using System.Linq;
using Jypeli;

namespace Kimble;

enum EyeLocation
{
    NE, E, SE, SW, W, NW, Center
}

/// <summary>
/// Dice
/// </summary>
internal class Dice : GameObject
{
    readonly Dictionary<int, EyeLocation[]> _numbers;
    readonly Dictionary<EyeLocation, GameObject> _eyes;
    private readonly double _dotsize;

    public Dice(double width, double height) : base(width, height)
    {
        _dotsize = width / 5;
        _numbers = new Dictionary<int, EyeLocation[]>()
        {
            { 1, new EyeLocation[] {EyeLocation.Center} },
            { 2, new EyeLocation[] {EyeLocation.NW, EyeLocation.SE} },
            { 3, new EyeLocation[] {EyeLocation.NW, EyeLocation.Center, EyeLocation.SE} },
            { 4, new EyeLocation[] {EyeLocation.NW, EyeLocation.NE, EyeLocation.SW, EyeLocation.SE} },
            { 5, new EyeLocation[] {EyeLocation.NW, EyeLocation.NE, EyeLocation.Center, EyeLocation.SW, EyeLocation.SE} },
            { 6, new EyeLocation[] {EyeLocation.NW, EyeLocation.NE, EyeLocation.SW, EyeLocation.SE, EyeLocation.W, EyeLocation.E } },
        };

        // GameObject _baseObject = new(width, height);

        _eyes = new Dictionary<EyeLocation, GameObject>()
        {
            {EyeLocation.NW, NW()},
            {EyeLocation.NE, NE()},
            {EyeLocation.SW, SW()},
            {EyeLocation.SE, SE()},
            {EyeLocation.W, W()},
            {EyeLocation.E, E()},
            {EyeLocation.Center, Center()}
        };

        foreach (var eye in _eyes.Values)
        {
            eye.Color = Jypeli.Color.Black;
            this.Add(eye);
        }
    }

    public void Hide()
    {
        foreach (var eye in _eyes.Values)
            eye.IsVisible = false;
    }

    /// <summary>
    /// Dice throwing animation is complete.
    /// </summary>
    public Action DiceAnimationComplete;

    /// <summary>
    /// Throw dice. 
    /// </summary>
    /// <returns>Number.</returns>
    public void Roll(int value)
    {
        // This is the actual returned number.
        //int value = new Random().Next(1, 7);

        // We need this construct for the animation
        // because the lambda (below) would capture the
        // i value of the loop, thus, we can not use 
        // i inside lambda. 
        const int AnimSteps = 5;
        int[] animNumbers;
        int animStepNow = 0;
        this.Color = Jypeli.Color.LightGray;
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

        // This shows the actual value of the dice.
        Timer.SingleShot(0.1 * (AnimSteps + 1), () =>
        {
            Hide();
            Show(value);
            this.Color = Jypeli.Color.White;
            DiceAnimationComplete?.Invoke();
        });

        //return value;
        int Next() { return animNumbers[animStepNow++]; }
    }

    public void Show(int value)
    {
        foreach (var go in _numbers[value])
            _eyes[go].IsVisible = true;
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