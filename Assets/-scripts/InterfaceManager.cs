using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager instance;
    public GameObject bg;
    public float tilingDivider;
    public Material mat;
    public Canvas canv;
    private Vector3 mousePositionStart;
    private Node[] data;
    private Link[] links;
    public GameObject nodePrefab;
    public GameObject linkPrefab;
    public Image liveLink;
    private int nodeCounter;
    private int linkCounter;
    private GameObject selectedNode;
    private GameObject initialNode;
    private bool linking;
    private Vector3 trueMousePosition;
    public Camera mainCam;
    private float screenHeight;
    private float screenWidth;
    public float pixelsToCamSize;
    private void Start()
    {
        nodeCounter = 0;
        linkCounter = 0;
        data = new Node[1000];
        links = new Link[1000];
        bg.transform.localPosition = new Vector3(0f, 0f, 0f);
        bg.transform.localScale = new Vector3(Screen.width, Screen.height, 1f);
        mat.mainTextureScale = new Vector2(Screen.width / tilingDivider,Screen.height / tilingDivider);
        mainCam.orthographicSize = Screen.height / pixelsToCamSize;
        //print(Screen.width + " x " + Screen.height);
        //(mainCam.orthographicSize* 2 *mainCam.aspect + " x " + mainCam.orthographicSize * 2);
    }

    private void Update()
    {
        mat.mainTextureScale = new Vector2(Screen.width / tilingDivider,Screen.height / tilingDivider);/////////////
        
        trueMousePosition = new Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0f);
        if (Input.GetMouseButtonDown(1)) //start movement by RMB
        {
            mousePositionStart = trueMousePosition;
        } else if (Input.GetMouseButton(1)) //movement by RMB
        {
            mat.SetTextureOffset("_MainTex", ((Vector2)mousePositionStart - (Vector2)trueMousePosition) / tilingDivider + mat.mainTextureOffset);
            mainCam.transform.position += (mousePositionStart - trueMousePosition) * canv.transform.localScale.x;//+= (mousePositionStart - trueMousePosition) / tilingDivider;
            bg.transform.localPosition += (mousePositionStart - trueMousePosition) ;
            
            
            
            mousePositionStart = trueMousePosition;
        }
        else if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))||(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.N))) //create new node
        {
            CreateNode();
        }
        else if (Input.GetKeyDown(KeyCode.F) && !linking && selectedNode != null) //start new link
        {
            initialNode = selectedNode;
            linking = true;
            liveLink.transform.localPosition = new Vector3((initialNode.transform.localPosition.x + trueMousePosition.x)/2, (initialNode.transform.localPosition.y + trueMousePosition.y)/2, -1f);
            liveLink.transform.eulerAngles = new Vector3(0f, 0f, GetVectorAngle(initialNode.transform.localPosition, trueMousePosition));
            liveLink.sprite.rect.Set(liveLink.sprite.rect.x, liveLink.sprite.rect.y,Vector2.Distance(trueMousePosition, initialNode.transform.localPosition),liveLink.sprite.rect.height);
        }
        else if ((Input.GetKeyDown(KeyCode.F)|| Input.GetKeyDown(KeyCode.Escape)) && linking) //stop linking by pressing F
        {
            DiscardLink();
        }
        else if (linking) //continue linking
        {
            liveLink.transform.localPosition = new Vector3((initialNode.transform.localPosition.x + trueMousePosition.x)/2, (initialNode.transform.localPosition.y + trueMousePosition.y)/2, -1f);
            liveLink.transform.eulerAngles = new Vector3(0f, 0f, GetVectorAngle(initialNode.transform.localPosition, trueMousePosition));
            print(Vector2.Distance(trueMousePosition, initialNode.transform.localPosition));
            liveLink.rectTransform.sizeDelta = new Vector2(Vector2.Distance(trueMousePosition, initialNode.transform.localPosition) / liveLink.transform.localScale.x, liveLink.rectTransform.rect.height);
                     //transform.localScale = new Vector3(Vector2.Distance(trueMousePosition, initialNode.transform.localPosition) / liveLink.sprite.rect.width / canv.transform.localScale.x, liveLink.transform.localScale.y,liveLink.transform.localScale.z);
        }
    }

    public void DiscardLink()
    {
        linking = false;
        liveLink.transform.localPosition = Vector3.forward;
        initialNode = null;
        selectedNode = null;
    }
    
    public void EditMainText(int id, string text)
    {
        data[id].value = text;
        //print(id + " " + text);
    }

    public void SelectNode(GameObject node)
    {
        selectedNode = node;
    }

    public void DeselectNode(GameObject node)
    {
        if (node == selectedNode && !linking)
            selectedNode = null;
    }

    public void ClickedNode(GameObject node)
    {
        if (linking)
        {
            if (node == initialNode)
            {
                DiscardLink();
            }
            else if (node != initialNode)
            {
                CreateLink(node);
                DiscardLink();
            }
        }
        else
        {
            selectedNode = node;
        }
    }

    public void CreateNode()
    {
        data[++nodeCounter] = new Node();
        data[nodeCounter].gameObj = Instantiate(nodePrefab);
        data[nodeCounter].gameObj.transform.parent = canv.transform;
        data[nodeCounter].gameObj.transform.localPosition = trueMousePosition;
        data[nodeCounter].gameObj.name = nodeCounter.ToString();
    }

    public void CreateLink(GameObject newNode)
    {
        links[++linkCounter] = new Link();
        links[linkCounter].gameObj = Instantiate(linkPrefab);////////////////////////////////////////////////
        links[linkCounter].linkedRect = links[linkCounter].gameObj.GetComponent<RectTransform>();
        links[linkCounter].gameObj.transform.parent = canv.transform;
        links[linkCounter].linkedRect.SetAsFirstSibling();
        links[linkCounter].gameObj.transform.localPosition = new Vector3((initialNode.transform.localPosition.x + newNode.transform.localPosition.x)/2, (initialNode.transform.localPosition.y + newNode.transform.localPosition.y)/2, -0.5f);;
        links[linkCounter].gameObj.transform.eulerAngles = new Vector3(0f, 0f, GetVectorAngle(initialNode.transform.localPosition, newNode.transform.localPosition));
        links[linkCounter].linkedRect.sizeDelta = new Vector2(Vector2.Distance(newNode.transform.localPosition, initialNode.transform.localPosition) / liveLink.transform.localScale.x, liveLink.rectTransform.rect.height);
        
        links[linkCounter].linkedNodes = new GameObject[2];
        links[linkCounter].linkedNodes[0] = initialNode;
        links[linkCounter].linkedNodes[1] = newNode;
        
        data[Int32.Parse(initialNode.name)].linkCounter++;
        if (data[Int32.Parse(initialNode.name)].linkCounter == 1)
        {
            data[Int32.Parse(initialNode.name)].links = new int[1000];
        }
        data[Int32.Parse(initialNode.name)].links[data[Int32.Parse(initialNode.name)].linkCounter] = Int32.Parse(newNode.name);
        
        
        
        data[Int32.Parse(newNode.name)].linkCounter++;
        if (data[Int32.Parse(newNode.name)].linkCounter == 1)
        {
            data[Int32.Parse(newNode.name)].links = new int[1000];
        }
        data[Int32.Parse(newNode.name)].links[data[Int32.Parse(newNode.name)].linkCounter] = Int32.Parse(initialNode.name);
    }

    public float GetVectorAngle(Vector2 first, Vector2 second)
    {
        return Convert.ToSingle(Math.Atan((first.y - second.y) / (first.x - second.x)) * 180 / Math.PI);
    }
}
