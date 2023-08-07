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
    public RectTransform viewContainer;

    // prefab used to display results
    public GameObject segmentPrefab;

    // list of all available segments
    public List<GameObject> segments = new List<GameObject>();

    [Header("Info Dropdown")]

    // info dropdown object
    public RectTransform infoDropdown;
    public RectTransform FilterButtons; 
    public RectTransform DeckButtons;
    public RectTransform MainMenuButtons;

    public String cardID;
    public String tempType;
    public String tempRace;
    public String[] cardBan;
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
    public TMP_Dropdown FormatChoice;

    public Image cardArt;
    public Sprite[] availableImageTypes;
    public Sprite TwoCard;
    public Sprite ThreeCard;
    public Sprite IconBan;
    public Sprite IconLimit;
    public Sprite IconSemi;


    public static UI instance;

    private void Awake()
    {
        instance = this;
        cardBan = new String[3] { "temp", "temp","temp" };
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
        // preload 1 segment
        //PreLoadSegments(1);
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

            //if(records[x]["banlist_info"] != null)
              //  Debug.Log("ban: " + records[x]["banlist_info"].ToString());

            // here we can save banlist info to segments.

            // set imageType
            //String cardType = CalcCardType(records[x]["type"]);
            String cardType = records[x]["frameType"];
            //Debug.Log("cct result: " + cardType);

            
            switch (cardType)
            {
                case "normal":
                    cardTypeImage = availableImageTypes[0];
                    break;
                case "effect":
                    cardTypeImage = availableImageTypes[1];
                    break;
                case "ritual":
                    cardTypeImage = availableImageTypes[2];
                    break;
                case "fusion":
                    cardTypeImage = availableImageTypes[3];
                    break;
                case "synchro":
                    cardTypeImage = availableImageTypes[4];
                    break;
                case "xyz":
                    cardTypeImage = availableImageTypes[5];
                    break;
                case "link":
                    cardTypeImage = availableImageTypes[6];
                    break;
                case "normal_pendulum":
                    cardTypeImage = availableImageTypes[7];
                    break;
                case "effect_pendulum":
                    cardTypeImage = availableImageTypes[8];
                    break;
                case "ritual_pendulum":
                    cardTypeImage = availableImageTypes[9];
                    break;
                case "fusion_pendulum":
                    cardTypeImage = availableImageTypes[10];
                    break;
                case "synchro_pendulum":
                    cardTypeImage = availableImageTypes[11];
                    break;
                case "xyz_pendulum":
                    cardTypeImage = availableImageTypes[12];
                    break;
                case "spell":
                    cardTypeImage = availableImageTypes[13];
                    break;
                case "trap":
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

            // assuming format is all but banlist is tcg

            DeckBuild.DeckFormat tempFormat = (DeckBuild.DeckFormat)AppManager.instance.GetComponent<DeckBuild>().getFormat();
            string banlist = "temp";

            switch (tempFormat.ToString())
            {
                case "ALL":
                    break;

                case "TCG":
                    banlist = "ban_tcg";
                    break;

                case "GOAT":
                    banlist = "ban_goat";
                    break;

                case "OCG":
                    banlist = "ban_ocg";
                    break;
            }




            
            string restriction = "9999";
            

            if(records[x]["banlist_info"][banlist] && !tempFormat.ToString().Equals("ALL"))
            {
                restriction = records[x]["banlist_info"][banlist];
                Transform listCard = segment.transform.Find("BanListImage");
                listCard.gameObject.SetActive(true);

                switch (restriction)
                {
                    case "Banned":
                        
                        listCard.GetComponent<Image>().sprite = IconBan;
                        break;

                    case "Limited":
                        
                        listCard.GetComponent<Image>().sprite = IconLimit;
                        break;

                    case "Semi-Limited":

                        listCard.GetComponent<Image>().sprite = IconSemi;
                        break;
                }
            } else
            {
                segment.transform.Find("BanListImage").gameObject.SetActive(false);
            }

        }



        // set the container size to clamp to the segments
        // CLAMP COPY TO ELSEWHERE
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
        deactivateCards();
        //Debug.Log("Filtering:");
        int tempCount = 0;
        foreach (DeckBuild.Card card in newDeck)
        {
            //Debug.Log("Fil: " + card.name);


            try
            {
                List<GameObject> dupes = new();
                // find every prefab of given card (max 2)
                //Debug.Log("deckContainer count: "+deckContainer.childCount);
                foreach(Transform g in deckContainer.transform) { 
                if(g.gameObject.name == card.name)
                    {
                        dupes.Add(g.gameObject);                       
                    }
                }

                for (int i = 0;i<dupes.Count;i++) {
                    GameObject temp = dupes[i];

                    bool isSide = AppManager.instance.GetComponent<DeckBuild>().isSide;

                    //GameObject copiesRun = temp.transform.Find("CopiesRunImage").gameObject;
                    GameObject copiesRun = temp.transform.GetChild(0).GetChild(0).gameObject;

                    //Debug.Log(card.name + ": mse: " + card.MainCopies + "/" + card.SideCopies + "/" + card.ExtraCopies);
                    
                    // if showing all cards or main deck cards



                    if(!isSide || viewContainer.gameObject.activeInHierarchy == true)
                    {
                        // if main/extra atleast 1
                        if ((card.MainCopies > 0 || card.ExtraCopies > 0))
                        {
                            temp.gameObject.SetActive(true);
                            copiesRun.SetActive(true);

                            if (card.MainCopies == 2 || card.ExtraCopies == 2)
                            {
                                //Debug.Log(card.name+": " +"main 2");
                                copiesRun.GetComponent<Image>().sprite = TwoCard;
                            }

                            else if (card.MainCopies == 3 || card.ExtraCopies == 3)
                            {
                                //Debug.Log(card.name + ": " + "main 3");
                                copiesRun.GetComponent<Image>().sprite = ThreeCard;
                            }
                            else
                            {
                                //Debug.Log(card.name + ": " + "main 1");
                                copiesRun.SetActive(false);
                            }

                            if (!isSide || viewContainer.gameObject.activeInHierarchy == true)
                            {
                                temp.SetActive(true);
                                tempCount++;
                            }
                            else
                            {
                                temp.SetActive(false);
                            }
                        }



                    }

                    // else if show all cards or side deck cards
                    if (isSide || viewContainer.gameObject.activeInHierarchy == true)
                    {
                        if (card.SideCopies > 0)
                        {

                            temp.gameObject.SetActive(true);
                            copiesRun.SetActive(true);

                            if (card.SideCopies == 2)
                            {
                                //Debug.Log(card.name + ": " + "side 2");
                                copiesRun.GetComponent<Image>().sprite = TwoCard;
                            }

                            else if (card.SideCopies == 3)
                            {
                                //Debug.Log(card.name);
                                Debug.Log(card.name + ": " + "side 3");
                                copiesRun.GetComponent<Image>().sprite = ThreeCard;
                            }
                            else
                            {
                                //Debug.Log(card.name + ": " + "side 1");
                                copiesRun.SetActive(false);
                            }

                            if (isSide || viewContainer.gameObject.activeInHierarchy == true)
                            {
                                temp.SetActive(true);
                                tempCount++;
                            }
                            else
                            {
                                temp.SetActive(false);
                            }

                        }
                    }
                    // if side atleast 1

                    if (dupes.Count > 1)
                    {
                        Debug.Log("Dupe found: " + dupes[0].name);
                        // if side atleast 1
                        if (card.SideCopies > 0)
                        {
                            temp = dupes[1];
                            copiesRun = temp.transform.GetChild(0).GetChild(0).gameObject;

                            temp.gameObject.SetActive(true);
                            copiesRun.SetActive(true);

                            if (card.SideCopies == 2)
                            {
                                //Debug.Log("side 2 dupe");
                                copiesRun.GetComponent<Image>().sprite = TwoCard;
                            }

                            else if (card.SideCopies == 3)
                            {
                                //Debug.Log("side 3 dupe");
                                copiesRun.GetComponent<Image>().sprite = ThreeCard;
                            }
                            else
                            {
                               //Debug.Log("side 1 dupe");
                                copiesRun.SetActive(false);
                            }

                            if (isSide || viewContainer.gameObject.activeInHierarchy == true)
                            {
                                temp.SetActive(true);
                                tempCount++;
                            }
                            else
                            {
                                temp.SetActive(false);
                            }

                        }
                        break;
                    }

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
        
        deckContainer.sizeDelta = new Vector2(GetDeckWidth(tempCount), deckContainer.sizeDelta.y);


        //deckContainer.sizeDelta = new Vector2(0, container.sizeDelta.y);
    }

    void deactivateCards()
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
    public float GetViewContainerHeight(int count)
    {
        float height = 0.0f;

        // include all segment heights
        height += count * (AppManager.instance.GetComponent<DeckBuild>().cardPrefab.GetComponent<RectTransform>().sizeDelta.y * 0.75f);

        // include the spacing between segments
        height += count * viewContainer.GetComponent<GridLayoutGroup>().spacing.y;

        // include the info dropdown height
        //height += infoDropdown.sizeDelta.y;
        //height -= 1150;
        return height;
    }
    float GetDeckWidth(int count)
    {
        //Debug.Log(count+": visible");
        float width = 0.0f;

        // include all segment widths
        width += count * (AppManager.instance.GetComponent<DeckBuild>().cardPrefab.GetComponent<RectTransform>().sizeDelta.x * 0.75f);

        // include the spacing between segments
        width += count * deckContainer.GetComponent<HorizontalLayoutGroup>().spacing;


        return width - 800;
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
        
        if(records[index]["banlist_info"]["ban_tcg"])
        {
            cardBan[0] = records[index]["banlist_info"]["ban_tcg"];
            
        } 
        else
        {
            cardBan[0] = "temp";
        }
        
        if (records[index]["banlist_info"]["ban_goat"])
        {
            cardBan[1] = records[index]["banlist_info"]["ban_goat"];
        }
        else
        {
            cardBan[1] = "temp";
        }

        if (records[index]["banlist_info"]["ban_ocg"])
        {
            cardBan[2] = records[index]["banlist_info"]["ban_ocg"];
        }
        else
        {
            cardBan[2] = "temp";
        }

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
            if (AppManager.instance.ImageRequests[i].Equals(records[index]["id"]) && AppManager.instance.ImageStorage[i].croppedURL != null)
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
            if (AppManager.instance.ImageStorage[i].cardID.Equals(records[index]["id"]) && AppManager.instance.ImageStorage[i].croppedURL != null)
            {
                imageSaved = true;
                Debug.Log("Already saved image: slot: "+i);
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
            if (AppManager.instance.ImageRequests[i].Equals(currentCard.id) && AppManager.instance.ImageStorage[i].croppedURL != null)
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
            if (AppManager.instance.ImageStorage[i].cardID.Equals(currentCard.id) 
                && AppManager.instance.ImageStorage[i].croppedURL != null)
            {
                imageSaved = true;
                Debug.Log("Already saved image: slot: "+i);
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
        if (input.text.Length - 1 >= 3) 
        {
            foreach (Transform child in container.transform)
            {
                if (!child.name.Equals("InfoDropDown"))
                    Destroy(child.gameObject);
            }
            UI.instance.segments.Clear();

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

    public void returnToDeckSelect()
    {
        // delete old data
        AppManager.instance.GetComponent<DeckBuild>().DeckList.Clear();
        AppManager.instance.GetComponent<DeckBuild>().FilteredDeckList.Clear();
        AppManager.instance.GetComponent<DeckBuild>().cardPrefabs.Clear();
        AppManager.instance.GetComponent<LoadDeck>().loadedDecks.Clear();
        UI.instance.segments.Clear();


        foreach (Transform child in AppManager.instance.GetComponent<LoadDeck>().LDC.transform)
            Destroy(child.gameObject);

        foreach (Transform child in container.transform) { 
            if(!child.name.Equals("InfoDropDown"))
                Destroy(child.gameObject);
        }
            
        
        foreach (Transform child in deckContainer.transform)
            Destroy(child.gameObject);
        
        foreach (Transform child in viewContainer.transform)
            Destroy(child.gameObject);

        AppManager.instance.GetComponent<LoadDeck>().LoadAllDecks();
        AppManager.instance.GetComponent<DrawCalc>().Return();


        if(FilterButtons.gameObject.activeInHierarchy)
        {
            AppManager.instance.jsonResult.Clear();
            AppManager.instance.jsonFilter.Clear();
        }

        // reset visuals 
        GameObject canvas = GameObject.Find("Canvas");
        
        FilterButtons.gameObject.SetActive(false);
        infoDropdown.gameObject.SetActive(false);
        
        canvas.transform.Find("MainMenuButtons").transform.GetChild(1).gameObject.SetActive(false);
        canvas.transform.Find("MainMenuButtons").gameObject.SetActive(false);
        canvas.transform.Find("CardDeckButtons").gameObject.SetActive(false);
        canvas.transform.Find("CardList").gameObject.SetActive(false);
        canvas.transform.Find("CardSearch").gameObject.SetActive(false);
        canvas.transform.Find("DeckBuild").gameObject.SetActive(false);
        canvas.transform.Find("DeckView").gameObject.SetActive(false);
        canvas.transform.Find("DrawCalcView").gameObject.SetActive(false);
        canvas.transform.Find("ChooseDeck").GetChild(0).gameObject.SetActive(true);
        canvas.transform.Find("LoadDeckButtons").gameObject.SetActive(true);
        canvas.transform.Find("Header").GetChild(0).gameObject.SetActive(false);
        canvas.transform.Find("Header").GetChild(2).gameObject.SetActive(false);
    }

    public void updateSegments()
    {
        // calc banlist data here
        JSONNode Deck = AppManager.instance.jsonFilter;
        List<GameObject> prefabs = segments;
        DeckBuild.DeckFormat DF = (DeckBuild.DeckFormat)AppManager.instance.GetComponent<DeckBuild>().getFormat();

        string banlist = "temp";

        switch (DF.ToString())
        {
            case "ALL":
                break;

            case "TCG":
                banlist = "ban_tcg";
                break;

            case "GOAT":
                banlist = "ban_goat";
                break;

            case "OCG":
                banlist = "ban_ocg";
                break;
        }



        for (int i =0;i<Deck.Count;i++)
        {
            string restriction = "9999";


            if (Deck[i]["banlist_info"][banlist] && !DF.ToString().Equals("ALL"))
            {
                restriction = Deck[i]["banlist_info"][banlist];
                Transform listCard = prefabs[i].transform.Find("BanListImage");
                listCard.gameObject.SetActive(true);

                switch (restriction)
                {
                    case "Banned":

                        listCard.GetComponent<Image>().sprite = IconBan;
                        break;
                        
                    case "Limited":
                        
                        listCard.GetComponent<Image>().sprite = IconLimit;
                        break;
                    case "Semi-Limited":
                        
                        listCard.GetComponent<Image>().sprite = IconSemi;
                        break;
                }
            }
            else
            {
                prefabs[i].transform.Find("BanListImage").gameObject.SetActive(false);
            }

        }

    }

    public void changeFormat()
    {
        AppManager.instance.GetComponent<DeckBuild>().DF = (DeckBuild.DeckFormat)UI.instance.FormatChoice.value;
        AppManager.instance.applyBanlist();
    }
}



