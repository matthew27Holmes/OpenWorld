using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using System.Linq;



//this script should just create cell script objects 

public class WorldGeneration : MonoBehaviour
{

    struct cellObject
    {
        public int cellID;
        public GameObject[] objects;
        public GameObject cellCube; //for debug purpose
        public float PostionX;
        public float PostionZ;
        public float Width;
        public float Height;
        public bool isLoaded;
    }

    cellObject[,] map;
    public GameObject cellCube;
    public BoxCollider floor;

    int gridHeghit, gridwidth;

    public GameObject player;
    Vector2 playerCord = new Vector2(0, 0);
    Vector2 playerLastCord = new Vector2(0, 0);

    public List<GameObject> ActiveEnemies;
    int LoadUnloadBatchSize = 8;
    void Start()
    {
        initialise();
    }

    #region initialise
    void initialise()
    {
        // create cells array
        ActiveEnemies = new List<GameObject>();
        //define node postions
        CreateGrid();
        // create the temp files for the enemies
        createEnemyTempFiles();
    }

    void CreateGrid()
    {
        //initialise grid variables
        Vector3 scale = new Vector3(100, 0, 100);
        gridHeghit = (int)Mathf.Ceil((floor.size.y * 1.16f) / scale.z);//1.16 is a fudge number to get it to create a grid just larger then the map 
        gridwidth = (int)Mathf.Ceil((floor.size.x * 1.16f) / scale.x);
        // get offset for map size 
        Vector3 startPos = new Vector3(-205, 0, -310);//floor.gameObject.transform.position.x - ((floor.size.x / 2)), 0, floor.gameObject.transform.position.z - ((floor.size.y / 2)));
        map = new cellObject[gridwidth, gridHeghit];
        int Id = 0;
        //draw grid
        for (int z = 0; z < gridHeghit; z++)
        {
            for (int x = 0; x < gridwidth; x++)
            {
                cellObject cell = new cellObject();
                cell.isLoaded = false;

                cell.cellCube = Instantiate(cellCube); // used for AI random point finding and debugging
                cell.Width = scale.x;
                cell.Height = scale.z;

                float postionX = startPos.x + x * cell.Width;
                float postionY = startPos.z + z * cell.Height;

                cell.cellCube.transform.position = new Vector3(postionX, 30, postionY);

                cell.PostionX = postionX;
                cell.PostionZ = postionY;

                cell.cellCube.SetActive(cell.isLoaded);
                cell.objects = new GameObject[0];
                cell.cellCube.GetComponent<MeshRenderer>().enabled = false;
                cell.cellID = Id;
                cell.cellCube.name = cell.cellID.ToString();
                map[x, z] = cell;
                Id++;
            }
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
       checkPlayersCell(); //should check to see if its inside player view port instead
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

    public Vector2 getActiveCellWidthHeight(int cellId)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((int)playerCord.y + i <= gridHeghit - 1
                    && (int)playerCord.x + j <= gridwidth - 1)
                {
                    if ((int)playerCord.y + i >= 0 && (int)playerCord.x + j >= 0)
                    {
                        if (map[(int)playerCord.x + j, (int)playerCord.y + i].cellID == cellId)
                        {
                            return new Vector2(map[(int)playerCord.x + j, (int)playerCord.y + i].Width, map[(int)playerCord.x + j, (int)playerCord.y + i].Height);
                        }
                    }
                }
            }
        }
        return new Vector2();// this should never be hit
    }

    #region updateWorld

    private void checkPlayersCell()
    {
        // get players postion
        //check all cells to find which one the player is currently in 
        for (int z = 0; z < gridHeghit; z++)
        {
            for (int x = 0; x < gridwidth; x++)
            {
                if (cellCollison(map[x, z], player.transform.position.x, player.transform.position.z))
                {
                    if (playerCord != new Vector2(x, z))
                    {
                        playerLastCord = playerCord;
                        playerCord = new Vector2(x, z);
                        LoadNodes();
                        unLoadNodes();
                    }
                }
            }
        }
    }

    #region Load
    // should be run in corouten
    void LoadNodes()
    {
        // load neighbours cells if not done 
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((int)playerCord.y + i <= gridHeghit - 1
                    && (int)playerCord.x + j <= gridwidth - 1)
                {
                    if ((int)playerCord.y + i >= 0 && (int)playerCord.x + j >= 0)
                    {
                       StartCoroutine(LoadObjects((int)playerCord.x + j, (int)playerCord.y + i));
                    }
                }
            }
        }
    }

    IEnumerator LoadObjects(int x, int z)
    {
        if (!map[x, z].isLoaded)
        {
            //load in cell 
            map[x, z].isLoaded = true;
            map[x, z].cellCube.SetActive(map[x, z].isLoaded);
            int loadingSpilt = 0;
            createXML.Node NodeContainerRef = createXML.Node.Load(
                createXML.path + map[x, z].cellID.ToString() + ".XML");

            for (int k = 0; k < map[x, z].objects.Length; k++)
            {
                if (map[x, z].objects[k] != null)
                {
                    Debug.Log("not all objects unloaded");
                }
            }
            map[x, z].objects = new GameObject[NodeContainerRef.assets.Count];

            for (int i = 0; i < map[x, z].objects.Length; i++)
            {           
                // format asset path
                createXML.StreamingAsset asset = NodeContainerRef.assets[i];
                string assetName = asset.Name.Split(' ')[0];
                string assetPath = "OpenWorldObjects/" + assetName;
                if (assetName == "Terrain")
                {
                    Debug.Log("Node " + map[x, z].cellID);
                }

                if (asset == null)
                {
                    Debug.Log("error in Xml" + map[x, z].cellID);
                }

                GameObject instance = Instantiate(
                    Resources.Load<GameObject>(assetPath)) as GameObject;

                instance.transform.position = asset.postion;
                instance.transform.localScale = asset.Scale;
                instance.transform.eulerAngles = asset.Rotation;

                instance.name = asset.Name;
                map[x, z].objects[i] = instance;
                loadingSpilt++;

                if (loadingSpilt == NodeContainerRef.assets.Count / LoadUnloadBatchSize)
                {
                    loadingSpilt = 0;
                    yield return new WaitForSecondsRealtime(0.05f);
                }
            }

            LoadEnemy(x,z);
        }
    }

    public bool isCellLoaded(int x, int y)
    {
        return map[x, y].isLoaded;
    }


    void LoadEnemy(int x,int y)
    {
        string path = EnemyXMLHandler.TempPath + map[x,y].cellID.ToString() + ".XML";
        EnemyXMLHandler.EnemiesNode ContainerRef = EnemyXMLHandler.EnemiesNode.Load(path);

        foreach (EnemyXMLHandler.EnemyAsset EnemeyData in ContainerRef.Enemies)
        {
            // format asset path
            string assetName = EnemeyData.Name.Split(' ')[0];
            string assetPath = "OpenWorldObjects/" + assetName;
            string enemyHash = map[x, y].cellID.ToString() + assetName + EnemeyData.postion.ToString();// this isnt gonna work have to store in XML

            if (!isEnemyLoaded(enemyHash))
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
                if (EnemeyData.BirthNode == Vector2.zero)//means enemy cant be placed in square 0,0 but that square is not with in the play space any way 
                {
                    AI.BirthNodeID = FindEnemyBirthNode(AI.PatrolRoute[0]);
                }
                Enemey.name = EnemeyData.Name;

                ActiveEnemies.Add(Enemey);
            }
        }
    }

    bool isEnemyLoaded(string HashId)
    {
        foreach (GameObject Enemy in ActiveEnemies)
        {
            SkeletonBehaviour skeleton = Enemy.GetComponent<SkeletonBehaviour>();
            if (skeleton.HashID == HashId)
            {
                return true;
            }
        }
        return false;
    }

    Vector2 FindEnemyBirthNode(Vector3 firstPatrolPoint)
    {
        for (int y = 0; y < gridHeghit; y++)
        {
            for (int x = 0; x < gridwidth; x++)
            {
                //if objects postion inside of cell then add it cells list 
                if (cellCollison(map[x,y], firstPatrolPoint.x, firstPatrolPoint.z))
                {
                    return new Vector2(x,y);
                }
            }
        }
        return new Vector2(gridwidth+1, gridHeghit+1); ;// debug should ever be hit 
    }

    #endregion

    #region unLoad
    void unLoadNodes()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((int)playerLastCord.y + i <= gridHeghit - 1
                    && (int)playerLastCord.x + j <= gridwidth - 1)
                {
                    if ((int)playerLastCord.y + i >= 0 && (int)playerLastCord.x + j >= 0)
                    {
                        //check isnt current neghbiour 
                        if (!isNodePlyNeighbour((int)playerLastCord.x + j, (int)playerLastCord.y + i))
                        {
                            StartCoroutine(UnLoadObjects((int)playerLastCord.x + j, (int)playerLastCord.y + i));
                        }
                    }
                }
            }
        }
    }

    bool isNodePlyNeighbour(int x, int y)
    {
        cellObject PlyCell = map[x, y];

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((int)playerCord.y + i <= gridHeghit - 1
                    && (int)playerCord.x + j <= gridwidth - 1)
                {
                    if ((int)playerCord.y + i >= 0 && (int)playerCord.x + j >= 0)
                    {
                        if (map[(int)playerCord.x + j, (int)playerCord.y + i].cellID == map[x, y].cellID)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    IEnumerator UnLoadObjects(int x,int y)
    {
        if (map[x, y].isLoaded)
        {
            map[x, y].isLoaded = false;
            map[x, y].cellCube.SetActive(map[x, y].isLoaded);
            int unloadingSpilt = 0;
            //save to temp here
            unLoadEnemey(x,y);
            for (int i = 0; i < map[x, y].objects.Length; i++)
            {
                GameObject obj = map[x, y].objects[i];
                Destroy(obj);
                map[x, y].objects[i] = null; 
                unloadingSpilt++;
                if (unloadingSpilt == map[x, y].objects.Length / LoadUnloadBatchSize)
                {
                    unloadingSpilt = 0;
                    yield return new WaitForSecondsRealtime(0.05f);
                }
            }
            //map[x, y].objects = new GameObject[0];
        }
    }

    void unLoadEnemey(int x, int y)
    {
        GameObject [] Temp = ActiveEnemies.ToArray();
        List<GameObject> SavingArray = new List<GameObject>();
        foreach (GameObject Enemy in Temp)
        {
            SkeletonBehaviour EnemyBehaviour = Enemy.GetComponent<SkeletonBehaviour>();
            if (EnemyBehaviour.NodeID == map[x, y].cellID)
            {
                SavingArray.Add(Enemy);
                ActiveEnemies.Remove(Enemy);// changing list in corroutine  it dosnt like it 
                Destroy(Enemy);
            }
        }
        if (SavingArray.Count > 0)
        {
            saveEnemiesToXML(SavingArray.ToArray());//update temp files//not the most comuputinional efficent way to do it 
        }
    }
    #endregion

    #region create and update Enemy temps

    void createEnemyTempFiles()
    {
      EnemyXMLHandler.EnemiesNode [] NodeList  = new EnemyXMLHandler.EnemiesNode[gridwidth* gridHeghit];
        for (int z = 0; z < gridHeghit; z++)
        {
            for (int x = 0; x < gridwidth; x++)
            {
                string path = EnemyXMLHandler.path + map[x,z].cellID.ToString() + ".XML";
                NodeList[map[x, z].cellID] = EnemyXMLHandler.EnemiesNode.Load(path);
                NodeList[map[x, z].cellID].Save(EnemyXMLHandler.TempPath + map[x, z].cellID.ToString() + ".XML");
            }
        }
    }

    // get all eneimes in the scene 
    void saveEnemiesToXML(GameObject[] Enemies)
    {
        EnemyXMLHandler.EnemiesNode[] NodeList = new EnemyXMLHandler.EnemiesNode[gridwidth * gridHeghit];
        for (int i = 0; i < NodeList.Length; i++)
        {
            NodeList[i] = (new EnemyXMLHandler.EnemiesNode());
        }

        foreach (GameObject Enemy in Enemies)
        {
            SkeletonBehaviour skeletonBehaviour = Enemy.GetComponent<SkeletonBehaviour>();
            List<Vector3> patrolPoints = skeletonBehaviour.PatrolRoute;

            NodeList[skeletonBehaviour.NodeID].Enemies.Add(createEnemy(skeletonBehaviour, patrolPoints));
            NodeList[skeletonBehaviour.NodeID].Save(EnemyXMLHandler.TempPath + skeletonBehaviour.NodeID.ToString() + ".XML");// need to get eneimes from this first and then save
        }
    }

    EnemyXMLHandler.EnemyAsset createEnemy(SkeletonBehaviour Skeleton, List<Vector3> Patrol)
    {
        EnemyXMLHandler.EnemyAsset enemy = new EnemyXMLHandler.EnemyAsset();
        enemy.Name = Skeleton.name;
        enemy.health = Skeleton.health;
        enemy.BirthNode = Skeleton.BirthNodeID;
        enemy.postion = Skeleton.transform.position;
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

    void DeleteTemp()
    {
        for (int z = 0; z < gridHeghit; z++)
        {
            for (int x = 0; x < gridwidth; x++)
            {
                File.Delete(EnemyXMLHandler.TempPath + map[x, z].cellID.ToString() + ".XML");
                File.Delete(EnemyXMLHandler.TempPath + map[x, z].cellID.ToString() + ".XML.meta");
            }
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        DeleteTemp();
    }

    #endregion
}