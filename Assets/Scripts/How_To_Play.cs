using UnityEngine;
using UnityEngine.SceneManagement;

public class How_To_Play : MonoBehaviour
{
        [SerializeField] private string sceneName;
        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }
}
