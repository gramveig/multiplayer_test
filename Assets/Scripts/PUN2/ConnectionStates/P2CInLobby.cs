using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CInLobby : PUN2ConnectionState
    {
        public P2CInLobby(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            Debug.Log("PUN2: Listening to room list updates");

            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("PUN2: not connected to server");
                OnServerDisconnect();
                return;
            }

            if (!PhotonNetwork.InLobby)
            {
                Debug.LogError("PUN2: not in Lobby");
                OnFailure();
                return;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("PUN2: Unexpectedly disconnected from Photon server. Cause: " + cause);
            OnServerDisconnect();
        }

        public override void OnLeftLobby()
        {
            Debug.Log("PUN2: Unexpectedly left Lobby");
            OnFailure();
        }

        //room list updates while we're in lobby
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            int roomCount = p2c.UpdateCachedRoomList(roomList);

            if (p2c.IsDetailedLog)
            {
                Debug.Log("PUN2: Updating room list. Room count: " + roomCount);
            }
        }

        void OnFailure()
        {
            p2c.SetConnectionState(p2c.csWaitBeforeRejoinLobby);
        }

        void OnServerDisconnect()
        {
            p2c.CallOnDisconnectEvent();
        }
    }
}