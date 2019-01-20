using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Empty: can place barrier in the begining or place piece
* Barrier: won't change in the future
* Piece: can change into barrier
*/
public enum BoxContent { Empty, Barrier, Piece };

public class BoardBox : MonoBehaviour
{
    //[System.NonSerialized]
    public int rowNum;
    //[System.NonSerialized]
    public int colNum;
    //[System.NonSerialized]
    public BoxContent boxContent;
    public bool isSequence;
    public bool isDiagonalSequence;
    // variable to reference which piece is on this box
    [System.NonSerialized]
    public Piece currentPiece;
    //small structure help to store barrier info in the Barrier status
    public bool[] barrierInfo;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            UIManager.instance.OnClickBox(gameObject.GetComponent<BoardBox>());
        });
        barrierInfo = new bool[2] {false,false };

        isSequence = false;
        isDiagonalSequence = false;
        currentPiece = null;
    }
}
