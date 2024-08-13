using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToStart : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(BackToStartButtonWithDelay(3f));
    }

    public IEnumerator BackToStartButtonWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}