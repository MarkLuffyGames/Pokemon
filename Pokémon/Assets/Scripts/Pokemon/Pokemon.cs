using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private IVStats ivStats;
    [SerializeField] private EVStats evStats;
    [SerializeField] private Nature nature;
    [SerializeField] private int level;

    public PokemonBase Base => _base;
    public int Level
    {
        get => level;
        set => level = value;
    }

    [SerializeField] private List<Move> moves;
    public List<Move> Moves
    {
        get => moves; 
        set => moves = value;
    }

    private int hp;
    public int HP
    {
        get => hp;
        set => hp = value;
    }

    private int experience;
    public int Experience
    {
        get => experience;
        set => experience = value;
    }

    public Pokemon(PokemonBase @base, int level)
    {
        _base = @base;
        this.level = level;

        InitPokemon();
    }

    public void InitPokemon()
    {
        nature = (Nature)Random.Range(0, Enum.GetValues(typeof(Nature)).Length);

        ivStats = GenerateIVs();

        evStats = new EVStats
        {
            HP = 0,
            Attack = 0,
            Defense = 0,
            SpecialAttack = 0,
            SpecialDefense = 0,
            Speed = 0
        };

        hp = maxHP;
        experience = Base.GetNecessaryExpForNextLevel(level);

        moves = new List<Move>();

        foreach (var lMove in _base.LearnableMoves)
        {
            if (lMove.Level <= level)
            {
                moves.Add(new Move(lMove.Move));
            }

            if (moves.Count > 3)
            {
                break;
            }
        }
    }

    public DamageDescription RecibeDamage(Move move, Pokemon pokemon)
    {
        float typeEffectiveness = PokemonTypeChart.GetEffectiveness(move.MoveBase.MoveType, _base.Type1)
            * PokemonTypeChart.GetEffectiveness(move.MoveBase.MoveType, _base.Type2);

        float STAB = move.MoveBase.MoveType == pokemon.Base.Type1 || move.MoveBase.MoveType == pokemon.Base.Type2
            ? 1.5f : 1.0f;
        float random = Random.Range(85, 101) / 100f;

        float critical = Random.Range(0, 100) < 6 ? 1.5f : 1.0f;

        float modifier = typeEffectiveness * STAB * random * critical;


        float damageBase = (2 * pokemon.Level / 5f + 2) * move.MoveBase.Power * (move.MoveBase.MoveClass == MoveClass.Physical ? pokemon.Attack / (float)Defense : pokemon.SpecialAttack / (float)SpecialDefense) / 50f + 2;
        int damge = Mathf.FloorToInt(damageBase * modifier);

        string returnType = "";
        if(typeEffectiveness == 0)
        {
            returnType = "inmune";
        }
        else if(typeEffectiveness == 0.25f)
        {
            returnType = "muy poco efectivo";
        }
        else if (typeEffectiveness == 0.5f)
        {
            returnType = "poco efectivo";
        }
        else if (typeEffectiveness == 1.0f)
        {
            returnType = "";
        }
        else if (typeEffectiveness == 2.0f)
        {
            returnType = "efectivo";
        }
        else if (typeEffectiveness == 4.0f)
        {
            returnType = "muy efectivo";
        }


        HP -= damge;
        if(HP <= 0)
        {
            HP = 0;
            return new DamageDescription(critical == 1.5f ? true : false, true, returnType);
        }

        return new DamageDescription(critical == 1.5f ? true : false, false, returnType); ;
    }

    public int maxHP => CalculateHP();
    public int Attack => CalculateStat(_base.Attack, ivStats.Attack, evStats.Attack, NatureMultiplier(nature, "Attack"));
    public int Defense => CalculateStat(_base.Defense, ivStats.Defense, evStats.Defense, NatureMultiplier(nature, "Defense"));
    public int SpecialAttack => CalculateStat(_base.SpAttack, ivStats.SpecialAttack, evStats.SpecialAttack, NatureMultiplier(nature, "SpecialAttack"));
    public int SpecialDefense => CalculateStat(_base.SpDefense, ivStats.SpecialDefense, evStats.SpecialDefense, NatureMultiplier(nature, "SpecialDefense"));
    public int Speed => CalculateStat(_base.Speed, ivStats.Speed, evStats.Speed, NatureMultiplier(nature, "Speed"));



    private int CalculateHP()
    {
        return ((2 * _base.MaxHP + ivStats.HP + (evStats.HP / 4)) * Level) / 100 + Level + 10;
    }

    private int CalculateStat(int baseStat, int iv, int ev, float natureMultiplier)
    {
        return (int)((((2 * baseStat + iv + (ev / 4)) * Level) / 100 + 5) * natureMultiplier);
    }

    private float NatureMultiplier(Nature nature, string statName)
    {
        switch (nature)
        {
            case Nature.Hardy: return 1.0f;
            case Nature.Lonely: return statName == "Attack" ? 1.1f : statName == "Defense" ? 0.9f : 1.0f;
            case Nature.Brave: return statName == "Attack" ? 1.1f : statName == "Speed" ? 0.9f : 1.0f;
            case Nature.Adamant: return statName == "Attack" ? 1.1f : statName == "SpecialAttack" ? 0.9f : 1.0f;
            case Nature.Naughty: return statName == "Attack" ? 1.1f : statName == "SpecialDefense" ? 0.9f : 1.0f;
            case Nature.Bold: return statName == "Defense" ? 1.1f : statName == "Attack" ? 0.9f : 1.0f;
            case Nature.Docile: return 1.0f;
            case Nature.Relaxed: return statName == "Defense" ? 1.1f : statName == "Speed" ? 0.9f : 1.0f;
            case Nature.Impish: return statName == "Defense" ? 1.1f : statName == "SpecialAttack" ? 0.9f : 1.0f;
            case Nature.Lax: return statName == "Defense" ? 1.1f : statName == "SpecialDefense" ? 0.9f : 1.0f;
            case Nature.Timid: return statName == "Speed" ? 1.1f : statName == "Attack" ? 0.9f : 1.0f;
            case Nature.Hasty: return statName == "Speed" ? 1.1f : statName == "Defense" ? 0.9f : 1.0f;
            case Nature.Serious: return 1.0f;
            case Nature.Jolly: return statName == "Speed" ? 1.1f : statName == "SpecialAttack" ? 0.9f : 1.0f;
            case Nature.Naive: return statName == "Speed" ? 1.1f : statName == "SpecialDefense" ? 0.9f : 1.0f;
            case Nature.Modest: return statName == "SpecialAttack" ? 1.1f : statName == "Attack" ? 0.9f : 1.0f;
            case Nature.Mild: return statName == "SpecialAttack" ? 1.1f : statName == "Defense" ? 0.9f : 1.0f;
            case Nature.Quiet: return statName == "SpecialAttack" ? 1.1f : statName == "Speed" ? 0.9f : 1.0f;
            case Nature.Bashful: return 1.0f;
            case Nature.Rash: return statName == "SpecialAttack" ? 1.1f : statName == "SpecialDefense" ? 0.9f : 1.0f;
            case Nature.Calm: return statName == "SpecialDefense" ? 1.1f : statName == "Attack" ? 0.9f : 1.0f;
            case Nature.Gentle: return statName == "SpecialDefense" ? 1.1f : statName == "Defense" ? 0.9f : 1.0f;
            case Nature.Sassy: return statName == "SpecialDefense" ? 1.1f : statName == "Speed" ? 0.9f : 1.0f;
            case Nature.Careful: return statName == "SpecialDefense" ? 1.1f : statName == "SpecialAttack" ? 0.9f : 1.0f;
            case Nature.Quirky: return 1.0f;
            default: return 1.0f;
        }
    }


    private IVStats GenerateIVs()
    {
        var IVs = new int[6];
        for (int i = 0; i < IVs.Length; i++)
        {
            IVs[i] = Random.Range(0, 32);
        }

        IVStats iv = new IVStats
        {
            HP = IVs[0],
            Attack = IVs[1],
            Defense = IVs[2],
            SpecialAttack = IVs[3],
            SpecialDefense = IVs[4],
            Speed = IVs[5]
        };

        return iv;
    }

    public void AddEVs(int hp = 0, int attack = 0, int defense = 0, int specialAttack = 0, int specialDefense = 0, int speed = 0)
    {
        evStats.HP += hp;
        evStats.Attack += attack;
        evStats.Defense += defense;
        evStats.SpecialAttack += specialAttack;
        evStats.SpecialDefense += specialDefense;
        evStats.Speed += speed;
    }
}

[Serializable]
public class IVStats
{
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SpecialAttack { get; set; }
    public int SpecialDefense { get; set; }
    public int Speed { get; set; }
}

[Serializable]
public class EVStats
{
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SpecialAttack { get; set; }
    public int SpecialDefense { get; set; }
    public int Speed { get; set; }
}

public enum Nature
{
    Hardy,
    Lonely,
    Brave,
    Adamant,
    Naughty,
    Bold,
    Docile,
    Relaxed,
    Impish,
    Lax,
    Timid,
    Hasty,
    Serious,
    Jolly,
    Naive,
    Modest,
    Mild,
    Quiet,
    Bashful,
    Rash,
    Calm,
    Gentle,
    Sassy,
    Careful,
    Quirky
}

public class DamageDescription
{
    public bool Critical;
    public bool IsFainted;
    public string type;

    public DamageDescription(bool critical, bool isFainted, string type)
    {
        Critical = critical;
        IsFainted = isFainted;
        this.type = type;
    }
}
