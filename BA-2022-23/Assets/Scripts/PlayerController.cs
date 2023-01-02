using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int maxHealth;

    public int health;

    public float speed;
    public float jumpForce;
    private float moveInput;

    [HideInInspector] public Rigidbody2D rb;

    private bool isGrounded;
    private bool isOnSemiPlatform;
    public Transform groundCheck;
    public float checkRadius;
    public float checkRadiusForSemiGround;
    public LayerMask whatIsGround;
    public LayerMask whatIsSemiGround;

    private int extraJumps;
    public int extraJumpsValue;

    [SerializeField] private SpriteRenderer playerSprite;

    [SerializeField] private Collider2D playerCollider;

    private Collider2D currentSemiCollider;

    public float jumpTime;
    private float _jumpTime;

    private bool isJumping;


    private float vertical;
    public float ladderSpeed;
    private bool isLadder;
    private bool isClimbing;

    private float startGravity;

    public float StartGravity
    {
        get
        {
            return startGravity;
        }
    }

    [SerializeField] private float maxYVelocity;
    [SerializeField] private float maxXVelocity;

    [Range(0f,1f)][SerializeField] private float xDrag;
    [Range(0f,1f)][SerializeField] private float yDrag;

    private bool knockbackReceived;

    public float enemyKnockbackDuration;
    public float weaponKnockbackDuration;

    public GameObject reloadIndicator;

    public Image reloadFillImage;

    private bool canMove = true;

    private int currentCoins;

    public int CurrentCoins
    {
        get
        {
            return currentCoins;
        }
        set
        {
            currentCoins = value;
        }
    }

    [SerializeField] private GameObject playerDamageEffect;
    [SerializeField] private GameObject playerJumpEffect;
    [SerializeField] private GameObject playerWalkEffect;

    private IInteractable closestInteractable;

    [SerializeField] private GameObject interactIndicator;

    [HideInInspector] public List<Weapon> currentSelectedWeapons;

    public Weapon currentSelectedWeapon;

    [SerializeField] private Animator anim;

    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreLayerCollision(9,11);
        startGravity = rb.gravityScale;
    }

    void FixedUpdate()
    {

        if (canMove)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            isOnSemiPlatform = Physics2D.OverlapCircle(groundCheck.position, checkRadiusForSemiGround, whatIsSemiGround);
            if (isOnSemiPlatform)
            {
                if(currentSemiCollider == null)
                {
                    currentSemiCollider = Physics2D.OverlapCircle(groundCheck.position, checkRadiusForSemiGround, whatIsSemiGround);
                }
            }
            else
            {
                currentSemiCollider = null;
            }

            if (!knockbackReceived)
            {
                moveInput = Input.GetAxisRaw("Horizontal");
                rb.AddForce(new Vector2(moveInput * speed, rb.velocity.y));
            }

            if(moveInput != 0)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }

            if (rb.velocity.y > 0)
            {
                anim.SetBool("isJumping", true);
                anim.SetBool("isFalling", false);
            }
            else if(rb.velocity.y < 0)
            {
                anim.SetBool("isFalling", true);
                anim.SetBool("isJumping", false);
            }
            else if(rb.velocity.y == 0)
            {
                anim.SetBool("isFalling", false);
                anim.SetBool("isJumping", false);
            }



            //Apply maximum y velocity 
            if (rb.velocity.y > maxYVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxYVelocity);
            }
            if (rb.velocity.y < -maxYVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxYVelocity);
            }


            //Apply maximum x velocity 
            if (rb.velocity.x > maxXVelocity)
            {
                rb.velocity = new Vector2(maxXVelocity, rb.velocity.y);
            }
            if (rb.velocity.x < -maxXVelocity)
            {
                rb.velocity = new Vector2(-maxXVelocity, rb.velocity.y);
            }



            //Apply drag in x direction when no knockback
            if (!knockbackReceived)
            {
                rb.velocity = new Vector2(rb.velocity.x * xDrag, rb.velocity.y);
            }

            //Apply drag in y direction
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yDrag);


            if (isClimbing)
            {
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(rb.velocity.x, vertical * ladderSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rb.gravityScale = startGravity;
            }

            if(Input.GetKey(KeyCode.Space) && isJumping)
            {
                if(_jumpTime > 0)
                {
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    _jumpTime -= Time.fixedDeltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }
        }
    }

    void Update()
    {
        if (GameManager.instance.paused) return;
        if (!canMove) return;
        CheckFlip();
        SearchForClosestInteractable();
        CheckForInteraction();
        if(isGrounded == true)
        {
            extraJumps = extraJumpsValue;
            if (isOnSemiPlatform)
            {
                if (Input.GetKeyDown(KeyCode.S) && currentSemiCollider != null)
                {
                    Physics2D.IgnoreCollision(currentSemiCollider, playerCollider, true);
                    StartCoroutine(ReactivateCollision(currentSemiCollider));
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0,jumpForce), ForceMode2D.Impulse);
            extraJumps--;
            isJumping = true;
            _jumpTime = jumpTime;
            SoundManager.instance.PlayJumpChickenSound();
            GameObject newParticle = Instantiate(playerJumpEffect, transform.position, Quaternion.identity);
            GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 5));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(closestInteractable != null)
            {
                closestInteractable.Interact();
            }
        }


        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        vertical = Input.GetAxis("Vertical");

        if(isLadder && Input.GetKey(KeyCode.W) && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }

    private void CheckFlip()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        playerSprite.flipX = direction.x > 0 ? false : true;
    }

    public void GetKnockback(Vector3 _direction, float _intensity, float _knockbackDuration)
    {
        rb.AddForce(_direction * _intensity, ForceMode2D.Impulse);
        knockbackReceived = true;
        StartCoroutine(DelayedKnockbackDeactivation(_knockbackDuration));
    }

    private void SearchForClosestInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1);
        bool foundInteractable = false;
        foreach(var r in hits)
        {
            if(closestInteractable == null)
            {
                IInteractable i = r.GetComponent<IInteractable>();
                if(i != null)
                {
                    closestInteractable = i;
                    foundInteractable = true;
                }
                
            }
            else
            {
                IInteractable i = r.GetComponent<IInteractable>();
                if(i != null && i != closestInteractable)
                {
                    closestInteractable = i;
                    foundInteractable = true;
                }
                else if(i == closestInteractable)
                {
                    foundInteractable = true;
                }
            }
        }
        if (!foundInteractable)
        {
            closestInteractable = null;
        }
    }

    private void CheckForInteraction()
    {
        if(closestInteractable != null)
        {
            if (closestInteractable.PlayerInTrigger && closestInteractable.CanInteract)
            {
                interactIndicator.SetActive(true);
            }
            else
            {
                interactIndicator.SetActive(false);
            }
        }
        else
        {
            interactIndicator.SetActive(false);
        }
    }

    public void GetDamage(int _amount, Vector3 _direction, float _intensity, float _knockbackDuration)
    {
        health -= _amount;
        GameManager.instance.UpdateHealthBars();
        GetKnockback(_direction, _intensity, _knockbackDuration);
        if (GameManager.instance.inShop)
        {
            GameManager.instance.shopCam.GetComponent<CameraShake>().DoCameraShake();
        }
        else
        {
            GameManager.instance.mainCam.GetComponent<CameraShake>().DoCameraShake();
        }
        GameObject newParticle = Instantiate(playerDamageEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 4));
        SoundManager.instance.PlayHitChickenSound();
        if (health <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("gotDamage");
        }
    }

    public void ToggleMovement()
    {
        canMove = !canMove;
    }

    public void Heal(int _amount)
    {
        if (health + _amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += _amount;
        }
        GameManager.instance.UpdateHealthBars();
    }

    public void IncreaseCoins(int _amount)
    {
        currentCoins += _amount;
    }

    private void Die()
    {
        anim.SetTrigger("isDead");
        GameManager.instance.KillPlayer();
    }

    private IEnumerator ReactivateCollision(Collider2D _targetCollider)
    {
        yield return new WaitForSecondsRealtime(.2f);
        Physics2D.IgnoreCollision(_targetCollider, playerCollider, false);
    }

    private IEnumerator DelayedKnockbackDeactivation(float _duration)
    {
        yield return new WaitForSecondsRealtime(_duration);
        knockbackReceived = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}