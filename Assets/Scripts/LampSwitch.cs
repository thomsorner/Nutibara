using UnityEngine;
using TMPro;

public class LampSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Lamp targetLamp;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Settings")]
    [SerializeField] private string failMessage = "No pasó nada...";
    [SerializeField] private float dialogDuration = 2f;

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

        TryActivateLamp();
    }

    private void TryActivateLamp()
    {
        if (targetLamp == null)
            return;

        targetLamp.TurnOn();

        // Validación real
        if (targetLamp.IsOn)
        {
            used = true;
            HideDialog();
        }
        else
        {
            ShowDialog();
        }
    }

    // ================= DIALOG =================

    private void ShowDialog()
    {
        if (dialogText == null)
            return;

        dialogText.text = failMessage;
        dialogText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideDialog));
        Invoke(nameof(HideDialog), dialogDuration);
    }

    private void HideDialog()
    {
        if (dialogText != null)
            dialogText.gameObject.SetActive(false);
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
