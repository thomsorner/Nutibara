using UnityEngine;
using TMPro;

public class EndDemoUI : MonoBehaviour
{
    [SerializeField] private GameObject messageRoot;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake()
    {
        messageRoot.SetActive(false);
    }

    public void ShowEndDemo()
    {
        messageText.text =
            "Felicitaciones por llegar hasta aquí.\n\nFin del demo.";

        messageRoot.SetActive(true);

        // Pausar el juego
        Time.timeScale = 0f;
    }
}
