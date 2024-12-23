namespace Day21;

public class Keypad
{
    public List<List<char>> Buttons { get; set; }
    public (int x, int y) EmptyButton { get; set; }
    
    public Dictionary<char, (int x, int y)> ButtonCoordinates = new();
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
                ButtonCoordinates.Add(Buttons[y][x], (x, y));
            }
        }
    }
    
    public List<string> GetSequencesFromTo((int x, int y) start, (int x, int y) end)
    {
        var horizontalInput = end.x < start.x ? new string('<', start.x - end.x) : new string('>', end.x - start.x);
        var verticalInput = end.y < start.y ? new string('^', start.y - end.y) : new string('v', end.y - start.y);
        
        HashSet<string> directionInputs = new();
        directionInputs.Add(horizontalInput + verticalInput);
        directionInputs.Add(verticalInput + horizontalInput);
        
        //Remove sequences that hit the empty spot
        List<string> result = new(directionInputs);
        result.RemoveAll(SequenceHitsEmptySpot);
        
        return result;
    }
    
    public List<string> GetAllButtonPressSequence(string code, (int x, int y)? start = null)
    {
        var currentPosition = _positionA;
        if (start != null)
        {
            currentPosition = start.Value;
        }

        List<List<string>> directionInputs = new();
        
        foreach (var c in code)
        {
            var coordinate = ButtonCoordinates[c];
            directionInputs.Add(GetSequencesFromTo(currentPosition, coordinate));
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
        
        return result;
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
}