using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MapGenerator1 : MonoBehaviour{



    
    public int gridWidth;
    public int gridHeight;

    public GameObject mapRoot;
    
    List<Vector2Int> frontierCells = new List<Vector2Int>();
    private struct Cell{ 
        public bool active;
        public bool frontier;
        public bool N;
        public bool E;
        public bool S;
        public bool W;
        public int depth;
        public int startDirection; //0-3 N-W
    }
    public float cellWidth;
    public float cellHeight;
    public float hallwayLength;
    public float spawnRoomHeight;
    
    private Cell[,] map;
    private int furthestDist;
    private Vector2Int furthestCell;

    public GameObject[] roomPrefabN;
    public GameObject[] roomPrefabNE;
    public GameObject[] roomPrefabNES;
    public GameObject[] roomPrefabNESW;
    public GameObject[] roomPrefabNS; 
    public GameObject[] hallwayPrefabs;
    public GameObject spawnPrefab;
    public GameObject gateRoom;


    private Dictionary<(bool, bool, bool, bool), float>
        rotationDict = new Dictionary<(bool, bool, bool, bool), float>() {
            {(true, false, false, false), 0}, //n
            {(false, true, false, false), 90}, //e
            {(false, false, true, false), 180}, //s
            {(false, false, false, true), 270}, //w
            
            {(true, true, false, false), 0}, //ne
            {(false, true, true, false), 90}, //es
            {(false, false, true, true), 180}, //sw
            {(true, false, false, true), 270}, //wn
            
            {(true, true, true, false), 0}, //nes
            {(false, true, true, true), 90}, //esw
            {(true, false, true, true), 180}, //swn
            {(true, true, false, true), 270}, //wne
            
            {(true, false, true, false), 0}, // ns
            {(false, true, false, true), 90}, //ew
            
            {(true, true, true, true), 0}, //nesw
            {(false, false, false, false), 0} //
            
        };

    private Dictionary<(bool, bool, bool, bool), GameObject[]> prefabDict = new Dictionary<(bool, bool, bool, bool), GameObject[]>();

    private Cell CreateDefaultCell(){
        Cell cell = new Cell();
        
        cell.active = false;
        cell.frontier = false;
        cell.N = false;
        cell.E = false;
        cell.S = false;
        cell.W = false;
        cell.depth = 0;
        
        return cell;
    }

    void SetCellDirections(int x, int y, bool N, bool E, bool S, bool W){
        Cell cell = map[y, x];
        cell.N = N;
        cell.E = E;
        cell.S = S;
        cell.W = W;

        map[y, x] = cell;
    }

    void InitMap(){
        for (int y = 0; y < gridHeight; y++) {
            for (int x = 0; x < gridWidth; x++) {
                map[y, x] = CreateDefaultCell();
            }
        }
    }

    void GenerateModels(){
        for (int y = 0; y < gridHeight; y++) {
            for (int x = 0; x < gridWidth; x++) {
                Vector3 pos = new Vector3(
                    x * (cellWidth + hallwayLength) - (cellWidth + hallwayLength) * gridWidth/2,
                    1,
                    y * (cellHeight + hallwayLength) - (cellHeight + hallwayLength) * gridHeight/2);

                Cell cell = map[y, x];
                (bool, bool, bool, bool) dirs = (cell.N, cell.E, cell.S, cell.W);
                
                GameObject[] prefabs = prefabDict[dirs];
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                float yRot = rotationDict[dirs];

                Quaternion rot = Quaternion.Euler(0, yRot, 0);

                GameObject newCell;
                if (x == furthestCell.x && y == furthestCell.y) {
                    newCell = Instantiate(gateRoom, pos, rot, mapRoot.transform);
                }
                else {
                    newCell = Instantiate(prefab, pos, rot, mapRoot.transform);
                }



                DungeonRoom roomScript = newCell.GetComponent<DungeonRoom>();
                roomScript.roomID = y * gridWidth + x;
                roomScript.startDirection = map[y, x].startDirection;
                roomScript.roomRotation = yRot;

                if (map[y, x].E) {
                    Vector3 hallPos = pos + new Vector3(cellWidth / 2 + hallwayLength / 2, 0, 0);
                    prefab = hallwayPrefabs[Random.Range(0, hallwayPrefabs.Length)];
                    Instantiate(prefab, hallPos, Quaternion.Euler(0,90,0), mapRoot.transform);
                }
                if (map[y, x].S) {
                    Vector3 hallPos = pos - new Vector3(0, 0, cellHeight / 2 + hallwayLength / 2);
                    prefab = hallwayPrefabs[Random.Range(0, hallwayPrefabs.Length)];
                    Instantiate(prefab, hallPos, Quaternion.Euler(0,0,0), mapRoot.transform);
                }
            }
        }
        
        //create start room
        int startCellX = gridWidth / 2;
        int startCellY = 0;
        Vector3 startCellPos = new Vector3(
            startCellX * (cellWidth + hallwayLength) - (cellWidth + hallwayLength) * gridWidth/2,
            1,
            startCellY * (cellHeight + hallwayLength) - (cellHeight + hallwayLength) * gridHeight/2);

        Vector3 spawnRoomPos = new Vector3(startCellPos.x, 1,
            startCellPos.z - cellHeight / 2 - hallwayLength - spawnRoomHeight/2);
        GameObject spawnRoom = Instantiate(spawnPrefab, spawnRoomPos, Quaternion.Euler(0,0,0), mapRoot.transform);
        spawnRoom.GetComponent<DungeonRoom>().roomID = -1;
    }
    

    //cells must touch eachother
    // 
    void CarvePassage(Vector2Int frontierCell, Vector2Int activeCell){
        if (frontierCell.x > activeCell.x) { // cell 1 is right of cell 2
            map[frontierCell.y, frontierCell.x].W = true;
            map[activeCell.y, activeCell.x].E = true;
            map[frontierCell.y, frontierCell.x].startDirection = 3;
        }
        if (frontierCell.x < activeCell.x) { // cell 1 is left of cell 2
            map[frontierCell.y, frontierCell.x].E = true;
            map[activeCell.y, activeCell.x].W = true;
            map[frontierCell.y, frontierCell.x].startDirection = 1;
        }
        if (frontierCell.y > activeCell.y) { // cell 1 is above cell 2
            map[frontierCell.y, frontierCell.x].S = true;
            map[activeCell.y, activeCell.x].N = true;
            map[frontierCell.y, frontierCell.x].startDirection = 2;
        }
        if (frontierCell.y < activeCell.y) { // cell 1 is below cell 2
            map[frontierCell.y, frontierCell.x].N = true;
            map[activeCell.y, activeCell.x].S = true;
            map[frontierCell.y, frontierCell.x].startDirection = 0;
        }
        
    }

    bool IsInsideGrid(int x, int y){
        if (x >= 0 && x < gridWidth ) {
            if (y >= 0 && y < gridHeight) {
                return true;
                
            }
        }
        return false;
    }

    List<Vector2Int> GetActiveNeighbours(Vector2Int cell) {
        List<Vector2Int> activeNeighbours = new List<Vector2Int>();
        
        if (IsInsideGrid(cell.x, cell.y + 1) && map[cell.y + 1, cell.x].active) {
            activeNeighbours.Add(new Vector2Int( cell.x, cell.y + 1));
        }
        if ( IsInsideGrid(cell.x + 1, cell.y) && map[cell.y, cell.x + 1].active) {
            activeNeighbours.Add(new Vector2Int( cell.x + 1, cell.y));
        }
        if (IsInsideGrid(cell.x, cell.y - 1) && map[cell.y - 1, cell.x].active) {
            activeNeighbours.Add(new Vector2Int( cell.x,cell.y - 1));
        };
        if ( IsInsideGrid(cell.x - 1, cell.y) && map[cell.y, cell.x - 1].active) {
            activeNeighbours.Add(new Vector2Int(cell.x - 1,cell.y));
        }
        return activeNeighbours;
    }

    void SetNeighboursAsFrontier(Vector2Int cell){
        //above
        if (IsInsideGrid(cell.x, cell.y + 1) && !map[cell.y + 1, cell.x].active && !map[cell.y + 1, cell.x].frontier) {
            map[cell.y + 1, cell.x].frontier = true;
            frontierCells.Add(new Vector2Int(cell.x, cell.y + 1));
        }
        //right
        if (IsInsideGrid(cell.x + 1, cell.y) && !map[cell.y, cell.x + 1].active && !map[cell.y, cell.x + 1].frontier) {
            map[cell.y, cell.x + 1].frontier = true;
            frontierCells.Add(new Vector2Int(cell.x + 1, cell.y));
        }
        //below
        if (IsInsideGrid(cell.x, cell.y - 1) && !map[cell.y - 1, cell.x].active && !map[cell.y - 1, cell.x].frontier) {
            map[cell.y - 1, cell.x].frontier = true;
            frontierCells.Add(new Vector2Int(cell.x, cell.y - 1));
        }
        //left
        if (IsInsideGrid(cell.x - 1, cell.y) && !map[cell.y, cell.x - 1].active && !map[cell.y, cell.x - 1].frontier) {
            map[cell.y , cell.x - 1].frontier = true;
            frontierCells.Add(new Vector2Int(cell.x - 1, cell.y));
        }
    }

    void printMap(){
        string output = "\n";
        for (int y = gridHeight-1; y >= 0; y--) {
            for (int x = 0; x < gridWidth; x++) {
                output += map[y, x].depth;

            }

            output += "\n";
        }
        print(output);
    }
    
    private void GenerateMap(){
        map = new Cell[gridHeight,gridWidth];
        
        InitMap();
        
        frontierCells = new List<Vector2Int>();
        int startX = gridWidth / 2;
        int startY = 0;
        SetCellDirections(startX, startY, false, false, true, false);
        map[startY, startX].active = true;
        map[startY, startX].frontier = false;
        map[startY, startX].depth = 0;
        map[startY, startX].startDirection = 2;


        
        SetNeighboursAsFrontier(new Vector2Int(startX, startY));


        while (frontierCells.Count > 0) {

            

            int randomIndex = Random.Range(0, frontierCells.Count);
            Vector2Int currentCell = frontierCells[randomIndex];
            frontierCells.RemoveAt(randomIndex);




            List<Vector2Int> activeNeighbours = GetActiveNeighbours(currentCell);
            randomIndex = Random.Range(0, activeNeighbours.Count);

            Vector2Int chosenNeighbour = activeNeighbours[randomIndex];
            map[currentCell.y, currentCell.x].active = true;
            CarvePassage(currentCell, chosenNeighbour);
            int newDist = map[chosenNeighbour.y, chosenNeighbour.x].depth + 1;

            if (newDist > furthestDist) {
                furthestDist = newDist;
                furthestCell = new Vector2Int(currentCell.x, currentCell.y);
            }
            
            map[currentCell.y, currentCell.x].depth = newDist;
            
            SetNeighboursAsFrontier(currentCell);
            


        }
        // printMap();
        GenerateModels();
            
    }


    
    // Start is called before the first frame update
    void Start()
    {
        

        prefabDict.Add((true, false, false, false), roomPrefabN);
        prefabDict.Add((false, true, false, false), roomPrefabN);
        prefabDict.Add((false, false, true, false), roomPrefabN);
        prefabDict.Add((false, false, false, true), roomPrefabN);
        
        prefabDict.Add((true, true, false, false), roomPrefabNE);
        prefabDict.Add((false, true, true, false), roomPrefabNE);
        prefabDict.Add((false, false, true, true), roomPrefabNE);
        prefabDict.Add((true, false, false, true), roomPrefabNE);
        
        prefabDict.Add((true, true, true, false), roomPrefabNES);
        prefabDict.Add((false, true, true, true), roomPrefabNES);
        prefabDict.Add((true, false, true, true), roomPrefabNES);
        prefabDict.Add((true, true, false, true), roomPrefabNES);
        
        prefabDict.Add((true, false, true, false), roomPrefabNS);
        prefabDict.Add((false, true, false, true), roomPrefabNS);
        
        prefabDict.Add((true, true, true, true), roomPrefabNESW);
        prefabDict.Add((false, false, false, false), roomPrefabN);
        

        GenerateMap();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
