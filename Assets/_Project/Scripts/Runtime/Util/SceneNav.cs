using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

[Serializable]
public class SceneNav : MonoBehaviour
{
    public double easyDifficulty = 0.1;
    public double mediumDifficulty = 0.2;
    public double hardDifficulty = 0.3;

    public enum Scenes
    {
        MainMenu,
        Red1Blue2,
        Red2Blue1,
        AIBattle,
        RedWin,
        BlueWin,
        AIWin,
        AILose
    }

    public enum UICanvas
    {
        MainMenuCanvas,
        GameModeSelectCanvas,
        SinglePlayerCanvas,
        MultiPlayerDiffSelectCanvas,
    }

    public float fadeTime = 1.0f;
    public GameObject MainMenuCanvas;
    public GameObject GameModeSelectCanvas;
    public GameObject SinglePlayerCanvas;
    public GameObject MultiPlayerDiffSelectCanvas;
    public Dictionary<UICanvas, GameObject> canvasDict;
    public Dictionary<GameObject, UICanvas> CanvasGameObjDict;
    public GameObject currentCanvas;

    #region UNITY METHODS

    public void Start()
    {
        canvasDict = new Dictionary<UICanvas, GameObject>
        {
            { UICanvas.MainMenuCanvas, MainMenuCanvas },
            { UICanvas.GameModeSelectCanvas, GameModeSelectCanvas },
            { UICanvas.SinglePlayerCanvas, SinglePlayerCanvas },
            { UICanvas.MultiPlayerDiffSelectCanvas, MultiPlayerDiffSelectCanvas },
        };

        CanvasGameObjDict = new Dictionary<GameObject, UICanvas>
        {
            { MainMenuCanvas, UICanvas.MainMenuCanvas },
            { GameModeSelectCanvas, UICanvas.GameModeSelectCanvas },
            { SinglePlayerCanvas, UICanvas.SinglePlayerCanvas },
            { MultiPlayerDiffSelectCanvas, UICanvas.MultiPlayerDiffSelectCanvas },
        };

        foreach (var item in canvasDict)
        {
            item.Value.gameObject.SetActive(false);
        }

        MainMenuCanvas.SetActive(true);
        MainMenuCanvas.GetComponent<CanvasFade>().setAlpha(1);
        currentCanvas = MainMenuCanvas;
    }

    public void loadMainMenuCanvas()
    {
        if (currentCanvas == MainMenuCanvas)
        {
            return;
        }
        if (currentCanvas != null)
        {
            //currentCanvas.GetComponent<CanvasFade>().FadeOutCanvas(fadeTime);

            StartCoroutine(FadeOutCanvas(currentCanvas));
        }

        currentCanvas = MainMenuCanvas;
        //currentCanvas.SetActive(true);
        //MainMenuCanvas.GetComponent<CanvasFade>().FadeInCanvas(fadeTime);
        StartCoroutine(FadeInCanvas(MainMenuCanvas));
    }

    public void loadGameModeSelectCanvas()
    {
        if (currentCanvas == GameModeSelectCanvas)
        {
            return;
        }
        if (currentCanvas != null)
        {
            //currentCanvas.GetComponent<CanvasFade>().FadeOutCanvas(fadeTime);
            StartCoroutine(FadeOutCanvas(currentCanvas));
        }

        currentCanvas = GameModeSelectCanvas;
        //currentCanvas.SetActive(true);
        //GameModeSelectCanvas.GetComponent<CanvasFade>().FadeInCanvas(fadeTime);
        StartCoroutine(FadeInCanvas(GameModeSelectCanvas));
    }

    public void loadSinglePlayerCanvas()
    {
        if (currentCanvas == SinglePlayerCanvas)
        {
            return;
        }
        if (currentCanvas != null)
        {
            currentCanvas.GetComponent<CanvasFade>().FadeOutCanvas(fadeTime);
            StartCoroutine(FadeOutCanvas(currentCanvas));
        }

        currentCanvas = SinglePlayerCanvas;
        //currentCanvas.SetActive(true);
        //SinglePlayerCanvas.GetComponent<CanvasFade>().FadeInCanvas(fadeTime);
        StartCoroutine(FadeInCanvas(SinglePlayerCanvas));
    }

    public void loadMultiPlayerDiffSelectCanvas()
    {
        if (currentCanvas == MultiPlayerDiffSelectCanvas)
        {
            return;
        }
        if (currentCanvas != null)
        {
            currentCanvas.GetComponent<CanvasFade>().FadeOutCanvas(fadeTime);
            StartCoroutine(FadeOutCanvas(currentCanvas));
        }

        currentCanvas = MultiPlayerDiffSelectCanvas;
        StartCoroutine(FadeInCanvas(MultiPlayerDiffSelectCanvas));
        //currentCanvas.SetActive(true);
        //MultiPlayerDiffSelectCanvas.GetComponent<CanvasFade>().FadeInCanvas(fadeTime);
    }

    public void LoadCanvas(UICanvas canvas)
    {
        foreach (var item in canvasDict)
        {
            //item.Value.gameObject.SetActive(false);
            if (item.Value.gameObject.activeSelf & item.Key != canvas)
            {
                StartCoroutine(FadeOutCanvas(item.Value));
            }
        }
        StartCoroutine(FadeInCanvas(canvasDict[canvas]));
        //canvasDict[canvas].gameObject.SetActive(true);
    }

    public void StartRandom()
    {
        var random = new System.Random();
        var scene = random.Next(1, 100);
        if (scene % 2 == 0)
        {
            LoadScene(Scenes.Red1Blue2.ToString());
        }
        else
        {
            LoadScene(Scenes.Red2Blue1.ToString());
        }
    }

    public void StartRed()
    {
        LoadScene(Scenes.Red1Blue2.ToString());
    }

    public void StartBlue()
    {
        LoadScene(Scenes.Red2Blue1.ToString());
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApp()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.ExitPlaymode();
    }

    #endregion UNITY METHODS

    #region METHODS

    public IEnumerator FadeInCanvas(GameObject canvas)
    {
        //Debug.Log("Fading in canvas");
        yield return new WaitForSeconds(fadeTime);
        canvas.SetActive(true);
        //var img = canvas.GetComponent<UnityEngine.UI.Image>();
        canvas.GetComponent<CanvasFade>().FadeInCanvas(fadeTime);
        /*CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.alpha = Mathf.Lerp(0, 1, fadeTime);*/

        //yield return new WaitForSeconds(1);
    }

    public IEnumerator FadeOutCanvas(GameObject canvas)
    {
        //var img = canvas.GetComponent<UnityEngine.UI.Image>();
        //img.CrossFadeAlpha(1, fadeTime, true);
        canvas.GetComponent<CanvasFade>().FadeOutCanvas(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        canvas.SetActive(false);
    }

    #endregion METHODS
}