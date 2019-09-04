using UnityEngine;
using UnityEditor;

public class Settings : MonoBehaviour
{
    [SerializeField] public Color[] colorArray;

    [SerializeField] public int Rows = 12;
    [SerializeField] public int Columns = 8;

    public static  Settings Instance;

    private void Awake()
    {
        Instance = this;
    }
}