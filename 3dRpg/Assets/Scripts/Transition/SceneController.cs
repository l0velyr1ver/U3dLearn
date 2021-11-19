using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent agent;
    public SceneFader sceneFaderPrefab;
    bool fadeFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition (string sceneName,TransitionDestination.DestinationTag destinationTag)
    {

        SaveManager.Instance.SavePlayerData();
        if(SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            agent = player.GetComponent<NavMeshAgent>();
            agent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            agent.enabled = true;
            yield return null;
        }

    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }


    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if(scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2.5f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab,GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.5f));
            yield break;
        }
        
    }


    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("main");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }


    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        for ( int i = 0; i < entrances.Length; i++)
        {
            if(entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }


        return null;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        } 
    }
}
