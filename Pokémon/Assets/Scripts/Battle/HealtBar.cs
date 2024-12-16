using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealtBar : MonoBehaviour
{
    [SerializeField] private Image healtBar;
    [SerializeField] private TextMeshProUGUI healtNumber;


    [SerializeField]private int MaxHP;
    [SerializeField]private float currentHP;

    [SerializeField] private float updateSpeed;
    [SerializeField] private Sprite[] sprites;

    public void SetHP(float hp, int maxHP)
    {
        currentHP = hp;
        MaxHP = maxHP;

        healtBar.fillAmount = currentHP / MaxHP;
        if(healtNumber != null) healtNumber.text = $"{(int)currentHP}/{MaxHP}";

        if(healtBar.fillAmount >= 0.5f)
        {
            healtBar.sprite = sprites[0];
        }
        else if(healtBar.fillAmount <= 0.15f)
        {
            healtBar.sprite = sprites[2];
        }
        else
        {
            healtBar.sprite = sprites[1];
        }
    }

    public IEnumerator UpdateHealt(int hp)
    {

        while(currentHP > hp)
        {
            currentHP -= Time.deltaTime * 10;
            SetHP(currentHP < hp ? hp:currentHP, MaxHP);
            yield return null;
        }
        
        while(currentHP < hp)
        {
            currentHP += Time.deltaTime;
            SetHP(currentHP, MaxHP);
            yield return null;
        }

        currentHP = hp;
    }
}
