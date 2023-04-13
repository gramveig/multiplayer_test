using Photon.Pun;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CCreateRoom : PUN2ConnectionState
    {
        public P2CCreateRoom(PUN2Controller p2c)
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

            p2c.StartRoomCreation();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("PUN2: Successfully created new room");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN2: Successfully joined created room (or the existing one, if the room with the same name already existed)");
            p2c.CallOnJoinedRoomEvent();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"PUN2: Unable to join room. Return code: {returnCode}. Message: {message}");
            p2c.CallOnDisconnectEvent();
        }
    }
}