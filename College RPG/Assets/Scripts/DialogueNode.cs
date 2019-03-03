/*  This stores all of the information that a dialoge setpiece has.
 * 
 *  TODO: Have a way to easily fill in all of this information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    [System.Serializable]
    public class DialogueNode {

        [Header("Dialogue Variables")]
        public Sprite portrait = null;
        public string nameText = "";
        public string choiceText = "";
        [TextArea(1, 4)]
        public string dialogueText = "";

        [Header("External References")]
        public BaseEvent endEvent;         // This GameObject corresponds to an Event that can occur once this is completed

        [Header("List Variables")]
        public int nodeId = -1;                     // Identifies each Dialogue node based on the current Dialogue Event. This should be the same number as its array index position
        public int[] childrenNodeIds;               // References to any of the other dialouge options that are in the current Dialogue Event

        // Constructor that createa a Node with no children
        public DialogueNode(Sprite portrait, string nameText, string choiceText, string dialogueText, int nodeId)
        {
            this.portrait = portrait;
            this.nameText = nameText;
            this.choiceText = choiceText;
            this.dialogueText = dialogueText;
            this.nodeId = nodeId;
        }

        // Constructor that create a Node with one child node
        public DialogueNode(Sprite portrait, string nameText, string choiceText, string dialogueText, int nodeId, int childNodeId)
        {
            this.portrait = portrait;
            this.nameText = nameText;
            this.choiceText = choiceText;
            this.dialogueText = dialogueText;
            this.nodeId = nodeId;

            this.childrenNodeIds = new int[1] {childNodeId};
        }

        // Constructor that create a Node that has multiple child nodes
        public DialogueNode(Sprite portrait, string nameText, string choiceText, string dialogueText, int nodeId, int[] childrenNodeIds)
        {
            this.portrait = portrait;
            this.nameText = nameText;
            this.choiceText = choiceText;
            this.dialogueText = dialogueText;
            this.nodeId = nodeId;

            this.childrenNodeIds = childrenNodeIds;
        }

        // Returns true if the node has children
        public bool CheckIfChildrenExist()
        {
            if(childrenNodeIds == null)
            {
                return false;
            }
            else if(childrenNodeIds.Length == 0)
            {
                return false;
            }
            return true;
        }
    
        // Returns true if the dialogue has more than one child
        public bool CheckIfMultipleChilrenExist()
        {
            if(CheckIfChildrenExist())
            {
                if(childrenNodeIds.Length > 1)
                {
                    return true;
                }
            }
            return false;
        }

        // Returns the first child Id of this dialogue. If there are no childre, return -1
        public int GetFirstChildId()
        {
            if(CheckIfChildrenExist())
            {
                return childrenNodeIds[0];
            }
            return -1;
        }
    
        // Returns true if there is a gameobject associated with this dialogue that has a BaseEvent component
        public bool CheckIfEventExists()
        {
            if(endEvent != null)
            {
                return true;
            }
            return false;
        }

        // If an event is associated with this dialogue, this will activate it upon being called
        public void ActivateEvent()
        {
            if(CheckIfEventExists())
            {
                endEvent.gameObject.SetActive(true);
            }
        }
    }
}