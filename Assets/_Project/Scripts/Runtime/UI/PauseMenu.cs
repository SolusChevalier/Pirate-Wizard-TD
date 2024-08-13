using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneNav;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            /* try
             {
                 SFXManager.Instance.PlaySFX(SFXManager.Instance.pause);
             }
             catch (NullReferenceException)
             {
                 Debug.Log("you are working in the editor, so you havent loaded the instance in the main menu");
             }*/
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        EventManager.OnGameResumed?.Invoke();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        EventManager.OnGamePaused?.Invoke();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        var random = new System.Random();
        var scene = random.Next(1, 100);
        if (scene % 2 == 0)
        {
            SceneManager.LoadScene(Scenes.Red1Blue2.ToString());
        }
        else
        {
            SceneManager.LoadScene(Scenes.Red2Blue1.ToString());
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}