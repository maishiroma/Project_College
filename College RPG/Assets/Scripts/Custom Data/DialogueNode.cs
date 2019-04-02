/*  This stores all of the information that a dialoge setpiece has.
 *  Contains useful methods and variables that allow for the streamlineing of setting this up.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    [System.Serializable]
    public class DialogueNode {

        [Header("Dialogue Variables")]
        [SerializeField]
        private Sprite portrait = null;
        [SerializeField]
        private string nameText = "";
        [SerializeField]
        private string choiceText = "";
        [SerializeField]
        [TextArea(1, 4)]
        private string dialogueText = "";
        [SerializeField]
        private int delayTime = 0;

        [Header("External References")]
        [SerializeField]
        private BaseEvent endEvent;                  // This GameObject corresponds to an Event that can occur once this is completed

        [Header("List Variables")]
        [SerializeField]
        private int nodeId = -1;                     // Identifies each Dialogue node based on the current Dialogue Event. This should be the same number as its array index position
        [SerializeField]
        private int[] childrenNodeIds;               // References to any of the other dialouge options that are in the current Dialogue Event

        // Getters/Setters
        public Sprite DialoguePortrait {
            get {return portrait;}
        }

        public string DialogueName {
            get {return nameText;}
        }

        public string DialogueChoiceText {
            get {return choiceText;}
        }

        public string DialogueText {
            get {return dialogueText;}
        }

        public int DialogueId{
            get {return nodeId;}
        }

        public int GetDelayTime{
            get { return delayTime;}
        }

        public BaseEvent DialogueEvent {
            get {return endEvent;}
        }

        // Constructor that createa a Node with no children
        public DialogueNode(Sprite portrait, string nameText, string choiceText, string dialogueText, int nodeId, int delayTime, BaseEvent endEvent = null)
        {
            this.portrait = portrait;
            this.nameText = nameText;
            this.choiceText = choiceText;
            this.dialogueText = dialogueText;
            this.nodeId = nodeId;
            this.delayTime = delayTime;
            this.endEvent = endEvent;
        }

        // Constructor that create a Node with one child node
        public DialogueNode(Sprite portrait, string nameText, string choiceText, string dialogueText, int nodeId, int childNodeId, int delayTime, BaseEvent endEvent = null)
        {
            this.portrait = portrait;
            this.nameText = nameText;
            this.choiceText = choiceText;
            this.dialogueText = dialogueText;
            this.nodeId = nodeId;
            this.delayTime = delayTime;
            this.endEvent = endEvent;

            this.childrenNodeIds = new int[1] {childNodeId};
        }

        // Constructor that create a Node that has multiple child nodes
        public DialogueNode(Sprite portrait, string nameText, string choiceText, string dialogueText, int nodeId, int[] childrenNodeIds, int delayTime, BaseEvent endEvent = null)
        {
            this.portrait = portrait;
            this.nameText = nameText;
            this.choiceText = choiceText;
            this.dialogueText = dialogueText;
            this.nodeId = nodeId;
            this.delayTime = delayTime;
            this.endEvent = endEvent;

            this.childrenNodeIds = childrenNodeIds;
        }

        // Retrieves the Id of a child dialogue node at the specified index point. If the index point is invalid, -1 is returned
        public int GetChildIdAtIndex(int indexPoint)
        {
            if(CheckIfChildrenExist())
            {
                if(indexPoint < childrenNodeIds.Length)
                {
                    return childrenNodeIds[indexPoint];
                }
            }
            return -1;
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

        // A helper method that checks if the end event in the dialogue is completed.
        // Returns false if the event is not done yet.
        // Returns true if there is no event or if the event is done
        public bool CheckIfEventIsCompleted()
        {
            if(CheckIfEventExists())
            {
                return endEvent.IsFinished;   
            }
            return true;
        }
    }
}