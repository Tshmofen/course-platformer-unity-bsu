using UnityEngine;
using PathLogger = ThirdParty.QPathFinder.Script.Logger;

public class GameManager : MonoBehaviour
{
    #region Unity call

    private void Start()
    {
        PathLogger.SetLoggingLevel(PathLogger.Level.None);
    }
    
    #endregion
}