using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject SecondaryExplosion;

    #region UNITY METHODS

    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //explode();
        /*if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(1);
        }*/
    }

    #endregion UNITY METHODS

    #region UNITY METHODS

    public void OnTriggerEnter(Collider other)
    {
        //explode();
    }

    public void OnDestroy()
    {
        //explode();
    }

    #endregion UNITY METHODS

    #region METHODS

    public void explode()
    {
        Vector3 ExplosionPoint = this.transform.position;
        Instantiate(Explosion, ExplosionPoint, Quaternion.identity);
        Instantiate(SecondaryExplosion, ExplosionPoint, Quaternion.identity);
        Instantiate(Explosion, ExplosionPoint, Quaternion.identity);
        Instantiate(SecondaryExplosion, ExplosionPoint, Quaternion.identity);
        Instantiate(Explosion, ExplosionPoint, Quaternion.identity);
        Instantiate(SecondaryExplosion, ExplosionPoint, Quaternion.identity);
        Destroy(this);
    }

    #endregion METHODS
}