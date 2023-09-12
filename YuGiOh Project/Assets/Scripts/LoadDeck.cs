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

    // Method loads decks into app
    public void LoadAllDecks()
    {

        // get all filepaths for player made decks
        // should not be commented out but was for survey 2, mistake

        //string[] filePaths = System.IO.Directory.GetFiles(Application.persistentDataPath);

        // get all filepaths for test decks
        string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath);
        
        // for all filepaths
        for (int i = 0;i< filePaths.Length;i++)
        {
            // get the files data
            string fileData = System.IO.File.ReadAllText(filePaths[i]);

            // if file contains data relevant to app
            if (fileData.Contains("deckName")) {

                // Create DeckData from Json file
                DeckData deckData = JsonUtility.FromJson<DeckData>(fileData);
                
                // add DeckData to app
                this.loadedDecks.Add(deckData);
            }   
        }

        // for all loaded decks
        for (int j = 0; j < loadedDecks.Count; j++)
        {
            // create button to load deck
            GameObject tempButton = Instantiate(loadPrefab);

            // update button data
            tempButton.transform.SetParent(LDC.transform);
            tempButton.name = loadedDecks[j].deckName;
            tempButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedDecks[j].deckName;

            // add button listener
            tempButton.GetComponent<Button>().onClick.AddListener(() => {
                AppManager.instance.GetComponent<DeckBuild>().loadDeck(tempButton.name);
            });
        }

        // set templists data to all loaded decks
        AppManager.instance.GetComponent<DeckBuild>().tempList = loadedDecks;
    }
}


