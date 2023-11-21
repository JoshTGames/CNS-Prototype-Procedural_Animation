using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewStats", menuName = "ScriptableObjects/InventoryManagement/Create Stats")]
public class ItemStats : ScriptableObject{
    [Serializable] public class WeaponData{
        [Tooltip("This is the damage the object or the projectile will inflict")] public float weaponDamage, fireRate, reloadTime;
        public int magCapacity;
        [Tooltip("If assigned, this will be instantiated when triggered")] public GameObject projectile;
    }
    [Tooltip("If for a weapon, open this")] public WeaponData weaponData;

    [Serializable] public class ConsumableData{        
        public float buffAmount, duration, cooldown;
    }
    [Tooltip("If for a consumable, open this")] public ConsumableData consumableData;
}
