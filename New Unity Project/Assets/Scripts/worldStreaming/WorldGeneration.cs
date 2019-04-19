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
    public Vector3 scale;
    private Vector3 startPos;
    public int rows, columes;
    public int numCell;

    public GameObject player;
    int playersCurenntCell;
    int playersLastCell;
   // string GenerationLayer;

    //List<createXML.Node> NodeContainerRefs;

    public List<GameObject> ActiveEnemies;

    void Start()
    {
        //temp
        //NodeContainerRefs = new List<createXML.Node>();
        initialiseCells();
        createEnemyTempFiles();
    }

    #region createNodeGrid
    void initialiseCells()
    {
        playersCurenntCell = 0;
        playersLastCell = 0;
       // GenerationLayer = "WorldStreming";
        // create cells array
        Cells = new List<cellObject>();//should be a list of cell script pointers 
        ActiveEnemies = new List<GameObject>();
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

                cell.cellCube.SetActive(cell.isLoaded);//used to debug which chuncks the system is loading in
                cell.cellCube.GetComponent<MeshRenderer>().enabled = false;
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

    private void OnApplicationQuit()
    {
        DeleteTemp();
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

    public GameObject getCellCube(int cellId)
    {
        return Cells[cellId].cellCube;
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

    public bool isCellLoaded(int cellId)
    {
        return Cells[cellId].isLoaded;
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
        foreach (GameObject obj in cell.objects)
        {
            obj.AddComponent<MeshCollider>();
        }
        yield return null;
    }

    void LoadEnemy(int cellId)
    {
        string path = EnemyXMLHandler.TempPath + cellId.ToString() + ".XML";
        EnemyXMLHandler.EnemiesNode ContainerRef = EnemyXMLHandler.EnemiesNode.Load(path);

        foreach (EnemyXMLHandler.EnemyAsset EnemeyData in ContainerRef.Enemies)
        {
            // format asset path
            string assetName = EnemeyData.Name.Split(' ')[0];
            string assetPath = "OpenWorldObjects/" + assetName;
            string enemyHash = cellId.ToString() + assetName + EnemeyData.postion.ToString();

            if (checkIfEnemyLoaded(enemyHash))
            {
                GameObject Enemey = Instantiate(
                  Resources.Load<GameObject>(assetPath)) as GameObject;

                Enemey.transform.position = EnemeyData.postion;
                Enemey.transform.localScale = EnemeyData.Scale;
                Enemey.transform.eulerAngles = EnemeyData.Rotation;
                SkeletonBehaviour AI = Enemey.GetComponent<SkeletonBehaviour>();
                AI.HashID = enemyHash;

                if (EnemeyData.health > 0)//account for health being 0 at the being
                {
                    AI.health = EnemeyData.health;
                }

                foreach (EnemyXMLHandler.PatrolPoint patrolPoint in EnemeyData.patrol)
                {
                    AI.PatrolRoute.Add(patrolPoint.postion);
                }
                AI.BirthNodeID = FindEnemyBirthNode(AI.PatrolRoute[0]);
                Enemey.name = EnemeyData.Name;

                ActiveEnemies.Add(Enemey);
            }
        }
    }

    bool checkIfEnemyLoaded(string HashId)
    {
        //GameObject[] Enemeies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject Enemy in ActiveEnemies)
        {
            SkeletonBehaviour skeleton = Enemy.GetComponent<SkeletonBehaviour>();
            if(skeleton.HashID == HashId)
            {
                return false;
            }
        }
            return true;
    }

    int FindEnemyBirthNode(Vector3 firstPatrolPoint)
    {
        for (int i = 0; i < Cells.Count; i++)
        {
            cellObject cell = Cells[i];
            //if objects postion inside of cell then add it cells list 
            if (cellCollison(cell, firstPatrolPoint.x, firstPatrolPoint.z))
            {
                return i;
            }
        }
        return 100;// debug should ever be hit 
    }

    #endregion

    #region unLoad
    IEnumerator unLoadNodes()
    {
        cellObject LastCell = Cells[playersLastCell];
        cellObject PlyCell = Cells[playersCurenntCell];

        StartCoroutine(RemovePlayerNodeColliders(playersLastCell));
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < LastCell.neighbours.Count; i++)
        {
            cellObject cell = LastCell.neighbours[i];
            if ((!isNodeANeighbours(cell)) && (cell.cellID != PlyCell.cellID))
            {
                LastCell.neighbours[i] = UnLoadObjects(cell.cellID);
            }
        }
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
        //   GameObject[] Enemeies = GameObject.FindGameObjectsWithTag("Enemy");

        List<GameObject> Temp = new List<GameObject>(ActiveEnemies);
        foreach (GameObject Enemy in Temp)
        {
            SkeletonBehaviour EnemyBehaviour = Enemy.GetComponent<SkeletonBehaviour>();
            if(EnemyBehaviour.NodeID == cellID)
            {
                saveEnemiesToXML(Temp);//update temp files//not the most comuputinional efficent way to do it 
                ActiveEnemies.Remove(Enemy);// changing list in corroutine  it dosnt like it 
                Destroy(Enemy);
            }
        }
    }
    #endregion

    #region EnemyTemp

    void createEnemyTempFiles()
    {
        List<EnemyXMLHandler.EnemiesNode> NodeList = new List<EnemyXMLHandler.EnemiesNode>();
        for (int i = 0; i < Cells.Count; i++)
        {
            string path = EnemyXMLHandler.path + i.ToString() + ".XML";
            NodeList.Add(EnemyXMLHandler.EnemiesNode.Load(path));
        }
        SaveEnemyNodes(NodeList);
    }

    //// get all eneimes in the scene 
    void saveEnemiesToXML(List<GameObject>Enemies)
    {
        List<EnemyXMLHandler.EnemiesNode> NodeList = new List<EnemyXMLHandler.EnemiesNode>();
        for (int i = 0; i < Cells.Count; i++)
        {
            NodeList.Add(new EnemyXMLHandler.EnemiesNode());
        }

        //GameObject[] Enemeies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject Enemy in Enemies)
        {
            SkeletonBehaviour skeletonBehaviour = Enemy.GetComponent<SkeletonBehaviour>();

            List<Vector3> patrolPoints = skeletonBehaviour.PatrolRoute;

            NodeList[skeletonBehaviour.NodeID].Enemies.Add(createEnemy(skeletonBehaviour, patrolPoints));
        }
        SaveEnemyNodes(NodeList);
    }

    EnemyXMLHandler.EnemyAsset createEnemy(SkeletonBehaviour Skeleton, List<Vector3> Patrol)
    {
        EnemyXMLHandler.EnemyAsset enemy = new EnemyXMLHandler.EnemyAsset();
        enemy.Name = Skeleton.name;
        //enemy.hashCode = Skeleton.HashID;
        enemy.health = Skeleton.health;
        enemy.postion = Skeleton.transform.position;//Patrol[0];
        enemy.Rotation = Skeleton.transform.eulerAngles;
        enemy.Scale = Skeleton.transform.localScale;
        for (int j = 0; j < Patrol.Count; j++)
        {
            enemy.patrol.Add(createPatrolPoints(Patrol[j]));
        }

        return enemy;
    }

    EnemyXMLHandler.PatrolPoint createPatrolPoints(Vector3 pos)
    {
        EnemyXMLHandler.PatrolPoint nwpoint = new EnemyXMLHandler.PatrolPoint();
        nwpoint.postion = pos;
        return nwpoint;
    }

    void SaveEnemyNodes(List<EnemyXMLHandler.EnemiesNode> NodeList)
    {

        for (int i = 0; i < NodeList.Count; i++)
        {
            EnemyXMLHandler.EnemiesNode node = NodeList[i];
            node.Save(EnemyXMLHandler.TempPath + i.ToString() + ".XML");
        }
    }

    void DeleteTemp()
    {
        for (int i = 0; i < Cells.Count; i++)
        {
            File.Delete(EnemyXMLHandler.TempPath + i.ToString() + ".XML");
            File.Delete(EnemyXMLHandler.TempPath + i.ToString() + ".XML.meta");
        }
    }

    #endregion

    #endregion
}
