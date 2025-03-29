using UnityEngine;

public class Design : MonoBehaviour
{
    public enum ColorType { Main, NoUse, Use, Informational };
    public static Color _MainColor => new Color(0.1215686f, 0.1215686f, 0.1215686f, 1);
    public static Color _NoUseColor => new Color(0.2f, 0.2f, 0.2f, 1);
    public static Color _UseColor => new Color(0.3f, 0.3f, 0.3f, 1);
    public static Color _InformationalColor => new Color(1, 1, 1, 0.8f);
}
