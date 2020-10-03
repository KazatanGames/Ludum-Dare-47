using UnityEngine;

[CreateAssetMenu(fileName = "AppConfig", menuName = "Initialisation Config", order = 99999)]
public class AppConfigSO : ScriptableObject
{
    public Utilities.SceneField initialScene;
    public bool debugMode;
    public bool skipIntros;
}
