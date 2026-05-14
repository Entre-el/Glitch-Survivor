// // 仅用于被扫描实体的被动效果接口
// public interface IContactEffector
// {
//     void ApplyContactEffect(PlayerDamageReceiver player);
// }

// public class SlimeEnemy : PoolItem, IContactEffector
// {
//     public float baseDamage = 10f;

//     // 当玩家扫描到该实体时，由玩家主动调用
//     public void ApplyContactEffect(PlayerDamageReceiver player)
//     {
//         player.TakeDamage(baseDamage);

//         // 扩展：例如给玩家上减速Buff
//         // player.AddBuff(new SlowBuff());
//     }
// }
