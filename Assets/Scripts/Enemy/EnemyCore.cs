using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyCore : MonoBehaviour
{
    public EnemyStats Stats { get; private set; }
    public EnemyMovement Movement { get; private set; }
    public TransformAnchorSO TargetAnchor { get; private set; }
    private void Awake()
    {
        TryGetComponent<EnemyStats>(out EnemyStats stats);
        Stats = stats;
        TryGetComponent<EnemyMovement>(out EnemyMovement movement);
        Movement = movement;

        if(TargetAnchor == null)
        {
            Debug.LogError("TargetAnchor is not set");
            return;
        }
    }
}