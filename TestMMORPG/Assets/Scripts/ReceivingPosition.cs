using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class ReceivingPosition : MonoBehaviour
    {
        static Dictionary<string, GameObject> otherPlayers;
        static GameObject myPlayer;
        static GameObject otherPlayerContainer;

        public GameObject otherPlayerPrefab;
        static GameObject staticPrefab;


        void Start()
        {
            otherPlayers = new Dictionary<string, GameObject>();
            myPlayer = GameObject.Find("MyPlayer");
            otherPlayerContainer = GameObject.Find("OtherPlayers");
            staticPrefab = otherPlayerPrefab;
        }

        public static void CharacterPosition(byte[] buff)
        {
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
        }
    }
}