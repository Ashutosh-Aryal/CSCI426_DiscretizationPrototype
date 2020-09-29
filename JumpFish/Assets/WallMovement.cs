using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    private const float TIME_BETWEEN_SPAWNS = 5.0f;
    private const float LEFT_X_BOUNDARY = -220.0f;
    
    private bool m_ShouldSpawnWall = true;

    private float m_SpawnTimer = 0.0f;

    [SerializeField]
    private GameObject m_WallsContainer;

    [SerializeField]
    private Transform m_WallSpawnLocation;

    private List<GameObject> m_Walls = new List<GameObject>();

    private List<GameObject> m_MovingWalls = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int numChildren = m_WallsContainer.transform.childCount;

        for(int x = 0; x < numChildren; x++) {
            m_Walls.Add(m_WallsContainer.transform.GetChild(x).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_ShouldSpawnWall)
        {
            SpawnWall();
        }

        CheckIfOutOfBounds();

        m_SpawnTimer += Time.deltaTime;
    }

    private void SpawnWall()
    {
        int randIndex = Random.Range(0, m_Walls.Count);

        GameObject spawnedWall = Instantiate(m_Walls[randIndex], m_WallSpawnLocation);
        spawnedWall.GetComponent<Rigidbody2D>().velocity = new Vector2(-100.0f, 0.0f);

        m_MovingWalls.Add(spawnedWall);

        m_ShouldSpawnWall = false;
    }

    private void CheckIfOutOfBounds()
    {
        List<int> removedIndices = new List<int>();
        
        for(int x = 0; x < m_MovingWalls.Count; x++)
        {
            if(m_MovingWalls[x].transform.position.x <= LEFT_X_BOUNDARY)
            {
                removedIndices.Add(x);
                m_ShouldSpawnWall = true;
            }
        }

        foreach(int index in removedIndices)
        {
            Destroy(m_MovingWalls[index]);
            m_MovingWalls.RemoveAt(index);
        }

    }
}
