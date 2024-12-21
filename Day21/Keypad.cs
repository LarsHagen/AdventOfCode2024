namespace Day21;

public class Keypad
{
    public List<List<char>> Buttons { get; set; }
    public (int x, int y) EmptyButton { get; set; }
    
    private Dictionary<char, (int x, int y)> _buttonCoordinates = new();
    private (int x, int y) _positionA;
    
    public Keypad(bool robotStyle)
    {
        Buttons = new();
        if (robotStyle)
        {
            Buttons.Add(new List<char>{' ', '^', 'A'});
            Buttons.Add(new List<char>{'<', 'v', '>'});
            
            EmptyButton = (0, 0);
            _positionA = (2, 0);
        }
        else
        {
            Buttons.Add(new List<char>{'7', '8', '9'});
            Buttons.Add(new List<char>{'4', '5', '6'});
            Buttons.Add(new List<char>{'1', '2', '3'});
            Buttons.Add(new List<char>{' ', '0', 'A'});
            
            EmptyButton = (0, 3);
            _positionA = (2, 3);
        }
        
        for (int y = 0; y < Buttons.Count; y++)
        {
            for (int x = 0; x < Buttons[y].Count; x++)
            {
                _buttonCoordinates.Add(Buttons[y][x], (x, y));
            }
        }
    }
    
    
    public List<string> GetAllButtonPressSequence(string code)
    {
        var currentPosition = _positionA;

        List<List<string>> directionInputs = new();
        
        foreach (var c in code)
        {
            var coordinate = _buttonCoordinates[c];
            
            string directions = ""; 
            if (coordinate.x < currentPosition.x)
            {
                directions += new string('<', currentPosition.x - coordinate.x);
            }
            else if (coordinate.x > currentPosition.x)
            {
                directions += new string('>', coordinate.x - currentPosition.x);
            }
            
            if (coordinate.y < currentPosition.y)
            {
                directions += new string('^', currentPosition.y - coordinate.y);
            }
            else if (coordinate.y > currentPosition.y)
            {
                directions += new string('v', coordinate.y - currentPosition.y);
            }
            //Console.WriteLine("Directions: " + directions);
            directionInputs.Add(GetAllPermutations(directions).ToList());
            currentPosition = coordinate;
        }
        
        
        //Build all possible sequences
        List<string> result = new();
        foreach (var directionInput in directionInputs)
        {
            List<string> nextIteration = new();
            
            foreach (var direction in directionInput)
            {
                var directionWithA = direction + "A";
                if (result.Count == 0)
                {
                    nextIteration.Add(directionWithA);
                }
                else
                {
                    foreach (var sequence in result)
                    {
                        nextIteration.Add(sequence + directionWithA);
                    }
                }
            }
            
            result = nextIteration;
        }
        
        //Remove sequences that hit the empty spot
        result.RemoveAll(SequenceHitsEmptySpot);
        
        return result;

        //TODO: avoid gaps? Doesn't seem necessary for the result as only the length is used
    }

    private bool SequenceHitsEmptySpot(string sequence)
    {
        (int x, int y) currentPosition = _positionA;
        foreach (var direction in sequence)
        {
            switch (direction)
            {
                case '^':
                    currentPosition.y--;
                    break;
                case 'v':
                    currentPosition.y++;
                    break;
                case '<':
                    currentPosition.x--;
                    break;
                case '>':
                    currentPosition.x++;
                    break;
                case 'A':
                    continue;
            }

            if (currentPosition == EmptyButton)
            {
                return true;
            }
        }

        return false;
    }
    
    public static HashSet<string> GetAllPermutations(string input)
    {
        if (input.Length < 2)
        {
            return new HashSet<string>{input};
        }
        
        HashSet<string> result = new HashSet<string>();
        Permute(input.ToCharArray(), 0, input.Length - 1, result);
        return result;
    }

    private static void Permute(char[] array, int left, int right, HashSet<string> result)
    {
        if (left == right)
        {
            result.Add(new string(array));
        }
        else
        {
            for (int i = left; i <= right; i++)
            {
                Swap(ref array[left], ref array[i]);
                Permute(array, left + 1, right, result);
                Swap(ref array[left], ref array[i]); // backtrack
            }
        }
    }
    
    private static void Swap(ref char a, ref char b)
    {
        (a, b) = (b, a);
    }
}