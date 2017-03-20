using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Psychometrics : MonoBehaviour
{
    private static ArrayList metrics = new ArrayList();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(SceneManager.GetActiveScene().buildIndex != 0)
    //    {
    //        if(Input.GetMouseButtonDown(0))
    //        {
    //            logEvent("Mouse Click - X: " + Input.mousePosition.x + " Y: " + Input.mousePosition.y);
    //        }
    //    }
    //}

    public static void logEvent(string str)
    {
        metrics.Add(str);
    }

    public static ArrayList getMetrics()
    {
        return metrics;
    }
}
