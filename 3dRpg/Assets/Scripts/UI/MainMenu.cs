using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;

    PlayableDirector director;
    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTImeline);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }


    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstLevel();
    }


    void PlayTImeline()
    {
        director.Play();
    }

    void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("ÍË³öÓÎÏ·");
    }

}
