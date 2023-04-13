using AlexeyVlasyuk.MultiplayerTest.PUN2;
using AlexeyVlasyuk.MultiplayerTest.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField]
        private int _numCoins = 20;

        [SerializeField]
        private GameObject _coinPrefab;

        [SerializeField]
        private Borders _borders;

        private Camera _cam;
        private Vector2 _worldBtmLeftCorner;
        private Vector2 _worldTopRightCorner;
        
        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);

            _cam = Camera.main;

            Subscribe();

            int roomSeed;
            if (PUN2Controller.Instance.IsCurrentRoom)
            {
                roomSeed = PUN2Controller.Instance.GetCurrentRoomSeed();
            }
            else
            {
                Debug.Log("No current room defined. Scene is running in test mode");
                roomSeed = PUN2Controller.Instance.GetRandomSeed();
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
            _borders.Generate();
            ScatterGold();
        }

        private void ScatterGold()
        {
            const float Margin = 0.5f;
            
            _worldBtmLeftCorner = _cam.ScreenToWorldPoint(Vector3.zero);
            _worldTopRightCorner = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            for (int i = 0; i < _numCoins; i++)
            {
                var pos = new Vector2(Random.Range(_worldBtmLeftCorner.x + Margin, _worldTopRightCorner.x - Margin), Random.Range(_worldBtmLeftCorner.y + Margin, _worldTopRightCorner.y - Margin));
                Instantiate(_coinPrefab, pos, Quaternion.identity);
            }
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