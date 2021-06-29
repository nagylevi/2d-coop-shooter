using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private int health;

    public void SetUpEnemy(Enemy enemy) {
        health = enemy.health;
        gameObject.name = enemy.name;
    }
}
