using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy; 
    Transform player;

    public float moveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, enemy.currentMoveSpeed * Time.deltaTime); 
    }
}
