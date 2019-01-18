using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Color { Red, Blue };
public enum GameStatus {Idle, SetBarrier, Fight, Finish};
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    // specifies the amount of rows and columns on the board
    public int amtRows;
    public int amtColumns;
    public List<Sprite> pieceSprites;
    public Board board;

    public Player player1;
    public Player player2;
    public Player currentPlayer;
    public UIManager uimanager;

    public GameStatus status;

    //time each palyer takes to set barrier
    public float barrierTime;
    //the maximum time of each turn
    public float everyTurnMaxTime;
    //maximum barriers for each player
    public int maxBarriers;

    public Type currentTypeMode;

    

    public Timer timer;
    
    
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

    void positionBox(BoardBox box)
    {
        float xPos = 0.0f;
        float yPos = 0.0f;

        float widthBox = box.gameObject.GetComponent<RectTransform>().rect.width;
        float heightBox = box.gameObject.GetComponent<RectTransform>().rect.height;

        //board locate at center of the screen 
        //relative_column and relative_row shows how far this box to the center

        float relative_column = (float) box.colNum + 0.5f - (float) amtColumns / 2.0f;
        float relative_row = ((float) box.rowNum + 0.5f - (float) amtRows / 2.0f)*-1.0f;

        xPos = box.gameObject.GetComponentInParent<Transform>().position.x + (widthBox * relative_column);
        yPos = box.gameObject.GetComponentInParent<Transform>().position.y + (heightBox * relative_row);

        box.gameObject.transform.position = new Vector3(xPos, yPos, 0.0f);
    }

    void setupBoard()
    {
        List<List<BoardBox>> newBoard = new List<List<BoardBox>>();

        // construct the board
        for (int i = 0; i < amtRows; ++i)
        {
            List<BoardBox> newRow = new List<BoardBox>();

            for (int j = 0; j < amtColumns; ++j)
            {
                GameObject box = Instantiate(board.boardBoxPrefab, board.gameObject.transform);

                // create a new box and set it up
                box.GetComponent<BoardBox>().rowNum = i;
                box.GetComponent<BoardBox>().colNum = j;
                box.GetComponent<BoardBox>().boxContent=BoxContent.Empty;

                // position the box's gameobject
                positionBox(box.GetComponent<BoardBox>());

                newRow.Add(box.GetComponent<BoardBox>());
            }

            // add new row to the board
            newBoard.Add(newRow);
        }

        board.board = newBoard;
        board.transform.localScale=new Vector3(1.1f, 1.1f, 1.0f);
    }

    void Start()
    {

        // based on the amt of rows and columns, change the position of the board to make sure it's centered
        board.gameObject.transform.position = new Vector3 (Screen.width*0.5f,Screen.height*0.5f,0.0f);
        status = GameStatus.Idle;
        setupBoard();
       
    }

    void turn()
    {
        if (currentPlayer==player1)
        {
            currentPlayer = player2;
        }
        else
        {
            currentPlayer = player1;
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
       if(status == GameStatus.Fight)
        {

        }else if(status == GameStatus.SetBarrier)
        {
            if (Time.fixedTime - timer.globalStartTime > 2*barrierTime)
            {
                status = GameStatus.Fight;
                //TBC
                //1.process barriers
                //2.reset timer
                //3.change palyer
            }else if(Time.fixedTime - timer.globalStartTime > barrierTime && currentPlayer == player1)
            {
                currentPlayer = player2;
            }
        }
    }
    //Game start and time running
    
}
