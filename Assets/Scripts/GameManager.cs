using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public bool hasGameStarted;
    private int health = 3;

    private int enemyCount;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        WinGame();
    }
    public void KillEnemyCount()
    {
        enemyCount--;
    }
    public void Restart()
    {
        enemyCount = 0;
        hasGameStarted = false;
        SceneManager.LoadScene(0);
    }
    private void WinGame()
    {
        if(enemyCount <= 0)
        {
            Debug.Log("Game Won!");
        }
    }
    public void InitEnemy()
    {
        enemyCount++;
    }
}
