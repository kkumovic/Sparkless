using Sparkless.Common;
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
                    Instantiate(_bgCellPrefab, new Vector3(i+0.5f,0.1f, j + 0.5f), Quaternion.identity);
                }
            }
            Camera.main.orthographicSize = curentLevelSize
                +0.5f;
            Camera.main.transform.position = new Vector3 (curentLevelSize/2f, curentLevelSize, 0f);
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

                    if(colorIdForSpawnedNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                        _nodes.Add(spawnedNode);
                        _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                        spawnedNode.gameObject.name = i.ToString() + j.ToString();
                        spawnedNode.Pos2D = new Vector2Int(i, j);
                    }
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
            Color result = NodeColors[colorID];
            result.a = 0.4f;
            return result;

        }
        #endregion
        #endregion

        #region Update_methods

        #endregion

        #region Win_condition

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