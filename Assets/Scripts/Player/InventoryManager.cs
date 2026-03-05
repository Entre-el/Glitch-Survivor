using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6]; // Assuming 6 weapon slots
    public List<Image> weaponSlotImages = new List<Image>(6); // UI images for weapon slots
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public List<Image> passiveItemSlotImages = new List<Image>(6); // UI images for passive item slots
    public int[] passiveItemLevels = new int[6]; // Assuming 6 passive item slots
    
        public void AddWeapon(int slotIndex ,WeaponController weapon)
        {
            if (slotIndex >= 0 && slotIndex < weaponSlots.Count)
            {
                weaponSlots[slotIndex] = weapon;
                weaponLevels[slotIndex] = weapon.weaponData.Level; // Initialize weapon level to 1
                weaponSlotImages[slotIndex].enabled = true; // Ensure the UI image is visible
                weaponSlotImages[slotIndex].sprite = weapon.weaponData.Icon; // Update UI image
            }
            else
            {
                Debug.LogError("Invalid weapon slot index: " + slotIndex);
            }
        }
        public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
        {
            if (slotIndex >= 0 && slotIndex < passiveItemSlots.Count)
            {
                passiveItemSlots[slotIndex] = passiveItem;
                passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level; // Initialize passive item level to 1
                passiveItemSlotImages[slotIndex].enabled = true; // Ensure the UI image is visible
                passiveItemSlotImages[slotIndex].sprite = passiveItem.passiveItemData.Icon; // Update UI image
            }
            else
            {
                Debug.LogError("Invalid passive item slot index: " + slotIndex);
            }
        }
        public void LevelUpWeapon(int slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];
            if (slotIndex >= 0 && slotIndex < weaponLevels.Length)
            {
                if(!weapon.weaponData.NextLevelPrefab)
                {
                    Debug.LogWarning("Weapon at slot " + slotIndex + " is already at max level.");
                    return;
                }
                GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
                upgradedWeapon.transform.SetParent(transform);
                AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
                Destroy(weapon.gameObject); // Remove the old weapon
            }
            else
            {
                Debug.LogError("Invalid weapon slot index: " + slotIndex);
            }
        }
        public void LevelUpItem(int slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];
            if (slotIndex >= 0 && slotIndex < passiveItemLevels.Length)
            {
                if(!passiveItem.passiveItemData.NextLevelPrefab)
                {
                    Debug.LogWarning("Passive item at slot " + slotIndex + " is already at max level.");
                    return;
                }
                GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
                upgradedPassiveItem.transform.SetParent(transform);
                AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
                Destroy(passiveItem.gameObject); // Remove the old passive item
            }
            else
            {
                Debug.LogError("Invalid passive item slot index: " + slotIndex);
            }
        }
}