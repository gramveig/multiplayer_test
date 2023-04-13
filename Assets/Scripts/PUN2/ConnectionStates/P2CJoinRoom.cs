using Photon.Pun;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CJoinRoom : PUN2ConnectionState
    {
        public P2CJoinRoom(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN2: not connected to server");
                p2c.CallOnDisconnectEvent();
                return;
            }

            p2c.StartJoinRoom();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN2: Successfully joined room");
            p2c.CallOnFollowingClientJoinedRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"PUN2: Unable to join room. Return code: {returnCode}. Message: {message}");
            p2c.CallOnCannotJoinRoomEvent();
            p2c.SetConnectionState(p2c.csJoinLobby);
        }
    }
}