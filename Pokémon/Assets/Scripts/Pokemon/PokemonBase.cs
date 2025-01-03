using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pok�mon", menuName = "Pok�mon/New Pok�mon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private int ID;
    [SerializeField] private string pokemonName;
    [TextArea][SerializeField] private string description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;

    [SerializeField] private int catchRate;
    [SerializeField] private int baseExp;
    [SerializeField] private GrowthRate growthRate;

    //Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefence;
    [SerializeField] private int speed;

    //Moves
    [SerializeField] private List<LearnableMoves> learnableMoves;


    //public

    public string PokemonName => pokemonName;
    public string Description => description;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public PokemonType Type1 => type1;
    public PokemonType Type2 => type1;
    public int CatchRate => catchRate;
    public int BaseExp => baseExp;
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defense => defence;
    public int SpAttack => spAttack;
    public int SpDefense => spDefence;
    public int Speed => speed;

    public List<LearnableMoves> LearnableMoves => learnableMoves;


    public int GetNecessaryExpForLevel(int level)
    {
        switch (growthRate)
        {
            case GrowthRate.Erratic:
                if(level < 50)
                    return Mathf.FloorToInt((Mathf.Pow(level, 3) * (100 - level)) / 50);
                else if(level < 68)
                    return Mathf.FloorToInt((Mathf.Pow(level, 3) * (150 - level)) / 100);
                else if(level < 98)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((1911 - level*10) / 3) / 500);
                else
                    return Mathf.FloorToInt((Mathf.Pow(level, 3) * (160 - level)) / 100);

            case GrowthRate.Fast:
                return Mathf.FloorToInt(4 * Mathf.Pow(level, 3) / 5);

            case GrowthRate.MediumFast:
                return Mathf.FloorToInt(Mathf.Pow(level, 3));

            case GrowthRate.MediumSlow:
                return Mathf.FloorToInt(6 * Mathf.Pow(level, 3) / 5 - 15 * Mathf.Pow(level, 2) + 100 * level - 140);

            case GrowthRate.Slow:
                return Mathf.FloorToInt(5 * Mathf.Pow(level, 3) / 4);

            case GrowthRate.Fluctuating:
                if (level < 15)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 1) / 3 + 24) / 50);
                else if (level < 36)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (level + 14) / 50);
                else
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (level / 2 + 32) / 50);
        }
        return -1;
    }
}

public enum GrowthRate
{
    Erratic, Fast, MediumFast, MediumSlow, Slow, Fluctuating
}

public enum Stat
{
    Attack, Defense, SpAttack, SpDefense, Speed
}
public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public class PokemonTypeChart
{
    private static readonly float[,] typeChart =
    {
        //         NON NOR FIR WAT ELE GRA ICE FIG POI GRO FLY PSY BUG ROC GHO DRA DAR STE FAI
        /* NON */ { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /* NOR */ { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f },
        /* FIR */ { 1f, 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f },
        /* WAT */ { 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f },
        /* ELE */ { 1f, 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f },
        /* GRA */ { 1f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f },
        /* ICE */ { 1f, 1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f },
        /* FIG */ { 1f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f },
        /* POI */ { 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0.5f, 1f, 0.5f, 1f, 1f, 0f, 2f },
        /* GRO */ { 1f, 1f, 2f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f },
        /* FLY */ { 1f, 1f, 1f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f },
        /* PSY */ { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f },
        /* BUG */ { 1f, 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 1f, 0.5f },
        /* ROC */ { 1f, 1f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f },
        /* GHO */ { 1f, 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f },
        /* DRA */ { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f },
        /* DAR */ { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 1f, 0.5f, 1f, 2f },
        /* STE */ { 1f, 1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f },
        /* FAI */ { 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f }
    };

    public static float GetEffectiveness(PokemonType attacker, PokemonType defender)
    {
        return typeChart[(int)attacker, (int)defender];
    }
}

[Serializable]
public class LearnableMoves
{
    [SerializeField] private MoveBase move;
    [SerializeField] private int level;

    public MoveBase Move => move;
    public int Level => level;
}
