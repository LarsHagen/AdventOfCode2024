using System.Security.Cryptography;

namespace Day21;

public class Keypad
{
    public List<List<char>> Buttons { get; set; }
    public (int x, int y) EmptyButton { get; set; }
    
    public Keypad(bool robotStyle)
    {
        Buttons = new();
        if (robotStyle)
        {
            Buttons.Add(new List<char>{' ', '^', 'A'});
            Buttons.Add(new List<char>{'<', 'v', '>'});
            
            EmptyButton = (0, 0);
        }
        else
        {
            Buttons.Add(new List<char>{'7', '8', '9'});
            Buttons.Add(new List<char>{'4', '5', '6'});
            Buttons.Add(new List<char>{'1', '2', '3'});
            Buttons.Add(new List<char>{' ', '0', 'A'});
            
            EmptyButton = (0, 3);
        }
    }
    
    public List<(int x, int y)> GetButtonCoordinates(params char[] buttonValues)
    {
        List<(int x, int y)> coordinates = new();

        foreach (var button in buttonValues)
        {
            for (int y = 0; y < Buttons.Count; y++)
            {
                for (int x = 0; x < Buttons[y].Count; x++)
                {
                    if (Buttons[y][x] ==button)
                    {
                        coordinates.Add((x, y));
                    }
                }
            }
        }
        
        
        return coordinates;
    }
    
    public List<string> GetAllButtonPressSequence(List<(int x, int y)> coordinates)
    {
        var currentPosition = GetButtonCoordinates('A').First();

        List<List<string>> directionInputs = new();
        
        foreach (var coordinate in coordinates)
        {
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
            var allPossibleSequencesSoFar = result.ToList();
            result.Clear();
            foreach (var direction in directionInput)
            {
                if (allPossibleSequencesSoFar.Count == 0)
                {
                    result.Add(direction + "A");
                }
                else
                {
                    foreach (var sequence in allPossibleSequencesSoFar)
                    {
                        result.Add(sequence + direction + "A");
                    }
                }
            }
        }
        
        //Remove sequences that hit the empty spot
        result.RemoveAll(SequenceHitsEmptySpot);
        
        return result;

        //TODO: avoid gaps? Doesn't seem necessary for the result as only the length is used
    }

    private bool SequenceHitsEmptySpot(string sequence)
    {
        (int x, int y) currentPosition = GetButtonCoordinates('A').First();
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
    
    /*private HashSet<string> GetAllPermutations(string input)
    {
        HashSet<string> result = new();
        if (input.Length == 1)
        {
            result.Add(input);
            return result;
        }
        
        for (int i = 0; i < input.Length; i++)
        {
            var remaining = input.Remove(i, 1);
            var permutations = GetAllPermutations(remaining);
            foreach (var permutation in permutations)
            {
                result.Add(input[i] + permutation);
            }
        }

        return result;
    }*/
    
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
        char temp = a;
        a = b;
        b = temp;
    }
}