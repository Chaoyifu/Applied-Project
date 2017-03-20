using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the in game survey system.
/// The main menu game object should not be enabled when using this!
/// </summary>
public class Survey : MonoBehaviour
{
    public bool playAFirst;

    public Canvas[] screens;

    // All the UI elements I need.

    // Background Info
    public InputField ageInput;
    public Dropdown genderInput;
    public Dropdown schoolInput;
    public InputField gradeInput;
    public Dropdown gamerInput;
    public InputField gameTimeInput;

    // Cognitive Load 1
    public Dropdown cogLoadOne;

    // User Engagement 1
    public Dropdown[] engagement11;
    public Dropdown[] engagement12;

    // Cognitive Load 2
    public Dropdown cogLoadTwo;

    // User Engagement 2
    public Dropdown[] engagement21;
    public Dropdown[] engagement22;

    // Comment Field
    public InputField comments;

    // Statics to track progress
    private static int activeScreen = 1;
    private static bool playedAFirst;
    private static int numVisits = 0;

    // String to be sent to Sheets
    private static ArrayList sheets = new ArrayList();

    // Use this for initialization
    void Start()
    {
        if (numVisits == 1)
        {
            activeScreen = 3;
            if (playedAFirst)
            {
                FrameworkCore.setContent(GameInfo.vocabularyContent);
            }
            else
            {
                FrameworkCore.setContent(GameInfo.mathContent);
            }
        }
        else if (numVisits == 2)
        {
            activeScreen = 7;
        }
        else
        {
            sendResults(GameInfo.gameTitle);
            if(playAFirst)
            {
                hitContentA();
            }
            else
            {
                hitContentB();
            }
        }

        changeScreens();
    }

    // Helper for swapping to active survey screen.
    private void changeScreens()
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if (i == activeScreen)
            {
                screens[i].gameObject.SetActive(true);
            }
            else
            {
                screens[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// This is the function that we need to have post data to the Google Sheet.
    /// It can call a helper or whatever, but here's the one focus point for all that.
    /// </summary>
    /// <param name="str">The string to be sent to the sheet.</param>
    private void sendResults(string str)
    {
        sheets.Add(str);
    }

    // Button functions.

    public void hitContentA()
    {
        sendResults("A");
        playedAFirst = true;
        FrameworkCore.setContent(GameInfo.mathContent);
        activeScreen = 1;
        changeScreens();
    }

    public void hitContentB()
    {
        sendResults("B");
        playedAFirst = false;
        FrameworkCore.setContent(GameInfo.vocabularyContent);
        activeScreen = 1;
        changeScreens();
    }

    public void hitContinueBackgroundScreen()
    {
        sendResults(ageInput.text);
        string temp = "";
        switch (genderInput.value) // Not 100% I want to do it this way vs keeping it as ints.
        {
            case 0:
                temp = "None";
                break;
            case 1:
                temp = "Male";
                break;
            default:
                temp = "Female";
                break;
        }
        sendResults(temp);
        switch (schoolInput.value)
        {
            case 0:
                temp = "None";
                break;
            case 1:
                temp = "Middle School";
                break;
            case 2:
                temp = "High School";
                break;
            case 3:
                temp = "College";
                break;
            default:
                temp = "Not a Student";
                break;
        }
        sendResults(temp);
        sendResults(gradeInput.text);
        switch (gamerInput.value)
        {
            case 0:
                temp = "None";
                break;
            case 1:
                temp = "A Gamer";
                break;
            default:
                temp = "Not a Gamer";
                break;
        }
        sendResults(temp);
        sendResults(gameTimeInput.text);
        activeScreen = 2;
        changeScreens();
    }

    public void hitFirstPlay()
    {
        numVisits = 1;
        SceneManager.LoadScene(1);
    }

    public void hitContinueFirstCognitiveScreen()
    {
        sendResults((cogLoadOne.value + 1).ToString());
        activeScreen = 4;
        changeScreens();
    }

    public void hitContinueFirstEngagementPart1Screen()
    {
        for (int i = 0; i < engagement11.Length; i++)
        {
            sendResults((engagement11[i].value + 1).ToString());
        }
        activeScreen = 5;
        changeScreens();
    }

    public void hitContinueFirstEngagementPart2Screen()
    {
        for (int i = 0; i < engagement12.Length; i++)
        {
            sendResults((engagement12[i].value + 1).ToString());
        }
        activeScreen = 6;
        changeScreens();
    }

    public void hitSecondPlay()
    {
        numVisits = 2;
        SceneManager.LoadScene(1);
    }

    public void hitContinueSecondCognitiveScreen()
    {
        sendResults((cogLoadTwo.value + 1).ToString());
        activeScreen = 8;
        changeScreens();
    }

    public void hitContinueSecondEngagementPart1Screen()
    {
        for (int i = 0; i < engagement21.Length; i++)
        {
            sendResults((engagement21[i].value + 1).ToString());
        }
        activeScreen = 9;
        changeScreens();
    }

    public void hitContinueSecondEngagementPart2Screen()
    {
        for (int i = 0; i < engagement22.Length; i++)
        {
            sendResults((engagement22[i].value + 1).ToString());
        }
        activeScreen = 10;
        changeScreens();
    }

    public void hitContinueCommentScreen()
    {
        sendResults(comments.text);
        activeScreen = 11;
        changeScreens();
        ArrayList metrics = Psychometrics.getMetrics();
        // Google Sheets has a hard limit of 256 columns
        foreach(string item in metrics)
        {
            sheets.Add(item);
        }
        while (sheets.Count > 255)
        {
            sheets.RemoveAt(sheets.Count - 1);
        }
        UnityDataConnector.instance.SendDataToSheet((string[])sheets.ToArray(typeof(string)));
    }

    // In addition to the column limit, there is a limit of 6100 chars total. In case we still go over, use this to chop list in half.
    public static void largeRequest()
    {
        int count = sheets.Count / 2;
        while (sheets.Count > count)
        {
            sheets.RemoveAt(sheets.Count - 1);
        }
        UnityDataConnector.instance.SendDataToSheet((string[])sheets.ToArray(typeof(string)));
    }

}
