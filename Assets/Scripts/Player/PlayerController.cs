using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour {

    public List<Behaviour> componentsToDisable;

    public PlayerMovement playerMovement;

    [Header("Player Settings")]
    public GameObject playerGFX;
    public GameObject headAimTarget;
    public Transform weaponHolder;
    public Transform weaponOffset;

    [Header("Animation IK References")]
    public Transform leftArmSolverTarget;
    public Transform rightArmSolverTarget;
    public Transform leftArmGunPos;
    public Transform rightArmGunPos;

    [HideInInspector]
    public bool isFacingRight = true;
    private Vector3 mousePosition;

    private PhotonView view;

    void Start() {
        view = GetComponent<PhotonView>();
        if (!view.IsMine) {
            DisableComponents();
        }
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
        leftArmSolverTarget.position = leftArmGunPos.position;
        rightArmSolverTarget.position = rightArmGunPos.position;
    }

    void CheckPlayerPositionToMousePosition() {
        if (isFacingRight && mousePosition.x < transform.position.x) {
            FlipPlayerGFX();
        } else if (!isFacingRight && mousePosition.x > transform.position.x) {
            FlipPlayerGFX();
        }
    }

    void FlipPlayerGFX() {
        isFacingRight = !isFacingRight;
        playerGFX.transform.Rotate(0f, 180f, 0f);
        weaponOffset.Rotate(180f, 0f, 0f);
    }

    void DisableComponents() {
        foreach(Behaviour component in componentsToDisable) {
            component.enabled = false;
        }
    }

    void RotateGunTowardsMouse() {
        Vector3 difference = mousePosition - weaponHolder.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        weaponHolder.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

}