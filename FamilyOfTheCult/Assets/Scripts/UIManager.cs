using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public AudioSource audioSource;

    public GameObject[] CloseUI;
    public GameObject[] OpenUI;
    [SerializeField] ControllerForPlayer playerScript;
    [SerializeField] GameManager gameManager;

    // Switch UIs
    public void SwitchUI()
    {
        if (CloseUI != null)
        {
            //Disable all animators at start
            for (int i = 0; i < CloseUI.Length; i++)
                CloseUI[i].SetActive(false);
        }

        if (OpenUI != null)
        {
            //Disable all animators at start
            for (int i = 0; i < OpenUI.Length; i++)
                OpenUI[i].SetActive(true);
        }
    }


    //Main Menu Functions
    public void PlayNextScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //Pause Menu Functions
    public void ClosePauseUI()
    {
        gameManager.NormalTime();
        gameManager.TurnOnNormalUI();
        gameManager.currentGameState = GameManager.GameState.Normal;

        if (CloseUI != null)
        {
            //Close all previous UIs
            for (int i = 0; i < CloseUI.Length; i++)
                CloseUI[i].SetActive(false);
        }
    }
    public void ResetToCheckpoint()
    {
        //playerScript.Respawn();
        gameManager.NormalTime();

        if (CloseUI != null)
        {
            //Close all previous UIs
            for (int i = 0; i < CloseUI.Length; i++)
                CloseUI[i].SetActive(false);
        }
    }
    public void ReloadScene()
    {
        gameManager.NormalTime();

        if (CloseUI != null)
        {
            //Close all previous UIs
            for (int i = 0; i < CloseUI.Length; i++)
                CloseUI[i].SetActive(false);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
