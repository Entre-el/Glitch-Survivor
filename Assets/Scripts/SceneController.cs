using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip buttonSFX;
    public AudioClip sceneBGM;
    public float cutInDuration = 2f;
    public void SceneChange(string sceneName)
    {
        AudioManager.instance.PlaySFX(buttonSFX,false);
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
    void Start()
    {
        AudioManager.instance.CrossfadeBGM(sceneBGM,cutInDuration);
    }
}
