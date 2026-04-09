using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] private string nextScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Invoke(nameof(LoadScene), 0.5f);
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
