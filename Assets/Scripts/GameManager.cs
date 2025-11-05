using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    private string currentFloor = "Lobby"; // escena inicial

    private void Awake()
    {
        // Singleton para acceder desde otros scripts
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        // Cargar piso inicial (Lobby) y elevador
        yield return LoadSceneAdditive("Lobby");
        yield return LoadSceneAdditive("Elevator");
    }

    public IEnumerator LoadSceneAdditive(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadOp.isDone)
                yield return null;
        }
    }

    public IEnumerator UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            while (!unloadOp.isDone)
                yield return null;
        }
    }

    public void GoUp()
    {
        if (currentFloor == "Lobby")
            StartCoroutine(SwapFloor("Lobby", "Corredor"));
    }

    public void GoDown()
    {
        if (currentFloor == "Corredor")
            StartCoroutine(SwapFloor("Corredor", "Lobby"));
    }

    private IEnumerator SwapFloor(string from, string to)
    {
        yield return UnloadScene(from);
        yield return LoadSceneAdditive(to);
        currentFloor = to;
    }
}
