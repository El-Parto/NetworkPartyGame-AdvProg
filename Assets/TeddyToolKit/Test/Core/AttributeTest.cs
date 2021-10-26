using System.Collections;
using System.Collections.Generic;
using TeddyToolKit.Core.Attribute;
using UnityEngine;

public class AttributeTest : MonoBehaviour
{
    /// <summary>
    /// player tag will be highlighted in the inspector
    /// </summary>
    [Tag, SerializeField] private string playerTag = "Player";
    /// <summary>
    /// nothing will be highlighted in the inspector, the default is untagged
    /// </summary>
    [Tag, SerializeField] private string invalidTag;
}
