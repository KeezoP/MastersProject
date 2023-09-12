using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawCalc : MonoBehaviour
{

    private bool AddingToHand;
    private bool ShowingPercent;
    private bool isOneNotTwo;
    private bool isTestHands;
    private bool ShowingPercentTCC;
    public Button toggleHandButton;
    public Button togglePercentButton;
    public Button toggleTCCButton;
    public Button OneCCButton;
    public Button TwoCCButton;
    public Button CalcProbButton;
    public Button CalcDesiresButton;

    public GameObject DrawCalcScreen;
    public RectTransform DCDeckContainer;
    public RectTransform DCHandContainer;
    public RectTransform ProbMainMenuButtons;
    public RectTransform ProbFeatureButtons;
    public RectTransform DrawCalcMainMenuButtons;
    public RectTransform DrawCalcFeatureButtons;
    public RectTransform DesiresFilterButtons;
    public List<GameObject> Prefabs;
    public List<GameObject> PrefabsHand;
    public List<GameObject> PrefabsPairs;
    public List<int> PairsperCard;
    public List<DCCard> DeckCards;
    public TextMeshProUGUI probResults;
    public RectTransform Results;
    public int deckSize;
    public int currentDeckSize;
    public int currentDesireTargetCopies;

    public int OneCardComboTargets;
    public int OCCSuccesses;

    double banish0;
    double banish1;
    double banish2;
    double banish3;

    double drawBan0;
    double drawBan1;
    double drawBan2;

    double chance0;
    double chance1;
    double chance2;

    public void Awake()
    {
        AddingToHand = true;
        ShowingPercent = true;
        Prefabs = new();
        DeckCards = new();
        PairsperCard = new();
        OCCSuccesses = 1;
        isOneNotTwo = true;
        isTestHands = true;
        ShowingPercentTCC = true;
        currentDesireTargetCopies = 0;
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

        probResults.text = "Click on Cards in Deck to ";
        if (AddingToHand)
            probResults.text += "Add to Hand";
        else
            probResults.text += "Remove from Deck";
    }

    public void ToggleShowPercent()
    {
        if (ShowingPercent)
        {
            ShowingPercent = false;
            togglePercentButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show %?: No";

            for (int i = 0; i < Prefabs.Count; i++)
            {
                Prefabs[i].transform.GetChild(2).gameObject.SetActive(false);
                Prefabs[i].transform.GetChild(3).gameObject.SetActive(false);
            }

            for (int i = 0; i < PrefabsHand.Count; i++)
            {
                PrefabsHand[i].transform.GetChild(2).gameObject.SetActive(false);
                PrefabsHand[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }

        else
        {
            ShowingPercent = true;
            togglePercentButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show %?: Yes";

            for (int i = 0; i < Prefabs.Count; i++)
            {
                Prefabs[i].transform.GetChild(2).gameObject.SetActive(true);
                Prefabs[i].transform.GetChild(3).gameObject.SetActive(true);
            }

            for (int i = 0; i < PrefabsHand.Count; i++)
            {
                PrefabsHand[i].transform.GetChild(2).gameObject.SetActive(true);
                PrefabsHand[i].transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    public void ToggleShowPercentTCC()
    {
        if (ShowingPercentTCC)
        {
            ShowingPercentTCC = false;
            toggleTCCButton.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: Total Hands";
            CalcTCC(true);
        }

        else
        {
            ShowingPercentTCC = true;
            toggleTCCButton.GetComponentInChildren<TextMeshProUGUI>().text = "Showing: Draw %";
            CalcTCC(true);
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


        probResults.text = "Click on Cards in Deck to ";
        if (AddingToHand)
            probResults.text += "Add to Hand";
        else
            probResults.text += "Remove from Deck";


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
                        deckSize += mCopies;
                    }

                    break;
                }
            }
        }

        for (int p = 0; p < MainPrefabs.Count; p++)
        {
            MainPrefabs[p].transform.SetParent(DCDeckContainer.transform);
            MainPrefabs[p].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            string temp = MainPrefabs[p].name;
            MainPrefabs[p].GetComponent<Button>().onClick.AddListener(() => { ClickCard(temp); });
        }

        int divby5 = MainPrefabs.Count;
        divby5 /= 5; divby5 += 1;

        DCDeckContainer.sizeDelta = new Vector2(DCDeckContainer.sizeDelta.x, UI.instance.GetViewContainerHeight(divby5));

        Prefabs = MainPrefabs;
        currentDeckSize = deckSize;
        CalculateDrawChance();

        if (ShowingPercent) 
        {
            for (int i = 0; i < Prefabs.Count; i++)
            {
                Prefabs[i].transform.GetChild(2).gameObject.SetActive(true);
                Prefabs[i].transform.GetChild(3).gameObject.SetActive(true);
            }
        }


        
    }

    public void ClickCard(string searchName)
    {
        //Debug.Log("Searching for: " + searchName);
        for(int i = 0;i<DeckCards.Count;i++)
        {
            if(searchName.Contains(DeckCards[i].cardName))
            {
                int current = DeckCards[i].currentCopies;

                if (current > 0)
                {
                    current--;
                    DeckCards[i].currentCopies = current;
                    currentDeckSize--;

                    if(AddingToHand)
                    {
                        AddToHand(i);
                    }



                    // updated prefabs
                    UpdatePrefab(Prefabs[i], current);

                }
                else
                {
                    break;
                }
            }
        }
        CalculateDrawChance();
    }

    public void RemoveFromTestHand(string searchName, int PHPos)
    {
        Debug.Log("Clicked on card :" + PHPos);



        for (int i = 0; i < DeckCards.Count; i++)
        {
            if (searchName.Contains(DeckCards[i].cardName))
            {
                int current = DeckCards[i].currentCopies;

                current++;
                DeckCards[i].currentCopies = current;
                currentDeckSize++;

                Destroy(DCHandContainer.GetChild(PHPos).gameObject);
                PrefabsHand.RemoveAt(PHPos);

                // reset listener PHPos value
                for(int k = 0;k<PrefabsHand.Count;k++)
                {
                    PrefabsHand[k].GetComponent<Button>().onClick.RemoveAllListeners();
                    string newSearchName = PrefabsHand[k].name.Substring(0, PrefabsHand[k].name.Length - 7);
                    int newPosValue = k;
                    PrefabsHand[k].GetComponent<Button>().onClick.AddListener(() => { RemoveFromTestHand(newSearchName, newPosValue); });
                }

                // updated prefabs
                UpdatePrefab(Prefabs[i], current);
            }
        }
        CalculateDrawChance();
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
        if(DeckCards != null)
        {
            // reset deckdata and prefabs in deck section
            for (int i = 0; i < DeckCards.Count; i++)
            {
                DeckCards[i].currentCopies = DeckCards[i].totalCopies;
                UpdatePrefab(Prefabs[i], DeckCards[i].currentCopies);
            }

            // reset hand section
            if (PrefabsHand.Count > 0)
                PrefabsHand.Clear();

            foreach (Transform child in DCHandContainer.transform)
                Destroy(child.gameObject);

            DCHandContainer.sizeDelta = new Vector2(0,0);
            currentDeckSize = deckSize;
            CalculateDrawChance();

            if (ShowingPercent)
            {
                if (!ProbMainMenuButtons.gameObject.activeInHierarchy)
                {
                    for (int i = 0; i < Prefabs.Count; i++)
                    {
                        Prefabs[i].transform.GetChild(2).gameObject.SetActive(true);
                        Prefabs[i].transform.GetChild(3).gameObject.SetActive(true);
                    }
                }
            }

            if(isTestHands)
            {

                //reset listeners in case of pot of desires click
                foreach (GameObject g in Prefabs)
                {
                    g.GetComponent<Button>().onClick.RemoveAllListeners();
                    string searchName = g.name.Substring(0, g.name.Length - 7);

                    // if prefab is greyed out then no new listener attached
                    g.GetComponent<Button>().onClick.AddListener(() => { ClickCard(searchName); });
                }

                DrawCalcFeatureButtons.gameObject.SetActive(true);
                // reset desires buttons
                DesiresFilterButtons.gameObject.SetActive(false);
                CalcDesiresButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pot of Desires";
                // update text
                probResults.text = "Click on Cards in Deck to ";
                if (AddingToHand)
                    probResults.text += "Add to Hand";
                else
                    probResults.text += "Remove from Deck";
            }


            
        }

        
    }

    public void ResetProb()
    {
        ResetDrawCalc();
        OneCardComboTargets = 0;
        
        PairsperCard.Clear();

        foreach(GameObject g in Prefabs)
        {
            PairsperCard.Add(0);
        }

        if (!isTestHands)
        {
            // if OCC
            if (isOneNotTwo)
            {
                toggleTCCButton.gameObject.SetActive(false);
                CalcProbButton.gameObject.SetActive(true);
                probResults.text = "Select the One Card Combo Starters you want to see";
                DCHandContainer.GetComponent<HorizontalLayoutGroup>().padding.left = 63;
                DCHandContainer.GetComponent<HorizontalLayoutGroup>().spacing = 150;
            }
            // else if TCC
            else
            {
                probResults.text = "Select First Card for Two Card Combo Pair";
                DCHandContainer.GetComponent<HorizontalLayoutGroup>().padding.left = 107;
                DCHandContainer.GetComponent<HorizontalLayoutGroup>().spacing = 252;
                toggleTCCButton.gameObject.SetActive(true);
                CalcProbButton.gameObject.SetActive(false);
            }
        }
        // else Test Hands
        else
        {
            toggleTCCButton.gameObject.SetActive(false);
            CalcProbButton.gameObject.SetActive(true);
            probResults.text = "Click on Cards in Deck to ";
            if (AddingToHand)
                probResults.text += "Add to Hand";
            else
                probResults.text += "Remove from Deck";


            DCHandContainer.GetComponent<HorizontalLayoutGroup>().padding.left = 63;
            DCHandContainer.GetComponent<HorizontalLayoutGroup>().spacing = 126;
        }
    }

    public float CalcHandWidth(int count)
    {

        //Debug.Log(count+": visible");
        float width = 0.0f;

        // include all segment widths
        width += count * (PrefabsHand[0].GetComponent<RectTransform>().sizeDelta.x * 0.75f);

        // include the spacing between segments
        width += count * DCHandContainer.GetComponent<HorizontalLayoutGroup>().spacing;


        return width-800;
        
    }

    public void AddToHand(int i)
    {
        GameObject tempPrefabs = Instantiate(Prefabs[i]);

        tempPrefabs.transform.SetParent(DCHandContainer.transform);
        tempPrefabs.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        int position = PrefabsHand.Count;

        tempPrefabs.GetComponent<Button>().onClick.AddListener(() => { RemoveFromTestHand(tempPrefabs.name,position); });



        //tempPrefabs.transform.GetChild(2).gameObject.SetActive(false);
        PrefabsHand.Add(tempPrefabs);
        PrefabsHand.Last<GameObject>().transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        DCHandContainer.sizeDelta = new Vector2(CalcHandWidth(PrefabsHand.Count), DCHandContainer.sizeDelta.y);

        float right = DCHandContainer.offsetMax.x;

        if (right > 0)
        {
            DCHandContainer.offsetMin -= new Vector2(right,0);
            DCHandContainer.offsetMax -= new Vector2(right,0);
        }
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
            //Debug.Log("Drawing: " + DeckCards[CardPos[randomCard]].cardName);
            DeckCards[CardPos[randomCard]].currentCopies--;
            currentDeckSize--;
            AddToHand(CardPos[randomCard]);
            UpdatePrefab(Prefabs[CardPos[randomCard]], DeckCards[CardPos[randomCard]].currentCopies);

            CalculateDrawChance();
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
        // reset variables
        deckSize = 0;

        if (DeckCards.Count > 0)
            DeckCards.Clear();

        if (Prefabs.Count > 0)
            Prefabs.Clear();

        if (PrefabsHand.Count > 0)
            PrefabsHand.Clear();

        foreach (Transform child in DCDeckContainer.transform)
            Destroy(child.gameObject);

        
        foreach (Transform child in DCHandContainer.transform)
            Destroy(child.gameObject);

        DrawCalcScreen.SetActive(false);

        GameObject canvas = GameObject.Find("Canvas");

        //canvas.transform.Find("CardList").gameObject.SetActive(true);
        canvas.transform.Find("CardSearch").gameObject.SetActive(true);
        //canvas.transform.Find("DeckBuild").gameObject.SetActive(true);
        canvas.transform.Find("DeckView").gameObject.SetActive(true);
        canvas.transform.Find("MainMenuButtons").gameObject.SetActive(true);
    }

    public void ReturnDrawCalc()
    {
        // reattach test hands listeners.
        for (int i = 0; i < Prefabs.Count; i++)
        {
            Prefabs[i].GetComponent<Button>().onClick.RemoveAllListeners();
            string searchName = Prefabs[i].name;

            Prefabs[i].GetComponent<Button>().onClick.AddListener(() => { ClickCard(searchName); });

        }

        isTestHands = true;
        isOneNotTwo = true;

        ProbMainMenuButtons.gameObject.SetActive(false);
        ProbFeatureButtons.gameObject.SetActive(false);
        DrawCalcFeatureButtons.gameObject.SetActive(true);
        DrawCalcMainMenuButtons.gameObject.SetActive(true);

        ResetProb();
    }

    public void CalculateDrawChance()
    {
        for (int i = 0; i < DeckCards.Count; i++)
        {
            if (currentDeckSize>0) {
            
                float chance = 0.0f;
                chance = (float)DeckCards[i].currentCopies / (float)currentDeckSize;
                chance *= 100.0f;

                chance = Mathf.Round(chance * 100f) / 100f;
    
                Prefabs[i].transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = chance.ToString() + "%";
            } else
            {
                Prefabs[i].transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text ="0%";
            }
            
        }
    }

    public double CalcFactorial(double input)
    {
        if (input <= 1)
            return 1;
        return input * CalcFactorial(input - 1);
    }

    public double CalcCombinations(int A, int B)
    {
        /*
            Combinations of A B:

            A! / B! (B-A)!
         */
        double fac1;
        double fac2;
        double fac3;
        double temp;

        fac1 = CalcFactorial(A);
        fac2 = CalcFactorial(B);
        fac3 = CalcFactorial(A-B);
        //Debug.Log("test: " + CalcFactorial(A) / (CalcFactorial(B) * CalcFactorial(A - B)));
        //Debug.Log("A: " + fac1);
        //Debug.Log("B: " + fac2);
        //Debug.Log("A-B: " + fac3);

        temp = fac2 * fac3;
        //Debug.Log("Temp: " + temp);
        double returnVal = fac1 / temp;
        //Debug.Log("F: " + returnVal);
        return returnVal;
    }

    public bool ValidateInputs(int x, int N, int n, int k)
    {


        if (x < 0 || x > N || x > n)
        {
            Debug.Log("Required Successes Invalid: "+x);
            return false;
        }
        else if (x > k)
        {
            Debug.Log("Please select card(s) for calculation"+x+" > "+k);
            return false;
        }
        else if (N < 40 || N > 60)
        {
            Debug.Log("Invalid deck size: " + N);
            return false;
        }
        else if (n < 1 || n > N)
        {
            Debug.Log("Invalid sample size: " + n);
            return false;
        }
        else if (k < 1 || k > N)
        {
            Debug.Log("Invalid potential successes: " + k);
            return false;
        }

        return true;
    }

    public void ViewProbMenu()
    {
        ResetProb();

        // reset deck data
        deckSize = 0;
        for (int i = 0; i < DeckCards.Count; i++)
        {
            DeckCards[i].currentCopies = DeckCards[i].totalCopies;
            deckSize += DeckCards[i].totalCopies;
        }
        currentDeckSize = deckSize;

        // reset hand data
        if (PrefabsHand.Count > 0)
            PrefabsHand.Clear();

        foreach (Transform child in DCHandContainer.transform)
            Destroy(child.gameObject);

        // display menu buttons
        GameObject canvas = GameObject.Find("Canvas");

        canvas.transform.Find("MainMenuButtons").gameObject.SetActive(false);
        ProbMainMenuButtons.gameObject.SetActive(true);
        ProbFeatureButtons.gameObject.SetActive(true);
        DrawCalcFeatureButtons.gameObject.SetActive(false);
        DrawCalcMainMenuButtons.gameObject.SetActive(false);

        if (ShowingPercent)
        {
            for (int i = 0; i < Prefabs.Count; i++)
            {
                Prefabs[i].transform.GetChild(2).gameObject.SetActive(false);
                Prefabs[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }

        ReadyOCCCalc();
    }   
    
    public void ReadyOCCCalc()
    {
        isOneNotTwo = true;
        isTestHands = false;
        ResetProb();


        // remove listeners from deck Prefabs
        for (int i =0;i< Prefabs.Count;i++)
        {
            Prefabs[i].GetComponent<Button>().onClick.RemoveAllListeners();
            string searchName = Prefabs[i].name;

            Prefabs[i].GetComponent<Button>().onClick.AddListener(() => { AddOCCTarget(searchName); });

        }
        
        OneCCButton.GetComponent<Image>().color = new Color(0.44f, 0.7f, 0.8f, 1.0f);
        TwoCCButton.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
    }
    
    public void ReadyTCCCalc()
    {
        isOneNotTwo = false;
        isTestHands = false;
        ResetProb();


        // remove listeners from deck Prefabs
        for (int i = 0; i < Prefabs.Count; i++)
        {
            Prefabs[i].GetComponent<Button>().onClick.RemoveAllListeners();
            string searchName = Prefabs[i].name;

            Prefabs[i].GetComponent<Button>().onClick.AddListener(() => { AddTCCTarget(searchName); });

        }
        //Debug.Log("PPC: " + PairsperCard.Count);
        OneCCButton.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        TwoCCButton.GetComponent<Image>().color = new Color(0.44f, 0.7f, 0.8f, 1.0f);

    }
   
    public void AddTCCTarget(string searchName)
    {
        bool isLeft = true;
        if (PrefabsHand.Count % 2 != 0)
            isLeft = false;

        for (int i = 0; i < DeckCards.Count; i++)
        {
            // get card data
            if (searchName.Contains(DeckCards[i].cardName)) 
            {
                if (Prefabs[i].transform.GetChild(1).gameObject.activeInHierarchy == true)
                    Debug.Log("invalid option");

                    // if click on invalid card to create a pair
                    if (Prefabs[i].transform.GetChild(1).gameObject.activeInHierarchy == true)
                    break;

                bool hasDupe = false;

                // if creating new pair
                if(isLeft)
                {
                    for (int f = 0; f < PrefabsHand.Count; f += 2)
                    {
                        if (PrefabsHand[f].name.Equals(PrefabsHand[f + 1].name)
                            &&PrefabsHand[f].name.Contains(Prefabs[i].name)) {
                            hasDupe = true;
                            break;
                        }
                            
                    }

                    if(hasDupe)
                    {
                        Prefabs[i].transform.GetChild(1).gameObject.SetActive(true);
                    }


                    // find all current pairs including this card
                    List<int> FoundPos = new();

                    // for every current pair, find pairs with this card
                    for (int j = 0; j < PrefabsHand.Count; j++)
                    {
                        // if pair found
                        if (PrefabsHand[j].name.Contains(searchName))
                        {

                            // is L or R in pair
                            bool pairLeft = true;
                            if (j % 2 != 0)
                                pairLeft = false;

                            string otherName;
                            if (pairLeft)
                                otherName = PrefabsHand[j + 1].name;
                            else
                                otherName = PrefabsHand[j - 1].name;

                            //Debug.Log("Other card in pair: " + otherName);

                            // find 'other' in prefabs
                            for (int k = 0; k < Prefabs.Count; k++)
                            {

                                // if other found
                                if (otherName.Contains(Prefabs[k].name))
                                {

                                    Prefabs[k].transform.GetChild(1).gameObject.SetActive(true);
                                    FoundPos.Add(k);
                                }
                            }
                        }

                    }
                    //Debug.Log("Pairs Left: " + (DeckCards.Count-FoundPos.Count));
                    // if able to make a new pair
                    if (FoundPos.Count-1 < DeckCards.Count)
                    {
                        PairsperCard[i]++;

                        GameObject tempPrefabs = Instantiate(Prefabs[i]);
                        GameObject temp = new();
                        temp.transform.SetParent(DCHandContainer.transform);

                        tempPrefabs.transform.GetChild(1).gameObject.SetActive(false);
                        tempPrefabs.transform.SetParent(temp.transform);
                        temp.AddComponent<RectTransform>();
                        temp.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
                        tempPrefabs.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,0,0);
                        
                        tempPrefabs.GetComponent<Button>().onClick.RemoveAllListeners();

                        int PHid = PrefabsHand.Count;
                        tempPrefabs.GetComponent<Button>().onClick.AddListener(() => { RemoveTCCTarget(searchName, PHid); });
                        PrefabsHand.Add(tempPrefabs);

                        PrefabsHand.Last<GameObject>().transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                        probResults.text = "Select Second Card for Two Card Combo Pair";

                        /*float width = 0.0f;
                        
                        // include all segment widths
                        width += DCHandContainer.transform.childCount * 252.0f * 0.75f;
                        Debug.Log("width pre spacing: " + width);
                        // include the spacing between segments
                        width += PrefabsHand.Count * DCHandContainer.GetComponent<HorizontalLayoutGroup>().spacing;
                        Debug.Log("width post spacing: " + width);
                        */
                        float width = 64+ (DCHandContainer.transform.childCount * 252.0f);
                        
                        if(width > 900)
                        {
                            DCHandContainer.sizeDelta = new Vector2(DCHandContainer.sizeDelta.x+ 252, DCHandContainer.sizeDelta.y);
                            float right = DCHandContainer.offsetMax.x;

                            if (right > 0)
                            {
                                DCHandContainer.offsetMin -= new Vector2(right, 0);
                                DCHandContainer.offsetMax -= new Vector2(right, 0);
                            }
                        }
                        
                        


                        // if more pairs allowed, reset prefabs[i] greyness incase of (L+R) pairs being the same card
                        if (FoundPos.Count <= DeckCards.Count && !hasDupe)
                            Prefabs[i].transform.GetChild(1).gameObject.SetActive(false);
                        else
                            Prefabs[i].transform.GetChild(1).gameObject.SetActive(true);

                        break;
                    }
                    else
                    {
                        Prefabs[i].transform.GetChild(1).gameObject.SetActive(true);
                        break;
                    }

                }

                // finishing a pair
                else
                {

                    // if can use this card to finish pair
                    if (Prefabs[i].transform.GetChild(1).gameObject.activeInHierarchy != true)
                    {
                        PairsperCard[i]++;
                        GameObject tempPrefabs = Instantiate(Prefabs[i]);
                        tempPrefabs.transform.SetParent(DCHandContainer.transform.GetChild(DCHandContainer.transform.childCount - 1));


                        DCHandContainer.transform.GetChild(DCHandContainer.transform.childCount - 1).GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
                        tempPrefabs.GetComponent<RectTransform>().anchoredPosition = new Vector3(63, 0, 0);


                        tempPrefabs.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                        tempPrefabs.GetComponent<Button>().onClick.RemoveAllListeners();

                        int PHid = PrefabsHand.Count;
                        tempPrefabs.GetComponent<Button>().onClick.AddListener(() => { RemoveTCCTarget(searchName, PHid); });

                        PrefabsHand.Add(tempPrefabs);
                        probResults.text = "Select First Card for Two Card Combo Pair";

                        CalcTCC(false);

                    }
                    foreach (GameObject Pre in Prefabs)
                        Pre.transform.GetChild(1).gameObject.SetActive(false);

                    

                }


                // block all completed card

                for (int c = 0; c < Prefabs.Count;c++)
                {
                    if (PairsperCard[c] > DeckCards.Count)
                    {
                        Prefabs[c].transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
                
                

                break;

                
                
            }
        }
    }

    public void RemoveTCCTarget(string searchName, int handID)
    {
        bool thisLeft = true;
        
        if (handID % 2 != 0)
            thisLeft = false;

        // if click right, destroy right and left
        if(!thisLeft)
        {
            string Right = searchName;
            string Left = PrefabsHand[handID-1].name;

            int LDeckPos = 9999;
            int RDeckPos = 9999;

            bool isDupe = Left.Contains(Right);
            // using hand id, find cards in deck that = PH[handID].name

            // for each card in pair, find positions in deck
            for (int i = 0; i < DeckCards.Count; i++)
            {

                // if found left
                if(Left.Contains(DeckCards[i].cardName))
                {
                    
                    LDeckPos = i;
                }
                
                // if found right
                if(Right.Contains(DeckCards[i].cardName))
                {
                   
                    RDeckPos = i;
                }

                // if both found, break;
                if(LDeckPos != 9999 && RDeckPos != 9999)
                {
                    break;
                }
            }

            // destroy parent of pair in deck
            foreach (Transform child in DCHandContainer.transform)
            {
                string LTarget = child.GetChild(0).name;
                string RTarget = child.GetChild(1).name;
                
                
                // if find the game object with children matching targets destroy
                if (LTarget.Equals(Left) && RTarget.Contains(Right)) {
                    
                    // Destroy Object in Hand
                    Destroy(child.gameObject);

                    // Remove from PH
                    PrefabsHand.RemoveAt(handID);
                    PrefabsHand.RemoveAt(handID-1);

                    // remove from PPC
                    if (!isDupe)
                    {
                        PairsperCard[LDeckPos]--;
                        PairsperCard[RDeckPos]--;
                    } 
                    else
                    {
                        PairsperCard[LDeckPos] -= 2;
                    }

                    // reset listeners
                    for(int m=0;m<PrefabsHand.Count;m++)
                    {
                        PrefabsHand[m].GetComponent<Button>().onClick.RemoveAllListeners();
                        string newTarget = PrefabsHand[m].name.Substring(0, PrefabsHand[m].name.Length - 7);
                        int listenM = m;

                        PrefabsHand[m].GetComponent<Button>().onClick.AddListener(() => { RemoveTCCTarget(newTarget, listenM); });
                    }

                    // reset container size
                    float width = 64 + (DCHandContainer.transform.childCount * 252.0f);

                    if (width > 900)
                    {
                        DCHandContainer.sizeDelta = new Vector2(DCHandContainer.sizeDelta.x - 252, DCHandContainer.sizeDelta.y);
                        float right = DCHandContainer.offsetMax.x;

                        if (right > 0)
                        {
                            DCHandContainer.offsetMin -= new Vector2(right, 0);
                            DCHandContainer.offsetMax -= new Vector2(right, 0);
                        }
                    }

                    break;
                }
                    
            }

        }

        // if click left, delete right if exists
        else if (thisLeft)
        {
            probResults.text = "Select First Card for Two Card Combo Pair";

            bool rightExists = false;

            if (handID+1 < PrefabsHand.Count)
                rightExists = true;

            string Left = searchName;
            string Right = "";
            bool isDupe = false;

            if (rightExists)
            {
                Right = PrefabsHand[handID + 1].name;
                isDupe = Left.Contains(Right);

            }

            int LDeckPos = 9999;
            int RDeckPos = 9999;

            // using hand id, find cards in deck that = PH[handID].name

            // for each card in pair, find positions in deck
            for (int i = 0; i < DeckCards.Count; i++)
            {

                // if found left
                if (Left.Contains(DeckCards[i].cardName))
                {
                    LDeckPos = i;
                }

                // if found right and it exists
                if(rightExists)
                {
                    if (Right.Contains(DeckCards[i].cardName))
                    {
                        RDeckPos = i;
                    }
                }
                

                // if both found or if only left exists and found, break;
                if ((LDeckPos != 9999 && RDeckPos != 9999) || (LDeckPos != 9999 && !rightExists))
                {
                    break;
                }
            }

            // destroy parent of pair in deck
            for (int k = 0; k < DCHandContainer.childCount; k++)
                
            {

                Transform child = DCHandContainer.GetChild(k);
                bool lastChild = false;

                if (k == DCHandContainer.childCount - 1)
                    lastChild = true;


                string LTarget = child.GetChild(0).name;
                string RTarget = "";
                
                if(!lastChild)
                    RTarget = child.GetChild(1).name;


                // if find the game object with children matching targets destroy

              


                if ((LTarget.Contains(Left) && RTarget.Equals(Right)) || lastChild)
                {

                    // Destroy Object in Hand
                    Destroy(child.gameObject);

                    // Remove from PH

                    if(rightExists)
                    {
                        PrefabsHand.RemoveAt(handID + 1);
                        PrefabsHand.RemoveAt(handID);

                        if (!isDupe)
                        {
                            PairsperCard[LDeckPos]--;
                            PairsperCard[RDeckPos]--;
                        }
                        else
                        {
                            PairsperCard[LDeckPos] -= 2;
                        }
                    }
                    else
                    {
                        PrefabsHand.RemoveAt(handID);
                        PairsperCard[LDeckPos]--;
                    }

                    for (int m = 0; m < PrefabsHand.Count; m++)
                    {
                        PrefabsHand[m].GetComponent<Button>().onClick.RemoveAllListeners();
                        string newTarget = PrefabsHand[m].name.Substring(0, PrefabsHand[m].name.Length - 7);
                        int listenM = m;

                        PrefabsHand[m].GetComponent<Button>().onClick.AddListener(() => { RemoveTCCTarget(newTarget, listenM); });
                    }

                    // reset container size
                    float width = 64 + (DCHandContainer.transform.childCount * 252.0f);

                    if (width > 900)
                    {
                        DCHandContainer.sizeDelta = new Vector2(DCHandContainer.sizeDelta.x - 252, DCHandContainer.sizeDelta.y);
                        float right = DCHandContainer.offsetMax.x;

                        if (right > 0)
                        {
                            DCHandContainer.offsetMin -= new Vector2(right, 0);
                            DCHandContainer.offsetMax -= new Vector2(right, 0);
                        }
                    }


                    break;
                }

            }

        }

        if(PrefabsHand.Count % 2 == 0)
            probResults.text = "Select First Card for Two Card Combo Pair";
        else
            probResults.text = "Select Second Card for Two Card Combo Pair";


    }

    public void AddOCCTarget(string searchName)
    {
        for (int i = 0; i < DeckCards.Count; i++)
        {
            if (searchName.Contains(DeckCards[i].cardName))
            {
                int current = DeckCards[i].currentCopies;

                if (current > 0)
                {
                    OneCardComboTargets += current;


                    current -= current;
                    DeckCards[i].currentCopies = current;
                    currentDeckSize -= current;

                    GameObject tempPrefabs = Instantiate(Prefabs[i]);

                    tempPrefabs.transform.SetParent(DCHandContainer.transform);
                    Prefabs[i].transform.GetChild(1).gameObject.SetActive(true);

                    tempPrefabs.GetComponent<Button>().onClick.AddListener(() => { RemoveOCCTarget(searchName); });

                    PrefabsHand.Add(tempPrefabs);
                    PrefabsHand.Last<GameObject>().transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    DCHandContainer.sizeDelta = new Vector2(CalcHandWidth(PrefabsHand.Count), DCHandContainer.sizeDelta.y);

                    float right = DCHandContainer.offsetMax.x;

                    if (right > 0)
                    {
                        DCHandContainer.offsetMin -= new Vector2(right, 0);
                        DCHandContainer.offsetMax -= new Vector2(right, 0);
                    }

                }
                else
                {
                    break;
                }
            }
        }
    }

    public void RemoveOCCTarget(string searchName)
    {
        for (int i = 0; i < DeckCards.Count; i++)
        {
            if (searchName.Contains(DeckCards[i].cardName))
            {
                // reset current copies
                DeckCards[i].currentCopies = DeckCards[i].totalCopies;
                OneCardComboTargets -= DeckCards[i].currentCopies;

                // turn off grey
                Prefabs[i].transform.GetChild(1).gameObject.SetActive(false);

                // remove from hand
                for (int j = 0;j<PrefabsHand.Count;j++)
                {
                    if(PrefabsHand[j].name.Contains(searchName))
                    {
                        PrefabsHand.RemoveAt(j);
                    }
                }

                foreach (Transform child in DCHandContainer.transform)
                {
                    if (child.name.Contains(searchName))
                        Destroy(child.gameObject);
                }

                if (PrefabsHand.Count > 0)
                {
                    DCHandContainer.sizeDelta = new Vector2(CalcHandWidth(PrefabsHand.Count), DCHandContainer.sizeDelta.y);

                    float right = DCHandContainer.offsetMax.x;

                    if (right > 0)
                    {
                        DCHandContainer.offsetMin -= new Vector2(right, 0);
                        DCHandContainer.offsetMax -= new Vector2(right, 0);
                    }
                }
            }
        }
    }
    
    public void AttemptCalc()
    {
        if (isOneNotTwo)
            CalcOCC();
        else
            CalcTCC(true);
    }

    public double HypergeometricFormulaCalc(int x, int N, int n, int k)
    {
        /*
            h(x;N,n,k) = [kCx][N-kCn-x]/[NCn]
            -C- is unique combinations given both variables
            
            x = exact successes wanted
            N = Total size
            n = Sample size
            k = Potential Successes
        */

        
        int A = N - k;
        int B = n - x;
        double comA = CalcCombinations(k, x);
        double comB = CalcCombinations(A, B);
        double comC = CalcCombinations(N, n);
        double temp = comA * comB;
        double answer = temp / comC;

        return answer;
    }

    public void CalcOCC()
    {
        int x = OCCSuccesses;
        int N = deckSize;
        int n = 5;
        int k = OneCardComboTargets;
        List<double> probabilities = new();
        double exactOne;
        double moreThenOne;
        double atLeastOne;

        if (ValidateInputs(x, N, n, k))
        {

            for (int i = 1; i < 6; i++)
            {
                probabilities.Add(HypergeometricFormulaCalc(i, N, n, k));

            }
            //ExactOne = HypergeometricFormulaCalc(x, N, n, k);

            // exactly one
            exactOne = probabilities[0];
            // more than one
            moreThenOne = probabilities[1] + probabilities[2] + probabilities[3] + probabilities[4];
            // one or more
            atLeastOne = exactOne + moreThenOne;

            // display results


            //Vector3 tempVector = DCHandContainer.parent.GetComponent<RectTransform>().anchoredPosition;
            //DCHandContainer.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(tempVector.x, -810, tempVector.z);


            probResults.text = "Exactly 1: " + Mathf.Clamp((float)exactOne * 100.0f, 0, 100.0f).ToString("n3") + "%     "
                + "More than 1: " + Mathf.Clamp((float)moreThenOne * 100.0f, 0, 100.0f).ToString("n3") + "%\n"
                + "At Least 1: " + Mathf.Clamp((float)atLeastOne * 100.0f, 0, 100.0f).ToString("n3") + "%";

        }
    }

    public void CalcTCC(bool updateResults)
    {
        List<double> probabilities = new();
        double atLeastOne = 0;

        // for every potential pair
        for (int i = 0;i<PrefabsHand.Count;i+=2)
        {
            int A = 9999;
            int B = 9999;
            
            double notA;
            double notB;
            double notEither;
            double totalHands;

            double probA;
            double probB;
            double probEither;
            long handsAB;
            double probAB;
            
            bool isDupe = false;

            // if pair exists
            if (i + 1 < PrefabsHand.Count)
            {

                // get pair data
                for (int k = 0; k < DeckCards.Count; k++)
                {
                    string testName = DeckCards[k].cardName;
                    bool isA = false;
                    bool isB = false;

                    // found A
                    if (PrefabsHand[i].name.Contains(testName))
                    {
                        A = DeckCards[k].totalCopies;
                        isA = true;
                    }

                    // found B
                    if (PrefabsHand[i+1].name.Contains(testName))
                    {
                        B = DeckCards[k].totalCopies;
                        isB = true;
                    }

                    // if dupes
                    if (isA && isB)
                    {
                        isDupe = true;
                        break;
                    }

                    // if found card data for both cards
                    if (A != 9999 && B != 9999)
                        break;
                }

                // calc
                notA = CalcCombinations(deckSize - A, 5);
                notB = CalcCombinations(deckSize - B, 5);
                notEither = CalcCombinations(deckSize - B - A, 5);
                totalHands = CalcCombinations(deckSize, 5);

                
                probA = notA / totalHands;
                probB = notB / totalHands;
                probEither = notEither / totalHands;

                handsAB = (long)(totalHands - (notA + notB- notEither));
                probAB = 1 - probA - probB + probEither;

                // if duplicate cards in pair, adjust results
                if(isDupe)
                {
                    handsAB /= A;
                    probAB /= A;
                }

                probabilities.Add(probAB);


                string clampProb = Mathf.Clamp((float)probAB * 100.0f, 0, 100.0f).ToString("n2");
                
                // display results

                Transform rightCard = DCHandContainer.transform.GetChild(i / 2).GetChild(1);
                rightCard.GetChild(2).gameObject.SetActive(true);
                rightCard.GetChild(3).gameObject.SetActive(true);

                // which results displaying
                if (ShowingPercentTCC)
                    rightCard.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = clampProb + "%";
                else
                    rightCard.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = handsAB.ToString();

            }
            else
            {

                // if no pairs exist, update text and return
                if(i == 0)
                {
                    // must be Card A exists, no pair exists
                    probResults.text = "Select Second Card for Two Card Combo Pair";
                    return;
                }
                break;
            }
                

        }

        // calc chance of opening at least 1 pair







        foreach (double val in probabilities)
            atLeastOne += val;

        if(updateResults)
        {
            if(ShowingPercentTCC)
                probResults.text = "On each pair, showing % chance to draw";
            else
                probResults.text = "On each pair, showing total hands they appear in";
        }     
    }

    public void ReadyDesires()
    {
        if (currentDeckSize >= 12)
        {
            if (DesiresFilterButtons.gameObject.activeInHierarchy
                || CalcDesiresButton.GetComponentInChildren<TextMeshProUGUI>().text.Contains("Cancel"))
            {
                CalcDesiresButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pot of Desires";
                DesiresFilterButtons.gameObject.SetActive(false);
                DrawCalcFeatureButtons.gameObject.SetActive(true);

                probResults.text = "Click on Cards in Deck to ";
                if (AddingToHand)
                    probResults.text += "Add to Hand";
                else
                    probResults.text += "Remove from Deck";

                foreach (GameObject g in Prefabs)
                {
                    g.GetComponent<Button>().onClick.RemoveAllListeners();
                    string searchName = g.name.Substring(0, g.name.Length - 7);

                    // if prefab is greyed out then no new listener attached
                    g.GetComponent<Button>().onClick.AddListener(() => { ClickCard(searchName); });
                }
            }
            
            else
            {
                // remove all listeners
                foreach (GameObject g in Prefabs)
                {
                    g.GetComponent<Button>().onClick.RemoveAllListeners();
                    string searchName = g.name;

                    // if prefab is greyed out then no new listener attached

                    if (g.transform.GetChild(1).gameObject.activeInHierarchy == false)
                        g.GetComponent<Button>().onClick.AddListener(() => { CalcDesires(searchName); });
                }
                probResults.text = "Click on one card in deck to view the likeliness of banishing/drawing card using pot of desires";
                CalcDesiresButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            }
        }
        else
            probResults.text = "Need at least 12 cards in deck, 10 to banish, 2 to draw";
    }

    public void CalcDesires(string targetName)
    {
        banish0 = 0;
        banish1 = 0;
        banish2 = 0;
        banish3 = 0;

        drawBan0 = 0;
        drawBan1 = 0;
        drawBan2 = 0;
        
        chance0 = 0;
        chance1 = 0;
        chance2 = 0;

        // for now only calculating the interactions of 1 card with desires
        
            for(int i = 0;i<Prefabs.Count;i++)
            {
                // if found card data
                if(targetName.Contains(DeckCards[i].cardName))
                {
                    int currentCopies = DeckCards[i].currentCopies;

                    /*
                        h(x;N,n,k) = [kCx][N-kCn-x]/[NCn]
                        -C- is unique combinations given both variables
            
                        x = exact successes wanted
                        N = Total size
                        n = Sample size
                        k = Potential Successes
                    */

                    // calc banish %
                    switch (currentCopies)
                    {
                        case 1:
                            // % chance banish 0
                            banish0 = HypergeometricFormulaCalc(0, currentDeckSize, 10, currentCopies);
                            // % chance banish 1
                            banish1 = HypergeometricFormulaCalc(1, currentDeckSize, 10, currentCopies);
                            break;

                        case 2:
                            // % chance banish 0
                            banish0 = HypergeometricFormulaCalc(0, currentDeckSize, 10, currentCopies);
                            // % chance banish 1
                            banish1 = HypergeometricFormulaCalc(1, currentDeckSize, 10, currentCopies);
                            // % chance banish 2
                            banish2 = HypergeometricFormulaCalc(2, currentDeckSize, 10, currentCopies);
                            break;

                        case 3:
                            // % chance banish 0
                            banish0 = HypergeometricFormulaCalc(0, currentDeckSize, 10, currentCopies);
                            // % chance banish 1
                            banish1 = HypergeometricFormulaCalc(1, currentDeckSize, 10, currentCopies);
                            // % chance banish 2
                            banish2 = HypergeometricFormulaCalc(2, currentDeckSize, 10, currentCopies);
                            // % chance banish 3
                            banish3 = HypergeometricFormulaCalc(3, currentDeckSize, 10, currentCopies);
                            break;
                    }

                    // calc draw %
                    switch(currentCopies)
                    {
                        case 1:
                            // banish 0 cards, odds of drawing 1
                            drawBan0 = HypergeometricFormulaCalc(1, currentDeckSize-10, 2, currentCopies);
                            drawBan1 = 0;
                            break;

                        case 2:
                            // banish 0 cards, odds of drawing at least 1
                            drawBan0 = HypergeometricFormulaCalc(1, currentDeckSize - 10, 2, currentCopies)
                                        + HypergeometricFormulaCalc(2, currentDeckSize - 10, 2, currentCopies);
    
                            // banish 1 card, odds of drawing 1
                            drawBan1 = HypergeometricFormulaCalc(1, currentDeckSize - 10, 2, currentCopies-1);

                            drawBan2 = 0;
                            break;

                        case 3:
                            // banish 0 cards, odds of drawing at least 1
                            drawBan0 = HypergeometricFormulaCalc(1, currentDeckSize - 10, 2, currentCopies)
                                        + HypergeometricFormulaCalc(2, currentDeckSize - 10, 2, currentCopies) 
                                        + HypergeometricFormulaCalc(3, currentDeckSize - 10, 2, currentCopies);
                            
                            // banish 1 card, odds of drawing at least 1
                            drawBan1 = HypergeometricFormulaCalc(1, currentDeckSize - 10, 2, currentCopies - 1)
                                        + HypergeometricFormulaCalc(2, currentDeckSize - 10, 2, currentCopies-1)
                                        + HypergeometricFormulaCalc(3, currentDeckSize - 10, 2, currentCopies-1);

                            // banish 2 cards, odds of drawing 1
                            drawBan2 = HypergeometricFormulaCalc(1, currentDeckSize - 10, 2, currentCopies-2);
                            break;
                    }


                chance0 = Mathf.Clamp((float)(banish0 * drawBan0) * 100, 0, 100.0f);
                chance1 = Mathf.Clamp((float)(banish1 * drawBan1) * 100, 0, 100.0f);
                chance2 = Mathf.Clamp((float)(banish2 * drawBan2) * 100, 0, 100.0f);

                banish0 = Mathf.Clamp((float)banish0 * 100, 0, 100.0f);
                banish1 = Mathf.Clamp((float)banish1 * 100, 0, 100.0f);
                banish2 = Mathf.Clamp((float)banish2 * 100, 0, 100.0f);
                banish3 = Mathf.Clamp((float)banish3 * 100, 0, 100.0f);

                drawBan0 = Mathf.Clamp((float)drawBan0 * 100, 0, 100.0f);
                drawBan1 = Mathf.Clamp((float)drawBan1 * 100, 0, 100.0f);
                drawBan2 = Mathf.Clamp((float)drawBan2 * 100, 0, 100.0f);
                
                
                

                currentDesireTargetCopies = currentCopies;
                // reset test hands listeners
                // remove all listeners

                foreach (GameObject g in Prefabs)
                    {
                        g.GetComponent<Button>().onClick.RemoveAllListeners();
                        string searchName = g.name.Substring(0, g.name.Length - 7);

                        // if prefab is greyed out then no new listener attached
                        g.GetComponent<Button>().onClick.AddListener(() => { ClickCard(searchName); });
                    }

                    // hide old menu buttons
                    DrawCalcFeatureButtons.gameObject.SetActive(false);
                    // set active filter buttons
                    DesiresFilterButtons.gameObject.SetActive(true);

                    foreach (Transform child in DesiresFilterButtons.transform)
                    {
                        child.GetComponent<Image>().color = new Color(0, 0, 0, 1.0f);
                        child.gameObject.SetActive(false);
                    }
                    for(int m = 0; m<currentCopies+1;m++)
                    {
                        DesiresFilterButtons.transform.GetChild(m).gameObject.SetActive(true);
                    }

                    CalcDesiresButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close Desires";
                    probResults.text = "Click on Banish Buttons to view percentages";
                    break;
                }
            }
         
        
        
    }

    public void FilterDesires(int index)
    {
        foreach (Transform child in DesiresFilterButtons.transform)
            child.GetComponent<Image>().color = new Color(0, 0, 0, 1.0f);

        DesiresFilterButtons.transform.GetChild(index).GetComponent<Image>().color = new Color(0.44f, 0.7f, 0.8f, 1.0f);

        double atLeast1 = 0;



        // display results
        switch (index)
        {

            case 0:
                probResults.text = "Banish Exactly 0: " + banish0.ToString("n3") + "%   Drawing At Least 1: " + drawBan0.ToString("n3")
                    + "%\nBanish 0, Draw: "+chance0.ToString("n3")+"%";
                break;

            case 1:
                atLeast1 = CalcAtLeast1(1);
                probResults.text = "Banish Exactly 1: " + banish1.ToString("n3") + "%   Banish At least 1: " + atLeast1.ToString("n3")
                    + "%\nDrawing At Least 1: " + drawBan1.ToString("n3") + "%   Banish 1, still Draw: " + chance1.ToString("n3") + "%";
                break;

            case 2:
                atLeast1 = CalcAtLeast1(2);
                probResults.text = "Banish Exactly 2: " + banish2.ToString("n3") + "%   Banish At least 2: " + atLeast1.ToString("n3")
                    + "%\nDrawing At Least 1: " + drawBan2.ToString("n3")+ "%   Banish 2, still Draw: "+chance2.ToString("n3")+"%";
                break;

            case 3:
                probResults.text = "Banish All 3: " + banish3.ToString("n3")+"%";
                break;
        }
    }

    public double CalcAtLeast1(int index)
    {
        double returnVal = 0;

        // user wants to see banish odds of index val



        switch(index)
        {
            // odds of banishing at least 1 card with CDTC copies in deck
            case 1:

                // if 2 copies in deck, at least 1 = 1+2
                if(currentDesireTargetCopies == 2)
                    returnVal = banish1 + banish2;

                // if 3 copies in deck, at least 1 = 1+2+3
                else if (currentDesireTargetCopies == 3)
                    returnVal = banish1 + banish2+banish3;
                else
                    returnVal = banish1;
            break;

            // odds of banishing at least 2 cards with CDTC copies in deck
            case 2:
                // if 2 copies in deck, at least 2 = 2+3
                if (currentDesireTargetCopies == 3)
                    returnVal = banish2 + banish3;
                else
                    returnVal = banish2;
                break;

        }

        


        return returnVal;
    }
}
