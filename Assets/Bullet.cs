using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public void Setup(Vector2 shootDir) {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float speed = 100f;
        rb.AddForce(shootDir * speed, ForceMode2D.Impulse);
    }
}
