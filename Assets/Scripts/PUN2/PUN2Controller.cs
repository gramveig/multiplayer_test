using System.Collections.Generic;
using AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Action = System.Action;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2
{
    public class PUN2Controller : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private LogLevel _logLevel;

        private static PUN2Controller _instance;
        private PUN2ConnectionState _connectionState;
        private SortedDictionary<string, RoomInfo> _cachedRoomList = new ();

        public bool IsInitialized { get; private set; }
        public bool IsReadyToSendReceiveEvents { get; set; }

        public event Action OnControllerDisconnected;

        //connection states
        public PUN2ConnectionState csDisconnected { get; private set; }
        public PUN2ConnectionState csConnectingToServer { get; private set; }
        public PUN2ConnectionState csWaitBeforeJoinLobby { get; private set; }

        public enum LogLevel
        {
            Normal,
            Detailed,
            Verbose
        }

        #region Standard Unity Callbacks
        
        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

#if UNITY_EDITOR
            if (_logLevel == LogLevel.Verbose)
            {
                Debug.LogWarning("PUN2: log level verbose not in editor. Lowering log level to Detailed");
                _logLevel = LogLevel.Detailed;
            }
#endif

            DontDestroyOnLoad(this);
        }

        void Start()
        {
            Debug.Log("PUN2: Initializing controller. PUN2 version: " + PhotonNetwork.PunVersion);

            InitConnectionStates();

            SetConnectionState(csDisconnected);

            IsInitialized = true;
        }
        
        #endregion

        #region Public

        public static PUN2Controller Instance => _instance;
        
        public void ConectToServer()
        {
            SetConnectionState(csConnectingToServer);
        }

        public string GetRandomPlayerName()
        {
            const string BaseName = "player";

            return BaseName + Random.Range(1, 999);
        }
        
        public void SetConnectionState(PUN2ConnectionState newConnectionState)
        {
            if (_connectionState == newConnectionState)
            {
                return;
            }

            _connectionState?.Finish();

            _connectionState = newConnectionState;

            _connectionState.Start();
        }

        public void ClearCachedRoomList()
        {
            _cachedRoomList.Clear();
        }

        public void CallOnDisconnectEvent()
        {
            OnControllerDisconnected?.Invoke();
        }

        #endregion
        
        #region Private
        
        private void InitConnectionStates()
        {
            csDisconnected = new P2CDisconnected(this);
            csConnectingToServer = new P2CConnectingToServer(this);
            csWaitBeforeJoinLobby = new P2CWaitBeforeJoinLobby(this);
        }

        #endregion
        
        //sending standard PUN2 callbacks to the states
        #region MonoBehaviourPunCallbacks

        public override void OnConnected()
        {
            _connectionState.OnConnected();
        }

        public override void OnLeftRoom()
        {
            _connectionState.OnLeftRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _connectionState.OnCreateRoomFailed(returnCode, message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _connectionState.OnJoinRoomFailed(returnCode, message);
        }

        public override void OnCreatedRoom()
        {
            _connectionState.OnCreatedRoom();
        }

        public override void OnJoinedLobby()
        {
            _connectionState.OnJoinedLobby();
        }

        public override void OnLeftLobby()
        {
            _connectionState.OnLeftLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _connectionState.OnDisconnected(cause);
        }

        public override void OnJoinedRoom()
        {
            _connectionState.OnJoinedRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            _connectionState.OnJoinRandomFailed(returnCode, message);
        }

        public override void OnConnectedToMaster()
        {
            _connectionState.OnConnectedToMaster();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _connectionState.OnRoomListUpdate(roomList);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            _connectionState.OnPlayerEnteredRoom(newPlayer);
        }

        #endregion
    }
}