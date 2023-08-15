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

        // some cards are restricted
        public string[] banlist = new string[3];

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

    
        public Card(string ID, string name, string desc, string race, string type,
            string attr, string atk, string def, string level, string linkVal, string scale, int m, int s, int e)
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
            this.MainCopies = m;
            this.SideCopies = s;
            this.ExtraCopies = e;

        }
    
    }

    public List<Card> DeckList;
    public List<Card> FilteredDeckList;
    public List<GameObject> cardPrefabs;

    public string deckName;
    public string deckNotes;
    int userChoice;
    public bool isSide;
    public bool showAll;
    public RectTransform container;
    public RectTransform viewContainer;
    public GameObject cardPrefab;

    public GameObject viewScreen;
    public Button showAllButton;

    public List<DeckData> tempList;
    public DeckFormat DF;

    public enum DeckFormat
    {
        ALL = 0,
        TCG = 1,
        GOAT = 2,
        OCG = 3
    }

    public void Awake()
    {
        isSide = false;
        DeckList = new List<Card>();
        cardPrefabs = new List<GameObject>();
        FilteredDeckList = new List<Card>();
        deckName = "default";
        showAll = false;
        tempList = new();
        
        DF = (DeckFormat)0;
        deckNotes = "";
        userChoice = 9999;
    }

    public void setDeckName(string input)
    {
        deckName = input;
    }

    public DeckFormat getFormat()
    {
        return DF;
    }

    public void SwapMainSide()
    {
        if (isSide == false)
        {
            isSide = true;
            // change 2 button text values

            // Main -> Side
            UI.instance.MainSideButtonTop.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: Side";
            UI.instance.MainSideButtonBottom.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: Side";
            //Debug.Log("test 1: tmp :" + UI.instance.MainSideButtonTop.GetComponentInChildren<TextMeshProUGUI>().text);
        }
        else {
            isSide = false;
            // change 2 button text values
            // Side -> Main
            UI.instance.MainSideButtonTop.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: Main";
            UI.instance.MainSideButtonBottom.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: Main";
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

        temp.banlist = UI.instance.cardBan;
        UI.instance.cardBan = new string[] { "temp", "temp", "temp" };

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

        int loadedCopies = newCard.MainCopies + newCard.SideCopies + newCard.ExtraCopies;
        //Debug.Log("Loaded: " + loadedCopies);
        // if not in deck
        if (alreadyExist == false) {

            if (loadedCopies == 0) {
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

                

            }
            // adds new card
            DeckList.Add(newCard);
            //Debug.Log("Unique cards: " + DeckList.Count);
            AddPrefab(newCard);

        }

        

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
        }
        else
        {
            AppManager.instance.FilterDeck(1);
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
                    break;
                }
            }
        }
    }

    public void AddPrefab(Card newCard)
    {
        GameObject deckCard = Instantiate(cardPrefab);

        //Debug.Log("addprefab: "+newCard.name);
        deckCard.name = newCard.name;
        deckCard.transform.SetParent(container.transform);
        deckCard.transform.localScale = new Vector3(0.75f,0.75f,0.75f);

        

        // add OnClick event listener to the button
        cardPrefabs.Add(deckCard);
        deckCard.GetComponent<Button>().onClick.AddListener(() => { UI.instance.OnShowFromDeck(newCard); });
        

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
                //Debug.Log("Already saved image: slot: "+i);
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
            AppManager.instance.StartCoroutine("GetImageSmall", newCard);
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

        int deckSizeMain = 0;
        int deckSizeSide = 0;
        int deckSizeExtra = 0;
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
            cardData.ban_tcg = c.banlist[0];
            cardData.ban_goat = c.banlist[1];
            cardData.ban_ocg = c.banlist[2];
            deck.Add(cardData);

            deckSizeMain += cardData.MainCopies;
            deckSizeSide += cardData.SideCopies;
            deckSizeExtra += cardData.ExtraCopies;
        }

        Debug.Log("saving deck\ndeck size: M("+deckSizeMain+") E("+deckSizeExtra+") S("+deckSizeSide+")");

        DeckData saveDeck = new DeckData(deckName, deck, deckNotes);

        AppManager.instance.GetComponent<SaveDeck>().SaveJSON(saveDeck);

    }

    public void readyDeckEdit()
    {

        

        // ready visuals
        GameObject canvas = GameObject.Find("Canvas");
        canvas.transform.Find("MainMenuButtons").gameObject.SetActive(true);
        canvas.transform.Find("CardList").gameObject.SetActive(true);
        canvas.transform.Find("CardSearch").gameObject.SetActive(true);
        canvas.transform.Find("DeckBuild").gameObject.SetActive(true);
        canvas.transform.Find("ChooseDeck").gameObject.SetActive(false);
        canvas.transform.Find("LoadDeckButtons").gameObject.SetActive(false);
        canvas.transform.Find("Header").GetChild(0).gameObject.SetActive(true);
        canvas.transform.Find("Header").GetChild(2).gameObject.SetActive(true);

        UI.instance.DeckFilterButtons.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        UI.instance.DeckFilterButtons.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        UI.instance.DeckFilterButtons.transform.GetChild(2).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        /*Debug.Log("count: "+AppManager.instance.GetComponent<LoadDeck>().loadedDecks.Count);
        Debug.Log("count: "+AppManager.instance.GetComponent<LoadDeck>().loadedDecks[0].deckName);*/
    }

    public void loadDeck(string userDeckName)
    {
        // we assume that a deck is loaded by user choice, to an already empty deck

        readyDeckEdit();

        //Debug.Log("Button clicked: " + userDeckName);

        

        for(int i = 0;i<tempList.Count;i++)
        {
            if (tempList[i].deckName == userDeckName)
            {
                userChoice = i;
                break;
            }
        }

        if(userChoice == 9999)
        {
            Debug.Log("Can't load this deck");
            return;
        }
        

        //Debug.Log("Load Deck: " + userChoice);

        //tempList = AppManager.instance.GetComponent<LoadDeck>().loadedDecks;

        //Debug.Log("TL Length: " + tempList.Count);

        string nameLoadedDeck = tempList[userChoice].deckName;
        deckNotes = tempList[userChoice].deckNotes;
        List<JCard> CL = tempList[userChoice].CardList;

        /*Debug.Log("Loaded Deck");

        foreach (JCard c in CL)
        {
            Debug.Log("Card: " + c.name);
        }
*/

        deckName = nameLoadedDeck;
        GameObject.Find("DeckName").GetComponent<TMP_InputField>().text = deckName;

        for (int i = 0;i< CL.Count;i++)
        {
            // convert jcard to card
            Card temp = new Card(CL[i].id, CL[i].name, CL[i].desc, CL[i].race, CL[i].type, 
                CL[i].attr, CL[i].atk, CL[i].def, CL[i].level, CL[i].linkVal, CL[i].scale,
                CL[i].MainCopies, CL[i].SideCopies, CL[i].ExtraCopies);

            temp.banlist[0] = CL[i].ban_tcg;
            temp.banlist[1] = CL[i].ban_goat;
            temp.banlist[2] = CL[i].ban_ocg;



            AddCard(temp);
        }


    }

    public void viewDeck() {

        GameObject canvas = GameObject.Find("Canvas");
        
        // if returning to deck edit
        if (viewScreen.activeInHierarchy) {

            foreach (Transform child in viewContainer.transform)
                Destroy(child.gameObject);


            viewScreen.SetActive(false);
            showAllButton.gameObject.SetActive(false);
            AppManager.instance.FilterDeck(AppManager.instance.DeckFilter);


            canvas.transform.Find("CardList").gameObject.SetActive(true);
            canvas.transform.Find("CardSearch").gameObject.SetActive(true);
            canvas.transform.Find("DeckBuild").gameObject.SetActive(true);

            UI.instance.ToggleViewButton.GetComponentInChildren<TextMeshProUGUI>().text = "View:";
            return;
        }

        UI.instance.ToggleViewButton.GetComponentInChildren<TextMeshProUGUI>().text = "Edit:";

        canvas.transform.Find("CardList").gameObject.SetActive(false);
        canvas.transform.Find("CardSearch").gameObject.SetActive(false);
        canvas.transform.Find("DeckBuild").gameObject.SetActive(false);

        showAllButton.gameObject.SetActive(true);
        viewScreen.SetActive(true);
        AppManager.instance.FilterDeck(3);
        GameObject.Find("NotesSection").GetComponent<TMP_InputField>().text = deckNotes;

        //showAll = true;

        // turn off copmonents

        // turn on view deck and button to return to edit
        // show unique/show all

        // for each card in deck

        // i should already have prefabs in cardPrefabs;
        List<GameObject> MonsterPrefabs = new();
        List<GameObject> SpellTrapPrefabs = new();
        List<GameObject> ExtraPrefabs = new();
        List<GameObject> SortedPrefabs = new();
        List<GameObject> MainPrefabs = new();
        List<GameObject> SidePrefabs = new();
        

        // sort decklist prefabs into 3: Monster, ST, Extra
        for (int i = 0;i < cardPrefabs.Count;i++)
        {
            GameObject temp;
            temp = cardPrefabs[i];

            if (DeckList[i].type.Contains("Fusion") || DeckList[i].type.Contains("Synchro")
                            || DeckList[i].type.Contains("XYZ") || DeckList[i].type.Contains("Link"))
            {
                ExtraPrefabs.Add(temp);

            } 
            else if (DeckList[i].type.Contains("Spell") || DeckList[i].type.Contains("Trap"))
            {
                SpellTrapPrefabs.Add(temp);
            } 
            else
            {
                MonsterPrefabs.Add(temp);
            }
        }

        SortedPrefabs.AddRange(MonsterPrefabs);
        SortedPrefabs.AddRange(SpellTrapPrefabs);
        SortedPrefabs.AddRange(ExtraPrefabs);

        // split sorted into main side

        // for every card in sorted prefab list. split main side, include duplicates depending on showall
        for (int j = 0; j < SortedPrefabs.Count; j++)
        {
            SortedPrefabs[j].SetActive(true);
            // find decklist data by comparing names
            for (int k = 0; k < DeckList.Count; k++)
            {
                // find decklist data
                if (DeckList[k].name == SortedPrefabs[j].name)
                {
                    int mCopies = 0;
                    int sCopies = 0;

                    mCopies = DeckList[k].MainCopies + DeckList[k].ExtraCopies;
                    sCopies = DeckList[k].SideCopies;

                    // if showing multiple copies of prefabs, display multiple based on decklist
                    if (showAll)
                    {
                        for (int m = 0; m < mCopies; m++)
                        {
                            SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                            GameObject tempPrefabs = Instantiate(SortedPrefabs[j]);
                            MainPrefabs.Add(tempPrefabs);
                        }
                        for (int m = 0; m < sCopies; m++)
                        {
                            SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                            GameObject tempPrefabs = Instantiate(SortedPrefabs[j]);
                            SidePrefabs.Add(tempPrefabs);
                        }
                    }

                    else
                    {
                        if (mCopies > 0)
                        {
                            if(mCopies > 1) 
                                SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                            else
                                SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

                            GameObject tempPrefabs = Instantiate(SortedPrefabs[j]);
                            MainPrefabs.Add(tempPrefabs);
                        } 
                        if (sCopies > 0)
                        {
                            if (sCopies > 1)
                                SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                            else
                                SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                            GameObject tempPrefabs = Instantiate(SortedPrefabs[j]);
                            SidePrefabs.Add(tempPrefabs);
                        }

                    }
                    break;
                }
            }
        }

        for (int p = 0;p < MainPrefabs.Count;p++)
        {
            MainPrefabs[p].transform.SetParent(viewContainer.transform);
            MainPrefabs[p].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }

        for (int p = 0; p < SidePrefabs.Count; p++)
        {
            SidePrefabs[p].transform.SetParent(viewContainer.transform);
            SidePrefabs[p].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }



        int divby5 = MainPrefabs.Count + SidePrefabs.Count;
        divby5 /= 5; //divby5 += 1;

        viewContainer.sizeDelta = new Vector2(viewContainer.sizeDelta.x, UI.instance.GetViewContainerHeight(divby5));
        //PrintTest();
    }


    public void isShowingAll()
    {

        if (showAll == false)
        {
            showAll = true;
            UI.instance.AllUniqueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: All";
        }
        else
        {
            showAll = false;
            UI.instance.AllUniqueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: Unique";
        }
        viewDeck();
        viewDeck();
    }

    public void saveNotes()
    {
        deckNotes = GameObject.Find("NotesSection").GetComponent<TMP_InputField>().text;
        saveDeck();
    }

}
