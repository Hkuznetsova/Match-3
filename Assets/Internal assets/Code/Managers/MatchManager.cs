using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchManager : MonoBehaviour
{
    public List<GameElement> currentMatches = new List<GameElement>();
    public List<GameElement> currentBonus = new List<GameElement>();
    bool isBonus =  false;

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private bool IsBonus(GameElement element)
    {
        return element.BonusType != BonusTypeEnum.None;
    }
    private void AddBonusToList(GameElement element)
    {
        if (!currentBonus.Contains(element))
        {
            currentBonus.Add(element);
        }
    }
    private void IsMatchHaveBonus(GameElement element1, GameElement element2, GameElement element3)
    {
        if (IsBonus(element1))
            AddBonusToList(element1);
        if (IsBonus(element2))
            AddBonusToList(element2);
        if (IsBonus(element3))
            AddBonusToList(element3);
    }


    private void AddToListAndMatch(GameElement element)
    {
        if (!currentMatches.Contains(element))
        {
            currentMatches.Add(element);
        }
        element.ChangeMatchState(true);
    }

    private void GetNearbyPieces(GameElement element1, GameElement element2, GameElement element3)
    {
        AddToListAndMatch(element1);
        AddToListAndMatch(element2);
        AddToListAndMatch(element3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                GameElement currentElement = GameManager.Instance.Grid[i, j];
                if (currentElement != null)
                {
                    if (i > 0 && i < Settings.Instance.Columns - 1)
                    {
                        GameElement leftElement = GameManager.Instance.Grid[i - 1, j];
                        GameElement rightElement = GameManager.Instance.Grid[i + 1, j];
                        if (leftElement != null && rightElement != null)
                        {
                            if (leftElement.ColorType == currentElement.ColorType && rightElement.ColorType == currentElement.ColorType)
                            {
                                IsMatchHaveBonus(leftElement, currentElement, rightElement);
                                GetNearbyPieces(leftElement, currentElement, rightElement);
                            }
                        }
                    }

                    if (j > 0 && j < Settings.Instance.Rows - 1)
                    {
                        GameElement upElement = GameManager.Instance.Grid[i, j + 1];
                        GameElement downElement = GameManager.Instance.Grid[i, j - 1];
                        if (upElement != null && downElement != null)
                        {
                            if (upElement.ColorType == currentElement.ColorType && downElement.ColorType == currentElement.ColorType)
                            {
                                IsMatchHaveBonus(upElement, currentElement, downElement);
                                GetNearbyPieces(upElement, currentElement, downElement);
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
            else if (GameManager.Instance.currentElement.otherDot != null)
            {
                GameElement otherDot = GameManager.Instance.currentElement.otherDot;
                if (otherDot.IsMatched)
                {
                    otherDot.ChangeMatchState(false);
                    otherDot.SetBonusType(BonusTypeEnum.ChangeCravityBonus);
                }
            }

        }
    }

    //Баг с уничтожением разных цветов
    private void DestroyMatchesAt(int column, int row)
    {
        if (GameManager.Instance.Grid[column, row].IsMatched)
        {
            if (currentMatches.Count >= 4)
            {
                CheckBonus();
            }
            Destroy(GameManager.Instance.Grid[column, row].gameObject);
            GameManager.Instance.Grid[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        foreach (var item in currentBonus)
        {
            if (item.BonusType == BonusTypeEnum.ChangeCravityBonus)
            {
                isBonus = !isBonus;
            }
        }
        for (int i = 0; i < Settings.Instance.Columns; i++)
        {
            for (int j = 0; j < Settings.Instance.Rows; j++)
            {
                if (GameManager.Instance.Grid[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
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
        StartCoroutine(FillBoardCo());
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
        StartCoroutine(FillBoardCo());
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

    private IEnumerator FillBoardCo()
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
        GameManager.Instance.currentState = GameState.MovingGameElements;

    }
}
