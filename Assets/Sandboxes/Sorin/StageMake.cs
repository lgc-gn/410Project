
    using NUnit.Framework;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BoardSpawner : MonoBehaviour
    {
        // First, we need an object we want to instantiate
        public GameObject cube1;
        public GameObject cube2;
        // Later, we will expand that object into instead being a list of objects that we randomly pull from
        public List<GameObject> cubeList = new List<GameObject>();
        public List<List<GameObject>> board = new List<List<GameObject>>();
        public int boardWidth = 5;
        public int boardLength = 5;
        // First we need a way to spawn a single instance of that object
        // Second, we need a way to spawn a second instance of that object
        // Last, we need to do so in an ordered, procedural manner

        private void Start()
        {

            for (int i = 0; i < boardWidth; i++)
            {
                board.Add(new List<GameObject>());

                for (int j = 0; j < boardLength; j++)
                {
                    // TODO: Change to random cube from cubeList
                    board[i].Add(Instantiate(cubeList[Random.Range(0, cubeList.Count)]));

                    for (int k = 0; k < i; k++)
                    {
                        board[i][j].transform.position += new Vector3(0, 0, 1.2f);
                    }
                    for (int k = 0; k < j; k++)
                    {
                        board[i][j].transform.position += new Vector3(1.2f, 0, 0);
                    }

                   
                }
            }

            int lastHeight = 0;
            int newHeight = 0;

            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0;j < boardLength; j++)
                {
                    if (Random.Range(0.0f, 1.0f) < 0.2f)
                    {
                        board[i][j].transform.position += new Vector3(0, 1f, 0);
                    }
                }
            }

            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardLength; j++)
                {
                    if (board[i][j].transform.position.y == 1f)
                    {
                        if (i > 0) { board[i - 1][j].transform.position += new Vector3(0,0.5f,0); }
                        if (i < boardWidth - 1) { board[i + 1][j].transform.position += new Vector3(0, 0.5f, 0); }
                        if (j > 0) { board[i][j - 1].transform.position += new Vector3(0, 0.5f, 0); }
                        if (j < boardLength - 1) { board[i][j + 1].transform.position += new Vector3(0, 0.5f, 0); }
                    }

                }
            }
            /*
            GameObject firstCube = Instantiate(cubeList[0]);
            GameObject secondCube = Instantiate(cubeList[1]);
            secondCube.transform.position += new Vector3(0.25f, 0, 0);
            */
        }
    }