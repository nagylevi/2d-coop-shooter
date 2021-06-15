using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour {

    public List<Behaviour> componentsToDisable;

    [Header("Player Settings")]
    public GameObject playerGFX;
    public GameObject headAimTarget;
    public Transform weaponHolder;
    public Transform weaponOffset;
    public LayerMask targetLayerMask;

    [Header("Animation IK References")]
    public Transform leftArmSolverTarget;
    public Transform rightArmSolverTarget;

    [HideInInspector]
    public bool isFacingRight = true;

    private Vector3 mousePosition;
    private PhotonView view;
    private Gun gun;

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

        // Handles the weapon rotation
        RotateGunTowardsMouse();

        // Hande Player rotation based on mouse positin
        CheckPlayerPositionToMousePosition();

        // Set the headAimTarget position based on mousePosition
        headAimTarget.transform.position = new Vector2(mousePosition.x, mousePosition.y);

        // Handle IK hand animation
        if (gun != null) {
            leftArmSolverTarget.position = gun.leftHandGrip.position;
            rightArmSolverTarget.position = gun.rightHandGrip.position;
        }

        // Handle Player Shooting
        if (Input.GetMouseButton(0)) {
            Shoot();
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
        weaponOffset.Rotate(180f, 0f, 0f);
    }

    void SetupPlayer() {
        if (!view.IsMine) {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
            foreach (Behaviour component in componentsToDisable) {
                component.enabled = false;
            }
        } else if (view.IsMine) {
            gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
            gun = weaponOffset.GetChild(FIRST_GUN_INDEX).GetComponent<Gun>();
        }
    }

    void RotateGunTowardsMouse() {
        Vector3 difference = mousePosition - weaponHolder.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        weaponHolder.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    void Shoot() {
        Vector2 shootDir = (mousePosition - gun.firePoint.position).normalized;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(gun.firePoint.position, shootDir, 100f, targetLayerMask);
        if (raycastHit2D.collider != null) {
            // Hit
            Debug.Log("We hit something");
        }
    }

}