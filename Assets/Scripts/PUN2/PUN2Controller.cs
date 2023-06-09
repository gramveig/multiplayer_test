using System.Collections.Generic;
using AlexeyVlasyuk.MultiplayerTest.Models;
using AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates;
using AlexeyVlasyuk.MultiplayerTest.Utilities;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2
{
    public enum PUN2CustomEvents : byte
    {
        RoomIsReady,
        MasterPlayerData
    };
    
    public class PUN2Controller : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private LogLevel _logLevel;

        private static PUN2Controller _instance;
        private PUN2ConnectionState _connectionState;
        private SortedDictionary<string, RoomInfo> _cachedRoomList = new ();
        private string _roomName;
        private List<string> _roomNames = new ();

        public bool IsInitialized { get; private set; }

        public event Action OnP2ControllerDisconnected;
        public event Action OnP2ControllerConnectedToLobby;
        public event Action OnP2ControllerJoinedRoom;
        public event Action OnP2ControllerCannotJoinRoom;
        public event Action OnP2ControllerOtherPlayersJoinedRoom;
        public event Action OnP2ControllerRoomIsReady;

        //connection states
        public PUN2ConnectionState csDisconnected { get; private set; }
        public PUN2ConnectionState csConnectingToServer { get; private set; }
        public PUN2ConnectionState csWaitBeforeJoinLobby { get; private set; }
        public PUN2ConnectionState csJoinLobby { get; private set; }
        public PUN2ConnectionState csWaitBeforeRejoinLobby { get; private set; }
        public PUN2ConnectionState csInLobby { get; private set; }
        public P2CCreateRoom csCreateRoom { get; private set; }
        public P2CJoinRoom csJoinRoom { get; private set; }
        public P2CInGame csInGame { get; private set; }

        public readonly TypedLobby customLobby = new TypedLobby("MultiplayerTestLobby", LobbyType.Default);

        public readonly RaiseEventOptions CommonRaiseEventOpts = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.AddToRoomCache
        };
        public readonly SendOptions CommonSendOpts = SendOptions.SendReliable;
        
        //max player in room
        private const byte MaxPlayersInRoom = 20; //20 is a free PUN version limit

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

        void Update()
        {
            _connectionState?.Update();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            PhotonNetwork.NetworkingClient.EventReceived += OnCustomPUN2Event;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            PhotonNetwork.NetworkingClient.EventReceived -= OnCustomPUN2Event;
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

            return BaseName + UnityEngine.Random.Range(1, 999);
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
            OnP2ControllerDisconnected?.Invoke();
        }

        public void CallOnConnectedToLobby()
        {
            OnP2ControllerConnectedToLobby?.Invoke();
        }

        public bool IsDetailedLog
        {
            get
            {
                return (int)_logLevel > (int)LogLevel.Normal;
            }
        }

        public int UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (var info in roomList)
            {
                if (info.RemovedFromList)
                    _cachedRoomList.Remove(info.Name);
                else
                    _cachedRoomList[info.Name] = info;
            }

            return _cachedRoomList.Count;
        }

        public bool IsInLobby => _connectionState == csInLobby;

        public void CreateRoom(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("PUN2: cannot create room: room name is empty");
                return;
            }

            _roomName = roomName;
            SetConnectionState(csCreateRoom);
        }

        /// <summary>
        /// Called from create room state
        /// </summary>
        public void StartRoomCreation()
        {
            if (string.IsNullOrEmpty(_roomName))
            {
                Debug.LogError("PUN2: cannot create room: room name is not defined");
                SetConnectionState(csInLobby);
                return;
            }

            int roomSeed = GetRandomSeed();
            //as PUN2 can't serialize colors, we will store in a room their shuffled indexes for ColorHelper.AllColors
            var colorIndexes = ArrayHelper.GetRandomizedIndexes(ColorHelper.AllColors.Length);
            ArrayHelper.ShuffleArray(colorIndexes);

            RoomOptions roomOptions = new RoomOptions
            {
                IsOpen = true,
                IsVisible = true,
                MaxPlayers = MaxPlayersInRoom,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "Seed", roomSeed },
                    { "ColorIndexes", colorIndexes }
                }
            };

            Debug.Log($"Player {PhotonNetwork.NickName} has attempted to create room '{_roomName}' or join the room with the same name if it's already created");

            bool result = PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, customLobby);
            if (!result)
            {
                Debug.Log("PUN2: Unable to create or join room");
                CallOnDisconnectEvent();
            }
        }

        public void JoinRoom(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("PUN2: cannot join room: room name is empty");
                return;
            }

            _roomName = roomName;

            SetConnectionState(csJoinRoom);
        }

        public void StartJoinRoom()
        {
            if (string.IsNullOrEmpty(_roomName))
            {
                Debug.LogError("PUN2: cannot join room: room name is not defined");
                SetConnectionState(csInLobby);
                return;
            }

            Debug.Log($"Player {PhotonNetwork.NickName} has attempted to join room '{_roomName}'");
            bool result = PhotonNetwork.JoinRoom(_roomName);
            if (!result)
            {
                Debug.Log("PUN2: Unable to join room");
                CallOnCannotJoinRoomEvent();
            }
        }

        public void AssureDisconnection()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        public void CallOnJoinedRoomEvent()
        {
            OnP2ControllerJoinedRoom?.Invoke();
        }

        public void CallOnCannotJoinRoomEvent()
        {
            OnP2ControllerCannotJoinRoom?.Invoke();
        }

        public void CallOnOtherPlayersJoinedRoom()
        {
            OnP2ControllerOtherPlayersJoinedRoom?.Invoke();
        }

       
        public List<string> GetRoomNames(string nameStart, int maxNames)
        {
            _roomNames.Clear();
            foreach (KeyValuePair<string, RoomInfo> keyValuePair in _cachedRoomList)
            {
                string roomName = keyValuePair.Key;
                if (roomName.StartsWith(nameStart))
                {
                    _roomNames.Add(roomName);
                }

                if (_roomNames.Count >= maxNames)
                {
                    break;
                }
            }

            return _roomNames;
        }

        public bool IsCurrentRoom => PhotonNetwork.CurrentRoom != null;

        public int GetCurrentRoomSeed()
        {
            var room = PhotonNetwork.CurrentRoom;
            if (room == null)
            {
                Debug.LogError("PUN2: No current room");
                return - 1;
            }
            
            if (!room.CustomProperties.ContainsKey("Seed"))
            {
                Debug.LogError("PUN2: Seed property is not saved in the room properties");
                return - 1;
            }

            int roomSeed = (int)room.CustomProperties["Seed"];

            return roomSeed;
        }

        public int GetRandomSeed()
        {
            return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public void RaiseRoomIsReadyEvent()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            PhotonNetwork.RaiseEvent((byte)PUN2CustomEvents.RoomIsReady, null, CommonRaiseEventOpts, CommonSendOpts);
        }
        
        public void DisableCustomPUN2Events()
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            Debug.Log("PUN2: Custom PUN2 messages disabled");
        }

        public void EnableCustomPun2Events()
        {
            PhotonNetwork.IsMessageQueueRunning = true;
            Debug.Log("PUN2: Custom PUN messages enabled");
        }

        public bool IsRoomCreated(string roomName)
        {
            foreach (KeyValuePair<string, RoomInfo> keyValuePair in _cachedRoomList)
            {
                if (roomName == keyValuePair.Key)
                {
                    return true;
                }
            }

            return false;
        }

        public void DisconnectFromServer()
        {
            SetConnectionState(csDisconnected);
        }

        #endregion
        
        #region Private
        
        private void InitConnectionStates()
        {
            csDisconnected = new P2CDisconnected(this);
            csConnectingToServer = new P2CConnectingToServer(this);
            csWaitBeforeJoinLobby = new P2CWaitBeforeJoinLobby(this);
            csJoinLobby = new P2CJoinLobby(this);
            csWaitBeforeRejoinLobby = new P2CWaitBeforeRejoinLobby(this);
            csInLobby = new P2CInLobby(this);
            csCreateRoom = new P2CCreateRoom(this);
            csJoinRoom = new P2CJoinRoom(this);
            csInGame = new P2CInGame(this);
        }
        
        private void OnCustomPUN2Event(EventData photonEvent)
        {
            PUN2CustomEvents eventCode = (PUN2CustomEvents)photonEvent.Code;

            switch (eventCode)
            {
                case PUN2CustomEvents.RoomIsReady: OnP2ControllerRoomIsReady?.Invoke(); break;
            }
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

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            _connectionState.OnMasterClientSwitched(newMasterClient);
        }

        #endregion
    }
}