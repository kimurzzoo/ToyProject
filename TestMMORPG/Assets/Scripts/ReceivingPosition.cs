using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class ReceivingPosition : MonoBehaviour
    {
        static Dictionary<string, GameObject> otherPlayers;
        static List<string> deletedPlayer;
        static GameObject myPlayer;
        static GameObject otherPlayerContainer;

        public GameObject otherPlayerPrefab;
        static GameObject staticPrefab;


        void Start()
        {
            otherPlayers = new Dictionary<string, GameObject>();
            deletedPlayer = new List<string>();
            myPlayer = GameObject.Find("MyPlayer");
            otherPlayerContainer = GameObject.Find("OtherPlayers");
            staticPrefab = otherPlayerPrefab;
        }

        public static void CharacterPosition(byte[] buff)
        {
            deletedPlayer.Clear();
            if (otherPlayers.Count > 0)
            {
                foreach(KeyValuePair<string, GameObject> kvp in otherPlayers)
                {
                    deletedPlayer.Add(kvp.Key);
                }
            }

            int offset = 4;
            DataStruct.PositionStruct myPosition = DataStruct.RawDeSerialize(buff, ref offset);
            
            myPlayer.transform.position = new Vector3(myPosition.X, myPosition.Y, -2);
            int count = DataStruct.ReadInt(buff, ref offset);
            Debug.Log("count : " + count.ToString());

            for(int i = 0; i < count; i++)
            {
                int length = DataStruct.ReadInt(buff, ref offset);
                string otherPlayerID = DataStruct.ReadString(buff, ref offset, length);
                DataStruct.PositionStruct otherPosition = DataStruct.RawDeSerialize(buff, ref offset);
                if(deletedPlayer.Contains(otherPlayerID))
                {
                    deletedPlayer.Remove(otherPlayerID);
                }
                Debug.Log("other player id : " + otherPlayerID + " , position : " + otherPosition.X.ToString() + ", " + otherPosition.Y.ToString());
                GameObject otherPlayer;
                if(otherPlayers.TryGetValue(otherPlayerID, out otherPlayer))
                {
                    otherPlayer.transform.position = new Vector3(otherPosition.X, otherPosition.Y, 0);
                }
                else
                {
                    GameObject otherPlayerInstance = Instantiate(staticPrefab, otherPlayerContainer.transform);
                    otherPlayerInstance.transform.position = new Vector3(otherPosition.X, otherPosition.Y, 0);
                    otherPlayers.Add(otherPlayerID, otherPlayerInstance);
                }
            }

            if(deletedPlayer.Count > 0)
            {
                Debug.Log("deleting start");
                for(int i = 0; i < deletedPlayer.Count; i++)
                {
                    GameObject deletingObject = otherPlayers[deletedPlayer[i]];
                    otherPlayers.Remove(deletedPlayer[i]);
                    Destroy(deletingObject);
                }
                Debug.Log("deleting end");
            }

            Debug.Log("end of character position");
        }
    }
}