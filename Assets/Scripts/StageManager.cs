using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class StageManager : MonoBehaviour
{
    // =========== Stage ==============
    // Stage tempaltes
    public List<GameObject> stageTemplates = new List<GameObject>();
    // Initial Stage
    public GameObject initStage;
    // Current Stage
    public GameObject currentStage;
    // Next Stage
    public GameObject nextStage;
    // Coin
    public GameObject coin;
    // Stage List
    private List<GameObject> stageSpawnList = new List<GameObject>();

    // Initial Position and Scale of the stage
    public Vector3 stageInitPosition;
    public Vector3 stageInitScale;

    // Maximum Distance to generate a stage
    private float maxDistance = 2;
    private List<Vector3> directionList = new List<Vector3> { new Vector3(1, 0, 0), new Vector3(0, 0, 1) };

    private int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = initStage;
        stageInitPosition = initStage.transform.localPosition;
        stageInitScale = initStage.transform.localScale;

        SpawnStage();
    }

    public void SpawnStage()
    {
        if (counter < 4)
        {
            counter++;
        }
        // Randomly pick out a stage from the template
        GameObject prefab = stageTemplates[Random.Range(0, stageTemplates.Count)];
        nextStage = GameObject.Instantiate(prefab);
        coin.SetActive(true);

        // Randomly create the stage on top side or right side of the current stage
        nextStage.transform.position = currentStage.transform.position + directionList[Random.Range(0, directionList.Count)] * Random.Range(1.2f, maxDistance);
        coin.transform.position = nextStage.transform.position + new Vector3(0f, 1f, 0f);
        // Randomly set the scale in a range
        var randomScale = Random.Range(0.5f, 1);
        nextStage.transform.localScale =
            new Vector3(randomScale - 0.05f * counter, 0.5f, randomScale - 0.05f * counter);
        // Randomly select a color
        nextStage.GetComponent<Renderer>().material.color =
            new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        
        stageSpawnList.Add(nextStage);
        if (stageSpawnList.Count > 2)
        {
            GameObject go = stageSpawnList[0];
            stageSpawnList.RemoveAt(0);
            Destroy(go);
        }
    }

}
