using UnityEngine;

public class Lamp : MonoBehaviour
{
    [Header("Initial State")]
    [SerializeField] private bool startOn = false;

    [Header("References")]
    [SerializeField] private Light pointLight;
    [SerializeField] private Renderer emissiveMesh;

    public bool IsOn { get; private set; }

    private void Awake()
    {
        SetState(startOn);
    }

    public void TurnOn()
    {
        SetState(true);
    }

    private void SetState(bool state)
    {
        IsOn = state;

        if (pointLight != null)
            pointLight.enabled = state;

        if (emissiveMesh != null)
            emissiveMesh.material.SetColor(
                "_EmissionColor",
                state ? Color.white * 2f : Color.black
            );
    }
}
