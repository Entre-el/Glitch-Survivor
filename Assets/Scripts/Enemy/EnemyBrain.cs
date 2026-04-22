using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyLocomotion))]
public class EnemyBrain : MonoBehaviour
{
    public EnemyStats Stats { get; private set; }
    public EnemyLocomotion Movement { get; private set; }
    public TransformAnchorSO TargetAnchor { get; private set; }
    private void Awake()
    {
        TryGetComponent<EnemyStats>(out EnemyStats stats);
        Stats = stats;
        TryGetComponent<EnemyLocomotion>(out EnemyLocomotion movement);
        Movement = movement;

        if(TargetAnchor == null)
        {
            Debug.LogError("TargetAnchor is not set");
            return;
        }
    }
}