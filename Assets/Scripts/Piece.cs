using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Type { Rock, Paper, Scissors, Barrier };

/*
 * rock x scissors
 * paper x rock
 * scissors x paper
 */

public class Piece : MonoBehaviour
{
    //[System.NonSerialized]
    public Type type;
    Player owner;
    Image image;

    void Awake()
    {
        owner = GameManager.instance.currentPlayer;
        type = GameManager.instance.currentTypeMode;
    }
}
