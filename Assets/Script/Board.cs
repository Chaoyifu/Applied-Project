using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public delegate void TimeOutDelegate();
public delegate void LevelClearedDelegate();
public delegate void GamePausedDelegate();

public class Board : MonoBehaviour {

    public GameObject leftMark;
    public GameObject RightMark;
    public float zTilePosition = 0.0f;
    public float zPiecePosition = 0.0f;
    public float specialPiece;
    public List <GameObject> PieceNormal;
    public List<GameObject> PieceStrong;
    private static int[,] intGrid = new int[10, 10]; 
    public int maxPieces = 5;
    public int boardNum = 1;
    public int rows = 10;
    public int columns = 10;
    public bool fillOnx = false;
    public bool CentreOnx = false;
    internal static float step = 0.0f;
    internal static float halfStep = 0.0f;
    private static Vector3 startPosition = new Vector3(0f, 0f, 0f);
    internal static PlayingPiece[,] PlayingPieces = new PlayingPiece[10, 10];
    internal static int[,] gdesc = new int[10,10];
    private static List<GameObject> piecesToUseNormal = new List<GameObject>();
    private static List<GameObject> piecesToUseStrong = new List<GameObject>();
    private static Board instance = null;
    private bool started = false;

    public GameObject tile;
    private static Vector3 generalScale = new Vector3(1f, 1f, 1f);

    public static bool PlayerCanMove = true;
    public GameStyle gameStyle = GameStyle.Standard;

    private bool _MovingPieces;
    private Vector2 _CurrentPosition;
    private static bool destroyed = false;
    //public AudioClip DestroyPiece;
    //public Material tileDoneMaterial;
    //public AudioClip SlidePiece;

    public bool newPieceFromTop = true;
    public AudioClip newPiece;


    public GUIText BoardPoints;
    public int PointsNormal = 5;

    public GUIText TimeInfo;
    public float levelTime = 5.0f;
    private static float gameTimer = 0f;
    private static float RestTime;
    private static float timeToAdd = 5f;
    internal static bool TimeOut = false;

    public GUIText DifficultyInfo;

    public static float WinningScore = 300f;
    public GameObject GameOverText;

    public static Board Instance{
        get {
            return instance;
        }
    }

    void Start() {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        started = false;
        //FrameworkCore.setContent(GameInfo.vocabularyContent);
        DifficultyManagement.setDifficulty(Difficulty.Four);
        ScoresManager.AddPoints(-ScoresManager.CurrentPoints);
        RestTime = levelTime * 60f;
        Time.timeScale = 1;
        instance = this;
        StartBoard();
       
    }
    internal void StartBoard() {
        PlayingPieces = new PlayingPiece[columns, rows];
        intGrid = new int[columns, rows];

        gdesc = new int[columns, rows];
        float val = Mathf.Max((float)(columns), (float)(rows));
        if (!fillOnx){
            step = (Mathf.Abs(leftMark.transform.position.x) + Mathf.Abs(RightMark.transform.position.x)) / val;
            halfStep = step / 2.0f;
            startPosition = new Vector3(leftMark.transform.position.x + halfStep, leftMark.transform.position.y - halfStep, zTilePosition);
        }
        else {
            Vector3 left = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.z));
            Vector3 right = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Min((float)Screen.width, (float)Screen.height), 0f, Camera.main.transform.position.z));
            step = (Mathf.Abs(left.x) + Mathf.Abs(right.x)) / val;
            halfStep = step / 2.0f;
            startPosition = new Vector3(-(Mathf.Abs(left.x)) + halfStep, left.y - halfStep, zTilePosition);
        }

        float tileScale = (step * val) / (tile.GetComponent<Renderer>().bounds.size.x * val);
        if (!CentreOnx) {
            Vector3 realRight = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width, 0f, Camera.main.transform.position.z));
            startPosition.x += Mathf.Abs(Mathf.Abs(realRight.x * 2f) - (step * columns)) / 2f;
        }

        generalScale = new Vector3(tileScale, tileScale, 1f);
        GridPositions.Init();
        
        //TextAsset TxTFile = (TextAsset)Resources.Load
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {
                gdesc[x, y] = new int();
                if (DifficultyManagement.currentDifficulty < Difficulty.Four || x == 0 || x == columns - 1 || y == 0 || y == rows - 1)
                    gdesc[x, y] = 1;
                else {
                    if (DifficultyManagement.currentDifficulty >= Difficulty.Four && x > 0 && x < columns - 1 && y >0 && y < rows - 1){
                        int tmp = Random.Range(1, 6);
                        gdesc[x, y] = 1 + tmp / 5;
                    }
                }
            }
        }
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {
                float xp, yp = 0f;
                if (gdesc[x, y] != 0) {
                    xp = startPosition.x + (x * step);
                    yp = startPosition.y - (y * step);
                    GridPositions.SetPosition(x, y, xp, yp);
                    if (gdesc[x, y] != 99) {
                        bool again = false;
                        do
                        {
                            int t = Random.Range(0, maxPieces);
                            int title;
                            if(DifficultyManagement.currentDifficulty < Difficulty.Three)
                                title = Random.Range(0, (int)DifficultyManagement.currentDifficulty + 1);
                            else
                                title = Random.Range(0, 4);
                            switch (gdesc[x, y])
                            {
                                case 1:
                                    GetPiecesToUse(title);
                                    PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(xp, yp, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColor)title);
                                    break;
                                case 2:
                                    GetStrongPiecesToUse(title);
                                    PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseStrong[t], new Vector3(xp, yp, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColor)title);
                                    break;
                            }

                            if (CheckTileMatchX(x, y, true) || CheckTileMatchY(x, y, true))
                            {
                                DestroyImmediate(PlayingPieces[x, y].Piece);
                                PlayingPieces[x, y] = null;
                                again = true;
                            }
                            else
                                again = false;
                        } while (again);
                        PlayingPieces[x, y].pieceScript.currentStrenght = (TileType)gdesc[x, y];
                        //PlayingPieces[x, y].pieceScript.MoveTo(x, y, zPiecePosition);
                        //PlayingPieces[x, y].Piece.transform.localScale = generalScale;
                    }
                }
            }
        }
        started = true;
    }

    void OnGUI() {
        BoardPoints.text = "Score: " + ScoresManager.CurrentPoints.ToString();
        TimeInfo.text = "Time: " + RestTime.ToString();
        DifficultyInfo.text = "Difficulty: " + (int)DifficultyManagement.currentDifficulty;

    }

    internal PieceColor checkCol(string str)
    {
        string[,] data = FrameworkCore.currentContent.getData();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (str == data[i,j]) {
                    return (PieceColor)i;
                }
            }
        }
        return PieceColor.Error;
    }

    internal bool CheckTileMatchX(int x, int y, bool justCheck)
    {
        return CheckTileMatchX(x, y, x, y, justCheck);
    }

    internal bool CheckTileMatchX(int x1, int y1, int x2, int y2, bool justCheck)
    {
        bool match = false;
        if (x1 < 0 || x1 > columns - 1 || y1 < 0 || y1 > rows - 1 || x2 < 0 || x2 > columns - 1 || y2 < 0 || y2 > rows - 1)
            return false;
        FillIntGrid();
        if (intGrid[x1, y1] == -1 || intGrid[x2, y2] == -1)
            return false;
        int z = intGrid[x1, y1];
        intGrid[x1, y1] = intGrid[x2, y2];
        intGrid[x2, y2] = z;
        if (gdesc[x1, y1] != (int)TileType.NoTile && gdesc[x2, y2] != (int)TileType.NoTile) {
            if ((x2 + 2 < columns) &&
                (intGrid[x2 + 1, y2] != -1) &&
                (intGrid[x2 + 2, y2] != -1) &&
                (gdesc[x2 + 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 2, y2] != (int)TileType.NoTile) &&
                (intGrid[x2, y2] == intGrid[x2 + 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 + 2, y2])) {
                if (!justCheck && !PlayingPieces[x1, y1].Moving && !PlayingPieces[x2 + 1, y2].Moving && !PlayingPieces[x2 + 2, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 + 1, y2].Selected = true;
                    PlayingPieces[x2 + 2, y2].Selected = true;
                    match = true;
                }
                else
                    return true;
            }
            if ((x2 + 1 < columns) && (x2 - 1 >= 0) &&
                (intGrid[x2 + 1, y2] != -1) &&
                (intGrid[x2 - 1, y2] != -1) &&
                (gdesc[x2 + 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.NoTile) &&
                (intGrid[x2, y2] == intGrid[x2 + 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 1, y2]))
            {
                if (!justCheck && !PlayingPieces[x1, y1].Moving && !PlayingPieces[x2 + 1, y2].Moving && !PlayingPieces[x2 - 1, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 + 1, y2].Selected = true;
                    PlayingPieces[x2 - 1, y2].Selected = true;
                    match = true;
                }
                else
                    return true;
            }
            if ((x2 - 2 >= 0) &&
                (intGrid[x2 - 2, y2] != -1) &&
                (intGrid[x2 - 1, y2] != -1) &&
                (gdesc[x2 - 2, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.NoTile) &&
                (intGrid[x2, y2] == intGrid[x2 - 2, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 1, y2]))
            {
                if (!justCheck && !PlayingPieces[x1, y1].Moving && !PlayingPieces[x2 - 2, y2].Moving && !PlayingPieces[x2 - 1, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 - 2, y2].Selected = true;
                    PlayingPieces[x2 - 1, y2].Selected = true;
                    match = true;
                }
                else
                    return true;
            }

        }
        return match;
    }

    internal bool CheckTileMatchY(int x, int y, bool justCheck)
    {
        return CheckTileMatchY(x, y, x, y, justCheck);
    }

    internal bool CheckTileMatchY(int x1, int y1, int x2, int y2, bool justCheck)
    {
        bool match = false;
        if (x1 < 0 || x1 > columns - 1 || y1 < 0 || y1 > rows - 1 || x2 < 0 || x2 > columns - 1 || y2 < 0 || y2 > rows - 1)
            return false;
        FillIntGrid();
        if (intGrid[x1, y1] == -1 || intGrid[x2, y2] == -1)
            return false;
        int z = intGrid[x1, y1];
        intGrid[x1, y1] = intGrid[x2, y2];
        intGrid[x2, y2] = z;
        if (gdesc[x1, y1] != (int)TileType.NoTile && gdesc[x2, y2] != (int)TileType.NoTile)
        {
            if ((y2 + 2 < rows) &&
                (intGrid[x2, y2 + 1] != -1) &&
                (intGrid[x2, y2 + 2] != -1) &&
                (gdesc[x2, y2 + 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 2] != (int)TileType.NoTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 2]))
            {
                if (!justCheck && !PlayingPieces[x1, y1].Moving && !PlayingPieces[x2, y2 + 1].Moving && !PlayingPieces[x2, y2 + 2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 + 1].Selected = true;
                    PlayingPieces[x2, y2 + 2].Selected = true;
                    match = true;
                }
                else
                    return true;
            }
            if ((y2 + 1 < rows) && (y2 - 1 >= 0) &&
                (intGrid[x2, y2 + 1] != -1) &&
                (intGrid[x2, y2 - 1] != -1) &&
                (gdesc[x2, y2 + 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.NoTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 1]))
            {
                if (!justCheck && !PlayingPieces[x1, y1].Moving && !PlayingPieces[x2, y2 + 1].Moving && !PlayingPieces[x2, y2 - 1].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 + 1].Selected = true;
                    PlayingPieces[x2, y2 - 1].Selected = true;
                    match = true;
                }
                else
                    return true;
            }
            if ((y2 - 2 >= 0) &&
                (intGrid[x2, y2 - 2] != -1) &&
                (intGrid[x2, y2 - 1] != -1) &&
                (gdesc[x2, y2 - 2] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.NoTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 2]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 1]))
            {
                if (!justCheck && !PlayingPieces[x1, y1].Moving && !PlayingPieces[x2, y2 - 2].Moving && !PlayingPieces[x2, y2 - 1].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 - 2].Selected = true;
                    PlayingPieces[x2, y2 - 1].Selected = true;
                    match = true;
                }
                else
                    return true;
            }

        }
        return match;
    }

    void FillIntGrid() {
        string[,] data = FrameworkCore.currentContent.getData();
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {
                intGrid[x, y] = new int();
                if (PlayingPieces[x, y] != null){
                    intGrid[x, y] = (int)PlayingPieces[x, y].Type;
                    //PlayingPieces[x, y].Piece.GetComponent<TextMesh>().text = intGrid[x, y].ToString();
                }
                else
                    intGrid[x, y] = -1;
            }
        }
    }

    private void GetPiecesToUse(int title) {
        piecesToUseNormal.Clear();
        string[,] data = FrameworkCore.currentContent.getData();
        /*for (int i = 0; i < maxPieces; i++) {
            bool redo = true;
            int t = 0;
            do
            {
                t = Random.Range(0, PieceNormal.Count);
                GameObject tmp = new GameObject();
                tmp = PieceNormal[t];
                tmp.GetComponent<TextMesh>().text = data[Random.RandomRange(0,5),Random.Range(0,2)];
                if (!piecesToUseNormal.Contains(tmp))
                {
                    piecesToUseNormal.Add(tmp);
                    redo = false;
                }
            } while (redo);*/
        int t = 0;
        GameObject tmp = PieceNormal[t];
        tmp.GetComponent<TextMesh>().text = data[title,Random.Range(0,2)];
        piecesToUseNormal.Add(tmp);
    }

    private void GetStrongPiecesToUse(int title)
    {
        piecesToUseStrong.Clear();
        string[,] data = FrameworkCore.currentContent.getData();
        /*for (int i = 0; i < maxPieces; i++) {
            bool redo = true;
            int t = 0;
            do
            {
                t = Random.Range(0, PieceNormal.Count);
                GameObject tmp = new GameObject();
                tmp = PieceNormal[t];
                tmp.GetComponent<TextMesh>().text = data[Random.RandomRange(0,5),Random.Range(0,2)];
                if (!piecesToUseNormal.Contains(tmp))
                {
                    piecesToUseNormal.Add(tmp);
                    redo = false;
                }
            } while (redo);*/
        int t = 0;
        GameObject tmp = PieceStrong[t];
        tmp.GetComponent<TextMesh>().text = data[title, Random.Range(0, 2)];
        piecesToUseStrong.Add(tmp);
    }

    private void SlideDown(int x, int y) {
        for (int y1 = y; y1 >= 1; y1--) {
            if (gdesc[x, y1] !=(int)TileType.NoTile && PlayingPieces[x, y1] == null)
            {
                int fy = -1;
                for (int z = y1 - 1; z >= 0; z--)
                {
                    if (gdesc[x, z] != (int)TileType.NoTile && PlayingPieces[x, z] != null)
                    {
                        fy = z;
                        break;
                    }
                }
                if (fy != -1)
                {
                    if (PlayingPieces[x, fy].pieceScript.currentStrenght == TileType.Normal)
                    {
                        PlayingPieces[x, fy].Selected = false;
                        PlayingPieces[x, fy].pieceScript.MoveTo(x, y1);
                        PlayingPieces[x, y1] = PlayingPieces[x, fy];
                        //PlayingPieces[x, y1].Type = PlayingPieces[x, fy].Type;
                        PlayingPieces[x, fy] = null;
                        //GetComponent<AudioSource>().PlayOneShot(SlidePiece);
                        SlideDown(x, y1);
                        break;
                    }
                }
            }
       }
    }

    private void NewPieces() {
        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                if (gdesc[x, y] != (int)TileType.NoTile && PlayingPieces[x, y] == null)
                {
                    int chance = Random.Range(0, 7);
                    Vector2 v0 = GridPositions.GetVector(x, y);
                    bool again = false;
                    do
                    {
                        int t = Random.Range(0, maxPieces);
                        int title;
                        if (DifficultyManagement.currentDifficulty < Difficulty.Three)
                            title = Random.Range(0, (int)DifficultyManagement.currentDifficulty + 1);
                        else
                            title = Random.Range(0, 4);
                        if (!newPieceFromTop){
                            if (DifficultyManagement.currentDifficulty < Difficulty.Five){
                                GetPiecesToUse(title);
                                PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(v0.x, v0.y, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColor)title);
                                PlayingPieces[x, y].pieceScript.currentStrenght = TileType.Normal;
                            }
                            else {
                                if (DifficultyManagement.currentDifficulty >= Difficulty.Five)
                                {
                                    int tmp = Random.Range(1, 9);
                                    if (tmp < 8)
                                    {
                                        GetPiecesToUse(title);
                                        PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(v0.x, v0.y, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColor)title);
                                        PlayingPieces[x, y].pieceScript.currentStrenght = TileType.Normal;
                                    }
                                    else {
                                        GetStrongPiecesToUse(title);
                                        PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseStrong[t], new Vector3(v0.x, v0.y, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColor)title);
                                        PlayingPieces[x, y].pieceScript.currentStrenght = TileType.Strong;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (DifficultyManagement.currentDifficulty < Difficulty.Five)
                            {
                                GetPiecesToUse(title);
                                PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(v0.x, v0.y + Random.Range(20f, 30f), zPiecePosition - 25f), Quaternion.identity) as GameObject, (PieceColor)title);
                                PlayingPieces[x, y].pieceScript.currentStrenght = TileType.Normal;
                            }
                            else {
                                if (DifficultyManagement.currentDifficulty >= Difficulty.Five)
                                {
                                    int tmp = Random.Range(1, 9);
                                    if (tmp < 8)
                                    {
                                        GetPiecesToUse(title);
                                        PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(v0.x, v0.y + Random.Range(20f, 30f), zPiecePosition - 25f), Quaternion.identity) as GameObject, (PieceColor)title);
                                        PlayingPieces[x, y].pieceScript.currentStrenght = TileType.Normal;
                                    }
                                    else {
                                        GetStrongPiecesToUse(title);
                                        PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseStrong[t], new Vector3(v0.x, v0.y + Random.Range(20f, 30f), zPiecePosition - 25f), Quaternion.identity) as GameObject, (PieceColor)title);
                                        PlayingPieces[x, y].pieceScript.currentStrenght = TileType.Strong;
                                    }
                                }
                            }

                        }
                        if (CheckTileMatchX(x, y, true) || CheckTileMatchY(x, y, true))
                        {
                            DestroyImmediate(PlayingPieces[x, y].Piece);
                            PlayingPieces[x, y] = null;
                            again = true;
                        }
                        else
                            again = false;
                    } while (again);
                    //audio.PlayOneShot(newPiece);
                    PlayingPieces[x, y].pieceScript.MoveTo(x, y, zPiecePosition - 25f);
                    PlayingPieces[x, y].Selected = false;
                }
                else if (gdesc[x, y] != (int)TileType.NoTile && PlayingPieces[x, y] != null && PlayingPieces[x, y].pieceScript.currentStrenght != TileType.Normal) {
                    break;
                }
            }
        }
    }

    void Update() {
        //checkMovesTimer += Time.deltaTime;
        if (ScoresManager.CurrentPoints >= WinningScore && started)
        {
            Time.timeScale = 0;
            GameOverText.GetComponent<Text>().text = "You Win!!";
            GameOverText.SetActive(true);
            started = false;
        }
        if (TimeOut && started) {
            Time.timeScale = 0;
            GameOverText.GetComponent<Text>().text = "Game Over";
            GameOverText.SetActive(true);
            started = false;
        }
        gameTimer += Time.deltaTime;
        RestTime -= Time.deltaTime;
        if (RestTime <= 0)
            TimeOut = true;
        

        if (started) {
            _MovingPieces = false;
            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < columns; x++) {
                    if (PlayingPieces[x, y] != null && PlayingPieces[x, y].Moving) {
                        _MovingPieces = true;
                        y = rows;
                        break;
                    }
                }
            }
            if (destroyed && !_MovingPieces) {
                PlayerCanMove = false;
                destroyed = false;
                for (int x = 0; x < columns; x++) {
                    SlideDown(x, rows - 1);
                }
                NewPieces();
                PlayerCanMove = true;
            }
            if (!_MovingPieces && !destroyed) {
                PlayerCanMove = false;
                for (int y = 0; y < rows; y++) {
                    for (int x = 0; x < columns; x++) {
                        CheckTileMatchX(x, y, false);
                        CheckTileMatchY(x, y, false);
                    }
                }
                //deal with coins
                int specialCount = 0;
                for (int y = 0; y < rows; y++) {
                    for (int x = 0; x < columns; x++) {
                        if (PlayingPieces[x, y] != null && PlayingPieces[x, y].Selected && PlayingPieces[x, y].Piece != null) {
                            //deal with coins
                            //todo
                            _CurrentPosition = GridPositions.GetVector(x, y);
                            switch (PlayingPieces[x, y].pieceScript.currentStrenght) {
                                case TileType.Normal:
                                    Destroy(PlayingPieces[x, y].Piece);
                                    PlayingPieces[x, y].Piece = null;
                                    PlayingPieces[x, y].Selected = false;
                                    PlayingPieces[x, y] = null;
                                    gdesc[x, y] = (int)TileType.Done;
                                    ScoresManager.AddPoints(PointsNormal);
                                    RestTime += timeToAdd;
                                    break;
                                case TileType.Strong:
                                    string name = PlayingPieces[x, y].Piece.GetComponent<TextMesh>().text.ToString();
                                    Destroy(PlayingPieces[x, y].Piece);
                                    GameObject tmp = PieceNormal[0];
                                    tmp.GetComponent<TextMesh>().text = name;
                                    PlayingPieces[x, y].Piece = Instantiate(tmp, new Vector3(_CurrentPosition.x, _CurrentPosition.y, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity )as GameObject;
                                    PlayingPieces[x, y].pieceScript.currentStrenght = TileType.Normal;
                                    PlayingPieces[x, y].Selected = false;
                                    gdesc[x, y] = (int)TileType.Normal;
                                    ScoresManager.AddPoints(PointsNormal);
                                    RestTime += timeToAdd;
                                    break;
                            }
                            //audio.PlayOneShot(destroyPiece);
                            destroyed = true;
                        }
                    }
                }
                PlayerCanMove = true;
            }
        } 
    }
}
