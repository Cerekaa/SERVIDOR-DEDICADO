using UnityEngine;
using TMPro;

public class ConsoleToUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField consoleInputField;
    private string logText = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logText += $"[{type}] {logString}\n";

        if (logText.Length > 10000)
        {
            logText = logText.Substring(logText.Length - 10000); // Mantiene los últimos 10K caracteres
        }

        consoleInputField.text = logText;
        consoleInputField.MoveTextEnd(false); // Auto-scroll al final
    }
}
