using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public Vector2Int cords;
    public int moveDistance;
    public float moveSpeed;
    public int health;
    public int maxHealth;
    public int attackDmg;
    public string attackName;
    public string unitName;
    public GameObject unitGameObject;

    public Unit(Vector2Int cords, int moveDistance, float moveSpeed, int health, int maxHealth, int attackDmg, string attackName, string unitName, GameObject unitGameObject)
    {
        this.cords = cords;
        this.moveDistance = moveDistance;
        this.moveSpeed = moveSpeed;
        this.health = health;
        this.maxHealth = maxHealth;
        this.attackDmg = attackDmg;
        this.attackName = attackName;
        this.unitName = unitName;
        this.unitGameObject = unitGameObject;
    }


}
