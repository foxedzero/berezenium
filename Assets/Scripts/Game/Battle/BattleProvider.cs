using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleProvider : MonoBehaviour
{
    //[SerializeField] private ActBar ActBar;
    //[SerializeField] private CameraShake CameraShake;
    //[SerializeField] private GameObject WorldTextPrefab;
    //[SerializeField] private GameObject FightButton;

    //[SerializeField] private BattlerSpawner Spawner;

    //[SerializeField] private Battler[] PlayerSide;
    //[SerializeField] private Battler[] EnemySide;

    //[SerializeField] private Material[] ActionMaterial;

    //private bool BattleLock = false;

    //public Battler[] _PlayerSide => PlayerSide;
    //public Battler[] _EnemySide => EnemySide;

    //public Material[] _ActionMaterial => ActionMaterial;

    //private void Awake()
    //{
    //    SaveData saveData = SaveManager._Instance._SaveData;

    //    Dictionary<string, string> info = StaticTools.GetParameters(saveData.BattleInfo);
    //    if (info["State"] == "End")
    //    {
    //        Debug.Log("<color=red>Запущен законченный бой</color>");

    //        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    //        return;
    //    }

    //    string[] enemies = info["Enemy"].Split(";");
    //    EnemySide = new Battler[Mathf.Min(enemies.Length, 6)];
    //    if(EnemySide.Length == 0)
    //    {
    //        Debug.Log("<color=red>В бою нет противников</color>");

    //        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    //        return;
    //    }
    //    for(int i = 0; i < EnemySide.Length; i++)
    //    {
    //        EnemySide[i] = Spawner.Spawn(enemies[i]);
    //    }

    //    PlayerSide = new Battler[Mathf.Min(saveData.BearDefenders.Length, 6)];
    //    if (PlayerSide.Length == 0)
    //    {
    //        Debug.Log("<color=red>В бою нет игрока</color>");

    //        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    //        return;
    //    }
    //    for (int i = 0; i < PlayerSide.Length; i++)
    //    {
    //        Bear bear = new Bear();
    //        bear._SaveInfo = saveData.Bears[saveData.BearDefenders[i]];
    //        PlayerSide[i] = Spawner.Spawn(bear);
    //    }
    //}

    //private void Start()
    //{
    //    Time.timeScale = 1;

    //    SetOnBoard();
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F3))
    //    {
    //        Fight();
    //    }
    //}

    //public void SetOnBoard()
    //{
    //    foreach (Battler battler in PlayerSide)
    //    {
    //        battler.SetBattleUI(true);

    //        battler._PowerBonus[0] = battler._PowerBonus[1];
    //        battler._PowerBonus[1] = 0;

    //        battler._Speed = Random.Range(battler._MinMaxSpeed[0], battler._MinMaxSpeed[1] + 1);

    //        if (battler._Stunned)
    //        {
    //            battler.SetAnimation(Battler.BattleAnimation.TakeDamage);
    //            battler._Action = Battler.Actions.Attack;
    //        }
    //        else
    //        {
    //            battler.SetAnimation(Battler.BattleAnimation.Pose);
    //        }

    //        if(battler is PervoprohodecBattler)
    //        {
    //            (battler as PervoprohodecBattler)._Streak = 0;
    //        }

    //        battler.transform.localEulerAngles = new Vector3(0, 90, 0);

    //        battler._Target = null;

    //        battler.NewRound();
    //    }

    //    foreach (Battler battler in EnemySide)
    //    {
    //        battler.SetBattleUI(true);

    //        battler._PowerBonus[0] = battler._PowerBonus[1];
    //        battler._PowerBonus[1] = 0;

    //        battler._Speed = Random.Range(battler._MinMaxSpeed[0], battler._MinMaxSpeed[1] + 1);

    //        if (battler._Stunned)
    //        {
    //            battler.SetAnimation(Battler.BattleAnimation.TakeDamage);
    //            battler._Action = Battler.Actions.Attack;
    //        }
    //        else
    //        {
    //            battler.SetAnimation(Battler.BattleAnimation.Pose);
    //        }

    //        if (battler is PervoprohodecBattler)
    //        {
    //            (battler as PervoprohodecBattler)._Streak = 0;
    //        }

    //        battler.transform.localEulerAngles = new Vector3(0, 270, 0);

    //        battler._Target = null;

    //        battler.NewRound();
    //    }

    //    StartCoroutine(SetOnTheBoard());
    //}

    //public void Fight()
    //{
    //    if (!BattleLock)
    //    {
    //        StartCoroutine(FightStage());
    //    }
    //}

    //public Vector3 BattlerPosition(int index, int count)
    //{
    //    switch (count)
    //    {
    //        case 1:
    //            return new Vector3(-9, 0, 0);
    //        case 2:
    //            switch (index)
    //            {
    //                case 0:
    //                    return new Vector3(-9, 0, 2);
    //                case 1:
    //                    return new Vector3(-9, 0, -2);
    //            }
    //            break;
    //        case 3:
    //            switch (index)
    //            {
    //                case 0:
    //                    return new Vector3(-7, 0, 2);
    //                case 1:
    //                    return new Vector3(-7, 0, -2);
    //                case 2:
    //                    return new Vector3(-10, 0, 0);
    //            }
    //            break;
    //        case 4:
    //            switch (index)
    //            {
    //                case 0:
    //                    return new Vector3(-7, 0, 2);
    //                case 1:
    //                    return new Vector3(-7, 0, -2);
    //                case 2:
    //                    return new Vector3(-10, 0, 3);
    //                case 3:
    //                    return new Vector3(-10, 0, -3);
    //            }
    //            break;
    //        case 5:
    //            switch (index)
    //            {
    //                case 0:
    //                    return new Vector3(-7, 0, 0);
    //                case 1:
    //                    return new Vector3(-7, 0, 3);
    //                case 2:
    //                    return new Vector3(-7, 0, -3);
    //                case 3:
    //                    return new Vector3(-10, 0, -3);
    //                case 4:
    //                    return new Vector3(-10, 0, 3);
    //            }
    //            break;
    //        case 6:
    //            switch (index)
    //            {
    //                case 0:
    //                    return new Vector3(-7, 0, 0);
    //                case 1:
    //                    return new Vector3(-7, 0, 3);
    //                case 2:
    //                    return new Vector3(-7, 0, -3);
    //                case 3:
    //                    return new Vector3(-10, 0, -3);
    //                case 4:
    //                    return new Vector3(-10, 0, 3);
    //                case 5:
    //                    return new Vector3(-10, 0, 0);
    //            }
    //            break;
    //    }

    //    return new Vector3(-9, 0, 0);
    //}

    //private IEnumerator FightStage()
    //{
    //    BattleLock = true;

    //    FightButton.SetActive(false);

    //    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    //    Battler[] activeBattlers = new Battler[0];
    //    foreach(Battler battler in PlayerSide)
    //    {
    //        battler.SetBattleUI(false);

    //        if(battler._Target != null && !battler._Stunned)
    //        {
    //            activeBattlers = StaticTools.ExpandMassive(activeBattlers, battler);

    //            if(battler._Action == Battler.Actions.Evade)
    //            {
    //                battler._Target._Target = battler;
    //            }
    //        }

    //        battler._Stunned = false;
    //    }
    //    foreach (Battler battler in EnemySide)
    //    {
    //        battler.SetBattleUI(false);

    //        if (battler._Target != null && !battler._Stunned)
    //        {
    //            activeBattlers = StaticTools.ExpandMassive(activeBattlers, battler);
    //        }

    //        battler._Stunned = false;
    //    }

    //    bool hold = false;

    //    while (activeBattlers.Length > 0)
    //    {
    //        foreach(Battler battler in activeBattlers)
    //        {
    //            battler.transform.position += (battler._Target.transform.position - battler.transform.position).normalized * 3 * battler._Speed * Time.deltaTime;
    //            battler.transform.rotation = Quaternion.LookRotation(battler._Target.transform.position - battler.transform.position);
    //            battler.SetAnimation(Battler.BattleAnimation.Strafe);

    //            if (Vector3.Distance(battler._Target.transform.position, battler.transform.position) < 1 || (battler._FarAttack && battler._Action == Battler.Actions.Attack) )
    //            {
    //                Battler forcing = battler;
    //                float forceDistance = 0;

    //                battler._Target.transform.rotation = Quaternion.LookRotation(-(battler._Target.transform.position - battler.transform.position));

    //                int success = 0;
    //                if (battler._Action == Battler.Actions.Heal)
    //                {
    //                    if (StaticTools.Contains(PlayerSide, battler))
    //                    {
    //                        ActBar.Activate(0, Random.Range(battler._MinMaxKrit[0], battler._MinMaxKrit[1]));

    //                        while (hold)
    //                        {
    //                            if (!Input.GetKey(KeyCode.Return))
    //                            {
    //                                hold = false;
    //                            }
    //                            yield return waitForEndOfFrame;
    //                        }
    //                        while (!Input.GetKey(KeyCode.Return))
    //                        {
    //                            yield return waitForEndOfFrame;
    //                        }
    //                        hold = true;

    //                        success = ActBar.Stop();
    //                    }
    //                    else
    //                    {
    //                        success = 1;
    //                        if (Random.Range(battler._MinMaxKrit[0], battler._MinMaxKrit[1]) > Random.Range(0, 100f))
    //                        {
    //                            success = 2;
    //                        }
    //                    }

    //                    Action(ref activeBattlers, battler, battler._Target, success, ref forcing, ref forceDistance);

                       
    //                }
    //                else
    //                {
    //                    battler._Target.SetAnimation(Battler.BattleAnimation.Strafe);

    //                    bool versus = !battler._Target._Stunned && ((battler._Target._Target == battler && StaticTools.Contains(activeBattlers, battler._Target)) || battler._Target._SideAction);
    //                    if (versus)
    //                    {
    //                        Battler playerSide = battler;
    //                        Battler enemySide = battler._Target;
    //                        if (!StaticTools.Contains(PlayerSide, playerSide))
    //                        {
    //                            Battler bufer = playerSide;
    //                            playerSide = enemySide;
    //                            enemySide = bufer;
    //                        }

    //                        if (battler._Target._Target == battler)
    //                        {
    //                            activeBattlers = StaticTools.RemoveFromMassive(activeBattlers, battler._Target);

    //                            if(battler._FarAttack && battler._Target._FarAttack)
    //                            {

    //                            }
    //                            else if (battler._FarAttack && battler._Action == Battler.Actions.Attack)
    //                            {
    //                                activeBattlers = StaticTools.ExpandMassive(activeBattlers, battler._Target);
    //                            }
    //                            else if (battler._Target._FarAttack && battler._Target._Action == Battler.Actions.Attack)
    //                            {
    //                                activeBattlers = StaticTools.ExpandMassive(activeBattlers, battler);
    //                            }
    //                        }

    //                        float hard = 0;
    //                        hard = enemySide._Power / playerSide._Power;
    //                        if (!(playerSide is PasechnikBattler) && playerSide._Action == Battler.Actions.Attack && enemySide._Action == Battler.Actions.Attack)
    //                        {
    //                            if (playerSide is ConstructorBattler)
    //                            {
    //                                if (playerSide._Target == enemySide)
    //                                {
    //                                    hard *= 2;
    //                                }
    //                            }
    //                            else
    //                            {
    //                                hard *= 2;
    //                            }
    //                        }
    //                        if (playerSide is ProgramistBattler)
    //                        {
    //                            hard *= 0.85f;
    //                        }

    //                        bool enemyKrit = Random.Range(enemySide._MinMaxKrit[0], enemySide._MinMaxKrit[1]) > Random.Range(0, 100f);
    //                        if (enemyKrit)
    //                        {
    //                            hard = 5;
    //                        }

    //                        ActBar.Activate(hard, Random.Range(playerSide._MinMaxKrit[0], playerSide._MinMaxKrit[1]));

    //                        while (hold)
    //                        {
    //                            if (!Input.GetKey(KeyCode.Return))
    //                            {
    //                                hold = false;
    //                            }
    //                            yield return waitForEndOfFrame;
    //                        }
    //                        while (!Input.GetKey(KeyCode.Return))
    //                        {
    //                            yield return waitForEndOfFrame;
    //                        }
    //                        hold = true;

    //                        success = ActBar.Stop();

    //                        if (success == 0)
    //                        {
    //                            Action(ref activeBattlers, enemySide, playerSide, (enemyKrit ? 2 : 1), ref forcing, ref forceDistance);
    //                        }
    //                        else
    //                        {
    //                            Action(ref activeBattlers, playerSide, enemySide, success, ref forcing, ref forceDistance);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (StaticTools.Contains(PlayerSide, battler))
    //                        {
    //                            ActBar.Activate(0, Random.Range(battler._MinMaxKrit[0], battler._MinMaxKrit[1]));

    //                            while (hold)
    //                            {
    //                                if (!Input.GetKey(KeyCode.Return))
    //                                {
    //                                    hold = false;
    //                                }
    //                                yield return waitForEndOfFrame;
    //                            }
    //                            while (!Input.GetKey(KeyCode.Return))
    //                            {
    //                                yield return waitForEndOfFrame;
    //                            }
    //                            hold = true;

    //                            success = ActBar.Stop();
    //                        }
    //                        else
    //                        {
    //                            success = 1;
    //                            if (Random.Range(battler._MinMaxKrit[0], battler._MinMaxKrit[1]) > Random.Range(0, 100f))
    //                            {
    //                                success = 2;
    //                            }
    //                        }

    //                        Action(ref activeBattlers, battler, battler._Target, success, ref forcing, ref forceDistance);
    //                    }
                      
    //                }

    //                forceDistance = Mathf.Max(Random.Range(1.2f, 1.6f), forceDistance);
    //                CameraShake.Shake(forceDistance * 0.2f, forceDistance);
    //                while (forceDistance > 0)
    //                {
    //                    float speed = Mathf.Max(0.1f, forceDistance) * 10 * Time.unscaledDeltaTime;

    //                    forcing.transform.position -= forcing.transform.forward * speed;
    //                    forceDistance-= speed;

    //                    yield return waitForEndOfFrame;
    //                }

    //                activeBattlers = StaticTools.RemoveFromMassive(activeBattlers, battler);

    //                yield return new WaitForSecondsRealtime(0.25f);
    //                break;
    //            }
    //        }

    //        yield return waitForEndOfFrame;
    //    }

    //    yield return new WaitForSecondsRealtime(0.5f);

    //    SetOnBoard();
    //}

    //private void Action(ref Battler[] activeBattlers, Battler first, Battler second, int success, ref Battler forcing, ref float forceDistance)
    //{
    //    WorldText worldText = Instantiate(WorldTextPrefab, null).GetComponent<WorldText>();
    //    Vector3 position = (first.transform.position + second.transform.position) / 2 + Vector3.up;
    //    position += (CameraShake.transform.position - position).normalized * 2;
    //    worldText._Text = ((int)(first._Power * success * 10) / 10f).ToString();
    //    worldText.transform.position = position;
    //    worldText.transform.LookAt(CameraShake.transform);

    //    switch (first._Action)
    //    {
    //        case Battler.Actions.Attack:
    //            first.SetAnimation(Battler.BattleAnimation.Attack);
    //            first.PlayEffect(Battler.BattleEffects.Attack, first._Power * success * 0.25f);

    //            if (!(second._FarAttack && StaticTools.Contains(activeBattlers, second) && second._Action == Battler.Actions.Attack))
    //            {
    //                second.SetAnimation(Battler.BattleAnimation.TakeDamage);
    //                second.TakeDamage(first._Power * success);

    //                worldText._Text = $"-{(int)(first._Power * success * 10) / 10f} !";
    //                worldText._Color = Color.red;

    //                if (success == 2)
    //                {
    //                    if (first is PasechnikBattler)
    //                    {
    //                        second._Stunned = true;
    //                        activeBattlers = StaticTools.RemoveFromMassive(activeBattlers, second);
    //                    }
    //                    else if (first is PervoprohodecBattler && (first as PervoprohodecBattler)._Streak < 3)
    //                    {
    //                        if(StaticTools.Contains(PlayerSide, first))
    //                        {
    //                            first._Target = EnemySide[Random.Range(0, EnemySide.Length)];
    //                        }
    //                        else
    //                        {
    //                            first._Target = PlayerSide[Random.Range(0, PlayerSide.Length)];
    //                        }
    //                        activeBattlers = StaticTools.ExpandMassive(activeBattlers, first);

    //                        (first as PervoprohodecBattler)._Streak++;
    //                    }
    //                }

    //                forcing = second;
    //                forceDistance = Mathf.Min(8, first._Power * success * 0.15f);
    //            }
    //            else
    //            {
    //                worldText._Text = "ОТБИТО !";
    //                worldText._Color = Color.red;

    //                if (second._Target == first && second._Action == Battler.Actions.Attack && StaticTools.Contains(activeBattlers, second))
    //                {
    //                    second.PlayEffect(Battler.BattleEffects.Attack, second._Power * 0.25f);
    //                }
    //            }

    //            if (first._FarAttack && second._Target == first)
    //            {
    //                activeBattlers = StaticTools.RemoveFromMassive(activeBattlers, second);
    //            }
    //            break;
    //        case Battler.Actions.Block:
    //            first.SetAnimation(Battler.BattleAnimation.Block);
    //            first.PlayEffect(Battler.BattleEffects.Block, first._Power * success * 0.25f);

    //            worldText._Text = "БЛОК !";
    //            worldText._Color = Color.blue;

    //            forcing = second;
    //            forceDistance = Mathf.Min(8, first._Power * success * 0.15f);

    //            if (first._FarAttack && second._Target == first)
    //            {
    //                activeBattlers = StaticTools.RemoveFromMassive(activeBattlers, second);
    //            }

    //            switch (second._Action)
    //            {
    //                case Battler.Actions.Attack:
    //                    second.SetAnimation(Battler.BattleAnimation.Attack);

    //                    if (second._FarAttack && second._Target == first)
    //                    {
    //                        second.PlayEffect(Battler.BattleEffects.Attack, second._Power * 0.25f);
    //                    }
    //                    break;
    //                case Battler.Actions.Block:
    //                    second.SetAnimation(Battler.BattleAnimation.Block);
    //                    break;
    //                case Battler.Actions.Evade:
    //                    second.SetAnimation(Battler.BattleAnimation.Evade);
    //                    break;
    //            }

    //            if(success == 2)
    //            {
    //                second._Stunned = true;
    //                activeBattlers = StaticTools.RemoveFromMassive(activeBattlers, second);
    //            }
    //            break;
    //        case Battler.Actions.Evade:
    //            first.SetAnimation(Battler.BattleAnimation.Evade);

    //            worldText._Text = "УВОРОТ !";
    //            worldText._Color = Color.cyan;

    //            first._PowerBonus[1] += 1;

    //            forcing = first;
    //            forceDistance = Mathf.Min(8, first._Power * success * 0.15f);

    //            if (first._FarAttack && second._Target == first)
    //            {
    //                activeBattlers = StaticTools.RemoveFromMassive(activeBattlers, second);
    //            }

    //            switch (second._Action)
    //            {
    //                case Battler.Actions.Attack:
    //                    second.SetAnimation(Battler.BattleAnimation.Attack);

    //                    if (second._FarAttack && second._Target == first)
    //                    {
    //                        second.PlayEffect(Battler.BattleEffects.Attack, second._Power * 0.25f);
    //                    }
    //                    break;
    //                case Battler.Actions.Block:
    //                    second.SetAnimation(Battler.BattleAnimation.Block);
    //                    break;
    //                case Battler.Actions.Evade:
    //                    second.SetAnimation(Battler.BattleAnimation.Evade);
    //                    break;
    //            }

    //            if (success == 2)
    //            {
    //                first._PowerBonus[1] += 1;
    //            }
    //            break;
    //        case Battler.Actions.Heal:
    //            second.SetAnimation(Battler.BattleAnimation.Pose);

    //            worldText._Text = $"+{(int)(first._Power * success * 10) / 10f} !";
    //            worldText._Color = Color.green;

    //            forcing = second;
    //            forceDistance = 0.25f;

    //            second._Health += first._Power * success;

    //            first.SetAnimation(Battler.BattleAnimation.Attack);
    //            first.PlayEffect(Battler.BattleEffects.Attack, first._Power * success * 0.25f);
    //            break;
    //    }
    //}

    //private IEnumerator SetOnTheBoard()
    //{
    //    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    //    int ready = 0;
    //    for (int i = 0; i < PlayerSide.Length; i++)
    //    {
    //        Vector3 position = BattlerPosition(i, PlayerSide.Length);
    //        if (PlayerSide[i].transform.position == position)
    //        {
    //            ready++;
    //        }
    //    }

    //    for (int i = 0; i < EnemySide.Length; i++)
    //    {
    //        Vector3 position = -BattlerPosition(i, EnemySide.Length);
    //        if (EnemySide[i].transform.position == position)
    //        {
    //            ready++;
    //        }
    //    }

    //    while (ready < PlayerSide.Length + EnemySide.Length)
    //    {
    //        for(int i = 0; i < PlayerSide.Length; i++)
    //        {
    //            Vector3 position = BattlerPosition(i, PlayerSide.Length);
    //            if (PlayerSide[i].transform.position == position)
    //            {
    //                continue;
    //            }

    //            PlayerSide[i].transform.position += ( position - PlayerSide[i].transform.position).normalized * 15 * Time.deltaTime;
    //            if (Vector3.Distance(position, PlayerSide[i].transform.position) < 0.5f)
    //            {
    //                PlayerSide[i].transform.position = position;
    //                ready++;
    //            }
    //        }

    //        for(int i = 0; i < EnemySide.Length; i++)
    //        {
    //            Vector3 position = -BattlerPosition(i, EnemySide.Length);
    //            if (EnemySide[i].transform.position == position)
    //            {
    //                continue;
    //            }

    //            EnemySide[i].transform.position += (position - EnemySide[i].transform.position).normalized * 15 * Time.deltaTime;
    //            if (Vector3.Distance(position, EnemySide[i].transform.position) < 0.5f)
    //            {
    //                EnemySide[i].transform.position = position;
    //                ready++;
    //            }
    //        }

    //        yield return waitForEndOfFrame;
    //    }

    //    FightButton.SetActive(true);

    //    BattleLock = false;
    //}
}
