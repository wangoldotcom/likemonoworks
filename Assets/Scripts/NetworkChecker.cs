using UnityEngine;
using UnityEngine.Networking;

public class NetworkChecker : MonoBehaviour
{
    public static bool IsOnline()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
