using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/FlashlightItem")]
public class FlashlightItem : Item
{
    // No necesita lógica de Use()
    public override void Use()
    {
        Debug.Log("Linterna equipada");
        // No hacemos nada más porque la luz ya está encendida por su Light
    }
}
