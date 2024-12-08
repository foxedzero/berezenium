using UnityEngine;

public class Design : MonoBehaviour
{
    public enum ColorType { Main, NoUse, Use, Informational };
    public static Color _MainColor => new Color(0.8470588f, 0.8235294f, 0.7607843f, 1);
    public static Color _NoUseColor => new Color(0.8078431f, 0.7764706f, 0.6862745f, 1);
    public static Color _UseColor => new Color(0.6509804f, 0.6117647f, 0.4862745f, 1);
    public static Color _InformationalColor => new Color(0, 0, 0, 0.5882353f);
}
