using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    #region FIELDS

    public static SFXManager Instance;

    public AudioClip buttonClick;
    public AudioClip pause;
    public AudioClip[] UnitDeath;
    public AudioClip[] EnemyDamageSound;
    public AudioClip[] EnemyAttackSound;

    private AudioSource audioSource;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    #endregion UNITY METHODS

    #region METHODS

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void playEnemyDamage()
    {
        int rand = Random.Range(0, EnemyDamageSound.Length);
        audioSource.PlayOneShot(EnemyDamageSound[rand]);
    }

    public void playEnemyAttack()
    {
        int rand = Random.Range(0, EnemyAttackSound.Length);
        audioSource.PlayOneShot(EnemyAttackSound[rand]);
    }

    public void playUnitDeath()
    {
        int rand = Random.Range(0, UnitDeath.Length);
        audioSource.PlayOneShot(UnitDeath[rand]);
    }

    #endregion METHODS
}