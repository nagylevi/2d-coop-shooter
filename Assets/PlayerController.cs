using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public List<Behaviour> componentsToDisable;

    [Header("Player Settings")]
    public GameObject playerGFX;

    private bool isFacingRight = true;
    private Vector3 mousePosition;

    void Update() {
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
}