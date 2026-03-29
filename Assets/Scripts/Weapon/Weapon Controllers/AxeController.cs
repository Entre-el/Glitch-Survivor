using System.Collections;
using UnityEngine;

public class AxeController : WeaponController
{
    [Header("Swing Settings")]
    public float swingRadius = 1.5f;
    public float swingDuration = 0.25f;
    public float startAngle = -45f;
    public float sweepAngle = 270f;
    [Header("Audio")]
    public AudioClip swingSFX;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack() 
    {
        base.Attack();
        AudioManager.Instance.PlaySFX(swingSFX,true);
        GameObject spawnedAxe = ObjectPoolManager.Instance.Get(prefab);
        spawnedAxe.transform.parent = transform;

        int facingMultiplier = pm.lastMoveVector.x < 0 ? -1 : 1;

        StartCoroutine(SwingAxeCoroutine(spawnedAxe, facingMultiplier));
    }

    private IEnumerator SwingAxeCoroutine(GameObject axe, int facingMultiplier)
    {
        Animator slashAnim = axe.GetComponentInChildren<Animator>();
        if (slashAnim != null)
        {
            slashAnim.gameObject.SetActive(true);
            slashAnim.Rebind();
            slashAnim.Update(0f);
        }
        float t = 0f;

        float currentStartAngle = startAngle * facingMultiplier;
        float currentSweepAngle = sweepAngle * facingMultiplier;

        while (t < swingDuration)
        {
            t += Time.deltaTime;
            float progress = t / swingDuration;

            float currentAngle = currentStartAngle + (currentSweepAngle * progress);
            float radian = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radian) * swingRadius;
            float y = Mathf.Sin(radian) * swingRadius;

            axe.transform.localPosition = new Vector3(x, y, 0);

            float rotationZ = currentAngle - 90f;
            axe.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);

            yield return null;
        }

        Destroy(axe);
    }
}
