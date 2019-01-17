using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardBox : MonoBehaviour
{
    //[System.NonSerialized]
    public int rowNum;
    //[System.NonSerialized]
    public int colNum;
    //[System.NonSerialized]
    public bool playable;

    // variable to reference which player has this board piece
    [System.NonSerialized]
    public Piece ownedByThisPlayer;

    // variable to reference what type (rock, paper, scissor) is on this board piece
}
