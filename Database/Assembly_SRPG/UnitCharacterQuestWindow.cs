﻿// Decompiled with JetBrains decompiler
// Type: SRPG.UnitCharacterQuestWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE644F5D-682F-4D6E-964D-A0DD77A288F7
// Assembly location: C:\Users\André\Desktop\Assembly-CSharp.dll

using GR;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SRPG
{
  [FlowNode.Pin(10, "リスト切り替え", FlowNode.PinTypes.Input, 10)]
  [FlowNode.Pin(100, "クエストが選択された", FlowNode.PinTypes.Output, 100)]
  public class UnitCharacterQuestWindow : MonoBehaviour, IFlowInterface
  {
    public UnitData CurrentUnit;
    public Transform QuestList;
    public GameObject StoryQuestItemTemplate;
    public GameObject StoryQuestDisableItemTemplate;
    public GameObject PieceQuestItemTemplate;
    public GameObject PieceQuestDisableItemTemplate;
    public GameObject QuestDetailTemplate;
    public string DisableFlagName;
    public GameObject CharacterImage;
    private List<QuestParam> mQuestList;
    private List<GameObject> mStoryQuestListItems;
    private List<GameObject> mPieceQuestListItems;
    private GameObject mQuestDetail;
    public string PieceQuestWorldId;
    public Image ListToggleButton;
    public Sprite StoryListSprite;
    public Sprite PieceListSprite;
    private bool mIsStoryList;
    private bool mListRefreshing;
    private bool mIsRestore;

    public UnitCharacterQuestWindow()
    {
      base.\u002Ector();
    }

    public bool IsRestore
    {
      get
      {
        return this.mIsRestore;
      }
      set
      {
        this.mIsRestore = value;
      }
    }

    private void Start()
    {
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.StoryQuestItemTemplate, (UnityEngine.Object) null))
        this.StoryQuestItemTemplate.get_gameObject().SetActive(false);
      if (this.IsRestore)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        UnitCharacterQuestWindow.\u003CStart\u003Ec__AnonStorey389 startCAnonStorey389 = new UnitCharacterQuestWindow.\u003CStart\u003Ec__AnonStorey389();
        // ISSUE: reference to a compiler-generated field
        startCAnonStorey389.lastQuestName = GlobalVars.LastPlayedQuest.Get();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (startCAnonStorey389.lastQuestName != null && !string.IsNullOrEmpty(startCAnonStorey389.lastQuestName))
        {
          // ISSUE: reference to a compiler-generated method
          QuestParam questParam = Array.Find<QuestParam>(MonoSingleton<GameManager>.Instance.Quests, new Predicate<QuestParam>(startCAnonStorey389.\u003C\u003Em__436));
          if (questParam != null && !string.IsNullOrEmpty(questParam.ChapterID))
            this.mIsStoryList = !(questParam.world == this.PieceQuestWorldId);
        }
      }
      this.UpdateToggleButton();
      this.RefreshQuestList();
    }

    private void CreateStoryList()
    {
      this.mQuestList.Clear();
      this.mQuestList = this.CurrentUnit.FindCondQuests();
      UnitData.CharacterQuestParam[] charaEpisodeList = this.CurrentUnit.GetCharaEpisodeList();
      QuestParam[] availableQuests = MonoSingleton<GameManager>.Instance.Player.AvailableQuests;
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      UnitCharacterQuestWindow.\u003CCreateStoryList\u003Ec__AnonStorey38A listCAnonStorey38A = new UnitCharacterQuestWindow.\u003CCreateStoryList\u003Ec__AnonStorey38A();
      // ISSUE: reference to a compiler-generated field
      listCAnonStorey38A.\u003C\u003Ef__this = this;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      for (listCAnonStorey38A.i = 0; listCAnonStorey38A.i < this.mQuestList.Count; ++listCAnonStorey38A.i)
      {
        // ISSUE: reference to a compiler-generated field
        bool flag1 = this.mQuestList[listCAnonStorey38A.i].IsDateUnlock(-1L);
        // ISSUE: reference to a compiler-generated method
        bool flag2 = Array.Find<QuestParam>(availableQuests, new Predicate<QuestParam>(listCAnonStorey38A.\u003C\u003Em__437)) != null;
        // ISSUE: reference to a compiler-generated field
        bool flag3 = this.mQuestList[listCAnonStorey38A.i].state == QuestStates.Cleared;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        bool flag4 = charaEpisodeList[listCAnonStorey38A.i] != null && charaEpisodeList[listCAnonStorey38A.i].IsAvailable && this.CurrentUnit.IsChQuestParentUnlocked(this.mQuestList[listCAnonStorey38A.i]);
        bool flag5 = flag1 && flag2 && !flag3;
        GameObject gameObject;
        if (flag4 || flag3)
        {
          gameObject = (GameObject) UnityEngine.Object.Instantiate<GameObject>((M0) this.StoryQuestItemTemplate);
          Button component = (Button) gameObject.GetComponent<Button>();
          if (!UnityEngine.Object.op_Equality((UnityEngine.Object) component, (UnityEngine.Object) null))
            ((Selectable) component).set_interactable(flag5);
          else
            continue;
        }
        else
          gameObject = (GameObject) UnityEngine.Object.Instantiate<GameObject>((M0) this.StoryQuestDisableItemTemplate);
        if (!UnityEngine.Object.op_Equality((UnityEngine.Object) gameObject, (UnityEngine.Object) null))
        {
          gameObject.SetActive(true);
          gameObject.get_transform().SetParent(this.QuestList, false);
          // ISSUE: reference to a compiler-generated field
          DataSource.Bind<QuestParam>(gameObject, this.mQuestList[listCAnonStorey38A.i]);
          DataSource.Bind<UnitData>(gameObject, this.CurrentUnit);
          DataSource.Bind<UnitParam>(gameObject, this.CurrentUnit.UnitParam);
          ListItemEvents component = (ListItemEvents) gameObject.GetComponent<ListItemEvents>();
          component.OnSelect = new ListItemEvents.ListItemEvent(this.OnQuestSelect);
          component.OnOpenDetail = new ListItemEvents.ListItemEvent(this.OnOpenItemDetail);
          component.OnCloseDetail = new ListItemEvents.ListItemEvent(this.OnCloseItemDetail);
          this.mStoryQuestListItems.Add(gameObject);
        }
      }
    }

    private void CreatePieceQuest()
    {
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) this.PieceQuestItemTemplate, (UnityEngine.Object) null))
        return;
      GameManager instance = MonoSingleton<GameManager>.Instance;
      List<QuestParam> questParamList = new List<QuestParam>();
      QuestParam[] quests = instance.Quests;
      for (int index = 0; index < quests.Length; ++index)
      {
        if (!string.IsNullOrEmpty(quests[index].world) && quests[index].world == this.PieceQuestWorldId && (!string.IsNullOrEmpty(quests[index].ChapterID) && quests[index].ChapterID == this.CurrentUnit.UnitID))
          questParamList.Add(quests[index]);
      }
      if (questParamList.Count <= 1)
        return;
      for (int index = 0; index < questParamList.Count; ++index)
      {
        GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate<GameObject>((M0) this.PieceQuestItemTemplate);
        gameObject.SetActive(true);
        gameObject.get_transform().SetParent(this.QuestList, false);
        DataSource.Bind<QuestParam>(gameObject, questParamList[index]);
        DataSource.Bind<UnitData>(gameObject, this.CurrentUnit);
        ListItemEvents component = (ListItemEvents) gameObject.GetComponent<ListItemEvents>();
        component.OnSelect = new ListItemEvents.ListItemEvent(this.OnQuestSelect);
        component.OnOpenDetail = new ListItemEvents.ListItemEvent(this.OnOpenItemDetail);
        component.OnCloseDetail = new ListItemEvents.ListItemEvent(this.OnCloseItemDetail);
        this.mPieceQuestListItems.Add(gameObject);
      }
    }

    private void RefreshQuestList()
    {
      if (this.mListRefreshing || UnityEngine.Object.op_Equality((UnityEngine.Object) this.StoryQuestItemTemplate, (UnityEngine.Object) null) || (UnityEngine.Object.op_Equality((UnityEngine.Object) this.StoryQuestDisableItemTemplate, (UnityEngine.Object) null) || UnityEngine.Object.op_Equality((UnityEngine.Object) this.QuestList, (UnityEngine.Object) null)))
        return;
      this.mListRefreshing = true;
      if (this.mStoryQuestListItems.Count <= 0)
        this.CreateStoryList();
      if (this.mPieceQuestListItems.Count <= 0)
        this.CreatePieceQuest();
      for (int index = 0; index < this.mStoryQuestListItems.Count; ++index)
        this.mStoryQuestListItems[index].SetActive(this.mIsStoryList);
      for (int index = 0; index < this.mPieceQuestListItems.Count; ++index)
        this.mPieceQuestListItems[index].SetActive(!this.mIsStoryList);
      UnitData data = new UnitData();
      data.Setup(this.CurrentUnit);
      data.SetJobSkinAll((string) null);
      DataSource.Bind<UnitData>(this.CharacterImage, data);
      this.mListRefreshing = false;
    }

    private void OnQuestSelect(GameObject button)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      UnitCharacterQuestWindow.\u003COnQuestSelect\u003Ec__AnonStorey38B selectCAnonStorey38B = new UnitCharacterQuestWindow.\u003COnQuestSelect\u003Ec__AnonStorey38B();
      List<GameObject> gameObjectList = !this.mIsStoryList ? this.mPieceQuestListItems : this.mStoryQuestListItems;
      int index = gameObjectList.IndexOf(button.get_gameObject());
      // ISSUE: reference to a compiler-generated field
      selectCAnonStorey38B.quest = DataSource.FindDataOfClass<QuestParam>(gameObjectList[index], (QuestParam) null);
      // ISSUE: reference to a compiler-generated field
      if (selectCAnonStorey38B.quest == null)
        return;
      // ISSUE: reference to a compiler-generated field
      if (!selectCAnonStorey38B.quest.IsDateUnlock(-1L))
      {
        UIUtility.NegativeSystemMessage((string) null, LocalizedText.Get("sys.DISABLE_QUEST_DATE_UNLOCK"), (UIUtility.DialogResultEvent) null, (GameObject) null, false, -1);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        if (Array.Find<QuestParam>(MonoSingleton<GameManager>.Instance.Player.AvailableQuests, new Predicate<QuestParam>(selectCAnonStorey38B.\u003C\u003Em__438)) == null)
        {
          UIUtility.NegativeSystemMessage((string) null, LocalizedText.Get("sys.DISABLE_QUEST_CHALLENGE"), (UIUtility.DialogResultEvent) null, (GameObject) null, false, -1);
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          GlobalVars.SelectedQuestID = selectCAnonStorey38B.quest.iname;
          FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
        }
      }
    }

    private void OnCloseItemDetail(GameObject go)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mQuestDetail, (UnityEngine.Object) null))
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.mQuestDetail.get_gameObject());
      this.mQuestDetail = (GameObject) null;
    }

    private void OnOpenItemDetail(GameObject go)
    {
      QuestParam dataOfClass = DataSource.FindDataOfClass<QuestParam>(go, (QuestParam) null);
      if (!UnityEngine.Object.op_Equality((UnityEngine.Object) this.mQuestDetail, (UnityEngine.Object) null) || dataOfClass == null)
        return;
      this.mQuestDetail = (GameObject) UnityEngine.Object.Instantiate<GameObject>((M0) this.QuestDetailTemplate);
      DataSource.Bind<QuestParam>(this.mQuestDetail, dataOfClass);
      DataSource.Bind<UnitData>(this.mQuestDetail, this.CurrentUnit);
      this.mQuestDetail.SetActive(true);
    }

    private void OnToggleButton()
    {
      if (this.mListRefreshing)
        return;
      this.mIsStoryList = !this.mIsStoryList;
      this.UpdateToggleButton();
      this.RefreshQuestList();
    }

    private void UpdateToggleButton()
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ListToggleButton, (UnityEngine.Object) null))
        return;
      this.ListToggleButton.set_sprite(!this.mIsStoryList ? this.PieceListSprite : this.StoryListSprite);
    }

    public void Activated(int pinID)
    {
      if (pinID != 10)
        return;
      this.OnToggleButton();
    }
  }
}
