using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        moveWalls(1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void moveWalls(int i)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Wall");
        GameObject go = objects[i];
        while(transform.position.x > -300)
        {
            go.transform.position = new Vector3(go.transform.position.x - 0.01f, go.transform.position.y, go.transform.position.z);
        }  
        
    }
}
