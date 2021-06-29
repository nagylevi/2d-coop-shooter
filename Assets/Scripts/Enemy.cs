using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Enemy", menuName = "ScriptableObjects/Enemy")]
public class Enemy : ScriptableObject {

    public string enemyName;

    public int health;
    public int damage;

    public float moveSpeed;
    public float attackSpeed;
    public float attackRange;
}
