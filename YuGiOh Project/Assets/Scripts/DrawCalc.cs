using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawCalc : MonoBehaviour
{

    private bool AddingToHand;
    public Button toggleHandButton;
    public GameObject DrawCalcScreen;
    public RectTransform DCDeckContainer;
    public RectTransform DCHandContainer;
    public List<GameObject> Prefabs;
    public List<GameObject> PrefabsHand;
    public List<DCCard> DeckCards;

    public void Awake()
    {
        AddingToHand = true;
        Prefabs = new();
        DeckCards = new();
    }

    public class DCCard
    {
        public int currentCopies;
        public int totalCopies;
        public string cardName;


        public DCCard(string cardName, int total)
        {
            this.cardName = cardName;
            this.totalCopies = total;
            this.currentCopies = total;
            
        }
    }

    public void ToggleAddingToHand()
    {
        if(AddingToHand)
        {
            AddingToHand = false;
            toggleHandButton.GetComponentInChildren<TextMeshProUGUI>().text = "Add To Hand?: No";
        } 
        
        else
        {
            AddingToHand = true;
            toggleHandButton.GetComponentInChildren<TextMeshProUGUI>().text = "Add To Hand?: Yes";
        }
    }

    public void LoadDrawView()
    {
        GameObject canvas = GameObject.Find("Canvas");

        canvas.transform.Find("CardList").gameObject.SetActive(false);
        canvas.transform.Find("CardSearch").gameObject.SetActive(false);
        canvas.transform.Find("DeckBuild").gameObject.SetActive(false);
        canvas.transform.Find("DeckView").gameObject.SetActive(false);
        canvas.transform.Find("MainMenuButtons").gameObject.SetActive(false);

        DrawCalcScreen.SetActive(true);
        AppManager.instance.FilterDeck(3);
        List<GameObject> cardPrefabs = AppManager.instance.GetComponent<DeckBuild>().cardPrefabs;
        List<DeckBuild.Card> DeckList = AppManager.instance.GetComponent<DeckBuild>().DeckList;

        // for each card in deck

        // i should already have prefabs in cardPrefabs;
        List<GameObject> MonsterPrefabs = new();
        List<GameObject> SpellTrapPrefabs = new();
        List<GameObject> SortedPrefabs = new();
        List<GameObject> MainPrefabs = new();


        // sort decklist prefabs into 3: Monster, ST, Extra
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject temp;
            temp = cardPrefabs[i];

            if (DeckList[i].type.Contains("Spell") || DeckList[i].type.Contains("Trap"))
            {
                SpellTrapPrefabs.Add(temp);
            }
            else if(!(DeckList[i].type.Contains("Fusion") || DeckList[i].type.Contains("Synchro")
                            || DeckList[i].type.Contains("XYZ") || DeckList[i].type.Contains("Link")))
            {
                MonsterPrefabs.Add(temp);
            }
        }

        SortedPrefabs.AddRange(MonsterPrefabs);
        SortedPrefabs.AddRange(SpellTrapPrefabs);

        // split sorted into main side

        // for every card in sorted prefab list.
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

                    mCopies = DeckList[k].MainCopies;

                    
                    if (mCopies > 0)
                    {
                        DeckCards.Add(new DCCard(DeckList[k].name, mCopies));
                        //Debug.Log("Adding: "+DeckCards.Last<DCCard>().cardName + ": "+mCopies+" Copies");
                        if (mCopies > 1)
                            SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        else
                            SortedPrefabs[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

                        GameObject tempPrefabs = Instantiate(SortedPrefabs[j]);
                        MainPrefabs.Add(tempPrefabs);
                    }

                    break;
                }
            }
        }

        for (int p = 0; p < MainPrefabs.Count; p++)
        {
            MainPrefabs[p].transform.SetParent(DCDeckContainer.transform);
            MainPrefabs[p].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            string temp = MainPrefabs[p].name;
            MainPrefabs[p].GetComponent<Button>().onClick.AddListener(() => { ClickCard(temp); });
        }

        int divby5 = MainPrefabs.Count;
        divby5 /= 5; divby5 += 1;

        DCDeckContainer.sizeDelta = new Vector2(DCDeckContainer.sizeDelta.x, UI.instance.GetViewContainerHeight(divby5));

        Prefabs = MainPrefabs;
    }

    public void ClickCard(string searchName)
    {
        //Debug.Log("Searching for: " + searchName);
        for(int i = 0;i<DeckCards.Count;i++)
        {
            if(searchName.Contains(DeckCards[i].cardName))
            {
                int total = DeckCards[i].totalCopies;
                int current = DeckCards[i].currentCopies;

                if (current > 0)
                {
                    current--;
                    DeckCards[i].currentCopies = current;
                    
                    if(AddingToHand)
                    {
                        AddToHand(i);
                    }



                    // updated prefabs
                    UpdatePrefab(Prefabs[i], current);

                }
                else
                {
                    return;
                }




                //Debug.Log("This is "+DeckCards[i].cardName +" || "+DeckCards[i].totalCopies+" total cards");
            }
        }

    }

    public void UpdatePrefab(GameObject Prefab, int current)
    {
        switch (current)
        {
            case 0:
                Prefab.transform.GetChild(1).gameObject.SetActive(true);
                break;

            case 1:
                Prefab.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                Prefab.transform.GetChild(1).gameObject.SetActive(false);
                break;

            case 2:
                Prefab.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                Prefab.transform.GetChild(1).gameObject.SetActive(false);
                Prefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = UI.instance.TwoCard;
                break;

            case 3:
                Prefab.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                Prefab.transform.GetChild(1).gameObject.SetActive(false);
                Prefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = UI.instance.ThreeCard;
                break;
        }
    }

    public void ResetDrawCalc()
    {
        // reset deckdata and prefabs in deck section
        for (int i = 0;i<DeckCards.Count;i++)
        {
            DeckCards[i].currentCopies = DeckCards[i].totalCopies;
            UpdatePrefab(Prefabs[i], DeckCards[i].currentCopies);
        }

        // reset hand section
        if (PrefabsHand.Count > 0)
            PrefabsHand.Clear();
        
        foreach (Transform child in DCHandContainer.transform)
            Destroy(child.gameObject);
    }

    public float CalcHandWidth(int count)
    {

        //Debug.Log(count+": visible");
        float width = 0.0f;

        // include all segment widths
        width += count * (PrefabsHand[0].GetComponent<RectTransform>().sizeDelta.x * 0.75f);

        // include the spacing between segments
        width += count * DCHandContainer.GetComponent<HorizontalLayoutGroup>().spacing;


        return width - 800;
        
    }

    public void AddToHand(int i)
    {
        GameObject tempPrefabs = Instantiate(Prefabs[i]);

        tempPrefabs.transform.SetParent(DCHandContainer.transform);
        tempPrefabs.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        PrefabsHand.Add(tempPrefabs);
        PrefabsHand.Last<GameObject>().transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        DCHandContainer.sizeDelta = new Vector2(CalcHandWidth(PrefabsHand.Count), DCHandContainer.sizeDelta.y);
    }

    public void DrawOneCard()
    {
        List<int> CardPos = new();

        for (int i =0;i<DeckCards.Count;i++)
        {
            if (DeckCards[i].currentCopies > 0)
                CardPos.Add(i);
        }

        if (CardPos.Count > 0)
        {
            int randomCard = Random.Range(0, CardPos.Count);

            DeckCards[CardPos[randomCard]].currentCopies--;

            AddToHand(CardPos[randomCard]);
            UpdatePrefab(Prefabs[CardPos[randomCard]], DeckCards[CardPos[randomCard]].currentCopies);
        }
    }

    public void DrawStartingHand()
    {
        ResetDrawCalc();
        for(int i =0;i<5;i++)
        {
            DrawOneCard();
        }
    }

    public void Return()
    {
        ResetDrawCalc();
        DrawCalcScreen.SetActive(false);
    }
}
