using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchManager : MonoBehaviour
{
    public List<Baloon> currentMatches = new List<Baloon>();
    public List<Baloon> currentBonus = new List<Baloon>();
    bool isBonus =  false;

    public void ProcessMatches()
    {
        StartCoroutine(ProcessMatchesCo());
    }

    private bool IsBonus(Baloon element)
    {
        return element.BonusType != BonusTypeEnum.None;
    }

    private void AddBonusesToList(List<Baloon> elements)
    {
        foreach (var element in elements)
        {
            if (IsBonus(element))
            {
                if (!currentBonus.Contains(element))
                {
                    currentBonus.Add(element);
                }
            }
        }
    }


    private void AddToCurrentMatches(List<Baloon> elements)
    {
        foreach (var element in elements)
        {
            if (!currentMatches.Contains(element))
            {
                currentMatches.Add(element);
            }
        }
    }

    private void MarkAsMatched(List<Baloon> elements)
    {
        foreach (var element in elements)
        {
            element.ChangeMatchState(true);
        }
    }

    private IEnumerator ProcessMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                Baloon currentElement = GameManager.Instance.Grid[i, j];
                if (currentElement != null)
                {
                    if (i > 0 && i < Settings.Instance.Columns - 1)
                    {
                        Baloon leftElement = GameManager.Instance.Grid[i - 1, j];
                        Baloon rightElement = GameManager.Instance.Grid[i + 1, j];
                        if (leftElement != null && rightElement != null)
                        {
                            if (leftElement.ColorType == currentElement.ColorType && rightElement.ColorType == currentElement.ColorType)
                            {
                                List < Baloon > lineElements = new List<Baloon> { leftElement, currentElement, rightElement };
                                AddBonusesToList(lineElements);
                                AddToCurrentMatches(lineElements);
                                MarkAsMatched(lineElements);
                            }
                        }
                    }

                    if (j > 0 && j < Settings.Instance.Rows - 1)
                    {
                        Baloon upElement = GameManager.Instance.Grid[i, j + 1];
                        Baloon downElement = GameManager.Instance.Grid[i, j - 1];
                        if (upElement != null && downElement != null)
                        {
                            if (upElement.ColorType == currentElement.ColorType && downElement.ColorType == currentElement.ColorType)
                            {
                                List<Baloon> lineElements = new List<Baloon> { upElement, currentElement, downElement };
                                AddBonusesToList(lineElements);
                                AddToCurrentMatches(lineElements);
                                MarkAsMatched(lineElements);
                            }
                        }
                    }

                }
            }
        }

    }
    public void CheckBonus()
    {
        if (GameManager.Instance.currentElement != null)
        {
            if (GameManager.Instance.currentElement.IsMatched)
            {
                GameManager.Instance.currentElement.ChangeMatchState(false);
                GameManager.Instance.currentElement.SetBonusType(BonusTypeEnum.ChangeCravityBonus);
            }
            else if (GameManager.Instance.currentElement.OtherBaloon != null)
            {
                Baloon otherDot = GameManager.Instance.currentElement.OtherBaloon;
                if (otherDot.IsMatched)
                {
                    otherDot.ChangeMatchState(false);
                    otherDot.SetBonusType(BonusTypeEnum.ChangeCravityBonus);
                }
            }

        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                if (GameManager.Instance.Grid[i, j] != null)
                {
                    if (GameManager.Instance.Grid[i, j].IsMatched)
                    {
                        if (currentMatches.Count >= 4)
                        {
                            CheckBonus();
                        }
                        Destroy(GameManager.Instance.Grid[i, j].gameObject);
                        GameManager.Instance.Grid[i, j] = null;
                    }
                }
            }
        }
        foreach (var item in currentBonus)
        {
            if (item.BonusType == BonusTypeEnum.ChangeCravityBonus)
            {
                isBonus = !isBonus;
            }
        }
        currentBonus.Clear();
        currentMatches.Clear();
        if (!isBonus)
        {
            StartCoroutine(DecreaseRowUpsideDownCo());
        }
        else
        {
            StartCoroutine(DecreaseRowDownUpCo());
        }
    }

    private IEnumerator DecreaseRowUpsideDownCo()
    {
        int nullCount = 0;
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                if (GameManager.Instance.Grid[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    GameManager.Instance.Grid[i, j].ChangeColumAndRowTo(0, -nullCount);
                    GameManager.Instance.Grid[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillGridCo());
    }

    private IEnumerator DecreaseRowDownUpCo()
    {
        int nullCount = 0;
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = Settings.Instance.Rows - 1; j >= 0; j--)
            {
                if (GameManager.Instance.Grid[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    GameManager.Instance.Grid[i, j].ChangeColumAndRowTo(0,nullCount);
                    GameManager.Instance.Grid[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillGridCo());
    }
    private bool MatchesOnBoard()
    {
        
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                if (GameManager.Instance.Grid[i, j] != null)
                {
                    if (GameManager.Instance.Grid[i, j].IsMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillGridCo()
    {
        GameManager.Instance.SpawnManager.RefillGrid();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();

        }
        currentMatches.Clear();
        GameManager.Instance.currentElement = null;
        yield return new WaitForSeconds(.5f);
        GameManager.Instance.currentState = GameState.WaitingForAction;

    }
}
