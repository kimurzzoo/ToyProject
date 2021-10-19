using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class QuitGame : MonoBehaviour
    {
        public void ExitGame()
        {
            Application.Quit(); // 어플리케이션 종료
        }
    }
}