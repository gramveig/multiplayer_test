using System;
using AlexeyVlasyuk.MultiplayerTest.PUN2;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class LobbySceneManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _createRoomInput;
        
        [SerializeField]
        private TMP_InputField _joinRoomInput;

        private bool _isInitialized;
        
        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);

            Subscribe();

            if (!PUN2Controller.Instance.IsInLobby)
            {
                PUN2Controller.Instance.ConectToServer();
            }
            else
            {
                _isInitialized = true;
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void OnCreateRoomPressed()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            string roomName = _createRoomInput.text;
            if (string.IsNullOrEmpty(roomName))
            {
                return;
            }

            Debug.Log("Creating room: " + roomName);
        }
        
        public void OnJoinRoomPressed()
        {
            if (!_isInitialized)
            {
                return;
            }

            string roomName = _joinRoomInput.text;
            if (string.IsNullOrEmpty(roomName))
            {
                return;
            }

            Debug.Log("Joining room: " + roomName);
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
            _isInitialized = true;
        }
    }
}