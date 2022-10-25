using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;

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

    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreLayerCollision(9,11);
    }

    void FixedUpdate()
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


        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

    }

    void Update()
    {
        CheckFlip();
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
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
            isJumping = true;
            _jumpTime = jumpTime;
        }

        if(Input.GetKey(KeyCode.Space) && isJumping)
        {
            print("Hold");
            if(_jumpTime > 0)
            {
                print("isGreater");
                rb.velocity = Vector2.up * jumpForce * jumpForce;
                _jumpTime -= Time.deltaTime;
            }
            else
            {
                print("isSmaler");
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }


        /*else if(Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
        }*/
    }

    public void GetContactDamage()
    {
        print("Got contact damage");
    }

    private void CheckFlip()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        playerSprite.flipX = direction.x > 0 ? false : true;
    }

    private IEnumerator ReactivateCollision(Collider2D _targetCollider)
    {
        yield return new WaitForSecondsRealtime(.2f);
        Physics2D.IgnoreCollision(_targetCollider, playerCollider, false);
    }
}

    
