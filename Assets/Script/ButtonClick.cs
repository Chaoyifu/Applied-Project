using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour {

    public void BackOnClick() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void RestartOnClick(){
        Time.timeScale = 1;
        Application.LoadLevel("GameScene");
    }
}
