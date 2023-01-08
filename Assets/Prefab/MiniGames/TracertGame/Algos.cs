using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Prefab.MiniGames.TracertGame
{
    public class DFSNode
    {
        public string Name;
        public Stack<DFSNode> Parent = new Stack<DFSNode>();
        public bool Visited = false;
        public int PathLength = 0;

        public DFSNode(string name)
        {
            Name = name;
        }
    }

    public static class DFS
    {
        private static List<string>? NodesStr;
        private static List<(string N1, string N2)>? EdgesStr;
        private static Dictionary<string, int>? nodeByIndex;
        private static DFSNode[]? Nodes;
        private static bool[,]? adjMatrix;
        public static List<List<string>> allPaths(List<string> nodes, List<(string N1, string N2)> edges, string srcNodeStr, string destNodeStr)
        {
            NodesStr = nodes;
            EdgesStr = edges;

            // create node names to index mapping
            nodeByIndex = new();
            Nodes = new DFSNode[NodesStr.Count];
            int ind = 0;
            foreach (string node in NodesStr)
            {
                if (!nodeByIndex.ContainsKey(node))
                {
                    Nodes[ind] = new DFSNode(node);
                    nodeByIndex.Add(node, ind);
                    ind++;
                }
            }

            // create adjacency matrix
            adjMatrix = new bool[Nodes.GetLength(0), Nodes.GetLength(0)];
            foreach ((string N1, string N2) in EdgesStr)
            {
                adjMatrix[nodeByIndex[N1], nodeByIndex[N2]] = true;
                adjMatrix[nodeByIndex[N2], nodeByIndex[N1]] = true;
            }


            // actual DFS
            if (!nodeByIndex.ContainsKey(srcNodeStr) || !nodeByIndex.ContainsKey(destNodeStr)) return new();
            List<List<string>> allPaths = new List<List<string>>();
            DFSNode srcNode = Nodes[nodeByIndex[srcNodeStr]];
            DFSNode destNode = Nodes[nodeByIndex[destNodeStr]];

            Stack<DFSNode> dfs = new Stack<DFSNode>();
            dfs.Push(srcNode);
            while (dfs.Count != 0)
            {
                DFSNode currNode = dfs.Peek();

                // check if destNode
                if (currNode.Name == destNodeStr)
                {
                    // create the path from neighbor node -> currNode
                    DFSNode tempCurr = currNode;
                    List<string> path = new();

                    while (true)
                    {
                        path.Add(tempCurr.Name);
                        if (tempCurr.Parent.Count == 0) break;
                        tempCurr = tempCurr.Parent.Peek();
                    }
                    path.Reverse();
                    allPaths.Add(path);
                    currNode.Visited = false;
                    currNode.Parent.Pop();
                    dfs.Pop();
                    continue;
                }

                int ind2 = nodeByIndex[currNode.Name];

                if (!currNode.Visited)
                {
                    currNode.Visited = true;
                    for (int i = 0; i < Nodes.GetLength(0); i++)
                    {
                        if (adjMatrix[ind2, i])
                        {
                            DFSNode neighbor = Nodes[i];

                            if (!neighbor.Visited)
                            {
                                neighbor.PathLength = currNode.PathLength + 1;
                                neighbor.Parent.Push(currNode);
                                dfs.Push(neighbor);
                            }
                        }
                    }
                }
                else
                {
                    currNode.Visited = false;
                    if (currNode.Parent.Count != 0)
                        currNode.Parent.Pop();
                    dfs.Pop();
                }

            }

            allPaths.OrderBy(x => x.Count);

            // Time Complexity: O(E^2)
            // Space Complexity: O(V^2)
            return allPaths;
        }
    }
}
