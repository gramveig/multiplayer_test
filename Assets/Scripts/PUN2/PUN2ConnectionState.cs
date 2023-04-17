using System.Collections.Generic;
using Photon.Realtime;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2
{
    public class PUN2ConnectionState
    {
        protected PUN2Controller p2c;

        #region Base

        /// <summary>
        /// Called on state start
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Called on state frame-by-frame update
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Called on state exit
        /// </summary>
        public virtual void Finish() { }

        #endregion

        //these callbacks are repeating MonoBehaviourPunCallbacks.
        //They're used to distribute callbacks to the states
        #region Callbacks

        public virtual void OnConnected()
        {
        }

        public virtual void OnLeftRoom()
        {
        }

        public virtual void OnCreateRoomFailed(short returnCode, string message)
        {
        }

        public virtual void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public virtual void OnCreatedRoom()
        {
        }

        public virtual void OnJoinedLobby()
        {
        }

        public virtual void OnLeftLobby()
        {
        }

        public virtual void OnDisconnected(DisconnectCause cause)
        {
        }

        public virtual void OnJoinedRoom()
        {
        }

        public virtual void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public virtual void OnConnectedToMaster()
        {
        }

        public virtual void OnRoomListUpdate(List<RoomInfo> roomList)
        {
        }

        public virtual void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public virtual void OnMasterClientSwitched(Player newMasterClient)
        {
        }
        
        //NOTE!
        //if adding callback here, it should also be added to PUN2Controller

        #endregion
    }
}