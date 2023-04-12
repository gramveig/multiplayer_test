using System;
using AlexeyVlasyuk.MultiplayerTest.PUN2;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class LoadingSceneManager : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);

            PUN2Controller.Instance.ConectToServer();

            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            PUN2Controller.Instance.OnControllerDisconnected += OnControllerDisconnected;
            PUN2Controller.Instance.OnConnectedToLobby += OnConnectedToLobby;
        }

        private void Unsubscribe()
        {
            PUN2Controller.Instance.OnControllerDisconnected -= OnControllerDisconnected;
            PUN2Controller.Instance.OnConnectedToLobby -= OnConnectedToLobby;
        }
        
        private void OnControllerDisconnected()
        {
            SceneManager.LoadScene("Disconnect");
        }

        private void OnConnectedToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}