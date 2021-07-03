using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    Rigidbody2D rb;
    private int damage;

    public GameObject impactEffect;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetUp(float speed, Vector2 dir, int _damage) {
        rb.AddForce(dir * speed);
        damage = _damage;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerController>().health -= damage;
        }
        if (!collision.CompareTag("Enemy")) {
            Instantiate(impactEffect, transform.localPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
