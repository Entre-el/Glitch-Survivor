using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float currentHealth = 20f;
    private Rigidbody2D rb;

    [Header("醉酒状态记录")]
    private int drunkStacks = 0;
    private float drunkTimer = 0f;
    private float defaultSpeed = 3f; // 假设怪物原始移速是3

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 假设这里初始化你的默认移速
    }

    private void Update()
    {
        // 持续处理醉酒状态
        if (drunkStacks > 0)
        {
            drunkTimer -= Time.deltaTime;
            if (drunkTimer <= 0)
            {
                RemoveDrunkEffect();
            }
        }
    }

    // 暴露给葡萄贴纸和葡萄酒区域的接口
    public void AddDrunkStack()
    {
        drunkStacks++;
        drunkTimer = 2f; // 刷新持续时间
        
        // 叠加减速：每一层降低 70% 移速？或者固定降到 30%？
        // 这里采用：只要有醉酒，移速就变成 30%，防止叠多层变成负数反向跑
        UpdateSpeed();

        // 停止上一个毒伤协程，重新开始
        StopAllCoroutines(); 
        StartCoroutine(DrunkDoTCoroutine());
        
        Debug.Log($"<color=#800080>🍷 怪物叠了 {drunkStacks} 层醉酒！当前每秒毒伤：{drunkStacks * 2}</color>");
    }

    private IEnumerator DrunkDoTCoroutine()
    {
        // 只要还有层数，每秒扣血
        while (drunkStacks > 0)
        {
            yield return new WaitForSeconds(1f);
            
            float dotDamage = drunkStacks * 2f;
            TakeDamage(dotDamage, false); // 毒伤不触发暴击
        }
    }

    private void RemoveDrunkEffect()
    {
        drunkStacks = 0;
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        // 如果你的怪物移动逻辑在别的地方，这里提供思路：
        float currentSpeed = drunkStacks > 0 ? defaultSpeed * 0.3f : defaultSpeed;
        // 把 currentSpeed 赋值给怪物的 NavMeshAgent 或自定义移动脚本
    }

    public void TakeDamage(float damage, bool isCrit)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}