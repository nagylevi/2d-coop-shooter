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

        CheckPlayerPositionToMousePosition();

        headAimTarget.transform.position = new Vector2(mousePosition.x, mousePosition.y);
    }

    void CheckPlayerPositionToMousePosition() {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (isFacingRight && mousePosition.x < transform.position.x) {
            FlipPlayerGFX();
        } else if (!isFacingRight && mousePosition.x > transform.position.x) {
            FlipPlayerGFX();
        }
    }

    void FlipPlayerGFX() {
        isFacingRight = !isFacingRight;
        playerGFX.transform.Rotate(0f, 180f, 0f);
    }

    void DisableComponents() {
        foreach(Behaviour component in componentsToDisable) {
            component.enabled = false;
        }
    }

}