﻿// Decompiled with JetBrains decompiler
// Type: SRPG.QuestBookmarkKakeraWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using GR;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SRPG
{
  [FlowNode.Pin(100, "クエスト選択", FlowNode.PinTypes.Output, 100)]
  [FlowNode.Pin(1, "Refresh", FlowNode.PinTypes.Input, 1)]
  public class QuestBookmarkKakeraWindow : MonoBehaviour, IFlowInterface
  {
    [SerializeField]
    private RectTransform QuestListParent;
    [SerializeField]
    private GameObject QuestListItemTemplate;
    private List<GameObject> mGainedQuests;

    public QuestBookmarkKakeraWindow()
    {
      base.\u002Ector();
    }

    public void Activated(int pinID)
    {
      if (pinID != 1)
        return;
      GameParameter.UpdateAll(((Component) this).get_gameObject());
    }

    private void Awake()
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.QuestListItemTemplate, (UnityEngine.Object) null))
        return;
      this.QuestListItemTemplate.SetActive(false);
    }

    public void Refresh(UnitParam unit, IEnumerable<QuestParam> quests)
    {
      if (unit == null || quests == null)
        return;
      DataSource.Bind<UnitParam>(((Component) this).get_gameObject(), unit);
      this.RefreshGainedQuests(unit, quests);
      GameParameter.UpdateAll(((Component) this).get_gameObject());
    }

    private void RefreshGainedQuests(UnitParam unit, IEnumerable<QuestParam> quests)
    {
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) this.QuestListItemTemplate, (UnityEngine.Object) null) || UnityEngine.Object.op_Equality((UnityEngine.Object) this.QuestListParent, (UnityEngine.Object) null) || (unit == null || !UnityEngine.Object.op_Inequality((UnityEngine.Object) QuestDropParam.Instance, (UnityEngine.Object) null)))
        return;
      QuestParam[] availableQuests = MonoSingleton<GameManager>.Instance.Player.AvailableQuests;
      foreach (QuestParam quest in quests)
        this.AddPanel(quest, availableQuests);
    }

    private void AddPanel(QuestParam questparam, QuestParam[] availableQuests)
    {
      if (questparam == null || questparam.IsMulti)
        return;
      GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate<GameObject>((M0) this.QuestListItemTemplate);
      SRPG_Button component1 = (SRPG_Button) gameObject.GetComponent<SRPG_Button>();
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component1, (UnityEngine.Object) null))
        component1.AddListener(new SRPG_Button.ButtonClickEvent(this.OnQuestSelect));
      this.mGainedQuests.Add(gameObject);
      Button component2 = (Button) gameObject.GetComponent<Button>();
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component2, (UnityEngine.Object) null))
      {
        bool flag1 = questparam.IsDateUnlock(-1L);
        bool flag2 = Array.Find<QuestParam>(availableQuests, (Predicate<QuestParam>) (p => p == questparam)) != null;
        ((Selectable) component2).set_interactable(flag1 && flag2);
      }
      DataSource.Bind<QuestParam>(gameObject, questparam);
      gameObject.get_transform().SetParent((Transform) this.QuestListParent, false);
      gameObject.SetActive(true);
    }

    private void OnQuestSelect(SRPG_Button button)
    {
      QuestParam quest = DataSource.FindDataOfClass<QuestParam>(this.mGainedQuests[this.mGainedQuests.IndexOf(((Component) button).get_gameObject())], (QuestParam) null);
      if (quest == null)
        return;
      if (!quest.IsDateUnlock(-1L))
        UIUtility.NegativeSystemMessage((string) null, LocalizedText.Get("sys.DISABLE_QUEST_DATE_UNLOCK"), (UIUtility.DialogResultEvent) null, (GameObject) null, false, -1);
      else if (Array.Find<QuestParam>(MonoSingleton<GameManager>.Instance.Player.AvailableQuests, (Predicate<QuestParam>) (p => p == quest)) == null)
      {
        UIUtility.NegativeSystemMessage((string) null, LocalizedText.Get("sys.DISABLE_QUEST_CHALLENGE"), (UIUtility.DialogResultEvent) null, (GameObject) null, false, -1);
      }
      else
      {
        GlobalVars.SelectedQuestID = quest.iname;
        FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
      }
    }
  }
}
