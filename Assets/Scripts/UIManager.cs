using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    public GameObject pieceChoicePanel;

    public Text currentPlayerText;
    public Text currentModeText;

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }

        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }
        Time.timeScale = 1f;
    }

    public void OnClickModeButton(string type)
    {
        if (type == "rock")
        {
            GameManager.instance.currentTypeMode = Type.Rock;
            currentModeText.text = "Rock Mode";
        }
        else if (type == "scissor")
        {
            GameManager.instance.currentTypeMode = Type.Scissors;
            currentModeText.text = "Scissors Mode";
        }
        else
        {
            GameManager.instance.currentTypeMode = Type.Paper;
            currentModeText.text = "Paper Mode";
        }
    }

    private void Update()
    {
        if (GameManager.instance.currentPlayer.color  == GameManager.instance.player1.color)
        {
            currentPlayerText.text = "Player 1's Turn";
        }
        else
        {
            currentPlayerText.text = "Player 2's Turn";
        }
    }


}
