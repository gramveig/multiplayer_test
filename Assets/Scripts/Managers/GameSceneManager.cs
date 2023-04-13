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
        private Coin _coinPrefab;

        [SerializeField]
        private Borders _borders;

        [SerializeField]
        private Player _playerPrefab;

        [SerializeField]
        private FixedJoystick _coordJoyst;
        
        [SerializeField]
        private FixedJoystick _rotationJoyst;
        
        private Camera _cam;
        private Vector2 _worldBtmLeftCorner;
        private Vector2 _worldTopRightCorner;
        private Player _player;
        
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

        private void Update()
        {
            if (_player != null)
            {
                _player.UpdateCoord(_coordJoyst.Horizontal, _coordJoyst.Vertical);
                _player.UpdateRotation(_rotationJoyst.Horizontal, _rotationJoyst.Vertical);
            }
        }
        
        private void CreateRoom(int roomSeed)
        {
            Random.InitState(roomSeed);
            _borders.Generate();
            ScatterGold();
            AddPlayer();
        }

        private void ScatterGold()
        {
            const float Margin = 0.5f;
            
            _worldBtmLeftCorner = _cam.ScreenToWorldPoint(Vector3.zero);
            _worldTopRightCorner = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            for (int i = 0; i < _numCoins; i++)
            {
                var pos = new Vector2(Random.Range(_worldBtmLeftCorner.x + Margin, _worldTopRightCorner.x - Margin), Random.Range(_worldBtmLeftCorner.y + Margin, _worldTopRightCorner.y - Margin));
                var coin = Instantiate(_coinPrefab, pos, Quaternion.identity);
                coin.Init(OnCoinPicked);
            }
        }

        private void AddPlayer()
        {
            var pos = new Vector2();
            _player = Instantiate(_playerPrefab, pos, Quaternion.identity);
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

        private void OnCoinPicked()
        {
            Debug.Log("On coin picked");
        }

        public void OnFirePressed()
        {
            Debug.Log("Fire pressed");
        }
    }
}