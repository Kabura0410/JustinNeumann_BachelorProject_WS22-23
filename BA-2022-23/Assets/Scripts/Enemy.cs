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

    [SerializeField] private float attackSpeed;
    private float _attackSpeed;

    private bool doMove = true;

    [SerializeField] private float knockbackDuration;

    [SerializeField] private float knockbackIntensity;

    [Range(0f, 1f)] [SerializeField] private float xDrag;


    [Header("Groundcheck")]
    public bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;

    public enum FocusType
    {
        none,
        player,
        crystal
    }

    public FocusType focusType;

    [SerializeField] private EnemyWeapon weapon;

    [SerializeField] private LayerMask raycastLayerForSight;

    public enum EnemyType
    {
        melee,
        ranged
    }

    public EnemyType type;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(9,9);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //funcion for ignoring the first obstacle
        if(logicDelay > 0)
        {
            logicDelay -= Time.deltaTime;
            if(logicDelay <= 0)
            {
                ignoreMovementLogic = false;
            }
        }

        //function for caping the velocity in y direction
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

        //handle the enemy health and kill if health <= 0
        if(health <= 0)
        {
            GameObject go = Instantiate(deathEffect, transform.position, Quaternion.identity);
            GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
            GameManager.instance.allSpawnedEnemies.Remove(this);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        CheckWalls();
        DoMovement();
        DoTimer();
        switch (type)
        {
            case EnemyType.melee:
                if (!doMove)
                {
                    DoAttack();
                }
                break;
            case EnemyType.ranged:
                CheckSight();
                break;
        }

        //handel the drag in x direction
        rb.velocity = new Vector2(rb.velocity.x * xDrag, rb.velocity.y);

    }

    private void DoMovement()
    {
        if (doMove)
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
    }

    private void DoTimer()
    {
        if(_attackSpeed > 0)
        {
            _attackSpeed -= Time.fixedDeltaTime;
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

    public void TakeDamage(int damage, Vector3 _direction, float _intensity = 0f)
    {
        health -= damage;
        GameObject go = Instantiate(damageEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
        GetKnockback(_direction, _intensity);
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

    public void ChangeFocus(FocusType _focusType)
    {
        focusType = _focusType;
    }

    public void DoAttack()
    {
        if(_attackSpeed <= 0)
        {
            switch (focusType)
            {
                case FocusType.none:
                    break;
                case FocusType.player:
                    float x = (GameManager.instance.player.transform.position - transform.position).normalized.x;
                    if(x > 0)
                    {
                        GameManager.instance.player.rb.velocity = Vector3.zero;
                        GameManager.instance.player.GetDamage(damage,new Vector3(1, .8f, 0), knockbackIntensity, GameManager.instance.player.enemyKnockbackDuration);
                    }
                    else
                    {
                        GameManager.instance.player.rb.velocity = Vector3.zero;
                        GameManager.instance.player.GetDamage(damage, new Vector3(-1, .8f, 0), knockbackIntensity, GameManager.instance.player.enemyKnockbackDuration);
                    }
                    GameManager.instance.UpdateHealthBars();
                    break;
                case FocusType.crystal:
                    GameManager.instance.crystal.GetDamage(damage);
                    GameManager.instance.UpdateHealthBars();
                    break;
            }
            _attackSpeed = attackSpeed;
        }
    }

    public void GetKnockback(Vector3 _direction, float _intensity)
    {
        rb.AddForce(_direction * _intensity, ForceMode2D.Impulse);
    }

    public void ToggleMovement()
    {
        doMove = !doMove;
    }

    private void CheckSight()
    {
        float playerDistance = Vector2.Distance(transform.position, GameManager.instance.player.transform.position);
        float crystalDistance = Vector2.Distance(transform.position, GameManager.instance.crystal.transform.position);
        if(playerDistance < crystalDistance)
        {
            RaycastHit2D hitInfo = Physics2D.CircleCast(transform.position, .5f,GameManager.instance.player.transform.position - transform.position, playerDistance, raycastLayerForSight);
            if (hitInfo.collider.CompareTag("Player"))
            {
                weapon.targetObject = GameManager.instance.player.gameObject;
                doMove = false;
            }
            else
            {
                weapon.targetObject = null;
                doMove = true;
                Vector3 difference = GameManager.instance.player.transform.position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + weapon.offset);
            }
        }
        else
        {
            RaycastHit2D hitInfo = Physics2D.CircleCast(transform.position, .5f,GameManager.instance.crystal.transform.position - transform.position, crystalDistance, raycastLayerForSight);
            if (hitInfo.collider.CompareTag("Crystal"))
            {
                weapon.targetObject = GameManager.instance.crystal.gameObject;
                doMove = false;
            }
            else
            {
                weapon.targetObject = null;
                doMove = true;
                Vector3 difference = GameManager.instance.crystal.transform.position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + weapon.offset);
            }
        }
    }

    public IEnumerator ToggleMovementDelayed(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        ToggleMovement();
    }
}
