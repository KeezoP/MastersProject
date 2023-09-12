using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System.Text;
using System;
using System.Linq;
using SimpleJSON;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    // Card Search Filter
    public enum Filters
    {
        All = 0,
        Monster = 1,
        Spell = 2,
        Trap = 3, 
        Extra = 4
    }
    
    // Deck Build Filter
    public enum FiltersDeck
    {
        Monster = 0,
        SpellTrap = 1,
        Extra = 2,
        All = 3
    }

    public string url; // API url
    public string cropImageURL; // cropped image url
    public string smallImageURL; // small image url

    public List<string> ImageRequests; // list of cropped card image requests
    public List<string> ImageRequestsSmall; // list of small card image requests
    public List<CardImages> ImageStorage = new(); // list of images received
    public int DeckFilter = 9999;

    // JSON from API request
    public JSONNode jsonResult; // original from API request
    public JSONNode jsonFilter; // filter of jsonResult
    public JSONNode stapleCards; // storage for API Staple request
    public bool haveStaples; // prevents repeat staple requests

    public static AppManager instance;

    // Class used to store images, grouped by card id
    public class CardImages
    {
        public String cardID;
        public Sprite smallURL;
        public Sprite croppedURL;
        public Image largeURL; // not currently used

        // set CardImages
        public CardImages(String ID, Sprite sprite, int choice)
        {
            cardID = ID;

            // store image in location based on choice input
            switch(choice)
            {
                case 0:
                    croppedURL = sprite;
                    break;
                case 1:
                    smallURL = sprite;
                    break;
            }

            
        }
    }

    

    void Awake() 
    {
        instance = this;
        haveStaples = false;
    }

    void Start()
    {
        Screen.SetResolution(400, 711, false);
    }

    // Method filters Card Search results
    public void FilterByExampleButton (int filterIndex)
    {
        // get unedited array of records
        JSONArray records = jsonResult["data"].AsArray;

        // get users filter choice
        Filters fil = (Filters)filterIndex;
        string filter = "9999";


        // prepare UI
        UI.instance.infoDropdown.gameObject.SetActive(false);
        UI.instance.DeckButtons.gameObject.SetActive(false);
        UI.instance.MainMenuButtons.gameObject.SetActive(true);

        // reset UI button colour
        UI.instance.FilterButtons.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        UI.instance.FilterButtons.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        UI.instance.FilterButtons.transform.GetChild(2).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        UI.instance.FilterButtons.transform.GetChild(3).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        UI.instance.FilterButtons.transform.GetChild(4).GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        // filter by card type
        // switch updates filter and colour of selected filter button
        switch (fil)
        {
            case Filters.All:
                filter = "All";
                UI.instance.FilterButtons.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.7f, 1.0f);
                break;

            case Filters.Monster:
                filter = "Monster";
                UI.instance.FilterButtons.transform.GetChild(1).GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.7f, 1.0f);
                break;

            case Filters.Spell:
                filter = "Spell";
                UI.instance.FilterButtons.transform.GetChild(2).GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.7f, 1.0f);
                break;

            case Filters.Trap:
                filter = "Trap";
                UI.instance.FilterButtons.transform.GetChild(3).GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.7f, 1.0f);
                break;

            case Filters.Extra:
                filter = "Extra";
                UI.instance.FilterButtons.transform.GetChild(4).GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.7f, 1.0f);
                break;
        }

        // display original data
        if (filter.Equals("All"))
        {
            AppManager.instance.jsonFilter = AppManager.instance.jsonResult["data"];
            UI.instance.SetSegments(records);
        } 
        
        // display filtered data for extra deck monsters
        else if(filter.Equals("Extra"))
        {
            // create a new array of filtered results
            JSONArray filteredRecords = new JSONArray();

            // for every card, check if meets filter
            for (int i = 0; i < records.Count; i++)
            {
                // get card type
                String recordCardType = records[i]["type"];

                // compare card type with required type
                if (recordCardType.Contains("Fusion") || recordCardType.Contains("Synchro") 
                    || recordCardType.Contains("XYZ") || recordCardType.Contains("Link"))
                {
                    filteredRecords.Add(records[i]);
                }
            }

            // set to filtered cards
            AppManager.instance.jsonFilter = filteredRecords;

            // display the results on screen
            UI.instance.SetSegments(filteredRecords);
        }

        // display filtered data for main deck monsters
        else if (filter.Equals("Monster"))
        {
            // create a new array of filtered results
            JSONArray filteredRecords = new JSONArray();

            // for every card, check if meets filter
            for (int i = 0; i < records.Count; i++)
            {
                // get card type
                String recordCardType = records[i]["type"];

                // compare card type with required type
                if (!recordCardType.Contains("Fusion") && !recordCardType.Contains("Synchro") 
                    && !recordCardType.Contains("XYZ") && !recordCardType.Contains("Link") 
                    && recordCardType.Contains("Monster"))
                {
                    filteredRecords.Add(records[i]);
                }
            }

            // set to filtered cards
            AppManager.instance.jsonFilter = filteredRecords;

            // display the results on screen
            UI.instance.SetSegments(filteredRecords);
        }

        // display filtered data for spells / traps
        else
        {
            // create a new array of filtered results
            JSONArray filteredRecords = new JSONArray();

            // for every card, check if meets filter
            for (int i = 0; i < records.Count; i++)
            {
                // get card type
                String recordCardType = records[i]["type"];

                // compare card type with required type
                if (recordCardType.Contains(filter))
                {
                    filteredRecords.Add(records[i]);
                }
            }

            // set to filtered cards
            AppManager.instance.jsonFilter = filteredRecords;

            // display the results on screen
            UI.instance.SetSegments(filteredRecords);
        }
    }

    // Method filters Deck Builder
    public void FilterDeck(int filterIndex)
    {
        // update filter
        if(filterIndex != 3)
            AppManager.instance.DeckFilter = filterIndex;
        
        List<DeckBuild.Card> Deck = AppManager.instance.GetComponent<DeckBuild>().DeckList;
        List<DeckBuild.Card> NewDeck = new List<DeckBuild.Card>();
        applyBanlist();
        FiltersDeck fil = (FiltersDeck)filterIndex;
        
        string filter = "9999";

        // reset colours
        UI.instance.DeckFilterButtons.transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,0.5f);
        UI.instance.DeckFilterButtons.transform.GetChild(1).GetComponent<Image>().color = new Color(0,0,0,0.5f);
        UI.instance.DeckFilterButtons.transform.GetChild(2).GetComponent<Image>().color = new Color(0,0,0,0.5f);

        // filter by card type
        switch (fil)
        {

            case FiltersDeck.Monster:
                filter = "Monster";
                UI.instance.DeckFilterButtons.transform.GetChild(0).GetComponent<Image>().color = new Color(0.44f, 0.7f, 0.8f, 1.0f);
                break;

            case FiltersDeck.SpellTrap:
                filter = "SpellTrap";
                UI.instance.DeckFilterButtons.transform.GetChild(1).GetComponent<Image>().color = new Color(0.44f, 0.7f, 0.8f, 1.0f);
                break;

            case FiltersDeck.Extra:
                filter = "Extra";
                UI.instance.DeckFilterButtons.transform.GetChild(2).GetComponent<Image>().color = new Color(0.44f, 0.7f, 0.8f, 1.0f);
                break;

            case FiltersDeck.All:
                filter = "All";
                break;
        }

        
        // extra deck monsters have multiple types
        if (filter.Equals("Extra"))
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                // get card type
                String recordCardType = Deck[i].type;


                // compare card type with required type
                if (recordCardType.Contains("Fusion") || recordCardType.Contains("Synchro")
                    || recordCardType.Contains("XYZ") || recordCardType.Contains("Link"))
                {
                    NewDeck.Add(Deck[i]);
                }
            }

            AppManager.instance.GetComponent<DeckBuild>().FilteredDeckList = NewDeck;

            // display the results on screen
            UI.instance.displayFilterCards(NewDeck);
        }
        
        // extra deck monsters have multiple types
        else if (filter.Equals("Monster"))
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                // get card type
                String recordCardType = Deck[i].type;


                // compare card type with required type
                if (!recordCardType.Contains("Fusion") && !recordCardType.Contains("Synchro")
                    && !recordCardType.Contains("XYZ") && !recordCardType.Contains("Link")
                    && recordCardType.Contains("Monster"))
                {
                    NewDeck.Add(Deck[i]);
                }
            }

            AppManager.instance.GetComponent<DeckBuild>().FilteredDeckList = NewDeck;

            // display the results on screen
            UI.instance.displayFilterCards(NewDeck);
        }
        // display all cards
        else if(filter.Equals("All"))
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                NewDeck.Add(Deck[i]);
            }

            AppManager.instance.GetComponent<DeckBuild>().FilteredDeckList = NewDeck;
            UI.instance.displayFilterCards(NewDeck);
        }
        // display Spell/Trap cards
        else
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                // get card type
                String recordCardType = Deck[i].type;

                // compare card type with required type
                if (recordCardType.Contains("Spell") || recordCardType.Contains("Trap"))
                {
                    NewDeck.Add(Deck[i]);
                }
            }

            AppManager.instance.GetComponent<DeckBuild>().FilteredDeckList = NewDeck;

            // display the results on screen
            UI.instance.displayFilterCards(NewDeck);
        }
    }

    // Method updates prefabs to display ban list
    public void applyBanlist()
    {
        List<DeckBuild.Card> Deck = AppManager.instance.GetComponent<DeckBuild>().DeckList;
        List<GameObject> prefabs = AppManager.instance.GetComponent<DeckBuild>().cardPrefabs;
        DeckBuild.DeckFormat DF = (DeckBuild.DeckFormat)AppManager.instance.GetComponent<DeckBuild>().getFormat();

        // get requested ban list data of each card
        for (int i = 0;i<Deck.Count;i++)
        {
            string whatBanlist = "temp";
            switch (DF.ToString())
            {
                case "ALL":
                    break;

                case "TCG":
                    whatBanlist = Deck[i].banlist[0];
                    break;

                case "GOAT":
                    whatBanlist = Deck[i].banlist[1];
                    break;

                case "OCG":
                    whatBanlist = Deck[i].banlist[2];
                    break;
            }

            GameObject banImage = prefabs[i].transform.GetChild(0).GetChild(1).gameObject;

            // display ban list in prefab
            if (!whatBanlist.Equals("temp") && !DF.ToString().Equals("ALL"))
            {

                banImage.gameObject.SetActive(true);

                switch (whatBanlist)
                {
                    case "Banned":

                        banImage.GetComponent<Image>().sprite = UI.instance.IconBan;
                        break;

                    case "Limited":

                        banImage.GetComponent<Image>().sprite = UI.instance.IconLimit;
                        break;
                    case "Semi-Limited":

                        banImage.GetComponent<Image>().sprite = UI.instance.IconSemi;
                        break;

                }
            }
            else
            {
                banImage.gameObject.SetActive(false);
            }
        }
        if(UI.instance.segments.Count > 0)
            UI.instance.updateSegments();
    }

    // Method displays Staple cards
    public void viewStaples()
    {
        // if second method call, display staples
        if (haveStaples == true) {

            // set arrays for records
            AppManager.instance.jsonFilter = AppManager.instance.stapleCards["data"];
            AppManager.instance.jsonResult = AppManager.instance.stapleCards;

            // close info dropdown if open
            if (UI.instance.infoDropdown.gameObject.activeInHierarchy)
                UI.instance.infoDropdown.gameObject.SetActive(false);

            // display data
            UI.instance.SetSegments(AppManager.instance.stapleCards["data"]);
        }
        // else request staples
        else
        {
            AppManager.instance.StartCoroutine("GetStaples");
            haveStaples = true;
        }
    }



    /*  4 Methods for API requests    */

    IEnumerator GetData(string cardName)
    {

        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();

        
        string format = "";

        DeckBuild.DeckFormat tempFormat = (DeckBuild.DeckFormat)AppManager.instance.GetComponent<DeckBuild>().getFormat();
       
        switch(tempFormat.ToString())
        {
            case "ALL":
                break;

            case "TCG":
                format = "&format=tcg";
                break;

            case "GOAT":
                format = "&format=goat";
                break;

            case "OCG":
                format = "&format=ocg";
                break;
        }

        // url and query, broken after 02/09/2023
        //webReq.url = String.Format("{0}?fname={1}&desc={1}{2}", url, cardName,format);
        
        // url temp
        webReq.url = String.Format("{0}?fname={1}{2}", url, cardName,format);

        Debug.Log("attempt to search: " + webReq.url);
        yield return webReq.SendWebRequest();

        string rawJson = Encoding.Default.GetString(webReq.downloadHandler.data);

        jsonResult = JSON.Parse(rawJson);
        AppManager.instance.jsonFilter = jsonResult["data"];

        Debug.Log("result: " + jsonResult.ToString());
        // display results
        UI.instance.SetSegments(jsonResult["data"]);

    }

    IEnumerator GetImageCropped(string cardID)
    {
        Debug.Log("Attempting download of image: "+cardID.ToString());
        AppManager.instance.ImageRequests.Add(cardID);

        //UnityWebRequest webReq = UnityWebRequestTexture.GetTexture(String.Format("https://cors-anywhere.herokuapp.com/{0}{1}.jpg", cropImageURL, cardID));
        UnityWebRequest webReq = UnityWebRequestTexture.GetTexture(String.Format("{0}{1}.jpg", cropImageURL, cardID));
        /*webReq.SetRequestHeader("Access-Control-Allow-Credentials", "true");
        webReq.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
        webReq.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS"); 
        webReq.SetRequestHeader("Access-Control-Allow-Origin", "*");*/

        
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            // Get downloaded asset bundle
            var texture = DownloadHandlerTexture.GetContent(webReq);

            //Image tempImage = UI.instance.cardArt;
            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            //tempImage.sprite = newSprite;
            bool found = false;
            for (int i = 0; i < ImageStorage.Count; i++)
            {

                if (ImageStorage[i].cardID == cardID)
                {
                    found = true;
                    ImageStorage[i].croppedURL = newSprite;
                }
            }

            // if loaded image
            if (found == false)
            {
                Debug.Log("No cropped saved");
                AppManager.instance.ImageStorage.Add(new CardImages(cardID, newSprite,0));
            }
            UI.instance.cardArt.sprite = newSprite;
        }
    }

    IEnumerator GetImageSmall(DeckBuild.Card newCard)
    {
        //Debug.Log("Attempting download of image: " +String.Format("{0}{1}.jpg", smallImageURL, cardID));
       AppManager.instance.ImageRequestsSmall.Add(newCard.id);

        //UnityWebRequest webReq = UnityWebRequestTexture.GetTexture(String.Format("https://cors-anywhere.herokuapp.com/{0}{1}.jpg", smallImageURL, newCard.id));

        // url and image request
        UnityWebRequest webReq = UnityWebRequestTexture.GetTexture(String.Format("{0}{1}.jpg", smallImageURL, newCard.id));
        
        /*webReq.SetRequestHeader("Access-Control-Allow-Credentials", "true");
        webReq.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
        webReq.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        webReq.SetRequestHeader("Access-Control-Allow-Origin", "*");*/

        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            // Get downloaded asset bundle
            var texture = DownloadHandlerTexture.GetContent(webReq);

            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            //AppManager.instance.ImageStorage.Add(new CardImages(cardID, newSprite));
            //UI.instance.cardArt.sprite = newSprite;

            // every card should have a cropped image stored - Not with loaded however
            bool found = false;

            for(int i = 0; i < ImageStorage.Count; i++) 
            { 
                
                if(ImageStorage[i].cardID == newCard.id)
                {
                    found = true;
                    ImageStorage[i].smallURL = newSprite;
                    GameObject temp = AppManager.instance.GetComponent<DeckBuild>().cardPrefabs.Last();
                    temp.transform.Find("CardImage").GetComponent<Image>().sprite = newSprite;
                }
            }

            // if loaded image
            if(found == false)
            {
                AppManager.instance.ImageStorage.Add(new CardImages(newCard.id, newSprite, 1));
            }

            List<GameObject> ListOfDecks = AppManager.instance.GetComponent<DeckBuild>().cardPrefabs;

            for (int i = 0;i< ListOfDecks.Count;i++)
            {
                if(ListOfDecks[i].name == newCard.name)
                {
                    ListOfDecks[i].transform.Find("CardImage").GetComponent<Image>().sprite = newSprite;
                    break;
                }
            }

            //temp.transform.Find("CardImage").gameObject.SetActive(false);
            
            //Debug.Log("tmp sprite: " + tempSprite.ToString());
            //tempSprite = newSprite;
        }
    }

    IEnumerator GetStaples()
    {

        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // url and query
        webReq.url = String.Format("{0}?staple=yes", url);

        yield return webReq.SendWebRequest();

        string rawJson = Encoding.Default.GetString(webReq.downloadHandler.data);

        jsonResult = JSON.Parse(rawJson);
        AppManager.instance.jsonFilter = jsonResult["data"];
        AppManager.instance.stapleCards = jsonResult;


        // display results
        UI.instance.SetSegments(jsonResult["data"]);
        UI.instance.FilterButtons.gameObject.SetActive(true);
    }
}