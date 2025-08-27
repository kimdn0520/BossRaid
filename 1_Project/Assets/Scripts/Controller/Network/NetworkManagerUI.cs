using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        // 호스트 시작 후 UI는 안 보이게 비활성화
        gameObject.SetActive(false);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        gameObject.SetActive(false);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }
}