using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health;
    public GameObject deathEffect;
    public GameObject damageEffect;
    public int damage;

    private Vector3 direction;

    public Transform leftRayPos, rightRayPos;

    public LayerMask whatIsGround;

    public LayerMask whatIsLadder;

    [SerializeField] private float movementSpeed;

    public Rigidbody2D rb;

    [SerializeField] private float maxYVelocity;

    private bool ignoreMovementLogic = true;

    [SerializeField] private float logicDelay;

    [SerializeField] private SpriteRenderer enemySprite;

    public bool isClimbing;

    [SerializeField] private float ladderJumpHeight;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(9,9);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(logicDelay > 0)
        {
            logicDelay -= Time.deltaTime;
            if(logicDelay <= 0)
            {
                ignoreMovementLogic = false;
            }
        }
        if(Mathf.Abs(rb.velocity.y) > maxYVelocity)
        {
            if(rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxYVelocity);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, maxYVelocity);
            }
        }
        if(health <= 0)
        {
            GameObject go = Instantiate(deathEffect, transform.position, Quaternion.identity);
            GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        CheckWalls();
        DoMovement();
        
    }

    private void DoMovement()
    {
        if (!isClimbing)
        {
            transform.position += direction * Time.deltaTime * movementSpeed;
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            transform.position += Vector3.up * Time.deltaTime * movementSpeed;
        }
    }

    private void CheckWalls()
    {
        if(direction == Vector3.right)
        {
            if(Physics2D.OverlapCircle(rightRayPos.position, 0.1f, whatIsGround) && !ignoreMovementLogic)
            {
                if (!isClimbing)
                {
                    ChangeDirection();
                }
            }
            else 
            {
                if(Physics2D.OverlapCircle(rightRayPos.position, 0.2f, whatIsLadder))
                {
                    isClimbing = true;
                }
                else
                {
                    if (isClimbing)
                    {
                        isClimbing = false;
                        rb.AddForce(new Vector2(0, ladderJumpHeight));
                    }
                    else
                    {
                        isClimbing = false;
                    }
                }
            }
        }
        if(direction == Vector3.left)
        {
            if (Physics2D.OverlapCircle(leftRayPos.position, 0.1f, whatIsGround) && !ignoreMovementLogic)
            {
                if (!isClimbing)
                {
                    ChangeDirection();
                }
            }
            else 
            {
                if (Physics2D.OverlapCircle(leftRayPos.position, 0.2f, whatIsLadder))
                {
                    isClimbing = true;
                }
                else
                {
                    if (isClimbing)
                    {
                        isClimbing = false;
                        rb.AddForce(new Vector2(0, ladderJumpHeight));
                    }
                    else
                    {
                        isClimbing = false;
                    }
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        GameObject go = Instantiate(damageEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
    }

    public void SetStartDirection(Vector3 _direction)
    {
        direction = _direction;
        enemySprite.flipX = direction.x > 0 ? false : true;
    }

    public void ChangeDirection()
    {
        if(direction == Vector3.right)
        {
            direction = Vector3.left;
        }
        else
        {
            direction = Vector3.right;
        }
        enemySprite.flipX = direction.x > 0 ? false : true;
    }
}
