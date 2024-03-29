﻿using UnityEngine;

public class Baloon : MonoBehaviour
{
    [SerializeField] SpriteRenderer changeGravityBonusSpritePrefab;

    [HideInInspector] public BonusTypeEnum BonusType { get; private set; }
    [HideInInspector] public int ColorType { get; private set; }
    [HideInInspector] public int Column { get; private set; }
    [HideInInspector] public int Row { get; private set; }
    [HideInInspector] public bool IsMatched { get; private set; }

    [HideInInspector] public Baloon OtherBaloon;
    
    private SpriteRenderer sr;

    private void Update()
    {
        SetMatchedColor();
    }
    private void SetMatchedColor()
    {
        if (IsMatched)
        {
            sr = GetComponent<SpriteRenderer>();
            Color currentColor = sr.color;
            sr.color = new Color(currentColor.r, currentColor.g, currentColor.b, .5f);
        }
        if (!IsMatched)
        {
            sr = GetComponent<SpriteRenderer>();
            Color currentColor = sr.color;
            sr.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }
    }
    private void SetColor(int color)
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        ColorType = color;
        sr.color = Settings.Instance.colorArray[(int)color];
    }

    public Baloon()
    {
        BonusType = BonusTypeEnum.None;
    }

    public bool IsColorType(Baloon ge)
    {
        return ge.ColorType == ColorType;
    }

    public void Init(int column, int row, int color)
    {
        SetColum(column);
        SetRow(row);
        SetColor(color);
        SetBonusType(BonusTypeEnum.None);
    }
    public void SetColum(int column)
    {
        Column = column;
    }
    public void SetRow(int row)
    {
        Row = row;
    }

    public void ChangeColumAndRowTo(int columnChangeValue, int rowChangeValue)
    {
        Column += columnChangeValue;
        Row += rowChangeValue;
    }
    public void ChangeMatchState(bool isMatched)
    {
        IsMatched = isMatched;
    }
    public void SetBonusType(BonusTypeEnum bonusType)
    {
        BonusType = bonusType;
        if (BonusType == BonusTypeEnum.None)
        {
            return;
        }
        if (BonusType == BonusTypeEnum.ChangeCravityBonus)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, -0.5f);
            Instantiate(changeGravityBonusSpritePrefab, pos, Quaternion.identity, transform);
        }
        
    }

}
