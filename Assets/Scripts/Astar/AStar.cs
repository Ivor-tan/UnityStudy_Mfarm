using System.Collections;
using System.Collections.Generic;
using MFarm.Astar;
using MFarm.Map;
using UnityEngine;

namespace MFarm.AStar
{
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList;    //��ǰѡ��Node��Χ��8����
        private HashSet<Node> closedNodeList;   //���б�ѡ�еĵ�

        private bool pathFound;


        /// <summary>
        /// ����·��
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="movementSteps"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos,Stack<MovementStep> movementSteps)
        {
            pathFound = false;

            if (GenerateGridNodes(sceneName, startPos, endPos))
            {
                //�������·��
                if (FindShortestPath())
                {
                    //����NPC�ƶ�·��
                    UpadtePathMovementStepStak(sceneName, movementSteps);
                }
            }
        }



        /// <summary>
        /// ��������ڵ���Ϣ����ʼ�������б�
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="startPos">���</param>
        /// <param name="endPos">�յ�</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
            
                //������Ƭ��ͼ��Χ���������ƶ��ڵ㷶Χ����
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();

                closedNodeList = new HashSet<Node>();
            }
            else
                return false;

            //gridNodes�ķ�Χ�Ǵ�0,0��ʼ������Ҫ��ȥԭ������õ�ʵ��λ��
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    var key = tilePos.x + "x" + tilePos.y + "y" + sceneName;
                    TileDetails tile = GridMapManager.Instance.GetTileDetails(key);
                    //TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);    //͵��

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                            node.isObstacle = true;
                    }
                }
            }

            return true;
        }


        private bool FindShortestPath()
        {
            //������
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {//�ڵ�����Node�ں��ȽϺ���
                openNodeList.Sort();

                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                //������Χ8��Node���䵽OpenList
                EvaluateNeighbourNodes(closeNode);
            }

            return pathFound;
        }

        /// <summary>
        /// ������Χ8���㣬�����ɶ�Ӧ����ֵ
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);

                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            //���Ӹ��ڵ�
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// �ҵ���Ч��Node,���ϰ�������ѡ��
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
                return null;
            else
                return neighbourNode;
        }


        /// <summary>
        /// �����������ֵ
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14�ı���+10�ı���</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        /// <summary>
        /// ����ÿһ������
        /// </summary>
        /// <param name="sceneName">������</param>
        /// <param name="movementStep">ÿһ����Ϣ</param>
        private void UpadtePathMovementStepStak(string sceneName , Stack<MovementStep> movementStep)
        {
            Node nextNode = targetNode;

            while(nextNode != null)
            {
                MovementStep npcStep = new MovementStep();
                npcStep.sceneName = sceneName;
                npcStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

                //ѹ��
                movementStep.Push(npcStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}