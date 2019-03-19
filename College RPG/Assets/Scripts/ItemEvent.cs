/*  A basic event that gives the player an item when activated
 */

using UnityEngine;
using System.Collections;

namespace MattScripts {

    public class ItemEvent : MonoBehaviour
    {
        public ScriptableObject itemToGive;

        // Upon being enabled, this event will grant the player the specific object
		private void OnEnable()
		{
            if(itemToGive is ItemData)
            {
                Debug.Log("Gave player" + ((ItemData)(itemToGive)).itemName);
                GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>().AddToInventory((ItemData)itemToGive);
                gameObject.SetActive(false);
            }
		}
	}   
}
