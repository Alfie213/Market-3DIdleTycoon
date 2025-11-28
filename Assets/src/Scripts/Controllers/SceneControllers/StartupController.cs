using System.Collections;
using UnityEngine;

public class StartupController : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null; 
        SceneLoader.Instance.LoadScene(SceneLoader.SceneType.MainMenuScene);
    }
}