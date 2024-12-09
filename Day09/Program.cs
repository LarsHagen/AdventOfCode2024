using System.Text;

List<long> input = new();
foreach (var c in File.ReadAllText("Input/Input.txt"))
{
    input.Add(long.Parse("" + c));
}

Memory memoryPart1 = new Memory();
memoryPart1.Start = new();

MemoryPosition currentPosition = memoryPart1.Start;
long fileIndex = 0;

bool isFile = true;
foreach (var blockSize in input)
{
    for (int i = 0; i < blockSize; i++)
    {
        if (isFile)
            currentPosition.Value = fileIndex;
        
        currentPosition.Next = new();
        currentPosition.Next.MemoryIndex = currentPosition.MemoryIndex + 1;
        currentPosition.Next.Previous = currentPosition;
        currentPosition = currentPosition.Next;
    }
    isFile = !isFile;

    if (isFile)
        fileIndex++;
}

long highestFileIndex = fileIndex;

memoryPart1.End = currentPosition;
Memory memoryPart2 = new Memory(memoryPart1);

//Part 1
var currentFreePosition = memoryPart1.Start;
var currentBackPosition = memoryPart1.End;

while (currentBackPosition.MemoryIndex >= currentFreePosition.MemoryIndex)
{
    while (currentFreePosition.Value.HasValue)
    {
        currentFreePosition = currentFreePosition.Next;
    }

    if (currentBackPosition.MemoryIndex < currentFreePosition.MemoryIndex)
        break;
    
    if (currentBackPosition.Value.HasValue)
    {
        currentFreePosition.Value = currentBackPosition.Value;
        currentBackPosition.Value = null;
    }
    
    currentBackPosition = currentBackPosition.Previous;
}

Console.WriteLine("Part 1: " + memoryPart1.Checksum());

//Part 2

List<MemoryBlock> memoryBlocks = new();
MemoryBlock currentBlock = new();
currentBlock.Value = memoryPart2.Start.Value;

currentPosition = memoryPart2.Start.Next;
while (currentPosition != null)
{
    currentBlock.Length++;
    
    if (currentPosition.Value != currentBlock.Value)
    {
        memoryBlocks.Add(currentBlock);
        currentBlock = new MemoryBlock();
        currentBlock.Value = currentPosition.Value;
    }
    
    currentPosition = currentPosition.Next;
}

long valueToMove = highestFileIndex;
while (valueToMove > 0)
{
    //Merge empty blocks next to each other
    for (var index = 0; index < memoryBlocks.Count - 1; index++)
    {
        if (memoryBlocks[index].Value.HasValue || memoryBlocks[index + 1].Value.HasValue)
            continue;
        
        memoryBlocks[index].Length += memoryBlocks[index + 1].Length;
        memoryBlocks.RemoveAt(index + 1);
    }
    

    var blockToMove = memoryBlocks.First(b => b.Value == valueToMove);
    var blockToMoveIndex = memoryBlocks.IndexOf(blockToMove);

    for (var index = 0; index < blockToMoveIndex; index++)
    {
        var freeMemoryBlock = memoryBlocks[index];
        if (freeMemoryBlock.Value.HasValue)
            continue; //Looking for empty blocks

        if (freeMemoryBlock.Length < blockToMove.Length)
            continue;

        //Move block
        memoryBlocks.Insert(blockToMoveIndex, new MemoryBlock {Length = blockToMove.Length});
        memoryBlocks.Remove(blockToMove);
        memoryBlocks.Insert(index, blockToMove);
        freeMemoryBlock.Length -= blockToMove.Length;
        if (freeMemoryBlock.Length == 0)
            memoryBlocks.Remove(freeMemoryBlock);
        
        break;
    }

    valueToMove--;
}

//Rebuild memory
memoryPart2.Start = new MemoryPosition();
currentPosition = memoryPart2.Start;
currentPosition.MemoryIndex = 0;
foreach (var memoryBlock in memoryBlocks)
{
    for (int i = 0; i < memoryBlock.Length; i++)
    {
        currentPosition.Value = memoryBlock.Value;
        currentPosition.Next = new MemoryPosition();
        currentPosition.Next.MemoryIndex = currentPosition.MemoryIndex + 1;
        currentPosition = currentPosition.Next;
    }
}

Console.WriteLine("Part 2: " + memoryPart2.Checksum());


class MemoryBlock()
{
    public long? Value { get; set; } = null;
    public long Length { get; set; } = 0;
}

class Memory
{
    public MemoryPosition Start { get; set; }
    public MemoryPosition End { get; set; }
    
    public Memory() {}

    public Memory(Memory copy)
    {
        Start = new();
        var currentNew = Start;
        var currentOld = copy.Start;

        while (currentOld != null)
        {
            currentNew.Value = currentOld.Value;
            currentNew.MemoryIndex = currentOld.MemoryIndex;
            currentNew.Next = new();
            currentNew.Next.Previous = currentNew;
            
            currentOld = currentOld.Next;
            currentNew = currentNew.Next;
        }
    }

    public long Checksum()
    {
        long checksum = 0;
        var currentPosition = Start;
        while (currentPosition != null)
        {
            if (currentPosition.Value.HasValue)
            {
                checksum += currentPosition.Value.Value * currentPosition.MemoryIndex;
            }
            currentPosition = currentPosition.Next;
        }

        return checksum;
    }
}

class MemoryPosition
{
    public long? Value { get; set; } = null;
    public MemoryPosition Next { get; set; }
    public MemoryPosition Previous { get; set; }
    public long MemoryIndex { get; set; } = 0;
}