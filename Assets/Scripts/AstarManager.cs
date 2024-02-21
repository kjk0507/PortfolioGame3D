using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}

//벽을 레이어 wall로 설정하면 검색할수있다.
public class AstarManager : MonoBehaviour
{
    public Transform trTarget;
    public Transform trPos;

    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<Node> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;

    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

    public int m_blackTileMax = 10;
    public int m_blackTileCur = 0;
    public GameObject prefabTile;
    public List<GameObject> prefabList;
    public GameObject miniGameConsole;

    // 버튼
    public TextMeshProUGUI m_answer1;
    public TextMeshProUGUI m_answer2;
    public TextMeshProUGUI m_answer3;
    public int m_answerNum1;
    public int m_answerNum2;
    public int m_answerNum3;
    List<int> m_answerList;
    int m_wayNum;

    // 작동 관련
    public GameObject m_player;

    public int SizeX { get { return sizeX; } }
    public int SizeY { get { return sizeY; } }


    public void Init()
    {
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        //sizeX = topRight.x - bottomLeft.x;
        //sizeY = topRight.y - bottomLeft.y;
    }

    public void PathFinding()
    {
        // 초기화
        foreach (GameObject prefab in prefabList)
        {
            Destroy(prefab);
        }

        prefabList = new List<GameObject>();
        m_blackTileCur = 0;
        m_answerList = new List<int>();
        m_wayNum = 0;

        // NodeArray의 크기 정해주고, isWall, x, y 대입
        //sizeX = topRight.x - bottomLeft.x + 1;
        //sizeY = topRight.y - bottomLeft.y + 1;
        sizeX = (topRight.x / 50 - bottomLeft.x) ;
        sizeY = (topRight.y / 50 - bottomLeft.y);
        NodeArray = new Node[sizeX, sizeY];

        Debug.Log("sizeX : " + sizeX);
        Debug.Log("sizeY : " + sizeY);

        startPos.x = (int)trPos.localPosition.x;
        startPos.y = (int)trPos.localPosition.y;
        //startPos.x = 700 / 50;
        //startPos.y = 50 / 50;

        //Debug.Log("startX : " + startPos.x);
        //Debug.Log("startY : " + startPos.y);

        //targetPos.x = (int)trTarget.localPosition.x;
        //targetPos.y = (int)trTarget.localPosition.y;
        targetPos.x = 50 / 50;
        targetPos.y = 250 / 50;

        //Debug.Log("targetX : " + targetPos.x);
        //Debug.Log("targetY : " + targetPos.y);

        //StartNode = NodeArray[13, 1];
        //TargetNode = NodeArray[1, 4];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                //Vector3 vColPos = new Vector3(i + bottomLeft.x, j + bottomLeft.y);
                //float fRad = 0.4f;
                //foreach (Collider col in Physics.OverlapSphere(vColPos, fRad))
                //    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                
                if(m_blackTileCur < m_blackTileMax)
                {
                    if(!((i == 13 && j == 1) || (i == 1 && j == 4) || j == 6))
                    {
                        System.Random random = new System.Random();
                        int randomNum = random.Next(6);
                        if(randomNum == 0)
                        {
                            isWall = true;
                            m_blackTileCur++;
                            GameObject objTile = Instantiate(prefabTile, this.transform);
                            objTile.transform.localPosition = new Vector3(50 * i - 365, 50 * j - 120, 0);
                            prefabList.Add(objTile);
                        }
                    }
                }
                

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        //Debug.Log("count : " + (Mathf.Abs(startPos.x) - Mathf.Abs(bottomLeft.x)) / 50);
        //Debug.Log("count : " + (Mathf.Abs(startPos.y) - Mathf.Abs(bottomLeft.y)) / 50);

        //StartNode = NodeArray[(Mathf.Abs(startPos.x) - Mathf.Abs(bottomLeft.x)) / 50,
        //                        (Mathf.Abs(startPos.y) - Mathf.Abs(bottomLeft.y)) / 50];
        //TargetNode = NodeArray[(Mathf.Abs(targetPos.x) - Mathf.Abs(bottomLeft.x)) / 50,
        //                        (Mathf.Abs(targetPos.y) - Mathf.Abs(bottomLeft.y)) / 50];

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        //StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        //TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];
        //StartNode = NodeArray[(startPos.x - bottomLeft.x) / 50, (startPos.y - bottomLeft.y) / 50];
        //TargetNode = NodeArray[(targetPos.x - bottomLeft.x) / 50, (targetPos.y - bottomLeft.y) / 50];
        StartNode = NodeArray[13,1];
        TargetNode = NodeArray[1,4];

        //Debug.Log("Start.x : " + (startPos.x - bottomLeft.x) / 50);
        //Debug.Log("Start.y : " + (startPos.y - bottomLeft.y) / 50);


        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            //Debug.Log("cur.x : " + CurNode.x);
            //Debug.Log("cur.y : " + CurNode.y);

            // 마지막
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (int i = 0; i < FinalNodeList.Count; i++) print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].y);

                m_wayNum = FinalNodeList.Count;
                      
                int numA = MakeRandomNum(m_wayNum);
                int numB = MakeRandomNum(m_wayNum);

                if(numA == numB)
                {
                    numB = m_wayNum + 6;
                }

                m_answerList.Add(m_wayNum);
                m_answerList.Add(numA);
                m_answerList.Add(numB);

                ShuffleList(m_answerList);

                m_answer1.text = m_answerList[0].ToString();
                m_answer2.text = m_answerList[1].ToString();
                m_answer3.text = m_answerList[2].ToString();

                m_answerNum1 = m_answerList[0];
                m_answerNum2 = m_answerList[1];
                m_answerNum3 = m_answerList[2];

                return;
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 50, CurNode.y + 50);
                OpenListAdd(CurNode.x - 50, CurNode.y + 50);
                OpenListAdd(CurNode.x - 50, CurNode.y - 50);
                OpenListAdd(CurNode.x + 50, CurNode.y - 50);
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }

        // 답이 없어 여기로 내려온 경우 다시 실행
        PathFinding();
    }

    void OpenListAdd(int checkX, int checkY)
    {
        try
        {
            Debug.Log("checkX : " + checkX);
            Debug.Log("checkY : " + checkY);        

            // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
            if (checkX >= bottomLeft.x && checkX < topRight.x// + 1
                && checkY >= bottomLeft.y && checkY < topRight.y// + 1 
                && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall 
                && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y])
                )
            {
                // 대각선 허용시, 벽 사이로 통과 안됨
                if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

                // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
                if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;


                // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
                Node NeighborNode = NodeArray[(checkX - bottomLeft.x), (checkY - bottomLeft.y)];
                int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);

                // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
                }
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
    int MakeRandomNum(int answer)
    {
        int num = 0;

        System.Random random1 = new System.Random();
        System.Random random2 = new System.Random();
        int randomNum = random1.Next(5) + 1;
        int randomMulti = random1.Next(2);

        num = answer + (randomMulti == 0 ? -1 : 1) * randomNum;

        return num;
    }

    void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void ChickButton(int num)
    {
        int checkNum = m_answerList[num];

        if(checkNum == m_wayNum)
        {
            GameManager.m_cInstance.LeaveMiniGame();
            GameManager.m_cInstance.SetComment("정답입니다. 축하드립니다.");
            miniGameConsole.GetComponent<MiniGameControl>().CanMove();
            GameManager.m_cInstance.ChangePlayerControler();

        }

        if(checkNum != m_wayNum)
        {
            GameManager.m_cInstance.SetComment("틀렸습니다....");
            PathFinding();
        }
    }

    void OnDrawGizmos()
    {
        int width = 500;
        int height = 385;

        if (FinalNodeList.Count != 0)
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
            {
                Gizmos.color = Color.black;
                //Vector3 vStart = new Vector3(FinalNodeList[i].x, 0, FinalNodeList[i].y);
                //Vector3 vEnd = new Vector3(FinalNodeList[i+1].x, 0, FinalNodeList[i+1].y);
                Vector3 vStart = new Vector3(FinalNodeList[i].x * 50 + width, FinalNodeList[i].y * 50 + height);
                Vector3 vEnd = new Vector3(FinalNodeList[i + 1].x * 50 + width, FinalNodeList[i + 1].y * 50 + height);
                Gizmos.DrawLine(vStart, vEnd);
            }
    }
}
