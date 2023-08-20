using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class LoadDeck : MonoBehaviour
{
    private void Start()
    {
        LoadAllDecks();
        loadedDecks = new();
    }

    public List<DeckData> loadedDecks;
    public RectTransform LDC;
    public GameObject loadPrefab;
    public TextAsset UnchainedDeck;
    public TextAsset KashtiraPreDeck;
    public TextAsset KashtiraPostDeck;
    public TextAsset RunickFHSDeck;


    public void LoadAllDecks()
    {

        // get all filepaths
        //string[] filePaths = System.IO.Directory.GetFiles(Application.persistentDataPath);
        string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath);
        

        for (int i = 0;i< filePaths.Length;i++)
        {
            // i + 1 is a temporary measure due to streamingassetpath already containing non json file
            string fileData = System.IO.File.ReadAllText(filePaths[i]);

            if (fileData.Contains("deckName")) {
                DeckData deckData = JsonUtility.FromJson<DeckData>(fileData);
                //Debug.Log("deckname: " + deckData.deckName);

                /*for(int j = 0;j<deckData.CardList.Count;j++)
                {
                    Debug.Log(deckData.CardList[j].name);
                }*/

                this.loadedDecks.Add(deckData);
            }


            
        }

        // test decks
       
        /*this.loadedDecks.Add(JsonUtility.FromJson<DeckData>(UnchainedDeck.ToString()));
        this.loadedDecks.Add(JsonUtility.FromJson<DeckData>(KashtiraPreDeck.ToString()));
        this.loadedDecks.Add(JsonUtility.FromJson<DeckData>(KashtiraPostDeck.ToString()));
        this.loadedDecks.Add(JsonUtility.FromJson<DeckData>(RunickFHSDeck.ToString()));*/


        for (int j = 0; j < loadedDecks.Count; j++)
        {
            Debug.Log(loadedDecks[j].deckName);
            GameObject tempButton = Instantiate(loadPrefab);
            tempButton.transform.SetParent(LDC.transform);

            tempButton.name = loadedDecks[j].deckName;
            tempButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedDecks[j].deckName;

            tempButton.GetComponent<Button>().onClick.AddListener(() => {
                AppManager.instance.GetComponent<DeckBuild>().loadDeck(tempButton.name);
            });
        }

        

        AppManager.instance.GetComponent<DeckBuild>().tempList = loadedDecks;

        //AppManager.instance.GetComponent<DeckBuild>().readyDeckEdit();







        //AppManager.instance.GetComponent<DeckBuild>().loadDeck(2);

    }
}


