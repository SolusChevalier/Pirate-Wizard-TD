using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class GameScenes
{
    #region FIELDS

    public enum Scenes
    {
        MainMenu,
        Game,
        Win,
        Lose
    }

    #endregion FIELDS

    #region METHODS

    public static string GetSceneName(Scenes scene)
    {
        return scene.ToString();
    }

    public static string GetSceneName(int scene)
    {
        return ((Scenes)scene).ToString();
    }

    public static void LoadScene(Scenes scene)
    {
        SceneManager.LoadScene(GetSceneName(scene));
    }

    public static void LoadScene(int scene)
    {
        SceneManager.LoadScene(GetSceneName(scene));
    }

    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void ExitApp()
    {
        //UnityEditor.EditorApplication.ExitPlaymode();
        UnityEngine.Application.Quit();
    }

    #endregion METHODS
}