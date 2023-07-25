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
    public RectTransform deckContainer;

    // prefab used to display results
    public GameObject segmentPrefab;

    // list of all available segments
    private List<GameObject> segments = new List<GameObject>();

    [Header("Info Dropdown")]

    // info dropdown object
    public RectTransform infoDropdown;
    public RectTransform FilterButtons; 
    public RectTransform DeckButtons;
    public RectTransform MainMenuButtons;

    public String cardID;
    public String tempType;
    public String tempRace;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardText;
    public TextMeshProUGUI cardRT;
    public TextMeshProUGUI cardLR;
    public TextMeshProUGUI cardLink;
    public TextMeshProUGUI cardScale;
    public TextMeshProUGUI cardAttr;
    public TextMeshProUGUI cardAtk;
    public TextMeshProUGUI cardDef;

    public Button AddCardButton;
    public Button RemoveCardButton;
    public Button MainSideButtonTop;
    public Button MainSideButtonBottom;


    public Image cardArt;
    public Sprite[] availableImageTypes;
    public Sprite TwoCard;
    public Sprite ThreeCard;

    public static UI instance;

    private void Awake()
    {
        instance = this;
    }

    GameObject CreateNewSegment ()
    {
        GameObject segment = Instantiate(segmentPrefab);
        //Debug.Log("seg scale test 1: " + segment.transform.localScale.x);
        segment.transform.SetParent(container.transform);
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

            // create a new segment if we don't have enough
            GameObject segment = x < segments.Count ? segments[x] : CreateNewSegment();
            segment.SetActive(true);


            TextMeshProUGUI nameText = segment.transform.Find("CardName").GetComponent<TextMeshProUGUI>();
            Image ifFail = segment.transform.Find("CardTypeImage").GetComponent<Image>();
            Sprite cardTypeImage = ifFail.sprite;

            nameText.text = records[x]["name"];

            // set imageType
            String cardType = CalcCardType(records[x]["type"]);
            //Debug.Log("cct result: " + cardType);
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

    public void displayFilterCards(List<DeckBuild.Card> newDeck)
    {
        deactiivateCards();
        //Debug.Log("Filtering:");
        foreach (DeckBuild.Card card in newDeck)
        {
            GameObject temp;
            try
            {
                temp = UI.instance.deckContainer.Find(card.name).gameObject;
                bool isSide = AppManager.instance.GetComponent<DeckBuild>().isSide;
                //GameObject copiesRun = temp.transform.Find("CopiesRunImage").gameObject;
                GameObject copiesRun = temp.transform.GetChild(0).GetChild(0).gameObject;


                // if side atleast 1
                if (isSide && card.SideCopies > 0)
                {
                    temp.gameObject.SetActive(true);

                    if (card.SideCopies == 2) 
                    {
                        Debug.Log("side 2");
                        copiesRun.SetActive(true);
                        copiesRun.GetComponent<Image>().sprite = TwoCard;
                    }

                    else if (card.SideCopies == 3) 
                    {
                        Debug.Log("side 3");
                        copiesRun.SetActive(true);
                        copiesRun.GetComponent<Image>().sprite = ThreeCard;
                    } else
                    {
                        
                        copiesRun.SetActive(false);
                    }

                }
                
                // if main/extra atleast 1
                else if (!isSide && (card.MainCopies > 0 || card.ExtraCopies > 0))
                {
                    temp.gameObject.SetActive(true);
                    
                    if (card.MainCopies == 2 || card.ExtraCopies == 2)
                    {

                        Debug.Log("me 3");
                        copiesRun.SetActive(true);
                        copiesRun.GetComponent<Image>().sprite = TwoCard;
                    }

                    else if (card.MainCopies == 3 || card.ExtraCopies == 3)
                    {
                        Debug.Log("me 3");
                        copiesRun.SetActive(true);
                        copiesRun.GetComponent<Image>().sprite = ThreeCard;
                    }
                    else
                    {
                        
                        copiesRun.SetActive(false);
                    }


                } else
                {
                    temp.SetActive(false);
                }
            }
            catch
            {
                Debug.Log("can't find object with that name: " + card.name);
            }
            //Debug.Log("cardname: " + temp.name);
            //temp.SetActive(true);
            //Debug.Log(card.name);
        }

    }

    void deactiivateCards()
    {
        foreach (GameObject card in AppManager.instance.GetComponent<DeckBuild>().cardPrefabs)
            card.SetActive(false);
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
        //height += infoDropdown.sizeDelta.y;

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
            // include the info dropdown height
            container.sizeDelta = new Vector2(container.sizeDelta.x, container.sizeDelta.y - infoDropdown.sizeDelta.y);

            infoDropdown.gameObject.SetActive(false);

            DeckButtons.gameObject.SetActive(false);
            MainMenuButtons.gameObject.SetActive(true);

            return;
        } 
        
        // else if dropdown isn't open
        else if (!infoDropdown.gameObject.activeInHierarchy)
        {
            // include the info dropdown height
            container.sizeDelta = new Vector2(container.sizeDelta.x, container.sizeDelta.y + infoDropdown.sizeDelta.y);

            // activate buttons
            DeckButtons.gameObject.SetActive(true);
            MainMenuButtons.gameObject.SetActive(false);

        }

        infoDropdown.gameObject.SetActive(true);

        JSONNode records = AppManager.instance.jsonFilter;
        //Debug.Log("showmoreinfo using filtered records: " + records.ToString());
        // set the dropdown to appear below the selected segment
        infoDropdown.transform.SetSiblingIndex(index + 1);

        // set dropdown info text for base info

        tempType = records[index]["type"];
        tempRace = records[index]["race"];

        cardName.text = records[index]["name"];
        cardText.text = records[index]["desc"];
        cardID = records[index]["id"];
        // combine race and type, while also editing type
        cardRT.text = combineRace(tempRace, tempType);

        // if monster
        if(tempType.Contains("Monster")) 
        {
            cardAttr.gameObject.SetActive(true);
            cardAtk.gameObject.SetActive(true);
            cardDef.gameObject.SetActive(true);


            cardAttr.text = records[index]["attribute"];
            cardAtk.text = "Atk: "+records[index]["atk"];
            cardDef.text = "Def: "+records[index]["def"];

            // if XYZ
            if (tempType.Contains("XYZ"))
            {
                cardLR.gameObject.SetActive(true);
                cardLR.text = "Rank: " + records[index]["level"];
            }
            else
            {               
                cardLR.gameObject.SetActive(true);
                cardLR.text = "Level: " + records[index]["level"];
            }

            // if Pend
            if (tempType.Contains("Pend"))
            {
                cardScale.gameObject.SetActive(true);
                cardScale.text = "Scale: " + records[index]["scale"];
            }
            else
            {
                cardScale.gameObject.SetActive(false);
            }

            // if link
            if (tempType.Contains("Link"))
            {
                cardLink.gameObject.SetActive(true);
                cardLR.gameObject.SetActive(false);
                cardLink.text = "Link: " + records[index]["linkval"];

                cardDef.text = "-";
            }
            else
            {
                cardLink.gameObject.SetActive(false);
            }
        }

        else
        {

            // spells & traps don't have this data, so hide displays
            cardAttr.gameObject.SetActive(false);
            cardAtk.gameObject.SetActive(false);
            cardDef.gameObject.SetActive(false);
            cardLR.gameObject.SetActive(false);
            cardLink.gameObject.SetActive(false);
            cardScale.gameObject.SetActive(false);
        }


        // set image
        
        // search for card ID to get card image
        bool imageSaved = false;
        bool gettingImage = false;

        // check to see if already attempted to recieve image
        for (int i = 0; i < AppManager.instance.ImageRequests.Count; i++)
        {
            if (AppManager.instance.ImageRequests[i].Equals(records[index]["id"]))
            {
                Debug.Log("Already downloading image");
                gettingImage = true;
                break;
            }
        }

        // check to see if saved

        for (int i = 0; i < AppManager.instance.ImageStorage.Count; i++)
        {
            //Debug.Log("saved image ids: "+ AppManager.instance.ImageStorage[i].cardID);
            if (AppManager.instance.ImageStorage[i].cardID.Equals(records[index]["id"]))
            {
                imageSaved = true;
                //Debug.Log("Already saved image: slot: "+i);
                // set image to already saved image
                cardArt.sprite = AppManager.instance.ImageStorage[i].croppedURL;
                 //break;
            }
        }

        // if no image, download image and save to list
        if (imageSaved == false && gettingImage == false)
        {
            String imageID = records[index]["id"];
            AppManager.instance.StartCoroutine("GetImageCropped", imageID );
        }
    }

    public void OnShowFromDeck(DeckBuild.Card currentCard)
    {

        // if we're pressing the card that's already open, close the dropdown
        if (currentCard.name.Equals(cardName.text) && infoDropdown.gameObject.activeInHierarchy)
        {
            // include the info dropdown height
            container.sizeDelta = new Vector2(container.sizeDelta.x, container.sizeDelta.y - infoDropdown.sizeDelta.y);

            infoDropdown.gameObject.SetActive(false);

            DeckButtons.gameObject.SetActive(false);
            MainMenuButtons.gameObject.SetActive(true);

            return;
        }

        // if dropdown isn't open
        else if (!infoDropdown.gameObject.activeInHierarchy)
        {
            // include the info dropdown height
            container.sizeDelta = new Vector2(container.sizeDelta.x, container.sizeDelta.y + infoDropdown.sizeDelta.y);

            // activate buttons
            DeckButtons.gameObject.SetActive(true);
            MainMenuButtons.gameObject.SetActive(false);

        }

        infoDropdown.gameObject.SetActive(true);

        // set dropdown info text for base info

        tempType = currentCard.type;
        tempRace = currentCard.race;

        cardName.text = currentCard.name;
        cardText.text = currentCard.desc;
        cardID = currentCard.id;

        // combine race and type, while also editing type
        cardRT.text = combineRace(tempRace, tempType);

        // if monster
        if (tempType.Contains("Monster"))
        {
            cardAttr.gameObject.SetActive(true);
            cardAtk.gameObject.SetActive(true);
            cardDef.gameObject.SetActive(true);


            cardAttr.text = currentCard.attr;
            cardAtk.text = "Atk: " + currentCard.atk;
            cardDef.text = "Def: " + currentCard.def;
            

            // if XYZ
            if (tempType.Contains("XYZ"))
            {
                cardLR.gameObject.SetActive(true);
                cardLR.text = "Rank: " + currentCard.level;
            }
            else
            {
                cardLR.gameObject.SetActive(true);
                cardLR.text = "Level: " + currentCard.level;
            }

            // if Pend
            if (tempType.Contains("Pend"))
            {
                cardScale.gameObject.SetActive(true);
                cardScale.text = "Scale: " + currentCard.scale;
            }
            else
            {
                cardScale.gameObject.SetActive(false);
            }

            // if link
            if (tempType.Contains("Link"))
            {
                cardLink.gameObject.SetActive(true);
                cardLR.gameObject.SetActive(false);
                cardLink.text = "Link: " + currentCard.linkVal;

                cardDef.text = "-";
            }
            else
            {
                cardLink.gameObject.SetActive(false);
            }
        }

        else
        {

            // spells & traps don't have this data, so hide displays
            cardAttr.gameObject.SetActive(false);
            cardAtk.gameObject.SetActive(false);
            cardDef.gameObject.SetActive(false);
            cardLR.gameObject.SetActive(false);
            cardLink.gameObject.SetActive(false);
            cardScale.gameObject.SetActive(false);
        }


        // set image

        // search for card ID to get card image
        bool imageSaved = false;
        bool gettingImage = false;

        // check to see if already attempted to recieve image
        for (int i = 0; i < AppManager.instance.ImageRequests.Count; i++)
        {
            if (AppManager.instance.ImageRequests[i].Equals(currentCard.id))
            {
                Debug.Log("Already downloading image");
                gettingImage = true;
                break;
            }
        }

        // check to see if saved

        for (int i = 0; i < AppManager.instance.ImageStorage.Count; i++)
        {
            //Debug.Log("saved image ids: "+ AppManager.instance.ImageStorage[i].cardID);
            if (AppManager.instance.ImageStorage[i].cardID.Equals(currentCard.id))
            {
                imageSaved = true;
                //Debug.Log("Already saved image: slot: "+i);
                // set image to already saved image
                cardArt.sprite = AppManager.instance.ImageStorage[i].croppedURL;
                //break;
            }
        }

        // if no image, download image and save to list
        if (imageSaved == false && gettingImage == false)
        {
            String imageID = currentCard.id;
            AppManager.instance.StartCoroutine("GetImageCropped", imageID);
        }
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
            if(Input.Contains("Pendulum"))
            {
                return "Pend Fusion";
            } else
            {
                return "Fusion";
            }
        }

        else if (Input.Contains("Synchro"))
        {
            if (Input.Contains("Pendulum"))
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
            if (Input.Contains("Pendulum"))
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
            if (Input.Contains("Pendulum"))
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
            if (Input.Contains("Pendulum"))
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
            if (Input.Contains("Pendulum"))
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

    public String combineRace(String race, String type)
    {
        String[] inputs = { race, type};

        for (int i = 0;i < inputs.Length;i++)
        {
            string temp = inputs[i];
            int lastSpace = temp.LastIndexOf(" ");
            
            if (lastSpace > -1)
            {
                inputs[i] = temp.Remove(lastSpace);
            }
            else
            {
                inputs[i] = temp;
            }
        }
        return inputs[0]+" / "+ inputs[1].Replace(" ", " / ");
    }
}



