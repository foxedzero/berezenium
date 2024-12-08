using UnityEngine;
using UnityEngine.UI;

    public abstract class Window : MonoBehaviour
    {
        [SerializeField] protected RectTransform RectTransform;
        [SerializeField] protected Text Label;
        [SerializeField] protected GameObject Shadow;
        [SerializeField] protected int Index = 0;
        protected WindowLister Lister = null;

        public Transform _Canvas => Lister._Canvas;
        public RectTransform _Rect => RectTransform;
        public virtual string _Label => "окно";
        public virtual int _Index
        {
            get
            {
                return Index;
            }
            set
            {
                Lister.ChangeWindowIndex(this, value);
            }
        }
        public bool _Current => this == Lister._Current;

        public virtual void SetLister(WindowLister lister)
        {
            Lister = lister;

            Lister.AddWindow(this);

            if (Label != null)
            {
                Label.text = _Label;
            }
        }

        public void DirectSetIndex(int value)
        {
            Index = value;

            if (Label != null)
            {
                Label.text = _Label;
            }
        }

        public virtual void Close()
        {
            Lister.RemoveWindow(this);

            Destroy(gameObject);
        }

        public virtual void Select()
        {
            if (Lister._Current == this)
            {
                return;
            }

        Shadow.SetActive(true);

            Lister.Select(this);
        }

        public virtual void Deselect()
    {
        Shadow.SetActive(false);
        }

        public virtual void RightMouse()
        {
        UserInteract.AskVariants(_Label, new string[] { "закрыть окно" }, new int[] { 0 }, RightMouseActions);
        }

        public virtual void RightMouseActions(int index)
        {
            switch (index)
            {
                case 0:
                Close();
                    break;
            }
        }
    }
