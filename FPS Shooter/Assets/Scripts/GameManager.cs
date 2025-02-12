using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Duration of the fade-to-black at the end of the game")]
    public float EndSceneLoadDelay = 3f;

    //[Tooltip("The canvas group of the fade-to-black screen")]
    //public CanvasGroup EndGameFadeCanvasGroup;

    //[Header("Win")]
    //[Tooltip("This string has to be the name of the scene you want to load when winning")]
    //public string WinSceneName = "WinScene";

    [Tooltip("Duration of delay before the fade-to-black, if winning")]
    public float DelayBeforeFadeToBlack = 4f;

    [Tooltip("Win game message")]
    public string WinGameMessage;
    [Tooltip("Duration of delay before the win message")]
    public float DelayBeforeWinMessage = 2f;

    //[Header("Lose")]
    //[Tooltip("This string has to be the name of the scene you want to load when losing")]
    //public string LoseSceneName = "LoseScene";


    public bool GameIsEnding { get; private set; }

    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;

    void Update()
    {
        if (GameIsEnding)
        {
            float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / EndSceneLoadDelay;
            //EndGameFadeCanvasGroup.alpha = timeRatio;


            // See if it's time to load the end scene (after the delay)
            if (Time.time >= m_TimeLoadEndGameScene)
            {
                SceneManager.LoadScene(m_SceneToLoad);
                GameIsEnding = false;
            }
        }
    }

    void EndGame(bool win)
    {
        // unlocks the cursor before leaving the scene, to be able to click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Remember that we need to load the appropriate end scene after a delay
        GameIsEnding = true;
        //EndGameFadeCanvasGroup.gameObject.SetActive(true);
        if (win)
        {
           // m_SceneToLoad = WinSceneName;
            m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay + DelayBeforeFadeToBlack;
        }
        else
        {
            //m_SceneToLoad = LoseSceneName;
            m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay;
        }
    }
}
