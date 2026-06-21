using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Create Enemy Type")]
public class EnemyToSpawn : ScriptableObject
{
    [Header("Enemy Model To Spawn In World")]
    public GameObject EnemyModelRepresentation;

    [Header("List of childs the enemy representation can hold")]
    // this is the one to spawn in battlescene
    public List<GameObject> ChildEnemy;
}
