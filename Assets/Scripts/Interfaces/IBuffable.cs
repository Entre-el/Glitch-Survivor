using UnityEngine;

// 任何可受击的实体必须实现此接口
public interface IBuffable
{
    public void AddBuff(BaseEnemyBuff newBuff, float duration = 2f, int stackCount = 1)
    {
        throw new System.NotImplementedException();
    }
    public void RemoveBuff(BaseEnemyBuff buff)
    {
        throw new System.NotImplementedException();
    }
    public void UpdateBuffDisplay();
}
