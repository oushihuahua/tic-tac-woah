using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { Rock, Paper, Scissors };

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

    void Start()
    {
        // set up sprite based on the type    
    }
}
