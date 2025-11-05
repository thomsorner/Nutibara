using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    //public Sprite icon;

    [Header("Prefab (lo que se muestra en la mano)")]
    public GameObject itemPrefab;

    /// <summary>
    /// Acción que realiza este ítem cuando el jugador lo usa.
    /// </summary>
    public virtual void Use()
    {
        Debug.Log($"{itemName} usado!");
    }
}
