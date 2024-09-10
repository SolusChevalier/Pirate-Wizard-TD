using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region FIELDS

    private NavMeshAgent _navMeshAgent;
    public PlayerManager PlayerManager;
    public EnemyType Type;
    public Vector3 FinalDest;
    public Transform eyeOrigin;
    public int Health = 50;
    public float sightRange = 5f;
    public bool ReachedTower = false;
    public MeshRenderer meshRenderer;
    public Color[] Colours;

    [Header("Attack Settings")]
    public Defender CurrentlyAttacking;

    private float timeSinceLastAttack = 0f;
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
        Colours = new Color[meshRenderer.materials.Length];
        int i = 0;
        foreach (var mat in meshRenderer.materials)
        {
            Colours[i] = mat.color;
            i++;
        }
        EventManager.OnDefenderDestroyed += (defender) =>
        {
            if (CurrentlyAttacking == defender)
            {
                //_navMeshAgent.isStopped = false;
                CurrentlyAttacking = null;
                try
                {
                    _navMeshAgent.SetDestination(FinalDest);
                }
                catch (MissingReferenceException e)
                {
                    Debug.Log("The enemy died and was destroyed before you could grab the agent");
                    throw e;
                }
            }
        };
    }

    private void OnEnable()
    {
        //EventManager.OnGamePaused += stopAgent;
        //EventManager.OnGameResumed += startAgent;
    }

    private void OnDisable()
    {
        //EventManager.OnGamePaused -= stopAgent;
        //EventManager.OnGameResumed -= startAgent;
        EventManager.OnDefenderDestroyed -= (defender) =>
        {
            if (CurrentlyAttacking == defender)
            {
                //_navMeshAgent.isStopped = false;
                CurrentlyAttacking = null;
                _navMeshAgent.SetDestination(FinalDest);
            }
        };
    }

    private void Update()
    {
        if (CurrentlyAttacking == null)
        {
            //Debug.Log("CurrentlyAttacking is null");
            foreach (Defender def in PlayerManager.defenders)
            {
                //Debug.Log(IsTargetInSight(def.Target));
                if (IsTargetInSight(def.Target))
                {
                    CurrentlyAttacking = def;
                    _navMeshAgent.SetDestination(CurrentlyAttacking.Target.position);
                    //def.EnemyTarget.Add(this);
                    return;
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, FinalDest) < 0.1f)
        {
            ReachedTower = true;
            PlayerManager.AttackableEnemyList.Add(this);
            _navMeshAgent.isStopped = true;
        }
        if (ReachedTower)
        {
            timeSinceLastAttack += Time.deltaTime;
            if (CanAttackTower())
            {
                AttackTower();
                timeSinceLastAttack = 0f;
            }
        }

        if (CurrentlyAttacking != null && !ReachedTower)
        {
            timeSinceLastAttack += Time.deltaTime;
            if (CanAttackDefender())
            {
                AttackPlayer();
                timeSinceLastAttack = 0f;
            }
            //StartCoroutine(Attack());
        }
        if (Health <= 0)
        {
            //EventManager.OnEnemyDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        //_navMeshAgent.Move(FinalDest);
    }

    #endregion UNITY METHODS

    #region METHODS

    public void TakeDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(MaterialChange());
        if (Health <= 0)
        {
            //EventManager.OnEnemyDestroyed?.Invoke(this);

            Destroy(gameObject);
        }
    }

    public void EnemyAttack(Defender other)
    {
        CurrentlyAttacking = other.GetComponent<Defender>();
        _navMeshAgent.isStopped = true;
        StartCoroutine(Attack());
    }

    public void MoveTo(Vector3 target)
    {
        _navMeshAgent.SetDestination(FinalDest);
    }

    public void OnDestroy()
    {
        EventManager.OnEnemyDestroyed?.Invoke(this);
    }

    private void AttackPlayer()
    {
        if (CurrentlyAttacking != null)
        {
            CurrentlyAttacking.GetComponent<Defender>().TakeDamage(attackDamage);
        }
    }

    private void AttackTower()
    {
        PlayerManager.TakeDamage(attackDamage);
    }

    public bool IsTargetInSight(Transform Target)
    {
        RaycastHit hit;
        Vector3 directionToTarget = (Target.position - eyeOrigin.position).normalized;

        Debug.DrawRay(eyeOrigin.position, directionToTarget * sightRange, Color.red, 2f);

        if (Physics.Raycast(eyeOrigin.position, directionToTarget, out hit, sightRange))
        {
            return hit.transform.CompareTag("Defender");
        }

        return false;
    }

    private bool CanAttackDefender()
    {
        if (CurrentlyAttacking == null) return false;

        bool isInRange = Vector3.Distance(transform.position, CurrentlyAttacking.Target.position) <= attackRange;
        bool canAttack = timeSinceLastAttack >= attackCooldown;
        //bool hasLineOfSight = !Physics.Linecast(transform.position, CurrentlyAttacking.Target.position, LayerMask.GetMask("Default"));

        return isInRange && canAttack;// && hasLineOfSight;
    }

    private bool CanAttackTower()
    {
        if (CurrentlyAttacking != null) return false;

        //bool isInRange = Vector3.Distance(transform.position, CurrentlyAttacking.Target.position) <= attackRange;
        bool canAttack = timeSinceLastAttack >= attackCooldown;

        return canAttack;
    }

    public IEnumerator Attack()
    {
        while (CurrentlyAttacking != null)
        {
            CurrentlyAttacking.GetComponent<Defender>().TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private IEnumerator MaterialChange()
    {
        var mats = this.gameObject.GetComponentInChildren<MeshRenderer>().materials;
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
}

#endregion METHODS