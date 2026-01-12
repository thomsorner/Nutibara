using UnityEngine;

public class FinalLampSwitch : MonoBehaviour
{
    [Header("Objects to remove")]
    [SerializeField] private GameObject[] objectsToRemove;

    private bool playerInside;
    private bool used;

    // ================= INTERACTION =================

    /// <summary>
    /// Llamado por el PlayerController (E o botón móvil)
    /// </summary>
    public void Interact()
    {
        if (!playerInside || used)
            return;

        RemoveObjects();
    }

    private void RemoveObjects()
    {
        used = true;

        foreach (GameObject obj in objectsToRemove)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    // ================= TRIGGER =================

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
