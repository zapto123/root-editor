using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int id;
    public string title;
    public string value;
    public int[] links;
    public int linkCounter;
    public string requiredEvent;
    public string trigger;
    public Vector2 position;
    public GameObject gameObj;
}
