#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostProcessRemoveFrameworks
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS) return;

        string unityFrameworkPath = Path.Combine(path, "Frameworks/UnityFramework.framework/Frameworks");

        if (Directory.Exists(unityFrameworkPath))
        {
            Directory.Delete(unityFrameworkPath, true);
            UnityEngine.Debug.Log("Removed disallowed Frameworks folder inside UnityFramework.framework");
        }
    }
}
#endif
