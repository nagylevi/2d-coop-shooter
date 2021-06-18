using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    float speed;
    Vector2 shootDir;

    void Update() {
        transform.Translate(shootDir * speed * Time.deltaTime);
    }

    public void Setup(Vector2 shootDir) {
        speed = 100f;
        this.shootDir = shootDir;
    }
}
