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
        public int cellID;
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
    public GameObject player;
    public int numCell;

    int playersCurenntCell;
    int gridHeghit, gridwidth;
    public int rows,columes;
    int NumberOfObjects;
    string GenerationLayer;
    Vector3 scale;
    public GameObject FloorTile;

    List<createXML.Node> NodeContainerRefs;

    void Start()
    {
        //temp
        NodeContainerRefs = new List<createXML.Node>();
        initialiseCells();  
    }
    #region createNodeGrid
    void initialiseCells()
    {
        playersCurenntCell = 0;
        GenerationLayer = "WorldStreming";
        // create cells array
        Cells = new List<cellObject>();//should be a list of cell script pointers 
        //define node postions
        CreateGrid();
        //find nodes neighbours
        findNeighbouringNodes();
        // define node children for createing the xml
        //findCellsObjects();
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

               // StartCoroutine(createFloorTile(new Vector3(postionX, 0.0f, postionY)));
                ////temp
               // createXML.Node node = new createXML.Node();
               // NodeContainerRefs.Add(node);

                cell.cellCube.SetActive(cell.isLoaded);
                cell.cellID = Cells.Count;
                Cells.Add(cell);
            }
        }
    }
    void findNeighbouringNodes()
    {
        rows = (gridHeghit / (int)scale.z) + 1;
        columes = (gridwidth / (int)scale.x) + 1;
        int currentRow = 0, currentColume = 0;
        for (int i = 0; i < Cells.Count; i++)
        {
            currentColume++;

            cellObject cell = Cells[i];
            int neighbouId = 0;

            ////TOPLEFT
            if (currentColume > 0
                && currentRow < rows
                && i + (columes + 1) <= Cells.Count - 1)
            {
                cell.neighbours.Add(neighbouId, Cells[i + (columes + 1)]);
                neighbouId++;
            }

            //TOP
            if (currentRow < rows
                && i + columes <= Cells.Count - 1)
            {
                cell.neighbours.Add(neighbouId, Cells[i + columes]);
                neighbouId++;
            }
            ////TOPRIGHT
            if (currentColume < columes
                && currentRow < rows
                && i + (rows - 1) <= Cells.Count - 1)
            {
                cell.neighbours.Add(neighbouId, Cells[i + (columes - 1)]);
                neighbouId++;
            }

            //RIGHT
            if (currentColume < columes
                && i + 1 <= Cells.Count - 1)
            {
                cell.neighbours.Add(neighbouId, Cells[i + 1]);
                neighbouId++;
            }

            ////BOTTOMRIGHT
            if (currentColume < columes
                && currentRow > 0
                && i - (columes + 1) >= 0)
            {
                cell.neighbours.Add(neighbouId, Cells[i - (columes - 1)]);
                neighbouId++;
            }

            //BOTTOM
            if (currentRow > 0
                && i - columes >= 0)
            {
                cell.neighbours.Add(neighbouId, Cells[i - columes]);
                neighbouId++;
            }

            ////BOTTOMLEFT
            if (currentColume > 0
                && currentRow > 0
                && i - (columes + 1) >= 0)
            {
                cell.neighbours.Add(neighbouId, Cells[i - (columes + 1)]);
                neighbouId++;
            }

            //LEFT
            if (currentColume > 0
                && i - 1 >= 0)
            {
                cell.neighbours.Add(neighbouId, Cells[i - 1]);
                neighbouId++;
            }

            if (currentColume == columes)
            {
                currentColume = 0;
                currentRow++;
            }
        }
    }
    #endregion
    #region Temp SaveToXML

    IEnumerator createFloorTile(Vector3 Postion)
    {
        Instantiate(FloorTile, Postion, Quaternion.identity);
        yield return null;
    }
  
    void findCellsObjects()//debug for saving to xml
    {
        //this should be in cell 

        // get all objects in the scene that are on a spercfic layer 
        var root = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => go.hideFlags == HideFlags.None).ToArray();

        // run trough all objecs and attach them to a node if there are on the correct layer 
        foreach (GameObject obj in root)//Transform t in SceneParent.transform.GetComponentsInChildren(typeof(GameObject),false))
        {
            if (LayerMask.LayerToName(obj.layer) == GenerationLayer)
            {
                Transform t = obj.transform;

                for (int i = 0; i < Cells.Count; i++)
                {
                    cellObject cell = Cells[i];
                    //if objects postion inside of cell then add it cells list 
                    if (cellCollison(cell, t.localPosition.x, t.localPosition.z))
                    {
                        addToXML(obj, i);
                        cell.objects.Add(obj);
                      //  obj.transform.parent = cell.cellCube.transform;
                        //obj.SetActive(false);
                        Cells[i] = cell;

                        i = Cells.Count;
                    }
                    //cell.cellCube.SetActive(false);
                }
            }
        }
        SaveNodes();
    }

    void SaveNodes()
    {
        for (int i = 0; i < NodeContainerRefs.Count; i++)
        {
            NodeContainerRefs[i].Save(createXML.path + i.ToString() + ".XML");
        }
    }

    //Temp
    void addToXML(GameObject obj, int i)
    {
        Transform t = obj.transform;

        createXML.StreamingAsset asset = new createXML.StreamingAsset();
        asset.Name = obj.name;
        asset.postion = t.localPosition;
        asset.Rotation = t.eulerAngles;
        asset.Scale = t.localScale;
        NodeContainerRefs[i].assets.Add(asset);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //run in coruntine 
        numCell = Cells.Count;
        StartCoroutine(checkPlayersCell()); //should check to see if its inside player view port instead
    }
    #region updateWorld
    IEnumerator checkPlayersCell()
    {
        // get players postion
        //check all cells to find which one the player is currently in 
        for (int i = 0; i < Cells.Count; i++)
        {
            if(cellCollison(Cells[i], player.transform.position.x,player.transform.position.z))
            {
                if (playersCurenntCell != i)
                {
                    int LastPlayerCell = playersCurenntCell;
                    StartCoroutine(unLoadNodes());
                    playersCurenntCell = i;
                    StartCoroutine(LoadNodes());
                }
            }
        }
        yield return null;
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
    #region Load
    // should be run in corouten
    IEnumerator LoadNodes()
    {
        // find current player node 
        cellObject PlyCell = Cells[playersCurenntCell];

        PlyCell = LoadObjects(PlyCell.cellID);

       // StartCoroutine(addPlayerNodeColliders(PlyCell.cellID));

        // load neighbours cells if not done 
        for (int i = 0; i < PlyCell.neighbours.Count; i++)
        {
            cellObject cell = PlyCell.neighbours[i];

            PlyCell.neighbours[i] = LoadObjects(cell.cellID);
        }
        yield return null;
    }

    cellObject LoadObjects(int NodeID)
    {
        cellObject cell = Cells[NodeID];
        if (!cell.isLoaded)
        {
            //load in cell 
            cell.isLoaded = true;
            cell.cellCube.SetActive(cell.isLoaded);

            createXML.Node NodeContainerRef = createXML.Node.Load(
                createXML.path + NodeID.ToString() + ".XML");

            foreach (createXML.StreamingAsset asset in NodeContainerRef.assets)
            {
                // format asset path

                string assetName = asset.Name.Split(' ')[0];
                string assetPath = "OpenWorldObjects/" + assetName;
                
                GameObject instance = Instantiate(
                    Resources.Load<GameObject>(assetPath)) as GameObject;

                instance.transform.position = asset.postion;
                instance.transform.localScale = asset.Scale;
                instance.transform.eulerAngles = asset.Rotation;
               
                instance.name = asset.Name;
                cell.objects.Add(instance);
            }
        }
        Cells[NodeID] = cell;
        return cell;
    }

    IEnumerator addPlayerNodeColliders(int NodeID)
    {
        cellObject cell = Cells[NodeID];
        foreach(GameObject obj in cell.objects)
        {
            obj.AddComponent<MeshCollider>();
        }
        yield return null;
    }
    #endregion
    #region unLoad
    IEnumerator unLoadNodes()
    {
        cellObject PlyCell = Cells[playersCurenntCell];
        
        for (int i = 0; i < PlyCell.neighbours.Count; i++)
        {
            cellObject cell = PlyCell.neighbours[i];
            PlyCell.neighbours[i] = UnLoadObjects(cell.cellID);
        }
        yield return null;
    }

    cellObject UnLoadObjects(int NodeID)
    {
        cellObject cell = Cells[NodeID];

        //needs to check if neighbours is gonna be loaded wth the next object  
        if (cell.isLoaded)
        {
            cell.isLoaded = false;
            cell.cellCube.SetActive(cell.isLoaded);

            foreach (GameObject obj in cell.objects)
            {
                Destroy(obj);//this hsould store to cache for a period
            }
            cell.objects.Clear();     
        }
        Cells[NodeID] = cell;
        return cell;
    }
    #endregion
    #endregion
}
