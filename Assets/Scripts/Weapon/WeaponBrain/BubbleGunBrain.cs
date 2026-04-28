using System.Collections; // 别忘了引入协程命名空间
using UnityEngine;

public class BubbleGunBrain : WeaponBrain
{
    [Header("泡泡枪专属配置")]
    public GameObject bubblePrefab; // 泡泡子弹预制体
    public float baseDamage = 5f;   // 基础伤害
    public int basePierce = 0;          // 基础穿透数（0表示不穿透
    public float baseKnockback = 1f;     // 基础击退力

    [Header("射击手感微调")]
    public float spreadAngle = 10f; // 散射角度
    public int burstCount = 4;      // 每次扣下扳机，吐出几个泡泡？
    public float burstInterval = 0.2f; // 吐泡泡的间隔时间（极快，噗噗噗的感觉）

    public override void Initialize(PlayerCore core)
    {
        base.Initialize(core);
        
        FireRate = 2f; 
        
        // 确保有一个插槽环境
        if (SlotManager.SubContexts.Count == 0)
        {
            SlotManager.SubContexts.Add(new SubWeaponContext { ContextName = "Main Bubble" });
        }
        ObjectPoolManager.Instance.RegisterPool(bubblePrefab);
    }

    protected override void ExecuteFire(Vector2 direction)
    {
        // 基类下达了开火指令，子类启动连发协程
        StartCoroutine(FireBurstCoroutine(direction));
    }

    private IEnumerator FireBurstCoroutine(Vector2 aimDirection)
        {
            for (int i = 0; i < burstCount; i++)
            {
                // 1. 组装当前发子弹的数据快照
                CombatPayload payload = SlotManager.GeneratePayload(0, baseDamage, basePierce,baseKnockback, core);

                // 2. 算好这颗泡泡的散射方向和枪口位置
                float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                Vector2 finalDirection = Quaternion.Euler(0, 0, randomAngle) * aimDirection;
                Vector3 spawnPos = emitter != null ? emitter.position : weaponPivot.position;

                // 🌟 3. 拦截点：是谁在调用 OnFireSlot？就是这里！
                if (payload.FireSticker != null)
                {
                    // 如果有开火贴纸（比如葡萄贴纸），把发射坐标、快照、方向全部交给它！
                    // 接下来怎么分裂、怎么发射，由贴纸里的代码说了算。
                    payload.FireSticker.OnFireSlot(payload, emitter, finalDirection);
                }
                else
                {
                    // 4. 如果没有开火贴纸，执行武器自带的、最原汁原味的发射逻辑
                    GameObject bubbleObj = ObjectPoolManager.Instance.Get(bubblePrefab, spawnPos, Quaternion.identity);
                    if (bubbleObj != null && bubbleObj.TryGetComponent<ProjectileBase>(out var projectile))
                    {
                        projectile.Initialize(payload, finalDirection);
                    }
                }

                // 等待一小段极短的时间，再吐下一发
                yield return new WaitForSeconds(burstInterval);
            }
        }
}