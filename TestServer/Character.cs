using System;

namespace Backend
{
    class Character
    {
        public static void CharacterMoving(byte[] buff, string ID)
        {
            DataPacket.PositionStruct playerPosition = Server.UserPositions[ID];
            
            int offset = 4;
            int input = DataPacket.ReadInt(buff, ref offset);
            switch((DataPacket.PressedButton) input)
            {
                case DataPacket.PressedButton.UP:
                {
                    if(playerPosition.Y < 300)
                    {
                        playerPosition.Y += 1;
                    }
                    break;
                }
                case DataPacket.PressedButton.DOWN:
                {
                    if(playerPosition.Y > 0)
                    {
                        playerPosition.Y -= 1;
                    }
                    break;
                }
                case DataPacket.PressedButton.LEFT:
                {
                    if(playerPosition.X > 0)
                    {
                        playerPosition.X -= 1;
                    }
                    break;
                }
                case DataPacket.PressedButton.RIGHT:
                {
                    if(playerPosition.X < 600)
                    {
                        playerPosition.X += 1;
                    }
                    break;
                }
            }

            Console.WriteLine(ID + " : " + playerPosition.X.ToString() + ", " + playerPosition.Y.ToString());
            Server.UserPositions[ID] = playerPosition;
        }
    }
}