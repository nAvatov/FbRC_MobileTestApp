using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class InternetConnection
{
    const string trusterWebUrl = "http://google.com";

    public static IEnumerator CheckInternetConnection(System.Action<bool> act) {
        UnityWebRequest req = new UnityWebRequest(trusterWebUrl);
        yield return req.SendWebRequest();
        
        act(req.error == null);
    }

    public static IEnumerator SendRequestTo(string url, System.Action<string> callback) {
        UnityWebRequest req = new UnityWebRequest(url);

        yield return req.SendWebRequest();

        callback(req.downloadHandler.text);
    }
}
