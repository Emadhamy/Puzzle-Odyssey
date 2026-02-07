using UnityEngine;

[CreateAssetMenu(fileName = "NewShape", menuName = "Puzzle Odyssey/Shape Data")]
public class ShapeData : ScriptableObject
{
    [System.Serializable]
    public struct ShapeTile
    {
        public int x;
        public int y;
    }
    
    public string shapeName;
    public Color shapeColor = Color.white;
    public ShapeTile[] tiles;
    public Vector2Int[] GetTilePositions()
    {
        Vector2Int[] positions = new Vector2Int[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            positions[i] = new Vector2Int(tiles[i].x, tiles[i].y);
        }
        return positions;
    }
    
    public static ShapeData CreateShape_Square()
    {
        ShapeData shape = ScriptableObject.CreateInstance<ShapeData>();
        shape.shapeName = "Square";
        shape.shapeColor = Color.blue;
        shape.tiles = new ShapeTile[]
        {
            new ShapeTile { x = 0, y = 0 },
            new ShapeTile { x = 1, y = 0 },
            new ShapeTile { x = 0, y = 1 },
            new ShapeTile { x = 1, y = 1 }
        };
        return shape;
    }
    
    public static ShapeData CreateShape_Line()
    {
        ShapeData shape = ScriptableObject.CreateInstance<ShapeData>();
        shape.shapeName = "Line";
        shape.shapeColor = Color.green;
        shape.tiles = new ShapeTile[]
        {
            new ShapeTile { x = 0, y = 0 },
            new ShapeTile { x = 1, y = 0 },
            new ShapeTile { x = 2, y = 0 },
            new ShapeTile { x = 3, y = 0 }
        };
        return shape;
    }
    
    public static ShapeData CreateShape_L()
    {
        ShapeData shape = ScriptableObject.CreateInstance<ShapeData>();
        shape.shapeName = "L Shape";
        shape.shapeColor = Color.red;
        shape.tiles = new ShapeTile[]
        {
            new ShapeTile { x = 0, y = 0 },
            new ShapeTile { x = 0, y = 1 },
            new ShapeTile { x = 0, y = 2 },
            new ShapeTile { x = 1, y = 2 }
        };
        return shape;
    }
    
    public static ShapeData CreateShape_T()
    {
        ShapeData shape = ScriptableObject.CreateInstance<ShapeData>();
        shape.shapeName = "T Shape";
        shape.shapeColor = Color.yellow;
        shape.tiles = new ShapeTile[]
        {
            new ShapeTile { x = 0, y = 0 },
            new ShapeTile { x = 1, y = 0 },
            new ShapeTile { x = 2, y = 0 },
            new ShapeTile { x = 1, y = 1 }
        };
        return shape;
    }
    
    public static ShapeData CreateShape_Single()
    {
        ShapeData shape = ScriptableObject.CreateInstance<ShapeData>();
        shape.shapeName = "Single";
        shape.shapeColor = Color.magenta;
        shape.tiles = new ShapeTile[]
        {
            new ShapeTile { x = 0, y = 0 }
        };
        return shape;
    }
}