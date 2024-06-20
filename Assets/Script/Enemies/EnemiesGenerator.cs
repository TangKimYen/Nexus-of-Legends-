using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class EnemiesGenerator : MonoBehaviour
{
    [SerializeField] PhotonView myPV;
    public GameObject enemyPrefab;
    [SerializeField] float yRange;
    [SerializeField] int enemyCount;
    EnemiesList enemiesList;
    [SerializeField] Transform xRangeMin;
    [SerializeField] Transform xRangeMax;
    
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Here");
            enemiesList = new EnemiesList();
            enemiesList.List = new List<Vector2>();

            HashSet<int> usedRandx = new HashSet<int>();

            for (int i = 0; i < enemyCount; i++)
            {
                int randx;
                do
                {
                    randx = (int)Random.Range(xRangeMin.position.x, xRangeMax.position.x);
                } while (usedRandx.Contains(randx));

                usedRandx.Add(randx);

                GameObject enemy = Instantiate(enemyPrefab, transform);
                enemy.transform.position = new Vector3(randx, yRange, 0);
                enemiesList.List.Add(enemy.transform.position);
            }

            string enemyString = JsonUtility.ToJson(enemiesList);
            myPV.RPC("RPC_SyncEnemies", RpcTarget.OthersBuffered, enemyString);
        }
    }

    public void RPC_SyncEnemies(string enemies)
    {
        enemiesList = JsonUtility.FromJson<EnemiesList>(enemies);
        Debug.Log(enemiesList.List.Count);
        for (int i = 0; i < enemiesList.List.Count; i++) 
        {
            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.transform.position = new Vector3(enemiesList.List[i].x, enemiesList.List[i].y, 0);
        }
    }

    class EnemiesList
    {
        public List<Vector2> List;
    }
}
