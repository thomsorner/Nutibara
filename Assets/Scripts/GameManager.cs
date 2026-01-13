using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("Scenes")]
    [SerializeField] private string mainScene = "Main";
    [SerializeField] private string lobbyScene = "Lobby";
    [SerializeField] private string corridorScene = "Corredor";

    private string currentFloor;

    // ================= LIFECYCLE =================

    private void Awake()
    {
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

    private void Start()
    {
        // Main es solo contenedor, el juego REAL inicia en Lobby
        StartCoroutine(LoadInitialFloor());
    }

    private IEnumerator LoadInitialFloor()
    {
        // Asegurar que Lobby est� cargado
        if (!SceneManager.GetSceneByName(lobbyScene).isLoaded)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(lobbyScene, LoadSceneMode.Additive);
            while (!loadOp.isDone)
                yield return null;
        }

        currentFloor = lobbyScene;

        // Opcional: establecer Lobby como escena activa
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(lobbyScene));
        ShowLobbyMessage();
    }

    // ================= PUBLIC API =================

    public void GoUp()
    {
        if (currentFloor == lobbyScene)
            StartCoroutine(SwapFloor(lobbyScene, corridorScene));
    }

    public void GoDown()
    {
        if (currentFloor == corridorScene)
            StartCoroutine(SwapFloor(corridorScene, lobbyScene));
    }

    // ================= CORE =================

    private IEnumerator SwapFloor(string from, string to)
    {
        // Cargar el nuevo piso
        if (!SceneManager.GetSceneByName(to).isLoaded)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
            while (!loadOp.isDone)
                yield return null;
        }

        // Activar la nueva escena
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(to));

        // Descargar la anterior
        if (SceneManager.GetSceneByName(from).isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(from);
            while (!unloadOp.isDone)
                yield return null;
        }

        currentFloor = to;
        
        if (to == corridorScene)
        {
            StartCoroutine(ShowEndDemoAfterDelay(10f));
        }
    }
    
    private void ShowLobbyMessage()
    {
        SceneMessageUI messageUI = Object.FindFirstObjectByType<SceneMessageUI>();

        if (messageUI)
        {
            messageUI.ShowMessage(
                "Completa la secuencia para desbloquear el ascensor e ir a la habitación 507."
            );
        }
    }
    
    private IEnumerator ShowEndDemoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        var endDemoUI = Object.FindFirstObjectByType<EndDemoUI>();
        if (endDemoUI)
        {
            endDemoUI.ShowEndDemo();
        }
    }
}
