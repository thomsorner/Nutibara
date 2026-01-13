using UnityEngine;
using TMPro;
using System.Collections;

public class SceneMessageUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float showDuration = 10f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        messageText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(showDuration);

        messageText.gameObject.SetActive(false);
    }
}
