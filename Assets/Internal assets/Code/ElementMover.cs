using UnityEngine;
using UnityEditor;
using System.Collections;

public class ElementMover : MonoBehaviour
{
    private GameElement gameElement;
    private int previousColumn;
    private int previousRow;
    private int targetX;
    private int targetY;
    private Vector2 mouseDownPosition;
    private Vector2 mouseUpPosition;
    private Vector2 tempPosition;

    private float swipeAngle = 0;
    private float swipeResist = 0.1f;

    private void Start()
    {
        gameElement = gameObject.GetComponent<GameElement>();
    }
    private void Update()
    {
        targetX = gameElement.Column;
        targetY = gameElement.Row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (GameManager.Instance.Grid[gameElement.Column, gameElement.Row] != gameElement)
            {
                GameManager.Instance.Grid[gameElement.Column, gameElement.Row] = gameElement;
            }
            GameManager.Instance.MatchManager.ProcessMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;

        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            //TODO: проверить это место
            if (GameManager.Instance.Grid[gameElement.Column, gameElement.Row] != gameElement)
            {
                GameManager.Instance.Grid[gameElement.Column, gameElement.Row] = gameElement;
            }
            GameManager.Instance.MatchManager.ProcessMatches();

        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;

        }

    }
    private void OnMouseDown()
    {
        if (GameManager.Instance.currentState == GameState.MovingGameElements)
        {
            mouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.currentState == GameState.MovingGameElements)
        {
            mouseUpPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    void CalculateAngle()
    {
        if (Mathf.Abs(mouseUpPosition.y - mouseDownPosition.y) > swipeResist || Mathf.Abs(mouseUpPosition.x - mouseDownPosition.x) > swipeResist)
        {
            GameManager.Instance.currentState = GameState.WaitForMatching;
            swipeAngle = Mathf.Atan2(mouseUpPosition.y - mouseDownPosition.y, mouseUpPosition.x - mouseDownPosition.x) * 180 / Mathf.PI;
            MoveElement();

            GameManager.Instance.currentElement = gameElement;
        }
        else
        {
            GameManager.Instance.currentState = GameState.MovingGameElements;
        }
    }
    void MoveElementActual(Vector2Int diraction)
    {
        gameElement.otherDot = GameManager.Instance.Grid[gameElement.Column + diraction.x, gameElement.Row + diraction.y];
        previousRow = gameElement.Row;
        previousColumn = gameElement.Column;
        gameElement.otherDot.ChangeColumAndRowTo(-1 * diraction.x, -1 * diraction.y);
        gameElement.ChangeColumAndRowTo(diraction.x, diraction.y);
        StartCoroutine(CheckMoveCo());
    }
    void MoveElement()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && gameElement.Column < Settings.Instance.Columns - 1)
        {
            MoveElementActual(Vector2Int.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && gameElement.Row < Settings.Instance.Rows - 1)
        {
            MoveElementActual(Vector2Int.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && gameElement.Column > 0)
        {
            MoveElementActual(Vector2Int.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && gameElement.Row > 0)
        {
            MoveElementActual(Vector2Int.down);
        }
        else
        {
            GameManager.Instance.currentState = GameState.MovingGameElements;
        }

    }
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if (gameElement.otherDot != null)
        {
            if (!gameElement.IsMatched && !gameElement.otherDot.IsMatched)
            {
                gameElement.otherDot.SetRow(gameElement.Row);
                gameElement.otherDot.SetColum(gameElement.Column);
                gameElement.SetRow(previousRow);
                gameElement.SetColum(previousColumn);
                yield return new WaitForSeconds(.5f);
                GameManager.Instance.currentElement = null;
                GameManager.Instance.currentState = GameState.MovingGameElements;
            }
            else
            {
                GameManager.Instance.MatchManager.DestroyMatches();

            }
        }

    }
}