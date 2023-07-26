using Sparkless.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Sparkless.Core
{
    public class GameplayManager : MonoBehaviour
    {
        #region Start_methods
        #region Start_variables
        public static GameplayManager instance;

        [HideInInspector] public bool hasGameFinished;

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private GameObject _winText;
        [SerializeField] private SpriteRenderer _clickHighlight;

        private void Awake()
        {
            instance = this;

        }
        private void Start()
        {
            hasGameFinished = false;
            _winText.SetActive(false);
            _titleText.gameObject.SetActive(true);
            _titleText.text = GameManager.Instance.StageName + " - " + GameManager.Instance.CurrentLevel.ToString();

            CurrentLevelData = GameManager.Instance.GetLevel();


            SpawnBoard();
            SpawnNodes();
        }
        #endregion

        #region Board_spawn
        [SerializeField] private SpriteRenderer _boardPrefab;
        [SerializeField] private GameObject _bgCellPrefab;
        [SerializeField] private GameObject _bgCellPrefab2;
        private void SpawnBoard()
        {
            int curentLevelSize = GameManager.Instance.CurrentStage + 4;
            var board = Instantiate(_boardPrefab,
                new Vector3(curentLevelSize / 2f, 0f, curentLevelSize / 2f),
                //new Vector3(curentLevelSize/2f,curentLevelSize/2f,0f),
                Quaternion.AngleAxis(90, Vector3.left));

            board.size = new Vector2 (curentLevelSize+0.08f,curentLevelSize+0.08f);
            for(int i = 0; i< curentLevelSize; i++)
            {
                for(int j = 0; j< curentLevelSize; j++)
                {
                    if((i+j)%2 == 0 || i + j == 0)
                    {
                        Instantiate(_bgCellPrefab2, new Vector3(i + 0.5f, 0f, j + 0.5f), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(_bgCellPrefab, new Vector3(i + 0.5f, 0f, j + 0.5f), Quaternion.identity);
                    }

                }
            }
            Camera.main.orthographicSize = curentLevelSize
                +0.2f;
            Camera.main.transform.position = new Vector3 (1f, curentLevelSize+1.5f, curentLevelSize/2f);
            _clickHighlight.size = new Vector2(curentLevelSize/4,curentLevelSize/4);
            _clickHighlight.transform.position = new Vector3(0,0,0);
            _clickHighlight.gameObject.SetActive(false);
        }
        #endregion

        #region Node_spawn
        private LevelData CurrentLevelData;
        [SerializeField] private Node _nodePrefab;
        private List<Node> _nodes;

        public Dictionary<Vector2Int, Node> _nodeGrid;
        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();
            int currentLevelSize = GameManager.Instance.CurrentStage + 4;
            Node spawnedNode;
            Vector3 spawnPos;
            for(int i = 0; i < currentLevelSize; i++)
            {
                for(int j = 0; j < currentLevelSize; j++)
                {
                    //spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                    spawnPos = new Vector3(i + 0.5f, 0f, j + 0.5f);
                    spawnedNode = Instantiate(_nodePrefab,spawnPos,Quaternion.identity);
                    spawnedNode.Init();

                    int colorIdForSpawnedNode = GetColorId(i,j);

                    if (colorIdForSpawnedNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                    }
                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos3D = new Vector3Int(i, 0, j);
                }
                List<Vector2Int> offsetPos = new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

                foreach(var item in _nodeGrid)
                {
                    foreach (var offset in offsetPos)
                    {
                        var checkPos = item.Key + offset;
                        if (_nodeGrid.ContainsKey(checkPos))
                        {
                            item.Value.SetEdge(offset, _nodeGrid[checkPos]);
                        }
                    }

                }

            }
        }
        public List<Color> NodeColors;

        public int GetColorId(int i, int j)
        {
            List<Edge> edges = CurrentLevelData.Edges;
            Vector2Int point = new Vector2Int(i,j);
            for(int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point || edges[colorId].EndPoint == point)
                {
                    return colorId;
                }
            }
            return -1;
        }

        public Color GetHighLightColor(int colorID)
        {
            Color result = NodeColors[colorID % NodeColors.Count];
            result.a = 0.4f;
            return result;

        }
        #endregion
        #endregion

        #region Update_methods
        private Node startNode;
        public void Update()
        {
            if (hasGameFinished) return;
            if(Input.GetMouseButtonDown(0))
            {
                startNode = null;
                return;
            }
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Node tempNode = hit.collider.gameObject.GetComponent<Node>();
                    if (startNode == null)
                    {
                        if (tempNode != null && tempNode.IsClickable)
                        {
                            //Debug.Log("1" + hit.collider.gameObject.name);
                            startNode = tempNode;
                            _clickHighlight.gameObject.SetActive(true);
                            _clickHighlight.gameObject.transform.position = hit.point;
                            _clickHighlight.color = GetHighLightColor(tempNode.colorId);
                        }
                        return;
                    }
                    _clickHighlight.gameObject.transform.position = hit.point;

                    if (tempNode != null && startNode != tempNode)
                    {
                        if (startNode.colorId != tempNode.colorId && tempNode.IsEndNode)
                        {
                            return;
                        }
                        //Debug.Log("2" +hit.collider.gameObject.name);
                        startNode.UpdateInput(tempNode);
                        CheckWin();
                        startNode = null;
                    }
                    return;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                startNode = null;
                _clickHighlight.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Win_condition
        private void CheckWin()
        {
            Debug.Log("CheckWin 1");
            bool IsWinning = false;
            foreach(var item in _nodes)
            {
                item.SolveHighlight();
            }
            Debug.Log("CheckWin 2");
            foreach (var item in _nodes)
            {
                IsWinning = item.IsWin;
                print("IDEEEE");
                if(!IsWinning)
                {
                    Debug.Log(gameObject.name);
                    return;
                }
            }
            Debug.Log("CheckWin 3");
            GameManager.Instance.UnlockLevel();
            _winText.gameObject.SetActive(true);
            _clickHighlight.gameObject.SetActive(false);

            hasGameFinished = true;
            Debug.Log("Winnnnnn");
        }
        #endregion

        #region Button_functions
        public void ClickedBack()
        {
            GameManager.Instance.GoToMainMenu();
        }
        public void ClickedRestart()
        {
            GameManager.Instance.GoToGameplay();
        }
        public void ClickedNextLevel()
        {
            if (hasGameFinished) return;
            GameManager.Instance.GoToGameplay();
        }
        #endregion

    }
}