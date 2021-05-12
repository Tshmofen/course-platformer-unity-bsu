using UnityEngine;
using PathLogger = ThirdParty.QPathFinder.Script.Logger;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        PathLogger.SetLoggingLevel(PathLogger.Level.None);
    }
}