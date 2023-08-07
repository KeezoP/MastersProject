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
    public RectTransform OCCButtons;
    public List<GameObject> Prefabs;
    public List<GameObject> PrefabsHand;
    public List<DCCard> DeckCards;
    public int deckSize;
    public int currentDeckSize;

    public int OneCardComboTargets;
    public int OCCSuccesses;

    public void Awake()
    {
        AddingToHand = true;
        Prefabs = new();
        DeckCards = new();
        OCCSuccesses = 1;
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
            currentDeckSize--;
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

        canvas.transform.Find("CardList").gameObject.SetActive(true);
        canvas.transform.Find("CardSearch").gameObject.SetActive(true);
        canvas.transform.Find("DeckBuild").gameObject.SetActive(true);
        canvas.transform.Find("DeckView").gameObject.SetActive(true);
        canvas.transform.Find("MainMenuButtons").gameObject.SetActive(true);
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

                Prefabs[i].transform.GetChild(2).gameObject.SetActive(true);
                Prefabs[i].transform.GetChild(3).gameObject.SetActive(true);
                Prefabs[i].transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = chance.ToString() + "%";
            } else
            {
                Prefabs[i].transform.GetChild(2).gameObject.SetActive(true);
                Prefabs[i].transform.GetChild(3).gameObject.SetActive(true);
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
        Debug.Log("A: " + fac1);
        Debug.Log("B: " + fac2);
        Debug.Log("A-B: " + fac3);

        temp = fac2 * fac3;
        Debug.Log("Temp: " + temp);
        double returnVal = fac1 / temp;
        Debug.Log("F: " + returnVal);
        return returnVal;
    }

    public bool ValidateInputs(int x, int N, int n, int k)
    {


        if (x < 0 || x > N)
        {
            Debug.Log("Required Successes Invalid: "+x);
            return false;
        }
        else if (x > k)
        {
            Debug.Log("Required successes larger potential successes: "+x+" > "+k);
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
    
    public void ReadyOCCCalc()
    {
        // remove listeners from deck Prefabs

        for(int i =0;i< Prefabs.Count;i++)
        {
            Prefabs[i].GetComponent<Button>().onClick.RemoveAllListeners();
            string searchName = Prefabs[i].name;

            Prefabs[i].GetComponent<Button>().onClick.AddListener(() => { AddOCCTarget(searchName); });

        }
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
        OCCButtons.gameObject.SetActive(true);
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
                PrefabsHand.Remove(Prefabs[i]);
                
                
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
    
    public void AttemptCalc()
    {
        int x = OCCSuccesses;
        int N = deckSize;
        int n = 5;
        int k = OneCardComboTargets;
        double probability;

        if (ValidateInputs(x, N, n, k))
        {
            probability = HypergeometricFormulaCalc(x, N, n, k);
            Debug.Log("Probability: " + probability);
            //Debug.Log("P%: " + Mathf.Round(probability * 100f) / 100f);
        }
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

        //Debug.Log("x: " + x);
        //Debug.Log("N: " + N);
        //Debug.Log("n: " + n);
        //Debug.Log("k: " + k);
        int A = N - k;
        int B = n - x;

        double comA = CalcCombinations(k, x);
        double comB = CalcCombinations(A, B);
        double comC = CalcCombinations(N, n);

        double temp = comA * comB;
        Debug.Log(comA + " * " + comB + " / " + comC);
        Debug.Log(temp + " / " + comC);
        
        double answer = temp / comC;

        return answer;
    }
}
