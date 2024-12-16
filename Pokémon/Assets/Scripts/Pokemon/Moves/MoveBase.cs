using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokémon/New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string moveName;
    [TextArea][SerializeField] private string description;

    [SerializeField] private PokemonType moveType;
    [SerializeField] private MoveClass moveClass;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;
    [SerializeField] private bool priority = false;

    public string MoveName => moveName;
    public string Description => description;
    public PokemonType MoveType => moveType;
    public MoveClass MoveClass => moveClass;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;
    public bool Priority => priority;
}

public enum MoveClass
{
    Physical,
    Special,
    Status
}
