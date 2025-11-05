using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Referencia al ScriptableObject del Item")]
    public Item itemData; // este lo asignas en el Inspector

    [Header("Highlight Settings")]
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalMaterial = rend.material;
    }

    /// <summary>
    /// Activa o desactiva el highlight cuando el jugador mira el objeto.
    /// </summary>
    public void Highlight(bool state)
    {
        if (rend == null) return;

        if (state && highlightMaterial != null)
            rend.material = highlightMaterial;
        else if (originalMaterial != null)
            rend.material = originalMaterial;
    }

    /// <summary>
    /// Recoge el objeto y lo agrega al inventario.
    /// </summary>
    public void Pickup()
    {
        if (itemData == null)
        {
            Debug.LogWarning("PickupItem no tiene asignado un Item (ScriptableObject).");
            return;
        }

        // Añadir al inventario
        Inventory.instance.AddItem(itemData);

        // Destruir objeto en escena
        Destroy(gameObject);
    }
}
