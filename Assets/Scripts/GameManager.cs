using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(-300, 20, -400);
        cube.GetComponent<MeshRenderer>().material.color = Color.red;
        List<Vector3> path = Pathfinder.instance.AStar(new Vector3(-300, Pathfinder.instance.height_detection_threshold, -400), new Vector3(300, Pathfinder.instance.height_detection_threshold, 400));
        foreach(Vector3 positions in path)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = positions + new Vector3(0, 10, 0);
            cube.GetComponent<MeshRenderer>().material.color = Color.blue;
            cube.transform.localScale = cube.transform.localScale * 5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
