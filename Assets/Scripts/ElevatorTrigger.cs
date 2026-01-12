using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private bool playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("[ElevatorTrigger] Jugador entró al elevador");

        SceneLoader.Instance.GoUp(); // o GoDown según la escena
    }
}
