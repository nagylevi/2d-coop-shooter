using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Pathfinding;

public class EnemyController : MonoBehaviourPun {

    [Header("References")]
    public GameObject enemyGFX;

    [Header("AI")]
    public float nextWaypointDistance = 3f;
    public float jumpForce = 15f;
    public LayerMask targetLayerMask;

    [Header("Ground Check")]
    public float groundCheckRadiusMultiplyer = 0.28f;
    public float groundCheckOffsetY = 0f;
    public LayerMask platformLayerMask;
    private float groundCheckRadius;
    private bool isGrounded;

    // Enemy
    private int health;
    private int damage;
    private float moveSpeed;
    private float attackSpeed;
    private float attackRange;
    private EnemyType enemyType;
    private GameObject deathEffect;
    private GameObject projectile;

    private bool isReadyToShoot = true;

    private GameData gameData;

    // private AI
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2D;
    private Seeker seeker;
    private Path path;
    private Transform target;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    void Awake() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        gameData = GameObject.Find("_GameData").GetComponent<GameData>();
    }

    void Start() {
        InvokeRepeating(nameof(UpdatePath), 0f, .5f);
    }

    void Update() {
        isGrounded = IsGrounded();
        target = FindClosestTarget();
    }

    void UpdatePath() {
        if (seeker.IsDone() && target != null) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void FixedUpdate() {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        } else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * moveSpeed * Time.deltaTime;

        // Movement
        if (Vector2.Distance(rb.position, target.position) > attackRange || !CheckTargetIsInSight()) {
            rb.velocity = new Vector2(force.x, rb.velocity.y);

            // Jump
            if (target.position.y > (transform.position.y + 0.5f) && isGrounded) {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        } else {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (isReadyToShoot) {
                base.photonView.RPC("EnemyShootRPC", RpcTarget.AllViaServer, (Vector2)(target.position - transform.position).normalized);
                isReadyToShoot = false;
                Invoke(nameof(ResetShoot), attackSpeed);
            }
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        // Flip enemyGFX based on velocity
        if (force.x >= 0.01f) {
            base.photonView.RPC("FlipEnemyGFX_RPC", RpcTarget.AllViaServer, new Vector3(1f, 1f, 1f));
        } else if (force.x <= 0.01f) {
            base.photonView.RPC("FlipEnemyGFX_RPC", RpcTarget.AllViaServer, new Vector3(-1f, 1f, 1f));
        }
    }

    public void SetUpEnemy(int enemyID) {
        base.photonView.RPC("SetUpEnemyRPC", RpcTarget.AllBufferedViaServer, enemyID);
    }

    public void TakeDamage(int _damage) {
        health -= _damage;
        if (health <= 0) {
            base.photonView.RPC("DieRPC", RpcTarget.AllViaServer);
        }
    }

    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    bool IsGrounded() {
        groundCheckRadius = capsuleCollider2D.bounds.extents.y * groundCheckRadiusMultiplyer;
        RaycastHit2D raycastHit = Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - capsuleCollider2D.bounds.extents.y + groundCheckOffsetY), groundCheckRadius, Vector2.zero, 0f, platformLayerMask);
        return raycastHit.collider != null;
    }

    bool CheckTargetIsInSight() {
        bool targetHit = false;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, 100f, targetLayerMask);
        if (raycastHit2D.collider != null) {
            targetHit = raycastHit2D.collider.CompareTag("Player");
        }
        return targetHit;
    }

    void ResetShoot() {
        isReadyToShoot = true;
    }

    Transform FindClosestTarget() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestTarget = players[0].transform;
        for (int i = 0; i < players.Length; i++) {
            if (Vector2.Distance(transform.position, players[i].transform.position) < Vector2.Distance(transform.position, closestTarget.position)) {
                closestTarget = players[i].transform;
            }
        }
        return closestTarget;
    }

    [PunRPC]
    void EnemyShootRPC(Vector2 dir) {
        GameObject projectileRef = Instantiate(projectile, transform.position, Quaternion.identity);
        projectileRef.GetComponent<Projectile>().SetUp(400f, dir, damage);
    }

    [PunRPC]
    void SetUpEnemyRPC(int enemyID) {
        Enemy enemy = gameData.enemies[enemyID];

        gameObject.name = enemy.enemyName;
        health = enemy.health;
        damage = enemy.damage;
        moveSpeed = enemy.moveSpeed;
        attackSpeed = enemy.attackSpeed;
        attackRange = enemy.attackRange;
        enemyType = enemy.enemyType;
        enemyGFX.GetComponent<SpriteRenderer>().color = enemy.enemyColor;
        deathEffect = enemy.deathEffect;
        projectile = enemy.projectile;
    }

    [PunRPC]
    void DieRPC() {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    [PunRPC]
    void FlipEnemyGFX_RPC(Vector3 scale) {
        enemyGFX.transform.localScale = scale;
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
