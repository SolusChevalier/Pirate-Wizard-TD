using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region FIELDS

    public int MaxTowerHealth;
    public int TowerHealth;
    public int Money;
    public int UpgradeCost = 50;
    public static Transform TowerCenter;
    public static BuildingTile selectedTile;
    public GameObject GolemDefenderPrefab, GunDefenderPrefab, CanonDefenderPrefab;
    public static GameObject TowerObject;
    public static GameObject Upgrade1, Upgrade2;
    public Sprite IconUpgrade1, IconUpgrade2;
    public static Sprite Icon1, Icon2;
    public bool Upgraded = false;
    public static List<Defender> defenders = new List<Defender>();
    private bool EvenOdd => ((MapRepresentation.Width % 2) == 0);
    private int EvenOddMultiplier => EvenOdd ? 1 : 2;
    public MeshRenderer meshRenderer;
    public Color[] Colours;

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            if (hit.collider.gameObject.tag == "Tower")
            {
                TowerCenter = hit.collider.transform;
            }
        }
    }

    private void Start()
    {
        Money = 50;
        updateMoneyUI();
        TowerHealth = MaxTowerHealth;
        attackRange *= EvenOddMultiplier;
        Icon1 = IconUpgrade1;
        Icon2 = IconUpgrade2;
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
        EventManager.OnMapGenerated += GetColours;
        EventManager.UpgradeTower += UpgradeTower;
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
        EventManager.OnMapGenerated -= GetColours;
        EventManager.UpgradeTower -= UpgradeTower;
    }

    #endregion UNITY METHODS

    #region HEALTH

    public void TakeDamage(int damage)
    {
        TowerHealth -= damage;
        StartCoroutine(MaterialChange());
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

    public void GetColours()
    {
        meshRenderer = TowerObject.GetComponentInChildren<MeshRenderer>();
        Colours = new UnityEngine.Color[meshRenderer.materials.Length];
        int i = 0;
        foreach (var mat in meshRenderer.materials)
        {
            Colours[i] = mat.color;
            i++;
        }
    }

    public void BuyDefender(BuildingTile tile, int DefenderType)
    {
        if (tile.properties.Occupied) return;

        switch (DefenderType)
        {
            case 0:
                if (Money >= GolemDefenderPrefab.GetComponent<Defender>().cost)
                {
                    RemoveMoney(GolemDefenderPrefab.GetComponent<Defender>().cost);
                    tile.instantiateUnit(GolemDefenderPrefab);
                }
                break;

            case 1:
                if (Money >= GunDefenderPrefab.GetComponent<Defender>().cost)
                {
                    RemoveMoney(GunDefenderPrefab.GetComponent<Defender>().cost);
                    tile.instantiateUnit(GunDefenderPrefab);
                }
                break;

            case 2:
                if (Money >= CanonDefenderPrefab.GetComponent<Defender>().cost)
                {
                    RemoveMoney(CanonDefenderPrefab.GetComponent<Defender>().cost);
                    tile.instantiateUnit(CanonDefenderPrefab);
                }
                break;
        }
        /*if (Money >= GolemDefenderPrefab.GetComponent<Defender>().cost)
        {
            RemoveMoney(GolemDefenderPrefab.GetComponent<Defender>().cost);
            tile.instantiateUnit(GolemDefenderPrefab);
        }*/
    }

    public void SellDefender(BuildingTile tile)
    {
        if (!tile.properties.Occupied) return;

        AddMoney(tile.properties.OccupyingUnit.GetComponent<Defender>().cost / 2);
        tile.SellDefender();
    }

    public void UpgradeTower(int num)
    {
        if (Money >= UpgradeCost)
        {
            RemoveMoney(UpgradeCost);

            //TowerObject.SetActive(false);
            if (num == 1)
            {
                Upgrade1.SetActive(true);
                attackDamage *= 3;
                TowerHealth = MaxTowerHealth;
                UpgradeCost *= 2;
            }
            else if (num == 2)
            {
                Upgrade2.SetActive(true);
                MaxTowerHealth *= 5;
                TowerHealth = MaxTowerHealth;
                UpgradeCost *= 2;
            }
        }
    }

    public void UpgradeDefender(BuildingTile tile, int num)
    {
        if (!tile.properties.Occupied) return;
        if (Money >= GolemDefenderPrefab.GetComponent<Defender>().UpgradeCost)
        {
            RemoveMoney(GolemDefenderPrefab.GetComponent<Defender>().UpgradeCost);
            tile.UpgradeDefender(num);
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
        CurrentlyAttacking = null;
        AddMoney((int)Mathf.Round(Money / 100.0f));
    }

    private IEnumerator MaterialChange()
    {
        var mats = meshRenderer.materials;
        int i = 0;
        foreach (var mat in mats)
        {
            mat.color = Color.red;
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