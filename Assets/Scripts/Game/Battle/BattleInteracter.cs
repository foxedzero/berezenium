using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleInteracter : MonoBehaviour
{
    //[SerializeField] private Camera Camera;
    //[SerializeField] private Grid Grid;
    //[SerializeField] private BattleProvider BattleProvider;
    //[SerializeField] private LayerMask LayerMask;

    //[SerializeField] private BattlerInfo BattlerInfo;

    //[SerializeField] private BattleArrow BattleArrow;

    //[SerializeField] private Battler Pointed;
    //[SerializeField] private Battler OverHolder;
    //[SerializeField] private Battler OnHold;
   
    //private NeedCursorOrder NeedCursorOrder = new NeedCursorOrder();

    //public Battler _OverHolder
    //{
    //    get
    //    {
    //        return OverHolder;
    //    }
    //    set
    //    {
    //        OverHolder = value;
    //    }
    //}
    //public Battler _Pointed
    //{
    //    get
    //    {
    //        return Pointed;
    //    }
    //    set
    //    {
    //        if(Pointed == value)
    //        {
    //            return;
    //        }

    //        if(Pointed != null)
    //        {
    //            Pointed.SetIndicate(false);
    //        }

    //        Pointed = value;

    //        if(Pointed != null)
    //        {
    //            Pointed.SetIndicate(true);

    //            BattlerInfo.SetBattler(value);
    //        }

    //        if (OnHold == null)
    //        {
    //            BattleArrow.Show(false);
    //        }
    //    }
    //}

    //private void Start()
    //{
    //    CursorManager.SetNeedMouse(NeedCursorOrder, false);
    //}

    //private void Update()
    //{
    //    if (OnHold != null)
    //    {
    //        BattleArrow.SetColor(OnHold._Action);
    //        if (Pointed != null)
    //        {
    //            BattleArrow.Set(OnHold.transform.position, Pointed.transform.position - (Pointed.transform.position - OnHold.transform.position).normalized * 0.75f);
    //        }
    //        else
    //        {
    //            BattleArrow.Set(OnHold.transform.position, Grid._RealPoint);
    //        }
    //        BattleArrow.Show(true);

    //        if (Input.GetKeyUp(KeyCode.Mouse0))
    //        {
    //            BattleArrow.Show(false);

    //            if (OnHold == Pointed)
    //            {
    //                OnHold.NextAction();
    //            }

    //            if(Pointed != null && Pointed != OnHold)
    //            {
    //                if (StaticTools.Contains(BattleProvider._PlayerSide, Pointed) && StaticTools.Contains(BattleProvider._PlayerSide, OnHold))
    //                {
    //                    if(OnHold._Action == Battler.Actions.Heal)
    //                    {
    //                        OnHold._Target = Pointed;
    //                    }
    //                }
    //                else
    //                {
    //                    OnHold._Target = Pointed;
    //                }
    //            }

    //            OnHold = null;
    //        }
    //    }

    //    if (CheckClick())
    //    {
    //        RaycastHit hit;
    //        if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask))
    //        {
    //            Battler battler = hit.transform.GetComponentInParent<Battler>();
    //            if (battler != null)
    //            {
    //                _Pointed = battler;
    //            }
    //            else
    //            {
    //                _Pointed = OverHolder;
    //            }
    //        }
    //        else
    //        {
    //            _Pointed = OverHolder;
    //        }
    //    }
    //    else
    //    {
    //        _Pointed = OverHolder;
    //    }

    //    if (Pointed != null && Input.GetKeyDown(KeyCode.Mouse0) && !Pointed._Stunned)
    //    {
    //        BattleArrow.Set(Pointed.transform.position, Pointed.transform.position + Vector3.right * 2);
    //        OnHold = Pointed;
    //    }

    //    if (OnHold == null && Pointed != null)
    //    {
    //        if (Pointed._Target != null)
    //        {
    //            BattleArrow.SetColor(Pointed._Action);
    //            BattleArrow.Set(Pointed.transform.position, Pointed._Target.transform.position - (Pointed._Target.transform.position - Pointed.transform.position).normalized * 0.75f);
    //            BattleArrow.Show(true);
    //        }
    //        else
    //        {
    //            BattleArrow.Show(false);
    //        }
    //    }
    //}

    //private bool CheckClick()
    //{
    //    PointerEventData eventData = new PointerEventData(EventSystem.current);
    //    eventData.position = Input.mousePosition;
    //    List<RaycastResult> results = new List<RaycastResult>(0);
    //    EventSystem.current.RaycastAll(eventData, results);

    //    if (results.Count == 0)
    //    {
    //        return true;
    //    }

    //    return false;
    //}
}
