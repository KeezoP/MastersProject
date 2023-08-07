using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveDeck : MonoBehaviour
{

    public void SaveJSON(DeckData deckToSave)
    {
        string deck = JsonUtility.ToJson(deckToSave);
        Debug.Log("Deck : "+deck);
        

        System.IO.File.WriteAllText(Application.persistentDataPath +
            "/" + deckToSave.deckName.ToString() + ".json", deck);

        Debug.Log("Saving to: " + Application.persistentDataPath);
    }


}
[Serializable]
public class DeckData
{
    public string deckName;
    public string deckNotes;
    public List<JCard> CardList = new();

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
    // every card has a numerical id
    public string id;
    public int MainCopies;
    public int SideCopies;
    public int ExtraCopies;
    public string ban_tcg;
    public string ban_goat;
    public string ban_ocg;

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
}
