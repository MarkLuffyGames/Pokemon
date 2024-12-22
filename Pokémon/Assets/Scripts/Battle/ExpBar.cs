using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Image exptBar;

    [SerializeField] private float currentExp;

    [Tooltip("Tiempo que demora en rellenar la barra al completo")]
    [SerializeField] private float updateSpeed = 3;

    public void SetExp(float currentExp, int necessaryExp, int necessaryExpToNext)
    {
        this.currentExp = currentExp;

        exptBar.fillAmount = (this.currentExp - necessaryExp) / (necessaryExpToNext - necessaryExp);
    }
    public IEnumerator UpdateExp(int currentExp, int necessaryExp, int necessaryExpToNext)
    {
        while (this.currentExp < currentExp)
        {
            this.currentExp += Time.deltaTime * ((necessaryExpToNext - necessaryExp) / updateSpeed);
            SetExp(this.currentExp, necessaryExp, necessaryExpToNext);
            yield return new WaitForEndOfFrame();
        }

        this.currentExp = currentExp;
    }
}
