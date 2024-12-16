using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyHUD : MonoBehaviour
{
    [SerializeField] private PartyMemberHUD[] partyMemberHUDs;
    [SerializeField] private TextMeshProUGUI text;

    private int pokemonsCount;

    public void SetPartyData(List<Pokemon> pokemons)
    {
        text.text = "Selecciona un Pokémon.";

        pokemonsCount = pokemons.Count;

        for (int i = 0; i < partyMemberHUDs.Length; i++)
        {
            if(i < pokemons.Count)
            {
                partyMemberHUDs[i].SetPokemonData(pokemons[i]);
            }
            else
            {
                partyMemberHUDs[i].NoPokemon();
            }
        }
    }

    public void SelectPokemon(int select)
    {
        for (int i = 0; i < pokemonsCount; i++)
        {
            partyMemberHUDs[i].Deselect();
        }

        partyMemberHUDs[select].Select();
    }
}
