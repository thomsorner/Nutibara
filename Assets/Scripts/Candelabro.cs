using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/CandelabroItem")]
public class CandelabroItem : Item
{
    public override void Use()
    {
        Debug.Log("Encendiendo velas cercanas...");

        // Buscar todas las velas en el entorno (con un tag, por ejemplo)
        GameObject[] velas = GameObject.FindGameObjectsWithTag("Vela");
        foreach (var vela in velas)
        {
            // Aquí deberías tener un script "Vela" en cada una
            vela.GetComponent<VelaEncendible>().Encender();
        }
    }
}
