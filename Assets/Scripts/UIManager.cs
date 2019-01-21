using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    public GameObject pieceChoicePanel;
    public GameObject piecePrefab;

    public Button barrierButton;
    public Button rockButton;
    public Button paperButton;
    public Button scissorButton;

    public Button startGameButton;

    public Text currentPlayerText;
    public Text currentModeText;
    public Text errorText;
    public Text p1ScoreText;
    public Text p2ScoreText;

    public Text p1Win;
    public Text p2Win;

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
        PlayerColor curColor = GameManager.instance.currentPlayer.color;
        //Debug.Log(GameManager.instance.currentPlayer);
        if (GameManager.instance.status == GameStatus.Fight)
        {
            if (clickedBox.boxContent == BoxContent.Barrier) {
                StartCoroutine(flashErrorText("Can't place anything in a box with a barrier!"));
                return;
            }         
            int buffer = 0;
            if (curColor == PlayerColor.Blue)
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
                if (GameManager.instance.checkGameOver())
                {
                    //game is over right now, no place to place any piece now.
                    int res=GameManager.instance.gameOver();
                    if (res == 0)
                        p1Win.gameObject.SetActive(true);
                    else if (res == 1)
                        p2Win.gameObject.SetActive(true);
                    else if (res == 2)
                    {
                        p1Win.gameObject.SetActive(true);
                        p2Win.gameObject.SetActive(true);
                    }
                    startGameButton.interactable = true;
                }
            }
        }else if(GameManager.instance.status == GameStatus.SetBarrier)
        {
            if (clickedBox.boxContent == BoxContent.Empty && GameManager.instance.countBarrier<GameManager.instance.maxBarriers)
            {
                //GameObject newPiece = Instantiate(piecePrefab, clickedBox.transform);
                clickedBox.GetComponent<Image>().sprite = GameManager.instance.pieceSprites[6];
                //dont set box content here, just remember all the barriers
                clickedBox.boxContent = BoxContent.Barrier;
                GameManager.instance.countBarrier++;
                if (curColor == PlayerColor.Blue)
                {
                    clickedBox.barrierInfo[0] = true;
                }
                else
                    clickedBox.barrierInfo[1] = true;
            }else if(clickedBox.boxContent == BoxContent.Barrier)
            {
                clickedBox.GetComponent<Image>().sprite = GameManager.instance.pieceSprites[7];
                clickedBox.boxContent = BoxContent.Empty;
                if (curColor == PlayerColor.Blue)
                {
                    clickedBox.barrierInfo[0] = false;
                }
                else
                    clickedBox.barrierInfo[1] = false;
                GameManager.instance.countBarrier--;
            }
        }

    }

    public void OnClickModeButton(string type)
    {
        if (type == "rock" && GameManager.instance.status == GameStatus.Fight)
        {
            rockButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xb9, 0x45,0xff);
            scissorButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
            paperButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
            GameManager.instance.currentTypeMode = Type.Rock;
            currentModeText.text = "Rock Mode";
        }
        else if (type == "scissor" && GameManager.instance.status == GameStatus.Fight)
        {
            scissorButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xb9, 0x45, 0xff);
            rockButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
            paperButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
            GameManager.instance.currentTypeMode = Type.Scissors;
            currentModeText.text = "Scissors Mode";
        }
        else if (type == "paper" && GameManager.instance.status == GameStatus.Fight)
        {
            paperButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xb9, 0x45, 0xff);
            scissorButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
            rockButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
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
        GameManager.instance.startGame();
        barrierButton.interactable = true;
        p1Win.gameObject.SetActive(false);
        p2Win.gameObject.SetActive(false);
        currentModeText.text = "Set Barrier Mode";
        rockButton.interactable = false;
        paperButton.interactable = false;
        scissorButton.interactable = false;

        paperButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
        scissorButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);
        rockButton.gameObject.GetComponent<Image>().color = new Color32(0xff, 0xff, 0xff, 0xff);

        //status thing all changed by GameManager
        //GameManager.instance.status = GameStatus.Fight;
        startGameButton.gameObject.GetComponentInChildren<Text>().text = "Restart Game";
    }
    public void reAbleButtons()
    {
        barrierButton.interactable = false;
        rockButton.interactable = true;
        paperButton.interactable = true;
        scissorButton.interactable = true;
    }
}
