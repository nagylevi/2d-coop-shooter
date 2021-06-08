using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour {

    public List<Behaviour> componentsToDisable;

    [Header("Player Settings")]
    public GameObject playerGFX;

    private bool isFacingRight = true;
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