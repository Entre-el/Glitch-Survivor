 using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy; 
    PlayerMovement player;
    Transform playerTransform;
    Vector2 knockbackVelocity;
    float knockbackDuration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            enemy = GetComponent<EnemyStats>();
            player = GameObject.FindAnyObjectByType<PlayerMovement>();
            playerTransform = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else if(player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, enemy.currentMoveSpeed * Time.deltaTime); 
            if(transform.position.x > playerTransform.position.x + 0.2f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = -Mathf.Abs(currentScale.x);
                transform.localScale = currentScale;
            }
            else if(transform.position.x < playerTransform.position.x - 0.2f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x);
                transform.localScale = currentScale;
            }
        }
    }
    public void Knockback(Vector2 velocity,float duration)
    {
        if(knockbackDuration > 0)return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
