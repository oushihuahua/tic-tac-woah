﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Color { Red, Blue };

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    // specifies the amount of rows and columns on the board
    public int amtRows;
    public int amtColumns;

    public List<Sprite> pieceSprites;

    public Player player1;
    public Player player2;
    public Player currentPlayer;

    public Type currentTypeMode;

    public Board board;

    [System.NonSerialized]
    public bool endTurn;

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
        endTurn = false;
    }

    void positionBox(BoardBox box)
    {
        float xPos = 0.0f;
        float yPos = 0.0f;

        float widthBox = box.gameObject.GetComponent<RectTransform>().rect.width;
        float heightBox = box.gameObject.GetComponent<RectTransform>().rect.height;

        xPos = box.gameObject.GetComponentInParent<Transform>().position.x + (widthBox * box.rowNum);
        yPos = box.gameObject.GetComponentInParent<Transform>().position.y - (heightBox * box.colNum);

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
                box.GetComponent<BoardBox>().playable = true;

                // position the box's gameobject
                positionBox(box.GetComponent<BoardBox>());

                newRow.Add(box.GetComponent<BoardBox>());
            }

            // add new row to the board
            newBoard.Add(newRow);
        }

        board.board = newBoard;
    }

    void Start()
    {

        // based on the amt of rows and columns, change the position of the board to make sure it's centered
        board.gameObject.transform.position = new Vector3
        (
            board.gameObject.transform.position.x - (board.boardBoxPrefab.gameObject.GetComponent<RectTransform>().rect.width / 2 * (amtRows - 1)),
            board.gameObject.transform.position.y + (board.boardBoxPrefab.gameObject.GetComponent<RectTransform>().rect.height / 2 * (amtColumns - 1)),
            0.0f
        );

        setupBoard();

        /*
         * 
         * set up barriers at the beginning of the game
         * 
         */

    }

    void turn()
    {
        // determine who's turn it is
        if (currentPlayer.color == player1.color)
        {
            currentPlayer = player2;
        }
        else if (currentPlayer.color == player2.color)
        {
            currentPlayer = player1;
        }
        else
        {
            // for the first turn
            currentPlayer = player1;
        }

        while (!endTurn)
        {

        }


    }

    void Update()
    {
        turn();
    }
}
