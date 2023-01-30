using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    //Script responsavel para controlar cada objeto da loja

    [Header("Parameters")]
    [SerializeField] bool hasItem;
    [SerializeField] bool equipped;
    [SerializeField] WeaponScriptable weaponScriptable;

    [Header("References")]
    [SerializeField] Image icon;
    [SerializeField] Image statusImage;
    [SerializeField] TMP_Text value;

    [Header("Colors")]
    [SerializeField] Color selectedColor;
    [SerializeField] Color deselectedColor;
    [SerializeField] Color blockedColor;

    public WeaponScriptable Weapon { get { return weaponScriptable; } }

    public WeaponScriptable WeaponScriptable { get { return weaponScriptable; } set { weaponScriptable = value; } }

    //Checa se tem o equipamento
    bool CheckHasItem() {
        if (PlayerPrefs.GetInt(weaponScriptable.WeaponName) == 0)
            return false;
        return true;
    }
    //Checa se está equipado
    bool CheckEquipped() {
        if (PlayerPrefs.GetInt("Equipped") != weaponScriptable.WeaponIndex)
            return false;
        return true;
    }

    //Executa o setup do item
    public void Setup(Sprite _icon, float _value)
    {
        icon.sprite = _icon;
        hasItem = CheckHasItem();
        if(hasItem)
            equipped = CheckEquipped();

        if (!hasItem) {
            value.text = _value.ToString() + " $";
            statusImage.color = blockedColor;
        }
        else if (equipped) {
            value.text = "Equipped";
            statusImage.color = selectedColor;
        }
        else {
            value.text = "Equip";
            statusImage.color = deselectedColor;
        }
    }

    //Trigger quando se clica no item
    public void ExecuteFunction()
    {
        if (hasItem) {
            Equip();
        }
        else {
            Buy();
        }
    }

    //Funções dos items
    void Equip()
    {
        if (!hasItem)
            return;

        PlayerPrefs.SetInt("Equipped", weaponScriptable.WeaponIndex);
        GameController.instance.RefreshShop();
        GameController.instance.SetupPlayer();
    }
    void Buy()
    {                                                                    
        if(GameController.instance.Balance >= weaponScriptable.WeaponValue) {
            GameController.instance.Balance = -(int)weaponScriptable.WeaponValue;
            PlayerPrefs.SetInt(weaponScriptable.WeaponName, 1);
            Setup(weaponScriptable.WeaponIcon, weaponScriptable.WeaponValue);
            GameController.instance.RefreshShopBalance();
        }
    }
}
