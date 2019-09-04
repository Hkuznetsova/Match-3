using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] MatchManager matchManager;
    public SpawnManager SpawnManager { get { return spawnManager; } }
    public MatchManager MatchManager { get { return matchManager; } }

    [HideInInspector] public GameState currentState = GameState.MovingGameElements;

    [HideInInspector] public GameElement[,] Grid;

    [HideInInspector] public GameElement currentElement;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
