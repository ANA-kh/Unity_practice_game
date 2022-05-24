using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates : MonoBehaviour
{
    public event Action<int,int> UpdateHealthBarAttack;
    public CharacterData_SO templateData;
    public AttackData_SO attackData;
    public CharacterData_SO characterData;
    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if(templateData !=null)
            characterData = Instantiate(templateData);
    }

    #region  Read from Data_SO
    public int MaxHealth
    {
        get{ if(characterData !=null)return characterData.maxHealth;else return 0; }
        set{ characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get{ if(characterData !=null)return characterData.currentHealth;else return 0; }
        set{ characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get{ if(characterData !=null)return characterData.baseDefence;else return 0; }
        set{ characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get{ if(characterData !=null)return characterData.currentDefence;else return 0; }
        set{ characterData.currentDefence = value; }
    }
    #endregion
    

    public void TakeDamage(CharacterStates attacker, CharacterStates defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth -damage,0);

        if(attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO: UI   经验
        UpdateHealthBarAttack?.Invoke(CurrentHealth,MaxHealth);
    }

    public void TakeDamage(int damage, CharacterStates defener)
    {
        int coretDamage = UnityEngine.Random.Range(damage - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth -coretDamage,0);
        UpdateHealthBarAttack?.Invoke(CurrentHealth,MaxHealth);
    }
    private int CurrentDamage()
    {
        float curDamage = UnityEngine.Random.Range(attackData.minDamage,attackData.maxDamage);

        if(isCritical)
        {
            curDamage *= attackData.criticalMultiplier;
        }
        return (int)curDamage; 
        
    }
}
