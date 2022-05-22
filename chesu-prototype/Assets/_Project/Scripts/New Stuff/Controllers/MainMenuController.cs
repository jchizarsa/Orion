using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance;
    [SerializeField] GameObject mainMenuObjects;
    [SerializeField] GameObject howToMenuObjects;
    [SerializeField] GameObject controlsObjects;
    [SerializeField] GameObject rulesObjects;
    [SerializeField] GameObject winObjects;
    [SerializeField] GameObject gameBoard;
    [SerializeField] GameObject mainCamera;
    void Awake(){
        Instance = this;
    }
    public void PlayGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame(){
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    public void ReturnToMainMenu(){
        winObjects.SetActive(false);
        SceneManager.LoadScene(0);
    }
    public void HowToPlay(){
        mainMenuObjects.SetActive(false);
        howToMenuObjects.SetActive(true);
    }
    public void ReturnToMainFromHowTo(){
        howToMenuObjects.SetActive(false);
        mainMenuObjects.SetActive(true);
    }
    public void OpenControls(){
        howToMenuObjects.SetActive(false);
        controlsObjects.SetActive(true);
    }
    public void CloseControls(){
        controlsObjects.SetActive(false);
        howToMenuObjects.SetActive(true);
    }
    public void OpenRules(){
        howToMenuObjects.SetActive(false);
        rulesObjects.SetActive(true);
    }
    public void CloseRules(){
        rulesObjects.SetActive(false);
        howToMenuObjects.SetActive(true);
    }
    public void OpenWinCam(){
        Destroy(gameBoard);
        Destroy(mainCamera);
        winObjects.SetActive(true);
    }
}
