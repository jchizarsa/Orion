using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Winning condition
        if(Chessboard.Instance.blackKingDead){
            Debug.Log("Congratulations, you won!");
            Chessboard.Instance.blackKingDead = false;
            MainMenuController.Instance.OpenWinCam();
        }
    }
}
