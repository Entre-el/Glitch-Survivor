using UnityEngine;
using UnityEngine.UI;
using System;   
using TMPro;
using System.Collections;
public class LoadingPanel : BasePanel
{
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;
    private SceneSO currentSceneData;
    private int currentLogIndex = 0;
    public override void OnShow()
    {
        currentLogIndex = 0;
        EventCenter.AddListener<SceneSO>(EventDefine.OnLoadingStart,StartLoadingSequence);
        EventCenter.AddListener<SceneSO>(EventDefine.OnPoolInit, OnPoolInited);
        base.OnShow();
    }
    public override void OnHide()
    {
        EventCenter.RemoveListener<SceneSO>(EventDefine.OnLoadingStart,StartLoadingSequence);
        EventCenter.RemoveListener<SceneSO>(EventDefine.OnPoolInit,OnPoolInited);
        base.OnHide();
    }
    private void StartLoadingSequence(SceneSO sceneData){
        currentSceneData = sceneData;
        StartCoroutine(LoadingSequence());
    }
    private void OnPoolInited(SceneSO sceneData){
        currentSceneData = sceneData;
        StartCoroutine(OnPoolInitedSequence());
    }
    private IEnumerator LoadingSequence(){
        loadingText.SetText($"> 建立连接: {currentSceneData.loadingMainText}");
        yield return new WaitForSeconds(0.1f); // 视觉缓冲

        loadingSlider.value = 0.1f;
        loadingText.SetText(PullNextLog("正在读取底层场景资产..."));
        yield return new WaitForSeconds(0.1f);

        loadingSlider.value = 0.2f;
        loadingText.SetText(PullNextLog("正在预分配对象池内存区块..."));
        yield return null; // 等一帧，让上面的 UI 文字真正在屏幕上渲染出来
        loadingSlider.value = 0.3f;
    }

    private IEnumerator OnPoolInitedSequence(){
        loadingSlider.value = 0.9f;
        loadingText.SetText(PullNextLog("执行强制 GC 内存碎片回收..."));
        yield return null; 

        System.GC.Collect();

        loadingSlider.value = 1f;
        loadingText.SetText(PullNextLog("系统重组完毕。进入实战。"));
        yield return new WaitForSeconds(0.15f); // 故意停留极其短暂的瞬间，营造一种“系统跑得太快了”的错觉

        EventCenter.Broadcast(EventDefine.OnLoadingScreenFinished);
    }
    private string PullNextLog(String fallback) 
    {
       string result;
       if(currentLogIndex < currentSceneData.loadingLogTexts.Length && currentSceneData.loadingLogTexts[currentLogIndex] != null){
        result = currentSceneData.loadingLogTexts[currentLogIndex];
        currentLogIndex++; // 拿完之后，游标自动前进！
        return result;
       }
       else{
        return "> " + fallback;
       }
    }
}
