using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

   [Header("音源通道")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    [Tooltip("拾取音轨")]
    public AudioSource pickupSource;

    [Header("打击限流设置")]
    public float hitSoundCooldown = 0.05f; 
    private float lastHitTime;
    private Dictionary<AudioClip, float> sfxCooldowns = new(32);

    [Header("拾取连击设置")]
    public float comboResetTime = 0.5f;
    private float lastPickupTime;
    private float currentPickupPitch = 1f;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySFX(AudioClip clip, bool randomizePitch = true)
    {
        if (!clip) return;
        sfxSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
        sfxSource.PlayOneShot(clip);
    }
    public void PlayBGM(string clipName)
    {
        AudioClip clip = GetClipByName(clipName);
        if (!clip || bgmSource.clip == clip) return; // 如果正在播这首，就不管它
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = 1f; // 确保音量是满的
        bgmSource.Play();
    }
    public void FadeOutBGM(float fadeDuration = 1.5f)
    {
        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }
    public void CrossfadeBGM(string newClipName, float fadeDuration = 2.0f)
    {
        AudioClip newClip = GetClipByName(newClipName);
        if (bgmSource.clip == newClip) return;
        StartCoroutine(CrossfadeCoroutine(newClip, fadeDuration));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip, float duration)
    {
        float startVolume = bgmSource.volume;
        float t = 0;

        while (t < duration / 2) 
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / (duration / 2));
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();
        
        t = 0;
        while (t < duration / 2) 
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, startVolume, t / (duration / 2));
            yield return null;
        }
        bgmSource.loop = true;
        bgmSource.volume = startVolume; 
    }
    public void PlayHitSFX(AudioClip clip)
    {
        if (!clip) return;
        if (sfxCooldowns.TryGetValue(clip, out float lastTmieplayed) && Time.time - lastTmieplayed < hitSoundCooldown) return;
        sfxCooldowns[clip] = Time.time;
        PlaySFX(clip, true); 
    }

    public void PlayPickupSFX(AudioClip clip)
    {
        if (!clip) return;

        if (Time.time - lastPickupTime > comboResetTime)
        {
            currentPickupPitch = 1f;
        }
        pickupSource.pitch = currentPickupPitch;
        pickupSource.PlayOneShot(clip);
        currentPickupPitch = Mathf.Min(currentPickupPitch + 0.05f, 2.0f);
        lastPickupTime = Time.time;
    }
    public AudioClip GetClipByName(string clipName)
    {
        if(string.IsNullOrEmpty(clipName)) return null;
        AudioClip clip = Resources.Load<AudioClip>($"BGM/{clipName}");
        if(clip == null)
        {
            Debug.LogError($"AudioClip {clipName} not found");
            return null;
        }
        return clip;
    }
}