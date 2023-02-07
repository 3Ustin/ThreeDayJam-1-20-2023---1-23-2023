using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    //Serialized Fields
    //TileMaps
    public Tilemap Background;
    public Tilemap Settlement;
    public Tilemap Mountains;
    public Tilemap Forests;

    //Tiles for Background Drawing
    public TileBase plainHover;
    public TileBase plain;
    public TileBase avalanche;

    ///Tiles for Settlements Drawing
    public TileBase Village;
    public TileBase VillagePlus;
    public TileBase VillagePlusPlus;
    public TileBase Town;
    public TileBase TownPlus;
    public TileBase TownPlusPlus;
    public TileBase Kingdom;
    public TileBase KingdomPlus;
    public TileBase KingdomPlusPlus;

    //resources tiles
    public TileBase forest1;
    public TileBase forest2;
    public TileBase forest3;
    public TileBase forest4;
    public TileBase mountain00;
    public TileBase mountain01;
    public TileBase mountain02;
    public TileBase mountain03;

    private int wood;
    private int metal;
    private int stone;

    //Time Tracker Value
    public TimeTracker timeTracker;

    //Private Fields
    private Vector3 mouseWorldPos;
    private Grid grid;
    private bool mouseDown0;
    private bool mouseDown1;
    public LinkedList<Vector3Int> mouseDown0TileList = new LinkedList<Vector3Int>();
    private LinkedList<Vector3Int> currentSettlements = new LinkedList<Vector3Int>();
    private TileBase mouse0Highlight;
    private TileBase mouse1HighlightTile;
    private Vector3Int mouse1HighlightLocation;
    
    void Start()
    {
        //Variable instantiations
        mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        grid = this.GetComponent<Grid>();
        mouseDown0 = false;
        mouseDown1 = false;
        for(int x = Settlement.cellBounds.min.x; x< Settlement.cellBounds.max.x;x++){
        for(int y= Settlement.cellBounds.min.y; y< Settlement.cellBounds.max.y;y++){
            if(Settlement.GetTile( new Vector3Int(x,y,0)) != null){
                currentSettlements.AddLast(new Vector3Int(x,y,0));
            }
        }}
        wood = 1000000;
        stone = 1000000;
        metal = 1000000;
    }

    // Update is called once per frame
    void Update()
    {
                                                        //
                                                        //
                                                        //
//                                                      */
//             MouseButton1 interations                 */
//                                                      */
        if(Input.GetMouseButtonDown(1)){
            mouse1HighlightTile = getTile()[1];
            mouse1HighlightLocation = getTileLocation();
        }
        if(Input.GetMouseButtonUp(1)){
            //Upgrading currently highlighted building.
            if(getTile()[1] == mouse1HighlightTile && getTileLocation() == mouse1HighlightLocation){
                if(mouse1HighlightTile == Village && wood >= 3){ Settlement.SetTile(getTileLocation(), Town); wood-= 3;}
                if(mouse1HighlightTile == Town && wood >= 5){ Settlement.SetTile(getTileLocation(), Kingdom); wood -= 5;}
                if(mouse1HighlightTile == Kingdom && wood >= 10 && stone >= 3){ Settlement.SetTile(getTileLocation(), null); currentSettlements.Remove(getTileLocation()); metal+=1; wood-=10; stone-=3;}
                if(mouse1HighlightTile == VillagePlus && wood >= 10){ Settlement.SetTile(getTileLocation(), TownPlus); wood -= 10;}
                if(mouse1HighlightTile == TownPlus && wood >= 15){ Settlement.SetTile(getTileLocation(), KingdomPlus); wood-=15;}
                if(mouse1HighlightTile == KingdomPlus && wood >= 30 && stone >= 5){ Settlement.SetTile(getTileLocation(), null); currentSettlements.Remove(getTileLocation()); metal+=2; wood-=30; stone-=5;}
                if(mouse1HighlightTile == VillagePlusPlus && wood >= 30){ Settlement.SetTile(getTileLocation(), TownPlusPlus); wood-=30;}
                if(mouse1HighlightTile == TownPlusPlus && wood >= 35){ Settlement.SetTile(getTileLocation(), KingdomPlusPlus); wood-=35;}
                if(mouse1HighlightTile == KingdomPlusPlus && wood >= 75 && stone >= 10){ Settlement.SetTile(getTileLocation(), null); currentSettlements.Remove(getTileLocation()); metal+=100;wood-=75;stone-=10;}
            }
        }
                                                        //
                                                        //
                                                        //
//                                                      */
//             MouseButton0 interations                 */
//                                                      */
        if(Input.GetMouseButtonDown(0)){
            if(getTile()[0] == plain && getTile() != null && getTile()[1] != null){
                mouseDown0TileList.AddLast(getTileLocation());
                mouse0Highlight = getTile()[1];
            }
            mouseDown0 = true;
        }
        if(Input.GetMouseButtonUp(0)){
            mouseDown0 = false;
        }
        if(mouseDown0){
            if(
                ((mouse0Highlight == Village || mouse0Highlight == VillagePlus || mouse0Highlight == VillagePlusPlus) && mouseDown0TileList.Count <=1) ||
                ((mouse0Highlight == Town || mouse0Highlight ==TownPlus || mouse0Highlight == TownPlusPlus) && mouseDown0TileList.Count <=3) ||
                ((mouse0Highlight == Kingdom || mouse0Highlight == KingdomPlus || mouse0Highlight == KingdomPlusPlus) && mouseDown0TileList.Count <=5)
            ){
                if(getTile()[3] != forest4){
                    if(getTile()[0] == plain && getTile() != null && mouseDown0TileList.Count != 0 && getTile()[3] != forest4){
                            if(!mouseDown0TileList.Contains(getTileLocation())){
                                mouseDown0TileList.AddLast(getTileLocation());
                            }
                            Background.SetTile(mouseDown0TileList.Last.Value, plainHover);
                    }
                }
                else{
                    mouseDown0 = false;
                }
            }
        }
        else{
            foreach(Vector3Int value in mouseDown0TileList){
                Background.SetTile(value,plain);
            }
            for(LinkedListNode<Vector3Int> node = mouseDown0TileList.First; node != null; node = node.Next){
                if(node.Next == null && !timeTracker.buildingsToBuild.Contains(node.Value)){
                    timeTracker.buildingsToBuild.AddLast(node.Value);
                }
            }
            mouse0Highlight = null;
            mouseDown0TileList.Clear();
        }
    }






    private Vector3Int getTileLocation(){
        mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        Vector3Int gridLocation = grid.WorldToCell(mouseWorldPos);
        //BUGFIX: reset the z axis to 0 for some reason.
        gridLocation.z = 0;
        return gridLocation;
    }

    private TileBase[] getTile(){
            //Returning all tiles associated to grid location.
            TileBase[] returnedTiles = new TileBase[6];
            returnedTiles[0] = Background.GetTile(getTileLocation());
            returnedTiles[1] = Settlement.GetTile(getTileLocation());
            returnedTiles[2] = Mountains.GetTile(getTileLocation());
            returnedTiles[3] = Forests.GetTile(getTileLocation());
            return returnedTiles;
    }

    private void clearSelections(){
            foreach(Vector3Int value in mouseDown0TileList){
                Background.SetTile(value,plain);
            }
            mouseDown0TileList.Clear();
    }

    public void moveAvalanche(){
        //Check for wether mountains slow down the Avalanche movement.
        clearSelections();
        //Then MoveAvalanche along map
        int y = 5 - timeTracker.timeTrackerValue/timeTracker.AvalancheSpeed;
        for(int x = -11; x <= -1; x ++){
            Vector3Int vect = new Vector3Int(x,y,0);
            Background.SetTile(vect, avalanche);
            Forests.SetTile(vect, null);
            //Check for Settlement, if so Delete from current Settlement arr
            if(Settlement.GetTile(vect) != null){
                for(LinkedListNode<Vector3Int> node = currentSettlements.First; node != null; node = node.Next){
                    if(vect == node.Value){
                        currentSettlements.Remove(vect);
            }}}
            Settlement.SetTile(vect,null);
        }
    }

    //Build A Building at end of highlight.
    public void buildBuilding(){
        Debug.Log(timeTracker.buildingsToBuild.Count);
            for(LinkedListNode<Vector3Int> node = timeTracker.buildingsToBuild.First; node != null; node = node.Next){
                    if(Settlement.GetTile(node.Value) == null){
                        Settlement.SetTile(node.Value, Village); currentSettlements.AddLast(node.Value); wood-=3;
                        /*Debug.Log(mouse0Highlight);
                        if(mouse0Highlight == Village){Settlement.SetTile(node.Value, Village); currentSettlements.AddLast(node.Value); wood-=3;}
                        if(mouse0Highlight == VillagePlus){Settlement.SetTile(node.Value, VillagePlus); currentSettlements.AddLast(node.Value); wood-=10;}
                        if(mouse0Highlight == VillagePlusPlus){Settlement.SetTile(node.Value, VillagePlusPlus); currentSettlements.AddLast(node.Value); wood-=30;}
                        if(mouse0Highlight == Town){Settlement.SetTile(node.Value, Village); currentSettlements.AddLast(node.Value); wood-=3;}
                        if(mouse0Highlight == TownPlus){Settlement.SetTile(node.Value, VillagePlus); currentSettlements.AddLast(node.Value); wood-=10;}
                        if(mouse0Highlight == TownPlusPlus){Settlement.SetTile(node.Value, VillagePlusPlus); currentSettlements.AddLast(node.Value); wood-=30;}
                        if(mouse0Highlight == Kingdom){Settlement.SetTile(node.Value, VillagePlus ); currentSettlements.AddLast(node.Value); wood-=3;}
                        if(mouse0Highlight == KingdomPlus){Settlement.SetTile(node.Value, VillagePlusPlus); currentSettlements.AddLast(node.Value); wood-=10;}
                        if(mouse0Highlight == KingdomPlusPlus){Settlement.SetTile(node.Value, KingdomPlusPlus); currentSettlements.AddLast(node.Value); wood-=30;} */
                    }
            }
        timeTracker.buildingsToBuild.Clear();
    }

    //Mine all resources. Tick related see TimeTracker for more info.
    public void mineResources(){
        TileBase forest;
        TileBase mountain;
        //For each settlement we'll mine the resource alotted.
        foreach(Vector3Int settlement in currentSettlements){
            forest = Forests.GetTile(settlement);
            mountain = Mountains.GetTile(settlement);
            //mine forests
            if(forest != null){
                if(forest == forest1){Forests.SetTile(settlement,null); wood+=1;}
                if(forest == forest2){Forests.SetTile(settlement,forest1); wood+=1;}
                if(forest == forest3){Forests.SetTile(settlement,forest2); wood+=1;}
            }
            //mine mountains
            //BALANCECHANGE: Put a check to see if it is a Kingdom. Only kingdoms mine mountains.
            if(mountain != null){
                if(mountain == mountain00){Mountains.SetTile(settlement,mountain01);stone+=1;}
                if(mountain == mountain01){Mountains.SetTile(settlement,mountain02);stone+=1;}
                if(mountain == mountain02){Mountains.SetTile(settlement,mountain03);stone+=1;}
                if(mountain == mountain03){Mountains.SetTile(settlement,null); metal+=1;}
            }
        }
    }
}
