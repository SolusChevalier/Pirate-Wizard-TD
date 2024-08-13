using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfiniteSkies._Project.Scripts.Runtime.Common.Data.GameData.Scripts
{
    public class SettingsSceneNav : MonoBehaviour
    {
        #region METHODS

        public void BackBtnClicked()
        {
            SceneManager.LoadScene(1);
        }

        #endregion METHODS
    }
}