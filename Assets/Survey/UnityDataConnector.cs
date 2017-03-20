using System.Collections;
using UnityEngine;
 

/// <summary>
/// Unity to GoogleSheet connector Singleton
/// </summary>
public class UnityDataConnector : MonoBehaviour
{
	public bool updating;
	public string currentStatus;

	public static UnityDataConnector instance;

	void Awake()
	{
		if(instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	void Start ()
	{
		updating = false;
		currentStatus = "Offline";

		DontDestroyOnLoad(gameObject);

		ConnectToGoogleSheet();
	}

    public void ConnectToGoogleSheet()
	{
		if (updating)
			return;

		StartCoroutine(StartConecting());   
	}

	IEnumerator StartConecting()
	{
		updating = true;

		string connectionString = Constants.url + "?ssid=" + Constants.id + "&sheet=" + Constants.statisticsSheetName + "&pass=" + Constants.password + "&action=GetData";
		WWW www = new WWW(connectionString);
		float elapsedTime = 0.0f;
		currentStatus = "Establishing Connection... ";
		while (!www.isDone)
		{
			elapsedTime += Time.deltaTime;			
			if (elapsedTime >= Constants.maxWaitTime)
			{
				currentStatus = "Connection aborted. TimeUp.";
				Debug.Log(currentStatus);
				updating = false;
				break;
			}
			
			yield return null;  
		}

		if (!www.isDone || !string.IsNullOrEmpty(www.error))
		{
			currentStatus = "Connection error after" + elapsedTime.ToString() + "seconds: " + www.error;
			Debug.Log(currentStatus);
			updating = false;
			yield break;
		}

		currentStatus = "Connection established.";
		Debug.Log("Connection established... ");
		updating = false;
	}

    /// <summary>
    /// Pass a string array (size < 256), with the data to be saved on the defined google sheet.
    /// </summary>
    /// <param name="data"></param>
	public void SendDataToSheet(string[] data)
	{
		StartCoroutine(SendData(data));
	}
	
	IEnumerator SendData(string[] paramraw )
	{
        string data = "";
        for(int i = 0; i < paramraw.Length; i++)
        {
            data += "&info=" + WWW.EscapeURL(paramraw[i]);
        }

		if(!updating)
		{
            string connectionString = Constants.url + "?ssid=" + Constants.id + "&sheet=" + Constants.statisticsSheetName + "&pass=" + Constants.password
                                     + data + "&action=SetData";

			WWW www = new WWW(connectionString);
			float elapsedTime = 0.0f;

            //FileManagement.wwwDump("param: " + string.Format("[{0}]", string.Join(",", paramraw)));
            //FileManagement.wwwDump("data: " + data);
            //FileManagement.wwwDump("connectionString: " + connectionString);
			
			while (!www.isDone)
			{
				elapsedTime += Time.deltaTime;			
				if (elapsedTime >= Constants.maxWaitTime)
				{
					break;
				}
				
				yield return null;  
			}

            if (!www.isDone || !string.IsNullOrEmpty(www.error))
			{
				Debug.LogError ("Connection error while sending analytics... Error:" + www.error);

				// Error handling here.
                //FileManagement.wwwDump("Error: " + www.error + " Text: " + www.text);
                if (www.error.Equals("Error: 413 Request Entity Too Large"))
                {
                    yield return new WaitForSeconds(1);
                    Survey.largeRequest();
                }
                yield break;
			}
			
			if (www.text.Contains("OKs"))
			{
				Debug.Log("Data Sent successfully.");
				yield break;
			}
		}
	}
}
	
	class Constants
{
    public static string password = "4V1QsJGcyGL8EdPlditCA5KKHdqiX0U5rBGO7D8gce96IcdA359sXJxnOrvDFNjkHTR1QnJvnbFJ2veVuIHTUpGc3zO4BVVHGy9u6qeTWfEp7JDJJlIdLladyp4zZBF6";
    public static float maxWaitTime = 20f;
    public static string statisticsSheetName = "Statistics";

    public static string url = "https://script.google.com/macros/s/AKfycbzb80Fy832LRMln9-o2bLXh_w-YZLDZIV_jPy9fpafpI0rOD1Ro/exec";
    public static string id = "14CL2AD6lRn0BO5LAcrmi0c8dZ3_Q9Igkvly0hAzZVXM";
}