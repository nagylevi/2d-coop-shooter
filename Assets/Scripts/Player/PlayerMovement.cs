using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour {

    [Header("Basic Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;
    public int extraJumps = 1;

    [Header("Ground Check Settings")]
    public float groundCheckRadiusMultiplyer = 0.28f;
    public float groundCheckOffsetY = 0.1f;
    public LayerMask platformLayerMask;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2D;

    private int availableJumps;
    private float movementX;
    private float groundCheckRadius;
    private bool isGrounded;

    private PhotonView view;

    void Start() {
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        availableJumps = extraJumps;
    }

    void Update() {

        if (!view.IsMine)
            return;

        movementX = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && (isGrounded || CanDoubleJump())) {
            Jump();
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) {
            AdjustJump();
        }

        if (isGrounded) {
            availableJumps = extraJumps;
        }
    }

    void FixedUpdate() {
        isGrounded = IsGrounded();
        MoveCharacter();
    }

    void MoveCharacter() {
        rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
    }

    void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        availableJumps--;
    }

    void AdjustJump() { // Perform short jump
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }

    bool IsGrounded() {
        groundCheckRadius = capsuleCollider2D.bounds.extents.y * groundCheckRadiusMultiplyer;
        RaycastHit2D raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - capsuleCollider2D.bounds.extents.y + groundCheckOffsetY), groundCheckRadius, Vector2.zero, 0f, platformLayerMask);
        return raycastHit.collider != null;
    }

    bool CanDoubleJump() {
        return availableJumps > 0;
    }

    // ----- G R O U N D   C H E C K   D E B U G -----
    /*private void OnDrawGizmos() {
        if (!isGrounded) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y - capsuleCollider2D.bounds.extents.y + groundCheckOffsetY), groundCheckRadius);
    }*/
}
