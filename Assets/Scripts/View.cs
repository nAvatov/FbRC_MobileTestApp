using UnityEngine;

[RequireComponent(typeof(UniWebView))]
public class View : MonoBehaviour
{
    private UniWebView _webViewComponent;

    private void Awake() {
        _webViewComponent = GetComponent<UniWebView>();
    }

    public void Show(string url) {
        _webViewComponent.Load(url);
        _webViewComponent.Show();
    }   
}
