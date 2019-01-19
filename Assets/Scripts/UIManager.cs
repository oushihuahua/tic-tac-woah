using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    public GameObject pieceChoicePanel;
    public GameObject piecePrefab;

    public Text currentPlayerText;
    public Text currentModeText;
    public Text errorText;
    public Text p1ScoreText;
    public Text p2ScoreText;

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

    IEnumerator flashErrorText(string text)
    {
        errorText.text = text;
        errorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        errorText.gameObject.SetActive(false);
    }

    public void OnClickBox(BoardBox clickedBox)
    {
        if (GameManager.instance.status == GameStatus.Fight)
        {
            if (clickedBox.boxContent == BoxContent.Barrier) {
                StartCoroutine(flashErrorText("Can't place anything in a box with a barrier!"));
                return;
            }

            Color curColor = GameManager.instance.currentPlayer.color;
            int buffer = 0;
            if (curColor == Color.Blue)
            {
                buffer += 3;
            }

            if (clickedBox.boxContent == BoxContent.Empty)
            {
                GameObject newPiece = Instantiate(piecePrefab, clickedBox.transform);
                clickedBox.GetComponent<Image>().sprite = GameManager.instance.pieceSprites[(int)GameManager.instance.currentTypeMode + buffer];
                clickedBox.boxContent = BoxContent.Piece;
                clickedBox.currentPiece = newPiece.GetComponent<Piece>();
                ++GameManager.instance.currentPlayer.score;
                GameManager.instance.processPlay(clickedBox);
            }
        }
    }

    public void OnClickModeButton(string type)
    {
        if (type == "rock" && GameManager.instance.status != GameStatus.SetBarrier)
        {
            GameManager.instance.currentTypeMode = Type.Rock;
            currentModeText.text = "Rock Mode";
        }
        else if (type == "scissor" && GameManager.instance.status != GameStatus.SetBarrier)
        {
            GameManager.instance.currentTypeMode = Type.Scissors;
            currentModeText.text = "Scissors Mode";
        }
        else if (type == "paper" && GameManager.instance.status != GameStatus.SetBarrier)
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
    public void OnClickStartGameButton()
    {
        GameManager.instance.timer.globalStartTime = Time.fixedTime;
        GameManager.instance.status = GameStatus.SetBarrier;
        GameManager.instance.currentPlayer = GameManager.instance.player1;
        GameManager.instance.currentTypeMode = Type.Barrier;
        currentModeText.text = "Set Barrier Mode";
        //GameManager.instance.status = GameStatus.Fight;
    }

}
