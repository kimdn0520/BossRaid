using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        // ȣ��Ʈ ���� �� UI�� �� ���̰� ��Ȱ��ȭ
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