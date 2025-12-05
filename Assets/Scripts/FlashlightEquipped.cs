using UnityEngine;

public class FlashlightEquipped : MonoBehaviour
{
    private Light spot;

    private void Awake()
    {
        spot = GetComponentInChildren<Light>();

        if (spot == null)
            Debug.LogError(" El prefab de la linterna NO tiene un Light (Spot).");

        // Si la linterna debe estar siempre encendida
        if (spot != null)
            spot.enabled = true;
    }
}
