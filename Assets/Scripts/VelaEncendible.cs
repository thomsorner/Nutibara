using UnityEngine;

public class VelaEncendible : MonoBehaviour
{
    public GameObject fuego; // asigna aquí el prefab/objeto de la llama
    private bool encendida = false;

    public void Encender()
    {
        if (!encendida)
        {
            fuego.SetActive(true);
            encendida = true;
        }
    }
}
