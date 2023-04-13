using AlexeyVlasyuk.MultiplayerTest.PUN2;
using AlexeyVlasyuk.MultiplayerTest.Views;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField]
        private int _numCoins = 20;

        [SerializeField]
        private string _coinPrefab;

        [SerializeField]
        private Borders _borders;

        [SerializeField]
        private Player _playerPrefab;

        [SerializeField]
        private FixedJoystick _coordJoyst;
        
        [SerializeField]
        private FixedJoystick _rotationJoyst;

        [SerializeField]
        private Canvas _joystCanvas;
        
        [SerializeField]
        private Canvas _uiCanvas;
        
        private Camera _cam;
        private Player _player;
        private bool _isTestMode;
        private bool _isRoomBuilt;
        private bool _isGameStarted;
        private int _roomSeed;

        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);

            _cam = Camera.main;

            Subscribe();

            if (PUN2Controller.Instance.IsCurrentRoom)
            {
                _roomSeed = PUN2Controller.Instance.GetCurrentRoomSeed();

                if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount < 2)
                {
                    //cover screen with waiting for players message if must wait for other players
                    _joystCanvas.enabled = false;
                    _uiCanvas.enabled = true;
                }
                else
                {
                    _joystCanvas.enabled = true;
                    _uiCanvas.enabled = false;
                    _isGameStarted = true;
                }
            }
            else
            {
                Debug.Log("No current room defined. Scene is running in test mode");
                _isTestMode = true;
                _roomSeed = PUN2Controller.Instance.GetRandomSeed();
            }

            PUN2Controller.Instance.EnableCustomPun2Events();

            //start building room immediately if master client.
            //Otherwise waiting for RoomIsReady net event
            if (PhotonNetwork.IsMasterClient)
            {
                CreateRoom(_roomSeed);
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Update()
        {
            if (!_isRoomBuilt || !_isGameStarted)
            {
                return;
            }

            if (_player != null)
            {
                _player.UpdateCoord(_coordJoyst.Horizontal, _coordJoyst.Vertical);
                _player.UpdateRotation(_rotationJoyst.Horizontal, _rotationJoyst.Vertical);
            }
        }
        
        private void CreateRoom(int roomSeed)
        {
            _borders.Generate();
            AddPlayer();
            Random.InitState(roomSeed);
            ScatterCoins();
            _isRoomBuilt = true;
            PUN2Controller.Instance.RaiseRoomIsReadyEvent();
        }

        private void ScatterCoins()
        {
            const float Margin = 0.5f;
            
            var worldBtmLeftCorner = _cam.ScreenToWorldPoint(Vector3.zero);
            var worldTopRightCorner = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            for (int i = 0; i < _numCoins; i++)
            {
                var pos = new Vector2(Random.Range(worldBtmLeftCorner.x + Margin, worldTopRightCorner.x - Margin), Random.Range(worldBtmLeftCorner.y + Margin, worldTopRightCorner.y - Margin));
                Coin coin;
                if (!_isTestMode)
                {
                    var coinObj = PhotonNetwork.InstantiateRoomObject(_coinPrefab, pos, Quaternion.identity);
                    if (coinObj == null)
                    {
                        Debug.LogError("Instantiate returned null object for coin " + i);
                    }
                    coin = coinObj.GetComponent<Coin>();
                }
                else
                {
                    var coinPrefab = Resources.Load<Coin>(_coinPrefab);
                    coin = Instantiate(coinPrefab, pos, Quaternion.identity);
                }

                coin.Init(OnCoinPicked);
            }
        }

        private void AddPlayer()
        {
            var worldBtmLeftCorner = _cam.ScreenToWorldPoint(Vector3.zero);
            var worldTopRightCorner = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            var scrWidthUnits = worldTopRightCorner.x - worldBtmLeftCorner.x;
            var scrHeightUnits = worldTopRightCorner.y - worldBtmLeftCorner.y;
            var pos = new Vector2(Random.Range(-scrWidthUnits/2f, scrWidthUnits/2f), Random.Range(-scrHeightUnits/2f, scrHeightUnits/2f));
            _player = Instantiate(_playerPrefab, pos, Quaternion.identity);
        }
        
        private void Subscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected += OnP2ControllerDisconnected;
            PUN2Controller.Instance.OnP2ControllerOtherPlayersJoinedRoom += OnP2ControllerOtherPlayersJoinedRoom;
            PUN2Controller.Instance.OnP2ControllerRoomIsReady += OnP2ControllerRoomIsReady;
        }

        private void Unsubscribe()
        {
            PUN2Controller.Instance.OnP2ControllerDisconnected -= OnP2ControllerDisconnected;
            PUN2Controller.Instance.OnP2ControllerOtherPlayersJoinedRoom -= OnP2ControllerOtherPlayersJoinedRoom;
            PUN2Controller.Instance.OnP2ControllerRoomIsReady -= OnP2ControllerRoomIsReady;
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
            _player.StartFire();
        }
        
        public void OnFireReleased()
        {
            _player.StopFire();
        }

        private void OnP2ControllerOtherPlayersJoinedRoom()
        {
            _joystCanvas.enabled = true;
            _uiCanvas.enabled = false;
            _isGameStarted = true;
        }

        private void OnP2ControllerRoomIsReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            CreateRoom(_roomSeed);
        }
    }
}