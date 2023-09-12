using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveDeck : MonoBehaviour
{
    // Method saves current deck
    public void SaveJSON(DeckData deckToSave)
    {
        // create Json structured string of deck
        string deck = JsonUtility.ToJson(deckToSave);

        // save deck
        System.IO.File.WriteAllText(Application.persistentDataPath +
            "/" + deckToSave.deckName.ToString() + ".json", deck);

        //Debug.Log("Saving to: " + Application.persistentDataPath);
    }
}

[Serializable]
public class DeckData
{
    // Class used for Json card data

    public string deckName; // name of deck
    public string deckNotes; // optional deck notes
    public List<JCard> CardList = new(); // list of cards

    // set DeckData
    public DeckData(string name, List<JCard> cards, string notes)
    {
        this.deckName = name;
        this.CardList = cards;
        this.deckNotes = notes;
    }
}

[Serializable]
public class JCard
{
    // Class used for Json card data


    // each card can potentially have these optional values
    public int MainCopies;
    public int SideCopies;
    public int ExtraCopies;
    public string ban_tcg; 
    public string ban_goat; 
    public string ban_ocg;

    // each card will have these values
    public string id;
    public string name;
    public string desc;
    public string race;
    public string type;

    // monster cards will have these values
    public string attr;
    public string atk;

    // non-link monster cards will have these values
    public string def;
    public string level;

    // link monster cards will have this value
    public string linkVal;

    // Pendulum monster cards will have this value
    public string scale;
}
