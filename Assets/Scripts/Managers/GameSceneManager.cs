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
        
        [SerializeField]
        private Canvas _playersCanvas;

        [SerializeField]
        private PlayerName _playerLabelPrefab;

        private static GameSceneManager _instance;
        private Camera _cam;
        private Player _localPlayer;
        private bool _isTestMode;
        private bool _isRoomBuilt;
        private bool _isGameStarted;
        private int _roomSeed;

        #region Standard Unity Callbacks
        
        private void Awake()
        {
            _instance = this;
        }
        
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

            if (_localPlayer != null)
            {
                _localPlayer.UpdateCoord(_coordJoyst.Horizontal, _coordJoyst.Vertical);
                _localPlayer.UpdateRotation(_rotationJoyst.Horizontal, _rotationJoyst.Vertical);
            }
        }

        #endregion

        #region Public

        public static GameSceneManager Instance => _instance;

        public void OnCoinPicked()
        {
            Debug.Log("On coin picked");
        }

        public void OnPlayerInstantiated(Player player, string playerName)
        {
            AddPlayerLabel(player, playerName);
        }

        public void OnPlayerHit(float damage)
        {
            Debug.Log($"Player is hit with {damage} damage");
        }

        #endregion
        
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
                if (!_isTestMode)
                {
                    PhotonNetwork.InstantiateRoomObject(_coinPrefab, pos, Quaternion.identity);
                }
                else
                {
                    var coinPrefabObj = Resources.Load<Coin>(_coinPrefab);
                    Instantiate(coinPrefabObj, pos, Quaternion.identity);
                }
            }
        }

        private void AddPlayer()
        {
            var worldBtmLeftCorner = _cam.ScreenToWorldPoint(Vector3.zero);
            var worldTopRightCorner = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            var scrWidthUnitsHalf = (worldTopRightCorner.x - worldBtmLeftCorner.x)/2f;
            var scrHeightUnitsHalf = (worldTopRightCorner.y - worldBtmLeftCorner.y)/2f;
            var pos = new Vector2(Random.Range(-scrWidthUnitsHalf/2f, scrWidthUnitsHalf/2f), Random.Range(-scrHeightUnitsHalf/2f, scrHeightUnitsHalf/2f));
            if (!_isTestMode)
            {
                var playerObj = PhotonNetwork.Instantiate(_playerPrefab, pos, Quaternion.identity);
                _localPlayer = playerObj.GetComponent<Player>();
            }
            else
            {
                var playerPrefabObj = Resources.Load<Player>(_playerPrefab);
                _localPlayer = Instantiate(playerPrefabObj, pos, Quaternion.identity);
                AddPlayerLabel(_localPlayer, "local player");
            }
        }

        private void AddPlayerLabel(Player player, string nickName)
        {
            var pos = _cam.WorldToScreenPoint(player.CachedTransform.position);
            var playerLabel = Instantiate(_playerLabelPrefab, pos, Quaternion.identity, _playersCanvas.transform);
            playerLabel.Init(nickName, player);
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

        public void OnFirePressed()
        {
            _localPlayer.StartFire();
        }
        
        public void OnFireReleased()
        {
            _localPlayer.StopFire();
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