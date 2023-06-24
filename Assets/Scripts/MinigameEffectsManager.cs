using System.Collections;
using UnityEngine;

public class MinigameEffectsManager : MonoBehaviour
{
    private Vector3 _startingPoint;
    [SerializeField] private GameObject MovingPrefab;
    [SerializeField] private GameObject NormalBugsPrefab;
    [SerializeField] private GameObject NormalVegsPrefab;
    [SerializeField] private GameObject SpecialBugsPrefab;
    [SerializeField] private GameObject SpecialVegsPrefab;

    public void SetStartingPoint(Vector3 transformSp)
    {
        _startingPoint = transformSp;
    }

    public IEnumerator SpawnProjectile(Vector3 aim)
    {
        var movingGo = Instantiate(MovingPrefab, _startingPoint, Quaternion.Euler(0f, 0f, 0f));
        while (Vector3.Distance(movingGo.transform.position, aim) > 0.01f)
        {
            movingGo.transform.position = Vector3.Lerp(movingGo.transform.position, aim, 10 * Time.deltaTime);
            yield return null;
        }
        Destroy(movingGo);
    }


    public IEnumerator SpawnAttack(AttackType attackType, Vector3 aim)
    {
        GameObject prefab = null;
        switch (attackType)
        {
            case AttackType.None:
                break;
            case AttackType.NormalBugs:
                prefab = NormalBugsPrefab;
                break;
            case AttackType.NormalVegs:
                prefab = NormalVegsPrefab;
                break;
            case AttackType.SpecialBugs:
                prefab = SpecialBugsPrefab;
                break;
            case AttackType.SpecialVegs:
                prefab = SpecialVegsPrefab;
                break;
        }
        if (prefab != null)
        {
            var effect = Instantiate(prefab, aim, Quaternion.Euler(0f, 0f, 0f));
            var duration = effect.GetComponent<ParticleSystem>().main.duration;
            yield return new WaitForSeconds(duration);
            Destroy(effect);
        }
    }
    
}

public enum AttackType
{
    None = 0,
    NormalBugs = 1,
    NormalVegs = 2,
    SpecialBugs = 3,
    SpecialVegs = 4
}
