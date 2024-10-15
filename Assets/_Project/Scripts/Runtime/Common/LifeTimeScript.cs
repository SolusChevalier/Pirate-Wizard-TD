using System.Collections;
using UnityEngine;

namespace InfiniteSkies._Project.Scripts.Runtime.Common
{
    public class LifeTimeScript : MonoBehaviour
    {
        #region FIELDS

        public float lifetime;

        #endregion FIELDS



        #region METHODS

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(lifetime);
            Destroy(gameObject);
        }

        #endregion METHODS
    }
}