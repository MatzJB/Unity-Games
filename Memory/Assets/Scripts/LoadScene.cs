using UnityEngine;

public class LoadScene : MonoBehaviour
{
   public void Load(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
