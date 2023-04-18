using System.Threading.Tasks;
using System;
using Firebase.Extensions;
using Firebase.RemoteConfig;

public class FB_GServices
{
    static Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    public static bool isRemoteConfigFetched = false;
    public static void IncludeDependencies() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
          InitializeFirebase();
        } else {
          UnityEngine.Debug.LogError(
            "Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
      });
    }

    private static void InitializeFirebase() {
      // [START set_defaults]
      System.Collections.Generic.Dictionary<string, object> defaults =
        new System.Collections.Generic.Dictionary<string, object>();

      // These are the values that are used if we haven't fetched data from the
      // server
      // yet, or if we ask for values that the server doesn't have:
      defaults.Add("url", "");

      Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
        .ContinueWithOnMainThread(task => {
        // [END set_defaults]
        UnityEngine.Debug.Log("RemoteConfig configured and ready!");
        FetchDataAsync();
      });

    }

    public static Task FetchDataAsync() {
        System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private static void FetchComplete(Task fetchTask) {
        if (!fetchTask.IsCompleted) {
            UnityEngine.Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if(info.LastFetchStatus != LastFetchStatus.Success) {
            UnityEngine.Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
            .ContinueWithOnMainThread(
            task => {
                UnityEngine.Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
                isRemoteConfigFetched = true;
            });
}
}
