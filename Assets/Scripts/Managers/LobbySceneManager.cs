using System;
using System.Threading;
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

        [SerializeField]
        private TMP_Text _foundRooms;

        [SerializeField]
        private GameObject _roomNotFound;

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

            PUN2Controller.Instance.CreateRoom(roomName);
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

            PUN2Controller.Instance.JoinRoom(roomName);
        }

        public void OnJoinRoomNameChanged()
        {
            const int MaxNames = 3;

            if (!_isInitialized)
            {
                return;
            }

            string roomName = _joinRoomInput.text;
            if (string.IsNullOrEmpty(roomName))
            {
                _foundRooms.text = "";
                return;
            }

            var foundNames = PUN2Controller.Instance.GetRoomNames(roomName, MaxNames);
            if (foundNames.Count == 0)
            {
                _foundRooms.text = "";
                return;
            }
            
            string names = "Found rooms: ";
            for (int i = 0; i < foundNames.Count; i++)
            {
                string name = foundNames[i];
                names += name;
                if (i < foundNames.Count - 1)
                {
                    names += ", ";
                }
            }

            _foundRooms.text = names;
        }
        
        private void Subscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected += OnP2ControllerDisconnected;
            PUN2Controller.Instance.OnP2ControllerConnectedToLobby += OnP2ControllerConnectedToLobby;
            PUN2Controller.Instance.OnP2ControllerMainClientJoinedRoom += OnP2ControllerMainClientJoinedRoom;
            PUN2Controller.Instance.OnP2ControllerFollowingClientJoinedRoom += OnP2ControllerFollowingClientJoinedRoom;
            PUN2Controller.Instance.OnP2ControllerCannotJoinRoom += OnP2ControllerCannotJoinRoom;
        }

        private void Unsubscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected -= OnP2ControllerDisconnected;
            PUN2Controller.Instance.OnP2ControllerConnectedToLobby -= OnP2ControllerConnectedToLobby;
            PUN2Controller.Instance.OnP2ControllerMainClientJoinedRoom -= OnP2ControllerMainClientJoinedRoom;
            PUN2Controller.Instance.OnP2ControllerFollowingClientJoinedRoom -= OnP2ControllerFollowingClientJoinedRoom;
            PUN2Controller.Instance.OnP2ControllerCannotJoinRoom -= OnP2ControllerCannotJoinRoom;
        }

        private void OnP2ControllerDisconnected()
        {
            SceneManager.LoadScene("Disconnect");
        }

        private void OnP2ControllerConnectedToLobby()
        {
            _isInitialized = true;
        }

        private void OnP2ControllerMainClientJoinedRoom()
        {
            PUN2Controller.Instance.DisableCustomPUN2Events();
            SceneManager.LoadScene("Game");
        }

        private void OnP2ControllerFollowingClientJoinedRoom()
        {
            SceneManager.LoadScene("Game");
        }
        
        private void OnP2ControllerCannotJoinRoom()
        {
            ShowAbsentRoomWarning();
        }

        private async void ShowAbsentRoomWarning()
        {
            _roomNotFound.SetActive(true);

            await UniTask.Delay(3000);

            _roomNotFound.SetActive(false);
        }
    }
}