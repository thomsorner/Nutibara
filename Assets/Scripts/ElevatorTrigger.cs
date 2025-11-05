using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorTrigger : MonoBehaviour
{
    private bool playerInside = false;

    private void Update()
    {
        if (!playerInside) return;

        // Verifica si hay teclado disponible
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Subir
        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            Debug.Log("[ElevatorTrigger] ↑ Flecha arriba presionada dentro del elevador.");

            if (SceneLoader.Instance == null)
            {
                Debug.LogError("[ElevatorTrigger] ❌ SceneLoader.Instance es NULL. Asegúrate de tener un objeto con SceneLoader activo en la escena Main.");
                return;
            }

            SceneLoader.Instance.GoUp();
        }

        // Bajar
        if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            Debug.Log("[ElevatorTrigger] ↓ Flecha abajo presionada dentro del elevador.");

            if (SceneLoader.Instance == null)
            {
                Debug.LogError("[ElevatorTrigger] ❌ SceneLoader.Instance es NULL. Asegúrate de tener un objeto con SceneLoader activo en la escena Main.");
                return;
            }

            SceneLoader.Instance.GoDown();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("[ElevatorTrigger] 🚶‍♂️ El jugador ha ENTRADO al trigger del elevador.");
        }
        else
        {
            Debug.Log($"[ElevatorTrigger] Otro objeto entró al trigger: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("[ElevatorTrigger] 🚶‍♂️ El jugador ha SALIDO del trigger del elevador.");
        }
    }
}
