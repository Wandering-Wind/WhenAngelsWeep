using UnityEngine;
using UnityEngine.SceneManagement;

public class How_To_Play : MonoBehaviour
{
    public class SimpleSceneLoader : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
