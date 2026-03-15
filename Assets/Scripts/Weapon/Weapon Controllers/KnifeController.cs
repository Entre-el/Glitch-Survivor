using System.Collections;
using UnityEngine;

public class KnifeController : WeaponController
{
    [Header("Multi-shot Settings")]
    public int knifeCount = 2;
    public float spawnDelay = 0.15f;
    public float spreadDistance = 0.2f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        StartCoroutine(FireKnivesCoroutine());
    }

    private IEnumerator FireKnivesCoroutine()
    {
        Vector2 aimDirection = pm.lastMoveVector.normalized;
        Vector2 perpendicular = new Vector2(-aimDirection.y, aimDirection.x);

        for (int i = 0; i < knifeCount; i++)
        {
            float offsetMultiplier = (i - (knifeCount - 1) / 2f); 
            Vector3 spawnOffset = perpendicular * (offsetMultiplier * spreadDistance);

            GameObject spawnedKnife = Instantiate(weaponData.Prefab);
            spawnedKnife.transform.position = transform.position + spawnOffset;
            spawnedKnife.transform.parent = null; 
            spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMoveVector);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
