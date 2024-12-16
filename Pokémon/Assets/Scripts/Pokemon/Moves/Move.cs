using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private MoveBase moveBase;
    private int powerPoints;

    public MoveBase MoveBase
    {
        get => moveBase; 
        set => moveBase = value; 
    }

    public int PowerPoints
    {
        get => powerPoints;
        set => powerPoints = value;
    }

    public Move(MoveBase move)
    {
        moveBase = move;
        powerPoints = move.PP;
    }
}
