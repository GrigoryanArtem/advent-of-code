namespace Puzzles.Runner._2022;

[Puzzle("No Space Left On Device", 7, 2022)]
public class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private class Node
    {
        public Node? Parent { get; set; }
        public required string Name { get; init; }

        public virtual bool IsDir => true;
        public virtual long Size => Childrens.Values.Sum(c => c.Size);

        public Dictionary<string, Node> Childrens { get; } = [];

        public string GetFullName()
        {
            List<string> names = [Name];

            var current = Parent;
            while (current != null)
            {
                names.Add(current.Name != "/" ? current.Name : "");

                current = current.Parent;
            }

            names.Reverse();
            return String.Join("/", names);
        }

        public virtual void Append(Node node)
        {
            node.Parent = this;
            Childrens.Add(node.Name, node);
        }
    }

    private class FileNode(long size) : Node
    {
        public override bool IsDir => false;
        public override long Size => size;
    }

    public string SolvePart1()
    {
        Node root = BuildNode();

        var dict = new Dictionary<string, long>();
        DirectrySizes(root, dict);
        return dict.Sum(kv => kv.Value < 100000 ? kv.Value : 0L).ToString();
    }

    public string SolvePart2()
    {
        Node root = BuildNode();

        var total = 70000000;
        var target = 30000000;

        var free = total - root.Size;
        var need = target - free;

        var dict = new Dictionary<string, long>();
        DirectrySizes(root, dict);

        return dict.Values.OrderBy(v => v).First(v => v >= need).ToString();
    }

    private Node BuildNode()
    {
        Node root = new() { Name = "" };
        Node current = root;

        foreach (var line in input.Lines)
        {
            if (line.StartsWith("$ cd"))
            {
                var path = line[5..];

                if (path == "..")
                {
                    current = current.Parent!;
                }
                else
                {
                    if (current.Childrens.TryGetValue(path, out var nextNode))
                    {
                        current = nextNode;
                    }
                    else
                    {
                        var newNode = new Node() { Name = path };
                        current.Append(newNode);
                        current = newNode;
                    }
                }

                continue;
            }

            if (line.StartsWith("$ ls"))
                continue;

            if (line.StartsWith("dir"))
            {
                var name = line[4..];

                if (!current.Childrens.ContainsKey(name))
                {
                    var newNode = new Node() { Name = name };
                    current.Append(newNode);
                }

                continue;
            }

            var tokens = line.Split(' ', 2);
            var (size, file) = (Convert.ToInt64(tokens[0]), tokens[1]);

            if (!current.Childrens.ContainsKey(file))
            {
                var newNode = new FileNode(size) { Name = file };
                current.Append(newNode);
            }
        }
        var result = root.Childrens.Single().Value;
        result.Parent = null;

        return result;
    }

    private static void DirectrySizes(Node node, Dictionary<string, long> dirs)
    {
        if (!node.IsDir)
            return;

        dirs.TryAdd(node.GetFullName(), node.Size);
        node.Childrens.ForEach(children => DirectrySizes(children.Value, dirs));
    }
}
