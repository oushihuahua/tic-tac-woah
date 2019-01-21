using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerColor { Red, Blue };
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
    public CountDownTimer countDownTimer;

    public GameStatus status;

    //time each palyer takes to set barrier
    public float barrierTime;
    //the maximum time of each turn
    public float everyTurnMaxTime;
    //maximum barriers for each player
    public int maxBarriers;
    public Sprite barrierSprite;
    public int countBarrier;

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

    //box1 and box2 are from same palyer and they are same piece
    bool isPair(BoardBox box1, BoardBox box2)
    {
        if (box1 == null || box2 == null) return false;
        if (box1.boxContent != BoxContent.Piece || box2.boxContent != BoxContent.Piece) return false;

        return box1.currentPiece.owner == box2.currentPiece.owner && box1.currentPiece.type == box2.currentPiece.type;
    }

    //three same  piece in a row
    bool checkForSequence(BoardBox box)
    {
        BoardBox leftBox = null;
        BoardBox rightBox = null;
        BoardBox upBox = null;
        BoardBox downBox = null;
        BoardBox posDiagonal = null;
        BoardBox negDiagonal = null;
        BoardBox leftDiagonal = null;
        BoardBox rightDiagonal = null;
        if (box.rowNum + 1 < amtRows)
        {
            rightBox = board.board[box.rowNum + 1][box.colNum];
            if (rightBox.isSequence && isPair(box, rightBox)) return true;
            else if (box.rowNum + 2 < amtRows)
            {
                if (isPair(box, rightBox) && isPair(rightBox, board.board[box.rowNum + 2][box.colNum]))
                {
                    rightBox.isSequence = true;
                    board.board[box.rowNum + 2][box.colNum].isSequence = true;
                    return true;
                }
            }
        }
        if (box.rowNum - 1 >= 0)
        {
            leftBox = board.board[box.rowNum - 1][box.colNum];
            if (leftBox.isSequence && isPair(box, leftBox)) return true;
            else if (box.rowNum - 2 >= 0)
            {
                if (isPair(box, leftBox) && isPair(leftBox, board.board[box.rowNum - 2][box.colNum]))
                {
                    leftBox.isSequence = true;
                    board.board[box.rowNum - 2][box.colNum].isSequence = true;
                    return true;
                }
            }
        }
        if (box.colNum + 1 < amtColumns)
        {
            upBox = board.board[box.rowNum][box.colNum + 1];
            if (upBox.isSequence && isPair(box, upBox)) return true;
            else if (box.colNum + 2 < amtColumns)
            {
                if (isPair(box, upBox) && isPair(upBox, board.board[box.rowNum][box.colNum + 2]))
                {
                    upBox.isSequence = true;
                    board.board[box.rowNum][box.colNum + 2].isSequence = true;
                    return true;
                }
            }
        }
        if (box.colNum - 1 >= 0)
        {
            downBox = board.board[box.rowNum][box.colNum - 1];
            if (downBox.isSequence && isPair(box, downBox)) return true;
            else if (box.colNum - 2 >= 0)
            {
                if (isPair(box, downBox) && isPair(downBox, board.board[box.rowNum][box.colNum - 2]))
                {
                    downBox.isSequence = true;
                    board.board[box.rowNum][box.colNum - 2].isSequence = true;
                    return true;
                }
            }
        }
        if (box.rowNum + 1 < amtRows && box.colNum + 1 < amtColumns)
        {
            posDiagonal = board.board[box.rowNum + 1][box.colNum + 1];
            if (posDiagonal.isDiagonalSequence && isPair(box, posDiagonal)) return true;
            else if (box.rowNum + 2 < amtRows && box.colNum + 2 < amtColumns)
            {
                if (isPair(box, posDiagonal) && isPair(posDiagonal, board.board[box.rowNum + 2][box.colNum + 2]))
                {
                    posDiagonal.isDiagonalSequence = true;
                    board.board[box.rowNum + 2][box.colNum + 2].isDiagonalSequence = true;
                    box.isDiagonalSequence = true;
                    return true;
                }
            }
        }
        if (box.rowNum - 1 >= 0 && box.colNum - 1 >= 0)
        {
            negDiagonal = board.board[box.rowNum - 1][box.colNum - 1];
            if (negDiagonal.isDiagonalSequence && isPair(box, negDiagonal)) return true;
            else if (box.rowNum - 2 >= 0 && box.colNum - 2 >= 0)
            {
                if (isPair(box, negDiagonal) && isPair(negDiagonal, board.board[box.rowNum - 2][box.colNum - 2]))
                {
                    negDiagonal.isDiagonalSequence = true;
                    board.board[box.rowNum - 2][box.colNum - 2].isDiagonalSequence = true;
                    box.isDiagonalSequence = true;
                    return true;
                }
            }
        }
        if (box.rowNum + 1 < amtRows && box.colNum - 1 >= 0)
        {
            leftDiagonal = board.board[box.rowNum + 1][box.colNum - 1];
            if (leftDiagonal.isDiagonalSequence && isPair(box, leftDiagonal)) return true;
            else if (box.rowNum + 2 < amtRows && box.colNum - 2 >= 0)
            {
                if (isPair(box, leftDiagonal) && isPair(leftDiagonal, board.board[box.rowNum + 2][box.colNum - 2]))
                {
                    leftDiagonal.isDiagonalSequence = true;
                    board.board[box.rowNum + 2][box.colNum - 2].isDiagonalSequence = true;
                    box.isDiagonalSequence = true;
                    return true;
                }
            }
        }
        if (box.rowNum - 1 >= 0 && box.colNum + 1 < amtColumns)
        {
            rightDiagonal = board.board[box.rowNum - 1][box.colNum + 1];
            if (rightDiagonal.isDiagonalSequence && isPair(box, rightDiagonal)) return true;
            else if (box.rowNum - 2 >= 0 && box.colNum + 2 < amtColumns)
            {
                if (isPair(box, rightDiagonal) && isPair(rightDiagonal, board.board[box.rowNum - 2][box.colNum + 2]))
                {
                    rightDiagonal.isDiagonalSequence = true;
                    board.board[box.rowNum - 2][box.colNum + 2].isDiagonalSequence = true;
                    box.isDiagonalSequence = true;
                    return true;
                }
            }
        }
        if (isPair(box, upBox) && isPair(box, downBox))
        {           
            upBox.isSequence = true;
            downBox.isSequence = true;
            return true;
        }
        if (isPair(box, leftBox) && isPair(box, rightBox))
        {
            leftBox.isSequence = true;
            rightBox.isSequence = true;
            return true;
        }
        if (isPair(box, posDiagonal) && isPair(box, negDiagonal))
        {
            posDiagonal.isDiagonalSequence = true;
            negDiagonal.isDiagonalSequence = true;
            box.isDiagonalSequence = true;
            return true;
        }
        if (isPair(box, leftDiagonal) && isPair(box, rightDiagonal))
        {
            leftDiagonal.isDiagonalSequence = true;
            rightDiagonal.isDiagonalSequence = true;
            box.isDiagonalSequence = true;
            return true;
        }
        return false;
    }

    bool doesTrump(Piece piece1, Piece piece2)
    {
        if (piece1.type == Type.Rock && piece2.type == Type.Scissors) return true;
        if (piece1.type == Type.Scissors && piece2.type == Type.Paper) return true;
        if (piece1.type == Type.Paper && piece2.type == Type.Rock) return true;
        return false;
    }

    void checkForTrump(BoardBox box)
    {
        BoardBox barrierBox = null;
        if (box.rowNum + 2 < amtRows)
        {
            BoardBox right = board.board[box.rowNum + 1][box.colNum];
            if (right.boxContent == BoxContent.Piece)
            {
                if (right.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, right.currentPiece) && right.isSequence)
                {
                    BoardBox rightRight = board.board[box.rowNum + 2][box.colNum];
                    if (isPair(box, rightRight))
                    {
                        // change sprite of middle box to a barrier
                        right.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = right;
                        right.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score & fix booleans
                        right.isSequence = false;
                        while (right.colNum + 1 < amtColumns && board.board[right.rowNum][right.colNum + 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(right, board.board[right.rowNum][right.colNum + 1]))
                            {
                                --right.currentPiece.owner.score;
                                board.board[right.rowNum][right.colNum + 1].isSequence = false;
                            }
                            else { break; }
                            right = board.board[right.rowNum][right.colNum + 1];
                        }
                        right = board.board[box.rowNum + 1][box.colNum];
                        while (right.colNum - 1 >= 0 && board.board[right.rowNum][right.colNum - 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(right, board.board[right.rowNum][right.colNum - 1]))
                            {
                                --right.currentPiece.owner.score;
                                board.board[right.rowNum][right.colNum - 1].isSequence = false;
                            }
                            else { break; }
                            right = board.board[right.rowNum][right.colNum - 1];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
        if (box.rowNum - 2 >= 0)
        {
            BoardBox left = board.board[box.rowNum - 1][box.colNum];
            if (left.boxContent == BoxContent.Piece)
            {
                if (left.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, left.currentPiece) && left.isSequence)
                {
                    BoardBox leftLeft = board.board[box.rowNum - 2][box.colNum];
                    if (isPair(box, leftLeft))
                    {
                        // change sprite of middle box to a barrier
                        left.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = left;
                        left.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score
                        left.isSequence = false;
                        while (left.colNum + 1 < amtColumns && board.board[left.rowNum][left.colNum + 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(left, board.board[left.rowNum][left.colNum + 1]))
                            {
                                --left.currentPiece.owner.score;
                                board.board[left.rowNum][left.colNum + 1].isSequence = false;
                            }
                            else { break; }
                            left = board.board[left.rowNum][left.colNum + 1];
                        }
                        left = board.board[box.rowNum - 1][box.colNum];
                        while (left.colNum - 1 >= 0 && board.board[left.rowNum][left.colNum - 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(left, board.board[left.rowNum][left.colNum - 1]))
                            {
                                --left.currentPiece.owner.score;
                                board.board[left.rowNum][left.colNum - 1].isSequence = false;
                            }
                            else { break; }
                            left = board.board[left.rowNum][left.colNum - 1];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
        if (box.colNum + 2 < amtColumns)
        {
            BoardBox up = board.board[box.rowNum][box.colNum + 1];
            if (up.boxContent == BoxContent.Piece)
            {
                if (up.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, up.currentPiece) && up.isSequence)
                {
                    BoardBox upUp = board.board[box.rowNum][box.colNum + 2];
                    if (isPair(box, upUp))
                    {
                        // change sprite of middle box to a barrier
                        up.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = up;
                        up.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score
                        up.isSequence = false;
                        while (up.rowNum + 1 < amtRows && board.board[up.rowNum + 1][up.colNum].boxContent != BoxContent.Empty)
                        {
                            if (isPair(up, board.board[up.rowNum + 1][up.colNum]))
                            {
                                --up.currentPiece.owner.score;
                                board.board[up.rowNum + 1][up.colNum].isSequence = false;
                            }
                            else { break; }
                            up = board.board[up.rowNum + 1][up.colNum];
                        }
                        up = board.board[box.rowNum][box.colNum + 1];
                        while (up.rowNum - 1 >= 0 && board.board[up.rowNum - 1][up.colNum].boxContent != BoxContent.Empty)
                        {
                            if (isPair(up, board.board[up.rowNum - 1][up.colNum]))
                            {
                                --up.currentPiece.owner.score;
                                board.board[up.rowNum - 1][up.colNum].isSequence = false;
                            }
                            else { break; }
                            up = board.board[up.rowNum - 1][up.colNum];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
        if (box.colNum - 2 >= 0)
        {
            BoardBox down = board.board[box.rowNum][box.colNum - 1];
            if (down.boxContent == BoxContent.Piece)
            {
                if (down.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, down.currentPiece) && down.isSequence)
                {
                    BoardBox downDown = board.board[box.rowNum][box.colNum - 2];
                    if (isPair(box, downDown))
                    {
                        // change sprite of middle box to a barrier
                        down.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = down;
                        down.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score
                        down.isSequence = false;
                        while (down.rowNum + 1 < amtRows && board.board[down.rowNum + 1][down.colNum].boxContent != BoxContent.Empty)
                        {
                            if (isPair(down, board.board[down.rowNum + 1][down.colNum]))
                            {
                                --down.currentPiece.owner.score;
                                board.board[down.rowNum + 1][down.colNum].isSequence = false;
                            }
                            else { break; }
                            down = board.board[down.rowNum + 1][down.colNum];
                        }
                        down = board.board[box.rowNum][box.colNum - 1];
                        while (down.colNum - 1 >= 0 && board.board[down.rowNum - 1][down.colNum].boxContent != BoxContent.Empty)
                        {
                            if (isPair(down, board.board[down.rowNum - 1][down.colNum]))
                            {
                                --down.currentPiece.owner.score;
                                board.board[down.rowNum - 1][down.colNum].isSequence = false;
                            }
                            else { break; }
                            down = board.board[down.rowNum - 1][down.colNum];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
        // diagonal cases
        if (box.rowNum + 2 < amtRows && box.colNum - 2 >= 0)
        {
            BoardBox next = board.board[box.rowNum + 1][box.colNum - 1];
            if (next.boxContent == BoxContent.Piece)
            {
                if (next.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, next.currentPiece) && next.isDiagonalSequence)
                {
                    BoardBox nextNext = board.board[box.rowNum + 2][box.colNum - 2];
                    if (isPair(box, nextNext))
                    {
                        // change sprite of middle box to a barrier
                        next.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = next;
                        next.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score
                        next.isDiagonalSequence = false;
                        while (next.rowNum + 1 < amtRows && next.colNum + 1 < amtColumns
                               && board.board[next.rowNum + 1][next.colNum + 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum + 1][next.colNum + 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum + 1][next.colNum+1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum + 1][next.colNum+1];
                        }
                        next = board.board[box.rowNum + 1][box.colNum - 1];
                        while (next.rowNum - 1 >= 0 && next.colNum - 1 >= 0
                               && board.board[next.rowNum - 1][next.colNum - 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum - 1][next.colNum - 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum - 1][next.colNum - 1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum - 1][next.colNum - 1];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
        if (box.rowNum - 2 >= 0 && box.colNum + 2 < amtColumns)
        {
            BoardBox next = board.board[box.rowNum - 1][box.colNum + 1];
            if (next.boxContent == BoxContent.Piece)
            {
                if (next.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, next.currentPiece) && next.isDiagonalSequence)
                {
                    BoardBox nextNext = board.board[box.rowNum - 2][box.colNum + 2];
                    if (isPair(box, nextNext))
                    {
                        // change sprite of middle box to a barrier
                        next.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = next;
                        next.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score
                        next.isDiagonalSequence = false;
                        while (next.rowNum + 1 < amtRows && next.colNum + 1 < amtColumns
                               && board.board[next.rowNum + 1][next.colNum + 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum + 1][next.colNum + 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum + 1][next.colNum + 1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum + 1][next.colNum + 1];
                        }
                        next = board.board[box.rowNum - 1][box.colNum + 1];
                        while (next.rowNum - 1 >= 0 && next.colNum - 1 >= 0
                               && board.board[next.rowNum - 1][next.colNum - 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum - 1][next.colNum - 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum - 1][next.colNum - 1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum - 1][next.colNum - 1];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
        if (box.rowNum - 2 >= 0 && box.colNum - 2 >= 0)
        {
            BoardBox next = board.board[box.rowNum - 1][box.colNum - 1];
            if (next.boxContent == BoxContent.Piece)
            {
                if (next.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, next.currentPiece) && next.isDiagonalSequence)
                {
                    BoardBox nextNext = board.board[box.rowNum - 2][box.colNum - 2];
                    if (isPair(box, nextNext))
                    {
                        // change sprite of middle box to a barrier
                        next.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = next;
                        next.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score
                        next.isDiagonalSequence = false;
                        while (next.rowNum + 1 < amtRows && next.colNum - 1 >= 0
                               && board.board[next.rowNum + 1][next.colNum - 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum + 1][next.colNum - 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum + 1][next.colNum - 1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum + 1][next.colNum - 1];
                        }
                        next = board.board[box.rowNum - 1][box.colNum - 1];
                        while (next.rowNum - 1 >= 0 && next.colNum + 1 < amtColumns
                               && board.board[next.rowNum - 1][next.colNum + 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum - 1][next.colNum + 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum - 1][next.colNum + 1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum - 1][next.colNum + 1];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
        if (box.rowNum + 2 < amtRows && box.colNum + 2 < amtColumns)
        {
            BoardBox next = board.board[box.rowNum + 1][box.colNum + 1];
            if (next.boxContent == BoxContent.Piece)
            {
                if (next.currentPiece.owner != currentPlayer && doesTrump(box.currentPiece, next.currentPiece) && next.isDiagonalSequence)
                {
                    BoardBox nextNext = board.board[box.rowNum + 2][box.colNum + 2];
                    if (isPair(box, nextNext))
                    {
                        // change sprite of middle box to a barrier
                        next.GetComponent<Image>().sprite = barrierSprite;
                        barrierBox = next;
                        next.currentPiece.owner.score -= 2;
                        // refactor the opposing player's score
                        next.isDiagonalSequence = false;
                        while (next.rowNum + 1 < amtRows && next.colNum - 1 >= 0
                               && board.board[next.rowNum + 1][next.colNum - 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum + 1][next.colNum - 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum + 1][next.colNum - 1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum + 1][next.colNum - 1];
                        }
                        next = board.board[box.rowNum + 1][box.colNum + 1];
                        while (next.rowNum - 1 >= 0 && next.colNum + 1 < amtColumns
                               && board.board[next.rowNum - 1][next.colNum + 1].boxContent != BoxContent.Empty)
                        {
                            if (isPair(next, board.board[next.rowNum - 1][next.colNum + 1]))
                            {
                                --next.currentPiece.owner.score;
                                board.board[next.rowNum - 1][next.colNum + 1].isDiagonalSequence = false;
                            }
                            else { break; }
                            next = board.board[next.rowNum - 1][next.colNum + 1];
                        }
                        barrierBox.boxContent = BoxContent.Barrier;
                        barrierBox.currentPiece = null;
                        return;
                    }
                }
            }
        }
    }

    public bool checkGameOver()
    {
        for (int i = 0; i < amtRows; ++i)
        {
            for (int j = 0; j < amtColumns; ++j)
            {
                if (board.board[i][j].boxContent == BoxContent.Empty)
                {
                    //Debug.Log("not over yet");
                    return false;
                }
                    
            }
        }
        Debug.Log("Game Over");
        status = GameStatus.Finish;
        return true;
    }

    //a function to deal with game over things
    //bascially just compare score and return  to UImanager which player won
    //return 0 for palyer 1, 1 for palyer 2, 2 for draw
    public int gameOver()
    {
        if (player1.score > player2.score)
            return 0;
        else if (player1.score < player2.score)
            return 1;
        else
            return 2;
    }
    public void processPlay(BoardBox box)
    {
        // check if player played three in a row
        if (checkForSequence(box))
        {
            // played a sequence, factor multiplier in
            box.isSequence = true;
            currentPlayer.score -= 3;
            currentPlayer.score += 6;
        }

        // check if player trumped other player
        checkForTrump(box);

        // update score UI
        UIManager.instance.p1ScoreText.text = "P1: " + player1.score;
        UIManager.instance.p2ScoreText.text = "P2: " + player2.score;

        // swap current player
        turn();
    }

    //Program start
    //mainly for board set up
    void Start()
    {

        // based on the amt of rows and columns, change the position of the board to make sure it's centered
        board.gameObject.transform.position = new Vector3 (Screen.width*0.5f,Screen.height*0.5f,0.0f);
        status = GameStatus.Idle;
        setupBoard();
       
    }

    //Game start.(After player hit StartGame button)
    //call by UIManager
    public void startGame()
    {
        resetBoard();
        resetBox();
        player1.score = 0;
        player2.score = 0;
        timer.globalStartTime = Time.fixedTime;
        status = GameStatus.SetBarrier;
        currentPlayer = GameManager.instance.player1;
        currentTypeMode = Type.Barrier;
        countDownTimer.setTime((int)barrierTime);
        countBarrier = 0;
    }
    void initialFightStatus()
    {
        //TBC
        //1.process barriers

        setBarriers();
        turn();
        uimanager.reAbleButtons();
        currentTypeMode = Type.Rock;
        uimanager.OnClickModeButton("rock");
    }

    void turn()
    {
        if (currentPlayer==player1)
        {
            Camera.main.backgroundColor = new Color32(0xff, 0x65, 0x62, 0xff);
            currentPlayer = player2;
            //Debug.Log(currentPlayer);
        }
        else
        {
            Camera.main.backgroundColor = new Color32(0x31, 0x4d, 0x79, 0xff); 
            currentPlayer = player1;
            //Debug.Log(currentPlayer);
        }

        //reset timer
        if (status == GameStatus.SetBarrier)
            countDownTimer.setTime((int)barrierTime);
        else if (status == GameStatus.Fight)
            countDownTimer.setTime((int)everyTurnMaxTime);
    }

    //this function happen once when first player finish setting berriers
    //this function clear all barrier images and boxcontent on board
    void resetBoard()
    {
        for (int i = 0; i < amtRows; ++i)
        {
            for (int j = 0; j < amtColumns; ++j)
            {
                if (board.board[i][j].boxContent != BoxContent.Empty)
                {
                    board.board[i][j].boxContent = BoxContent.Empty;
                    board.board[i][j].GetComponent<Image>().sprite = pieceSprites[7];
                }
            }
        }
    }
    //only use when restart the game
    void resetBox()
    {
        for (int i = 0; i < amtRows; ++i)
        {
            for (int j = 0; j < amtColumns; ++j)
            {
                board.board[i][j].init();
                //Debug.Log("we cool here");
            }
        }
    }

    //this function happen once when Barrier status end
    //process all barriers and decide which box is barrier(set images and boxcontent)
    void setBarriers()
    {
        for (int i = 0; i < amtRows; ++i)
        {
            for (int j = 0; j < amtColumns; ++j)
            {
                if (board.board[i][j].barrierInfo[0] || board.board[i][j].barrierInfo[1])
                {
                    board.board[i][j].boxContent = BoxContent.Barrier;
                    board.board[i][j].GetComponent<Image>().sprite = pieceSprites[6];
                }
            }
        }
    }

    //see FixedUpdate function
    void Update()
    {
        
    }

    //main update function
    void FixedUpdate()
    {
       
       if(status == GameStatus.SetBarrier)
        {
            //player 2's turn to set barrier 
            if(Time.fixedTime - timer.globalStartTime > barrierTime && currentPlayer == player1)
            {
                //should only get into there once
                countBarrier = 0;
                resetBoard();
                turn();

            }
            //From Set Barrier mode to fight mode, only enter once
            else if (Time.fixedTime - timer.globalStartTime > 2 * barrierTime)
            {
                status = GameStatus.Fight;
                Debug.Log("Status: Fight");
                initialFightStatus();
            }
            else
            {

            }
        }
        else if (status == GameStatus.Fight)
        {
            if (countDownTimer.timeLeft == 0)
                turn();
        }
        else
        {

        }
    }
    
    
}
