using System;
using AlexeyVlasyuk.MultiplayerTest.PUN2;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class LobbySceneManager : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);

            PUN2Controller.Instance.OnControllerDisconnected += OnControllerDisconnected;
        }

        private void OnDestroy()
        {
            PUN2Controller.Instance.OnControllerDisconnected -= OnControllerDisconnected;
        }

        private void OnControllerDisconnected()
        {
            SceneManager.LoadScene("Disconnect");
        }
    }
}