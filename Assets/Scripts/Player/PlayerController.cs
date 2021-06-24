using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public List<Behaviour> componentsToDisable;

    [Header("Player Settings")]
    public GameObject playerGFX;
    public GameObject headAimTarget;
    public Transform gunHolder;
    public Transform gunOffset;
    public LayerMask targetLayerMask;
    public CinemachineShake cinemachineShake;

    [Header("Animation IK References")]
    public Transform leftArmSolverTarget;
    public Transform rightArmSolverTarget;

    [HideInInspector]
    public bool isFacingRight = true;

    private Vector3 mousePosition;
    private PhotonView view;
    private Gun currentGun;
    private bool shootingInput;

    private const int FIRST_GUN_INDEX = 0;

    void Start() {
        view = GetComponent<PhotonView>();
        SetupPlayer();
    }

    void Update() {
        if (!view.IsMine)
            return;

        // Get mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Handles the gun rotation
        RotateGunTowardsMouse();

        // Hande Player rotation based on mouse positin
        CheckPlayerPositionToMousePosition();

        // Set the headAimTarget position based on mousePosition
        headAimTarget.transform.position = new Vector2(mousePosition.x, mousePosition.y);

        // Handle IK hand animation
        if (currentGun != null) {
            leftArmSolverTarget.position = currentGun.leftHandGrip.position;
            rightArmSolverTarget.position = currentGun.rightHandGrip.position;
        }

        // Handle Player Input
        CheckPlayerInput();

        // Handle Player Shooting
        if (shootingInput && currentGun.IsReadyToShoot()) {
            // This method handles the fireRate locally
            currentGun.Shoot();
            // Camera Shake
            cinemachineShake.ShakeCamera(currentGun.cameraShakeIntensity, currentGun.cameraShakeTime);
            // RPC call
            view.RPC("RPC_Shoot", RpcTarget.All, currentGun.firePoint.position, currentGun.firePoint.right, currentGun.GetComponent<PhotonView>().ViewID);
        }
    }

    void CheckPlayerPositionToMousePosition() {
        if (isFacingRight && mousePosition.x < transform.position.x) {
            FlipPlayerAndGunGFX();
        } else if (!isFacingRight && mousePosition.x > transform.position.x) {
            FlipPlayerAndGunGFX();
        }
    }

    void FlipPlayerAndGunGFX() {
        isFacingRight = !isFacingRight;
        playerGFX.transform.Rotate(0f, 180f, 0f);
        gunOffset.Rotate(180f, 0f, 0f);
    }

    void SetupPlayer() {
        if (!view.IsMine) {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
            foreach (Behaviour component in componentsToDisable) {
                component.enabled = false;
            }
        } else if (view.IsMine) {
            gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
            currentGun = gunOffset.GetChild(FIRST_GUN_INDEX).GetComponent<Gun>();
        }
    }

    void RotateGunTowardsMouse() {
        Vector3 difference = mousePosition - gunHolder.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        gunHolder.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    void CheckPlayerInput() {
        if (currentGun.fireMode == GunFireMode.Automatic) {
            shootingInput = Input.GetMouseButton(0);
        } else if (currentGun.fireMode == GunFireMode.Single) {
            shootingInput = Input.GetMouseButtonDown(0);
        }
    }

    // This RPC call happens on every client when a player shoot
    [PunRPC]
    void RPC_Shoot(Vector3 firerPoint, Vector3 shootDir, int photonViewID) {
        float range = 50f;
        Gun gunRef = PhotonNetwork.GetPhotonView(photonViewID).gameObject.GetComponent<Gun>();
        RaycastHit2D raycastHit2D = Physics2D.Raycast(firerPoint, shootDir, range, targetLayerMask);
        if (raycastHit2D.collider != null) {
            // Instantiate impactParticle
            Instantiate(gunRef.impactParticle, raycastHit2D.point, Quaternion.identity);
            // Draw Line on hit
            StartCoroutine(DrawBulletTracingLine(gunRef.lineRenderer, firerPoint, raycastHit2D.point));
        } else {
            // Draw Line when no hit
            StartCoroutine(DrawBulletTracingLine(gunRef.lineRenderer, firerPoint, shootDir * range));
        }
        // Handle Fire animation
        gunRef.animator.ResetTrigger("FireTrigger");
        gunRef.animator.SetTrigger("FireTrigger");
        // Handle Muzzle Flash
        StartCoroutine(DoMuzzleFlash(gunRef.muzzleFlash));
    }

    IEnumerator DrawBulletTracingLine(LineRenderer renderer, Vector3 startPoint, Vector2 endPoint) {
        renderer.enabled = true;
        renderer.SetPosition(0, startPoint);
        renderer.SetPosition(1, endPoint);
        yield return new WaitForSeconds(0.02f);
        renderer.enabled = false;
    }

    IEnumerator DoMuzzleFlash(SpriteRenderer _muzzleFlash) {
        _muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.02f);
        _muzzleFlash.enabled = false;

    }
}