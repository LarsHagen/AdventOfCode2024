using System.Text;

namespace Day17;

public class Base8Number
{
    private long _base10 = 0;
    private string _base8 = "0";

    public long Base10
    {
        get => _base10;
        set
        {
            _base10 = value; 
            _base8 = ConvertToBase8(value);
        }
    }
    public string Base8 
    {
        get => _base8;
        set
        {
            _base8 = value;
            _base10 = ConvertToBase10(value);
        }
    }
    
    public static Base8Number CreateFromBase8(string base8)
    {
        return new Base8Number { Base8 = base8 };
    }
    
    public static Base8Number CreateFromBase10(long base10)
    {
        return new Base8Number { Base10 = base10 };
    }
    
    private string ConvertToBase8(long base10)
    {
        var result = new StringBuilder();

        do
        {
            result.Insert(0, base10 % 8);
            base10 /= 8;
        } while (base10 > 0);

        return result.ToString();
    }

    private long ConvertToBase10(string base8)
    {
        long result = 0;
        int position = 0;
        
        foreach (var digit in base8.Reverse())
        {
            //converted = digit * (base ^ position)
            result += long.Parse(digit.ToString()) * (long)Math.Pow(8, position);
            position++;
        }

        return result;
    }
}