using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitInfo : ScriptableObject
{
    public string unitName;         // ��������� �����

    public int helthPerUnit;        // �� �� ������� (�.� ��� �� = �� �� ������� * ���-�� ������ � ������
    public int damageMin;           // ����������� ���� � �������
    public int damageMax;           // ������������ ���� � �������
    public int speed;               // �������� - ��������� �����������

    public bool ranged;             // ����� �� ��������� �� ����������

    public Sprite sprite;           // ������ (�� ������������)

    // �������� - �������� �������� ��� ��������� ����� � ������
    public RuntimeAnimatorController animatorController;
}
