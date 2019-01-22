using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
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
    public GameObject cellCube;
    public BoxCollider floor;
    int gridHeghit, gridwidth;
    Vector3 scale;
    private Vector3 startPos;
    public int rows, columes;
    public int numCell;

    public GameObject player;
    int playersCurenntCell;
    int playersLastCell;
    string GenerationLayer;

    List<createXML.Node> NodeContainerRefs;

    GameObject[] enemies;

    void Start()
    {
        //temp
        NodeContainerRefs = new List<createXML.Node>();
        initialiseCells();
        //saveEnemiesToXML();
    }
    #region createNodeGrid
    void initialiseCells()
    {
        playersCurenntCell = 0;
        playersLastCell = 0;
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

                //cell.cellCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cell.cellCube.transform.localScale = new Vector3(scale.x, 1, scale.z);
                cell.cellCube = Instantiate(cellCube);
                cell.Width = scale.x;
                cell.Height = scale.z;

                float postionX = startPos.x + x;//* cell.Width;
                float postionY = startPos.z + z;//* cell.Height;

                cell.cellCube.transform.position = new Vector3(postionX, 30, postionY);

                cell.PostionX = postionX;
                cell.PostionZ = postionY;

               // StartCoroutine(createFloorTile(new Vector3(postionX, 0.0f, postionY)));

                cell.cellCube.SetActive(cell.isLoaded);
                cell.cellID = Cells.Count;
                cell.cellCube.name = cell.cellID.ToString();
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
    
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(checkPlayersCell()); //should check to see if its inside player view port instead
    }

    bool cellCollison(cellObject cell, float px, float pz)
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
                    playersLastCell = playersCurenntCell;
                    playersCurenntCell = i;
                    StartCoroutine(LoadNodes());
                    StartCoroutine(unLoadNodes());
                }
            }
        }
        yield return null;
    }

    #region Load
    // should be run in corouten
    IEnumerator LoadNodes()
    {
        // find current player node 
        cellObject PlyCell = Cells[playersCurenntCell];

        PlyCell = LoadObjects(PlyCell.cellID);
        StartCoroutine(addPlayerNodeColliders(PlyCell.cellID));

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
                if(assetName == "Terrain")
                {
                    Debug.Log("Node " + NodeID);
                }

                if (asset == null)
                {
                    Debug.Log("error in Xml" + NodeID);
                }
                
                GameObject instance = Instantiate(
                    Resources.Load<GameObject>(assetPath)) as GameObject;

                instance.transform.position = asset.postion;
                instance.transform.localScale = asset.Scale;
                instance.transform.eulerAngles = asset.Rotation;

                instance.name = asset.Name;
                cell.objects.Add(instance);
            }

            LoadEnemy(NodeID);
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

    void LoadEnemy(int cellId)
    {
        string path = EnemyXMLHandler.path + cellId.ToString() + ".XML";
        EnemyXMLHandler.EnemiesNode ContainerRef = EnemyXMLHandler.EnemiesNode.Load(path);

        foreach (EnemyXMLHandler.EnemyAsset Enemies in ContainerRef.Enemies)
        {
            // format asset path
            string assetName = Enemies.Name.Split(' ')[0];
            string assetPath = "OpenWorldObjects/" + assetName;


            GameObject Enemey = Instantiate(
                Resources.Load<GameObject>(assetPath)) as GameObject;

            Enemey.transform.position = Enemies.postion;
            Enemey.transform.localScale = Enemies.Scale;
            Enemey.transform.eulerAngles = Enemies.Rotation;
            SkeletonBehaviour AI = Enemey.GetComponent<SkeletonBehaviour>();

            foreach (EnemyXMLHandler.PatrolPoint patrolPoint in Enemies.patrol)
            {
                AI.PatrolRoute.Add(patrolPoint.postion);
            }
            AI.BirthNodeID = cellId;
            Enemey.name = Enemies.Name;
        }
    }
   
    #endregion

    #region unLoad
    IEnumerator unLoadNodes()
    {
        cellObject LastCell = Cells[playersLastCell];
        cellObject PlyCell = Cells[playersCurenntCell];

        StartCoroutine(RemovePlayerNodeColliders(playersLastCell));
        for (int i = 0; i < LastCell.neighbours.Count; i++)
        {
            cellObject cell = LastCell.neighbours[i];
            if ((!isNodeANeighbours(cell))&& (cell.cellID != PlyCell.cellID))
            {
                LastCell.neighbours[i] = UnLoadObjects(cell.cellID);
            }
        }

        yield return null;
    }

    bool isNodeANeighbours(cellObject cell)
    {
        cellObject PlyCell = Cells[playersCurenntCell];

        for (int i = 0; i < PlyCell.neighbours.Count; i++)
        {
            cellObject neighbour = PlyCell.neighbours[i];
            if (cell.cellID == neighbour.cellID)
            {
                return true;
            }
        }
        return false;
    }

    cellObject UnLoadObjects(int NodeID)
    {
        
        cellObject cell = Cells[NodeID];
        //needs to check if neighbours is gonna be loaded wth the next object  
        if (cell.isLoaded)
        {
            cell.isLoaded = false;
            cell.cellCube.SetActive(cell.isLoaded);

            //save to temp here
            //SaveTempNode(cell);
            unLoadEnemey(NodeID);

            foreach (GameObject obj in cell.objects)
            {
                Destroy(obj);//this hsould store to cache for a period
            }
            cell.objects.Clear();
        }
        Cells[NodeID] = cell;
        return cell;
    }
    IEnumerator RemovePlayerNodeColliders(int NodeID)
    {
        cellObject cell = Cells[NodeID];
        foreach (GameObject obj in cell.objects)
        {
            Destroy(obj.GetComponent<MeshCollider>());
        }
        yield return null;
    }

    void unLoadEnemey(int cellID)
    {
        GameObject[] Enemeies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject Enemy in Enemeies)
        {
            SkeletonBehaviour EnemyBehaviour = Enemy.GetComponent<SkeletonBehaviour>();
            if(EnemyBehaviour.NodeID == cellID)
            {
                Destroy(Enemy);
            }
        }
    }
    #endregion

    #region saveEnemyTemp


    //// get all eneimes in the scene 
    //void saveEnemiesToXML()
    //{
    //    EnemyXMLHandler.EnemiesNode node = new EnemyXMLHandler.EnemiesNode();
    //    GameObject[] Enemeies = GameObject.FindGameObjectsWithTag("Enemy");

    //    foreach (GameObject Enemy in Enemeies)
    //    {

    //        Transform EnemyRoute = PatrolRouteParent.GetChild(j);
    //        List<Vector3> patrolPoints = new List<Vector3>();
    //        for (int k = 0; k < EnemyRoute.childCount; k++)
    //        {
    //            Transform point = EnemyRoute.GetChild(k);
    //            patrolPoints.Add(point.position);
    //        }

    //        GameObject Enemy = Spawner.transform.GetChild(1).GetChild(j).gameObject;

    //        node.Enemies.Add(createEnemy(Enemy, patrolPoints));

    //        for (int i = 0; i < Cells.Count; i++)
    //        {
    //            cellObject cell = Cells[i];
    //            //if objects postion inside of cell then add it cells list 
    //            if (cellCollison(cell, Spawner.transform.localPosition.x, Spawner.transform.localPosition.z))
    //            {
    //                SaveEnemyNodes(node, i);
    //            }
    //        }

    //    }

    //}

    //void SaveEnemyNodes(EnemyXMLHandler.EnemiesNode node, int i)
    //{
    //    node.Save(EnemyXMLHandler.path + i.ToString() + ".XML");
    //}

    //EnemyXMLHandler.EnemyAsset createEnemy(GameObject Skeleton, List<Vector3> Patrol)
    //{
    //    EnemyXMLHandler.EnemyAsset enemy = new EnemyXMLHandler.EnemyAsset();
    //    enemy.Name = Skeleton.name;
    //    enemy.postion = Patrol[0];
    //    enemy.Rotation = Skeleton.transform.eulerAngles;
    //    enemy.Scale = Skeleton.transform.localScale;
    //    for (int j = 0; j < Patrol.Count; j++)
    //    {
    //        enemy.patrol.Add(createPatrolPoints(Patrol[j]));
    //    }

    //    return enemy;
    //}

    //EnemyXMLHandler.PatrolPoint createPatrolPoints(Vector3 pos)
    //{
    //    EnemyXMLHandler.PatrolPoint nwpoint = new EnemyXMLHandler.PatrolPoint();
    //    nwpoint.postion = pos;
    //    return nwpoint;
    //}

    //void DeleteTemp()
    //{
    //    foreach (cellObject cell in Cells)
    //    {
    //        File.Delete(EnemyXMLHandler.TempPath + cell.cellID.ToString() + ".XML");
    //    }
    //}

    #endregion
    #endregion
}
