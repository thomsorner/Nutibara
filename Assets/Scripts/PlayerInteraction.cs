using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public LayerMask interactableLayer;

    private Camera mainCamera;
    private PickupItem currentItem;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        DetectItem();

        // Click izquierdo = recoger
        if (Mouse.current.leftButton.wasPressedThisFrame && currentItem != null)
        {
            Pickup(currentItem);
        }
    }

    /// <summary>
    /// Detecta si el jugador está mirando un objeto recogible.
    /// </summary>
    void DetectItem()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();

            if (item != null)
            {
                // Si es un nuevo objeto, quitar highlight del anterior
                if (currentItem != null && currentItem != item)
                    currentItem.Highlight(false);

                currentItem = item;
                currentItem.Highlight(true);
                return;
            }
        }

        ClearHighlight();
    }

    /// <summary>
    /// Quita el highlight del objeto actual.
    /// </summary>
    void ClearHighlight()
    {
        if (currentItem != null)
        {
            currentItem.Highlight(false);
            currentItem = null;
        }
    }

    /// <summary>
    /// Recoge el objeto actual y lo añade al inventario.
    /// </summary>
    void Pickup(PickupItem item)
    {
        if (item.itemData == null)
        {
            Debug.LogWarning("PickupItem no tiene asignado un Item (ScriptableObject).");
            return;
        }

        Debug.Log($"Recogiste {item.itemData.itemName}");

        // Se lo pasamos al inventario
        Inventory.instance.AddItem(item.itemData);

        // Destruimos el objeto en el mundo
        Destroy(item.gameObject);
        currentItem = null;
    }
}
