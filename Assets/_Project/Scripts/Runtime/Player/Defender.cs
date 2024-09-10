using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class Defender : MonoBehaviour
{
    #region FIELDS

    public CoordStruct Coord;
    public BuildingTile Tile;
    public Transform Target;
    public int MaxHealth = 10;
    public int Health = 10;
    public int cost = 10;
    public MeshRenderer meshRenderer;
    public UnityEngine.Color[] Colours;
    public int UpgradeCost = 30;
    public List<Enemy> EnemyTarget = new List<Enemy>();

    [Header("Attack Settings")]
    public Enemy CurrentlyAttacking;

    private float timeSinceLastAttack = 0f;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
        Colours = new UnityEngine.Color[meshRenderer.materials.Length];
        int i = 0;
        foreach (var mat in meshRenderer.materials)
        {
            Colours[i] = mat.color;
            i++;
        }
    }

    private void OnEnable()
    {
        EventManager.OnWaveCompleted += () =>
        {
            Health = MaxHealth;
        };
        EventManager.OnEnemyDestroyed += (Enemy) =>
        {
            if (CurrentlyAttacking == Enemy)
            {
                EnemyTarget.Remove(Enemy);
            }
        };
    }

    private void OnDisable()
    {
        EventManager.OnWaveCompleted -= () =>
        {
            Health = MaxHealth;
        };
        EventManager.OnEnemyDestroyed -= (Enemy) =>
        {
            if (CurrentlyAttacking == Enemy)
            {
                EnemyTarget.Remove(Enemy);
            }
        };
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        //CurrentlyAttacking = GetNextTarget();
        if (CurrentlyAttacking != null)
        {
            AttackEnemy();
        }
        else
        {
            CurrentlyAttacking = GetNextTarget();
            RotateTowardsAttacker();
        }
    }

    private void Start()
    {
        Health = MaxHealth;
    }

    #endregion UNITY METHODS

    #region METHODS

    public void TakeDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(MaterialChange());
        if (Health <= 0)
        {
            PlayerManager.defenders.Remove(this);
            EventManager.OnDefenderDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }

    private Enemy GetNextTarget()
    {
        Enemy en = null;
        if (EnemyManager.EnemyList.Count > 0)
        {
            //Debug.Log("EnemyList.Count: " + EnemyManager.EnemyList.Count);
            foreach (Enemy enemy in EnemyManager.EnemyList)
            {
                //Debug.Log("Enemy: " + enemy + " : " + CanAttackEnemy(enemy));
                if (CanAttackEnemy(enemy))
                {
                    en = enemy;
                    break;
                }
            }
        }
        else
        {
            return null;
        }
        return en;
    }

    public void OnUpgrade()
    {
        attackDamage *= 3;
        MaxHealth *= 3;
        Health = MaxHealth;
        UpgradeCost *= 3;
    }

    private bool CanAttackEnemy(Enemy enemyTarget)
    {
        bool isInRange = Vector3.Distance(transform.position, enemyTarget.eyeOrigin.position) <= attackRange;
        bool canAttack = timeSinceLastAttack >= attackCooldown;
        //bool hasLineOfSight = !Physics.Linecast(transform.position, enemyTarget.eyeOrigin.position, LayerMask.GetMask("Default"));
        Vector3 directionToTarget = enemyTarget.eyeOrigin.position - transform.position;
        Debug.DrawRay(transform.position, directionToTarget * attackRange, UnityEngine.Color.red, 2f);
        return isInRange && canAttack; //&& hasLineOfSight;
    }

    private void AttackEnemy()
    {
        if (timeSinceLastAttack >= attackCooldown)
        {
            timeSinceLastAttack = 0;
            CurrentlyAttacking.TakeDamage(attackDamage);
        }
    }

    private void RotateTowardsAttacker()
    {
        if (CurrentlyAttacking == null) return;
        if (Vector3.Distance(transform.position, CurrentlyAttacking.eyeOrigin.position) <= attackRange)
        {
            Vector3 directionToTarget = CurrentlyAttacking.eyeOrigin.position - transform.position;
            directionToTarget.y = 0; // Only rotate on the y-axis
            Quaternion rotationToPlayer = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationToPlayer, Time.deltaTime * 1f);
        }
    }

    private IEnumerator MaterialChange()
    {
        var mats = this.gameObject.GetComponentInChildren<MeshRenderer>().materials;
        int i = 0;
        foreach (var mat in mats)
        {
            mat.color = UnityEngine.Color.red;
            i++;
        }
        yield return new WaitForSeconds(0.25f);
        i = 0;
        foreach (var mat in mats)
        {
            mat.color = Colours[i];
            i++;
        }
    }

    #endregion METHODS
}