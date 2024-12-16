using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    [SerializeField] private Image Image;
    [SerializeField] private TextMeshProUGUI pokemonName, level;
    [SerializeField] private HealtBar healtBar;

    [SerializeField] private GameObject selectedBacground;
    [SerializeField] private GameObject deselectedBacground;
    [SerializeField] private GameObject pokemonData;

    private Pokemon _pokemon;

    [SerializeField] private bool isFirst;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        Image.sprite = pokemon.Base.FrontSprite;
        pokemonName.text = pokemon.Base.PokemonName;
        level.text = $"LV  {pokemon.Level}";
        healtBar.SetHP(pokemon.HP,pokemon.maxHP);
        
        pokemonData.SetActive(true);
        if(isFirst)
        {
            selectedBacground.SetActive(true);
            deselectedBacground.SetActive(false);
        }
        else
        {
            selectedBacground.SetActive(false);
            deselectedBacground.SetActive(true);
        }
    }

    public void NoPokemon()
    {
        selectedBacground.SetActive(false);
        deselectedBacground.SetActive(false);
        pokemonData.SetActive(false);
    }

    public void Select()
    {
        selectedBacground.SetActive(true);
        deselectedBacground.SetActive(false);
    }

    public void Deselect()
    {
        selectedBacground.SetActive(false);
        deselectedBacground.SetActive(true);
    }

}
