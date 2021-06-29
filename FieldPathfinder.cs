using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FieldPathfinder
{
    static Node[,] nodes;
    static int rows, cols;
    static bool evenAreSmaller;

    public static void Initialize(TileController[,] tiles, bool evenAreSmaller)
    {
        rows = tiles.GetUpperBound(0) + 1;
        cols = tiles.Length / rows;
        nodes = new Node[rows, cols];

        FieldPathfinder.evenAreSmaller = evenAreSmaller;

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                nodes[i, j] = new Node(i, j, tiles[i,j]);
            }
        }

        Connect();
    }

    private static void Connect()
    {
        if (evenAreSmaller)
            LinkIfEvenAreSmaller();
        else
            LinkIfEvenAreNotSmaller();
    }

    private static void LinkIfEvenAreSmaller()
    {
        for (int row = 0; row < rows; row++)
        {
            AddToRightOrientedNodes(row);
            if (row < rows - 1)
                row++;
            AddToLeftOrientedNodes(row);
        }
    }

    private static void LinkIfEvenAreNotSmaller()
    {
        for (int row = 0; row < rows; row++)
        {
            AddToLeftOrientedNodes(row);
            if (row < rows - 1)
                row++;
            AddToRightOrientedNodes(row);
        }
    }

    private static void AddToLeftOrientedNodes(int row)
    {
        for(int col = 0; col < cols; col++)
        {
            if(col < cols - 1)
                nodes[row, col].AddNeighbour(nodes[row, col + 1]);
            if(row < rows - 1)
            {
                if(col > 0)
                    nodes[row, col].AddNeighbour(nodes[row + 1, col - 1]);
                nodes[row, col].AddNeighbour(nodes[row + 1, col]);
            }
        }
    }

    private static void AddToRightOrientedNodes(int row)
    {
        for (int col = 0; col < cols; col++)
        {
            if(col < cols - 1)
                nodes[row, col].AddNeighbour(nodes[row, col+1]);
            if(row < rows - 1)
            {
                if(col < cols - 1)
                    nodes[row, col].AddNeighbour(nodes[row + 1, col + 1]);
                nodes[row, col].AddNeighbour(nodes[row + 1, col]);
            }
        }
    }



    public static void ApplyNewMask(List<Mask> masks)
    {
        ClearMask();

        foreach (Mask mask in masks)
        {
            nodes[mask.Y, mask.X].ApplyMask(mask.IsHidden, mask.IsSelectable);
        }

    }

    public static Path FindPath(int x1, int y1, int x2, int y2)
    {
        return FindPath(nodes[y1, x1], nodes[y2, x2]);
    }

    public static Path FindPath(Node start, Node end)
    {
        List<Node> openedNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        openedNodes.Add(start);

        if (!end.IsTraversible)
            end.ClearMask();

        while (openedNodes.Count > 0)
        {
            Node current = FindWithLeastCost(openedNodes);
            openedNodes.Remove(current);
            closedNodes.Add(current);

            if (current.Equals(end))
                return GetPathFromNode(current);

            foreach (Node neighbour in current.Neighbours)
            {
                if (!neighbour.IsTraversible || closedNodes.Contains(neighbour))
                    continue;

                int distToEnd = GetActualDistanceBetween(neighbour, end);
                int newCost = current.DistanceFromBeginning + distToEnd + 1;

                if (!openedNodes.Contains(neighbour) || neighbour.Cost > newCost)
                {
                    // ¬озможно можно не считать дистанцию до current и просто заменить ее единичкой
                    neighbour.DistanceFromEnd = distToEnd;
                    neighbour.DistanceFromBeginning = current.DistanceFromBeginning + 1;
                    neighbour.Parent = current;
                    if (!openedNodes.Contains(neighbour))
                        openedNodes.Add(neighbour);
                }
            }
        }

        ClearParents();
        return new Path();
    }

    public static List<Pair> GetTilesInRadius(int x, int y, int radius)
    {
        return GetTilesInRadius(nodes[y, x], radius);
    }

    public static List<Pair> GetTilesInRadius(Node center, int radius)
    {
        List<Pair> pairs = new List<Pair>();

        List<Node> toHighlite = new List<Node>();
        List<Node> inUse = new List<Node>();
        List<Node> toUse = new List<Node>();

        inUse.Add(center);

        for (int i = 0; i < radius; i++)
        {
            foreach (Node node in inUse)
            {
                foreach (Node n in node.Neighbours)
                {
                    if (toHighlite.Contains(n) || inUse.Contains(n) || toUse.Contains(n) || !n.IsSelectable)
                        continue;

                    if (!n.IsTraversible)
                        toHighlite.Add(n);
                    else
                        toUse.Add(n);
                }
            }

            toHighlite.AddRange(toUse);
            inUse = toUse.GetRange(0, toUse.Count);
            toUse.Clear();
        }

        foreach (Node node in toHighlite)
            pairs.Add(new Pair(node.X, node.Y));

        return pairs;
    }



    private static void PrintNodeInfo(Node node)
    {
        MonoBehaviour.print("Main: " + node.X + " " + node.Y);
        foreach (Node n in node.Neighbours)
            MonoBehaviour.print(n.X + " " + n.Y);
    }

    private static int GetActualDistanceBetween(Node node1, Node node2)
    {
        int dx = Mathf.Abs(node1.X - node2.X);
        int dy = Mathf.Abs(node1.Y - node2.Y);

        if (dy % 2 == 0)
            return dx + dy - dy / 2;
        else
            return dx + dy - dy / 2 - ((Mathf.Min(node1.Y, node2.Y) % 2 == 0) == evenAreSmaller?1:0);
    }

    private static Path GetPathFromNode(Node node)
    {
        Path path = new Path();
        path.Nodes.Add(node);

        while (node.Parent != null)
        {
            path.Nodes.Add(node.Parent);
            node = node.Parent;
        }

        ClearParents();

        return path;
    }

    private static void ClearParents()
    {
        foreach (Node n in nodes)
            n.Parent = null;
    }

    private static Node FindWithLeastCost(List<Node> nodes)
    {
        int min = int.MaxValue;
        Node least = null;

        foreach(Node node in nodes)
        {
            if(min > node.Cost)
            {
                least = node;
                min = node.Cost;
            }
        }

        return least;
    }

    private static void ClearMask()
    {
        foreach (Node node in nodes)
            node.ClearMask();
    }

    public class Node
    {
        List<Node> neighbours;
        int x;
        int y;
        bool isTraversible;
        bool isSelectable;
        bool isHidden;
        Node parent;

        TileController tile;

        public TileController Tile
        {
            get { return tile; }
        }

        public List<Node> Neighbours
        {
            get { return neighbours; }
            set { neighbours = value; }
        }

        public Node Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int Cost
        {
            get { return DistanceFromBeginning + DistanceFromEnd; }
        }

        public int DistanceFromBeginning { get; set; }

        public int DistanceFromEnd { get; set; }

        public bool IsTraversible
        {
            get { return isTraversible && !isHidden; }
        }

        public bool IsSelectable
        {
            get { return IsTraversible || isSelectable; }
        }

        public Node()
        {
            neighbours = new List<Node>();
        }

        public Node(int x, int y, TileController tile)
        {
            neighbours = new List<Node>();
            X = x;
            Y = y;
            this.tile = tile;
            isTraversible = tile != null;
            Parent = null;
        }

        public void AddNeighbour(Node node)
        {
            Neighbours.Add(node);
            node.Neighbours.Add(this);
        }

        public void ApplyMask(bool isHidden, bool isSelectable)
        {
            this.isHidden = isHidden;
            this.isSelectable = isSelectable;
        }

        public void ClearMask()
        {
            isHidden = false;
            isSelectable = false;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Node))
                return false;

            Node n = obj as Node;

            return X == n.X && Y == n.Y;
        }

        public override int GetHashCode()
        {
            return x*x*x.GetHashCode() + y.GetHashCode();
        }
    }

    public class Path
    {
        List<Node> nodes = new List<Node>();

        public List<Node> Nodes
        {
            get { return nodes; }
            private set { nodes = value; }
        }

        public int Length
        {
            get { return nodes.Count; }
        }

        public Path()
        {
            nodes = new List<Node>();
        }

        public Path GetSubpath(int start)
        {
            return GetSubpath(start, Nodes.Count - start - 1);
        }

        public Path GetSubpath(int start, int length)
        {
            if (start + length >= Nodes.Count)
                throw new System.Exception("ѕуть не содержит столько точек");

            Path newPath = new Path();

            for(int i = start; i < start+length; i++)
            {
                newPath.Nodes.Add(Nodes[i]);
            }

            return newPath;
        }
    }

    public class Mask
    {
        Pair coords;
        bool isHidden;
        bool isSelectable;

        public int X
        {
            get { return coords.x; }
        }

        public int Y
        {
            get { return coords.y; }
        }

        public bool IsHidden
        {
            get { return isHidden; }
        }

        public bool IsSelectable
        {
            get { return isSelectable; }
        }

        public Mask(Pair coords, bool isHidden, bool isSelectable)
        {
            this.coords = coords;
            this.isHidden = isHidden;
            this.isSelectable = isSelectable;
        }

        public Mask(int x, int y, bool isHidden, bool isSelectable)
        {
            this.coords = new Pair(x, y);
            this.isHidden = isHidden;
            this.isSelectable = isSelectable;
        }
    }
}

public class Pair
{
    public int x;
    public int y;

    public Pair(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
