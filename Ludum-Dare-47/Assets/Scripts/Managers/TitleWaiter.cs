using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleWaiter : MonoBehaviour
{
    [SerializeField]
    protected Utilities.SceneField nextScene;

    protected bool gone = false;

    public void ClickGo()
    {
        if (gone) return;
        gone = true;
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
