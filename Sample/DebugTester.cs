using UnityEngine;

public class DebugTester : MonoBehaviour
{
    // Standard Logs
    public void Info()
    {
        Debug.Log("Info Test");
    }

    public void Warning()
    {
        Debug.LogWarning("Warning Test");
    }

    public void Error()
    {
        Debug.LogError("Error Test");
    }

    // Assertions (Forced to always trigger)
    public void LogAssert()
    {
        // Passing 'false' ensures the message always prints
        Debug.Assert(false, "Assert Test");
    }

    public void LogAssertion()
    {
        Debug.LogAssertion("Assertion Test");
    }

    // Exceptions
    public void LogException()
    {
        Debug.LogException(new System.Exception("Exception Test"));
    }

    // Formatted Logs
    public void InfoFormat()
    {
        Debug.LogFormat("InfoFormat Test");
    }

    public void WarningFormat()
    {
        Debug.LogWarningFormat("WarningFormat Test");
    }

    public void ErrorFormat()
    {
        Debug.LogErrorFormat("ErrorFormat Test");
    }

    public void Quit()
    {
        Application.Quit();
    }
}