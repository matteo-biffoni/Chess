using UnityEngine;

public class SimpleInteractionButtons : MonoBehaviour
{
    public void Win()
    {
        Confrontation.SetCurrentOutcome(Outcome.Success);
    }

    public void Lose()
    {
        Confrontation.SetCurrentOutcome(Outcome.Failure);
    }
}
