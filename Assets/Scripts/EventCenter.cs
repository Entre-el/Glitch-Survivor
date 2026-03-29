using System;
using System.Collections.Generic;
using UnityEngine;


public static class EventCenter
{
    private static Dictionary<EventDefine, Delegate> eventTable = new(100);
    public static void AddListener<T>(EventDefine eventType, Action<T> handler)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, handler);
        }
        else
        {
            eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
        }
    }
        public static void AddListener(EventDefine eventType, Action handler)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, handler);
        }
        else
        {
            eventTable[eventType] = (Action)eventTable[eventType] + handler;
        }
    }
    public static void RemoveListener<T>(EventDefine eventType, Action<T> handler)
    {
        if (eventTable.ContainsKey(eventType))
        {
            // 从委托链中“剥离（-=）”这个方法指针
            eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;
            
            // 如果频道里一个听众都没了，就把频道从字典里删掉，节约内存
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }
    public static void Broadcast(EventDefine eventType)
    {
        if (eventTable.TryGetValue(eventType, out Delegate d))
        {
            // 将底层的 Delegate 强转回 Action，然后执行
            if (d is Action action)
            {
                action.Invoke(); // 瞬间触发所有绑定的方法
            }
            else
            {
                Debug.LogError($"广播事件 {eventType} 的参数类型不匹配！没有Action类型");
            }
        }
    }
    public static void Broadcast<T>(EventDefine eventType, T arg)
    {
        if (eventTable.TryGetValue(eventType, out Delegate d))
        {
            // 将底层的 Delegate 强转回 Action<T>，然后执行
            if (d is Action<T> action)
            {
                action.Invoke(arg); // 瞬间触发所有绑定的方法
            }
            else
            {
                Debug.LogError($"广播事件 {eventType} 的参数类型不匹配！");
            }
        }
    }
}
public enum EventDefine
{
    OnRequestSceneChange,
    OnPlayerDied,
    OnGameOver,
    OnGameWin,
    OnGameQuit,
    OnLevelUpRequest,
    OnResumeRequest,
    OnRestartRequest,
    OnQuitRequest,
    OnLoadingStart,
    OnLoadingScreenShown,
    OnLoadingScreenReady,
    OnPoolInit,
    OnLoadingScreenFinished,
    OnBossDied,
    OnExpChanged,
    OnLevelUp,
    OnWeaponLevelUp,
    OnOptionsPicked,
}