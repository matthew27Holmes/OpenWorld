using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;



//this script should just create cell script objects 

public class WorldGeneration : MonoBehaviour {


    struct cellObject
    {
        //neighbours ints in clockwise postions 
        public Dictionary<int, cellObject> neighbours;
        public List<GameObject> objects;
        public GameObject cellCube; //for debug purpose
        public float PostionX;
        public float PostionZ;
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
        Cells = new List<cellObject>();//should be a list of cell script pointers 
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
                cell.objects = new List<GameObject>();
                cell.neighbours = new Dictionary<int, cellObject>();

                cell.cellCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.cellCube.transform.localScale = new Vector3(scale.x, 1, scale.z);

                cell.Width = scale.x;
                cell.Height = scale.z;

                float postionX = startPos.x + x;//* cell.Width;
                float postionY = startPos.z + z;//* cell.Height;

                cell.cellCube.transform.position = new Vector3(postionX, 30, postionY);

                cell.PostionX = postionX;
                cell.PostionZ = postionY;

                Cells.Add(cell);
            }
        }
    }

    void findNeighbouringNodes()
    {
       
        rows = (gridHeghit / (int)scale.z)+1;
        columes =  (gridwidth / (int)scale.x)+1;
        int currentRow = 0, currentColume = 0;
        for (int i = 0; i < Cells.Count; i++)
        {
            currentColume++;

            cellObject cell = Cells[i];
            ////TOPLEFT
            if (currentColume - 1 >= 0 
                && currentRow + 1 <= rows
                && i + (rows + 1) <= Cells.Count - 1)
            {
                cell.neighbours.Add(0, Cells[i + (rows + 1)]);
            }

            //TOP
            if (currentRow + 1 <= rows 
                && i + columes <= Cells.Count-1)
            {
                cell.neighbours.Add(1, Cells[i + columes]);
            }
            ////TOPRIGHT
            if (currentColume + 1 <= columes 
                && currentRow + 1 <= rows
                && i + (rows - 1) <= Cells.Count - 1)
            {
                cell.neighbours.Add(2, Cells[i + (rows - 1)]);
            }

            //RIGHT
            if (currentColume + 1 <= columes 
                && i + 1 <= Cells.Count-1)
            {
                cell.neighbours.Add(3, Cells[i + 1]);
            }

            ////BOTTOMRIGHT
            if (currentColume + 1 <= columes 
                && currentRow - 1 >= 0
                && i - (rows + 1) >= 0)
            {
                cell.neighbours.Add(4, Cells[i - (rows - 1)]);
            }

            //BOTTOM
            if (currentRow - 1 >= 0 
                && i - columes >= 0)
            {
                cell.neighbours.Add(5, Cells[i - columes]);
            }

            ////BOTTOMLEFT
            if (currentColume - 1 >= 0 
                && currentRow - 1 >= 0
                && i - (rows + 1) >= 0)
            {
                cell.neighbours.Add(6, Cells[i - (rows + 1)]);
            }

            //LEFT
            if (currentColume - 1 >= 0 
                && i - 1 >= 0)
            {
                cell.neighbours.Add(7, Cells[i - 1]);
            }
           
            if (currentColume >= columes)
            {
                currentColume = 0;
                currentRow++;
            }
        }
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
                    if (cellCollison(cell, t.localPosition.x, t.localPosition.z))
                    {
                        cell.objects.Add(obj);
                        obj.transform.parent = cell.cellCube.transform;
                        obj.SetActive(false);
                        Cells[i] = cell;
                        i = Cells.Count;
                    }
                    cell.cellCube.SetActive(false);
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

    bool cellCollison(cellObject cell,float px,float pz)
   {
        if (px >= cell.PostionX - (cell.Width / 2)
            && px <= cell.PostionX + (cell.Width / 2)
            && pz >= cell.PostionZ - (cell.Height / 2)
            && pz <= cell.PostionZ + (cell.Height) / 2)
        {
            return true;
        }
        
        return false;
    }

    // should be run in corouten
    void LoadNodes()
    {
        // find current player node 
        cellObject PlyCell = Cells[playersCurenntCell];

        // load in current players current cell if not done
        if(!PlyCell.isLoaded)
        {
            //load cell
           // loading objects should be its own function
            foreach(GameObject obj in PlyCell.objects)
            {
                PlyCell.cellCube.SetActive(true);
                obj.SetActive(true);
            }
        }
        // load nabiour cells if not done 
        for (int i = 0; i < PlyCell.neighbours.Count+1; i++)
        {
            if (PlyCell.neighbours.ContainsKey(i))
            {
                cellObject cell = PlyCell.neighbours[i];
                if (!cell.isLoaded)
                {
                    //load in cell 
                    cell.isLoaded = true;
                    cell.cellCube.SetActive(true);
                    foreach (GameObject obj in cell.objects)
                    {
                        obj.SetActive(true);
                    }
                    PlyCell.neighbours[i] = cell;
                }
            }
        }
    }
}
