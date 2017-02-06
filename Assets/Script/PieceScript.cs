using UnityEngine;
using System.Collections;

public class PieceScript : MonoBehaviour {

    private bool moving = false;
    //public AudioClip moveSound;
    private GridPoint _Gp;
    internal TileType currentStrenght = TileType.Normal;
    Vector3 prevPoint = Vector3.zero;
    Vector3 currentPoint = Vector3.zero;
    private const float FLOAT_DragDetection = 10f;
    private Transform myTransform;
    Vector3 velocity = Vector3.zero;
    Vector3 destination = Vector3.zero;

    internal bool Moving {
        get {
            return moving;
        }
    }

    // Use this for initialization
    void Start () {
        myTransform = transform;
	}

    public void MoveTo(int x, int y) {
        moving = true;
        destination = new Vector3(GridPositions.GetVector(x, y).x, GridPositions.GetVector(x, y).y, myTransform.position.z);
    }

    public void MoveTo(int x, int y, float z)
    {
        moving = true;
        destination = new Vector3(GridPositions.GetVector(x, y).x, GridPositions.GetVector(x, y).y, z);
    }

    float dragDelay = 0f;

    void OnMouseDrag() {
        if ((!Board.PlayerCanMove) || (Board.Instance.gameStyle != GameStyle.Standard) ||
            currentStrenght != TileType.Normal)
            return;
        dragDelay += Time.deltaTime;
        if (dragDelay < 0.2f)
            return;
        dragDelay = 0f;
        prevPoint = currentPoint;
        currentPoint = Input.mousePosition;
        _Gp = GridPositions.GetGridPosition(new Vector2(myTransform.position.x, myTransform.position.y));

        Vector3 dir = currentPoint - prevPoint;

        if (dir.x < -FLOAT_DragDetection) {
            if ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchX(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)))
            {
                MoveTo(_Gp.x - 1, _Gp.y);
                Board.PlayingPieces[_Gp.x - 1, _Gp.y].pieceScript.MoveTo(_Gp.x, _Gp.y);

                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x - 1, _Gp.y];
                Board.PlayingPieces[_Gp.x - 1, _Gp.y] = tmp;

                Board.PlayerCanMove = false;
                //GetComponent<AudioSource>().PlayOneShot(moveSound);

                dragDelay = 0f;
                return;
            }
        }
        if (dir.x > FLOAT_DragDetection)
        {
            if ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchX(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)))
            {
                MoveTo(_Gp.x + 1, _Gp.y);
                Board.PlayingPieces[_Gp.x + 1, _Gp.y].pieceScript.MoveTo(_Gp.x, _Gp.y);

                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x + 1, _Gp.y];
                Board.PlayingPieces[_Gp.x + 1, _Gp.y] = tmp;

                Board.PlayerCanMove = false;
                //GetComponent<AudioSource>().PlayOneShot(moveSound);

                dragDelay = 0f;
                return;
            }
        }

        if (dir.y > FLOAT_DragDetection)
        {
            if ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                (Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)))
            {
                MoveTo(_Gp.x, _Gp.y - 1);
                Board.PlayingPieces[_Gp.x, _Gp.y - 1].pieceScript.MoveTo(_Gp.x, _Gp.y);

                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x, _Gp.y - 1];
                Board.PlayingPieces[_Gp.x, _Gp.y - 1] = tmp;

                Board.PlayerCanMove = false;
                //GetComponent<AudioSource>().PlayOneShot(moveSound);

                dragDelay = 0f;
                return;
            }
        }

        if (dir.y < -FLOAT_DragDetection)
        {
            if ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                (Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)))
            {
                MoveTo(_Gp.x, _Gp.y + 1);
                Board.PlayingPieces[_Gp.x, _Gp.y + 1].pieceScript.MoveTo(_Gp.x, _Gp.y);

                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x, _Gp.y + 1];
                Board.PlayingPieces[_Gp.x, _Gp.y + 1] = tmp;

                Board.PlayerCanMove = false;
                //GetComponent<AudioSource>().PlayOneShot(moveSound);

                dragDelay = 0f;
                return;
            }
        }
    }

    void OnMouseDown() {
        currentPoint = Input.mousePosition;
        dragDelay = 0;
    }

    // Update is called once per frame
    void Update () {
        if (moving) {
            velocity = destination - myTransform.position;
            float damping = velocity.magnitude;
            if (velocity.sqrMagnitude < 0.5f)
            {
                moving = false;
                myTransform.position = destination;
            }
            else {
                velocity.Normalize();
                myTransform.position = (myTransform.position + (velocity * Time.deltaTime * (damping / 0.2f)));
            }
        }
	}
}
