using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTracker : MonoBehaviour
{
    public int AvalancheSpeed;
    public GameObject timeTracker0;
    public GameObject timeTracker1;
    public GridController gridController;
    public int timeTrackerValue;
    public LinkedList<Vector3Int> buildingsToBuild  = new LinkedList<Vector3Int>();
    private float currentTimeDifference;
    private bool MoveTimeTracker;
    private SpriteRenderer timeTracker0Renderer;
    private SpriteRenderer timeTracker1Renderer;

    // Start is called before the first frame update
    void Start()
    {
        timeTrackerValue = 0;
        MoveTimeTracker = false;
        AvalancheSpeed = 2;
        timeTracker0Renderer = timeTracker0.GetComponent<SpriteRenderer>();
        timeTracker1Renderer = timeTracker1.GetComponent<SpriteRenderer>();
        switchSpriteOpacity(timeTracker1Renderer);
    }

    // Update is called once per frame
    void Update()
    {
        //difference in time from last frame
        currentTimeDifference += Time.deltaTime;
        //when a second has passed move the ticker timer
        if(currentTimeDifference >= .98){
            timeTrackerValue += 1;
            currentTimeDifference = 0;
            MoveTimeTracker = true;
        }
        //run all code associated to the tick. 
        if(MoveTimeTracker){
            //move ticker
            switchSpriteOpacity(timeTracker0Renderer);
            switchSpriteOpacity(timeTracker1Renderer);
            //mine resources
            gridController.mineResources();
            //build buildings
            gridController.buildBuilding();
            //Avalanche if we've moved ten times.
            if(timeTrackerValue%AvalancheSpeed == 0 && timeTrackerValue != 0){
                Camera.main.transform.Translate(new Vector3(.508f,-.248f,0));
                gridController.moveAvalanche();
            }
            MoveTimeTracker = false;
        }
    }

    private void switchSpriteOpacity(SpriteRenderer sprite){
        Color color = sprite.material.color;
        if(color.a == 0){
            color.a = 1;
            sprite.material.color = color;
        }
        else if(color.a == 1){
            color.a = 0;
            sprite.material.color = color;
        }
    }
    
    private void buildBuildings(){

    }
}
