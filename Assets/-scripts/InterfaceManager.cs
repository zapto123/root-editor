using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public Node[] data;
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
    public Sprite link0;
    public Sprite link1;
    public Sprite link2;

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


    private void Awake()
    {
        print(GetLinkID("link1"));
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

    public void EditTitle()
    {
        
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
        data[nodeCounter].gameObj.transform.localPosition = trueMousePosition + bg.transform.localPosition;
        data[nodeCounter].gameObj.name = nodeCounter.ToString();
        data[nodeCounter].id = nodeCounter;
    }

    public void CreateLink(GameObject newNode)
    {
        links[linkCounter] = new Link();
        links[linkCounter].gameObj = Instantiate(linkPrefab);////////////////////////////////////////////////
        links[linkCounter].gameObj.name = "link" + linkCounter;
        links[linkCounter].img = links[linkCounter].gameObj.GetComponent<Image>();
        links[linkCounter].linkState = 0;
        links[linkCounter].linkedRect = links[linkCounter].gameObj.GetComponent<RectTransform>();
        links[linkCounter].gameObj.transform.parent = canv.transform;
        links[linkCounter].linkedRect.SetAsFirstSibling();
        links[linkCounter].gameObj.transform.localPosition = new Vector3((initialNode.transform.localPosition.x + newNode.transform.localPosition.x)/2, (initialNode.transform.localPosition.y + newNode.transform.localPosition.y)/2, -0.5f);;
        links[linkCounter].gameObj.transform.eulerAngles = new Vector3(0f, 0f, GetVectorAngle(initialNode.transform.localPosition, newNode.transform.localPosition));
        links[linkCounter].linkedRect.sizeDelta = new Vector2(Vector2.Distance(newNode.transform.localPosition, initialNode.transform.localPosition) / liveLink.transform.localScale.x, liveLink.rectTransform.rect.height);
        
        links[linkCounter].linkedNodes = new GameObject[2];
        links[linkCounter].linkedNodes[0] = new GameObject();
        links[linkCounter].linkedNodes[0] = initialNode;
        links[linkCounter].linkedNodes[1] = new GameObject();
        links[linkCounter].linkedNodes[1] = newNode;
        
        if (data[Int32.Parse(initialNode.name)].linkedNodeCounter == 0)
        {
            data[Int32.Parse(initialNode.name)].linkedNodes = new int[1000];
        }
        data[Int32.Parse(initialNode.name)].linkedNodes[data[Int32.Parse(initialNode.name)].linkedNodeCounter] = Int32.Parse(newNode.name);
        data[Int32.Parse(initialNode.name)].linkedNodeCounter++;

        
        if (data[Int32.Parse(newNode.name)].linkedNodeCounter == 0)
        {
            data[Int32.Parse(newNode.name)].linkedNodes = new int[1000];
        }
        data[Int32.Parse(newNode.name)].linkedNodes[data[Int32.Parse(newNode.name)].linkedNodeCounter] = Int32.Parse(initialNode.name);
        data[Int32.Parse(newNode.name)].linkedNodeCounter++;

        
        
        if (data[Int32.Parse(initialNode.name)].linkedLinkCounter == 0)
        {
            data[Int32.Parse(initialNode.name)].linkedLinks = new GameObject[1000];
        }

        data[Int32.Parse(initialNode.name)].linkedLinks[data[Int32.Parse(initialNode.name)].linkedLinkCounter] =
            links[linkCounter].gameObj;
        data[Int32.Parse(initialNode.name)].linkedLinkCounter++;

        if (data[Int32.Parse(newNode.name)].linkedLinkCounter == 0)
        {
            data[Int32.Parse(newNode.name)].linkedLinks = new GameObject[1000];
        }

        data[Int32.Parse(newNode.name)].linkedLinks[data[Int32.Parse(newNode.name)].linkedLinkCounter] =
            links[linkCounter].gameObj;
        
        data[Int32.Parse(newNode.name)].linkedLinkCounter++;
        linkCounter++;
    }

    public float GetVectorAngle(Vector2 first, Vector2 second)
    {
        return Convert.ToSingle(Math.Atan((first.y - second.y) / (first.x - second.x)) * 180 / Math.PI);
    }

    public void DisplayAllData()
    {
        for(int i = 1; i <= nodeCounter; i++)
        {
            print(data[i].id + " " + data[i].title + " " + data[i].value + " " + data[i].linkedNodes);
        }
    }

    public int GetLinkID(string name)
    {
        return Int32.Parse(new String(name.Where(Char.IsDigit).ToArray()));
    }

    public void LinkSwitchMain(string name)
    {
        int id = GetLinkID(name);
        links[id].linkState = ++links[id].linkState % 3;
        links[id].img.sprite = (links[id].linkState == 2) ? link2 : (links[id].linkState == 1) ? link1 : link0;
        if (links[id].linkState == 1)
        {
            links[id].img.sprite = link1;
        } else if (links[id].linkState == 2)
        {
            links[id].img.sprite = link2;
        } else {
            links[id].img.sprite = link0;
        }
    }

    public void MoveLink(int id)
    {
        links[id].gameObj.transform.localPosition = new Vector3((links[id].linkedNodes[0].transform.localPosition.x + links[id].linkedNodes[1].transform.localPosition.x)/2, (links[id].linkedNodes[0].transform.localPosition.y + links[id].linkedNodes[1].transform.localPosition.y)/2, -0.5f);;
        links[id].gameObj.transform.eulerAngles = new Vector3(0f, 0f, GetVectorAngle(links[id].linkedNodes[0].transform.localPosition, links[id].linkedNodes[1].transform.localPosition));
        links[id].linkedRect.sizeDelta = new Vector2(Vector2.Distance(links[id].linkedNodes[1].transform.localPosition, links[id].linkedNodes[0].transform.localPosition) / links[id].gameObj.transform.localScale.x, links[id].linkedRect.rect.height);
    }
}
