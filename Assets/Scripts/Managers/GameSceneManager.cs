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
        private string _playerPrefab;

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

            const int DesiredScreenWidthPixels = 1920;
            Vector2Int newResolution = new Vector2Int(DesiredScreenWidthPixels,Mathf.RoundToInt((float)Screen.height * DesiredScreenWidthPixels / Screen.width));
            Screen.SetResolution(newResolution.x,newResolution.y, true);

            Subscribe();

            if (PUN2Controller.Instance.IsCurrentRoom)
            {
                _roomSeed = PUN2Controller.Instance.GetCurrentRoomSeed();

                if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount < 2)
                {
                    //cover screen with waiting for players message if must wait for other players
                    ShowUI();
                }
                else
                {
                    HideUI();
                    _isGameStarted = true;
                }
            }
            else
            {
                Debug.Log("No current room defined. Scene is running in test mode");
                HideUI();
                _isTestMode = true;
                _isGameStarted = true;
                _roomSeed = PUN2Controller.Instance.GetRandomSeed();
            }

            PUN2Controller.Instance.EnableCustomPun2Events();

            //start building room immediately if master client.
            //Otherwise waiting for RoomIsReady net event
            if (PhotonNetwork.IsMasterClient || _isTestMode)
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

        private void ShowUI()
        {
            _joystCanvas.enabled = false;
            _uiCanvas.enabled = true;
        }

        private void HideUI()
        {
            _joystCanvas.enabled = true;
            _uiCanvas.enabled = false;
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

        private async void ScatterCoins()
        {
            const float Margin = 2f;
            
            var worldBtmLeftCorner = _cam.ScreenToWorldPoint(Vector3.zero);
            var worldTopRightCorner = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            for (int i = 0; i < _numCoins; i++)
            {
                var pos = new Vector2(Random.Range(worldBtmLeftCorner.x + Margin, worldTopRightCorner.x - Margin), Random.Range(worldBtmLeftCorner.y + Margin, worldTopRightCorner.y - Margin));
                Coin coin = null;
                if (!_isTestMode)
                {
                    var coinObj = PhotonNetwork.InstantiateRoomObject(_coinPrefab, pos, Quaternion.identity);
                    //fix problem with slow instantiation of coins when using PhotonNetwork.Instantiate
                    await UniTask.WaitUntil(() => coinObj != null);
                    
                    coin = coinObj.GetComponent<Coin>();
                }
                else
                {
                    var coinPrefabObj = Resources.Load<Coin>(_coinPrefab);
                    coin = Instantiate(coinPrefabObj, pos, Quaternion.identity);
                }

                coin.Init(OnCoinPicked);
            }
        }

        private async void AddPlayer()
        {
            var worldBtmLeftCorner = _cam.ScreenToWorldPoint(Vector3.zero);
            var worldTopRightCorner = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            var scrWidthUnits = worldTopRightCorner.x - worldBtmLeftCorner.x;
            var scrHeightUnits = worldTopRightCorner.y - worldBtmLeftCorner.y;
            var pos = new Vector2(Random.Range(-scrWidthUnits/2f, scrWidthUnits/2f), Random.Range(-scrHeightUnits/2f, scrHeightUnits/2f));
            if (PhotonNetwork.IsMasterClient && !_isTestMode)
            {
                var playerObj = PhotonNetwork.Instantiate(_playerPrefab, pos, Quaternion.identity);
                await UniTask.WaitUntil(() => playerObj != null);

                _player = playerObj.GetComponent<Player>();
            }
            else
            {
                var playerPrefabObj = Resources.Load<Player>(_playerPrefab);
                _player = Instantiate(playerPrefabObj, pos, Quaternion.identity);
            }
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
            HideUI();
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