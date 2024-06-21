using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class EnemiesGenerator : MonoBehaviourPunCallbacks
{
    public static EnemiesGenerator Instance;

    [SerializeField] PhotonView myPV;
    public GameObject enemyPrefab;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] int enemyCount;
    EnemiesList enemiesList;
    
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Here");
            enemiesList = new EnemiesList();
            enemiesList.List = new List<Vector2>();

            HashSet<float> usedRandx = new HashSet<float>();

            for (int i = 0; i < enemyCount; i++)
            {
                float randx;
                do
                {
                    randx = (float)Random.Range(SpawnPoint.position.x - 5f, SpawnPoint.position.x + 5f);
                } while (usedRandx.Contains(randx));

                usedRandx.Add(randx);

                GameObject enemy = Instantiate(enemyPrefab, transform);
                enemy.transform.position = new Vector3(randx, SpawnPoint.position.y, 0);
                enemiesList.List.Add(enemy.transform.position);
            }

            string enemyString = JsonUtility.ToJson(enemiesList);
            myPV.RPC("RPC_SyncEnemies", RpcTarget.OthersBuffered, enemyString);
        }
    }

    [PunRPC]
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
