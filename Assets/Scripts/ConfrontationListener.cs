using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfrontationListener : MonoBehaviour
{
    private bool _unloaded;
    private void Update()
    {
        if (Confrontation.GetCurrentOutcome() == Outcome.NotAvailable) return;
        if (!_unloaded)
        {
            _unloaded = true;
            Confrontation.ConfrontationSceneLoaded = false;
            SceneManager.UnloadSceneAsync(1);
        }
    }
}