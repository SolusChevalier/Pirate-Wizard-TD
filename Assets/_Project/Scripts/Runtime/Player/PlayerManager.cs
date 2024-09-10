using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region FIELDS

    public int MaxTowerHealth;
    public int TowerHealth;
    public int Money;
    public static Transform TowerCenter;
    public static BuildingTile selectedTile;
    public GameObject DefenderPrefab;
    public static List<Defender> defenders = new List<Defender>();
    private bool EvenOdd => ((MapRepresentation.Width % 2) == 0);
    private int EvenOddMultiplier => EvenOdd ? 1 : 2;

    [Header("Attack Settings")]
    public Enemy CurrentlyAttacking;

    public static List<Enemy> AttackableEnemyList = new List<Enemy>();
    private float timeSinceLastAttack = 0f;

    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    #endregion FIELDS

    #region UNITY METHODS

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
        }
    }

    private void Start()
    {
        Money = 100;
        updateMoneyUI();
        TowerHealth = MaxTowerHealth;
        attackRange *= EvenOddMultiplier;
    }

    private void OnEnable()
    {
        EventManager.AddMoney += AddMoney;
        EventManager.RemoveMoney += RemoveMoney;
        EventManager.UpdateMoneyUI += updateMoneyUI;
        EventManager.OnWaveCompleted += WaveEnd;
        EventManager.OnTileSelected += (tile) => { selectedTile = tile; };
        EventManager.OnTileDeselect += () => { selectedTile = null; };
        EventManager.BuyDefender += BuyDefender;
        EventManager.SellDefender += SellDefender;
        EventManager.UpgradeDefender += UpgradeDefender;
    }

    private void OnDisable()
    {
        EventManager.AddMoney -= AddMoney;
        EventManager.RemoveMoney -= RemoveMoney;
        EventManager.UpdateMoneyUI -= updateMoneyUI;
        EventManager.OnWaveCompleted -= WaveEnd;
        EventManager.OnTileSelected -= (tile) => { selectedTile = tile; };
        EventManager.OnTileDeselect -= () => { selectedTile = null; };
        EventManager.BuyDefender -= BuyDefender;
        EventManager.SellDefender -= SellDefender;
        EventManager.UpgradeDefender -= UpgradeDefender;
    }

    #endregion UNITY METHODS

    #region HEALTH

    public void TakeDamage(int damage)
    {
        TowerHealth -= damage;
        if (TowerHealth <= 0)
        {
            EventManager.OnGameEnded?.Invoke();
        }
    }

    #endregion HEALTH

    #region METHODS

    private bool CanAttackEnemy(Enemy enemyTarget)
    {
        //bool isInRange = Vector3.Distance(TowerCenter.position, enemyTarget.eyeOrigin.position) <= attackRange;
        bool canAttack = timeSinceLastAttack >= attackCooldown;
        //bool hasLineOfSight = !Physics.Linecast(transform.position, enemyTarget.eyeOrigin.position, LayerMask.GetMask("Default"));
        //Vector3 directionToTarget = enemyTarget.eyeOrigin.position - TowerCenter.position;
        //Debug.DrawRay(TowerCenter.position, directionToTarget * attackRange, Color.red, 2f);
        return canAttack; //&& hasLineOfSight;
    }

    private void AttackEnemy()
    {
        if (timeSinceLastAttack >= attackCooldown)
        {
            timeSinceLastAttack = 0;
            CurrentlyAttacking.TakeDamage(attackDamage);
        }
    }

    private Enemy GetNextTarget()
    {
        Enemy en = null;
        if (AttackableEnemyList.Count > 0)
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

    public void BuyDefender(BuildingTile tile)
    {
        if (tile.properties.Occupied) return;
        if (Money >= DefenderPrefab.GetComponent<Defender>().cost)
        {
            RemoveMoney(DefenderPrefab.GetComponent<Defender>().cost);
            tile.instantiateUnit(DefenderPrefab);
        }
    }

    public void SellDefender(BuildingTile tile)
    {
        if (!tile.properties.Occupied) return;
        AddMoney(DefenderPrefab.GetComponent<Defender>().cost / 2);
        tile.SellDefender();
    }

    public void UpgradeDefender(BuildingTile tile)
    {
        if (!tile.properties.Occupied) return;
        if (Money >= DefenderPrefab.GetComponent<Defender>().UpgradeCost)
        {
            RemoveMoney(DefenderPrefab.GetComponent<Defender>().UpgradeCost);
            tile.UpgradeDefender();
        }
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        updateMoneyUI();
    }

    public void RemoveMoney(int amount)
    {
        Money -= amount;
        updateMoneyUI();
    }

    public void updateMoneyUI()
    {
        EventManager.SetMoneyUI?.Invoke(Money);
    }

    public void WaveEnd()
    {
        //Debug.Log("Wave End: Added " + Mathf.Round(Money / 100.0f) + " as Interest");
        AddMoney((int)Mathf.Round(Money / 100.0f));
    }

    #endregion METHODS
}