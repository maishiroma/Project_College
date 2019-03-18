/*  A data file that is used to store and templete useful information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts 
{
    // What kind of item is this? Affects what this item can do
    public enum ItemType {
        HEALTH,
        SP,
        KEY_ITEM
    }

    [CreateAssetMenu(fileName = "New Item Data", menuName = "Custom Data/Item")]
    public class ItemData : ScriptableObject {

        [Header("Visuals")]
        public Sprite itemSprite;

        [Space]
        public string itemName;
        [TextArea(1,4)]
        public string itemDescription;

        [Header("Item Properties")]
        public ItemType itemType;
        [Range(1,9999)]
        public int itemAmount;
    }   
}
