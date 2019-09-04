using UnityEngine;
using UnityEditor;
using System.Collections;

public class BaloonMoveController: MonoBehaviour
{
    private Baloon baloon;
    private Vector2 mouseDownPosition;
    private Vector2 mouseUpPosition;
    private Vector2 tempPosition;

    private float swipeResist = 0.1f;

    private void Start()
    {
        baloon = gameObject.GetComponent<Baloon>();
    }

    private void Update()
    {
        if (Mathf.Abs(baloon.Column - transform.position.x) > .1)
        {
            tempPosition = new Vector2(baloon.Column, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (GameManager.Instance.Grid[baloon.Column, baloon.Row] != baloon)
            {
                GameManager.Instance.Grid[baloon.Column, baloon.Row] = baloon;
            }
            GameManager.Instance.MatchManager.ProcessMatches();
        }
        else
        {
            tempPosition = new Vector2(baloon.Column, transform.position.y);
            transform.position = tempPosition;

        }
        if (Mathf.Abs(baloon.Row - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, baloon.Row);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (GameManager.Instance.Grid[baloon.Column, baloon.Row] != baloon)
            {
                GameManager.Instance.Grid[baloon.Column, baloon.Row] = baloon;
            }
            GameManager.Instance.MatchManager.ProcessMatches();

        }
        else
        {
            tempPosition = new Vector2(transform.position.x, baloon.Row);
            transform.position = tempPosition;

        }

    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.currentState == GameState.WaitingForAction)
        {
            mouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.currentState == GameState.WaitingForAction)
        {
            mouseUpPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Mathf.Abs(mouseUpPosition.y - mouseDownPosition.y) > swipeResist || Mathf.Abs(mouseUpPosition.x - mouseDownPosition.x) > swipeResist)
            {
                GameManager.Instance.currentState = GameState.Matching;
                float swipeAngle = Mathf.Atan2(mouseUpPosition.y - mouseDownPosition.y, mouseUpPosition.x - mouseDownPosition.x) * 180 / Mathf.PI;
                StartCoroutine(MoveBaloonByAngleCo(swipeAngle));

                GameManager.Instance.currentElement = baloon;
            }
            else
            {
                GameManager.Instance.currentState = GameState.WaitingForAction;
            }
        }
    }
    private IEnumerator MoveBaloonByAngleCo(float swipeAngle)
    {
        Vector2Int vector = TransformAngleToVector(swipeAngle);

        if (vector == Vector2Int.zero)
        {
            GameManager.Instance.currentState = GameState.WaitingForAction;
            yield break;
        }

        baloon.OtherBaloon = GameManager.Instance.Grid[baloon.Column + vector.x, baloon.Row + vector.y];
        baloon.OtherBaloon.ChangeColumAndRowTo(-1 * vector.x, -1 * vector.y);
        int previousRow = baloon.Row;
        int previousColumn = baloon.Column;
        baloon.ChangeColumAndRowTo(vector.x, vector.y);
        yield return new WaitForSeconds(.5f);
        if (!baloon.IsMatched && !baloon.OtherBaloon.IsMatched)
        {
            baloon.OtherBaloon.SetRow(baloon.Row);
            baloon.OtherBaloon.SetColum(baloon.Column);
            baloon.SetRow(previousRow);
            baloon.SetColum(previousColumn);
            yield return new WaitForSeconds(.5f);
            GameManager.Instance.currentElement = null;
            GameManager.Instance.currentState = GameState.WaitingForAction;
        }
        else
        {
            GameManager.Instance.MatchManager.DestroyMatches();

        }
    }

    private Vector2Int TransformAngleToVector (float angle)
    {
        if (angle > -45 && angle <= 45 && baloon.Column < Settings.Instance.Columns - 1)
        {
            return Vector2Int.right;
        }
        else if (angle > 45 && angle <= 135 && baloon.Row < Settings.Instance.Rows - 1)
        {
            return Vector2Int.up;
        }
        else if ((angle > 135 || angle <= -135) && baloon.Column > 0)
        {
            return Vector2Int.left;
        }
        else if (angle < -45 && angle >= -135 && baloon.Row > 0)
        {
            return Vector2Int.down;
        }
        
        return Vector2Int.zero;
    }
}