using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public bool isPlayer;

    private Image pokemonImage;
    private Vector2 initPosition;
    private Color initialColor;
    [SerializeField]private BattleHUD battleHUD;

    public Pokemon pokemon;

    public BattleHUD BattleHUD => battleHUD;

    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        initPosition = transform.localPosition;
        initialColor = pokemonImage.color;
    }
    public void SetupPokemon(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        pokemonImage.color = initialColor;
        transform.localScale = Vector3.one;
        pokemonImage.sprite = isPlayer ? pokemon.Base.BackSprite : pokemon.Base.FrontSprite;
        AnimationStartBattle();

        battleHUD.SetPokemonData(pokemon);
    }

    public void AnimationStartBattle()
    {
        transform.localPosition = new Vector2(initPosition.x + (isPlayer? -1:1) * 400, initPosition.y);

        pokemonImage.transform.DOLocalMoveX(initPosition.x, 1);
    }

    public void AnimationAttack()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveX(initPosition.x + (isPlayer ? 1 : -1) * 50, 0.2f));
        seq.Append(pokemonImage.transform.DOLocalMoveX(initPosition.x, 0.2f));
    }

    public void AnimationRecibeDamage()
    {
        var seq = DOTween.Sequence();
        

        for (int i = 0; i < 3 ; i++)
        {
            seq.Append(pokemonImage.DOColor(Color.grey, 0.15f));
            seq.Append(pokemonImage.DOColor(initialColor, 0.05f));
        }
    }

    public void AnimationFainted()
    {
        var seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMoveY(initPosition.y - 200, 1));
        seq.Join(pokemonImage.DOFade(0, 1));
    }

    public IEnumerator CapturedAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOFade(0, 0.5f));
        seq.Join(transform.DOScale(new Vector3(0.25f, 0.25f, 1), 0.5f));
        seq.Join(transform.DOLocalMoveY(initPosition.y + 50f, 0.5f));
        yield return seq.WaitForCompletion();
    }

    public IEnumerator BreakOutAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOFade(1, 0.5f));
        seq.Join(transform.DOScale(new Vector3(1, 1, 1), 0.5f));
        seq.Join(transform.DOLocalMoveY(initPosition.y, 0.5f));
        yield return seq.WaitForCompletion();
    }
}
