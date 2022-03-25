using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject GameOverPanel,WinPanel;

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        GameOverPanel.SetActive(true);
        ComboManager.Instance.StopCombo();
    }

    public void Win()
    {
        WinPanel.SetActive(true);
        ComboManager.Instance.StopCombo();
    }
}
