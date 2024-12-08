using UnityEngine;

namespace WindowInterfaces
{
    public interface IMoveable
    {
        void RightMouse();
        void Select();
        void Move(Vector2 newPosition);

        WindowDrag _Dragger { get; }
    }

    public interface IMetaVisible { }

    public interface INotAllClosable { }

    public interface ISingleOne
    {
        void Close();
    }
}

