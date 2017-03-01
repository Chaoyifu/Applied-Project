using UnityEngine;
using System.Collections;

public class ScoresManager : ScriptableObject {

    public static GUIText ScoreOnScreen;
    private static ScoresManager instance = null;
    private static string workingFolder = "";
    private static string fullFileName = "";
    private static int currentScore = 0;
    private static ScoreTable scores = null;
    private static gameState userState = null;

    public static ScoresManager Instance {
        get {
            return instance;
        }
    }

    void OnEnable() {
        InitMe();
    }

    private void InitMe() {
        instance = this;
        workingFolder = Application.persistentDataPath;
        //fullFileName = Path.Combine(workingFolder, "savegame.dat");
        //if (File.Exists(fullFileName))
        //{
        //    LoadData();
        //}
        //else {
            scores = new ScoreTable();
        //}
    }

    public static void AddPoints(int pointsToAdd) {
        currentScore += pointsToAdd;
        if (userState != null) {
            userState.TotalScore += pointsToAdd;
        }
    }

    public static int CurrentPoints {
        get {
            return currentScore;
        }
    }
}