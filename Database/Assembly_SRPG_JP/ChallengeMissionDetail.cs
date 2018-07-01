﻿// Decompiled with JetBrains decompiler
// Type: SRPG.ChallengeMissionDetail
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using GR;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SRPG
{
  [FlowNode.Pin(1003, "装備強化画面へ移動", FlowNode.PinTypes.Output, 1004)]
  [FlowNode.Pin(1005, "マルチプレイへ移動", FlowNode.PinTypes.Output, 1005)]
  [FlowNode.Pin(1006, "エリートクエスト選択へ移動", FlowNode.PinTypes.Output, 1006)]
  [FlowNode.Pin(1007, "イベントクエスト選択へ移動", FlowNode.PinTypes.Output, 1007)]
  [FlowNode.Pin(1008, "アリーナへ移動", FlowNode.PinTypes.Output, 1008)]
  [FlowNode.Pin(1009, "ユニット強化画面へ移動", FlowNode.PinTypes.Output, 1009)]
  [FlowNode.Pin(1010, "ユニットアビリティセット画面へ移動", FlowNode.PinTypes.Output, 1010)]
  [FlowNode.Pin(1011, "ユニットジョブ画面へ移動", FlowNode.PinTypes.Output, 1011)]
  [FlowNode.Pin(1012, "ユニット選択へ移動", FlowNode.PinTypes.Output, 1012)]
  [FlowNode.Pin(1013, "アンナの店へ移動", FlowNode.PinTypes.Output, 1013)]
  [FlowNode.Pin(1014, "マリアの店へ移動", FlowNode.PinTypes.Output, 1014)]
  [FlowNode.Pin(1015, "ルイザの店へ移動", FlowNode.PinTypes.Output, 1015)]
  [FlowNode.Pin(1016, "ソウルショップへ移動", FlowNode.PinTypes.Output, 1016)]
  [FlowNode.Pin(1017, "ツアーの店へ移動", FlowNode.PinTypes.Output, 1017)]
  [FlowNode.Pin(1018, "闘技場交換所へ移動", FlowNode.PinTypes.Output, 1018)]
  [FlowNode.Pin(1019, "マルチ交換所へ移動", FlowNode.PinTypes.Output, 1019)]
  [FlowNode.Pin(1020, "魂の交換所へ移動", FlowNode.PinTypes.Output, 1020)]
  [FlowNode.Pin(1021, "武具の店へ移動", FlowNode.PinTypes.Output, 1021)]
  [FlowNode.Pin(1022, "武具錬成画面へ移動", FlowNode.PinTypes.Output, 1022)]
  [FlowNode.Pin(1023, "武具強化画面へ移動", FlowNode.PinTypes.Output, 1023)]
  [FlowNode.Pin(1024, "武具進化画面へ移動", FlowNode.PinTypes.Output, 1024)]
  [FlowNode.Pin(1025, "FgGID画面へ移動", FlowNode.PinTypes.Output, 1025)]
  [FlowNode.Pin(1026, "塔へ移動", FlowNode.PinTypes.Output, 1026)]
  [FlowNode.Pin(1027, "対戦へ移動", FlowNode.PinTypes.Output, 1027)]
  [FlowNode.Pin(1028, "初心者イベントクエスト選択へ移動", FlowNode.PinTypes.Output, 1028)]
  [FlowNode.Pin(1029, "初心者の館へ移動", FlowNode.PinTypes.Output, 1029)]
  [FlowNode.Pin(1030, "真理念装へ移動", FlowNode.PinTypes.Output, 1030)]
  [FlowNode.Pin(1031, "カルマへ移動", FlowNode.PinTypes.Output, 1031)]
  [FlowNode.Pin(1032, "ホームへ移動", FlowNode.PinTypes.Output, 1032)]
  [FlowNode.Pin(1033, "お知らせへ移動", FlowNode.PinTypes.Output, 1033)]
  [FlowNode.Pin(1034, "フレンド画面へ移動", FlowNode.PinTypes.Output, 1034)]
  [FlowNode.Pin(1002, "アビリティ強化画面へ移動", FlowNode.PinTypes.Output, 1003)]
  [FlowNode.Pin(1004, "ゴールド購入画面へ移動", FlowNode.PinTypes.Output, 1002)]
  [FlowNode.Pin(1001, "クエスト選択へ移動", FlowNode.PinTypes.Output, 1001)]
  [FlowNode.Pin(1000, "ガチャへ移動", FlowNode.PinTypes.Output, 1000)]
  [FlowNode.Pin(100, "キャンセル", FlowNode.PinTypes.Output, 100)]
  [FlowNode.Pin(0, "表示", FlowNode.PinTypes.Input, 0)]
  public class ChallengeMissionDetail : MonoBehaviour, IFlowInterface
  {
    public const int PIN_OPEN = 0;
    public const int PIN_CANCEL = 100;
    public const int PIN_GOTO_GACHA = 1000;
    public const int PIN_GOTO_QUEST = 1001;
    public const int PIN_GOTO_ABILITY = 1002;
    public const int PIN_GOTO_SOUBI = 1003;
    public const int PIN_GOTO_BUYGOLD = 1004;
    public const int PIN_GOTO_MULTI = 1005;
    public const int PIN_GOTO_ELITE = 1006;
    public const int PIN_GOTO_EVENT = 1007;
    public const int PIN_GOTO_ARENA = 1008;
    public const int PIN_GOTO_UNIT_STR = 1009;
    public const int PIN_GOTO_ABILITYSET = 1010;
    public const int PIN_GOTO_UNIT_JOB = 1011;
    public const int PIN_GOTO_UNIT = 1012;
    public const int PIN_GOTO_SHOP_NORMAL = 1013;
    public const int PIN_GOTO_SHOP_TABI = 1014;
    public const int PIN_GOTO_SHOP_KIMAGRE = 1015;
    public const int PIN_GOTO_SHOP_MONOZUKI = 1016;
    public const int PIN_GOTO_SHOP_TOUR = 1017;
    public const int PIN_GOTO_SHOP_ARENA = 1018;
    public const int PIN_GOTO_SHOP_MULTI = 1019;
    public const int PIN_GOTO_SHOP_KAKERA = 1020;
    public const int PIN_GOTO_SHOP_ARTIFACT = 1021;
    public const int PIN_GOTO_ATF_TRANS = 1022;
    public const int PIN_GOTO_ATF_STRTH = 1023;
    public const int PIN_GOTO_ATF_EVOLT = 1024;
    public const int PIN_GOTO_FGGID = 1025;
    public const int PIN_GOTO_TOWER = 1026;
    public const int PIN_GOTO_VERSUS = 1027;
    public const int PIN_GOTO_EVENT_BEGINEER = 1028;
    public const int PIN_GOTO_BEGINNERTOP = 1029;
    public const int PIN_GOTO_CONCEPTCARD = 1030;
    public const int PIN_GOTO_ORDEAL = 1031;
    public const int PIN_GOTO_HOME = 1032;
    public const int PIN_GOTO_NEWS = 1033;
    public const int PIN_GOTO_FRIEND = 1034;
    public RawImage ImageItem;
    public RawImage ImageExp;
    public RawImage ImageGold;
    public RawImage ImageStamina;
    public Text TextReward;
    public Button ButtonCancel;
    public Button ButtonTry;
    public ConceptCardIcon ConceptCardObject;

    public ChallengeMissionDetail()
    {
      base.\u002Ector();
    }

    public void Activated(int pinID)
    {
      if (pinID != 0)
        return;
      ((Component) this).get_gameObject().SetActive(true);
    }

    private void Start()
    {
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) this.ButtonCancel, (UnityEngine.Object) null) || UnityEngine.Object.op_Equality((UnityEngine.Object) this.ButtonTry, (UnityEngine.Object) null))
      {
        ((Behaviour) this).set_enabled(false);
      }
      else
      {
        // ISSUE: method pointer
        ((UnityEvent) this.ButtonCancel.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnCancel)));
        // ISSUE: method pointer
        ((UnityEvent) this.ButtonTry.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnTry)));
      }
    }

    private void OnEnable()
    {
      this.UpdateReward(DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null));
    }

    private void UpdateReward(TrophyParam trophy)
    {
      if (trophy == null)
        return;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = false;
      string str = string.Empty;
      ItemParam data = (ItemParam) null;
      if (trophy.Gold != 0)
      {
        flag3 = true;
        str = string.Format(LocalizedText.Get("sys.CHALLENGE_REWARD_GOLD"), (object) trophy.Gold);
      }
      else if (trophy.Exp != 0)
      {
        flag2 = true;
        str = string.Format(LocalizedText.Get("sys.CHALLENGE_REWARD_EXP"), (object) trophy.Exp);
      }
      else if (trophy.Coin != 0)
      {
        flag1 = true;
        str = string.Format(LocalizedText.Get("sys.CHALLENGE_REWARD_COIN"), (object) trophy.Coin);
        data = MonoSingleton<GameManager>.Instance.GetItemParam("$COIN");
      }
      else if (trophy.Stamina != 0)
      {
        flag4 = true;
        str = string.Format(LocalizedText.Get("sys.CHALLENGE_REWARD_STAMINA"), (object) trophy.Stamina);
      }
      else if (trophy.Items != null && trophy.Items.Length > 0)
      {
        flag1 = true;
        data = MonoSingleton<GameManager>.Instance.GetItemParam(trophy.Items[0].iname);
        if (data != null)
        {
          string empty = string.Empty;
          if (data.type == EItemType.Unit)
          {
            UnitParam unitParam = MonoSingleton<GameManager>.Instance.GetUnitParam(data.iname);
            if (unitParam != null)
              str = string.Format(LocalizedText.Get("sys.CHALLENGE_DETAIL_REWARD_UNIT"), (object) ((int) unitParam.rare + 1), (object) unitParam.name);
          }
          else
            str = string.Format(LocalizedText.Get("sys.CHALLENGE_DETAIL_REWARD"), (object) data.name, (object) trophy.Items[0].Num);
        }
      }
      else if (trophy.ConceptCards != null && trophy.ConceptCards.Length > 0)
      {
        flag5 = true;
        ConceptCardParam conceptCardParam = MonoSingleton<GameManager>.Instance.MasterParam.GetConceptCardParam(trophy.ConceptCards[0].iname);
        str = string.Format(LocalizedText.Get("sys.CHALLENGE_DETAIL_REWARD_CONCEPT_CARD"), (object) conceptCardParam.name, (object) trophy.ConceptCards[0].Num);
        if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ConceptCardObject, (UnityEngine.Object) null))
          this.ConceptCardObject.Setup(ConceptCardData.CreateConceptCardDataForDisplay(conceptCardParam.iname));
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ImageItem, (UnityEngine.Object) null))
        ((Component) this.ImageItem).get_gameObject().SetActive(flag1);
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ImageExp, (UnityEngine.Object) null))
        ((Component) this.ImageExp).get_gameObject().SetActive(flag2);
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ImageGold, (UnityEngine.Object) null))
        ((Component) this.ImageGold).get_gameObject().SetActive(flag3);
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ImageStamina, (UnityEngine.Object) null))
        ((Component) this.ImageStamina).get_gameObject().SetActive(flag4);
      if (data != null)
        DataSource.Bind<ItemParam>(((Component) this).get_gameObject(), data);
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TextReward, (UnityEngine.Object) null))
        this.TextReward.set_text(str);
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ConceptCardObject, (UnityEngine.Object) null))
        return;
      ((Component) this.ConceptCardObject).get_gameObject().SetActive(flag5);
    }

    private void OnCancel()
    {
      FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
    }

    private void OnTry()
    {
      TrophyParam dataOfClass = DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null);
      if (dataOfClass == null)
      {
        FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
      }
      else
      {
        QuestParam questParam = new QuestParam();
        PlayerData player = MonoSingleton<GameManager>.Instance.Player;
        switch (dataOfClass.Objectives[0].type)
        {
          case TrophyConditionTypes.winquest:
          case TrophyConditionTypes.winquestsoldier:
          case TrophyConditionTypes.losequest:
          case TrophyConditionTypes.damage_over:
          case TrophyConditionTypes.complete_all_quest_mission:
          case TrophyConditionTypes.complete_all_quest_mission_total:
          case TrophyConditionTypes.complete_all_mission_count:
          case TrophyConditionTypes.complete_story_mission_count:
            QuestTypes quest_type1 = QuestTypes.Story;
            if (!questParam.TransSectionGotoQuest(dataOfClass.Objectives[0].sval_base, out quest_type1, new UIUtility.DialogResultEvent(this.MsgBoxJumpToQuest)))
              break;
            QuestTypes questTypes1 = quest_type1;
            switch (questTypes1)
            {
              case QuestTypes.Event:
              case QuestTypes.Gps:
                FlowNode_GameObject.ActivateOutputLinks((Component) this, 1007);
                return;
              case QuestTypes.Tower:
                FlowNode_GameObject.ActivateOutputLinks((Component) this, 1026);
                return;
              case QuestTypes.Beginner:
                FlowNode_GameObject.ActivateOutputLinks((Component) this, 1028);
                return;
              case QuestTypes.MultiGps:
                FlowNode_GameObject.ActivateOutputLinks((Component) this, 1005);
                return;
              default:
                if (questTypes1 != QuestTypes.Multi)
                {
                  FlowNode_GameObject.ActivateOutputLinks((Component) this, 1001);
                  return;
                }
                goto case QuestTypes.MultiGps;
            }
          case TrophyConditionTypes.killenemy:
          case TrophyConditionTypes.getitem:
          case TrophyConditionTypes.playerlv:
          case TrophyConditionTypes.vip:
          case TrophyConditionTypes.stamina:
          case TrophyConditionTypes.card:
          case TrophyConditionTypes.review:
          case TrophyConditionTypes.followtwitter:
          case TrophyConditionTypes.unitlevel:
          case TrophyConditionTypes.evolutionnum:
          case TrophyConditionTypes.joblevel:
          case TrophyConditionTypes.logincount:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
            break;
          case TrophyConditionTypes.winelite:
          case TrophyConditionTypes.loseelite:
            if (!questParam.TransSectionGotoElite(new UIUtility.DialogResultEvent(this.MsgBoxJumpToQuest)))
              break;
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1001);
            break;
          case TrophyConditionTypes.winevent:
          case TrophyConditionTypes.loseevent:
          case TrophyConditionTypes.exclear_fire:
          case TrophyConditionTypes.exclear_water:
          case TrophyConditionTypes.exclear_wind:
          case TrophyConditionTypes.exclear_thunder:
          case TrophyConditionTypes.exclear_light:
          case TrophyConditionTypes.exclear_dark:
          case TrophyConditionTypes.exclear_fire_nocon:
          case TrophyConditionTypes.exclear_water_nocon:
          case TrophyConditionTypes.exclear_wind_nocon:
          case TrophyConditionTypes.exclear_thunder_nocon:
          case TrophyConditionTypes.exclear_light_nocon:
          case TrophyConditionTypes.exclear_dark_nocon:
            questParam.GotoEventListQuest((string) null, (string) null);
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1007);
            break;
          case TrophyConditionTypes.gacha:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1000);
            break;
          case TrophyConditionTypes.multiplay:
          case TrophyConditionTypes.winmulti:
          case TrophyConditionTypes.winmultimore:
          case TrophyConditionTypes.winmultiless:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1005);
            break;
          case TrophyConditionTypes.ability:
          case TrophyConditionTypes.makeabilitylevel:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1002);
            break;
          case TrophyConditionTypes.soubi:
            if (player.CheckUnlock(UnlockTargets.EnhanceEquip))
            {
              FlowNode_GameObject.ActivateOutputLinks((Component) this, 1003);
              break;
            }
            LevelLock.ShowLockMessage(player.Lv, player.VipRank, UnlockTargets.EnhanceEquip);
            break;
          case TrophyConditionTypes.buygold:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1004);
            break;
          case TrophyConditionTypes.arena:
          case TrophyConditionTypes.winarena:
          case TrophyConditionTypes.losearena:
            if (player.CheckUnlock(UnlockTargets.Arena))
            {
              FlowNode_GameObject.ActivateOutputLinks((Component) this, 1008);
              break;
            }
            LevelLock.ShowLockMessage(player.Lv, player.VipRank, UnlockTargets.Arena);
            break;
          case TrophyConditionTypes.fggid:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1025);
            break;
          case TrophyConditionTypes.upunitlevel:
          case TrophyConditionTypes.makeunitlevel:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1009);
            break;
          case TrophyConditionTypes.unitequip:
          case TrophyConditionTypes.upjoblevel:
          case TrophyConditionTypes.makejoblevel:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1011);
            break;
          case TrophyConditionTypes.limitbreak:
          case TrophyConditionTypes.evoltiontimes:
          case TrophyConditionTypes.changejob:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1012);
            break;
          case TrophyConditionTypes.changeability:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1010);
            break;
          case TrophyConditionTypes.buyatshop:
            this.GotoShop(dataOfClass);
            break;
          case TrophyConditionTypes.artifacttransmute:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1022);
            break;
          case TrophyConditionTypes.artifactstrength:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1023);
            break;
          case TrophyConditionTypes.artifactevolution:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1024);
            break;
          case TrophyConditionTypes.wintower:
          case TrophyConditionTypes.losetower:
            QuestTypes quest_type2 = QuestTypes.Story;
            if (!questParam.TransSectionGotoTower(dataOfClass.Objectives[0].sval_base, out quest_type2))
              break;
            QuestTypes questTypes2 = quest_type2;
            switch (questTypes2)
            {
              case QuestTypes.Gps:
                FlowNode_GameObject.ActivateOutputLinks((Component) this, 1007);
                return;
              case QuestTypes.Beginner:
                FlowNode_GameObject.ActivateOutputLinks((Component) this, 1028);
                return;
              default:
                if (questTypes2 != QuestTypes.Event)
                {
                  FlowNode_GameObject.ActivateOutputLinks((Component) this, 1026);
                  return;
                }
                goto case QuestTypes.Gps;
            }
          case TrophyConditionTypes.vs:
          case TrophyConditionTypes.vswin:
            if (player.CheckUnlock(UnlockTargets.MultiVS))
            {
              FlowNode_GameObject.ActivateOutputLinks((Component) this, 1027);
              break;
            }
            LevelLock.ShowLockMessage(player.Lv, player.VipRank, UnlockTargets.MultiVS);
            break;
          case TrophyConditionTypes.read_tips:
          case TrophyConditionTypes.read_tips_count:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1029);
            break;
          case TrophyConditionTypes.up_conceptcard_level:
          case TrophyConditionTypes.up_conceptcard_level_target:
          case TrophyConditionTypes.limitbreak_conceptcard:
          case TrophyConditionTypes.limitbreak_conceptcard_target:
          case TrophyConditionTypes.up_conceptcard_trust:
          case TrophyConditionTypes.up_conceptcard_trust_target:
          case TrophyConditionTypes.max_conceptcard_trust:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1030);
            break;
          case TrophyConditionTypes.unlock_tobira_total:
          case TrophyConditionTypes.unlock_tobira_unit:
          case TrophyConditionTypes.envy_unlock_unit:
          case TrophyConditionTypes.sloth_unlock_unit:
          case TrophyConditionTypes.lust_unlock_unit:
          case TrophyConditionTypes.greed_unlock_unit:
          case TrophyConditionTypes.wrath_unlock_unit:
          case TrophyConditionTypes.gluttonny_unlock_unit:
          case TrophyConditionTypes.pride_unlock_unit:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1012);
            break;
          case TrophyConditionTypes.send_present:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1034);
            break;
          case TrophyConditionTypes.complete_event_mission_count:
            questParam.GotoEventListQuest((string) null, (string) null);
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1007);
            break;
          case TrophyConditionTypes.complete_ordeal_mission_count:
          case TrophyConditionTypes.clear_ordeal:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1031);
            break;
          case TrophyConditionTypes.view_news:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1033);
            break;
          default:
            DebugUtility.Log(string.Format("未知の Trophy 条件 / {0}", (object) dataOfClass.Objectives[0].type));
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
            break;
        }
      }
    }

    private void GotoShop(TrophyParam trophy)
    {
      PlayerData player = MonoSingleton<GameManager>.Instance.Player;
      EShopType type = EShopType.Normal;
      if (!string.IsNullOrEmpty(trophy.Objectives[0].sval_base))
      {
        char[] chArray = new char[1]{ ',' };
        string[] strArray = trophy.Objectives[0].sval_base.Split(chArray);
        type = !string.IsNullOrEmpty(strArray[0]) ? (EShopType) MonoSingleton<GameManager>.Instance.MasterParam.GetShopType(strArray[0]) : EShopType.Normal;
      }
      if (type >= EShopType.Normal && player.CheckUnlockShopType(type))
      {
        switch (type)
        {
          case EShopType.Normal:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1013);
            break;
          case EShopType.Tabi:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1014);
            break;
          case EShopType.Kimagure:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1015);
            break;
          case EShopType.Monozuki:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1016);
            break;
          case EShopType.Tour:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1017);
            break;
          case EShopType.Arena:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1018);
            break;
          case EShopType.Multi:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1019);
            break;
          case EShopType.AwakePiece:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1020);
            break;
          case EShopType.Artifact:
            FlowNode_GameObject.ActivateOutputLinks((Component) this, 1021);
            break;
        }
      }
      if (type < EShopType.Normal)
        return;
      if (MonoSingleton<GameManager>.Instance.Player.CheckUnlockShopType(type))
        return;
      try
      {
        UnlockTargets unlockTargets = type.ToUnlockTargets();
        LevelLock.ShowLockMessage(player.Lv, player.VipRank, unlockTargets);
      }
      catch (Exception ex)
      {
      }
    }

    private void MsgBoxJumpToQuest(GameObject go)
    {
    }
  }
}
