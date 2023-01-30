using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponScriptable : ScriptableObject
{
    //Armazenamento de informação de cada arma
    public string WeaponName;
    public int WeaponIndex;
    public GameObject WeaponPrefab;
    public Sprite WeaponIcon;
    public float WeaponValue;
}
