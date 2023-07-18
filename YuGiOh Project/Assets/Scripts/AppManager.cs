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
    public enum Filters
    {
        All = 0,
        Monster = 1,
        Spell = 2,
        Trap = 3, 
        Extra = 4
    }

    // API url
    public string url;
    public string cropImageURL;
    public List<string> ImageRequests;

    // JSON from API request
    public JSONNode jsonResult;

    // JSON filtered
    public JSONNode jsonFilter;

    public static AppManager instance;

    public class CardImages
    {
        public String cardID;
        public Image smallURL;
        public Sprite croppedURL;
        public Image largeURL;

        public CardImages(String ID, Image small, Sprite crop, Image large)
        {
            cardID = ID;
            smallURL = small;
            croppedURL = crop;
            largeURL = large;
        }


        public CardImages(String ID, Sprite crop)
        {
            cardID = ID;
            croppedURL = crop;
        }
    }

    public List<CardImages> ImageStorage = new List<CardImages>();

    void Awake() 
    {
        instance = this;
    }

    IEnumerator GetData (string cardName) 
    {

        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // url and query
        webReq.url = String.Format("{0}?fname={1}", url, cardName);


        Debug.Log("attempt to search: " + webReq.url);
        yield return webReq.SendWebRequest();

        string rawJson = Encoding.Default.GetString(webReq.downloadHandler.data);

        jsonResult = JSON.Parse(rawJson);
        AppManager.instance.jsonFilter = jsonResult["data"];

        //Debug.Log("Test result: "+ jsonResult.ToString());
        // display results
        UI.instance.SetSegments(jsonResult["data"]);

    }

    public void FilterByExampleButton (int filterIndex)
    {
        UI.instance.infoDropdown.gameObject.SetActive(false);
        Filters fil = (Filters)filterIndex;

        // get unedited array of records
        JSONArray records = jsonResult["data"].AsArray;

        string filter = "9999";

        // filter by card type
        switch(fil)
        {
            case Filters.All:
                filter = "All";
                break;

            case Filters.Monster:
                filter = "Monster";
                break;

            case Filters.Spell:
                filter = "Spell";
                break;

            case Filters.Trap:
                filter = "Trap";
                break;

            case Filters.Extra:
                filter = "Extra";
                break;
        }

        if (filter.Equals("All"))
        {
            AppManager.instance.jsonFilter = AppManager.instance.jsonResult["data"];
            UI.instance.SetSegments(records);
        } 
        
        // extra deck monsters have multiple types
        else if(filter.Equals("Extra"))
        {
            // create a new array of filtered results
            JSONArray filteredRecords = new JSONArray();

            for (int i = 0; i < records.Count; i++)
            {
                // get card type
                String recordCardType = records[i]["type"];


                // compare card type with required type
                if (recordCardType.Contains("Fusion") || recordCardType.Contains("Synchro") 
                    || recordCardType.Contains("XYZ") || recordCardType.Contains("Link"))
                {
                    filteredRecords.Add(records[i]);
                    //Debug.Log("filter records data test: extra " + records[i].ToString());
                }
            }

            AppManager.instance.jsonFilter = filteredRecords;

            // display the results on screen
            UI.instance.SetSegments(filteredRecords);
        }

        // monsters have multiple types, removing extra deck types
        else if (filter.Equals("Monster"))
        {
            // create a new array of filtered results
            JSONArray filteredRecords = new JSONArray();

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
                    Debug.Log("filter records data test: mon main " + records[i].ToString());
                }
            }

            AppManager.instance.jsonFilter = filteredRecords;
            //Debug.Log("monster filter print filtered: " + AppManager.instance.jsonFilter.ToString());
            // display the results on screen
            UI.instance.SetSegments(filteredRecords);
        }

        else
        {
            // create a new array of filtered results
            JSONArray filteredRecords = new JSONArray();

            for (int i = 0; i < records.Count; i++)
            {
                // get card type
                String recordCardType = records[i]["type"];

                // compare card type with required type
                if (recordCardType.Contains(filter))
                {
                    filteredRecords.Add(records[i]);
                    //Debug.Log("filter records data test: s/t " + records[i].ToString());
                }

            }

            AppManager.instance.jsonFilter = filteredRecords;

            // display the results on screen
            UI.instance.SetSegments(filteredRecords);
        }
        

    }

    IEnumerator GetImage(string cardID)
    {
        //Debug.Log("Attempting download of image: "+cardID.ToString());
        AppManager.instance.ImageRequests.Add(cardID);

        UnityWebRequest webReq = UnityWebRequestTexture.GetTexture(String.Format("{0}{1}.jpg", cropImageURL, cardID));

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

            AppManager.instance.ImageStorage.Add(new CardImages(cardID, newSprite));
            UI.instance.cardArt.sprite = newSprite;
        }
    }
}

