using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WorldGeneration : MonoBehaviour {
    struct cellObject
    {
        public List<int> neighbours;
        public List<GameObject> objects;
        public float PostionX;
        public float PostionY;
        public float Width;
        public float Height;
    }

    List<cellObject> Cells;
    public BoxCollider floor;
    public Transform topLeft;
    public GameObject SceneParent;
    public GameObject player;

    int playersCurenntCell;
    int gridHeghit, gridwidth;
    int NumberOfObjects;

	// Use this for initialization
	void Start () {
        Cells = new List<cellObject>();
        gridHeghit = (int)Mathf.Ceil(floor.size.y);
        gridwidth = (int)Mathf.Ceil(floor.size.x);
       
        initialiseCells();  
    }
	
    void initialiseCells()
    {
        
        CreateGrid();

        ////runs through objects in scene and find what objects are under what cells
        //for (int k = 0; k < NumberOfObjects; k++)
        //{
        //    for (int i = 0; i < Cells.Count; i++)
        //    {
        //        // if objects postion inside of cell then add it cells list 
        //    }
        //}
    }

    void CreateGrid()
    {
        Vector3 startPos = new Vector3(topLeft.position.x, 0.0f, topLeft.position.z);

        for (int y = 0; y < gridHeghit/20; y++)
        {
            for (int x = 0; x < gridwidth/20; x++)
            {

                // place squares //todo replace with hexs
                // add node to list 
                cellObject cell = new cellObject();

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.localScale = new Vector3(20, 1, 20);

                cell.Width = 10;
                cell.Height = 10;

                float postionX = startPos.x + x * 20;//cell.Width;
                float postionY = startPos.z + y * 20;//cell.Height;

                cube.transform.position = new Vector3(postionX, 20, postionY);
                cell.PostionX = postionX;
                cell.PostionY = postionY;

                Cells.Add(cell);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkPlayersCell();
        LoadNodes();
    }

    void checkPlayersCell()
    {
        // get players postion
        //check all cells to find which one the player is currently in 
        for (int i = 0; i < Cells.Count; i++)
        {
            cellObject cell = Cells[i];
            if(cellCollison(cell,player.transform.position.x,player.transform.position.z))
            {
                if (playersCurenntCell != i)
                {
                    // update loaded cells 
                }
            }
        }
    }

    bool cellCollison(cellObject cell,float PosX,float PosY)
   {
        if (cell.PostionX >= (PosX + cell.Width))
        {
            if (cell.PostionY >= (PosY + cell.Height))
            {
                return true;
            }
        }
        return false;
    }

    void LoadNodes()
    {

    }
}
