using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu]
public class FigureInfo : ScriptableObject
{
    [SerializeField]
    UnitInfo unitInfo;                  // Информация о юните, из которого состоит отряд

    [SerializeField]
    int totalHealth;                    // Всего хп у отряда

    [SerializeField]
    bool player;                        // Управляется игроком

    public int movePointsRemaining;

    public bool canAttack;
    public bool waited;



    public int TotalHealth
    {
        get { return totalHealth; }
    }

    public UnitInfo UnitInfo
    {
        get { return unitInfo; }
    }

    public int UnitCount
    {
        get { return totalHealth / UnitInfo.helthPerUnit; }
    }

    public bool Player
    {
        get { return player; }
    }

    public string GetTextRepresentation()
    {
        return
            $"{unitInfo.unitName}" +
            $"\nHealth: {unitInfo.helthPerUnit}({TotalHealth%unitInfo.helthPerUnit})" +
            $"\nSpeed: {unitInfo.speed}" +
            $"\nDamage: {unitInfo.damageMin}-{unitInfo.damageMax}";
    }

    public void ResetMovePoints()
    {
        movePointsRemaining = UnitInfo.speed;
    }

    public void ReceiveDamage(int damage)
    {
        totalHealth -= damage;
    }

    public void SetHealth(int amount)
    {
        totalHealth = amount;
    }
}
