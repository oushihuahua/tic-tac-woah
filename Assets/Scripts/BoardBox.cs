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

    // variable to reference which player has this board piece
    [System.NonSerialized]
    public Piece ownedByThisPlayer;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            UIManager.instance.OnClickBox(gameObject.GetComponent<BoardBox>());
        });


        // variable to reference what type (rock, paper, scissor) is on this board piece
    }
}
