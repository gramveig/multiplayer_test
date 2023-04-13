using System;
using AlexeyVlasyuk.MultiplayerTest.PUN2;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class WaitingForPlayersSceneManager : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);


            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected += OnP2ControllerDisconnected;
            PUN2Controller.Instance.OnP2ControllerOtherPlayersJoinedRoom += OnP2ControllerOtherPlayersJoinedRoom;
        }

        private void Unsubscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected -= OnP2ControllerDisconnected;
            PUN2Controller.Instance.OnP2ControllerOtherPlayersJoinedRoom -= OnP2ControllerOtherPlayersJoinedRoom;
        }
        
        private void OnP2ControllerDisconnected()
        {
            SceneManager.LoadScene("Disconnect");
        }

        private void OnP2ControllerOtherPlayersJoinedRoom()
        {
            SceneManager.LoadScene("Game");
        }
    }
}