using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject Pause_Menu;
    [SerializeField] GameObject PlayerHUD;
    float originalTimeScale;
    [SerializeField] float slowTimeScale = 0.3f;

    public enum GameState
    {
        Normal,
        Paused,
        InMenu,
    }
    public GameState currentGameState;

    public void Start()
    {
        originalTimeScale = Time.timeScale;
    }

    private void Update()
    {
        if (Pause_Menu != null && Pause_Menu.activeInHierarchy)
        {
            currentGameState = GameState.Paused;
        }
    }

    void FixedUpdate()
    {
        switch (currentGameState)
        {
            case GameState.Normal:
                break;
            case GameState.Paused:
                TurnOffNormalUI();
                StopTime();
                break;
            case GameState.InMenu:
                //CursorModeOn();
                break;
        }
    }
    public void TurnOnNormalUI()
    {
        PlayerHUD.SetActive(true);
    }
    public void TurnOffNormalUI()
    {
        PlayerHUD.SetActive(false);
    }

    public void StopTime()
    {
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void NormalTime()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = originalTimeScale;
    }

    public void SlowTime()
    {
        Time.timeScale = slowTimeScale;
    }
}