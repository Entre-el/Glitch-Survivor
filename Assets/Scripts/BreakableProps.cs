using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BreakableProps : MonoBehaviour
{
    public Color damageColor = new Color(1,0,0,1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    Color originalColor;
    SpriteRenderer sr;
    public float health;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if(sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
            if(sr == null)
            {
                Debug.LogError("SpriteRenderer not found on " + gameObject.name);
            }
            else originalColor = sr.color;
        }
        else originalColor = sr.color;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Break();
        }
    }

    public void Break()
    {
        GetComponent<DropRateManager>().DropItem();
        StartCoroutine(BreakFade());
    }

    IEnumerator BreakFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;
        while(t < deathFadeTime)
        {
            yield return w;
            t+= Time.deltaTime;
            sr.color = new Color(sr.color.r,sr.color.g,sr.color.b,(1-t/deathFadeTime)*origAlpha);
        }
        Destroy(gameObject);
    }
}
