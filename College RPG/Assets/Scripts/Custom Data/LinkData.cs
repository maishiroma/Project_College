/*  A data file that is used to store and templete useful information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts 
{
    // What kind of link is this? Based on the 18 Tarot Cards
    public enum LinkType {
        JESTER,
        FOOL,
        MAGICIAN,
        PRIESTESS,
        EMPRESS,
        EMPEROR,
        LOVERS,
        CHARIOT,
        JUSTICE,
        HERMIT,
        FORTUNE,
        HANGED_MAN,
        TEMPERANCE,
        DEVIL,
        TOWER,
        STAR,
        MOON,
        SUN
    }

    [CreateAssetMenu(fileName = "New Link Data", menuName = "Custom Data/Link")]
    public class LinkData : ScriptableObject {

        [Header("Visuals")]
        public string linkName;
        [TextArea(1,4)]
        public string linkDescription;

        [Header("Link Properties")]
        public LinkType linkType;

        [Header("External References")]
        [Tooltip("The demon that is associtated with this link. This demon will have unique affinires and stats to go with this.")]
        public DemonData linkDemon;

        [Tooltip("The unique attacks that come with this link.")]
        public List<AttackData> linkAttacks = new List<AttackData>();
    }   
}
