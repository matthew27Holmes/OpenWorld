using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;



//this script should just create cell script objects 

public class WorldGeneration : MonoBehaviour {


    struct cellObject
    {
        public Dictionary<string, cellObject> neighbours;
        public List<GameObject> objects;
        public float PostionX;
        public float PostionY;
        public float Width;
        public float Height;
        public bool isLoaded;
    }

    


    List<cellObject> Cells;
    public BoxCollider floor;
    private Vector3 startPos;
    public GameObject SceneParent;
    public GameObject player;
    public int numCell;

    int playersCurenntCell;
    int gridHeghit, gridwidth;
    public int rows,columes;
    int NumberOfObjects;
    int GenerationLayer;
    Vector3 scale; 

    // Use this for initialization
    void Start()
    {
        initialiseCells();  
    }

    void initialiseCells()
    {
        playersCurenntCell = 0;
        GenerationLayer = 9;
        // create cells array
        Cells = new List<cellObject>();
        //define node postions
        CreateGrid();
        //find nodes neighbours
        findNeighbouringNodes();
        // define node children
        findCellsObjects();
    }

    void CreateGrid()
    {
        //initialise grid variables
        gridHeghit = (int)Mathf.Ceil(floor.size.y * 1.169096f);
        gridwidth = (int)Mathf.Ceil(floor.size.x * 1.169097f);
        scale = new Vector3(100, 0, 100);
        startPos = new Vector3(-(gridwidth / 2), 0, -(gridHeghit / 2));

        //draw grid
        for (int z = 0; z < gridHeghit; z = z + 1 * (int)scale.z)
        {
            for (int x = 0; x < gridwidth; x = x + 1 * (int)scale.x)
            {
                

                cellObject cell = new cellObject();
                cell.isLoaded = false;

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(scale.x, 1, scale.z);

                cell.Width = scale.x;
                cell.Height = scale.z;

                float postionX = startPos.x + x;//* cell.Width;
                float postionY = startPos.z + z;//* cell.Height;

                cube.transform.position = new Vector3(postionX, 30, postionY);

                cell.PostionX = postionX;
                cell.PostionY = postionY;

                Cells.Add(cell);
            }
        }
    }

    void findNeighbouringNodes()
    {
       /* rows = (gridHeghit / (int)scale.z)+1;
        columes =  (gridwidth / (int)scale.x)+1;
        for(int i = 0; i < Cells.Count;i++)
        {
            int T, TL, TR, R, BR, B, BL, L;
            cellObject cell = Cells[i];
            T = i-rows;
            TL = i-(rows + 1);
            TR = i-(rows - 1);
            R = i+ 1;
            L = i-1;
            B = i-rows;
            BR = i+(rows + 1);
            BL = i+(rows - 1);
            if (T < rows)
            {      
                cell.neighbours.Add("TOP", Cells[T]);
            }
            if (R < columes)
            {
                cell.neighbours.Add("RIGHT", Cells[R]);
            }
            if(L)
            cell.neighbours.Add("LEFT", Cells[L]);
            cell.neighbours.Add("BOTTOM",Cells[B])

        }*/
    }

    void findCellsObjects()
    {
        //this should be in cell 
        
        // get all objects in the scene that are on a spercfic layer 
        var root = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => go.hideFlags == HideFlags.None).ToArray();

        // run trough all objecs and attach them to a node if there are on the correct layer 
        foreach (GameObject obj in root)//Transform t in SceneParent.transform.GetComponentsInChildren(typeof(GameObject),false))
        {
            if (obj.layer == GenerationLayer)
            {
                Transform t = obj.transform;

                for (int i = 0; i < Cells.Count; i++)
                {
                    cellObject cell = Cells[i];

                    //if objects postion inside of cell then add it cells list 
                    if (cellCollison(cell, t.position.x, t.position.z))
                    {
                        cell.objects.Add(obj);
                        i = Cells.Count;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        numCell = Cells.Count;
        checkPlayersCell(); 
    }

    void checkPlayersCell()
    {
        // get players postion
        //check all cells to find which one the player is currently in 
        for (int i = 0; i < Cells.Count; i++)
        {
            //cellObject cell = Cells[i];
            if(cellCollison(Cells[i], player.transform.position.x,player.transform.position.z))
            {
                if (playersCurenntCell != i)
                {
                    playersCurenntCell = i;
                    LoadNodes();
                }
            }
        }
    }

    bool cellCollison(cellObject cell,float PosX,float PosY)
   {
        if (PosX >= (cell.PostionX + cell.Width))
        {
            if (PosY >= (cell.PostionY + cell.Height))
            {
                return true;
            }
        }
        return false;
    }

    void LoadNodes()
    {
        // find current player node 
        cellObject PlyCell = Cells[playersCurenntCell];

        // load in current players current cell if not done
        if(!PlyCell.isLoaded)
        {
            //load cell
        }
        // load of of nabiour cells if not done 
        for(int i =0; i < PlyCell.neighbours.Count;i++)
        {
            cellObject cell = PlyCell.neighbours[i];
            if(!cell.isLoaded)
            {
                //load in cell 
                cell.isLoaded = true;
            }
        }
    }
}
