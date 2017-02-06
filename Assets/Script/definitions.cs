using UnityEngine;
using System.Collections;

public enum PieceColor {
    First = 0,
    Second,
    Third,
    Forth,
    Error
}

public enum TileType
{
    Normal = 1,
    NoTile,
    Done,
}
public enum GameStyle{
    Standard = 0,
    Marinas,
}

public struct GridPoint {
    public int x;
    public int y;
    public static bool operator ==(GridPoint a, GridPoint b) {
        return (a.x == b.x) && (a.y == b.y);
    }
    public static bool operator !=(GridPoint a, GridPoint b)
    {
        return (a.x != b.x) || (a.y != b.y);
    }
    public override bool Equals(object obj)
    {
        GridPoint tmp = (GridPoint)obj;
        if (this == tmp)
            return true;
        else
            return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public static int operator -(GridPoint a, GridPoint b) {
        if (a.x == b.x)
            return a.y - b.y;
        else if (a.y == b.y)
            return a.x - b.x;
        else
            return 100;
    }
}

internal class PlayingPiece {
    private bool selected = false;
    private PieceScript ps = null;

    internal PieceColor Type = PieceColor.First;
    //internal bool SpecialPiece = false;
    private GameObject piece = null;
    internal GameObject Piece {
        get {
            return piece;
        }
        set {
            piece = value;
            if (piece != null)
                ps = piece.GetComponent<PieceScript>();
        }
    }

    internal bool Moving {
        get {
            if (ps != null)
                return ps.Moving;
            else
                return false;
        }
    }

    internal PlayingPiece(GameObject obj, PieceColor planet) {
        Piece = obj;
        Type = planet;
    }

    public PieceScript pieceScript {
        get {
            return ps;
        }
    }

    internal bool Selected {
        get {
            return selected;
        }
        set {
            if (piece != null) {
                if (ps.Moving)
                    selected = false;
                else
                    selected = value;
            }
        }
    }
}
public class definitions : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
