using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitInfo : ScriptableObject
{
    public string unitName;         // Названиие юнита

    public int helthPerUnit;        // Хп за единицу (т.е все хп = хп за единицу * кол-во единиц в отряде
    public int damageMin;           // Минимальный урон у единицы
    public int damageMax;           // Максимальный урон у единицы
    public int speed;               // Скорость - дальность перемещения

    public bool ranged;             // Может ли атаковать на расстоянии

    public Sprite sprite;           // Спрайт (не используется)

    // Аниматор - содержит анимации для состояний покоя и атакии
    public RuntimeAnimatorController animatorController;
}
