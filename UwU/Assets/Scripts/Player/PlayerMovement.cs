using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    public float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;
    public float direction = 1;

    private bool isJumping;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    public float checkRadius = 0.2f; // The radius of the circle to detect if the player is near a fall-through area
    public LayerMask fallLayerMask; // The layer mask to only detect collisions with fall-through areas

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;

    private Tilemap map;

    void Update()
    {
        

        //dash
        if (isDashing)
        {
            return;
        }
        //coyoteTime and Jump buffer
        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        //jump
        if (IsGrounded() && jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpBufferCounter = 0f;

            StartCoroutine(JumpCooldown());

        }

        /*if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }*/

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKey(KeyCode.S))
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerFallThrough");
        }
        if(!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.Space))
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        if (Input.GetKey(KeyCode.Space))
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerFallThrough");
        }
        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        float distance = 0.5f;
        Vector2 dir = new Vector2(0, -1);
        int layerMask = 1;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, dir, distance, layerMask);
        Debug.DrawRay(transform.position, dir * distance);
        if (hitInfo.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            direction *= -1;
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(.6f);
        isJumping = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    /*public bool canFallThrough(Vector2 playerPos)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        bool canFallThrough = dataFromTiles
    }*/
}
