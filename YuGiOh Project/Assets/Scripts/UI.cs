using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System;

public class UI : MonoBehaviour
{

    // holds all results vertically
    public RectTransform container;

    // prefab used to display results
    public GameObject segmentPrefab;

    // list of all available segments
    private List<GameObject> segments = new List<GameObject>();

    [Header("Info Dropdown")]

    // info dropdown object
    public RectTransform infoDropdown;
    public RectTransform FilterButtons;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardText;
    public Image cardArt;
    public Sprite[] availableImageTypes;

    public static UI instance;

    private void Awake()
    {
        instance = this;
    }

    GameObject CreateNewSegment ()
    {
        GameObject segment = Instantiate(segmentPrefab);
        //Debug.Log("seg scale test 1: " + segment.transform.localScale.x);
        segment.transform.parent = container.transform;
        segment.transform.localScale = Vector3.one;
        //Debug.Log("seg scale test 2: " + segment.transform.localScale.x);
        segments.Add(segment);

        // add OnClick event listener to the button
        segment.GetComponent<Button>().onClick.AddListener(() => { OnShowMoreInfo(segment); });

        // deactivate it by default
        segment.SetActive(false);
        return segment;
    }

    // instantiates a set number of segments to use later on
    void PreLoadSegments(int amount)
    {
        // instantiate 'amount' number of new segments
        for (int x = 0; x < amount; ++x)
            CreateNewSegment();
    }

    void Start()
    {
        // preload 10 segments
        PreLoadSegments(10);
    }

    // gets the JSON result and displays them on the screen with their respective segments
    public void SetSegments(JSONNode records)
    {
        DeactivateAllSegments();

        // loop through all records
        for (int x = 0; x < records.Count; ++x)
        {
            /*
            if (x == 0)
            {
                Debug.Log("Test Type: " + records[x]["type"]);
            }
            */

            // create a new segment if we don't have enough
            GameObject segment = x < segments.Count ? segments[x] : CreateNewSegment();
            segment.SetActive(true);

            //Debug.Log("setseg test: "+ records[x]["name"]);
            TextMeshProUGUI nameText = segment.transform.Find("CardName").GetComponent<TextMeshProUGUI>();
            Image ifFail = segment.transform.Find("CardTypeImage").GetComponent<Image>();
            Sprite cardTypeImage = ifFail.sprite;

            nameText.text = records[x]["name"];

            // set imageType
            String cardType = CalcCardType(records[x]["type"]);
            Debug.Log("cct result: " + cardType);
            switch (cardType)
            {
                case "Normal":
                    cardTypeImage = availableImageTypes[0];
                    break;
                case "Effect":
                    cardTypeImage = availableImageTypes[1];
                    break;
                case "Ritual":
                    cardTypeImage = availableImageTypes[2];
                    break;
                case "Fusion":
                    cardTypeImage = availableImageTypes[3];
                    break;
                case "Synchro":
                    cardTypeImage = availableImageTypes[4];
                    break;
                case "XYZ":
                    cardTypeImage = availableImageTypes[5];
                    break;
                case "Link":
                    cardTypeImage = availableImageTypes[6];
                    break;
                case "Pend Normal":
                    cardTypeImage = availableImageTypes[7];
                    break;
                case "Pend Effect":
                    cardTypeImage = availableImageTypes[8];
                    break;
                case "Pend Ritual":
                    cardTypeImage = availableImageTypes[9];
                    break;
                case "Pend Fusion":
                    cardTypeImage = availableImageTypes[10];
                    break;
                case "Pend Synchro":
                    cardTypeImage = availableImageTypes[11];
                    break;
                case "Pend XYZ":
                    cardTypeImage = availableImageTypes[12];
                    break;
                case "Spell":
                    cardTypeImage = availableImageTypes[13];
                    break;
                case "Trap":
                    cardTypeImage = availableImageTypes[14];
                    break;
                case "9999":
                    ifFail.gameObject.SetActive(false);
                    break;
                default:
                    ifFail.gameObject.SetActive(false);
                    break;

            }
            segment.transform.Find("CardTypeImage").GetComponent<Image>().sprite = cardTypeImage;
        }



        // set the container size to clamp to the segments
            container.sizeDelta = new Vector2(container.sizeDelta.x, GetContainerHeight(records.Count));
    }

    // deactivate all of the segment objects
    void DeactivateAllSegments()
    {
        // loop through all segments and deactivate them
        foreach (GameObject segment in segments)
            segment.SetActive(false);
    }

    // returns a height to make the container so it clamps to the size of all segments
    float GetContainerHeight(int count)
    {
        float height = 0.0f;
        // include all segment heights
        height += count * (segmentPrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
        // include the spacing between segments
        height += count * container.GetComponent<VerticalLayoutGroup>().spacing;
        // include the info dropdown height
        height += infoDropdown.sizeDelta.y;
        return height;
    }

    // called when the user selects a segment - toggles the dropdown
    public void OnShowMoreInfo(GameObject segmentObject)
    {
        // get the index of the segment
        int index = segments.IndexOf(segmentObject);

        // if we're pressing the segment that's already open, close the dropdown
        if (infoDropdown.transform.GetSiblingIndex() == index + 1 && infoDropdown.gameObject.activeInHierarchy)
        {
            infoDropdown.gameObject.SetActive(false);
            return;
        }

        infoDropdown.gameObject.SetActive(true);

        // get only the records
        //JSONNode records = AppManager.instance.jsonResult["result"]["records"];
        JSONNode records = AppManager.instance.jsonResult["data"];

        // set the dropdown to appear below the selected segment
        infoDropdown.transform.SetSiblingIndex(index + 1);

        // set dropdown info text
        cardName.text = records[index]["name"];
        cardText.text = records[index]["desc"];




        /*

        // set image

        // search for card ID to get card image
        bool imageSaved = false;

         for(int i = 0; i< AppManager.instance.ImageStorage.Count;i++)
                {
                    if (AppManager.instance.ImageStorage[i].cardID.Equals(records[index]["id"]))
                    {
                        imageSaved = true;

                        // set image to already saved image
                        cardArt = AppManager.instance.ImageStorage[i].croppedURL;
                        break;
                    }
                }


                // if no image, download image and save to list
                if(imageSaved == false)
                {
                    String imageID = records[index]["id"];
                    AppManager.instance.StartCoroutine("GetImage", imageID );
                }

                // display image


         */



    }

    public void OnSearch(TextMeshProUGUI input)
    {
        if (input.text.Length - 1 >= 3) {
            // get and set the data
            // unsure why, but invisible character is added to the end of user input, this removes it
            AppManager.instance.StartCoroutine("GetData", input.text.Remove(input.text.Length - 1));
            // disable the info dropdown
            infoDropdown.gameObject.SetActive(false);
            FilterButtons.gameObject.SetActive(true);   
        }

        
    }
    
    public String CalcCardType(String Input)
    {
        /* 
        Card Types Include more information than just the border colours of the card which is what we want
        Currently 27 types included in data, although there are only 15 unique card borders

        if new card type is added after project concludes, this will make sure that the image is hidden
        */

        String Output = "9999";


        // start by removing the singular types
        if (Input.Contains("Spell")) {
            return "Spell";
        }
        else if (Input.Contains("Trap"))
        {
            return "Trap";
        }
        else if (Input.Contains("Link"))
        {
            return "Link";
        }

        // most of the remaining have 2 versions, pendelum and non pendelum

        if (Input.Contains("Fusion"))
        {
            if(Input.Contains("Pendelum"))
            {
                return "Pend Fusion";
            } else
            {
                return "Fusion";
            }
        }

        else if (Input.Contains("Synchro"))
        {
            if (Input.Contains("Pendelum"))
            {
                return "Pend Synchro";
            }
            else
            {
                return "Synchro";
            }
        }

        else if (Input.Contains("XYZ"))
        {
            if (Input.Contains("Pendelum"))
            {
                return "Pend XYZ";
            }
            else
            {
                return "XYZ";
            }
        }

        else if (Input.Contains("Ritual"))
        {
            if (Input.Contains("Pendelum"))
            {
                return "Pend Ritual";
            }
            else
            {
                return "Ritual";
            }
        }

        else if (Input.Contains("Effect"))
        {
            if (Input.Contains("Pendelum"))
            {
                return "Pend Effect";
            }
            else
            {
                return "Effect";
            }
        }

        else if (Input.Contains("Normal"))
        {
            if (Input.Contains("Pendelum"))
            {
                return "Pend Normal";
            }
            else
            {
                return "Normal";
            }
        }

        // if none are true, output will be 9999
        return Output;
    }

}



