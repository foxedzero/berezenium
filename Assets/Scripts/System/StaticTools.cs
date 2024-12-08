using UnityEngine;
using System;
using System.Collections;

public delegate void SimpleVoid();

public class StaticTools
{
    public static Vector3 Coordinate(Vector3Int position) => new Vector3(31.5f - position.x, position.y * -4, position.z - 15.5f);

    static public float ScreenHeight => 1080f * Screen.height / Screen.width / 0.5625f;

    static public int[] Range(int lenght)
    {
        int[] range = new int[lenght];

        for(int i = 0; i < range.Length; i++)
        {
            range[i] = i;
        }

        return range;
    }

    static public string ToLegalFileName(string name)
    {
        string[] illegal = new string[] { "/", "\\", "\"", "?", "*", "|", "<", ">", ":" };

        foreach (string character in illegal)
        {
            name = name.Replace(character, "");
        }

        return name;
    }

    static public T[] Cross<T>(T[] a, T[] b)
    {
        T[] itog = new T[0];

        for (int i = 0; i < a.Length; i++)
        {
            foreach (T t in b)
            {
                if (a[i].Equals(t))
                {
                    if (!Contains(itog, t))
                    {
                        itog = ExpandMassive(itog, t);
                    }

                    break;
                }
            }
        }

        return itog;
    }

    static public int Match(string a, string b)
    {
        if(a.Length <= 0)
        {
            return 0;
        }
        else if(b.Length <= 0)
        {
            return -a.Length * 5;
        }

        int mismatch = 0;
        int symbol = 0;

        int distance = 0;

        for (int i = 0; i < b.Length; i++)
        {
            if (b[i] == a[symbol] || b[i].ToString().ToLower() == a[symbol].ToString().ToLower())
            {
                mismatch += distance;
                distance = 0;
                mismatch -= 10;

                symbol++;

                if(symbol >= a.Length)
                {
                    if(i + 1 < b.Length)
                    {
                        mismatch++;
                    }

                    break;
                }
            }
            else
            {
                distance++;
            }
        }

        mismatch += Mathf.Max(0, a.Length - symbol) * 5;

        return -mismatch;
    }

    static public float AverageSqrt(float value)
    {
        int seq = (int)value;
        while(seq * seq > value)
        {
            seq /= 2;
        }
        while(seq *seq < value)
        {
            seq++;
        }

        float sqr = seq * seq;
        if(sqr == value)
        {
            return seq;
        }

        return seq + (value - sqr) / seq / 2f;
    }

    static public float Sqrt(float value, int accuracy)
    {
        if (value == 0)
        {
            return 0;
        }

        if (value < 0)
        {
            return 0;
        }

        float itog = value / 2;

        float guess = 0;

        while (guess - itog != 0 && accuracy > 0)
        {
            accuracy--;

            guess = itog;
            itog = (guess + (value / guess)) / 2;
        }

        return itog;
    }

    static public string GetParameter(string info, string parameter)
    {
        int startPosition = -1;

        while((startPosition = info.IndexOf(parameter, startPosition + 1)) > -1)
        {
            if (startPosition + parameter.Length < info.Length && info[startPosition + parameter.Length] == '(')
            {
                break;
            }
        }

        if (startPosition < 0)
        {
            return "";
        }

        startPosition += parameter.Length + 1;

        int openCount = 1;
        for (int i = startPosition; i < info.Length; i++)
        {
            if (info[i] == '(')
            {
                openCount++;
            }
            else if (info[i] == ')')
            {
                openCount--;

                if (openCount == 0)
                {
                    return info.Substring(startPosition, i - startPosition);
                }
            }
        }

        return info.Substring(startPosition, info.Length - startPosition);
    }

    static public string SetParameter(string parameter, string info, string text)
    {
        int index = text.IndexOf(parameter);

        if(index < 0)
        {
            return text += $"{(text.Length < 1 ? "" : ",")}{parameter}({info})";
        }
        else
        {
            int length = parameter.Length;
            int open = 0;
            for(int i = index + parameter.Length; i < text.Length; i++)
            {
                length++;

                switch (text[i])
                {
                    case '(':
                        open++;
                        break;
                    case ')':
                        open--;

                        if(open < 1)
                        {
                            return text.Remove(index, length).Insert(index, $"{parameter}({info})");
                        }
                        break;
                }
            }

            return text.Remove(index) + $"{parameter}({info})";
        }
    }

    public static void Sort(float[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            int min = i;
            for (int ii = i; ii < values.Length; ii++)
            {
                if (values[min] > values[ii])
                {
                    min = ii;
                }
            }

            float buffer = values[i];
            values[i] = values[min];
            values[min] = buffer;
        }
    }

    static public string ToString<T>(T[] massive)
    {
        string itog = "";

        foreach (T t in massive)
        {
            itog += $" {t}";
        }

        return itog;
    }

    public static string ArrayToString(Array massive)
    {
        if (massive.Length == 0)
        {
            return $"[]";
        }

        string info = "";

        IEnumerator enumerator = massive.GetEnumerator();

        bool zapyataya = false;

        while (enumerator.MoveNext())
        {
            string subInfo = enumerator.Current is Array ? ArrayToString(enumerator.Current as Array) : enumerator.Current.ToString();

            if (zapyataya)
            {
                info += $", {subInfo}";
            }
            else
            {
                info += $"{subInfo}";
                zapyataya = true;
            }
        }

        return $"[{info}]";
    }

    static public float Summ(float[] values)
    {
        float itog = 0;

        foreach (float value in values)
        {
            itog += value;
        }

        return itog;
    }

    static public Vector2Int Summ(Vector2Int[] values)
    {
        Vector2Int itog = Vector2Int.zero;

        foreach (Vector2Int value in values)
        {
            itog += value;
        }

        return itog;
    }

    static public float Round(float value, int afterDot)
    {
        if (afterDot < 0)
        {
            afterDot = 0;
        }

        return Mathf.Round(value * Degree(10, afterDot)) / Degree(10, afterDot);
    }

    static public float Degree(float value, int count)
    {
        bool divive = false;

        if (count < 0)
        {
            divive = true;
            count = -count;
        }

        float itog = 1;

        while (count > 0)
        {
            if (divive)
            {
                itog /= value;
            }
            else
            {
                itog *= value;
            }

            count--;
        }

        return itog;
    }

    static public int Degree(int value, int count)
    {
        bool divive = false;

        if (count < 0)
        {
            divive = true;
            count = -count;
        }

        int itog = 1;

        while (count > 0)
        {
            if (divive)
            {
                itog /= value;
            }
            else
            {
                itog *= value;
            }

            count--;
        }

        return itog;
    }

    static public int IndexOf<T>(T[] massive, T value)
    {
        for (int i = 0; i < massive.Length; i++)
        {
            if (massive[i] != null && massive[i].Equals(value))
            {
                return i;
            }
        }

        return -1;
    }

    static public bool Contains<T>(T[] massive, T value)
    {
        for(int i = 0; i < massive.Length; i++)
        {
            if (massive[i] != null && massive[i].Equals(value))
            {
                return true;
            }
        }

        return false;
    }

    static public int ContainsType<A, B>(A[] massive)
    {
        for(int i = 0; i < massive.Length; i++)
        {
            if (massive[i].GetType() == typeof(B))
            {
                return i;
            }
        }

        return -1;
    }

    static public int ContainsType<T>(T[] massive, T value)
    {
        for (int i = 0; i < massive.Length; i++)
        {
            if (massive[i].GetType() == typeof(T))
            {
                return i;
            }
        }

        return -1;
    }

    static public T[] ExpandMassive<T>(T[] origin, T[] value)
    {
        T[] newMassive = new T[origin.Length + value.Length];

        Array.Copy(origin, newMassive, origin.Length);

        for(int i = origin.Length; i < newMassive.Length; i++)
        {
            newMassive[i] = value[i - origin.Length];
        }

        return newMassive;
    }

    static public T[] ExpandMassive<T>(T[] origin, T value)
    {
        T[] newMassive = new T[origin.Length + 1];

        Array.Copy(origin, newMassive, origin.Length);

        newMassive[newMassive.Length - 1] = value;

        return newMassive;
    }

    static public T[] ExcludingExpandMassive<T>(T[] origin, T[] value)
    {
        T[] newMassive = new T[origin.Length];

        Array.Copy(origin, newMassive, origin.Length);

        for (int i = 0; i < value.Length; i++)
        {
            newMassive = ExcludingExpandMassive(newMassive, value[i]);
        }

        return newMassive;
    }

    static public T[] ExcludingExpandMassive<T>(T[] origin, T value)
    {
        foreach (T t in origin)
        {
            if (t.Equals(value))
            {
                return origin;
            }
        }

        T[] newMassive = new T[origin.Length + 1];

        Array.Copy(origin, newMassive, origin.Length);

        newMassive[newMassive.Length - 1] = value;

        return newMassive;
    }

    static public T[] ExpandMassive<T>(T[] origin, T value, int index)
    {
        if(index > origin.Length)
        {
            index = origin.Length;
        }
        else if(index < 0)
        {
            index = 0;
        }

        T[] newMassive = new T[origin.Length + 1];
   
        int lastIndex = 0;
        for (int i = 0; i < newMassive.Length; i++)
        {
            if (i == index)
            {
                newMassive[i] = value;
            }
            else
            {
                newMassive[i] = origin[lastIndex];

                lastIndex++;
            }
        }

        return newMassive;
    }

    static public T[] RemoveDublicates<T>(T[] massive)
    {
        T[] newMassive = new T[0];

        foreach (T t in massive)
        {
            bool dublicate = false;
            foreach (T tt in newMassive)
            {
                if (tt.Equals(t))
                {
                    dublicate = true;
                    break;
                }
            }

            if (!dublicate)
            {
                newMassive = ExpandMassive(newMassive, t);
            }
        }

        return newMassive;
    }

    static public T[] ReduceMassive<T>(T[] origin, int index)
    {
        if(origin.Length == 0)
        {
            return origin;
        }

        T[] newMassive = new T[origin.Length - 1];

        int newIndex = 0;
        for (int i = 0; i < origin.Length; i++)
        {
            if (i != index)
            {
                newMassive[newIndex] = origin[i];

                newIndex++;
            }
        }

        return newMassive;
    }

    static public T[] RemoveFromMassive<T>(T[] origin, T value)
    {
        if(origin.Length == 0)
        {
            return origin;
        }

        T[] newMassive = new T[origin.Length - 1];

        int newIndex = 0;
        for (int i = 0; i < origin.Length; i++)
        {
            if (!origin[i].Equals(value))
            {
                if(newIndex >= newMassive.Length)
                {
                    return origin;
                }

                newMassive[newIndex] = origin[i];

                newIndex++;
            }
        }

        return newMassive;
    }

    public static float StringToFloat(string value)
    {
        float itog = 0;
        float ten = 1;

        int degree = 1;
        bool MEP = false;

        for (int i = value.Length - 1; i > -1; i--)
        {
            if (char.IsNumber(value[i]))
            {
                itog += GetNumber(value[i]) * ten;
                ten *= 10;
            }
            else if (value[i] == 'E')
            {
                MEP = true;
                degree = Mathf.RoundToInt(itog);

                itog = 0;
                ten = 1;
            }
            else if (value[i] == '.')
            {
                itog /= ten;
                ten = 1;
            }
            else if (value[i] == ',')
            {
                itog /= ten;
                ten = 1;
            }
            else if (value[i] == '-')
            {
                itog *= -1;
            }
        }

        if (MEP)
        {
            itog *= Degree(10, degree);
            Debug.Log($"<color=green>{itog}</color>");
        }
        
        return itog;
    }

    public static string IntToHex(int value)
    {
        if(value <= 16)
        {
            switch (value)
            {
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                case 16:
                    return "10";
                default:
                    return value.ToString();
            }
        }
        string info = "";

        int degree = 0;
        while(MathF.Pow(16, degree) < value)
        {
            degree++;
        }

        degree--;

        for(int i = degree; i > -1; i--)
        {
            float pow = MathF.Pow(16, i);
            switch ((int)(value / pow))
            {
                case 10:
                    info += "A";
                    break;
                case 11:
                    info += "B";
                    break;
                case 12:
                    info += "C";
                    break;
                case 13:
                    info += "D";
                    break;
                case 14:
                    info += "E";
                    break;
                case 15:
                    info += "F";
                    break;
                default:
                    info += ((int)(value / pow)).ToString();
                    break;
            }

            value %= (int)pow;
        }

        return info;
    }

    public static Color HexToColor(string hex)
    {
        Color color = new Color(1,1,1,1);

        while (hex.Length < 6)
        {
            hex += "0";
        }

        color.r = HexToInt(hex.Substring(0, 2)) / 255f;
        color.g = HexToInt(hex.Substring(2, 2)) / 255f;
        color.b =HexToInt(hex.Substring(4, 2)) / 255f;

        return color;
    }

    public static string ColorToHex(Color color)
    {
        string info = "";
        string hex = "";

        hex = IntToHex(Mathf.RoundToInt(color.r * 255f));
        if (hex.Length < 2)
        {
            hex = "0" + hex;
        }

        info += hex;

        hex = IntToHex(Mathf.RoundToInt(color.g * 255f));
        if (hex.Length < 2)
        {
            hex = "0" + hex;
        }

        info += hex;

        hex = IntToHex(Mathf.RoundToInt(color.b * 255f));
        if (hex.Length < 2)
        {
            hex = "0" + hex;
        }

        info += hex;

        return info;
    }

    public static int HexToInt(string hex)
    {
        hex = hex.ToUpper();

        int value = 0;

        for(int i = 0; i < hex.Length; i++)
        {
            value += GetNumber(hex[i]) * (int)Mathf.Pow(16, hex.Length - i-1);
        }

        return value;
    }

    public static int StringToInt(string value)
    {
        int itog = 0;
        int ten = 1;

        for (int i = value.Length - 1; i > -1; i--)
        {
            if (char.IsNumber(value[i]))
            {
                itog += GetNumber(value[i]) * ten;
                ten *= 10;
            }
            else if (value[i] == '.')
            {
                itog /= ten;
                ten = 1;
            }
            else if (value[i] == ',')
            {
                itog /= ten;
                ten = 1;
            }
            else if (value[i] == '-')
            {
                itog *= -1;
            }
        }

        return itog;
    }

    public static int GetNumber(char value)
    {
        switch (value)
        {
            case '0':
                return 0;
            case '1':
                return 1;
            case '2':
                return 2;
            case '3':
                return 3;
            case '4':
                return 4;
            case '5':
                return 5;
            case '6':
                return 6;
            case '7':
                return 7;
            case '8':
                return 8;
            case '9':
                return 9;
            case 'A':
                return 10;
            case 'B':
                return 11;
            case 'C':
                return 12;
            case 'D':
                return 13;
            case 'E':
                return 14;
            case 'F':
                return 15;
        }

        return 0;
    }

    public static int BooltoInt(bool boolean) => boolean ? 1 : 0;
}

public class ProceduralTools
{
    public static Vector2 Tangent(Vector2 position, string seed)
    {
        position += new Vector2(0.1f, 0.1f);

        Vector2 tangent = new Vector2(Mathf.Tan(Mathf.Sin(Vector2.Dot(position, new Vector2(12.9898f, 78.233f))) * 2333 + seed.GetHashCode()), Mathf.Tan(Mathf.Sin(Vector2.Dot(position, new Vector2(78.233f, 22.98f))) * 2666 + seed.GetHashCode()));

        return tangent.normalized;
    }

    public static float OctavianNoise(Vector2 position, string seed, float brightness, float scale, int count)
    {
        float noise = PerlinNoise(position * scale, seed, brightness);

        for (int i = 0; i < count; i++)
        {
            noise += PerlinNoise(position * 2 * (i + 1) * scale, seed, brightness) / 2 / (i + 1);
        }

        return noise;
    }

    public static float OctavianNoise(Vector2 position, string seed, float brightness, int count)
    {
        float noise = PerlinNoise(position, seed, brightness);

        for (int i = 0; i < count; i++)
        {
            noise += PerlinNoise(position * 2 * (i + 1), seed, brightness) / 2 / (i + 1);
        }

        return noise;
    }

    public static float PerlinNoise(Vector2 position, string seed, float brightness)
    {
        Vector2 zeroTangentPosition = new Vector2(Mathf.Floor(position.x), Mathf.Floor(position.y));
        Vector2 direction = position - zeroTangentPosition;

        Vector2 curved = new Vector2(Curve(direction.x), Curve(direction.y));

        float first = Vector2.Dot(direction, Tangent(zeroTangentPosition, seed));
        float second = Vector2.Dot(direction - new Vector2(1, 0), Tangent(zeroTangentPosition + new Vector2(1, 0), seed));
        float lerp1 = Mathf.Lerp(first, second, curved.x);

        first = Vector2.Dot(direction - new Vector2(0, 1), Tangent(zeroTangentPosition + new Vector2(0, 1), seed));
        second = Vector2.Dot(direction - new Vector2(1, 1), Tangent(zeroTangentPosition + new Vector2(1, 1), seed));
        float lerp2 = Mathf.Lerp(first, second, curved.x);

        return Mathf.Lerp(lerp1, lerp2, curved.y) + brightness;
    }

    public static float WhiteNoise(Vector2 position, string seed) => PseudoRandom(Mathf.Sin(Vector2.Dot(position, new Vector2(12.9898f, 78.233f))) * seed.GetHashCode());

    public static float PseudoRandom(float seed)
    {
        float[] simpleNumbers = new float[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

        if (seed < 0)
        {
            seed /= -simpleNumbers[NumberSumm((seed / 3).ToString()) % 10];
        }

        long index = NumberSumm(seed.ToString()) % 10;

        int count = 0;
        while (seed >= 1)
        {
            count++;
            seed = seed / simpleNumbers[index] + simpleNumbers[index] * 0.0005f;

            if (index >= simpleNumbers.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }

        return seed;
    }

    public static float Curve(float value)
    {
        return value * value * (3 - 2 * value);
    }

    private static int NumberSumm(string number)
    {
        int summ = 0;

        for (int i = 0; i < number.Length; i++)
        {
            if (char.IsNumber(number[i]))
            {
                summ += int.Parse(number[i].ToString());
            }
        }

        return summ;
    }
}
