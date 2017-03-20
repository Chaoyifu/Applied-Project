using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour {
    public GameObject GameOverInfo;

    public void BackOnClick() {
        Time.timeScale = 1;
        GameOverInfo.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void RestartOnClick(){
        Time.timeScale = 1;
        GameOverInfo.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }
}
