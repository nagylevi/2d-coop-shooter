using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyController : MonoBehaviourPun {

    public SpriteRenderer spriteRenderer;
    public GameObject deathEffect;

    private int health;
    private int damage;
    private float moveSpeed;
    private float attackSpeed;
    private float attackRange;
    private EnemyType enemyType;

    public void SetUpEnemy(Enemy enemy) {
        base.photonView.RPC("SetUpEnemyRPC", RpcTarget.AllBufferedViaServer, ConvertEnemyToObjectArray(enemy));
    }

    [PunRPC]
    void SetUpEnemyRPC(object[] datas) {
        gameObject.name = (string)datas[0];
        health = (int)datas[1];
        damage = (int)datas[2];
        moveSpeed = (float)datas[3];
        attackSpeed = (float)datas[4];
        attackRange = (float)datas[5];
        enemyType = (EnemyType)(int)datas[6];
        spriteRenderer.color = new Color((float)datas[7], (float)datas[8], (float)datas[9]);
    }

    public void TakeDamage(int _damage) {
        health -= _damage;
        if (health <= 0) {
            base.photonView.RPC("DieRPC", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    void DieRPC() {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private object[] ConvertEnemyToObjectArray(Enemy enemy) {
        return new object[] {
            enemy.name,
            enemy.health,
            enemy.damage,
            enemy.moveSpeed,
            enemy.attackSpeed,
            enemy.attackRange,
            (int)enemy.enemyType,
            enemy.enemyColor.r,
            enemy.enemyColor.b,
            enemy.enemyColor.g
        };
    }
}
