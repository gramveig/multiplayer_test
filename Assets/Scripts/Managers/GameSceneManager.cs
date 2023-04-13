using AlexeyVlasyuk.MultiplayerTest.PUN2;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField]
        private int _numCoins = 20;
        
        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);

            if (!PUN2Controller.Instance.IsCurrentRoom)
            {
                Debug.LogError("No current room");
                OnP2ControllerDisconnected();
                return;
            }

            Subscribe();

            int roomSeed = PUN2Controller.Instance.GetCurrentRoomSeed();
            if (roomSeed == -1)
            {
                OnP2ControllerDisconnected();
                return;
            }
            
            CreateRoom(roomSeed);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void CreateRoom(int roomSeed)
        {
            Random.InitState(roomSeed);
        }

        private void ScatterGold()
        {
            
        }

        private void Subscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected += OnP2ControllerDisconnected;
        }

        private void Unsubscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected -= OnP2ControllerDisconnected;
        }
        
        private void OnP2ControllerDisconnected()
        {
            SceneManager.LoadScene("Disconnect");
        }
    }
}