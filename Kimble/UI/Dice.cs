using System.Collections.Generic;
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

    public Dice(Game game, double width, double height) : base(width, height)
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
        game.Add(_baseObject);

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
            game.Add(item);
        }
    }

    public void Hide()
    {
        foreach (var go in _gameObjects.Values)
        {
            go.IsVisible = false;
        }
    }

    public void Show(int value)
    {
        Hide();
        foreach(var go in _figures[value])
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