using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

using System.Linq;

public class DeckBuild : MonoBehaviour
{


    public class Card
    {
        // every card has a numerical id
        public string id;
        public int MainCopies;
        public int SideCopies;
        public int ExtraCopies;

        // every card have these
        public string name;
        public string desc;
        public string race;
        public string type;

        // monster data
        public string attr;
        public string atk;

        // non-link monster data
        public string def;
        public string level;

        // link monster data
        public string linkVal;

        // Pendulum monster data
        public string scale;

        // cards have 3 images available
        public Image smallImage;
        public Image croppedImage;

        // currently not using this type of image
        public Image normalImage;

        // Basic card, typically spell/trap
        public Card(string ID, string name, string desc, string race, string type,
            string attr, string atk, string def, string level, string linkVal, string scale, Image cropped)
        {
            this.id = ID;
            this.name = name;
            this.desc = desc;
            this.race = race;
            this.type = type;
            this.attr = attr;
            this.atk = atk;
            this.def = def;
            this.level = level;
            this.linkVal = linkVal;
            this.scale = scale;

            this.croppedImage = cropped;
        }
    }

    public List<Card> DeckList;
    public List<Card> FilteredDeckList;
    public List<GameObject> cardPrefabs;

    public string deckName;
    public bool isSide;
    public RectTransform container;
    public GameObject cardPrefab;


    public void Awake()
    {
        isSide = false;
        DeckList = new List<Card>();
        cardPrefabs = new List<GameObject>();
        FilteredDeckList = new List<Card>();
        deckName = "default";
    }

    public void setDeckName(string input)
    {
        deckName = input;
    }

    public void SwapMainSide()
    {
        if (isSide == false)
        {
            isSide = true;
            // change 2 button text values

            // Main -> Side
            UI.instance.MainSideButtonTop.GetComponentInChildren<TextMeshProUGUI>().text = "Side: ";
            UI.instance.MainSideButtonBottom.GetComponentInChildren<TextMeshProUGUI>().text = "Side: ";
            //Debug.Log("test 1: tmp :" + UI.instance.MainSideButtonTop.GetComponentInChildren<TextMeshProUGUI>().text);
        }
        else {
            isSide = false;
            // change 2 button text values
            // Side -> Main
            UI.instance.MainSideButtonTop.GetComponentInChildren<TextMeshProUGUI>().text = "Main: ";
            UI.instance.MainSideButtonBottom.GetComponentInChildren<TextMeshProUGUI>().text = "Main: ";
        }

        if (DeckList.Count > 0)
            UI.instance.displayFilterCards(FilteredDeckList);
    }

    public void CreateCard()
    {
        string id = UI.instance.cardID;
        string name = UI.instance.cardName.text;
        string desc = UI.instance.cardText.text;
        string race = UI.instance.tempRace;
        string type = UI.instance.tempType;
        string attr = UI.instance.cardAttr.text;
        string atk = UI.instance.cardAtk.text;
        string def = UI.instance.cardDef.text;
        string level = UI.instance.cardLR.text;
        string linkVal = UI.instance.cardLink.text;
        string scale = UI.instance.cardScale.text;
        Image crop = UI.instance.cardArt;
        

        Card temp = new Card(id,name,desc,race,type,attr,atk,def,level,linkVal,scale,crop);
        AddCard(temp);
    }

    public void AddCard(Card newCard)
    {
        // check to see how many copies of given card exist in available decks 
        int currentCopies = 9999;
        bool alreadyExist = false;
     
        // check DeckList to update existing entry
        for (int i = 0; i < DeckList.Count; i++)
        {
            // if matching card id
            if (DeckList[i].id == newCard.id)
            {
                currentCopies = DeckList[i].MainCopies + DeckList[i].SideCopies + DeckList[i].ExtraCopies;
                alreadyExist = true;
                // if adding card to deck
                if (currentCopies < 3)
                {
                    // is it side Y/N, if N, is it main Y/N
                    if (isSide == true)
                    {

                        
                            DeckList[i].SideCopies++;
                        //UI.instance.displayFilterCards(DeckList);
                        AppManager.instance.FilterDeck(AppManager.instance.DeckFilter);
                    } 
                    
                    else
                    {
                        //Debug.Log("ct more: " + newCard.type);
                        //is extra deck?
                        if (newCard.type.Contains("Fusion") || newCard.type.Contains("Synchro") 
                            || newCard.type.Contains("XYZ") || newCard.type.Contains("Link"))
                        {
                            
                                DeckList[i].ExtraCopies++;
                        } 
                        else
                        {
                            
                                DeckList[i].MainCopies++;
                           
                        }

                        //UI.instance.displayFilterCards(DeckList);
                        AppManager.instance.FilterDeck(AppManager.instance.DeckFilter);
                    }

                    // if exists locally but isn't displaying
                    if (currentCopies == 0)
                    {
                        UI.instance.deckContainer.Find(newCard.name).gameObject.SetActive(true);
                    }
                }
                
                else
                {
                    Debug.Log("Max copies");
                    return;
                }
            }
        }

        // if not in deck
        if (alreadyExist == false) {
            // is it side Y/N, if N, is it main Y/N
            if (isSide == true)
            {
                newCard.SideCopies++;
            }
            else
            {
                //Debug.Log("ct new: " + newCard.type);
                //is extra deck?
                if (newCard.type.Contains("Fusion") || newCard.type.Contains("Synchro")
                    || newCard.type.Contains("XYZ") || newCard.type.Contains("Link"))
                {
                    newCard.ExtraCopies++;
                }
                else
                {
                    newCard.MainCopies++;
                }
            }

            // adds new card
            DeckList.Add(newCard);
            AddPrefab(newCard);

            // calc filter based on card type
            if (newCard.type.Contains("Fusion") || newCard.type.Contains("Synchro")
                    || newCard.type.Contains("XYZ") || newCard.type.Contains("Link")) 
            {
                AppManager.instance.FilterDeck(2);
            }

            else if (!newCard.type.Contains("Fusion") && !newCard.type.Contains("Synchro")
                    && !newCard.type.Contains("XYZ") && !newCard.type.Contains("Link") 
                    && newCard.type.Contains("Monster"))
            {
                AppManager.instance.FilterDeck(0);
            } else
            {
                AppManager.instance.FilterDeck(1);
            }      
        }   
    }

    public void MinusCard()
    {
        string id = UI.instance.cardID;
        Card newCard;

        // check to see how many copies of given card exist in available decks 
        int currentCopies = 9999;

        // check DeckList to update existing entry
        for (int i = 0; i < DeckList.Count; i++)
        {
            // if matching card id
            if (DeckList[i].id == id)
            {
                newCard = DeckList[i];
                currentCopies = DeckList[i].MainCopies + DeckList[i].SideCopies + DeckList[i].ExtraCopies;
                // if adding card to deck
                if (currentCopies > 0)
                {
                    // is it side Y/N, if N, is it main Y/N
                    if (isSide == true)
                    {
                        if (DeckList[i].SideCopies > 0)
                            DeckList[i].SideCopies--;
                        //Debug.Log("-1 side");

                        //UI.instance.displayFilterCards(DeckList);
                        AppManager.instance.FilterDeck(AppManager.instance.DeckFilter);
                    }

                    else
                    {
                        //Debug.Log("ct more: " + newCard.type);
                        //is extra deck?
                        if (newCard.type.Contains("Fusion") || newCard.type.Contains("Synchro")
                            || newCard.type.Contains("XYZ") || newCard.type.Contains("Link"))
                        {
                            if (DeckList[i].ExtraCopies > 0) 
                                DeckList[i].ExtraCopies--;
                        }
                        else
                        {
                            if (DeckList[i].MainCopies > 0)
                                DeckList[i].MainCopies--;
                            //Debug.Log("-1 main");
                        }
                        //UI.instance.displayFilterCards(DeckList);
                        AppManager.instance.FilterDeck(AppManager.instance.DeckFilter);
                    }
                }

                if( currentCopies == 1)
                {
                    //UI.instance.deckContainer.Find(newCard.name).gameObject.SetActive(false);
                    
                    // Destroy gameobject
                    Destroy(UI.instance.deckContainer.Find(newCard.name).gameObject);
                    cardPrefabs.RemoveAt(i);
                    DeckList.RemoveAt(i);

                    for (int j = 0;j< FilteredDeckList.Count;j++)
                    {
                        if(FilteredDeckList[j].id == id)
                        {
                            FilteredDeckList.RemoveAt(j);
                            break;
                        }
                    }
                    
                   
                    Debug.Log("Min copies");
                    PrintTest();
                    break;
                }
            }
        }
    }

    public void AddPrefab(Card newCard)
    {
        GameObject deckCard = Instantiate(cardPrefab);

        //Debug.Log("seg scale test 1: " + segment.transform.localScale.x);
        deckCard.name = newCard.name;
        deckCard.transform.SetParent(container.transform);
        deckCard.transform.localScale = new Vector3(0.75f,0.75f,0.75f);
        //Debug.Log("seg scale test 2: " + segment.transform.localScale.x);


        // add OnClick event listener to the button
        deckCard.GetComponent<Button>().onClick.AddListener(() => { UI.instance.OnShowFromDeck(newCard); });
        cardPrefabs.Add(deckCard);

        // set image

        // search for card ID to get card image
        bool imageSaved = false;
        bool gettingImage = false;

        // check to see if already attempted to recieve image
        for (int i = 0; i < AppManager.instance.ImageRequestsSmall.Count; i++)
        {
            if (AppManager.instance.ImageRequestsSmall[i].Equals(newCard.id))
            {
                //Debug.Log("Already downloading image");
                gettingImage = true;
                break;
            }
        }

        // check to see if saved

        for (int i = 0; i < AppManager.instance.ImageStorage.Count; i++)
        {
            if (AppManager.instance.ImageStorage[i].cardID.Equals(newCard.id) 
                && AppManager.instance.ImageStorage[i].smallURL != null)
            {
                imageSaved = true;
                Debug.Log("Already saved image: slot: "+i);
                // set image to already saved image
                GameObject temp = AppManager.instance.GetComponent<DeckBuild>().cardPrefabs.Last();

                //temp.transform.Find("CardImage").gameObject.SetActive(false);
                temp.transform.Find("CardImage").GetComponent<Image>().sprite = AppManager.instance.ImageStorage[i].smallURL;
                break;
            }
        }

        // if no image, download image and save to list
        if (imageSaved == false && gettingImage == false)
        {
            AppManager.instance.StartCoroutine("GetImageSmall", newCard.id);
        }


    }

    public void PrintTest()
    {
        for (int i = 0;i< DeckList.Count;i++) {
            Debug.Log(DeckList[i].id+": m/s/e: "+ DeckList[i].MainCopies+"/"+ DeckList[i].SideCopies+"/"+ DeckList[i].ExtraCopies+" name: " + DeckList[i].name);
        }
    }

    public List<GameObject> returnPrefabList()
    {
        return cardPrefabs;
    }

    public void saveDeck()
    {
        
        
        List<JCard> deck = new();
        foreach(Card c in DeckList)
        {
            JCard cardData = new();

            cardData.id = c.id;
            cardData.name = c.name;
            cardData.desc = c.desc;
            cardData.race = c.race;
            cardData.type = c.type;
            cardData.attr = c.attr;
            cardData.atk = c.atk;
            cardData.def = c.def;
            cardData.level = c.level;
            cardData.linkVal = c.linkVal;
            cardData.scale = c.scale;
            cardData.MainCopies = c.MainCopies;
            cardData.SideCopies = c.SideCopies;
            cardData.ExtraCopies = c.ExtraCopies;
            deck.Add(cardData);
        }

        Debug.Log("saving deck\ndeck size: "+deck.Count);

        DeckData saveDeck = new DeckData(deckName, deck);

        AppManager.instance.GetComponent<SaveDeck>().SaveJSON(saveDeck);

    }
    // save deck
    // create a json file that saves the following data of each card in the deck:

    // deckname
    // each unique card
    // copies of each unique card
    // json save image?

}
