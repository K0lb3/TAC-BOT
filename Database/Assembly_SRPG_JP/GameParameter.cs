﻿// Decompiled with JetBrains decompiler
// Type: SRPG.GameParameter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using GR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace SRPG
{
  [AddComponentMenu("UI/Game Parameter")]
  public class GameParameter : MonoBehaviour, IGameParameter
  {
    public static List<GameParameter> Instances = new List<GameParameter>();
    private const int PARAMETER_CATEGORY_SIZE = 100;
    private static bool[] mAlwaysUpdate;
    public GameParameter.ParameterTypes ParameterType;
    public int InstanceType;
    public int Index;
    private Slider mSlider;
    private UnityEngine.UI.Text mText;
    private InputField mInputField;
    private Animator mAnimator;
    private RawImage mImage;
    private ImageArray mImageArray;
    private Coroutine mUpdateCoroutine;
    private float mNextUpdateTime;
    private string mDefaultValue;
    private Vector2 mDefaultRangeValue;
    private Texture mDefaultImage;
    private Sprite mDefaultSprite;
    private bool mUpdate;
    private bool mIsEmptyGO;
    private bool mStarted;

    public GameParameter()
    {
      base.\u002Ector();
    }

    static GameParameter()
    {
      string[] names = Enum.GetNames(typeof (GameParameter.ParameterTypes));
      GameParameter.mAlwaysUpdate = new bool[names.Length];
      for (int index = 0; index < names.Length; ++index)
      {
        FieldInfo field = typeof (GameParameter.ParameterTypes).GetField(names[index]);
        if ((object) field != null)
          GameParameter.mAlwaysUpdate[index] = field.GetCustomAttributes(typeof (GameParameter.AlwaysUpdate), true).Length > 0;
      }
    }

    private SupportData GetSupportData()
    {
      return DataSource.FindDataOfClass<SupportData>(((Component) this).get_gameObject(), (SupportData) null) ?? (SupportData) GlobalVars.SelectedSupport;
    }

    private FriendData GetFriendData()
    {
      return DataSource.FindDataOfClass<FriendData>(((Component) this).get_gameObject(), (FriendData) null);
    }

    private AbilityParam GetAbilityParam()
    {
      AbilityParam dataOfClass = DataSource.FindDataOfClass<AbilityParam>(((Component) this).get_gameObject(), (AbilityParam) null);
      if (dataOfClass == null)
      {
        AbilityData abilityData = this.GetAbilityData();
        if (abilityData != null)
          dataOfClass = abilityData.Param;
      }
      return dataOfClass;
    }

    private AbilityData GetAbilityData()
    {
      return DataSource.FindDataOfClass<AbilityData>(((Component) this).get_gameObject(), (AbilityData) null);
    }

    private ArenaPlayer GetArenaPlayer()
    {
      switch ((GameParameter.ArenaPlayerInstanceTypes) this.InstanceType)
      {
        case GameParameter.ArenaPlayerInstanceTypes.Enemy:
          return (ArenaPlayer) GlobalVars.SelectedArenaPlayer;
        default:
          return DataSource.FindDataOfClass<ArenaPlayer>(((Component) this).get_gameObject(), (ArenaPlayer) null);
      }
    }

    private ArtifactParam GetArtifactParam()
    {
      switch (this.InstanceType)
      {
        case 0:
          ArtifactData dataOfClass1 = DataSource.FindDataOfClass<ArtifactData>(((Component) this).get_gameObject(), (ArtifactData) null);
          if (dataOfClass1 != null)
            return dataOfClass1.ArtifactParam;
          return DataSource.FindDataOfClass<ArtifactParam>(((Component) this).get_gameObject(), (ArtifactParam) null);
        case 1:
          QuestParam questParamAuto = this.GetQuestParamAuto();
          if (questParamAuto != null && 0 <= this.Index && (questParamAuto.bonusObjective != null && this.Index < questParamAuto.bonusObjective.Length))
            return MonoSingleton<GameManager>.Instance.MasterParam.GetArtifactParam(questParamAuto.bonusObjective[this.Index].item);
          break;
        case 2:
          ArtifactRewardData dataOfClass2 = DataSource.FindDataOfClass<ArtifactRewardData>(((Component) this).get_gameObject(), (ArtifactRewardData) null);
          if (dataOfClass2 != null)
          {
            ArtifactParam artifactParam = dataOfClass2.ArtifactParam;
            if (artifactParam != null)
              return artifactParam;
          }
          this.ResetToDefault();
          break;
      }
      return (ArtifactParam) null;
    }

    private ArtifactData GetArtifactData()
    {
      return DataSource.FindDataOfClass<ArtifactData>(((Component) this).get_gameObject(), (ArtifactData) null);
    }

    private void SetArtifactFrame(ArtifactParam param)
    {
      if (param == null)
        return;
      Image component = (Image) ((Component) this).GetComponent<Image>();
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null))
        return;
      int rareini = param.rareini;
      GameSettings instance = GameSettings.Instance;
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) instance, (UnityEngine.Object) null) || rareini >= instance.ArtifactIcon_Frames.Length)
        return;
      component.set_sprite(instance.ArtifactIcon_Frames[rareini]);
    }

    private Unit GetUnit()
    {
      switch (this.InstanceType)
      {
        case 3:
          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null) && SceneBattle.Instance.Battle.CurrentUnit != null)
            return SceneBattle.Instance.Battle.CurrentUnit;
          return (Unit) null;
        default:
          return DataSource.FindDataOfClass<Unit>(((Component) this).get_gameObject(), (Unit) null);
      }
    }

    private UnitParam GetUnitParam()
    {
      UnitParam dataOfClass = DataSource.FindDataOfClass<UnitParam>(((Component) this).get_gameObject(), (UnitParam) null);
      if (dataOfClass != null)
        return dataOfClass;
      return this.GetUnitData()?.UnitParam;
    }

    private UnitData GetUnitData()
    {
      UnitData unitData = (UnitData) null;
      switch (this.InstanceType)
      {
        case 0:
          unitData = DataSource.FindDataOfClass<UnitData>(((Component) this).get_gameObject(), (UnitData) null);
          if (unitData == null)
          {
            Unit unit = this.GetUnit();
            if (unit != null)
            {
              unitData = unit.UnitData;
              break;
            }
            break;
          }
          break;
        case 3:
          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null) && SceneBattle.Instance.Battle.CurrentUnit != null)
            return SceneBattle.Instance.Battle.CurrentUnit.UnitData;
          break;
        case 4:
        case 5:
        case 6:
          ArenaPlayer dataOfClass1 = DataSource.FindDataOfClass<ArenaPlayer>(((Component) this).get_gameObject(), (ArenaPlayer) null);
          if (dataOfClass1 != null)
          {
            int index = this.InstanceType - 4;
            unitData = dataOfClass1.Unit[index];
            break;
          }
          break;
        case 7:
        case 8:
        case 9:
          ArenaPlayer selectedArenaPlayer = (ArenaPlayer) GlobalVars.SelectedArenaPlayer;
          if (selectedArenaPlayer != null)
          {
            int index = this.InstanceType - 7;
            unitData = selectedArenaPlayer.Unit[index];
            break;
          }
          break;
        case 10:
        case 11:
        case 12:
          PlayerData player1 = MonoSingleton<GameManager>.Instance.Player;
          int index1 = this.InstanceType - 10;
          long unitUniqueId1 = player1.Partys[(int) GlobalVars.SelectedPartyIndex].GetUnitUniqueID(index1);
          unitData = player1.FindUnitDataByUniqueID(unitUniqueId1);
          break;
        case 13:
          PlayerData player2 = MonoSingleton<GameManager>.Instance.Player;
          long unitUniqueId2 = player2.Partys[7].GetUnitUniqueID(0);
          unitData = player2.FindUnitDataByUniqueID(unitUniqueId2);
          break;
        case 16:
        case 17:
        case 18:
          VersusCpuData dataOfClass2 = DataSource.FindDataOfClass<VersusCpuData>(((Component) this).get_gameObject(), (VersusCpuData) null);
          if (dataOfClass2 != null)
          {
            int index2 = this.InstanceType - 16;
            if (dataOfClass2.Units.Length > index2)
            {
              unitData = dataOfClass2.Units[index2];
              break;
            }
            break;
          }
          break;
        case 19:
          PlayerData player3 = MonoSingleton<GameManager>.Instance.Player;
          long unitUniqueId3 = player3.Partys[10].GetUnitUniqueID(0);
          unitData = player3.FindUnitDataByUniqueID(unitUniqueId3);
          break;
        case 20:
          unitData = MonoSingleton<GameManager>.Instance.Player.FindUnitDataByUniqueID((long) GlobalVars.SelectedSupportUnitUniqueID);
          break;
        case 21:
        case 22:
        case 23:
        case 24:
        case 25:
        case 26:
        case 27:
        case 28:
        case 29:
        case 30:
        case 31:
        case 32:
          List<UnitData> unitDataList;
          int index3;
          if (this.InstanceType < 27)
          {
            unitDataList = VersusDraftList.VersusDraftUnitDataListPlayer;
            index3 = this.InstanceType - 21;
          }
          else
          {
            unitDataList = VersusDraftList.VersusDraftUnitDataListEnemy;
            index3 = this.InstanceType - 27;
          }
          if (unitDataList != null && unitDataList.Count > index3)
          {
            unitData = unitDataList[index3];
            break;
          }
          break;
      }
      return unitData;
    }

    private JSON_MyPhotonPlayerParam.UnitDataElem GetMultiPlayerUnitData(int index)
    {
      JSON_MyPhotonPlayerParam roomPlayerParam = this.GetRoomPlayerParam();
      if (roomPlayerParam == null || roomPlayerParam.units == null)
        return (JSON_MyPhotonPlayerParam.UnitDataElem) null;
      return Array.Find<JSON_MyPhotonPlayerParam.UnitDataElem>(roomPlayerParam.units, (Predicate<JSON_MyPhotonPlayerParam.UnitDataElem>) (e => e.slotID == index));
    }

    private JSON_MyPhotonPlayerParam GetVersusPlayerParam(JSON_MyPhotonPlayerParam[] players, int cnt)
    {
      MyPhoton instance = PunMonoSingleton<MyPhoton>.Instance;
      JSON_MyPhotonPlayerParam photonPlayerParam = (JSON_MyPhotonPlayerParam) null;
      if (players.Length > cnt)
      {
        for (int index = 0; index < players.Length; ++index)
        {
          JSON_MyPhotonPlayerParam player = players[index];
          if (player != null && player.playerID != instance.GetMyPlayer().playerID)
          {
            photonPlayerParam = player;
            break;
          }
        }
      }
      return photonPlayerParam;
    }

    private JSON_MyPhotonPlayerParam GetVersusPlayerParam()
    {
      MyPhoton instance = PunMonoSingleton<MyPhoton>.Instance;
      JSON_MyPhotonPlayerParam photonPlayerParam = (JSON_MyPhotonPlayerParam) null;
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance, (UnityEngine.Object) null))
      {
        if (this.InstanceType == 0)
        {
          photonPlayerParam = JSON_MyPhotonPlayerParam.Create(0, 0);
        }
        else
        {
          MyPhoton.MyRoom currentRoom = instance.GetCurrentRoom();
          if (currentRoom != null)
          {
            JSON_MyPhotonRoomParam myPhotonRoomParam = JSON_MyPhotonRoomParam.Parse(currentRoom.json);
            if (myPhotonRoomParam != null)
            {
              JSON_MyPhotonPlayerParam[] players = myPhotonRoomParam.players;
              if (players != null && players.Length > 1)
                photonPlayerParam = this.GetVersusPlayerParam(players, 1);
            }
            if (photonPlayerParam == null)
            {
              string roomParam = instance.GetRoomParam("started");
              if (roomParam != null)
              {
                FlowNode_StartMultiPlay.PlayerList jsonObject = JSONParser.parseJSONObject<FlowNode_StartMultiPlay.PlayerList>(roomParam);
                if (jsonObject != null)
                  photonPlayerParam = this.GetVersusPlayerParam(jsonObject.players, 1);
              }
            }
          }
        }
      }
      return photonPlayerParam;
    }

    private PartyData GetPartyData()
    {
      return DataSource.FindDataOfClass<PartyData>(((Component) this).get_gameObject(), (PartyData) null);
    }

    private SkillParam GetLeaderSkill(PartyData party)
    {
      UnitData unitDataByUniqueId = MonoSingleton<GameManager>.Instance.Player.FindUnitDataByUniqueID(party.GetUnitUniqueID(party.LeaderIndex));
      if (unitDataByUniqueId != null && unitDataByUniqueId.LeaderSkill != null)
        return unitDataByUniqueId.LeaderSkill.SkillParam;
      return (SkillParam) null;
    }

    private ItemParam GetItemParam()
    {
      switch (this.InstanceType)
      {
        case 0:
          ItemData dataOfClass1 = DataSource.FindDataOfClass<ItemData>(((Component) this).get_gameObject(), (ItemData) null);
          if (dataOfClass1 != null)
            return dataOfClass1.Param;
          return DataSource.FindDataOfClass<ItemParam>(((Component) this).get_gameObject(), (ItemParam) null);
        case 1:
          PlayerData player1 = MonoSingleton<GameManager>.Instance.Player;
          if (0 <= this.Index && this.Index < player1.Inventory.Length)
            return player1.Inventory[this.Index].Param;
          break;
        case 2:
          QuestParam questParam = DataSource.FindDataOfClass<QuestParam>(((Component) this).get_gameObject(), (QuestParam) null);
          if (questParam == null && UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null))
            questParam = MonoSingleton<GameManager>.Instance.FindQuest(SceneBattle.Instance.Battle.QuestID) ?? DataSource.FindDataOfClass<QuestParam>(((Component) this).get_gameObject(), (QuestParam) null);
          if (questParam != null && questParam.type == QuestTypes.Tower)
          {
            TowerRewardItem towerRewardItem = this.GetTowerRewardItem();
            if (towerRewardItem == null)
              return (ItemParam) null;
            if (towerRewardItem.type != TowerRewardItem.RewardType.Item)
              return (ItemParam) null;
            return MonoSingleton<GameManager>.Instance.GetItemParam(towerRewardItem.iname);
          }
          if (questParam != null && questParam.IsVersus)
          {
            GameManager instance = MonoSingleton<GameManager>.Instance;
            PlayerData player2 = instance.Player;
            VersusTowerParam versusTowerParam = instance.GetCurrentVersusTowerParam(player2.VersusTowerFloor + 1);
            if (versusTowerParam == null)
              return (ItemParam) null;
            if (versusTowerParam.ArrivalItemType != VERSUS_ITEM_TYPE.item)
              return (ItemParam) null;
            string arrivalIteminame = (string) versusTowerParam.ArrivalIteminame;
            return instance.GetItemParam(arrivalIteminame);
          }
          if (questParam != null && 0 <= this.Index && (questParam.bonusObjective != null && this.Index < questParam.bonusObjective.Length))
            return MonoSingleton<GameManager>.Instance.GetItemParam(questParam.bonusObjective[this.Index].item);
          break;
        case 3:
          EquipData dataOfClass2 = DataSource.FindDataOfClass<EquipData>(((Component) this).get_gameObject(), (EquipData) null);
          if (dataOfClass2 != null)
            return dataOfClass2.ItemParam;
          break;
        case 4:
          EnhanceMaterial dataOfClass3 = DataSource.FindDataOfClass<EnhanceMaterial>(((Component) this).get_gameObject(), (EnhanceMaterial) null);
          if (dataOfClass3 != null && dataOfClass3.item != null)
            return dataOfClass3.item.Param;
          break;
        case 5:
          EnhanceEquipData dataOfClass4 = DataSource.FindDataOfClass<EnhanceEquipData>(((Component) this).get_gameObject(), (EnhanceEquipData) null);
          if (dataOfClass4 != null && dataOfClass4.equip != null)
            return dataOfClass4.equip.ItemParam;
          break;
        case 6:
          SellItem dataOfClass5 = DataSource.FindDataOfClass<SellItem>(((Component) this).get_gameObject(), (SellItem) null);
          if (dataOfClass5 != null && dataOfClass5.item != null)
            return dataOfClass5.item.Param;
          break;
        case 7:
          ConsumeItemData dataOfClass6 = DataSource.FindDataOfClass<ConsumeItemData>(((Component) this).get_gameObject(), (ConsumeItemData) null);
          if (dataOfClass6 != null)
            return dataOfClass6.param;
          break;
      }
      return (ItemParam) null;
    }

    private ItemData GetInventoryItemData()
    {
      PlayerData player = MonoSingleton<GameManager>.Instance.Player;
      if (0 <= this.Index && this.Index < player.Inventory.Length)
        return player.Inventory[this.Index];
      return (ItemData) null;
    }

    private PlayerLevelUpInfo GetLevelUpInfo()
    {
      return DataSource.FindDataOfClass<PlayerLevelUpInfo>(((Component) this).get_gameObject(), (PlayerLevelUpInfo) null);
    }

    private ItemParam GetInventoryItemParam()
    {
      return this.GetInventoryItemData()?.Param;
    }

    private SkillData GetSkillData()
    {
      return DataSource.FindDataOfClass<SkillData>(((Component) this).get_gameObject(), (SkillData) null);
    }

    private SkillParam GetSkillParam()
    {
      return DataSource.FindDataOfClass<SkillParam>(((Component) this).get_gameObject(), (SkillParam) null);
    }

    private JobParam GetJobParam()
    {
      JobParam dataOfClass = DataSource.FindDataOfClass<JobParam>(((Component) this).get_gameObject(), (JobParam) null);
      if (dataOfClass != null)
        return dataOfClass;
      return DataSource.FindDataOfClass<JobData>(((Component) this).get_gameObject(), (JobData) null)?.Param;
    }

    private EquipData GetUnitEquipData()
    {
      UnitData dataOfClass = DataSource.FindDataOfClass<UnitData>(((Component) this).get_gameObject(), (UnitData) null);
      if (dataOfClass != null && 0 <= this.Index && this.Index < dataOfClass.CurrentEquips.Length)
        return dataOfClass.CurrentEquips[this.Index];
      return (EquipData) null;
    }

    private EquipData GetEquipData()
    {
      return DataSource.FindDataOfClass<EquipData>(((Component) this).get_gameObject(), (EquipData) null);
    }

    private QuestParam GetQuestParamAuto()
    {
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null))
        return MonoSingleton<GameManager>.Instance.FindQuest(SceneBattle.Instance.Battle.QuestID);
      return DataSource.FindDataOfClass<QuestParam>(((Component) this).get_gameObject(), (QuestParam) null);
    }

    private TowerRewardItem GetTowerRewardItem()
    {
      QuestParam questParamAuto = this.GetQuestParamAuto();
      if (questParamAuto == null)
        return (TowerRewardItem) null;
      TowerRewardParam towerReward = MonoSingleton<GameManager>.Instance.FindTowerReward(MonoSingleton<GameManager>.Instance.FindTowerFloor(questParamAuto.iname).reward_id);
      if (towerReward == null)
        return (TowerRewardItem) null;
      List<TowerRewardItem> towerRewardItem = towerReward.GetTowerRewardItem();
      if (towerRewardItem == null || towerRewardItem.Count < this.Index)
        return (TowerRewardItem) null;
      return towerRewardItem[this.Index];
    }

    private QuestParam GetQuestParam()
    {
      QuestParam questParam = (QuestParam) null;
      switch (this.InstanceType)
      {
        case 0:
          questParam = DataSource.FindDataOfClass<QuestParam>(((Component) this).get_gameObject(), (QuestParam) null);
          break;
        case 1:
          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null))
          {
            questParam = MonoSingleton<GameManager>.Instance.FindQuest(SceneBattle.Instance.Battle.QuestID);
            break;
          }
          break;
        case 2:
          questParam = MonoSingleton<GameManager>.Instance.FindQuest(GlobalVars.SelectedQuestID);
          break;
      }
      return questParam;
    }

    private JSON_MyPhotonPlayerParam GetRoomPlayerParam()
    {
      JSON_MyPhotonPlayerParam dataOfClass = DataSource.FindDataOfClass<JSON_MyPhotonPlayerParam>(((Component) this).get_gameObject(), (JSON_MyPhotonPlayerParam) null);
      if (dataOfClass == null)
        return (JSON_MyPhotonPlayerParam) null;
      if (dataOfClass.playerIndex <= 0)
        return (JSON_MyPhotonPlayerParam) null;
      return dataOfClass;
    }

    private MultiPlayAPIRoom GetRoom()
    {
      return DataSource.FindDataOfClass<MultiPlayAPIRoom>(((Component) this).get_gameObject(), (MultiPlayAPIRoom) null);
    }

    private JSON_MyPhotonRoomParam GetRoomParam()
    {
      JSON_MyPhotonRoomParam dataOfClass = DataSource.FindDataOfClass<JSON_MyPhotonRoomParam>(((Component) this).get_gameObject(), (JSON_MyPhotonRoomParam) null);
      if (dataOfClass != null)
        return dataOfClass;
      MyPhoton.MyRoom currentRoom = PunMonoSingleton<MyPhoton>.Instance.GetCurrentRoom();
      if (currentRoom == null)
        return (JSON_MyPhotonRoomParam) null;
      return JSON_MyPhotonRoomParam.Parse(currentRoom.json);
    }

    private bool LoadItemIcon(string iconName)
    {
      IconLoader iconLoader = GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject());
      if (string.IsNullOrEmpty(iconName))
        return false;
      iconLoader.ResourcePath = AssetPath.ItemIcon(iconName);
      return true;
    }

    private bool LoadItemIcon(ItemParam itemParam)
    {
      IconLoader iconLoader = GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject());
      if (itemParam == null)
        return false;
      iconLoader.ResourcePath = AssetPath.ItemIcon(itemParam);
      return true;
    }

    private void SetItemFrame(ItemParam itemParam)
    {
      if (itemParam == null)
        return;
      Image component = (Image) ((Component) this).GetComponent<Image>();
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null))
        return;
      Sprite itemFrame = GameSettings.Instance.GetItemFrame(itemParam);
      component.set_sprite(itemFrame);
    }

    private void SetEquipItemFrame(ItemParam itemParam)
    {
      Sprite[] normalFrames = GameSettings.Instance.ItemIcons.NormalFrames;
      Image component = (Image) ((Component) this).GetComponent<Image>();
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null) || normalFrames.Length <= 0)
        return;
      if (itemParam != null && itemParam.rare < normalFrames.Length)
        component.set_sprite(normalFrames[itemParam.rare]);
      else
        component.set_sprite(normalFrames[0]);
    }

    private MailData GetMailData()
    {
      return DataSource.FindDataOfClass<MailData>(((Component) this).get_gameObject(), (MailData) null);
    }

    private SellItem GetSellItem()
    {
      return DataSource.FindDataOfClass<SellItem>(((Component) this).get_gameObject(), (SellItem) null);
    }

    private List<SellItem> GetSellItemList()
    {
      return DataSource.FindDataOfClass<List<SellItem>>(((Component) this).get_gameObject(), (List<SellItem>) null);
    }

    private ShopItem GetShopItem()
    {
      return DataSource.FindDataOfClass<ShopItem>(((Component) this).get_gameObject(), (ShopItem) null) ?? (ShopItem) this.GetLimitedShopItem() ?? (ShopItem) this.GetEventShopItem();
    }

    private LimitedShopItem GetLimitedShopItem()
    {
      return DataSource.FindDataOfClass<LimitedShopItem>(((Component) this).get_gameObject(), (LimitedShopItem) null);
    }

    private EventShopItem GetEventShopItem()
    {
      return DataSource.FindDataOfClass<EventShopItem>(((Component) this).get_gameObject(), (EventShopItem) null);
    }

    private GachaParam GetGachaParam()
    {
      return DataSource.FindDataOfClass<GachaParam>(((Component) this).get_gameObject(), (GachaParam) null);
    }

    private void SetBuyPriceTypeIcon(ESaleType type)
    {
      Sprite[] itemPriceIconFrames = GameSettings.Instance.ItemPriceIconFrames;
      if (itemPriceIconFrames == null || type == ESaleType.EventCoin)
        return;
      Image component = (Image) ((Component) this).GetComponent<Image>();
      int index = type == ESaleType.Coin_P ? 1 : (int) type;
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null) || index >= itemPriceIconFrames.Length)
        return;
      component.set_sprite(itemPriceIconFrames[index]);
    }

    private void SetBuyPriceEventCoinTypeIcon(string cost_iname)
    {
      Image component = (Image) ((Component) this).GetComponent<Image>();
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) component, (UnityEngine.Object) null) || cost_iname == null)
        return;
      SpriteSheet spriteSheet = AssetManager.Load<SpriteSheet>("EventShopCmn/eventcoin_small");
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet, (UnityEngine.Object) null))
        return;
      component.set_sprite(spriteSheet.GetSprite(cost_iname));
    }

    private TrophyParam GetTrophyParam()
    {
      return DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null);
    }

    private TrophyObjective GetTrophyObjective()
    {
      return DataSource.FindDataOfClass<TrophyObjective>(((Component) this).get_gameObject(), (TrophyObjective) null);
    }

    private EnhanceEquipData GetEnhanceEquipData()
    {
      return DataSource.FindDataOfClass<EnhanceEquipData>(((Component) this).get_gameObject(), (EnhanceEquipData) null);
    }

    private EnhanceMaterial GetEnhanceMaterial()
    {
      return DataSource.FindDataOfClass<EnhanceMaterial>(((Component) this).get_gameObject(), (EnhanceMaterial) null);
    }

    private EquipItemParameter GetEquipItemParameter()
    {
      return DataSource.FindDataOfClass<EquipItemParameter>(((Component) this).get_gameObject(), (EquipItemParameter) null);
    }

    private string GetParamTypeName(ParamTypes type)
    {
      if (type == ParamTypes.None)
        return (string) null;
      return LocalizedText.Get("sys." + type.ToString());
    }

    private bool CheckUnlockInstanceType()
    {
      return MonoSingleton<GameManager>.Instance.Player.CheckUnlock((UnlockTargets) this.InstanceType);
    }

    private TrickParam GetTrickParam()
    {
      return DataSource.FindDataOfClass<TrickParam>(((Component) this).get_gameObject(), (TrickParam) null);
    }

    private BattleCore.OrderData GetOrderData()
    {
      return DataSource.FindDataOfClass<BattleCore.OrderData>(((Component) this).get_gameObject(), (BattleCore.OrderData) null);
    }

    private MapEffectParam GetMapEffectParam()
    {
      return DataSource.FindDataOfClass<MapEffectParam>(((Component) this).get_gameObject(), (MapEffectParam) null);
    }

    private WeatherParam GetWeatherParam()
    {
      return DataSource.FindDataOfClass<WeatherParam>(((Component) this).get_gameObject(), (WeatherParam) null);
    }

    private bool LoadArtifactIcon(ArtifactParam param)
    {
      IconLoader iconLoader = GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject());
      if (param == null)
        return false;
      iconLoader.ResourcePath = AssetPath.ArtifactIcon(param);
      return true;
    }

    private void GetQuestObjectiveCount(QuestParam questParam, out int compCount, out int maxCount)
    {
      maxCount = questParam.bonusObjective.Length;
      compCount = 0;
      for (int index = 0; index < maxCount; ++index)
      {
        if ((questParam.clear_missions & 1 << index) != 0)
          ++compCount;
      }
    }

    private void InternalUpdateValue()
    {
      GameParameter.ParameterTypes parameterType = this.ParameterType;
      QuestParam questParam1;
      UnitData unitData1;
      GameManager instance1;
      SupportData supportData;
      SkillParam skillParam;
      switch (parameterType)
      {
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.Name);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
          GameManager instance2 = MonoSingleton<GameManager>.Instance;
          this.SetTextValue(instance2.Player.CalcLevel());
          this.SetSliderValue(instance2.Player.CalcLevel(), 99);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
          GameManager instance3 = MonoSingleton<GameManager>.Instance;
          instance3.Player.UpdateStamina();
          this.SetTextValue(instance3.Player.Stamina);
          this.SetSliderValue(instance3.Player.Stamina, instance3.Player.StaminaMax);
          this.SetUpdateInterval(1f);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.StaminaMax);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
          GameManager instance4 = MonoSingleton<GameManager>.Instance;
          this.SetTextValue(instance4.Player.Exp);
          this.SetSliderValue(instance4.Player.GetExp(), instance4.Player.GetExp() + instance4.Player.GetNextExp());
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.GetNextExp());
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.Gold);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_COIN:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.Coin);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINATIME:
          this.SetTextValue(TimeManager.ToMinSecString(MonoSingleton<GameManager>.Instance.Player.GetNextStaminaRecoverySec()));
          this.SetUpdateInterval(1f);
          break;
        case GameParameter.ParameterTypes.QUEST_NAME:
          QuestParam questParam2;
          if ((questParam2 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(questParam2.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_STAMINA:
          QuestParam questParam3;
          if ((questParam3 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(questParam3.RequiredApWithPlayerLv(MonoSingleton<GameManager>.Instance.Player.Lv, true));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_STATE:
          QuestParam questParam4;
          if ((questParam4 = this.GetQuestParam()) != null)
          {
            this.SetAnimatorInt("state", (int) questParam4.state);
            this.SetImageIndex((int) questParam4.state);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_OBJECTIVE:
          QuestParam questParam5;
          if ((questParam5 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(questParam5.cond);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_BONUSOBJECTIVE:
          QuestParam questParam6;
          if ((questParam6 = this.GetQuestParam()) != null && questParam6.bonusObjective != null && (0 <= this.Index && this.Index < questParam6.bonusObjective.Length))
          {
            this.SetTextValue(GameUtility.ComposeQuestBonusObjectiveText(questParam6.bonusObjective[this.Index]));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_ICON:
        case GameParameter.ParameterTypes.INVENTORY_ITEMICON:
          ItemParam itemParam1 = this.ParameterType != GameParameter.ParameterTypes.ITEM_ICON ? this.GetInventoryItemParam() : this.GetItemParam();
          if (itemParam1 != null && this.LoadItemIcon(itemParam1))
            break;
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_DESCRIPTION:
          QuestParam questParam7;
          if ((questParam7 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(questParam7.expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_NAME:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetTextValue(supportData.PlayerName);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_LEVEL:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetTextValue(supportData.PlayerLevel);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_UNITLEVEL:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetTextValue(supportData.UnitLevel);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_LEADERSKILLNAME:
          if ((supportData = this.GetSupportData()) != null)
          {
            skillParam = supportData.LeaderSkill;
            if (skillParam == null)
              break;
            this.SetTextValue(skillParam.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_ATK:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetTextValue((int) supportData.UnitParam.ini_status.param.atk);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_HP:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetTextValue((int) supportData.UnitParam.ini_status.param.hp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_MAGIC:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetTextValue((int) supportData.UnitParam.ini_status.param.mag);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_RARITY:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetAnimatorInt("rare", supportData.UnitRarity);
            this.SetImageIndex(supportData.UnitRarity);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_ELEMENT:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetAnimatorInt("element", (int) supportData.UnitElement);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_ICON:
          if ((supportData = this.GetSupportData()) != null && supportData.Unit != null)
          {
            string str = AssetPath.UnitSkinIconSmall(supportData.Unit.UnitParam, supportData.Unit.GetSelectedSkin(-1), supportData.JobID);
            if (!string.IsNullOrEmpty(str))
            {
              GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str;
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_LEADERSKILLDESC:
          if ((supportData = this.GetSupportData()) != null)
          {
            skillParam = supportData.LeaderSkill;
            if (skillParam == null)
              break;
            this.SetTextValue(skillParam.expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_SUBTITLE:
          QuestParam questParam8;
          if ((questParam8 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(questParam8.title);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_LEVEL:
          UnitData unitData2;
          if ((unitData2 = this.GetUnitData()) != null)
          {
            int num = unitData2.CalcLevel();
            this.SetTextValue(num);
            this.SetSliderValue(num, 99);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_HP:
          Unit unit1;
          if ((unit1 = this.GetUnit()) != null)
          {
            this.SetTextValue((int) unit1.CurrentStatus.param.hp);
            this.SetSliderValue((int) unit1.CurrentStatus.param.hp, (int) unit1.MaximumStatus.param.hp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_HPMAX:
          UnitData unitData3;
          if ((unitData3 = this.GetUnitData()) != null)
          {
            OInt oint = (OInt) 0;
            Unit unit2 = this.GetUnit();
            this.SetTextValue((int) (unit2 == null ? unitData3.Status.param.hp : unit2.MaximumStatus.param.hp));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_ATK:
          UnitData unitData4;
          if ((unitData4 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData4.Status.param.atk);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_MAG:
          UnitData unitData5;
          if ((unitData5 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData5.Status.param.mag);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_ICON:
          UnitData unitData6;
          if ((unitData6 = this.GetUnitData()) != null)
          {
            string str = AssetPath.UnitSkinIconSmall(unitData6.UnitParam, unitData6.GetSelectedSkin(-1), unitData6.CurrentJob.JobID);
            if (!string.IsNullOrEmpty(str))
            {
              GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str;
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_NAME:
          UnitParam unitParam1;
          if ((unitParam1 = this.GetUnitParam()) != null)
          {
            this.SetTextValue(unitParam1.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_RARITY:
          UnitData unitData7;
          if ((unitData7 = this.GetUnitData()) != null)
          {
            StarGauge component = (StarGauge) ((Component) this).GetComponent<StarGauge>();
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null))
            {
              component.Max = unitData7.GetRarityCap() + 1;
              component.Value = unitData7.Rarity + 1;
              break;
            }
            this.SetAnimatorInt("rare", unitData7.Rarity);
            this.SetImageIndex(unitData7.Rarity);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PARTY_LEADERSKILLNAME:
          PartyData partyData1;
          if ((partyData1 = this.GetPartyData()) != null)
          {
            skillParam = this.GetLeaderSkill(partyData1);
            if (skillParam != null)
            {
              this.SetTextValue(skillParam.name);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PARTY_LEADERSKILLDESC:
          PartyData partyData2;
          if ((partyData2 = this.GetPartyData()) != null)
          {
            skillParam = this.GetLeaderSkill(partyData2);
            if (skillParam != null)
            {
              this.SetTextValue(skillParam.expr);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_DEF:
          UnitData unitData8;
          if ((unitData8 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData8.Status.param.def);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_MND:
          UnitData unitData9;
          if ((unitData9 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData9.Status.param.mnd);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_SPEED:
          UnitData unitData10;
          if ((unitData10 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData10.Status.param.spd);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_LUCK:
          UnitData unitData11;
          if ((unitData11 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData11.Status.param.luk);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOBNAME:
          UnitData unitData12;
          if ((unitData12 = this.GetUnitData()) != null && unitData12.CurrentJob != null)
          {
            this.SetTextValue(unitData12.CurrentJob.Name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOBRANK:
          UnitData unitData13;
          if ((unitData13 = this.GetUnitData()) != null)
          {
            int rank = unitData13.CurrentJob.Rank;
            int jobRankCap = unitData13.GetJobRankCap();
            this.SetTextValue(rank);
            this.SetSliderValue(rank, jobRankCap);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_ELEMENT:
          UnitData unitData14;
          if ((unitData14 = this.GetUnitData()) != null)
          {
            int element = (int) unitData14.Element;
            this.SetAnimatorInt("element", element);
            this.SetImageIndex(element);
            break;
          }
          UnitParam unitParam2;
          if ((unitParam2 = this.GetUnitParam()) != null)
          {
            int element = (int) unitParam2.element;
            this.SetAnimatorInt("element", element);
            this.SetImageIndex(element);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PARTY_TOTALATK:
          PartyData partyData3;
          if ((partyData3 = this.GetPartyData()) != null)
          {
            GameManager instance5 = MonoSingleton<GameManager>.Instance;
            int num = 0;
            for (int index = 0; index < partyData3.MAX_UNIT; ++index)
            {
              long unitUniqueId = partyData3.GetUnitUniqueID(index);
              UnitData unitDataByUniqueId = instance5.Player.FindUnitDataByUniqueID(unitUniqueId);
              if (unitDataByUniqueId != null)
              {
                JobData jobFor = unitDataByUniqueId.GetJobFor(partyData3.PartyType);
                int jobIndex = unitDataByUniqueId.JobIndex;
                if (jobFor != unitDataByUniqueId.CurrentJob)
                  unitDataByUniqueId.SetJob(jobFor);
                num = num + (int) unitDataByUniqueId.Status.param.atk + (int) unitDataByUniqueId.Status.param.mag;
                if (unitDataByUniqueId.JobIndex != jobIndex)
                  unitDataByUniqueId.SetJobIndex(jobIndex);
              }
            }
            if (this.InstanceType == 1)
            {
              string selectedQuestId = GlobalVars.SelectedQuestID;
              if (!string.IsNullOrEmpty(selectedQuestId))
              {
                QuestParam quest = MonoSingleton<GameManager>.Instance.FindQuest(selectedQuestId);
                if (quest != null && quest.units.IsNotNull())
                {
                  for (int index = 0; index < quest.units.Length; ++index)
                  {
                    UnitData unitDataByUnitId = MonoSingleton<GameManager>.Instance.Player.FindUnitDataByUnitID(quest.units.Get(index));
                    if (unitDataByUnitId != null)
                    {
                      JobData jobFor = unitDataByUnitId.GetJobFor(partyData3.PartyType);
                      int jobIndex = unitDataByUnitId.JobIndex;
                      if (jobFor != unitDataByUnitId.CurrentJob)
                        unitDataByUnitId.SetJob(jobFor);
                      num = num + (int) unitDataByUnitId.Status.param.atk + (int) unitDataByUnitId.Status.param.mag;
                      if (jobIndex != unitDataByUnitId.JobIndex)
                        unitDataByUnitId.SetJobIndex(jobIndex);
                    }
                  }
                }
              }
              supportData = (SupportData) GlobalVars.SelectedSupport;
              if (supportData != null)
              {
                SupportData supportData1 = MonoSingleton<GameManager>.Instance.Player.Supports.Find((Predicate<SupportData>) (f => f.FUID == supportData.FUID));
                if (supportData1 != null && supportData1.Unit != null)
                  num = num + (int) supportData1.Unit.Status.param.atk + (int) supportData1.Unit.Status.param.mag;
              }
            }
            this.SetTextValue(num);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.INVENTORY_ITEMNAME:
          ItemParam inventoryItemParam;
          if ((inventoryItemParam = this.GetInventoryItemParam()) != null)
          {
            this.SetTextValue(inventoryItemParam.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_NAME:
          ItemParam itemParam2;
          if ((itemParam2 = this.GetItemParam()) != null)
          {
            if (itemParam2.type == EItemType.Unit)
            {
              UnitParam unitParam3 = MonoSingleton<GameManager>.Instance.MasterParam.GetUnitParam(itemParam2.iname);
              if (unitParam3 == null)
                break;
              this.SetTextValue(unitParam3.name);
              break;
            }
            this.SetTextValue(itemParam2.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_DESC:
          ItemParam itemParam3;
          if ((itemParam3 = this.GetItemParam()) != null)
          {
            this.SetTextValue(itemParam3.Expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_SELLPRICE:
          ItemParam itemParam4;
          if ((itemParam4 = this.GetItemParam()) != null)
          {
            this.SetTextValue(itemParam4.sell);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_BUYPRICE:
          ItemParam itemParam5;
          if ((itemParam5 = this.GetItemParam()) != null)
          {
            this.SetTextValue(itemParam5.buy);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_AMOUNT:
          switch (this.InstanceType)
          {
            case 0:
              ItemData dataOfClass1;
              if ((dataOfClass1 = DataSource.FindDataOfClass<ItemData>(((Component) this).get_gameObject(), (ItemData) null)) != null)
              {
                this.SetTextValue(dataOfClass1.Num);
                this.SetSliderValue(dataOfClass1.Num, dataOfClass1.HaveCap);
                return;
              }
              break;
            case 1:
              GameManager instance6 = MonoSingleton<GameManager>.Instance;
              if (0 <= this.Index && this.Index < instance6.Player.Inventory.Length)
              {
                ItemData itemData = instance6.Player.Inventory[this.Index];
                this.SetTextValue(itemData.Num);
                this.SetSliderValue(itemData.Num, itemData.HaveCap);
                return;
              }
              break;
            case 2:
              QuestParam questParamAuto1;
              if ((questParamAuto1 = this.GetQuestParamAuto()) != null && questParamAuto1.type == QuestTypes.Tower)
              {
                TowerRewardItem towerRewardItem = this.GetTowerRewardItem();
                if (towerRewardItem == null)
                  return;
                this.SetTextValue(towerRewardItem.num);
                if (string.IsNullOrEmpty(towerRewardItem.iname) || towerRewardItem.type != TowerRewardItem.RewardType.Item)
                  return;
                ItemParam itemParam6 = MonoSingleton<GameManager>.Instance.GetItemParam(towerRewardItem.iname);
                if (itemParam6 == null)
                  return;
                this.SetSliderValue(towerRewardItem.num, itemParam6.cap);
                return;
              }
              QuestParam questParamAuto2;
              if ((questParamAuto2 = this.GetQuestParamAuto()) != null && questParamAuto2.IsVersus)
              {
                GameManager instance5 = MonoSingleton<GameManager>.Instance;
                PlayerData player = instance5.Player;
                VersusTowerParam versusTowerParam = instance5.GetCurrentVersusTowerParam(player.VersusTowerFloor + 1);
                if (versusTowerParam != null)
                {
                  this.SetTextValue((int) versusTowerParam.ArrivalItemNum);
                  string arrivalIteminame = (string) versusTowerParam.ArrivalIteminame;
                  if (string.IsNullOrEmpty(arrivalIteminame) || versusTowerParam.ArrivalItemType != VERSUS_ITEM_TYPE.item)
                    return;
                  ItemParam itemParam6 = MonoSingleton<GameManager>.Instance.GetItemParam(arrivalIteminame);
                  if (itemParam6 == null)
                    return;
                  this.SetSliderValue((int) versusTowerParam.ArrivalItemNum, itemParam6.cap);
                  return;
                }
                this.ResetToDefault();
                return;
              }
              QuestParam questParamAuto3;
              ItemParam itemParam7;
              if ((questParamAuto3 = this.GetQuestParamAuto()) != null && questParamAuto3.bonusObjective != null && (0 <= this.Index && this.Index < questParamAuto3.bonusObjective.Length) && (itemParam7 = MonoSingleton<GameManager>.Instance.GetItemParam(questParamAuto3.bonusObjective[this.Index].item)) != null)
              {
                this.SetTextValue(questParamAuto3.bonusObjective[this.Index].itemNum);
                this.SetSliderValue(questParamAuto3.bonusObjective[this.Index].itemNum, itemParam7.cap);
                return;
              }
              break;
            case 3:
              ItemParam itemParam8 = this.GetItemParam();
              if (itemParam8 != null)
              {
                ItemData itemDataByItemId = MonoSingleton<GameManager>.Instance.Player.FindItemDataByItemID(itemParam8.iname);
                if (itemDataByItemId != null)
                {
                  this.SetTextValue(itemDataByItemId.Num);
                  this.SetSliderValue(itemDataByItemId.Num, itemDataByItemId.HaveCap);
                  return;
                }
                break;
              }
              break;
            case 4:
              EnhanceMaterial dataOfClass2 = DataSource.FindDataOfClass<EnhanceMaterial>(((Component) this).get_gameObject(), (EnhanceMaterial) null);
              if (dataOfClass2 != null && dataOfClass2.item != null)
              {
                this.SetTextValue(dataOfClass2.item.Num);
                this.SetSliderValue(dataOfClass2.item.Num, dataOfClass2.item.HaveCap);
                return;
              }
              break;
            case 5:
              EnhanceEquipData dataOfClass3 = DataSource.FindDataOfClass<EnhanceEquipData>(((Component) this).get_gameObject(), (EnhanceEquipData) null);
              if (dataOfClass3 != null && dataOfClass3.equip != null)
              {
                ItemData itemDataByItemId = MonoSingleton<GameManager>.Instance.Player.FindItemDataByItemID(dataOfClass3.equip.ItemID);
                if (itemDataByItemId != null)
                {
                  this.SetTextValue(itemDataByItemId.Num);
                  this.SetSliderValue(itemDataByItemId.Num, itemDataByItemId.HaveCap);
                  return;
                }
                break;
              }
              break;
            case 6:
              SellItem dataOfClass4 = DataSource.FindDataOfClass<SellItem>(((Component) this).get_gameObject(), (SellItem) null);
              if (dataOfClass4 != null && dataOfClass4.item != null)
              {
                this.SetTextValue(dataOfClass4.item.Num);
                this.SetSliderValue(dataOfClass4.item.Num, dataOfClass4.item.HaveCap);
                return;
              }
              break;
            case 7:
              ConsumeItemData dataOfClass5 = DataSource.FindDataOfClass<ConsumeItemData>(((Component) this).get_gameObject(), (ConsumeItemData) null);
              if (dataOfClass5 != null)
              {
                this.SetTextValue(dataOfClass5.num);
                return;
              }
              break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.INVENTORY_ITEMAMOUNT:
          ItemData inventoryItemData;
          if ((inventoryItemData = this.GetInventoryItemData()) != null && inventoryItemData.Param != null)
          {
            this.SetTextValue(inventoryItemData.Num);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYER_NUMUNITS:
          GameManager instance7 = MonoSingleton<GameManager>.Instance;
          this.SetTextValue(instance7.Player.UnitNum);
          this.SetSliderValue(instance7.Player.UnitNum, instance7.Player.UnitCap);
          break;
        case GameParameter.ParameterTypes.PLAYER_MAXUNITS:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.UnitCap);
          break;
        case GameParameter.ParameterTypes.SKILL_NAME:
          SkillData skillData1;
          if ((skillData1 = this.GetSkillData()) != null)
          {
            this.SetTextValue(skillData1.SkillParam.name);
            break;
          }
          if ((skillParam = this.GetSkillParam()) != null)
          {
            this.SetTextValue(skillParam.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SKILL_ICON:
          break;
        case GameParameter.ParameterTypes.SKILL_DESCRIPTION:
          SkillData skillData2;
          if ((skillData2 = this.GetSkillData()) != null)
          {
            this.SetTextValue(skillData2.SkillParam.expr);
            break;
          }
          if ((skillParam = this.GetSkillParam()) != null)
          {
            this.SetTextValue(skillParam.expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SKILL_MP:
          SkillData skillData3;
          if ((skillData3 = this.GetSkillData()) != null)
          {
            Unit unit2;
            if ((unit2 = this.GetUnit()) != null)
            {
              this.SetTextValue(unit2.GetSkillUsedCost(skillData3));
              break;
            }
            UnitData unitData15;
            if ((unitData15 = this.GetUnitData()) != null)
            {
              this.SetTextValue(unitData15.GetSkillUsedCost(skillData3));
              break;
            }
            this.SetTextValue(skillData3.Cost);
            break;
          }
          if ((skillParam = this.GetSkillParam()) != null)
          {
            UnitData unitData15;
            if ((unitData15 = this.GetUnitData()) != null)
            {
              this.SetTextValue(unitData15.GetSkillUsedCost(skillParam));
              break;
            }
            this.SetTextValue((int) skillParam.cost);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.BATTLE_GOLD:
          if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null))
            break;
          this.SetTextValue(SceneBattle.Instance.GoldCount);
          break;
        case GameParameter.ParameterTypes.BATTLE_TREASURE:
          if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null))
            break;
          this.SetTextValue(SceneBattle.Instance.DispTreasureCount);
          break;
        case GameParameter.ParameterTypes.UNIT_MP:
          Unit unit3;
          if ((unit3 = this.GetUnit()) != null)
          {
            int gems = unit3.Gems;
            int mp = (int) unit3.MaximumStatus.param.mp;
            this.SetTextValue(gems);
            this.SetSliderValue(gems, mp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_MPMAX:
          UnitData unitData16;
          if ((unitData16 = this.GetUnitData()) != null)
          {
            OInt oint = (OInt) 0;
            Unit unit2 = this.GetUnit();
            this.SetTextValue((int) (unit2 == null ? (OInt) unitData16.Status.param.mp : (OInt) unit2.MaximumStatus.param.mp));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ABILITY_ICON:
          AbilityParam abilityParam1;
          if ((abilityParam1 = this.GetAbilityParam()) != null)
          {
            GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = AssetPath.AbilityIcon(abilityParam1);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ABILITY_NAME:
          AbilityParam abilityParam2;
          if ((abilityParam2 = this.GetAbilityParam()) != null)
          {
            this.SetTextValue(abilityParam2.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_KAKERA_ICON:
          QuestParam questParam9;
          if ((questParam9 = this.GetQuestParam()) != null)
          {
            if (UnityEngine.Object.op_Equality((UnityEngine.Object) QuestDropParam.Instance, (UnityEngine.Object) null))
              break;
            ItemParam hardDropPiece = QuestDropParam.Instance.GetHardDropPiece(questParam9.iname, GlobalVars.GetDropTableGeneratedDateTime());
            if (hardDropPiece != null && this.LoadItemIcon(hardDropPiece.icon))
              break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_EXP:
          UnitData unitData17;
          if ((unitData17 = this.GetUnitData()) != null)
          {
            GameManager instance5 = MonoSingleton<GameManager>.Instance;
            int exp = unitData17.GetExp();
            int maxValue = instance5.MasterParam.GetUnitLevelExp(unitData17.GetNextLevel()) - instance5.MasterParam.GetUnitLevelExp(unitData17.Lv);
            this.SetTextValue(exp);
            this.SetSliderValue(exp, maxValue);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_EXPMAX:
          UnitData unitData18;
          if ((unitData18 = this.GetUnitData()) != null)
          {
            GameManager instance5 = MonoSingleton<GameManager>.Instance;
            this.SetTextValue(instance5.MasterParam.GetUnitLevelExp(unitData18.GetNextLevel()) - instance5.MasterParam.GetUnitLevelExp(unitData18.Lv));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_EXPTOGO:
          UnitData unitData19;
          if ((unitData19 = this.GetUnitData()) != null)
          {
            this.SetTextValue(unitData19.GetNextExp());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_KAKERA_NUM:
          UnitData unitData20;
          if ((unitData20 = this.GetUnitData()) != null)
          {
            int pieces = unitData20.GetPieces();
            int maxValue = unitData20.AwakeLv >= unitData20.GetAwakeLevelCap() ? pieces : unitData20.GetAwakeNeedPieces();
            this.SetTextValue(pieces);
            this.SetSliderValue(pieces, maxValue);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_KAKERA_MAX:
          UnitData unitData21;
          if ((unitData21 = this.GetUnitData()) != null)
          {
            this.SetTextValue(unitData21.GetAwakeNeedPieces());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_EXP:
          EquipData equipData1;
          if ((equipData1 = this.GetEquipData()) != null && equipData1.IsValid())
          {
            int exp = equipData1.GetExp();
            int nextExp = equipData1.GetNextExp();
            this.SetTextValue(exp);
            this.SetSliderValue(exp, nextExp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_EXPMAX:
          EquipData equipData2;
          if ((equipData2 = this.GetEquipData()) != null && equipData2.IsValid())
          {
            this.SetTextValue(equipData2.GetNextExp());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_RANK:
          EquipData equipData3;
          if ((equipData3 = this.GetEquipData()) != null && equipData3.IsValid())
          {
            this.SetTextValue(equipData3.Rank);
            this.SetAnimatorInt("rank", equipData3.Rank);
            this.SetImageIndex(equipData3.Rank);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ABILITY_RANK:
          AbilityData abilityData1;
          if ((abilityData1 = this.GetAbilityData()) != null)
          {
            this.SetTextValue(abilityData1.Rank);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.OBSOLETE_ABILITY:
          break;
        case GameParameter.ParameterTypes.ABILITY_NEXTGOLD:
          AbilityData abilityData2;
          if ((abilityData2 = this.GetAbilityData()) != null)
          {
            this.SetTextValue(abilityData2.GetNextGold());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ABILITY_SLOT:
          AbilityParam abilityParam3;
          if ((abilityParam3 = this.GetAbilityParam()) != null)
          {
            if (abilityParam3.type_detail != EAbilityTypeDetail.Default)
            {
              this.SetAnimatorInt("type", (int) abilityParam3.slot);
              if (this.GetImageLength() <= 3)
              {
                this.SetImageIndex((int) abilityParam3.slot);
                break;
              }
              this.SetImageIndex(GameParameter.AbilityTypeDetailToImageIndex(abilityParam3.slot, abilityParam3.type_detail));
              break;
            }
            this.SetAnimatorInt("type", (int) abilityParam3.slot);
            this.SetImageIndex((int) abilityParam3.slot);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOB_JOBICON:
        case GameParameter.ParameterTypes.UNIT_JOBICON:
        case GameParameter.ParameterTypes.UNIT_JOBICON2:
        case GameParameter.ParameterTypes.UNIT_JOB_JOBICON2:
        case GameParameter.ParameterTypes.UNIT_JOB_CLASSCHANGE_JOBICON:
        case GameParameter.ParameterTypes.UNIT_JOB_CLASSCHANGE_JOBICON2:
label_326:
          UnitData unitData22;
          JobParam job1 = (unitData22 = this.GetUnitData()) == null || !unitData22.IsJobAvailable(this.Index) ? this.GetJobParam() : (this.ParameterType == GameParameter.ParameterTypes.UNIT_JOB_CLASSCHANGE_JOBICON || this.ParameterType == GameParameter.ParameterTypes.UNIT_JOB_CLASSCHANGE_JOBICON2 ? unitData22.GetClassChangeJobParam(this.Index) : (this.ParameterType == GameParameter.ParameterTypes.UNIT_JOBICON || this.ParameterType == GameParameter.ParameterTypes.UNIT_JOBICON2 || this.ParameterType == GameParameter.ParameterTypes.UNIT_JOBICON2_BUGFIX ? unitData22.CurrentJob.Param : unitData22.Jobs[this.Index].Param));
          if (job1 != null)
          {
            string str = this.ParameterType == GameParameter.ParameterTypes.UNIT_JOB_JOBICON2 || this.ParameterType == GameParameter.ParameterTypes.UNIT_JOB_CLASSCHANGE_JOBICON2 || this.ParameterType == GameParameter.ParameterTypes.UNIT_JOBICON2_BUGFIX ? AssetPath.JobIconMedium(job1) : AssetPath.JobIconSmall(job1);
            if (string.IsNullOrEmpty(str))
              break;
            GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str;
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOB_RANK:
          UnitData unitData23;
          if ((unitData23 = this.GetUnitData()) != null && unitData23.IsJobAvailable(this.Index))
          {
            int rank = unitData23.Jobs[this.Index].Rank;
            int jobRankCap = unitData23.GetJobRankCap();
            this.SetTextValue(rank);
            this.SetSliderValue(rank, jobRankCap);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOB_NAME:
          UnitData unitData24;
          if ((unitData24 = this.GetUnitData()) != null && unitData24.IsJobAvailable(this.Index))
          {
            this.SetTextValue(unitData24.Jobs[this.Index].Name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOB_RANKMAX:
          UnitData unitData25;
          if ((unitData25 = this.GetUnitData()) != null && unitData25.IsJobAvailable(this.Index))
          {
            this.SetTextValue(unitData25.Jobs[this.Index].GetJobRankCap(unitData25));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_HP:
          EquipData equipData4;
          if ((equipData4 = this.GetEquipData()) != null)
          {
            int num = equipData4.Skill == null ? 0 : equipData4.Skill.GetBuffEffectValue(ParamTypes.Hp, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_AP:
          EquipData equipData5;
          if ((equipData5 = this.GetEquipData()) != null)
          {
            int num = equipData5.Skill == null ? 0 : equipData5.Skill.GetBuffEffectValue(ParamTypes.Mp, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_IAP:
          EquipData equipData6;
          if ((equipData6 = this.GetEquipData()) != null)
          {
            int num = equipData6.Skill == null ? 0 : equipData6.Skill.GetBuffEffectValue(ParamTypes.MpIni, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_ATK:
          EquipData equipData7;
          if ((equipData7 = this.GetEquipData()) != null)
          {
            int num = equipData7.Skill == null ? 0 : equipData7.Skill.GetBuffEffectValue(ParamTypes.Atk, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_DEF:
          EquipData equipData8;
          if ((equipData8 = this.GetEquipData()) != null)
          {
            int num = equipData8.Skill == null ? 0 : equipData8.Skill.GetBuffEffectValue(ParamTypes.Def, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_MAG:
          EquipData equipData9;
          if ((equipData9 = this.GetEquipData()) != null)
          {
            int num = equipData9.Skill == null ? 0 : equipData9.Skill.GetBuffEffectValue(ParamTypes.Mag, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_MND:
          EquipData equipData10;
          if ((equipData10 = this.GetEquipData()) != null)
          {
            int num = equipData10.Skill == null ? 0 : equipData10.Skill.GetBuffEffectValue(ParamTypes.Mnd, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_REC:
          EquipData equipData11;
          if ((equipData11 = this.GetEquipData()) != null)
          {
            int num = equipData11.Skill == null ? 0 : equipData11.Skill.GetBuffEffectValue(ParamTypes.Rec, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_SPD:
          EquipData equipData12;
          if ((equipData12 = this.GetEquipData()) != null)
          {
            int num = equipData12.Skill == null ? 0 : equipData12.Skill.GetBuffEffectValue(ParamTypes.Spd, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_CRI:
          EquipData equipData13;
          if ((equipData13 = this.GetEquipData()) != null)
          {
            int num = equipData13.Skill == null ? 0 : equipData13.Skill.GetBuffEffectValue(ParamTypes.Cri, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_LUK:
          EquipData equipData14;
          if ((equipData14 = this.GetEquipData()) != null)
          {
            int num = equipData14.Skill == null ? 0 : equipData14.Skill.GetBuffEffectValue(ParamTypes.Luk, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_MOV:
          EquipData equipData15;
          if ((equipData15 = this.GetEquipData()) != null)
          {
            int num = equipData15.Skill == null ? 0 : equipData15.Skill.GetBuffEffectValue(ParamTypes.Mov, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_JMP:
          EquipData equipData16;
          if ((equipData16 = this.GetEquipData()) != null)
          {
            int num = equipData16.Skill == null ? 0 : equipData16.Skill.GetBuffEffectValue(ParamTypes.Jmp, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_RANGE:
          EquipData equipData17;
          if ((equipData17 = this.GetEquipData()) != null)
          {
            int num = equipData17.Skill == null ? 0 : equipData17.Skill.GetBuffEffectValue(ParamTypes.EffectRange, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_SCOPE:
          EquipData equipData18;
          if ((equipData18 = this.GetEquipData()) != null)
          {
            int num = equipData18.Skill == null ? 0 : equipData18.Skill.GetBuffEffectValue(ParamTypes.EffectScope, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_EFFECTHEIGHT:
          EquipData equipData19;
          if ((equipData19 = this.GetEquipData()) != null)
          {
            int num = equipData19.Skill == null ? 0 : equipData19.Skill.GetBuffEffectValue(ParamTypes.EffectHeight, SkillEffectTargets.Target);
            this.SetTextValue(num);
            this.ToggleEmpty(num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_NAME:
          EquipData equipData20;
          if ((equipData20 = this.GetEquipData()) != null && equipData20.IsValid())
          {
            this.SetTextValue(equipData20.ItemParam.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_ICON:
          EquipData equipData21;
          if ((equipData21 = this.GetEquipData()) != null && equipData21.IsValid() && (!string.IsNullOrEmpty(equipData21.ItemParam.icon) && this.LoadItemIcon(equipData21.ItemParam.icon)))
            break;
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_NUM:
          break;
        case GameParameter.ParameterTypes.OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_RANKUPCOUNT:
          break;
        case GameParameter.ParameterTypes.OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_RANKUPCOUNTMAX:
          break;
        case GameParameter.ParameterTypes.OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_COOLDOWNTIME:
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_AMOUNT:
          EquipData equipData22;
          if ((equipData22 = this.GetEquipData()) != null)
          {
            this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.GetItemAmount(equipData22.ItemID));
            this.SetUpdateInterval(1f);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_REQLV:
          ItemParam itemParam9;
          if ((itemParam9 = this.GetItemParam()) != null)
          {
            this.SetTextValue(itemParam9.equipLv);
            break;
          }
          EquipData equipData23;
          if ((equipData23 = this.GetEquipData()) != null)
          {
            this.SetTextValue(equipData23.ItemParam.equipLv);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.JOBEVOITEM_AMOUNT:
          JobEvolutionRecipe dataOfClass6;
          if ((dataOfClass6 = DataSource.FindDataOfClass<JobEvolutionRecipe>(((Component) this).get_gameObject(), (JobEvolutionRecipe) null)) != null)
          {
            this.SetTextValue(dataOfClass6.Amount);
            this.SetSliderValue(dataOfClass6.Amount, dataOfClass6.RequiredAmount);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.JOBEVOITEM_REQAMOUNT:
          JobEvolutionRecipe dataOfClass7;
          if ((dataOfClass7 = DataSource.FindDataOfClass<JobEvolutionRecipe>(((Component) this).get_gameObject(), (JobEvolutionRecipe) null)) != null)
          {
            this.SetTextValue(dataOfClass7.RequiredAmount);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.JOBEVOITEM_ICON:
          JobEvolutionRecipe dataOfClass8;
          if ((dataOfClass8 = DataSource.FindDataOfClass<JobEvolutionRecipe>(((Component) this).get_gameObject(), (JobEvolutionRecipe) null)) != null && this.LoadItemIcon(dataOfClass8.Item.icon))
            break;
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.JOBEVOITEM_NAME:
          JobEvolutionRecipe dataOfClass9;
          if ((dataOfClass9 = DataSource.FindDataOfClass<JobEvolutionRecipe>(((Component) this).get_gameObject(), (JobEvolutionRecipe) null)) != null)
          {
            this.SetTextValue(dataOfClass9.Item.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_EVOCOST:
          RecipeParam dataOfClass10 = DataSource.FindDataOfClass<RecipeParam>(((Component) this).get_gameObject(), (RecipeParam) null);
          if (dataOfClass10 != null)
          {
            this.SetTextValue(dataOfClass10.cost);
            this.SetSliderValue(MonoSingleton<GameManager>.Instance.Player.Gold, dataOfClass10.cost);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_CRIT:
          UnitData unitData26;
          if ((unitData26 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData26.Status.param.cri);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_REGEN:
          UnitData unitData27;
          if ((unitData27 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData27.Status.param.rec);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_LEADERSKILLNAME:
          UnitData unitData28;
          if ((unitData28 = this.GetUnitData()) != null && unitData28.LeaderSkill != null)
          {
            this.SetTextValue(unitData28.LeaderSkill.Name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_LEADERSKILLDESC:
          UnitData unitData29;
          if ((unitData29 = this.GetUnitData()) != null && unitData29.LeaderSkill != null)
          {
            this.SetTextValue(unitData29.LeaderSkill.SkillParam.expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_VALUE:
          ItemParam itemParam10;
          if ((itemParam10 = this.GetItemParam()) != null)
          {
            this.SetTextValue(itemParam10.value);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_LEVELMAX:
          UnitData unitData30;
          if ((unitData30 = this.GetUnitData()) != null)
          {
            this.SetTextValue(unitData30.GetLevelCap(false));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOB_UNLOCKSTATE:
          UnitData unitData31;
          if ((unitData31 = this.GetUnitData()) != null && 0 <= this.Index && this.Index < unitData31.Jobs.Length)
          {
            this.SetAnimatorBool("unlocked", unitData31.Jobs[this.Index].IsActivated);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_JOBRANKMAX:
          UnitData unitData32;
          if ((unitData32 = this.GetUnitData()) != null)
          {
            int rank = unitData32.CurrentJob.Rank;
            int jobRankCap = unitData32.GetJobRankCap();
            this.SetTextValue(jobRankCap);
            this.SetSliderValue(rank, jobRankCap);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ABILITY_UNLOCKINFO:
          AbilityUnlockInfo dataOfClass11 = DataSource.FindDataOfClass<AbilityUnlockInfo>(((Component) this).get_gameObject(), (AbilityUnlockInfo) null);
          if (dataOfClass11 != null)
          {
            this.SetTextValue(string.Format(LocalizedText.Get("sys.ABILITY_UNLOCK_RANK"), (object) dataOfClass11.JobName, (object) dataOfClass11.Rank));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ABILITY_DESC:
          AbilityParam abilityParam4;
          if ((abilityParam4 = this.GetAbilityParam()) != null)
          {
            this.SetTextValue(abilityParam4.expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_FRAME:
          this.SetItemFrame(this.GetItemParam());
          break;
        case GameParameter.ParameterTypes.INVENTORY_FRAME:
          this.SetItemFrame(this.GetInventoryItemParam());
          break;
        case GameParameter.ParameterTypes.RECIPEITEM_AMOUNT:
          RecipeItemParameter dataOfClass12;
          if ((dataOfClass12 = DataSource.FindDataOfClass<RecipeItemParameter>(((Component) this).get_gameObject(), (RecipeItemParameter) null)) != null)
          {
            dataOfClass12.Amount = MonoSingleton<GameManager>.Instance.Player.GetItemAmount(dataOfClass12.Item.iname);
            this.SetTextValue(dataOfClass12.Amount);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.RECIPEITEM_REQAMOUNT:
          RecipeItemParameter dataOfClass13;
          if ((dataOfClass13 = DataSource.FindDataOfClass<RecipeItemParameter>(((Component) this).get_gameObject(), (RecipeItemParameter) null)) != null)
          {
            this.SetTextValue(dataOfClass13.RequiredAmount);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.RECIPEITEM_ICON:
          RecipeItemParameter dataOfClass14;
          if ((dataOfClass14 = DataSource.FindDataOfClass<RecipeItemParameter>(((Component) this).get_gameObject(), (RecipeItemParameter) null)) != null && this.LoadItemIcon(dataOfClass14.Item.icon))
            break;
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.RECIPEITEM_NAME:
          RecipeItemParameter dataOfClass15;
          if ((dataOfClass15 = DataSource.FindDataOfClass<RecipeItemParameter>(((Component) this).get_gameObject(), (RecipeItemParameter) null)) != null)
          {
            this.SetTextValue(dataOfClass15.Item.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.RECIPEITEM_CREATE_COST:
          RecipeItemParameter dataOfClass16;
          if ((dataOfClass16 = DataSource.FindDataOfClass<RecipeItemParameter>(((Component) this).get_gameObject(), (RecipeItemParameter) null)) != null)
          {
            RecipeParam recipeParam = MonoSingleton<GameManager>.Instance.GetRecipeParam(dataOfClass16.Item.recipe);
            if (recipeParam != null)
            {
              this.SetTextValue(recipeParam.cost);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.RECIPEITEM_CREATE_ITEM_NAME:
          RecipeItemParameter dataOfClass17;
          if ((dataOfClass17 = DataSource.FindDataOfClass<RecipeItemParameter>(((Component) this).get_gameObject(), (RecipeItemParameter) null)) != null)
          {
            this.SetTextValue(dataOfClass17.Item.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.RECIPEITEM_FRAME:
          RecipeItemParameter dataOfClass18;
          if ((dataOfClass18 = DataSource.FindDataOfClass<RecipeItemParameter>(((Component) this).get_gameObject(), (RecipeItemParameter) null)) == null)
            break;
          this.SetItemFrame(dataOfClass18.Item);
          break;
        case GameParameter.ParameterTypes.UNIT_PORTRAIT_MEDIUM:
          UnitData unitData33;
          if ((unitData33 = this.GetUnitData()) != null)
          {
            GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = AssetPath.UnitSkinIconMedium(unitData33.UnitParam, unitData33.GetSelectedSkin(-1), unitData33.CurrentJobId);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUESTRESULT_GOLD:
          BattleCore.Record dataOfClass19 = DataSource.FindDataOfClass<BattleCore.Record>(((Component) this).get_gameObject(), (BattleCore.Record) null);
          if (dataOfClass19 != null)
          {
            this.SetTextValue((int) dataOfClass19.gold);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUESTRESULT_PLAYEREXP:
          BattleCore.Record dataOfClass20 = DataSource.FindDataOfClass<BattleCore.Record>(((Component) this).get_gameObject(), (BattleCore.Record) null);
          if (dataOfClass20 != null)
          {
            this.SetTextValue((int) dataOfClass20.playerexp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUESTRESULT_PARTYEXP:
          BattleCore.Record dataOfClass21 = DataSource.FindDataOfClass<BattleCore.Record>(((Component) this).get_gameObject(), (BattleCore.Record) null);
          if (dataOfClass21 != null)
          {
            this.SetTextValue((int) dataOfClass21.unitexp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUESTRESULT_RATE:
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_LEVEL:
          PlayerLevelUpInfo levelUpInfo1;
          if ((levelUpInfo1 = this.GetLevelUpInfo()) != null)
          {
            this.SetTextValue(levelUpInfo1.LevelPrev);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_LEVELNEXT:
          PlayerLevelUpInfo levelUpInfo2;
          if ((levelUpInfo2 = this.GetLevelUpInfo()) != null)
          {
            this.SetTextValue(levelUpInfo2.LevelNext);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_STAMINA:
          PlayerLevelUpInfo levelUpInfo3;
          if ((levelUpInfo3 = this.GetLevelUpInfo()) != null)
          {
            this.SetTextValue(levelUpInfo3.StaminaNext);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_STAMINAMAX:
          PlayerLevelUpInfo levelUpInfo4;
          if ((levelUpInfo4 = this.GetLevelUpInfo()) != null)
          {
            this.SetTextValue(levelUpInfo4.StaminaMaxPrev);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_STAMINAMAXNEXT:
          PlayerLevelUpInfo levelUpInfo5;
          if ((levelUpInfo5 = this.GetLevelUpInfo()) != null)
          {
            this.SetTextValue(levelUpInfo5.StaminaMaxNext);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_FRIENDNUM:
          PlayerLevelUpInfo levelUpInfo6;
          if ((levelUpInfo6 = this.GetLevelUpInfo()) != null)
          {
            this.SetTextValue(levelUpInfo6.MaxFriendNumPrev);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_FRIENDNUMNEXT:
          PlayerLevelUpInfo levelUpInfo7;
          if ((levelUpInfo7 = this.GetLevelUpInfo()) != null)
          {
            this.SetTextValue(levelUpInfo7.MaxFriendNumNext);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYERLEVELUP_UNLOCKINFO:
          PlayerLevelUpInfo levelUpInfo8;
          if ((levelUpInfo8 = this.GetLevelUpInfo()) != null && 0 <= this.Index && this.Index < levelUpInfo8.UnlockInfo.Length)
          {
            this.SetTextValue(levelUpInfo8.UnlockInfo[this.Index]);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_BONUSOBJECTIVE_STATE:
          QuestParam questParam10;
          if ((questParam10 = this.GetQuestParam()) != null && questParam10.bonusObjective != null && (0 <= this.Index && this.Index < questParam10.bonusObjective.Length))
          {
            int index = (questParam10.clear_missions & 1 << this.Index) == 0 ? 0 : 1;
            this.SetAnimatorInt("state", index);
            this.SetImageIndex(index);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_SIDE:
          Unit unit4;
          if ((unit4 = this.GetUnit()) != null)
          {
            this.SetImageIndex((int) unit4.Side);
            this.SetAnimatorInt("index", (int) unit4.Side);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_CREATECOST:
          ItemParam itemParam11;
          if ((itemParam11 = this.GetItemParam()) != null)
          {
            this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.GetCreateItemCost(itemParam11));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_CAVESTAMINA:
          GameManager instance8 = MonoSingleton<GameManager>.Instance;
          instance8.Player.UpdateCaveStamina();
          this.SetTextValue(instance8.Player.CaveStamina);
          this.SetSliderValue(instance8.Player.CaveStamina, instance8.Player.CaveStaminaMax);
          this.SetUpdateInterval(1f);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_CAVESTAMINAMAX:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.CaveStaminaMax);
          this.SetUpdateInterval(1f);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_CAVESTAMINATIME:
          this.SetTextValue(TimeManager.ToMinSecString(MonoSingleton<GameManager>.Instance.Player.GetNextCaveStaminaRecoverySec()));
          this.SetUpdateInterval(1f);
          break;
        case GameParameter.ParameterTypes.ITEM_AMOUNTMAX:
          ItemParam itemParam12;
          if ((itemParam12 = this.GetItemParam()) != null)
          {
            this.SetTextValue(itemParam12.cap);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYER_NUMITEMS:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.GetItemSlotAmount());
          break;
        case GameParameter.ParameterTypes.QUEST_DIFFICULTY:
          QuestParam questParam11;
          if ((questParam11 = this.GetQuestParam()) != null)
          {
            int index = 0;
            switch (questParam11.difficulty)
            {
              case QuestDifficulties.Normal:
                index = 0;
                break;
              case QuestDifficulties.Elite:
                index = 1;
                break;
              case QuestDifficulties.Extra:
                index = 2;
                break;
            }
            this.SetImageIndex(index);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_HEIGHT:
          Unit unit5;
          if ((unit5 = this.GetUnit()) != null)
          {
            int num = 0;
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) SceneBattle.Instance, (UnityEngine.Object) null))
              num = SceneBattle.Instance.GetDisplayHeight(unit5);
            else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) MultiPlayVersusReady.Instance, (UnityEngine.Object) null))
              num = MultiPlayVersusReady.Instance.GetDisplayHeight(unit5);
            this.SetTextValue(num);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPMENT_FRAME:
          EquipData equipData24;
          if ((equipData24 = this.GetEquipData()) != null && equipData24.IsValid())
          {
            this.SetItemFrame(equipData24.ItemParam);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUESTLIST_CHAPTERNAME:
          ChapterParam dataOfClass22 = DataSource.FindDataOfClass<ChapterParam>(((Component) this).get_gameObject(), (ChapterParam) null);
          if (dataOfClass22 != null)
          {
            this.SetTextValue(dataOfClass22.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUESTLIST_SECTIONNAME:
          ChapterParam dataOfClass23 = DataSource.FindDataOfClass<ChapterParam>(((Component) this).get_gameObject(), (ChapterParam) null);
          if (dataOfClass23 != null)
          {
            this.SetTextValue(dataOfClass23.sectionParam != null ? dataOfClass23.sectionParam.name : string.Empty);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MAIL_MESSAGE:
          MailData mailData1;
          if ((mailData1 = this.GetMailData()) != null)
          {
            this.SetTextValue(mailData1.msg);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_MULTI_TYPE:
          if ((questParam1 = this.GetQuestParam()) != null)
          {
            this.SetImageIndex(!GlobalVars.SelectedMultiPlayQuestIsEvent ? 0 : 1);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_NAME:
          JSON_MyPhotonPlayerParam roomPlayerParam1 = this.GetRoomPlayerParam();
          if (roomPlayerParam1 == null)
          {
            this.SetTextValue(string.Empty);
            break;
          }
          this.SetTextValue(roomPlayerParam1.playerName);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_LEVEL:
          JSON_MyPhotonPlayerParam roomPlayerParam2 = this.GetRoomPlayerParam();
          if (roomPlayerParam2 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(roomPlayerParam2.playerLevel);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_STATE:
          JSON_MyPhotonPlayerParam roomPlayerParam3 = this.GetRoomPlayerParam();
          if (roomPlayerParam3 != null)
          {
            MyPhoton instance5 = PunMonoSingleton<MyPhoton>.Instance;
            bool flag = false;
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance5, (UnityEngine.Object) null))
            {
              List<MyPhoton.MyPlayer> roomPlayerList = instance5.GetRoomPlayerList();
              if (roomPlayerList != null)
              {
                MyPhoton.MyPlayer player = instance5.FindPlayer(roomPlayerList, roomPlayerParam3.playerID, roomPlayerParam3.playerIndex);
                if (player != null)
                  flag = player.start;
              }
            }
            if (this.Index == 0)
            {
              ((Component) this).get_gameObject().SetActive(roomPlayerParam3.state != 0 && roomPlayerParam3.state != 4 && roomPlayerParam3.state != 5 && !flag);
              break;
            }
            if (this.Index != 1)
              break;
            ((Component) this).get_gameObject().SetActive(roomPlayerParam3.state == this.InstanceType);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_ICON:
          UnitData unit6 = this.GetMultiPlayerUnitData(this.Index)?.unit;
          if (unit6 == null)
          {
            this.ResetToDefault();
            break;
          }
          string str1 = AssetPath.UnitSkinIconSmall(unit6.UnitParam, unit6.GetSelectedSkin(-1), unit6.CurrentJob.JobID);
          if (string.IsNullOrEmpty(str1))
          {
            this.ResetToDefault();
            break;
          }
          GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str1;
          break;
        case GameParameter.ParameterTypes.MAIL_GIFT_STRING:
          MailData mailData2;
          if ((mailData2 = this.GetMailData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          string str2 = string.Empty;
          GiftData[] gifts = mailData2.gifts;
          for (int index = 0; index < gifts.Length; ++index)
          {
            if (gifts[index].coin > 0)
              str2 = str2 + LocalizedText.Get("sys.COIN") + "×" + gifts[index].coin.ToString() + LocalizedText.Get("sys.MAILBOX_ITEM_NUM");
            if (gifts[index].gold > 0)
              str2 += string.Format(LocalizedText.Get("sys.CONVERT_TO_GOLD"), (object) gifts[index].gold);
            if (gifts[index].arenacoin > 0)
              str2 = str2 + LocalizedText.Get("sys.ARENA_COIN") + "×" + gifts[index].arenacoin.ToString() + LocalizedText.Get("sys.MAILBOX_ITEM_NUM_MAI");
            if (gifts[index].multicoin > 0)
              str2 = str2 + LocalizedText.Get("sys.MULTI_COIN") + "×" + gifts[index].multicoin.ToString() + LocalizedText.Get("sys.MAILBOX_ITEM_NUM_MAI");
            if (gifts[index].kakeracoin > 0)
              str2 = str2 + LocalizedText.Get("sys.PIECE_POINT") + "×" + gifts[index].kakeracoin.ToString() + LocalizedText.Get("sys.MAILBOX_ITEM_NUM_MAI");
            if (!string.IsNullOrEmpty(gifts[index].ConceptCardIname) && gifts[index].CheckGiftTypeIncluded(GiftTypes.ConceptCard))
            {
              ConceptCardParam conceptCardParam = MonoSingleton<GameManager>.Instance.MasterParam.GetConceptCardParam(gifts[index].ConceptCardIname);
              if (conceptCardParam != null)
              {
                string str3 = "×";
                str2 = str2 + conceptCardParam.name + str3 + gifts[index].ConceptCardNum.ToString() + LocalizedText.Get("sys.MAILBOX_ITEM_NUM");
              }
            }
            if (gifts[index].iname != null && gifts[index].num > 0)
            {
              if (gifts[index].CheckGiftTypeIncluded(GiftTypes.Artifact))
              {
                ArtifactParam artifactParam = MonoSingleton<GameManager>.Instance.MasterParam.GetArtifactParam(gifts[index].iname);
                if (artifactParam != null)
                {
                  string str3 = "×";
                  str2 = str2 + artifactParam.name + str3 + gifts[index].num.ToString() + LocalizedText.Get("sys.MAILBOX_ITEM_NUM");
                }
              }
              if (gifts[index].CheckGiftTypeIncluded(GiftTypes.Item) || gifts[index].CheckGiftTypeIncluded(GiftTypes.SelectSummonTickets))
              {
                ItemParam itemParam6 = MonoSingleton<GameManager>.Instance.GetItemParam(gifts[index].iname);
                if (itemParam6 != null)
                {
                  string str3 = "×";
                  str2 = str2 + itemParam6.name + str3 + gifts[index].num.ToString() + LocalizedText.Get("sys.MAILBOX_ITEM_NUM");
                }
              }
              if (gifts[index].CheckGiftTypeIncluded(GiftTypes.Unit))
              {
                UnitParam unitParam3 = MonoSingleton<GameManager>.Instance.MasterParam.GetUnitParam(gifts[index].iname);
                if (unitParam3 != null)
                  str2 += unitParam3.name;
              }
              if (gifts[index].CheckGiftTypeIncluded(GiftTypes.Award))
              {
                AwardParam awardParam = MonoSingleton<GameManager>.Instance.GetAwardParam(gifts[index].iname);
                if (awardParam != null)
                  str2 = str2 + LocalizedText.Get("sys.MAILBOX_ITEM_AWARD") + awardParam.name;
              }
            }
            if (str2 != string.Empty && str2[str2.Length - 1] != '、')
              str2 += "、";
          }
          if (str2 != string.Empty)
            str2 = str2.Substring(0, str2.Length - 1);
          this.SetTextValue(str2);
          break;
        case GameParameter.ParameterTypes.MAIL_GIFT_LIMIT:
          MailData mailData3;
          if ((mailData3 = this.GetMailData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          DateTime dateTime = GameUtility.UnixtimeToLocalTime(mailData3.post_at);
          dateTime = dateTime.AddDays(14.0);
          string format1 = "yyyy/MM/dd HH:mm";
          this.SetTextValue(dateTime.ToString(format1));
          break;
        case GameParameter.ParameterTypes.MAIL_GIFT_GETAT:
          MailData mailData4;
          if ((mailData4 = this.GetMailData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(GameUtility.UnixtimeToLocalTime(mailData4.read).ToString("yyyy/MM/dd HH:mm:ss"));
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_LIST_COMMENT:
          MultiPlayAPIRoom room1 = this.GetRoom();
          if (room1 == null)
          {
            this.SetTextValue(string.Empty);
            break;
          }
          this.SetTextValue(!string.IsNullOrEmpty(room1.comment) ? room1.comment : string.Empty);
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_LIST_OWNER_NAME:
          MultiPlayAPIRoom room2 = this.GetRoom();
          if (room2 == null)
          {
            this.SetTextValue(string.Empty);
            break;
          }
          this.SetTextValue(room2.owner == null || string.IsNullOrEmpty(room2.owner.name) ? "???" : room2.owner.name);
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_LIST_OWNER_LV:
          MultiPlayAPIRoom room3 = this.GetRoom();
          if (room3 == null || room3.owner == null)
          {
            this.SetTextValue(string.Empty);
            break;
          }
          this.SetTextValue(room3.owner.level);
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_LIST_QUEST_NAME:
          MultiPlayAPIRoom room4 = this.GetRoom();
          QuestParam questParam12 = room4 == null || room4.quest == null || string.IsNullOrEmpty(room4.quest.iname) ? (QuestParam) null : MonoSingleton<GameManager>.Instance.FindQuest(room4.quest.iname);
          if (questParam12 == null)
          {
            this.SetTextValue(string.Empty);
            break;
          }
          this.SetTextValue(!string.IsNullOrEmpty(questParam12.name) ? questParam12.name : "ERROR");
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_LIST_LOCKED_ICON:
          MultiPlayAPIRoom room5 = this.GetRoom();
          if (room5 == null)
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(MultiPlayAPIRoom.IsLocked(room5.pwd_hash));
            break;
          }
          ((Component) this).get_gameObject().SetActive(!MultiPlayAPIRoom.IsLocked(room5.pwd_hash));
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_LIST_PLAYER_NUM:
          MultiPlayAPIRoom room6 = this.GetRoom();
          if (room6 == null)
          {
            this.SetTextValue(string.Empty);
            break;
          }
          this.SetTextValue(room6.num);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_FRIENDCODE:
          GameManager instance9 = MonoSingleton<GameManager>.Instance;
          if (instance9.Player.FUID == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(instance9.Player.FUID);
          break;
        case GameParameter.ParameterTypes.FRIEND_FRIENDCODE:
          FriendData friendData1;
          if ((friendData1 = this.GetFriendData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(friendData1.FUID);
          break;
        case GameParameter.ParameterTypes.FRIEND_NAME:
          FriendData friendData2;
          if ((friendData2 = this.GetFriendData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(friendData2.PlayerName);
          break;
        case GameParameter.ParameterTypes.FRIEND_LEVEL:
          FriendData friendData3;
          if ((friendData3 = this.GetFriendData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(friendData3.PlayerLevel.ToString());
          break;
        case GameParameter.ParameterTypes.FRIEND_LASTLOGIN:
          FriendData friendData4;
          if ((friendData4 = this.GetFriendData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          TimeSpan timeSpan1 = DateTime.Now - GameUtility.UnixtimeToLocalTime(friendData4.LastLogin);
          int days = timeSpan1.Days;
          if (days > 0)
          {
            this.SetTextValue(LocalizedText.Get("sys.LASTLOGIN_DAY", new object[1]
            {
              (object) days.ToString()
            }));
            break;
          }
          int hours = timeSpan1.Hours;
          if (hours > 0)
          {
            this.SetTextValue(LocalizedText.Get("sys.LASTLOGIN_HOUR", new object[1]
            {
              (object) hours.ToString()
            }));
            break;
          }
          this.SetTextValue(LocalizedText.Get("sys.LASTLOGIN_MINUTE", new object[1]
          {
            (object) timeSpan1.Minutes.ToString()
          }));
          break;
        case GameParameter.ParameterTypes.PLAYER_MAXITEMS:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.GetItemSlotAmount());
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_SELLPRICE:
          SellItem sellItem1 = this.GetSellItem();
          if (sellItem1 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(sellItem1.item.Sell * sellItem1.num);
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_SELLNUM:
          SellItem sellItem2 = this.GetSellItem();
          if (sellItem2 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(sellItem2.num);
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_SELLINDEX:
          SellItem sellItem3 = this.GetSellItem();
          if (sellItem3 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(sellItem3.index + 1);
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_SELLSELECTCOUNT:
          List<SellItem> sellItemList1 = this.GetSellItemList();
          if (sellItemList1 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(sellItemList1.Count);
          break;
        case GameParameter.ParameterTypes.SHOP_SELLPRICETOTAL:
          List<SellItem> sellItemList2 = this.GetSellItemList();
          if (sellItemList2 == null)
          {
            this.ResetToDefault();
            break;
          }
          int num1 = 0;
          for (int index = 0; index < sellItemList2.Count; ++index)
            num1 += sellItemList2[index].item.Sell * sellItemList2[index].num;
          this.SetTextValue(num1);
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_STATE_INVENTORY:
          SellItem sellItem4 = this.GetSellItem();
          if (sellItem4 != null)
          {
            ((Component) this).get_gameObject().SetActive(MonoSingleton<GameManager>.Instance.Player.FindInventoryByItemID(sellItem4.item.Param.iname) != null);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_BUYAMOUNT:
          ShopItem shopItem1 = this.GetShopItem();
          if (shopItem1 != null)
          {
            this.SetTextValue(shopItem1.num);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_BUYPRICE:
          LimitedShopItem limitedShopItem1 = this.GetLimitedShopItem();
          EventShopItem eventShopItem1 = this.GetEventShopItem();
          if (limitedShopItem1 != null && limitedShopItem1.isSetSaleValue)
          {
            this.SetTextValue(limitedShopItem1.saleValue);
            break;
          }
          if (eventShopItem1 != null && eventShopItem1.isSetSaleValue)
          {
            this.SetTextValue(eventShopItem1.saleValue);
            break;
          }
          int num2 = 0;
          ShopItem shopItem2 = this.GetShopItem();
          if (shopItem2 != null)
          {
            if (shopItem2.isSetSaleValue)
            {
              this.SetTextValue(shopItem2.saleValue);
              break;
            }
            ItemParam itemParam6 = this.GetItemParam();
            if (itemParam6 != null)
            {
              switch (shopItem2.saleType)
              {
                case ESaleType.Gold:
                  num2 = shopItem2.num * itemParam6.buy;
                  break;
                case ESaleType.Coin:
                case ESaleType.Coin_P:
                  num2 = shopItem2.num * itemParam6.coin;
                  break;
                case ESaleType.TourCoin:
                  num2 = shopItem2.num * itemParam6.tour_coin;
                  break;
                case ESaleType.ArenaCoin:
                  num2 = shopItem2.num * itemParam6.arena_coin;
                  break;
                case ESaleType.PiecePoint:
                  num2 = shopItem2.num * itemParam6.piece_point;
                  break;
                case ESaleType.MultiCoin:
                  num2 = shopItem2.num * itemParam6.multi_coin;
                  break;
                case ESaleType.EventCoin:
                  DebugUtility.Assert("There is no common price in the event coin.");
                  break;
              }
              this.SetTextValue(num2);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_STATE_SOLDOUT:
          ShopItem shopItem3 = this.GetShopItem();
          if (shopItem3 == null)
            break;
          ((Component) this).get_gameObject().SetActive(shopItem3.is_soldout);
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_BUYTYPEICON:
          ShopItem shopItem4 = this.GetShopItem();
          if (shopItem4 != null)
          {
            this.SetBuyPriceTypeIcon(shopItem4.saleType);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_STATE_SELLSELECT:
          SellItem sellItem5 = this.GetSellItem();
          if (sellItem5 != null)
          {
            ((Component) this).get_gameObject().SetActive(sellItem5.index != -1);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_ICONSELLNUM:
          SellItem sellItem6 = this.GetSellItem();
          if (sellItem6 != null)
          {
            this.SetTextValue(-sellItem6.num);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_STATE_ENABLEEQUIPUNIT:
          ItemParam itemParam13 = this.GetItemParam();
          if (itemParam13 != null)
          {
            List<UnitData> units = MonoSingleton<GameManager>.Instance.Player.Units;
            for (int index1 = 0; index1 < units.Count; ++index1)
            {
              for (int index2 = 0; index2 < units[index1].Jobs.Length; ++index2)
              {
                JobData job2 = units[index1].Jobs[index2];
                if (job2.IsActivated)
                {
                  int equipSlotByItemId = job2.FindEquipSlotByItemID(itemParam13.iname);
                  if (equipSlotByItemId != -1 && job2.CheckEnableEquipSlot(equipSlotByItemId))
                  {
                    ((Component) this).get_gameObject().SetActive(true);
                    return;
                  }
                }
              }
            }
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_UPDATETIME:
          FixParam fixParam1 = MonoSingleton<GameManager>.Instance.MasterParam.FixParam;
          if (fixParam1.ShopUpdateTime != null)
          {
            DateTime serverTime = TimeManager.ServerTime;
            int num3 = (int) fixParam1.ShopUpdateTime[0];
            for (int index = 0; index < fixParam1.ShopUpdateTime.Length; ++index)
            {
              if (serverTime.Hour < (int) fixParam1.ShopUpdateTime[index])
              {
                num3 = (int) fixParam1.ShopUpdateTime[index];
                break;
              }
            }
            this.SetTextValue(num3.ToString().PadLeft(2, '0') + ":00");
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYER_FRIENDREQUESTNUM:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.FollowerNum);
          break;
        case GameParameter.ParameterTypes.UNIT_JOB_CLASSCHANGE_NAME:
          UnitData unitData34;
          if ((unitData34 = this.GetUnitData()) != null && unitData34.IsJobAvailable(this.Index))
          {
            JobParam classChangeJobParam = unitData34.GetClassChangeJobParam(this.Index);
            if (classChangeJobParam != null)
            {
              this.SetTextValue(classChangeJobParam.name);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_ICONSELLNUMSHOWED:
          SellItem sellItem7 = this.GetSellItem();
          if (sellItem7 != null)
          {
            ((Component) this).get_gameObject().SetActive(sellItem7.num != 0);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PLAYER_FRIENDMAX:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.FriendCap.ToString());
          break;
        case GameParameter.ParameterTypes.PLAYER_FRIENDNUM:
          PlayerData player1 = MonoSingleton<GameManager>.Instance.Player;
          if (player1.Friends == null)
            this.ResetToDefault();
          this.SetTextValue(player1.mFriendNum);
          break;
        case GameParameter.ParameterTypes.UNIT_PROFILETEXT:
          UnitData unitData35;
          if ((unitData35 = this.GetUnitData()) != null)
          {
            this.SetTextValue(LocalizedText.Get("unit." + unitData35.UnitParam.iname + "_PROFILE"));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_IMAGE:
          UnitData unitData36;
          if ((unitData36 = this.GetUnitData()) != null && !string.IsNullOrEmpty(unitData36.UnitParam.image))
          {
            IconLoader iconLoader = GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject());
            string str3 = AssetPath.UnitSkinImage(unitData36.UnitParam, unitData36.GetSelectedSkin(-1), unitData36.CurrentJob.JobID);
            if (!string.IsNullOrEmpty(str3))
            {
              iconLoader.ResourcePath = str3;
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_LIST_PLAYER_NUM_MAX:
          MultiPlayAPIRoom room7 = this.GetRoom();
          QuestParam questParam13 = room7 == null || room7.quest == null || string.IsNullOrEmpty(room7.quest.iname) ? (QuestParam) null : MonoSingleton<GameManager>.Instance.FindQuest(room7.quest.iname);
          if (questParam13 == null)
          {
            this.SetTextValue(string.Empty);
            break;
          }
          this.SetTextValue((int) questParam13.playerNum);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_ICON_FRAME:
          JSON_MyPhotonRoomParam roomParam1 = this.GetRoomParam();
          ((Component) this).get_gameObject().SetActive(0 <= this.Index && this.Index < (roomParam1 != null ? roomParam1.GetUnitSlotNum() : 0));
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_INDEX:
          JSON_MyPhotonPlayerParam roomPlayerParam4 = this.GetRoomPlayerParam();
          if (roomPlayerParam4 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(roomPlayerParam4.playerIndex);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_IS_ROOM_OWNER:
          MyPhoton instance10 = PunMonoSingleton<MyPhoton>.Instance;
          JSON_MyPhotonPlayerParam roomPlayerParam5 = this.GetRoomPlayerParam();
          List<MyPhoton.MyPlayer> roomPlayerList1 = instance10.GetRoomPlayerList();
          MyPhoton.MyPlayer myPlayer1 = roomPlayerParam5 == null ? (MyPhoton.MyPlayer) null : instance10.FindPlayer(roomPlayerList1, roomPlayerParam5.playerID, roomPlayerParam5.playerIndex);
          ((Component) this).get_gameObject().SetActive(myPlayer1 != null && instance10.IsHost(myPlayer1.playerID));
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_IS_EMPTY:
          JSON_MyPhotonPlayerParam roomPlayerParam6 = this.GetRoomPlayerParam();
          if (roomPlayerParam6 != null)
          {
            ((Component) this).get_gameObject().SetActive(roomPlayerParam6 != null && roomPlayerParam6.playerID <= 0);
            break;
          }
          ((Component) this).get_gameObject().SetActive(true);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_IS_VALID:
          JSON_MyPhotonPlayerParam roomPlayerParam7 = this.GetRoomPlayerParam();
          ((Component) this).get_gameObject().SetActive(roomPlayerParam7 != null && roomPlayerParam7.playerID > 0);
          break;
        case GameParameter.ParameterTypes.TROPHY_NAME:
          TrophyParam dataOfClass24 = DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null);
          if (dataOfClass24 != null)
          {
            this.SetTextValue(dataOfClass24.Name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_TYPE_IS_RAID:
          ((Component) this).get_gameObject().SetActive(GlobalVars.SelectedMultiPlayRoomType == JSON_MyPhotonRoomParam.EType.RAID);
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_TYPE_IS_VERSUS:
          ((Component) this).get_gameObject().SetActive(GlobalVars.SelectedMultiPlayRoomType == JSON_MyPhotonRoomParam.EType.VERSUS);
          break;
        case GameParameter.ParameterTypes.MULTI_PARTY_TOTALATK:
          JSON_MyPhotonRoomParam roomParam2 = this.GetRoomParam();
          JSON_MyPhotonPlayerParam multiPlayerParam = GlobalVars.SelectedMultiPlayerParam;
          if (multiPlayerParam == null || multiPlayerParam.units == null || roomParam2 == null)
          {
            this.ResetToDefault();
            break;
          }
          int num4 = 0;
          int unitSlotNum1 = roomParam2.GetUnitSlotNum(multiPlayerParam.playerIndex);
          for (int index = 0; index < multiPlayerParam.units.Length; ++index)
          {
            if (multiPlayerParam.units[index].slotID < unitSlotNum1 && multiPlayerParam.units[index].unit != null)
              num4 = num4 + (int) multiPlayerParam.units[index].unit.Status.param.atk + (int) multiPlayerParam.units[index].unit.Status.param.mag;
          }
          this.SetTextValue(num4);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_PLAYER_INDEX:
          SceneBattle instance11 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance11, (UnityEngine.Object) null) || instance11.Battle == null || instance11.Battle.CurrentUnit == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(instance11.Battle.CurrentUnit.OwnerPlayerIndex);
          break;
        case GameParameter.ParameterTypes.MULTI_MY_NEXT_TURN:
          SceneBattle instance12 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance12, (UnityEngine.Object) null))
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(instance12.GetNextMyTurn());
          break;
        case GameParameter.ParameterTypes.MULTI_INPUT_TIME_LIMIT:
          SceneBattle instance13 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance13, (UnityEngine.Object) null))
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(instance13.DisplayMultiPlayInputTimeLimit);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_PLAYER_NAME:
          SceneBattle bs1 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) bs1, (UnityEngine.Object) null) || bs1.Battle == null || bs1.Battle.CurrentUnit == null)
          {
            this.ResetToDefault();
            break;
          }
          JSON_MyPhotonPlayerParam photonPlayerParam1 = PunMonoSingleton<MyPhoton>.Instance.GetMyPlayersStarted()?.Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerIndex == bs1.Battle.CurrentUnit.OwnerPlayerIndex));
          if (photonPlayerParam1 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(photonPlayerParam1.playerName);
          break;
        case GameParameter.ParameterTypes.QUEST_MULTI_LOCK:
          ((Component) this).get_gameObject().SetActive(MultiPlayAPIRoom.IsLocked(GlobalVars.EditMultiPlayRoomPassCode));
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_COMMENT:
          MyPhoton.MyRoom currentRoom1 = PunMonoSingleton<MyPhoton>.Instance.GetCurrentRoom();
          JSON_MyPhotonRoomParam myPhotonRoomParam1 = currentRoom1 != null ? JSON_MyPhotonRoomParam.Parse(currentRoom1.json) : (JSON_MyPhotonRoomParam) null;
          if (myPhotonRoomParam1 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(myPhotonRoomParam1.comment);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_PASSCODE:
          MyPhoton.MyRoom currentRoom2 = PunMonoSingleton<MyPhoton>.Instance.GetCurrentRoom();
          JSON_MyPhotonRoomParam myPhotonRoomParam2 = currentRoom2 != null ? JSON_MyPhotonRoomParam.Parse(currentRoom2.json) : (JSON_MyPhotonRoomParam) null;
          if (myPhotonRoomParam2 == null || !MultiPlayAPIRoom.IsLocked(myPhotonRoomParam2.passCode))
          {
            this.ResetToDefault();
            break;
          }
          string input = string.Format("{0:D5}", (object) myPhotonRoomParam2.roomid);
          if (this.Index == 1)
            input = Regex.Replace(input, "[0-9]", (MatchEvaluator) (m => ((char) (65296 + ((int) m.Value[0] - 48))).ToString()));
          this.SetTextValue(input);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_UNIT_SLOT_DISABLE:
          bool flag1 = false;
          if (GameUtility.GetCurrentScene() == GameUtility.EScene.HOME_MULTI)
          {
            JSON_MyPhotonRoomParam roomParam3 = this.GetRoomParam();
            if (roomParam3 == null || !string.IsNullOrEmpty(roomParam3.iname))
            {
              JSON_MyPhotonPlayerParam roomPlayerParam8 = this.GetRoomPlayerParam();
              int playerIndex = roomPlayerParam8 != null ? roomPlayerParam8.playerIndex : 0;
              flag1 = this.Index >= (roomParam3 != null ? roomParam3.GetUnitSlotNum(playerIndex) : 0);
            }
          }
          ((Component) this).get_gameObject().SetActive(flag1);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_QUEST_NAME:
          JSON_MyPhotonRoomParam roomParam4 = this.GetRoomParam();
          if (roomParam4 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(MonoSingleton<GameManager>.Instance.FindQuest(roomParam4.iname).name);
          break;
        case GameParameter.ParameterTypes.QUEST_IS_MULTI:
          GameUtility.EScene currentScene1 = GameUtility.GetCurrentScene();
          bool flag2 = currentScene1 == GameUtility.EScene.HOME_MULTI || currentScene1 == GameUtility.EScene.BATTLE_MULTI;
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(!flag2);
            break;
          }
          if (this.Index == 1)
          {
            ((Component) this).get_gameObject().SetActive(flag2);
            break;
          }
          Button component1 = (Button) ((Component) this).get_gameObject().GetComponent<Button>();
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) component1, (UnityEngine.Object) null))
          {
            this.ResetToDefault();
            break;
          }
          if (this.Index == 2)
          {
            ((Selectable) component1).set_interactable(!flag2);
            break;
          }
          if (this.Index != 3)
            break;
          ((Selectable) component1).set_interactable(flag2);
          break;
        case GameParameter.ParameterTypes.TROPHY_CONDITION_TITLE:
          if (this.InstanceType == 0)
          {
            TrophyObjectiveData dataOfClass25 = DataSource.FindDataOfClass<TrophyObjectiveData>(((Component) this).get_gameObject(), (TrophyObjectiveData) null);
            if (dataOfClass25 != null)
            {
              this.SetTextValue(dataOfClass25.Description);
              break;
            }
          }
          else
          {
            TrophyParam dataOfClass25 = DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null);
            if (dataOfClass25 != null && 0 <= this.Index && this.Index < dataOfClass25.Objectives.Length)
            {
              this.SetTextValue(dataOfClass25.Objectives[this.Index].GetDescription());
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.TROPHY_CONDITION_COUNT:
          if (this.InstanceType == 0)
          {
            TrophyObjectiveData dataOfClass25 = DataSource.FindDataOfClass<TrophyObjectiveData>(((Component) this).get_gameObject(), (TrophyObjectiveData) null);
            if (dataOfClass25 != null)
            {
              if (dataOfClass25.Objective.type == TrophyConditionTypes.up_conceptcard_trust || dataOfClass25.Objective.type == TrophyConditionTypes.up_conceptcard_trust_target)
              {
                float num3 = Mathf.Min((float) dataOfClass25.Count / 100f, (float) dataOfClass25.CountMax / 100f);
                this.SetTextValue(string.Format("{0:0.0}", (object) num3));
                this.SetSliderValue((int) num3, dataOfClass25.CountMax);
                break;
              }
              int num5 = Mathf.Min(dataOfClass25.Count, dataOfClass25.CountMax);
              this.SetTextValue(num5);
              this.SetSliderValue(num5, dataOfClass25.CountMax);
              break;
            }
          }
          else
          {
            TrophyParam dataOfClass25 = DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null);
            if (dataOfClass25 != null && 0 <= this.Index && this.Index < dataOfClass25.Objectives.Length)
            {
              TrophyState trophyCounter = MonoSingleton<GameManager>.Instance.Player.GetTrophyCounter(dataOfClass25, false);
              if (trophyCounter == null || this.Index >= trophyCounter.Count.Length)
                break;
              if (dataOfClass25.Objectives[this.Index].type == TrophyConditionTypes.up_conceptcard_trust || dataOfClass25.Objectives[this.Index].type == TrophyConditionTypes.up_conceptcard_trust_target)
              {
                float num3 = Mathf.Min((float) trophyCounter.Count[this.Index] / 100f, (float) dataOfClass25.Objectives[this.Index].RequiredCount / 100f);
                this.SetTextValue(string.Format("{0:0.0}", (object) num3));
                this.SetSliderValue((int) num3, dataOfClass25.Objectives[this.Index].RequiredCount / 100);
                break;
              }
              int num5 = Mathf.Min(trophyCounter.Count[this.Index], dataOfClass25.Objectives[this.Index].RequiredCount);
              this.SetTextValue(num5);
              this.SetSliderValue(num5, dataOfClass25.Objectives[this.Index].RequiredCount);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.TROPHY_CONDITION_COUNTMAX:
          if (this.InstanceType == 0)
          {
            TrophyObjectiveData dataOfClass25 = DataSource.FindDataOfClass<TrophyObjectiveData>(((Component) this).get_gameObject(), (TrophyObjectiveData) null);
            if (dataOfClass25 != null)
            {
              if (dataOfClass25.Objective.type == TrophyConditionTypes.up_conceptcard_trust || dataOfClass25.Objective.type == TrophyConditionTypes.up_conceptcard_trust_target)
              {
                this.SetTextValue(string.Format("{0:0.0}", (object) ((float) dataOfClass25.CountMax / 100f)));
                break;
              }
              this.SetTextValue(dataOfClass25.CountMax);
              break;
            }
          }
          else
          {
            TrophyParam dataOfClass25 = DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null);
            if (dataOfClass25 != null && 0 <= this.Index && this.Index < dataOfClass25.Objectives.Length)
            {
              if (dataOfClass25.Objectives[this.Index].type == TrophyConditionTypes.up_conceptcard_trust || dataOfClass25.Objectives[this.Index].type == TrophyConditionTypes.up_conceptcard_trust_target)
              {
                this.SetTextValue(string.Format("{0:0.0}", (object) ((float) dataOfClass25.Objectives[this.Index].ival / 100f)));
                break;
              }
              this.SetTextValue(dataOfClass25.Objectives[this.Index].RequiredCount);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_ENHANCEPOINT:
          ItemParam itemParam14 = this.GetItemParam();
          if (itemParam14 != null)
          {
            this.SetTextValue(itemParam14.enhace_point);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_MATERIALSELECTCOUNT:
          EnhanceMaterial enhanceMaterial1 = this.GetEnhanceMaterial();
          if (enhanceMaterial1 != null)
          {
            this.SetTextValue(enhanceMaterial1.num);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_SHOWED_AMOUNT:
          ItemParam itemParam15 = this.InstanceType == 1 ? this.GetInventoryItemParam() : this.GetItemParam();
          if (itemParam15 != null)
          {
            ((Component) this).get_gameObject().SetActive(MonoSingleton<GameManager>.Instance.Player.GetItemAmount(itemParam15.iname) > 0);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_PARAMETER_NAME:
          EquipItemParameter equipItemParameter1 = this.GetEquipItemParameter();
          if (equipItemParameter1 != null)
          {
            this.SetTextValue(this.GetParamTypeName(equipItemParameter1.equip.Skill.GetBuffEffect(SkillEffectTargets.Target).targets[equipItemParameter1.param_index].paramType));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_PARAMETER_INITVALUE:
          EquipItemParameter equipItemParameter2 = this.GetEquipItemParameter();
          if (equipItemParameter2 != null)
          {
            int paramIndex = equipItemParameter2.param_index;
            SkillData skill = equipItemParameter2.equip.Skill;
            if (skill != null)
            {
              BuffEffect buffEffect = skill.GetBuffEffect(SkillEffectTargets.Target);
              if (buffEffect != null && buffEffect.param != null && buffEffect.param.buffs != null)
              {
                this.SetTextValue((int) buffEffect.targets[paramIndex].value);
                break;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_PARAMETER_RANKUPVALUE:
          EquipItemParameter equipItemParameter3 = this.GetEquipItemParameter();
          if (equipItemParameter3 != null)
          {
            int paramIndex = equipItemParameter3.param_index;
            int num3 = 0;
            SkillData skill = equipItemParameter3.equip.Skill;
            if (skill != null)
            {
              BuffEffect buffEffect = skill.GetBuffEffect(SkillEffectTargets.Target);
              if (buffEffect != null && buffEffect.param != null && buffEffect.param.buffs != null)
                num3 = buffEffect == null ? 0 : (int) buffEffect.targets[paramIndex].value - (int) buffEffect.param.buffs[paramIndex].value_ini;
            }
            if (num3 != 0)
            {
              this.SetTextValue("(+" + num3.ToString() + ")");
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_PARAMETER_SHOWED_RANKUPVALUE:
          EquipItemParameter equipItemParameter4 = this.GetEquipItemParameter();
          if (equipItemParameter4 != null)
          {
            int paramIndex = equipItemParameter4.param_index;
            int num3 = 0;
            SkillData skill = equipItemParameter4.equip.Skill;
            if (skill != null)
            {
              BuffEffect buffEffect = skill.GetBuffEffect(SkillEffectTargets.Target);
              if (buffEffect != null && buffEffect.param != null && buffEffect.param.buffs != null)
                num3 = buffEffect == null ? 0 : (int) buffEffect.targets[paramIndex].value - (int) buffEffect.param.buffs[paramIndex].value_ini;
            }
            if (num3 != 0)
            {
              ((Component) this).get_gameObject().SetActive(num3 > 0);
              break;
            }
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_SHOWED_MATERIALSELECTCOUNT:
          EnhanceMaterial enhanceMaterial2 = this.GetEnhanceMaterial();
          if (enhanceMaterial2 != null)
          {
            ((Component) this).get_gameObject().SetActive(enhanceMaterial2.num > 0);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_SHOWED_MATERIALSELECT:
          EnhanceMaterial enhanceMaterial3 = this.GetEnhanceMaterial();
          if (enhanceMaterial3 != null)
          {
            ((Component) this).get_gameObject().SetActive(enhanceMaterial3.selected);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_GAUGE:
          EnhanceEquipData enhanceEquipData1;
          if ((enhanceEquipData1 = this.GetEnhanceEquipData()) != null && enhanceEquipData1.is_enhanced)
          {
            EquipData equip = enhanceEquipData1.equip;
            int current = equip.Exp + enhanceEquipData1.gainexp;
            int num3 = equip.CalcRankFromExp(current);
            int rankCap = equip.GetRankCap();
            int num5 = num3 >= rankCap ? equip.GetNextExp(rankCap) : equip.GetExpFromExp(current);
            int nextExp = equip.GetNextExp(num3 >= rankCap ? rankCap : num3 + 1);
            this.SetTextValue(num5);
            this.SetSliderValue(num5, nextExp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_CURRENTEXP:
          EnhanceEquipData enhanceEquipData2;
          if ((enhanceEquipData2 = this.GetEnhanceEquipData()) != null)
          {
            EquipData equip = enhanceEquipData2.equip;
            int current = equip.Exp + enhanceEquipData2.gainexp;
            int num3 = equip.CalcRankFromExp(current);
            int rankCap = equip.GetRankCap();
            this.SetTextValue(num3 >= rankCap ? equip.GetNextExp(rankCap) : equip.GetExpFromExp(current));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_NEXTEXP:
          EnhanceEquipData enhanceEquipData3;
          if ((enhanceEquipData3 = this.GetEnhanceEquipData()) != null)
          {
            EquipData equip = enhanceEquipData3.equip;
            int current = equip.Exp + enhanceEquipData3.gainexp;
            int num3 = equip.CalcRankFromExp(current);
            int rankCap = equip.GetRankCap();
            this.SetTextValue(equip.GetNextExp(num3 >= rankCap ? rankCap : num3 + 1));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_RANKBEFORE:
          EnhanceEquipData enhanceEquipData4;
          if ((enhanceEquipData4 = this.GetEnhanceEquipData()) != null)
          {
            this.SetTextValue(enhanceEquipData4.equip.Rank);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPITEM_ENHANCE_RANKAFTER:
          EnhanceEquipData enhanceEquipData5;
          if ((enhanceEquipData5 = this.GetEnhanceEquipData()) != null)
          {
            EquipData equip = enhanceEquipData5.equip;
            this.SetTextValue(equip.CalcRankFromExp(equip.Exp + enhanceEquipData5.gainexp));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPDATA_RANKBADGE:
          EquipData equipData25;
          if ((equipData25 = this.GetEquipData()) != null)
          {
            int num3 = equipData25.Rank - 1;
            if (num3 > 0)
            {
              int index = num3 - 1;
              ((Component) this).get_gameObject().SetActive(true);
              this.SetAnimatorInt("rare", index);
              this.SetImageIndex(index);
              break;
            }
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNLOCK_SHOWED:
          ((Component) this).get_gameObject().SetActive(this.CheckUnlockInstanceType());
          break;
        case GameParameter.ParameterTypes.MULTI_NOTIFY_DISCONNECTED_PLAYER_INDEX:
          SceneBattle instance14 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance14, (UnityEngine.Object) null) || instance14.CurrentNotifyDisconnectedPlayer == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(instance14.CurrentNotifyDisconnectedPlayer.playerIndex);
          break;
        case GameParameter.ParameterTypes.MULTI_NOTIFY_DISCONNECTED_PLAYER_IS_ROOM_OWNER:
          SceneBattle instance15 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance15, (UnityEngine.Object) null) || instance15.CurrentNotifyDisconnectedPlayer == null)
          {
            this.ResetToDefault();
            break;
          }
          ((Component) this).get_gameObject().SetActive(instance15.CurrentNotifyDisconnectedPlayerType == (SceneBattle.ENotifyDisconnectedPlayerType) this.Index);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_PLAYER_IS_DISCONNECTED:
          SceneBattle bs2 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) bs2, (UnityEngine.Object) null) || bs2.Battle == null || bs2.Battle.CurrentUnit == null)
          {
            this.ResetToDefault();
            break;
          }
          GameManager instance16 = MonoSingleton<GameManager>.Instance;
          MyPhoton instance17 = PunMonoSingleton<MyPhoton>.Instance;
          JSON_MyPhotonPlayerParam photonPlayerParam2 = instance17.GetMyPlayersStarted()?.Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerIndex == bs2.Battle.CurrentUnit.OwnerPlayerIndex));
          List<MyPhoton.MyPlayer> roomPlayerList2 = instance17.GetRoomPlayerList();
          MyPhoton.MyPlayer myPlayer2 = photonPlayerParam2 != null ? instance17.FindPlayer(roomPlayerList2, photonPlayerParam2.playerID, photonPlayerParam2.playerIndex) : (MyPhoton.MyPlayer) null;
          if (instance16.AudienceMode || instance16.IsVSCpuBattle)
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(myPlayer2 == null);
            break;
          }
          if (this.Index == 1)
          {
            ((Component) this).get_gameObject().SetActive(myPlayer2 != null);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_PLAYER_IS_ROOM_OWNER:
          SceneBattle bs3 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) bs3, (UnityEngine.Object) null) || bs3.Battle == null || bs3.Battle.CurrentUnit == null)
          {
            this.ResetToDefault();
            break;
          }
          MyPhoton instance18 = PunMonoSingleton<MyPhoton>.Instance;
          JSON_MyPhotonPlayerParam photonPlayerParam3 = instance18.GetMyPlayersStarted()?.Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerIndex == bs3.Battle.CurrentUnit.OwnerPlayerIndex));
          ((Component) this).get_gameObject().SetActive(photonPlayerParam3 != null && instance18.IsOldestPlayer(photonPlayerParam3.playerID));
          break;
        case GameParameter.ParameterTypes.MULTI_I_AM_ROOM_OWNER:
          MyPhoton instance19 = PunMonoSingleton<MyPhoton>.Instance;
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(instance19.IsOldestPlayer());
            break;
          }
          ((Component) this).get_gameObject().SetActive(!instance19.IsOldestPlayer());
          break;
        case GameParameter.ParameterTypes.MULTI_ROOM_OWNER_PLAYER_INDEX:
          this.SetTextValue(PunMonoSingleton<MyPhoton>.Instance.GetOldestPlayer());
          break;
        case GameParameter.ParameterTypes.GACHA_DROPNAME:
          GachaResultData_old dataOfClass26 = DataSource.FindDataOfClass<GachaResultData_old>(((Component) this).get_gameObject(), (GachaResultData_old) null);
          if (dataOfClass26 == null)
            break;
          this.SetTextValue(dataOfClass26.Name);
          break;
        case GameParameter.ParameterTypes.TROPHY_BADGE:
          TrophyState[] trophyStates = MonoSingleton<GameManager>.Instance.Player.TrophyStates;
          bool flag3 = false;
          switch (this.InstanceType)
          {
            case 0:
              for (int index = 0; index < trophyStates.Length; ++index)
              {
                if (trophyStates[index].Param.IsShowBadge(trophyStates[index]))
                {
                  flag3 = true;
                  break;
                }
              }
              break;
            case 1:
              for (int index = 0; index < trophyStates.Length; ++index)
              {
                if (!trophyStates[index].Param.IsDaily && trophyStates[index].Param.IsShowBadge(trophyStates[index]))
                {
                  flag3 = true;
                  break;
                }
              }
              break;
          }
          ((Component) this).get_gameObject().SetActive(flag3);
          break;
        case GameParameter.ParameterTypes.TROPHY_REWARDGOLD:
          TrophyParam trophyParam1 = this.GetTrophyParam();
          if (trophyParam1 == null)
            break;
          this.SetTextValue(trophyParam1.Gold);
          ((Component) this).get_gameObject().SetActive(trophyParam1.Gold > 0);
          break;
        case GameParameter.ParameterTypes.TROPHY_REWARDCOIN:
          TrophyParam trophyParam2 = this.GetTrophyParam();
          if (trophyParam2 == null)
            break;
          this.SetTextValue(trophyParam2.Coin);
          ((Component) this).get_gameObject().SetActive(trophyParam2.Coin > 0);
          break;
        case GameParameter.ParameterTypes.TROPHY_REWARDEXP:
          TrophyParam trophyParam3 = this.GetTrophyParam();
          if (trophyParam3 == null)
            break;
          this.SetTextValue(trophyParam3.Exp);
          ((Component) this).get_gameObject().SetActive(trophyParam3.Exp > 0);
          break;
        case GameParameter.ParameterTypes.REWARD_EXP:
          RewardData dataOfClass27 = DataSource.FindDataOfClass<RewardData>(((Component) this).get_gameObject(), (RewardData) null);
          if (dataOfClass27 != null)
          {
            this.SetTextValue(dataOfClass27.Exp);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.REWARD_COIN:
          RewardData dataOfClass28 = DataSource.FindDataOfClass<RewardData>(((Component) this).get_gameObject(), (RewardData) null);
          if (dataOfClass28 != null)
          {
            this.SetTextValue(dataOfClass28.Coin);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.REWARD_GOLD:
          RewardData dataOfClass29 = DataSource.FindDataOfClass<RewardData>(((Component) this).get_gameObject(), (RewardData) null);
          if (dataOfClass29 != null)
          {
            this.SetTextValue(dataOfClass29.Gold);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_FAVORITE:
          UnitData unitData37 = this.GetUnitData();
          if (unitData37 != null)
          {
            ((Component) this).get_gameObject().SetActive(unitData37.IsFavorite);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.EQUIPDATA_FRAME:
          EquipData equipData26 = this.GetEquipData();
          if (equipData26 != null && equipData26.IsEquiped())
          {
            this.SetEquipItemFrame(equipData26.ItemParam);
            break;
          }
          this.SetEquipItemFrame((ItemParam) null);
          break;
        case GameParameter.ParameterTypes.UNIT_JOBRANKFRAME:
          UnitData unitData38;
          if ((unitData38 = this.GetUnitData()) != null)
          {
            this.SetImageIndex(unitData38.CurrentJob.Rank);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_CAPPEDLEVELMAX:
          UnitData unitData39;
          if ((unitData39 = this.GetUnitData()) != null)
          {
            GameManager instance5 = MonoSingleton<GameManager>.Instance;
            this.SetTextValue(Mathf.Min(unitData39.GetLevelCap(false), instance5.Player.Lv));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.APPLICATION_REVISION:
          TextAsset textAsset1 = (TextAsset) Resources.Load<TextAsset>("revision");
          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) textAsset1, (UnityEngine.Object) null))
          {
            this.SetTextValue(textAsset1.get_text());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.APPLICATION_BUILD:
          TextAsset textAsset2 = (TextAsset) Resources.Load<TextAsset>("build");
          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) textAsset2, (UnityEngine.Object) null))
          {
            this.SetTextValue(textAsset2.get_text());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.APPLICATION_ASSETREVISION:
          int assetRevision = AssetManager.AssetRevision;
          if (assetRevision > 0)
          {
            this.SetTextValue(assetRevision.ToString());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PRODUCT_NAME:
          PaymentManager.Product dataOfClass30 = DataSource.FindDataOfClass<PaymentManager.Product>(((Component) this).get_gameObject(), (PaymentManager.Product) null);
          if (dataOfClass30 == null || string.IsNullOrEmpty(dataOfClass30.name))
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(dataOfClass30.name);
          break;
        case GameParameter.ParameterTypes.PRODUCT_PRICE:
          PaymentManager.Product dataOfClass31 = DataSource.FindDataOfClass<PaymentManager.Product>(((Component) this).get_gameObject(), (PaymentManager.Product) null);
          if (dataOfClass31 == null || string.IsNullOrEmpty(dataOfClass31.price))
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(dataOfClass31.price);
          break;
        case GameParameter.ParameterTypes.ARENAPLAYER_RANK:
          ArenaPlayer arenaPlayer1 = this.GetArenaPlayer();
          if (arenaPlayer1 != null && arenaPlayer1.ArenaRank > 0)
          {
            this.SetTextValue(arenaPlayer1.ArenaRank);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARENAPLAYER_TOTALATK:
          ArenaPlayer arenaPlayer2 = this.GetArenaPlayer();
          if (arenaPlayer2 != null)
          {
            this.SetTextValue(arenaPlayer2.TotalAtk);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARENAPLAYER_LEADERSKILLNAME:
          ArenaPlayer arenaPlayer3 = this.GetArenaPlayer();
          if (arenaPlayer3 != null && arenaPlayer3.Unit[0] != null && arenaPlayer3.Unit[0].LeaderSkill != null)
          {
            this.SetTextValue(arenaPlayer3.Unit[0].LeaderSkill.Name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARENAPLAYER_LEADERSKILLDESC:
          ArenaPlayer arenaPlayer4 = this.GetArenaPlayer();
          if (arenaPlayer4 != null && arenaPlayer4.Unit[0] != null && arenaPlayer4.Unit[0].LeaderSkill != null)
          {
            this.SetTextValue(arenaPlayer4.Unit[0].LeaderSkill.SkillParam.expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARENAPLAYER_NAME:
          ArenaPlayer arenaPlayer5 = this.GetArenaPlayer();
          if (arenaPlayer5 != null)
          {
            this.SetTextValue(arenaPlayer5.PlayerName);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_ARENARANK:
          GameManager instance20 = MonoSingleton<GameManager>.Instance;
          if (instance20.Player.ArenaRank > 0)
          {
            this.SetTextValue(instance20.Player.ArenaRank);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_TICKET:
          QuestParam questParam14;
          if ((questParam14 = this.GetQuestParam()) != null && !string.IsNullOrEmpty(questParam14.ticket))
          {
            ItemData itemDataByItemId = MonoSingleton<GameManager>.Instance.Player.FindItemDataByItemID(questParam14.ticket);
            if (itemDataByItemId != null)
            {
              this.SetTextValue(itemDataByItemId.Num);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_IS_TICKET:
          QuestParam questParam15;
          if ((questParam15 = this.GetQuestParam()) != null && questParam15.state == QuestStates.Cleared && !string.IsNullOrEmpty(questParam15.ticket))
          {
            ItemData itemDataByItemId = MonoSingleton<GameManager>.Instance.Player.FindItemDataByItemID(questParam15.ticket);
            if (itemDataByItemId != null)
            {
              ((Component) this).get_gameObject().SetActive(itemDataByItemId.Num > 0);
              break;
            }
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_ARENATICKETS:
          GameManager instance21 = MonoSingleton<GameManager>.Instance;
          this.SetTextValue(instance21.Player.ChallengeArenaNum);
          this.SetSliderValue(instance21.Player.ChallengeArenaNum, instance21.Player.ChallengeArenaMax);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_ARENACOOLDOWNTIME:
          GameManager instance22 = MonoSingleton<GameManager>.Instance;
          instance22.Player.UpdateChallengeArenaTimer();
          long arenaCoolDownSec = instance22.Player.GetNextChallengeArenaCoolDownSec();
          this.SetTextValue(string.Format(LocalizedText.Get("sys.ARENA_COOLDOWN"), (object) (arenaCoolDownSec / 60L), (object) (arenaCoolDownSec % 60L)));
          this.SetUpdateInterval(0.25f);
          break;
        case GameParameter.ParameterTypes.MULTI_REST_REWARD:
          string format2 = LocalizedText.Get("sys.MP_REST_REWARD");
          if (string.IsNullOrEmpty(format2))
          {
            this.ResetToDefault();
            break;
          }
          PlayerData player2 = MonoSingleton<GameManager>.Instance.Player;
          int num6 = Math.Max(player2.ChallengeMultiMax - player2.ChallengeMultiNum, 0);
          this.SetTextValue(string.Format(format2, (object) num6));
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_UNIT_SLOT_DISABLE_NOT_INTERACTIVE:
          bool flag4 = false;
          if (GameUtility.GetCurrentScene() == GameUtility.EScene.HOME_MULTI)
          {
            JSON_MyPhotonRoomParam roomParam3 = this.GetRoomParam();
            JSON_MyPhotonPlayerParam roomPlayerParam8 = this.GetRoomPlayerParam();
            int playerIndex = roomPlayerParam8 != null ? roomPlayerParam8.playerIndex : 0;
            flag4 = this.Index >= roomParam3.GetUnitSlotNum(playerIndex);
          }
          Button component2 = (Button) ((Component) this).get_gameObject().GetComponent<Button>();
          if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component2, (UnityEngine.Object) null))
            break;
          ((Selectable) component2).set_interactable(!flag4);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_OKYAKUSAMACODE:
          string configOkyakusamaCode1 = GameUtility.Config_OkyakusamaCode;
          if (string.IsNullOrEmpty(configOkyakusamaCode1))
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(LocalizedText.Get("sys.OKYAKUSAMACODE", new object[1]
          {
            (object) configOkyakusamaCode1
          }));
          break;
        case GameParameter.ParameterTypes.REWARD_ARENAMEDAL:
          RewardData dataOfClass32 = DataSource.FindDataOfClass<RewardData>(((Component) this).get_gameObject(), (RewardData) null);
          if (dataOfClass32 != null)
          {
            this.SetTextValue(dataOfClass32.ArenaMedal);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_UPDATEDAY:
          FixParam fixParam2 = MonoSingleton<GameManager>.Instance.MasterParam.FixParam;
          if (fixParam2.ShopUpdateTime != null)
          {
            DateTime serverTime = TimeManager.ServerTime;
            for (int index = 0; index < fixParam2.ShopUpdateTime.Length; ++index)
            {
              if (serverTime.Hour < (int) fixParam2.ShopUpdateTime[index])
              {
                this.SetTextValue(LocalizedText.Get("sys.TODAY"));
                return;
              }
            }
            this.SetTextValue(LocalizedText.Get("sys.TOMORROW"));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARENAPLAYER_LEVEL:
          ArenaPlayer arenaPlayer6 = this.GetArenaPlayer();
          if (arenaPlayer6 != null)
          {
            this.SetTextValue(arenaPlayer6.PlayerLevel);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_VIPRANK:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.VipRank);
          break;
        case GameParameter.ParameterTypes.UNIT_EQUIPSLOT_UPDATE:
          UnitData unitData40;
          if ((unitData40 = this.GetUnitData()) == null)
            break;
          UnitEquipmentSlotEvents component3 = (UnitEquipmentSlotEvents) ((Component) this).get_gameObject().GetComponent<UnitEquipmentSlotEvents>();
          if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component3, (UnityEngine.Object) null))
            break;
          int index3 = this.Index;
          EquipData[] rankupEquips = unitData40.GetRankupEquips(unitData40.JobIndex);
          if (!rankupEquips[index3].IsValid())
          {
            ((Component) component3).get_gameObject().SetActive(false);
            break;
          }
          ((Component) component3).get_gameObject().SetActive(true);
          ItemParam itemParam16 = rankupEquips[index3].ItemParam;
          DataSource.Bind<ItemParam>(((Component) component3).get_gameObject(), itemParam16);
          DataSource.Bind<EquipData>(((Component) component3).get_gameObject(), rankupEquips[index3]);
          if (rankupEquips[index3].IsEquiped())
          {
            component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.Equipped;
            break;
          }
          if (MonoSingleton<GameManager>.Instance.Player.HasItem(itemParam16.iname))
          {
            if (itemParam16.equipLv > unitData40.Lv)
            {
              component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.NeedMoreLevel;
              break;
            }
            component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.HasEquipment;
            break;
          }
          if (MonoSingleton<GameManager>.Instance.Player.CheckCreateItem(itemParam16) == CreateItemResult.CanCreate)
          {
            if (itemParam16.equipLv > unitData40.Lv)
            {
              component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.EnableCraftNeedMoreLevel;
              break;
            }
            component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.EnableCraft;
            break;
          }
          if (unitData40.CheckCommon(unitData40.JobIndex, index3))
          {
            if (itemParam16.equipLv > unitData40.Lv)
            {
              component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.EnableCommonSoul;
              break;
            }
            component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.EnableCommon;
            break;
          }
          component3.StateType = UnitEquipmentSlotEvents.SlotStateTypes.Empty;
          break;
        case GameParameter.ParameterTypes.UNITPARAM_ICON:
          UnitParam unitParam4;
          if ((unitParam4 = this.GetUnitParam()) != null)
          {
            string str3 = AssetPath.UnitIconSmall(unitParam4, unitParam4.GetJobId(0));
            if (!string.IsNullOrEmpty(str3))
            {
              GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str3;
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNITPARAM_RARITY:
          UnitParam unitParam5;
          if ((unitParam5 = this.GetUnitParam()) != null)
          {
            this.SetAnimatorInt("rare", (int) unitParam5.rare);
            this.SetImageIndex((int) unitParam5.rare);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNITPARAM_JOBICON:
          UnitParam unitParam6;
          if ((unitParam6 = this.GetUnitParam()) != null)
          {
            string str3 = GameUtility.ComposeJobIconPath(unitParam6);
            if (!string.IsNullOrEmpty(str3))
            {
              GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str3;
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNITPARAM_PIECE_AMOUNT:
          UnitParam unitParam7;
          if ((unitParam7 = this.GetUnitParam()) != null)
          {
            this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.GetItemAmount(unitParam7.piece));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNITPARAM_PIECE_NEED:
          UnitParam unitParam8;
          if ((unitParam8 = this.GetUnitParam()) != null)
          {
            this.SetTextValue(unitParam8.GetUnlockNeedPieces());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNITPARAM_PIECE_GAUGE:
          UnitParam unitParam9;
          if ((unitParam9 = this.GetUnitParam()) != null)
          {
            GameManager instance5 = MonoSingleton<GameManager>.Instance;
            int unlockNeedPieces = unitParam9.GetUnlockNeedPieces();
            int num3 = Math.Min(instance5.Player.GetItemAmount(unitParam9.piece), unlockNeedPieces);
            this.SetTextValue(num3);
            this.SetSliderValue(num3, unlockNeedPieces);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNITPARAM_IS_UNLOCKED:
          UnitParam unitParam10;
          if ((unitParam10 = this.GetUnitParam()) != null)
          {
            GameManager instance5 = MonoSingleton<GameManager>.Instance;
            int unlockNeedPieces = unitParam10.GetUnlockNeedPieces();
            ((Component) this).get_gameObject().SetActive(instance5.Player.GetItemAmount(unitParam10.piece) >= unlockNeedPieces);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.QUEST_KAKERA_FRAME:
          QuestParam questParam16;
          if ((questParam16 = this.GetQuestParam()) != null)
          {
            if (UnityEngine.Object.op_Equality((UnityEngine.Object) QuestDropParam.Instance, (UnityEngine.Object) null))
              break;
            ItemParam hardDropPiece = QuestDropParam.Instance.GetHardDropPiece(questParam16.iname, GlobalVars.GetDropTableGeneratedDateTime());
            if (hardDropPiece != null)
            {
              this.SetItemFrame(hardDropPiece);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_COMBINATION:
          UnitData unitData41;
          if ((unitData41 = this.GetUnitData()) != null)
          {
            this.SetTextValue(unitData41.GetCombination());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_JOBCHANGED:
          UnitData unitData42;
          if ((unitData42 = this.GetUnitData()) != null)
          {
            JobData currentJob = unitData42.CurrentJob;
            ((Component) this).get_gameObject().SetActive(currentJob != null && currentJob.IsActivated);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.SHOP_STATE_MAINCOSTFRAME:
          ShopData shopData1 = MonoSingleton<GameManager>.Instance.Player.GetShopData(GlobalVars.ShopType);
          if (shopData1 != null)
          {
            for (int index1 = 0; index1 < shopData1.items.Count; ++index1)
            {
              if (shopData1.items[index1].saleType != ESaleType.Gold && shopData1.items[index1].saleType != ESaleType.Coin)
              {
                ((Component) this).get_gameObject().SetActive(true);
                return;
              }
            }
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.SHOP_MAINCOST_ICON:
          ShopData shopData2 = MonoSingleton<GameManager>.Instance.Player.GetShopData(GlobalVars.ShopType);
          if (shopData2 != null)
          {
            for (int index1 = 0; index1 < shopData2.items.Count; ++index1)
            {
              if (shopData2.items[index1].saleType != ESaleType.Gold && shopData2.items[index1].saleType != ESaleType.Coin)
              {
                this.SetBuyPriceTypeIcon(shopData2.items[index1].saleType);
                return;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_MAINCOST_AMOUNT:
          ShopData shopData3 = MonoSingleton<GameManager>.Instance.Player.GetShopData(GlobalVars.ShopType);
          if (shopData3 != null)
          {
            for (int index1 = 0; index1 < shopData3.items.Count; ++index1)
            {
              if (shopData3.items[index1].saleType != ESaleType.Gold && shopData3.items[index1].saleType != ESaleType.Coin)
              {
                this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.GetShopTypeCostAmount(shopData3.items[index1].saleType));
                return;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_BADGE_GROWUP:
          UnitData unitData43;
          if ((unitData43 = this.GetUnitData()) != null)
          {
            ((Component) this).get_gameObject().SetActive((unitData43.BadgeState & UnitBadgeTypes.EnableEquipment) != (UnitBadgeTypes) 0);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.UNITPARAM_BADGE_UNLOCK:
          UnitParam unitParam11;
          if ((unitParam11 = this.GetUnitParam()) != null)
          {
            ((Component) this).get_gameObject().SetActive(MonoSingleton<GameManager>.Instance.CheckEnableUnitUnlock(unitParam11));
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.ITEM_BADGE_EQUIPUNIT:
          ItemParam itemParam17;
          if ((itemParam17 = this.GetItemParam()) != null && MonoSingleton<GameManager>.GetInstanceDirect().Player.CheckEnableEquipUnit(itemParam17))
          {
            ((Component) this).get_gameObject().SetActive(true);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.BADGE_UNIT:
          if ((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.Unit) != ~GameManager.BadgeTypes.All || (MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.UnitUnlock) != ~GameManager.BadgeTypes.All)
          {
            ((Component) this).get_gameObject().SetActive(true);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.BADGE_UNITENHANCED:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.Unit) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_UNITUNLOCKED:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.UnitUnlock) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_GACHA:
          if ((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.GoldGacha) != ~GameManager.BadgeTypes.All || (MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.RareGacha) != ~GameManager.BadgeTypes.All)
          {
            ((Component) this).get_gameObject().SetActive(true);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.BADGE_GOLDGACHA:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.GoldGacha) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_RAREGACHA:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.RareGacha) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_ARENA:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.Arena) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_MULTIPLAY:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.Multiplay) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_DAILYMISSION:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.DailyMission) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_FRIEND:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.Friend) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_GIFTBOX:
          ((Component) this).get_gameObject().SetActive((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.GiftBox) != ~GameManager.BadgeTypes.All);
          break;
        case GameParameter.ParameterTypes.BADGE_SHORTCUTMENU:
          if ((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.Unit) != ~GameManager.BadgeTypes.All || (MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.UnitUnlock) != ~GameManager.BadgeTypes.All || ((MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.DailyMission) != ~GameManager.BadgeTypes.All || (MonoSingleton<GameManager>.Instance.BadgeFlags & GameManager.BadgeTypes.GiftBox) != ~GameManager.BadgeTypes.All))
          {
            ((Component) this).get_gameObject().SetActive(true);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_VIPPOINT:
          GameManager instance23 = MonoSingleton<GameManager>.Instance;
          int num7 = instance23.Player.VipPoint - (instance23.Player.VipRank <= 0 ? 0 : instance23.MasterParam.GetVipRankNextPoint(instance23.Player.VipRank - 1));
          int vipRankNextPoint = instance23.MasterParam.GetVipRankNextPoint(instance23.Player.VipRank);
          this.SetTextValue(num7);
          this.SetSliderValue(num7, vipRankNextPoint);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_VIPPOINTMAX:
          GameManager instance24 = MonoSingleton<GameManager>.Instance;
          this.SetTextValue(instance24.MasterParam.GetVipRankNextPoint(instance24.Player.VipRank));
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_COINFREE:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.FreeCoin);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_COINPAID:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.PaidCoin);
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_PARTYMEMBER:
          UnitData unitData44;
          if ((unitData44 = this.GetUnitData()) != null)
          {
            List<PartyData> partys = MonoSingleton<GameManager>.Instance.Player.Partys;
            for (int index1 = 0; index1 < partys.Count; ++index1)
            {
              if (partys[index1].IsPartyUnit(unitData44.UniqueID))
              {
                ((Component) this).get_gameObject().SetActive(true);
                return;
              }
            }
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.LOGINBONUS_DAYNUM:
          ItemData dataOfClass33;
          if ((dataOfClass33 = DataSource.FindDataOfClass<ItemData>(((Component) this).get_gameObject(), (ItemData) null)) != null && dataOfClass33 is LoginBonusData)
          {
            this.SetTextValue(((LoginBonusData) dataOfClass33).DayNum);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.None:
          this.BatchUpdate();
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_LVSORT:
          if ((unitData1 = this.GetUnitData()) != null)
          {
            ((Component) this).get_gameObject().SetActive(GameUtility.GetUnitShowSetting(17));
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_PARAMSORT:
          if ((unitData1 = this.GetUnitData()) != null && !GameUtility.GetUnitShowSetting(18) && !GameUtility.GetUnitShowSetting(17))
          {
            ((Component) this).get_gameObject().SetActive(true);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.UNIT_SORTTYPE_VALUE:
          UnitData unitData45;
          if ((unitData45 = this.GetUnitData()) != null)
          {
            if (GameUtility.GetUnitShowSetting(17))
            {
              this.SetTextValue(unitData45.Lv);
              break;
            }
            if (GameUtility.GetUnitShowSetting(18))
            {
              this.ResetToDefault();
              break;
            }
            if (GameUtility.GetUnitShowSetting(19))
            {
              JobData currentJob = unitData45.CurrentJob;
              this.SetTextValue(currentJob == null ? 1 : currentJob.Rank);
              break;
            }
            if (GameUtility.GetUnitShowSetting(20))
            {
              this.SetTextValue((int) unitData45.Status.param.hp);
              break;
            }
            if (GameUtility.GetUnitShowSetting(21))
            {
              this.SetTextValue((int) unitData45.Status.param.atk);
              break;
            }
            if (GameUtility.GetUnitShowSetting(22))
            {
              this.SetTextValue((int) unitData45.Status.param.def);
              break;
            }
            if (GameUtility.GetUnitShowSetting(23))
            {
              this.SetTextValue((int) unitData45.Status.param.mag);
              break;
            }
            if (GameUtility.GetUnitShowSetting(24))
            {
              this.SetTextValue((int) unitData45.Status.param.mnd);
              break;
            }
            if (GameUtility.GetUnitShowSetting(25))
            {
              this.SetTextValue((int) unitData45.Status.param.spd);
              break;
            }
            if (GameUtility.GetUnitShowSetting(26))
            {
              this.SetTextValue(0 + (int) unitData45.Status.param.atk + (int) unitData45.Status.param.def + (int) unitData45.Status.param.mag + (int) unitData45.Status.param.mnd + (int) unitData45.Status.param.spd + (int) unitData45.Status.param.dex + (int) unitData45.Status.param.cri + (int) unitData45.Status.param.luk);
              break;
            }
            if (GameUtility.GetUnitShowSetting(27))
            {
              this.SetTextValue(unitData45.AwakeLv);
              break;
            }
            if (GameUtility.GetUnitShowSetting(28))
            {
              this.SetTextValue(unitData45.GetCombination());
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SKILL_STATE_CONDITION:
          if ((skillParam = this.GetSkillParam()) != null)
          {
            UnitData unitData15;
            if ((unitData15 = this.GetUnitData()) != null)
            {
              ((Component) this).get_gameObject().SetActive(unitData15.GetSkillData(skillParam.iname) == null);
              break;
            }
            ((Component) this).get_gameObject().SetActive(true);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.SKILL_CONDITION:
          AbilityParam abilityParam5;
          if ((skillParam = this.GetSkillParam()) != null && (abilityParam5 = this.GetAbilityParam()) != null && (abilityParam5.skills != null && abilityParam5.skills.Length > 0))
          {
            if (!string.IsNullOrEmpty(skillParam.ReplacedTargetId))
            {
              SkillParam skillParam1 = MonoSingleton<GameManager>.Instance.GetSkillParam(skillParam.ReplacedTargetId);
              skillParam = skillParam1 == null ? skillParam : skillParam1;
            }
            LearningSkill learningSkill = Array.Find<LearningSkill>(abilityParam5.skills, (Predicate<LearningSkill>) (p => p.iname == skillParam.iname));
            if (learningSkill != null)
            {
              this.SetTextValue(string.Format(LocalizedText.Get("sys.SKILL_LEANING_CONDITION1"), (object) learningSkill.locklv));
              break;
            }
          }
          this.SetTextValue(string.Format(LocalizedText.Get("sys.SKILL_LEANING_CONDITION1"), (object) 1));
          break;
        case GameParameter.ParameterTypes.ABILITY_CONDITION:
          UnitData unitData46;
          if ((unitData46 = this.GetUnitData()) != null)
          {
            int rank = 1;
            string abilityID = (string) null;
            AbilityData abilityData3;
            if ((abilityData3 = this.GetAbilityData()) != null)
              abilityID = abilityData3.AbilityID;
            AbilityParam abilityParam6;
            if ((abilityParam6 = this.GetAbilityParam()) != null)
              abilityID = abilityParam6.iname;
            JobParam job2;
            MonoSingleton<GameManager>.Instance.GetLearningAbilitySource(unitData46, abilityID, out job2, out rank);
            if (job2 != null)
            {
              this.SetTextValue(string.Format(LocalizedText.Get("sys.ABILITY_LEANING_COND1"), (object) job2.name, (object) rank));
              break;
            }
          }
          this.SetTextValue((string) null);
          break;
        case GameParameter.ParameterTypes.GACHA_COST:
          GachaParam gachaParam1;
          if ((gachaParam1 = this.GetGachaParam()) != null)
          {
            if (gachaParam1.gold != 0)
            {
              this.SetTextValue(gachaParam1.gold);
              break;
            }
            if (gachaParam1.coin != 0)
            {
              this.SetTextValue(gachaParam1.coin);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.GACHA_NUM:
          GachaParam gachaParam2;
          if ((gachaParam2 = this.GetGachaParam()) != null)
          {
            this.SetTextValue(gachaParam2.num);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.GACHA_GOLD_RESTNUM:
          break;
        case GameParameter.ParameterTypes.GACHA_GOLD_STATE_RESTNUM:
          break;
        case GameParameter.ParameterTypes.GACHA_GOLD_TIMER:
          long gachaGoldCoolDownSec = MonoSingleton<GameManager>.Instance.Player.GetNextFreeGachaGoldCoolDownSec();
          this.SetTextValue((gachaGoldCoolDownSec / 3600L).ToString().PadLeft(2, '0') + ":" + (gachaGoldCoolDownSec % 3600L / 60L).ToString().PadLeft(2, '0') + ":" + (gachaGoldCoolDownSec % 60L).ToString().PadLeft(2, '0'));
          this.SetUpdateInterval(1f);
          break;
        case GameParameter.ParameterTypes.GACHA_GOLD_STATE_TIMER:
          GameManager instance25 = MonoSingleton<GameManager>.Instance;
          this.SetTextValue(((int) instance25.MasterParam.FixParam.FreeGachaGoldMax - instance25.Player.FreeGachaGold.num).ToString());
          break;
        case GameParameter.ParameterTypes.GACHA_GOLD_STATE_INTERACTIVE:
          break;
        case GameParameter.ParameterTypes.GACHA_COIN_TIMER:
          long gachaCoinCoolDownSec = MonoSingleton<GameManager>.Instance.Player.GetNextFreeGachaCoinCoolDownSec();
          this.SetTextValue((gachaCoinCoolDownSec / 3600L).ToString().PadLeft(2, '0') + ":" + (gachaCoinCoolDownSec % 3600L / 60L).ToString().PadLeft(2, '0') + ":" + (gachaCoinCoolDownSec % 60L).ToString().PadLeft(2, '0'));
          this.SetUpdateInterval(1f);
          break;
        case GameParameter.ParameterTypes.GACHA_COIN_STATE_TIMER:
          this.SetTextValue((1 - MonoSingleton<GameManager>.Instance.Player.FreeGachaCoin.num).ToString());
          break;
        case GameParameter.ParameterTypes.GACHA_COIN_STATE_INTERACTIVE:
          break;
        case GameParameter.ParameterTypes.UNIT_IMAGE2:
          UnitData unitData47;
          if ((unitData47 = this.GetUnitData()) != null && !string.IsNullOrEmpty(unitData47.UnitParam.image))
          {
            IconLoader iconLoader = GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject());
            string str3 = AssetPath.UnitSkinImage2(unitData47.UnitParam, unitData47.GetSelectedSkin(-1), unitData47.CurrentJob.JobID);
            if (!string.IsNullOrEmpty(str3))
            {
              iconLoader.ResourcePath = str3;
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ITEM_FLAVOR:
          ItemParam itemParam18;
          if ((itemParam18 = this.GetItemParam()) != null)
          {
            this.SetTextValue(itemParam18.Flavor);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_AWAKE:
        case GameParameter.ParameterTypes.UNIT_AWAKE2:
          UnitData unitData48;
          if ((unitData48 = this.GetUnitData()) != null)
          {
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null))
            {
              int awakeLv = unitData48.AwakeLv;
              int num3 = unitData48.GetAwakeLevelCap();
              if (this.ParameterType == GameParameter.ParameterTypes.UNIT_AWAKE2)
                num3 = (int) MonoSingleton<GameManager>.Instance.MasterParam.GetRarityParam(unitData48.GetRarityCap()).UnitAwakeLvCap;
              int num5 = 5;
              if (num3 / num5 > this.Index)
              {
                int index1 = num5;
                int num8 = this.Index * num5;
                if ((this.Index + 1) * num5 > awakeLv)
                  index1 = awakeLv - num8 >= 0 ? awakeLv % num5 : 0;
                this.SetImageIndex(index1);
                ((Component) this).get_gameObject().SetActive(true);
                break;
              }
              ((Component) this).get_gameObject().SetActive(false);
              break;
            }
            this.SetTextValue(unitData48.AwakeLv);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SUPPORTER_ISFRIEND:
          if ((supportData = this.GetSupportData()) != null)
          {
            ((Component) this).get_gameObject().SetActive(supportData.IsFriend());
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.SUPPORTER_COST:
          if ((supportData = this.GetSupportData()) != null)
          {
            this.SetTextValue(supportData.GetCost());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.Unit_ThumbnailedJobIcon:
          UnitData unitData49;
          if ((unitData49 = this.GetUnitData()) == null)
            break;
          SpriteSheet spriteSheet1 = AssetManager.Load<SpriteSheet>(AssetPath.JobIconThumbnail());
          Image component4 = (Image) ((Component) this).GetComponent<Image>();
          if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet1, (UnityEngine.Object) null) || !UnityEngine.Object.op_Inequality((UnityEngine.Object) component4, (UnityEngine.Object) null))
            break;
          JobData currentJob1 = unitData49.CurrentJob;
          if (currentJob1 != null)
          {
            Sprite sprite = spriteSheet1.GetSprite(currentJob1.JobResourceID);
            component4.set_sprite(sprite);
            break;
          }
          component4.set_sprite((Sprite) null);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_JOBRANKFRAME:
          UnitData unit7 = this.GetMultiPlayerUnitData(this.Index)?.unit;
          if (unit7 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetImageIndex(unit7.CurrentJob.Rank);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_JOBRANK:
          UnitData unit8 = this.GetMultiPlayerUnitData(this.Index)?.unit;
          if (unit8 == null)
          {
            this.ResetToDefault();
            break;
          }
          int rank1 = unit8.CurrentJob.Rank;
          int jobRankCap1 = unit8.GetJobRankCap();
          this.SetTextValue(rank1);
          this.SetSliderValue(rank1, jobRankCap1);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_JOBICON:
          UnitData unit9 = this.GetMultiPlayerUnitData(this.Index)?.unit;
          if (unit9 == null || unit9.CurrentJob == null)
          {
            this.ResetToDefault();
            break;
          }
          JobParam job3 = unit9.CurrentJob.Param;
          string str4 = job3 != null ? AssetPath.JobIconSmall(job3) : (string) null;
          if (string.IsNullOrEmpty(str4))
            break;
          GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str4;
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_RARITY:
          UnitData unit10 = this.GetMultiPlayerUnitData(this.Index)?.unit;
          if (unit10 == null)
          {
            this.ResetToDefault();
            break;
          }
          StarGauge component5 = (StarGauge) ((Component) this).GetComponent<StarGauge>();
          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component5, (UnityEngine.Object) null))
          {
            component5.Max = unit10.GetRarityCap() + 1;
            component5.Value = unit10.Rarity + 1;
            break;
          }
          this.SetAnimatorInt("rare", unit10.Rarity);
          this.SetImageIndex(unit10.Rarity);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_ELEMENT:
          UnitData unit11 = this.GetMultiPlayerUnitData(this.Index)?.unit;
          if (unit11 == null)
          {
            this.ResetToDefault();
            break;
          }
          int element1 = (int) unit11.Element;
          this.SetAnimatorInt("element", element1);
          this.SetImageIndex(element1);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_UNIT_LEVEL:
          UnitData unit12 = this.GetMultiPlayerUnitData(this.Index)?.unit;
          if (unit12 == null)
          {
            this.ResetToDefault();
            break;
          }
          int num9 = unit12.CalcLevel();
          this.SetTextValue(num9);
          this.SetSliderValue(num9, 99);
          break;
        case GameParameter.ParameterTypes.TROPHY_REWARDSTAMINA:
          TrophyParam trophyParam4 = this.GetTrophyParam();
          if (trophyParam4 == null)
            break;
          this.SetTextValue(trophyParam4.Stamina);
          ((Component) this).get_gameObject().SetActive(trophyParam4.Stamina > 0);
          break;
        case GameParameter.ParameterTypes.JOB_JOBICON:
        case GameParameter.ParameterTypes.JOB_JOBICON2:
          JobParam jobParam1;
          if ((jobParam1 = this.GetJobParam()) != null)
          {
            string str3 = this.ParameterType != GameParameter.ParameterTypes.JOB_JOBICON2 ? AssetPath.JobIconSmall(jobParam1) : AssetPath.JobIconMedium(jobParam1);
            if (!string.IsNullOrEmpty(str3))
            {
              GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = str3;
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.JOB_NAME:
          JobParam jobParam2;
          if ((jobParam2 = this.GetJobParam()) != null)
          {
            this.SetTextValue(jobParam2.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUESTRESULT_MULTICOIN:
          BattleCore.Record dataOfClass34 = DataSource.FindDataOfClass<BattleCore.Record>(((Component) this).get_gameObject(), (BattleCore.Record) null);
          if (dataOfClass34 != null)
          {
            this.SetTextValue((int) dataOfClass34.multicoin);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTI_REST_REWARD_IS_ZERO:
          GameUtility.EScene currentScene2 = GameUtility.GetCurrentScene();
          if (currentScene2 != GameUtility.EScene.HOME_MULTI && currentScene2 != GameUtility.EScene.BATTLE_MULTI)
            break;
          PlayerData player3 = MonoSingleton<GameManager>.Instance.Player;
          int num10 = player3.ChallengeMultiMax - player3.ChallengeMultiNum;
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(num10 <= 0);
            break;
          }
          if (this.Index == 1)
          {
            ((Component) this).get_gameObject().SetActive(num10 > 0);
            break;
          }
          if (this.Index == 2)
          {
            ((Component) this).get_gameObject().SetActive(num10 >= 0);
            break;
          }
          if (this.Index == 3)
          {
            ((Component) this).get_gameObject().SetActive(num10 < 0);
            break;
          }
          if (this.Index == 4)
          {
            ((Component) this).get_gameObject().SetActive(num10 == 0);
            break;
          }
          ((Component) this).get_gameObject().SetActive(num10 != 0);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_UNIT_SLOT_ENABLE:
          bool flag5 = false;
          if (GameUtility.GetCurrentScene() == GameUtility.EScene.HOME_MULTI)
          {
            JSON_MyPhotonRoomParam roomParam3 = this.GetRoomParam();
            JSON_MyPhotonPlayerParam roomPlayerParam8 = this.GetRoomPlayerParam();
            int playerIndex = roomPlayerParam8 != null ? roomPlayerParam8.playerIndex : 0;
            flag5 = this.Index < (roomParam3 != null ? roomParam3.GetUnitSlotNum(playerIndex) : 0);
          }
          ((Component) this).get_gameObject().SetActive(flag5);
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_UNIT_SLOT_VALID:
          bool flag6 = false;
          if (GameUtility.GetCurrentScene() == GameUtility.EScene.HOME_MULTI)
            flag6 = this.GetMultiPlayerUnitData(this.Index)?.unit != null;
          ((Component) this).get_gameObject().SetActive(flag6);
          break;
        case GameParameter.ParameterTypes.REWARD_STAMINA:
          RewardData dataOfClass35 = DataSource.FindDataOfClass<RewardData>(((Component) this).get_gameObject(), (RewardData) null);
          if (dataOfClass35 != null)
          {
            this.SetTextValue(dataOfClass35.Stamina);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_CHALLENGE_NUM:
          QuestParam questParam17;
          if ((questParam17 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(questParam17.GetChallangeCount());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_CHALLENGE_MAX:
          QuestParam questParam18;
          if ((questParam18 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(questParam18.GetChallangeLimit());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_RESET_NUM:
          QuestParam questParam19;
          if ((questParam19 = this.GetQuestParam()) != null)
          {
            this.SetTextValue((int) questParam19.dailyReset);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_RESET_MAX:
          this.SetTextValue((int) MonoSingleton<GameManager>.Instance.MasterParam.FixParam.EliteResetMax);
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_POISON:
          Unit unit13;
          if ((unit13 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit13.IsUnitCondition(EUnitCondition.Poison));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_PARALYSED:
          Unit unit14;
          if ((unit14 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit14.IsUnitCondition(EUnitCondition.Paralysed));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_STUN:
          Unit unit15;
          if ((unit15 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit15.IsUnitCondition(EUnitCondition.Stun));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_SLEEP:
          Unit unit16;
          if ((unit16 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit16.IsUnitCondition(EUnitCondition.Sleep));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_CHARM:
          Unit unit17;
          if ((unit17 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit17.IsUnitCondition(EUnitCondition.Charm));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_STONE:
          Unit unit18;
          if ((unit18 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit18.IsUnitCondition(EUnitCondition.Stone));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_BLINDNESS:
          Unit unit19;
          if ((unit19 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit19.IsUnitCondition(EUnitCondition.Blindness));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_DISABLESKILL:
          Unit unit20;
          if ((unit20 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit20.IsUnitCondition(EUnitCondition.DisableSkill));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_DISABLEMOVE:
          Unit unit21;
          if ((unit21 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit21.IsUnitCondition(EUnitCondition.DisableMove));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_DISABLEATTACK:
          Unit unit22;
          if ((unit22 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit22.IsUnitCondition(EUnitCondition.DisableAttack));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_ZOMBIE:
          Unit unit23;
          if ((unit23 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit23.IsUnitCondition(EUnitCondition.Zombie));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_DEATHSENTENCE:
          Unit unit24;
          if ((unit24 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit24.IsUnitCondition(EUnitCondition.DeathSentence));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_BERSERK:
          Unit unit25;
          if ((unit25 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit25.IsUnitCondition(EUnitCondition.Berserk));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_DISABLEKNOCKBACK:
          Unit unit26;
          if ((unit26 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit26.IsUnitCondition(EUnitCondition.DisableKnockback));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_DISABLEBUFF:
          Unit unit27;
          if ((unit27 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit27.IsUnitCondition(EUnitCondition.DisableBuff));
          break;
        case GameParameter.ParameterTypes.UNIT_STATE_CONDITION_DISABLEDEBUFF:
          Unit unit28;
          if ((unit28 = this.GetUnit()) == null)
            break;
          ((Component) this).get_gameObject().SetActive(unit28.IsUnitCondition(EUnitCondition.DisableDebuff));
          break;
        case GameParameter.ParameterTypes.TURN_UNIT_SIDE_FRAME:
          Unit unit29;
          if ((unit29 = this.GetUnit()) != null)
          {
            if (unit29.Side == EUnitSide.Player)
            {
              this.SetImageIndex(0);
              break;
            }
            this.SetImageIndex(1);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.JOBSET_UNLOCKCONDITION:
          JobSetParam dataOfClass36 = DataSource.FindDataOfClass<JobSetParam>(((Component) this).get_gameObject(), (JobSetParam) null);
          if (dataOfClass36 != null)
          {
            StringBuilder stringBuilder = GameUtility.GetStringBuilder();
            if (dataOfClass36.lock_rarity > 0)
            {
              stringBuilder.Append(string.Format(LocalizedText.Get("sys.UNLOCK_RARITY"), (object) (dataOfClass36.lock_rarity + 1)));
              stringBuilder.Append('\n');
            }
            if (dataOfClass36.lock_awakelv > 0)
            {
              stringBuilder.Append(string.Format(LocalizedText.Get("sys.UNLOCK_AWAKELV"), (object) dataOfClass36.lock_awakelv));
              stringBuilder.Append('\n');
            }
            if (dataOfClass36.lock_jobs != null)
            {
              for (int index1 = 0; index1 < dataOfClass36.lock_jobs.Length; ++index1)
              {
                if (!string.IsNullOrEmpty(dataOfClass36.lock_jobs[index1].iname) && dataOfClass36.lock_jobs[index1].lv > 0)
                {
                  JobParam jobParam3 = MonoSingleton<GameManager>.Instance.GetJobParam(dataOfClass36.lock_jobs[index1].iname);
                  stringBuilder.Append(string.Format(LocalizedText.Get("sys.UNLOCK_CONDITION"), (object) jobParam3.name, (object) dataOfClass36.lock_jobs[index1].lv));
                  stringBuilder.Append('\n');
                }
              }
            }
            if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\n')
              --stringBuilder.Length;
            this.SetTextValue(stringBuilder.ToString());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTI_REST_MY_UNIT_IS_ZERO:
          SceneBattle instance26 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance26, (UnityEngine.Object) null))
          {
            this.ResetToDefault();
            break;
          }
          int nextMyTurn = instance26.GetNextMyTurn();
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(nextMyTurn < 0);
            break;
          }
          ((Component) this).get_gameObject().SetActive(nextMyTurn >= 0);
          break;
        case GameParameter.ParameterTypes.MULTI_PLAYER_IS_ME:
          JSON_MyPhotonPlayerParam roomPlayerParam9 = this.GetRoomPlayerParam();
          if (roomPlayerParam9 == null || roomPlayerParam9.playerID <= 0)
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          bool flag7 = roomPlayerParam9.playerIndex == PunMonoSingleton<MyPhoton>.Instance.MyPlayerIndex;
          bool flag8 = true & roomPlayerParam9.state != 0 & roomPlayerParam9.state != 4 & roomPlayerParam9.state != 5;
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(flag7);
            break;
          }
          if (this.Index == 1)
          {
            ((Component) this).get_gameObject().SetActive(!flag7);
            break;
          }
          if (this.Index == 2)
          {
            ((Component) this).get_gameObject().SetActive(true);
            this.SetImageIndex(!flag7 ? 1 : 0);
            break;
          }
          if (this.Index == 3)
          {
            ((Component) this).get_gameObject().SetActive(flag7 && !flag8);
            break;
          }
          if (this.Index == 4)
          {
            ((Component) this).get_gameObject().SetActive(!flag7 || flag8);
            break;
          }
          if (this.Index == 5)
          {
            SRPG_Button component6 = (SRPG_Button) ((Component) this).get_gameObject().GetComponent<SRPG_Button>();
            if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component6, (UnityEngine.Object) null))
              break;
            ((Selectable) component6).set_interactable(flag7);
            break;
          }
          if (this.Index != 6)
            break;
          MyPhoton instance27 = PunMonoSingleton<MyPhoton>.Instance;
          List<MyPhoton.MyPlayer> roomPlayerList3 = instance27.GetRoomPlayerList();
          if (roomPlayerList3 != null)
          {
            MyPhoton.MyPlayer player4 = instance27.FindPlayer(roomPlayerList3, roomPlayerParam9.playerID, roomPlayerParam9.playerIndex);
            if (player4 == null)
            {
              ((Component) this).get_gameObject().SetActive(false);
              break;
            }
            ((Component) this).get_gameObject().SetActive(player4.start);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.QUESTLIST_SECTIONEXPR:
          ChapterParam dataOfClass37 = DataSource.FindDataOfClass<ChapterParam>(((Component) this).get_gameObject(), (ChapterParam) null);
          if (dataOfClass37 != null)
          {
            this.SetTextValue(dataOfClass37.expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTI_CURRENT_ROOM_IS_LOCKED:
          JSON_MyPhotonRoomParam roomParam5 = this.GetRoomParam();
          if (roomParam5 == null)
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          bool flag9 = MultiPlayAPIRoom.IsLocked(roomParam5.passCode);
          bool flag10 = PunMonoSingleton<MyPhoton>.Instance.IsOldestPlayer();
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(flag9);
            break;
          }
          if (this.Index == 1)
          {
            ((Component) this).get_gameObject().SetActive(!flag9);
            break;
          }
          if (this.Index == 2)
          {
            ((Component) this).get_gameObject().SetActive(flag10 && flag9);
            break;
          }
          ((Component) this).get_gameObject().SetActive(flag10 && !flag9);
          break;
        case GameParameter.ParameterTypes.MAIL_GIFT_RECEIVE:
          MailData mailData5;
          if ((mailData5 = this.GetMailData()) == null)
          {
            this.ResetToDefault();
            break;
          }
          DateTime serverTime1 = TimeManager.ServerTime;
          DateTime localTime = GameUtility.UnixtimeToLocalTime(mailData5.post_at);
          TimeSpan timeSpan2 = serverTime1 - localTime;
          string empty1 = string.Empty;
          string str5;
          if (timeSpan2.Days >= 1)
          {
            string format3 = "yyyy/MM/dd";
            str5 = localTime.ToString(format3);
          }
          else
            str5 = timeSpan2.Hours < 1 ? (timeSpan2.Minutes < 1 ? timeSpan2.Seconds.ToString() + "秒前" : timeSpan2.Minutes.ToString() + "分前") : timeSpan2.Hours.ToString() + "時間前";
          this.SetTextValue(str5);
          break;
        case GameParameter.ParameterTypes.QUEST_TIMELIMIT:
          if ((questParam1 = this.GetQuestParam()) != null)
            break;
          break;
        case GameParameter.ParameterTypes.UNIT_CHARGETIME:
          Unit unit30;
          if ((unit30 = this.GetUnit()) != null)
          {
            OInt oint = (OInt) Mathf.Min((int) unit30.ChargeTime, (int) unit30.ChargeTimeMax);
            this.SetTextValue(Mathf.Floor((float) (int) oint / 10f).ToString());
            this.SetSliderValue((int) oint, (int) unit30.ChargeTimeMax);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.UNIT_CHARGETIMEMAX:
          Unit unit31;
          if ((unit31 = this.GetUnit()) != null)
          {
            this.SetTextValue(Mathf.Floor((float) (int) unit31.ChargeTimeMax / 10f).ToString());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.GIMMICK_DESCRIPTION:
          Unit unit32;
          if ((unit32 = this.GetUnit()) != null)
          {
            this.SetTextValue(LocalizedText.Get("quest." + unit32.UnitParam.iname));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.PRODUCT_DESC:
          PaymentManager.Product dataOfClass38 = DataSource.FindDataOfClass<PaymentManager.Product>(((Component) this).get_gameObject(), (PaymentManager.Product) null);
          if (dataOfClass38 == null || string.IsNullOrEmpty(dataOfClass38.desc))
          {
            this.ResetToDefault();
            break;
          }
          string str6;
          if (this.Index == 0)
          {
            str6 = dataOfClass38.desc;
          }
          else
          {
            string[] strArray = dataOfClass38.desc.Split('|');
            int num3 = this.Index >= 0 ? this.Index : -this.Index;
            str6 = strArray == null || num3 - 1 >= strArray.Length ? (string) null : strArray[num3 - 1];
          }
          if (this.Index >= 0)
          {
            this.SetTextValue(str6 ?? string.Empty);
            break;
          }
          ((Component) this).get_gameObject().SetActive(str6 != null);
          break;
        case GameParameter.ParameterTypes.PRODUCT_NUMX:
          PaymentManager.Product dataOfClass39 = DataSource.FindDataOfClass<PaymentManager.Product>(((Component) this).get_gameObject(), (PaymentManager.Product) null);
          if (dataOfClass39 == null)
          {
            this.ResetToDefault();
            break;
          }
          string str7 = string.Empty;
          FixParam fixParam3 = MonoSingleton<GameManager>.Instance.MasterParam.FixParam;
          if (dataOfClass39.productID.Contains((string) fixParam3.VipCardProduct))
          {
            if (MonoSingleton<GameManager>.Instance.Player.CheckEnableVipCard())
            {
              DateTime vipExpiredAt = MonoSingleton<GameManager>.Instance.Player.VipExpiredAt;
              TimeSpan timeSpan3 = vipExpiredAt - TimeManager.FromUnixTime(Network.GetServerTime());
              str7 = 0 >= timeSpan3.Days ? string.Format(LocalizedText.Get("sys.VIP_EXPIRE_HHMM"), (object) vipExpiredAt.Hour, (object) vipExpiredAt.Minute) : string.Format(LocalizedText.Get("sys.VIP_REMAIN_D"), (object) timeSpan3.Days);
            }
          }
          else
          {
            int num3 = dataOfClass39.numPaid + dataOfClass39.numFree;
            if (0 < num3)
              str7 = string.Format(LocalizedText.Get("sys.CROSS_NUM"), (object) num3);
          }
          if (this.Index == -1)
          {
            ((Component) this).get_gameObject().SetActive(!string.IsNullOrEmpty(str7));
            break;
          }
          this.SetTextValue(str7);
          break;
        case GameParameter.ParameterTypes.UNIT_DEX:
          UnitData unitData50;
          if ((unitData50 = this.GetUnitData()) != null)
          {
            this.SetTextValue((int) unitData50.Status.param.dex);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARTIFACT_NAME:
          ArtifactParam artifactParam1 = this.GetArtifactParam();
          if (artifactParam1 != null)
          {
            this.SetTextValue(artifactParam1.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARTIFACT_DESC:
          ArtifactParam artifactParam2 = this.GetArtifactParam();
          if (artifactParam2 != null)
          {
            this.SetTextValue(artifactParam2.Expr);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARTIFACT_SPEC:
          ArtifactParam artifactParam3 = this.GetArtifactParam();
          if (artifactParam3 != null)
          {
            this.SetTextValue(artifactParam3.spec);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARTIFACT_RARITY:
          ArtifactData dataOfClass40 = DataSource.FindDataOfClass<ArtifactData>(((Component) this).get_gameObject(), (ArtifactData) null);
          if (dataOfClass40 != null)
          {
            this.SetTextValue((int) dataOfClass40.Rarity + 1);
            this.SetImageIndex((int) dataOfClass40.Rarity);
            this.SetSliderValue((int) dataOfClass40.Rarity, (int) dataOfClass40.RarityCap);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARTIFACT_RARITYCAP:
          ArtifactData dataOfClass41 = DataSource.FindDataOfClass<ArtifactData>(((Component) this).get_gameObject(), (ArtifactData) null);
          if (dataOfClass41 != null)
          {
            this.SetTextValue((int) dataOfClass41.RarityCap + 1);
            this.SetImageIndex((int) dataOfClass41.RarityCap);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARTIFACT_NUM:
          ArtifactParam artifactParam4 = this.GetArtifactParam();
          if (artifactParam4 != null)
          {
            int num3 = 0;
            List<ArtifactData> artifacts = MonoSingleton<GameManager>.Instance.Player.Artifacts;
            for (int index1 = 0; index1 < artifacts.Count; ++index1)
            {
              if (artifacts[index1].ArtifactParam == artifactParam4)
                ++num3;
            }
            this.SetTextValue(num3);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.ARTIFACT_SELLPRICE:
          ArtifactData dataOfClass42 = DataSource.FindDataOfClass<ArtifactData>(((Component) this).get_gameObject(), (ArtifactData) null);
          if (dataOfClass42 != null)
          {
            this.SetTextValue(dataOfClass42.GetSellPrice());
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.APPLICATION_VERSION:
          this.SetTextValue(MyApplicationPlugin.get_version());
          break;
        case GameParameter.ParameterTypes.SUPPORTER_UNITCAPPEDLEVELMAX:
          if ((supportData = this.GetSupportData()) != null)
          {
            instance1 = MonoSingleton<GameManager>.Instance;
            this.SetTextValue(Mathf.Min(supportData.Unit.GetLevelCap(false), supportData.PlayerLevel));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_PIECEPOINT:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.PiecePoint);
          break;
        case GameParameter.ParameterTypes.SHOP_KAKERA_SELLPRICETOTAL:
          List<SellItem> sellItemList3 = this.GetSellItemList();
          if (sellItemList3 == null)
          {
            this.ResetToDefault();
            break;
          }
          int num11 = 0;
          for (int index1 = 0; index1 < sellItemList3.Count; ++index1)
            num11 += (int) sellItemList3[index1].item.RarityParam.PieceToPoint * sellItemList3[index1].num;
          this.SetTextValue(num11);
          break;
        case GameParameter.ParameterTypes.ITEM_KAKERA_PRICE:
          ItemParam itemParam19;
          if ((itemParam19 = this.GetItemParam()) != null)
          {
            this.SetTextValue((int) MonoSingleton<GameManager>.GetInstanceDirect().MasterParam.GetRarityParam(itemParam19.rare).PieceToPoint);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_ITEM_KAKERA_SELLPRICE:
          SellItem sellItem8 = this.GetSellItem();
          if (sellItem8 == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue((int) sellItem8.item.RarityParam.PieceToPoint * sellItem8.num);
          break;
        case GameParameter.ParameterTypes.REWARD_MULTICOIN:
          RewardData dataOfClass43 = DataSource.FindDataOfClass<RewardData>(((Component) this).get_gameObject(), (RewardData) null);
          if (dataOfClass43 != null)
          {
            this.SetTextValue(dataOfClass43.MultiCoin);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.REWARD_KAKERACOIN:
          RewardData dataOfClass44 = DataSource.FindDataOfClass<RewardData>(((Component) this).get_gameObject(), (RewardData) null);
          if (dataOfClass44 != null)
          {
            this.SetTextValue(dataOfClass44.KakeraCoin);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_UNIT_ENTRYCONDITION:
          QuestParam questParam20;
          if ((questParam20 = this.GetQuestParam()) != null)
          {
            List<string> stringList = questParam20.type != QuestTypes.Character ? questParam20.GetEntryQuestConditions(true, true, true) : questParam20.GetEntryQuestConditionsCh(true, false, true);
            if (stringList != null && stringList.Count > 0)
            {
              string str3 = string.Empty;
              for (int index1 = 0; index1 < stringList.Count; ++index1)
              {
                if (index1 > 0)
                {
                  switch (this.Index)
                  {
                    case 0:
                    case 2:
                      str3 += "\n";
                      break;
                    default:
                      str3 += "、";
                      break;
                  }
                }
                str3 += stringList[index1];
              }
              if (!string.IsNullOrEmpty(str3))
              {
                if (this.Index != 0)
                {
                  switch (questParam20.GetPartyCondType())
                  {
                    case PartyCondType.Limited:
                      str3 = LocalizedText.Get("sys.PARTYEDITOR_COND_LIMIT") + str3;
                      break;
                    case PartyCondType.Forced:
                      str3 = LocalizedText.Get("sys.PARTYEDITOR_COND_FIXED") + str3;
                      break;
                  }
                }
                if (this.Index == 4)
                  str3 = str3.Replace("\n", string.Empty);
                this.SetTextValue(str3);
                break;
              }
            }
            else
            {
              this.SetTextValue(LocalizedText.Get("sys.PARTYEDITOR_COND_NO_LIMIT"));
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.OBSOLETE_QUEST_IS_UNIT_ENTRYCONDITION:
          QuestParam questParam21;
          if ((questParam21 = this.GetQuestParam()) != null)
          {
            List<string> entryQuestConditions = questParam21.GetEntryQuestConditions(true, true, true);
            bool flag11 = entryQuestConditions != null && entryQuestConditions.Count > 0;
            if (this.Index == 0)
            {
              ((Component) this).get_gameObject().SetActive(flag11);
              break;
            }
            ((Component) this).get_gameObject().SetActive(!flag11);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.QUEST_IS_UNIT_ENABLEENTRYCONDITION:
          UnitData unitData51;
          QuestParam questParam22;
          if ((unitData51 = this.GetUnitData()) != null && (questParam22 = this.GetQuestParam()) != null)
          {
            string empty2 = string.Empty;
            bool flag11 = questParam22.IsEntryQuestCondition(unitData51, ref empty2);
            if (this.Index == 0)
            {
              ((Component) this).get_gameObject().SetActive(flag11);
              break;
            }
            ((Component) this).get_gameObject().SetActive(!flag11);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.QUEST_CHARACTER_MAINUNITCONDITION:
          UnitData unitData52;
          QuestParam questParam23;
          if ((unitData52 = this.GetUnitData()) != null && (questParam23 = this.GetQuestParam()) != null)
          {
            this.SetTextValue(GameUtility.ComposeCharacterQuestMainUnitConditionText(unitData52, questParam23));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_CHARACTER_EPISODENUM:
          UnitData unitData53;
          if ((unitData53 = this.GetUnitData()) != null)
          {
            UnitData.CharacterQuestParam charaEpisodeData = unitData53.GetCurrentCharaEpisodeData();
            if (charaEpisodeData != null)
            {
              this.SetTextValue(LocalizedText.Get("sys.CHARQUEST_EP_NUM", new object[1]
              {
                (object) charaEpisodeData.EpisodeNum
              }));
              break;
            }
            this.ResetToDefault();
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_CHARACTER_EPISODENAME:
          UnitData unitData54;
          if ((unitData54 = this.GetUnitData()) != null)
          {
            UnitData.CharacterQuestParam charaEpisodeData = unitData54.GetCurrentCharaEpisodeData();
            if (charaEpisodeData != null)
            {
              this.SetTextValue(charaEpisodeData.EpisodeTitle);
              break;
            }
            this.ResetToDefault();
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_LIMITED_ITEM_BUYAMOUNT:
          LimitedShopItem limitedShopItem2 = this.GetLimitedShopItem();
          if (limitedShopItem2 == null)
            break;
          this.SetTextValue(limitedShopItem2.remaining_num);
          break;
        case GameParameter.ParameterTypes.UNIT_IS_JOBMASTER:
          JobData jobData = DataSource.FindDataOfClass<JobData>(((Component) this).get_gameObject(), (JobData) null);
          if (jobData != null)
          {
            ((Component) this).get_gameObject().SetActive(jobData.CheckJobMaster());
            break;
          }
          UnitData unitData55;
          if ((unitData55 = this.GetUnitData()) != null)
          {
            if (this.Index == -1)
              jobData = unitData55.CurrentJob;
            else if (unitData55.IsJobAvailable(this.Index))
              jobData = unitData55.GetJobData(this.Index);
            if (jobData != null && jobData.CheckJobMaster())
            {
              ((Component) this).get_gameObject().SetActive(true);
              break;
            }
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.UNIT_NEXTAWAKE:
          UnitData unitData56;
          if ((unitData56 = this.GetUnitData()) != null)
          {
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null))
            {
              int num3 = unitData56.AwakeLv + 1;
              int awakeLevelCap = unitData56.GetAwakeLevelCap();
              int num5 = 5;
              if (awakeLevelCap / num5 > this.Index)
              {
                int index1 = num5;
                int num8 = this.Index * num5;
                if ((this.Index + 1) * num5 > num3)
                  index1 = num3 - num8 >= 0 ? num3 % num5 : 0;
                this.SetImageIndex(index1);
                ((Component) this).get_gameObject().SetActive(true);
                break;
              }
              ((Component) this).get_gameObject().SetActive(false);
              break;
            }
            this.SetTextValue(unitData56.AwakeLv);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTIPLAY_ADD_INPUTTIME:
          SceneBattle instance28 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance28, (UnityEngine.Object) null))
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue("+" + ((int) instance28.MultiPlayAddInputTime).ToString());
          break;
        case GameParameter.ParameterTypes.UNIT_IS_AWAKEMAX:
          UnitData unitData57;
          if ((unitData57 = this.GetUnitData()) != null)
          {
            int awakeLv = unitData57.AwakeLv;
            int awakeLevelCap = unitData57.GetAwakeLevelCap();
            if (this.Index == 0)
            {
              ((Component) this).get_gameObject().SetActive(awakeLevelCap > awakeLv);
              break;
            }
            ((Component) this).get_gameObject().SetActive(awakeLevelCap <= awakeLv);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.CONFIG_IS_AUTOPLAY:
          bool flag12 = GameUtility.Config_UseAutoPlay.Value;
          ((Component) this).get_gameObject().SetActive(this.Index != 0 ? !flag12 : flag12);
          break;
        case GameParameter.ParameterTypes.FRIEND_ISFAVORITE:
          bool flag13 = false;
          FriendData friendData5 = this.GetFriendData();
          if (friendData5 != null)
            flag13 = friendData5.IsFavorite;
          if (this.Index == 1)
            flag13 = !flag13;
          ((Component) this).get_gameObject().SetActive(flag13);
          break;
        case GameParameter.ParameterTypes.QUEST_CHARACTER_ENTRYCONDITION:
          QuestParam questParam24;
          if ((questParam24 = this.GetQuestParam()) != null)
          {
            List<string> questConditionsCh = questParam24.GetEntryQuestConditionsCh(true, true, false);
            string str3 = string.Empty;
            if (questConditionsCh != null && questConditionsCh.Count > 0)
            {
              for (int index1 = 0; index1 < questConditionsCh.Count; ++index1)
              {
                if (index1 > 0)
                  str3 = this.Index != 0 ? str3 + "、" : str3 + "\n";
                str3 += questConditionsCh[index1];
              }
            }
            UnitData unitData15;
            if ((unitData15 = this.GetUnitData()) != null)
            {
              List<string> unlockConditions = unitData15.GetQuestUnlockConditions(questParam24);
              if (unlockConditions != null && unlockConditions.Count > 0)
              {
                for (int index1 = 0; index1 < unlockConditions.Count; ++index1)
                {
                  if (!string.IsNullOrEmpty(str3))
                    str3 = this.Index != 0 ? str3 + "、" : str3 + "\n";
                  str3 += unlockConditions[index1];
                }
              }
            }
            if (!string.IsNullOrEmpty(str3))
            {
              this.SetTextValue(str3);
              break;
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.QUEST_CHARACTER_IS_ENTRYCONDITION:
          QuestParam questParam25;
          if ((questParam25 = this.GetQuestParam()) != null)
          {
            List<string> questConditionsCh = questParam25.GetEntryQuestConditionsCh(true, true, true);
            bool flag11 = true;
            UnitData unitData15;
            if ((unitData15 = this.GetUnitData()) != null)
            {
              List<string> unlockConditions = unitData15.GetQuestUnlockConditions(questParam25);
              if (unlockConditions != null && unlockConditions.Count > 0)
              {
                for (int index1 = 0; index1 < unlockConditions.Count; ++index1)
                {
                  if (!string.IsNullOrEmpty(unlockConditions[index1]))
                  {
                    flag11 = false;
                    break;
                  }
                }
              }
            }
            bool flag14 = questConditionsCh != null && questConditionsCh.Count > 0 || !flag11;
            if (this.Index == 0)
            {
              ((Component) this).get_gameObject().SetActive(flag14);
              break;
            }
            ((Component) this).get_gameObject().SetActive(!flag14);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.QUEST_CHARACTER_CONDITIONATTENTION:
          UnitData unitData58;
          if ((unitData58 = this.GetUnitData()) != null)
          {
            this.SetTextValue(LocalizedText.Get("sys.PARTYEDITOR_COND_UNLOCK_TITLE", new object[1]
            {
              (object) unitData58.UnitParam.name
            }));
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.MULTIPLAY_RESUME_PLAYER_INDEX:
          SceneBattle instance29 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance29, (UnityEngine.Object) null) || instance29.CurrentResumePlayer == null)
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(instance29.CurrentResumePlayer.playerIndex);
          break;
        case GameParameter.ParameterTypes.MULTIPLAY_RESUME_PLAYER_IS_HOST:
          MyPhoton instance30 = PunMonoSingleton<MyPhoton>.Instance;
          SceneBattle instance31 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance31, (UnityEngine.Object) null) || instance31.CurrentResumePlayer == null)
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          ((Component) this).get_gameObject().SetActive(instance30.IsHost(instance31.CurrentResumePlayer.playerID));
          break;
        case GameParameter.ParameterTypes.MULTIPLAY_RESUME_BUT_NOT_PLAYER:
          SceneBattle instance32 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance32, (UnityEngine.Object) null))
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          ((Component) this).get_gameObject().SetActive(instance32.ResumeOnly);
          break;
        case GameParameter.ParameterTypes.EVENTCOIN_SHOPTYPEICON:
          EventCoinData dataOfClass45 = DataSource.FindDataOfClass<EventCoinData>(((Component) this).get_gameObject(), (EventCoinData) null);
          if (dataOfClass45 == null || dataOfClass45.param == null)
            break;
          this.SetBuyPriceEventCoinTypeIcon(dataOfClass45.iname);
          break;
        case GameParameter.ParameterTypes.EVENTCOIN_ITEMNAME:
          EventCoinData dataOfClass46 = DataSource.FindDataOfClass<EventCoinData>(((Component) this).get_gameObject(), (EventCoinData) null);
          if (dataOfClass46 != null && dataOfClass46.param != null)
          {
            this.SetTextValue(dataOfClass46.param.name);
            break;
          }
          this.SetTextValue(0);
          break;
        case GameParameter.ParameterTypes.EVENTCOIN_MESSAGE:
          EventCoinData dataOfClass47 = DataSource.FindDataOfClass<EventCoinData>(((Component) this).get_gameObject(), (EventCoinData) null);
          if (dataOfClass47 != null && dataOfClass47.param != null)
          {
            this.SetTextValue(dataOfClass47.param.Flavor);
            break;
          }
          this.SetTextValue(0);
          break;
        case GameParameter.ParameterTypes.EVENTCOIN_POSSESSION:
          EventCoinData dataOfClass48 = DataSource.FindDataOfClass<EventCoinData>(((Component) this).get_gameObject(), (EventCoinData) null);
          if (dataOfClass48 != null && dataOfClass48.have != null)
          {
            this.SetTextValue(dataOfClass48.have.Num);
            break;
          }
          this.SetTextValue(0);
          break;
        case GameParameter.ParameterTypes.EVENTCOIN_PRICEICON:
          EventShopItem eventShopItem2 = this.GetEventShopItem();
          if (eventShopItem2 != null)
          {
            this.SetBuyPriceEventCoinTypeIcon(eventShopItem2.cost_iname);
            break;
          }
          ItemParam dataOfClass49 = DataSource.FindDataOfClass<ItemParam>(((Component) this).get_gameObject(), (ItemParam) null);
          if (dataOfClass49 != null)
          {
            this.SetBuyPriceEventCoinTypeIcon(dataOfClass49.iname);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.SHOP_EVENT_ITEM_BUYAMOUNT:
          EventShopItem eventShopItem3 = this.GetEventShopItem();
          if (eventShopItem3 != null)
          {
            this.SetTextValue(eventShopItem3.remaining_num);
            break;
          }
          this.SetTextValue(0);
          break;
        case GameParameter.ParameterTypes.TROPHY_REMAININGTIME:
          TrophyParam trophyParam5 = this.GetTrophyParam();
          if (trophyParam5 == null)
            break;
          DateTime serverTime2 = TimeManager.ServerTime;
          DateTime base_time = trophyParam5.CategoryParam.end_at.DateTimes;
          if (trophyParam5.CategoryParam.IsBeginner)
          {
            DateTime beginnerEndTime = MonoSingleton<GameManager>.Instance.Player.GetBeginnerEndTime();
            base_time = !(base_time <= beginnerEndTime) ? beginnerEndTime : base_time;
          }
          if (!trophyParam5.CategoryParam.IsBeginner)
            base_time = trophyParam5.CategoryParam.GetQuestTime(base_time, true);
          if (MonoSingleton<GameManager>.Instance.Player.GetTrophyCounter(trophyParam5, false).IsCompleted)
            base_time = trophyParam5.GetGraceRewardTime();
          if (!string.IsNullOrEmpty(trophyParam5.CategoryParam.end_at.StrTime) && !trophyParam5.IsDaily && base_time >= serverTime2)
          {
            TimeSpan timeSpan3 = base_time - serverTime2;
            if (timeSpan3.Days > 0)
              this.SetTextValue(string.Format(LocalizedText.Get("sys.TROPHY_REMAINING_DAY"), (object) timeSpan3.Days));
            else if (timeSpan3.Hours > 0)
              this.SetTextValue(string.Format(LocalizedText.Get("sys.TROPHY_REMAINING_HOUR"), (object) timeSpan3.Hours));
            else
              this.SetTextValue(string.Format(LocalizedText.Get("sys.TROPHY_REMAINING_MINUTE"), (object) timeSpan3.Minutes));
            ((Component) this).get_gameObject().SetActive(true);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_OKYAKUSAMACODE2:
          string configOkyakusamaCode2 = GameUtility.Config_OkyakusamaCode;
          if (this.Index == 0)
          {
            ((Component) this).get_gameObject().SetActive(!string.IsNullOrEmpty(configOkyakusamaCode2));
            break;
          }
          if (this.Index != 1)
            break;
          if (string.IsNullOrEmpty(configOkyakusamaCode2))
          {
            this.ResetToDefault();
            break;
          }
          this.SetTextValue(configOkyakusamaCode2);
          break;
        case GameParameter.ParameterTypes.VERSUS_UNIT_IMAGE:
          if (MonoSingleton<GameManager>.Instance.AudienceMode)
          {
            JSON_MyPhotonPlayerParam player4 = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam().players[this.InstanceType];
            if (player4 != null)
            {
              player4.SetupUnits();
              UnitData unit2 = player4.units[0].unit;
              GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = AssetPath.UnitSkinImage(unit2.UnitParam, unit2.GetSelectedSkin(-1), unit2.CurrentJob.JobID);
              break;
            }
          }
          else
          {
            JSON_MyPhotonPlayerParam versusPlayerParam = this.GetVersusPlayerParam();
            if (versusPlayerParam != null)
            {
              versusPlayerParam.SetupUnits();
              UnitData unit2 = versusPlayerParam.units[0].unit;
              GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = AssetPath.UnitSkinImage(unit2.UnitParam, unit2.GetSelectedSkin(-1), unit2.CurrentJob.JobID);
              break;
            }
            if (MonoSingleton<GameManager>.Instance.IsVSCpuBattle)
            {
              VersusCpuData versusCpu = (VersusCpuData) GlobalVars.VersusCpu;
              if (versusCpu != null && versusCpu.Units != null && versusCpu.Units.Length > 0)
              {
                UnitData unit2 = versusCpu.Units[0];
                GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = AssetPath.UnitSkinImage(unit2.UnitParam, unit2.GetSelectedSkin(-1), unit2.CurrentJob.JobID);
                break;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.VERSUS_PLAYER_NAME:
          if (MonoSingleton<GameManager>.Instance.AudienceMode)
          {
            JSON_MyPhotonPlayerParam player4 = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam().players[this.InstanceType];
            if (player4 != null)
            {
              this.SetTextValue(player4.playerName);
              break;
            }
          }
          else
          {
            JSON_MyPhotonPlayerParam versusPlayerParam = this.GetVersusPlayerParam();
            if (versusPlayerParam != null)
            {
              this.SetTextValue(versusPlayerParam.playerName);
              break;
            }
            if (MonoSingleton<GameManager>.Instance.IsVSCpuBattle)
            {
              VersusCpuData versusCpu = (VersusCpuData) GlobalVars.VersusCpu;
              if (versusCpu != null)
              {
                this.SetTextValue(versusCpu.Name);
                break;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.VERSUS_PLAYER_LEVEL:
          if (MonoSingleton<GameManager>.Instance.AudienceMode)
          {
            JSON_MyPhotonPlayerParam player4 = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam().players[this.InstanceType];
            if (player4 != null)
            {
              this.SetTextValue(player4.playerLevel.ToString());
              break;
            }
          }
          else
          {
            JSON_MyPhotonPlayerParam versusPlayerParam = this.GetVersusPlayerParam();
            if (versusPlayerParam != null)
            {
              this.SetTextValue(versusPlayerParam.playerLevel.ToString());
              break;
            }
            if (MonoSingleton<GameManager>.Instance.IsVSCpuBattle)
            {
              VersusCpuData versusCpu = (VersusCpuData) GlobalVars.VersusCpu;
              if (versusCpu != null)
              {
                this.SetTextValue(versusCpu.Lv);
                break;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.VERSUS_PLAYER_TOTALATK:
          if (MonoSingleton<GameManager>.Instance.AudienceMode)
          {
            JSON_MyPhotonPlayerParam player4 = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam().players[this.InstanceType];
            if (player4 != null)
            {
              this.SetTextValue(player4.totalAtk.ToString());
              break;
            }
          }
          else
          {
            JSON_MyPhotonPlayerParam versusPlayerParam = this.GetVersusPlayerParam();
            if (versusPlayerParam != null)
            {
              this.SetTextValue(versusPlayerParam.totalAtk.ToString());
              break;
            }
            if (MonoSingleton<GameManager>.Instance.IsVSCpuBattle)
            {
              VersusCpuData versusCpu = (VersusCpuData) GlobalVars.VersusCpu;
              if (versusCpu != null)
              {
                this.SetTextValue(versusCpu.Score);
                break;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.VERSUS_RESULT:
          SceneBattle instance33 = SceneBattle.Instance;
          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance33, (UnityEngine.Object) null))
          {
            BattleCore battle = instance33.Battle;
            if (battle != null)
            {
              BattleCore.Record questRecord = battle.GetQuestRecord();
              if (questRecord != null)
              {
                ((Component) this).get_gameObject().SetActive(questRecord.result == (BattleCore.QuestResult) this.InstanceType);
                break;
              }
            }
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.VERSUS_RANK:
          break;
        case GameParameter.ParameterTypes.VERSUS_RANK_POINT:
          break;
        case GameParameter.ParameterTypes.VERSUS_RANK_NEXT_POINT:
          break;
        case GameParameter.ParameterTypes.VERSUS_RANK_ICON:
          break;
        case GameParameter.ParameterTypes.VERSUS_RANK_ICON_INDEX:
          break;
        case GameParameter.ParameterTypes.VERSUS_ROOMPLAYER_RANK_ICON:
          break;
        case GameParameter.ParameterTypes.VERSUS_ROOMPLAYER_RANK_ICON_INDEX:
          break;
        case GameParameter.ParameterTypes.VERSUS_MAP_THUMNAIL:
          Image component7 = (Image) ((Component) this).GetComponent<Image>();
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) component7, (UnityEngine.Object) null))
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          QuestParam dataOfClass50 = DataSource.FindDataOfClass<QuestParam>(((Component) this).get_gameObject(), (QuestParam) null);
          if (dataOfClass50 != null && !string.IsNullOrEmpty(dataOfClass50.VersusThumnail))
          {
            SpriteSheet spriteSheet2 = AssetManager.Load<SpriteSheet>("pvp/pvp_map");
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet2, (UnityEngine.Object) null))
            {
              component7.set_sprite(spriteSheet2.GetSprite(dataOfClass50.VersusThumnail));
              ((Behaviour) component7).set_enabled(true);
              break;
            }
          }
          component7.set_sprite((Sprite) null);
          ((Behaviour) component7).set_enabled(false);
          break;
        case GameParameter.ParameterTypes.VERSUS_MAP_THUMNAIL2:
          Image component8 = (Image) ((Component) this).GetComponent<Image>();
          if (UnityEngine.Object.op_Equality((UnityEngine.Object) component8, (UnityEngine.Object) null))
          {
            ((Component) this).get_gameObject().SetActive(false);
            break;
          }
          VersusMapParam dataOfClass51 = DataSource.FindDataOfClass<VersusMapParam>(((Component) this).get_gameObject(), (VersusMapParam) null);
          if (dataOfClass51 != null && dataOfClass51.quest != null && !string.IsNullOrEmpty(dataOfClass51.quest.VersusThumnail))
          {
            SpriteSheet spriteSheet2 = AssetManager.Load<SpriteSheet>("pvp/pvp_map");
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet2, (UnityEngine.Object) null))
            {
              component8.set_sprite(spriteSheet2.GetSprite(dataOfClass51.quest.VersusThumnail));
              break;
            }
          }
          component8.set_sprite((Sprite) null);
          ((Component) component8).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.VERSUS_MAP_NAME:
          VersusMapParam dataOfClass52 = DataSource.FindDataOfClass<VersusMapParam>(((Component) this).get_gameObject(), (VersusMapParam) null);
          if (dataOfClass52 != null)
          {
            this.SetTextValue(dataOfClass52.quest.name);
            break;
          }
          this.ResetToDefault();
          break;
        case GameParameter.ParameterTypes.VERSUS_MAP_SELECT:
          VersusMapParam dataOfClass53 = DataSource.FindDataOfClass<VersusMapParam>(((Component) this).get_gameObject(), (VersusMapParam) null);
          if (dataOfClass53 != null)
          {
            ((Component) this).get_gameObject().SetActive(dataOfClass53.selected);
            break;
          }
          ((Component) this).get_gameObject().SetActive(false);
          break;
        case GameParameter.ParameterTypes.SHOP_ARENA_COIN:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.ArenaCoin);
          break;
        case GameParameter.ParameterTypes.SHOP_MULTI_COIN:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.MultiCoin);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_COINCOM:
          this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.ComCoin);
          break;
        case GameParameter.ParameterTypes.GLOBAL_PLAYER_FREECOINSET:
          GameManager instance34 = MonoSingleton<GameManager>.Instance;
          this.SetTextValue(instance34.Player.FreeCoin + instance34.Player.ComCoin);
          break;
        default:
          switch (parameterType - 2400)
          {
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
              GameManager instance35 = MonoSingleton<GameManager>.Instance;
              if (instance35.Player.RankMatchRank > 0)
              {
                this.SetTextValue(instance35.Player.RankMatchRank);
                return;
              }
              this.ResetToDefault();
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
              this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.RankMatchScore);
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
              GameManager instance36 = MonoSingleton<GameManager>.Instance;
              int nextVersusRankClass = instance36.GetNextVersusRankClass(instance36.RankMatchScheduleId, instance36.Player.RankMatchClass, instance36.Player.RankMatchScore);
              if (nextVersusRankClass > 0)
              {
                this.SetTextValue(nextVersusRankClass);
                return;
              }
              this.ResetToDefault();
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
              GameManager instance37 = MonoSingleton<GameManager>.Instance;
              int maxBattlePoint = instance37.GetMaxBattlePoint(instance37.RankMatchScheduleId);
              this.SetTextValue(instance37.Player.RankMatchBattlePoint.ToString() + "/" + (object) maxBattlePoint);
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
              instance1 = MonoSingleton<GameManager>.Instance;
              Image component9 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component9, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              VersusRankClassParam dataOfClass54 = DataSource.FindDataOfClass<VersusRankClassParam>(((Component) this).get_gameObject(), (VersusRankClassParam) null);
              if (dataOfClass54 == null)
              {
                component9.set_sprite((Sprite) null);
                ((Behaviour) component9).set_enabled(false);
                return;
              }
              SpriteSheet spriteSheet3 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet3, (UnityEngine.Object) null))
                return;
              component9.set_sprite(spriteSheet3.GetSprite("class_" + (object) dataOfClass54.Class));
              ((Behaviour) component9).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
              Image component10 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component10, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              VersusRankClassParam dataOfClass55 = DataSource.FindDataOfClass<VersusRankClassParam>(((Component) this).get_gameObject(), (VersusRankClassParam) null);
              if (dataOfClass55 == null)
              {
                component10.set_sprite((Sprite) null);
                ((Behaviour) component10).set_enabled(false);
                return;
              }
              SpriteSheet spriteSheet4 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet4, (UnityEngine.Object) null))
                return;
              component10.set_sprite(spriteSheet4.GetSprite("classname_" + (object) dataOfClass55.Class));
              ((Behaviour) component10).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
              Image component11 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component11, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              VersusRankClassParam dataOfClass56 = DataSource.FindDataOfClass<VersusRankClassParam>(((Component) this).get_gameObject(), (VersusRankClassParam) null);
              if (dataOfClass56 == null)
              {
                component11.set_sprite((Sprite) null);
                ((Behaviour) component11).set_enabled(false);
                return;
              }
              SpriteSheet spriteSheet5 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet5, (UnityEngine.Object) null))
                return;
              component11.set_sprite(spriteSheet5.GetSprite("classframe_" + (object) dataOfClass56.Class));
              ((Behaviour) component11).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_COIN:
              GameManager instance38 = MonoSingleton<GameManager>.Instance;
              VersusRankClassParam dataOfClass57 = DataSource.FindDataOfClass<VersusRankClassParam>(((Component) this).get_gameObject(), (VersusRankClassParam) null);
              if (dataOfClass57 == null)
                return;
              ((Component) this).get_gameObject().SetActive(dataOfClass57.Class == instance38.Player.RankMatchClass);
              return;
            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINATIME:
              GameManager instance39 = MonoSingleton<GameManager>.Instance;
              if (instance39.Player.RankMatchRank > 3)
              {
                this.SetTextValue(string.Format(LocalizedText.Get("sys.RANK_MATCH_RANKING_RANK"), (object) instance39.Player.RankMatchRank));
                return;
              }
              ((Component) this).get_gameObject().SetActive(false);
              return;
            case GameParameter.ParameterTypes.QUEST_NAME:
              GameManager instance40 = MonoSingleton<GameManager>.Instance;
              if (instance40.Player.RankMatchRank > 3)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              Image component12 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component12, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet6 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet6, (UnityEngine.Object) null))
                return;
              component12.set_sprite(spriteSheet6.GetSprite("ranking_" + (object) instance40.Player.RankMatchRank));
              ((Behaviour) component12).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.QUEST_STAMINA:
              PlayerData player5 = MonoSingleton<GameManager>.Instance.Player;
              PartyData partyOfType = player5.FindPartyOfType(PlayerPartyTypes.RankMatch);
              int num12 = 0;
              for (int index1 = 0; index1 < partyOfType.MAX_UNIT; ++index1)
              {
                long unitUniqueId = partyOfType.GetUnitUniqueID(index1);
                UnitData unitDataByUniqueId = player5.FindUnitDataByUniqueID(unitUniqueId);
                if (unitDataByUniqueId != null)
                  num12 = num12 + (int) unitDataByUniqueId.Status.param.atk + (int) unitDataByUniqueId.Status.param.mag;
              }
              this.SetTextValue(num12);
              return;
            case GameParameter.ParameterTypes.QUEST_STATE:
              ReqRankMatchRanking.ResponceRanking dataOfClass58 = DataSource.FindDataOfClass<ReqRankMatchRanking.ResponceRanking>(((Component) this).get_gameObject(), (ReqRankMatchRanking.ResponceRanking) null);
              if (dataOfClass58 == null)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              if (dataOfClass58.rank > 3)
              {
                this.SetTextValue(string.Format(LocalizedText.Get("sys.RANK_MATCH_RANKING_RANK"), (object) dataOfClass58.rank));
                return;
              }
              ((Component) this).get_gameObject().SetActive(false);
              return;
            case GameParameter.ParameterTypes.QUEST_OBJECTIVE:
              ReqRankMatchRanking.ResponceRanking dataOfClass59 = DataSource.FindDataOfClass<ReqRankMatchRanking.ResponceRanking>(((Component) this).get_gameObject(), (ReqRankMatchRanking.ResponceRanking) null);
              if (dataOfClass59 == null)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              if (dataOfClass59.rank > 3)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              Image component13 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component13, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet7 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet7, (UnityEngine.Object) null))
                return;
              component13.set_sprite(spriteSheet7.GetSprite("ranking_" + (object) dataOfClass59.rank));
              ((Behaviour) component13).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.QUEST_BONUSOBJECTIVE:
              ReqRankMatchRanking.ResponceRanking dataOfClass60 = DataSource.FindDataOfClass<ReqRankMatchRanking.ResponceRanking>(((Component) this).get_gameObject(), (ReqRankMatchRanking.ResponceRanking) null);
              Image component14 = (Image) ((Component) this).GetComponent<Image>();
              if (dataOfClass60 == null || UnityEngine.Object.op_Equality((UnityEngine.Object) component14, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet8 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet8, (UnityEngine.Object) null))
                return;
              component14.set_sprite(spriteSheet8.GetSprite("class_" + (object) dataOfClass60.type));
              ((Behaviour) component14).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.ITEM_ICON:
              ReqRankMatchRanking.ResponceRanking dataOfClass61 = DataSource.FindDataOfClass<ReqRankMatchRanking.ResponceRanking>(((Component) this).get_gameObject(), (ReqRankMatchRanking.ResponceRanking) null);
              if (dataOfClass61 == null)
                return;
              this.SetTextValue(dataOfClass61.enemy.name);
              return;
            case GameParameter.ParameterTypes.QUEST_DESCRIPTION:
              ReqRankMatchRanking.ResponceRanking dataOfClass62 = DataSource.FindDataOfClass<ReqRankMatchRanking.ResponceRanking>(((Component) this).get_gameObject(), (ReqRankMatchRanking.ResponceRanking) null);
              if (dataOfClass62 == null)
                return;
              this.SetTextValue(dataOfClass62.enemy.lv);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_NAME:
              ReqRankMatchRanking.ResponceRanking dataOfClass63 = DataSource.FindDataOfClass<ReqRankMatchRanking.ResponceRanking>(((Component) this).get_gameObject(), (ReqRankMatchRanking.ResponceRanking) null);
              if (dataOfClass63 == null)
                return;
              this.SetTextValue(dataOfClass63.score);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_LEVEL:
              GameManager instance41 = MonoSingleton<GameManager>.Instance;
              Image component15 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component15, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet9 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet9, (UnityEngine.Object) null))
                return;
              int num13 = (int) instance41.Player.RankMatchClass;
              if (this.InstanceType == 1)
                num13 = (int) instance41.Player.RankMatchOldClass;
              component15.set_sprite(spriteSheet9.GetSprite("class_" + (object) num13));
              ((Behaviour) component15).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_UNITLEVEL:
            case GameParameter.ParameterTypes.SUPPORTER_LEADERSKILLNAME:
              VersusRankRankingRewardParam dataOfClass64 = DataSource.FindDataOfClass<VersusRankRankingRewardParam>(((Component) this).get_gameObject(), (VersusRankRankingRewardParam) null);
              if (dataOfClass64 == null)
              {
                ((Component) ((Component) this).get_transform().get_parent()).get_gameObject().SetActive(false);
                return;
              }
              int num14;
              if (this.ParameterType == GameParameter.ParameterTypes.RANKMATCH_RANKING_RANKREWARD_IMAGESET_BEGIN)
              {
                num14 = dataOfClass64.RankBegin;
                if (num14 == dataOfClass64.RankEnd)
                  ((Component) ((Component) this).get_transform().get_parent()).get_gameObject().SetActive(false);
              }
              else
                num14 = dataOfClass64.RankEnd;
              if (dataOfClass64.RankBegin > 3)
              {
                this.SetTextValue(num14);
                return;
              }
              ((Component) ((Component) this).get_transform().get_parent()).get_gameObject().SetActive(false);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_ATK:
              VersusRankRankingRewardParam dataOfClass65 = DataSource.FindDataOfClass<VersusRankRankingRewardParam>(((Component) this).get_gameObject(), (VersusRankRankingRewardParam) null);
              if (dataOfClass65 == null)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              if (dataOfClass65.RankBegin > 3)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              Image component16 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component16, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet10 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet10, (UnityEngine.Object) null))
                return;
              component16.set_sprite(spriteSheet10.GetSprite("ranking_" + (object) dataOfClass65.RankBegin));
              ((Behaviour) component16).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_HP:
              ReqRankMatchHistory.ResponceHistoryList dataOfClass66 = DataSource.FindDataOfClass<ReqRankMatchHistory.ResponceHistoryList>(((Component) this).get_gameObject(), (ReqRankMatchHistory.ResponceHistoryList) null);
              if (dataOfClass66 == null)
                return;
              Transform transform1 = !(dataOfClass66.result.result == "win") ? (dataOfClass66.result.result == "lose" || dataOfClass66.result.result == "retire" || (dataOfClass66.result.result == "cancel" || dataOfClass66.result.result == "crashed") ? ((Component) this).get_transform().FindChild("Lose") : ((Component) this).get_transform().FindChild("Draw")) : ((Component) this).get_transform().FindChild("Win");
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) transform1, (UnityEngine.Object) null))
                return;
              ((Component) transform1).get_gameObject().SetActive(true);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_MAGIC:
              ReqRankMatchHistory.ResponceHistoryList dataOfClass67 = DataSource.FindDataOfClass<ReqRankMatchHistory.ResponceHistoryList>(((Component) this).get_gameObject(), (ReqRankMatchHistory.ResponceHistoryList) null);
              string str8 = "0";
              Color color1 = Color.get_white();
              if (dataOfClass67 != null)
              {
                str8 = dataOfClass67.value.ToString();
                if (dataOfClass67.result.result == "win")
                {
                  str8 = "+" + str8;
                  color1 = Color.get_cyan();
                }
                else if (dataOfClass67.result.result == "lose")
                  color1 = Color.get_red();
              }
              this.mText.set_text(str8);
              ((Graphic) this.mText).set_color(color1);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_RARITY:
              ReqRankMatchHistory.ResponceHistoryList dataOfClass68 = DataSource.FindDataOfClass<ReqRankMatchHistory.ResponceHistoryList>(((Component) this).get_gameObject(), (ReqRankMatchHistory.ResponceHistoryList) null);
              Image component17 = (Image) ((Component) this).GetComponent<Image>();
              if (dataOfClass68 == null || UnityEngine.Object.op_Equality((UnityEngine.Object) component17, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet11 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet11, (UnityEngine.Object) null))
                return;
              component17.set_sprite(spriteSheet11.GetSprite("class_" + (object) dataOfClass68.type));
              ((Behaviour) component17).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_ELEMENT:
              ReqRankMatchHistory.ResponceHistoryList dataOfClass69 = DataSource.FindDataOfClass<ReqRankMatchHistory.ResponceHistoryList>(((Component) this).get_gameObject(), (ReqRankMatchHistory.ResponceHistoryList) null);
              if (dataOfClass69 == null)
                return;
              this.SetTextValue(dataOfClass69.enemy.name);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_ICON:
              ReqRankMatchHistory.ResponceHistoryList dataOfClass70 = DataSource.FindDataOfClass<ReqRankMatchHistory.ResponceHistoryList>(((Component) this).get_gameObject(), (ReqRankMatchHistory.ResponceHistoryList) null);
              if (dataOfClass70 == null)
                return;
              this.SetTextValue(dataOfClass70.enemy.lv);
              return;
            case GameParameter.ParameterTypes.SUPPORTER_LEADERSKILLDESC:
              ReqRankMatchHistory.ResponceHistoryList dataOfClass71 = DataSource.FindDataOfClass<ReqRankMatchHistory.ResponceHistoryList>(((Component) this).get_gameObject(), (ReqRankMatchHistory.ResponceHistoryList) null);
              if (dataOfClass71 == null)
                return;
              this.SetTextValue(dataOfClass71.enemyscore);
              return;
            case GameParameter.ParameterTypes.QUEST_SUBTITLE:
              VersusRankMissionParam dataOfClass72 = DataSource.FindDataOfClass<VersusRankMissionParam>(((Component) this).get_gameObject(), (VersusRankMissionParam) null);
              if (dataOfClass72 == null)
                return;
              this.SetTextValue(dataOfClass72.Name);
              return;
            case GameParameter.ParameterTypes.UNIT_LEVEL:
              VersusRankMissionParam dataOfClass73 = DataSource.FindDataOfClass<VersusRankMissionParam>(((Component) this).get_gameObject(), (VersusRankMissionParam) null);
              if (dataOfClass73 == null)
                return;
              this.SetTextValue(dataOfClass73.IVal);
              return;
            case GameParameter.ParameterTypes.UNIT_HP:
              ReqRankMatchMission.MissionProgress dataOfClass74 = DataSource.FindDataOfClass<ReqRankMatchMission.MissionProgress>(((Component) this).get_gameObject(), (ReqRankMatchMission.MissionProgress) null);
              if (dataOfClass74 == null)
              {
                this.SetTextValue("0");
                return;
              }
              this.SetTextValue(dataOfClass74.prog);
              return;
            case GameParameter.ParameterTypes.UNIT_HPMAX:
              GameManager instance42 = MonoSingleton<GameManager>.Instance;
              Image component18 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component18, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet12 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet12, (UnityEngine.Object) null))
                return;
              int num15 = (int) instance42.Player.RankMatchClass;
              if (this.InstanceType == 1)
                num15 = (int) instance42.Player.RankMatchOldClass;
              component18.set_sprite(spriteSheet12.GetSprite("classname_result_" + (object) num15));
              ((Behaviour) component18).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.UNIT_ATK:
              this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.RankMatchTotalCount);
              return;
            case GameParameter.ParameterTypes.UNIT_MAG:
              this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.RankMatchWinCount);
              return;
            case GameParameter.ParameterTypes.UNIT_ICON:
              this.SetTextValue(MonoSingleton<GameManager>.Instance.Player.RankMatchLoseCount);
              return;
            case GameParameter.ParameterTypes.UNIT_NAME:
              GameManager instance43 = MonoSingleton<GameManager>.Instance;
              int num16 = 0;
              int num17 = 1;
              VersusRankClassParam versusRankClass = instance43.GetVersusRankClass(instance43.RankMatchScheduleId, instance43.Player.RankMatchClass);
              if (versusRankClass != null)
              {
                num16 = versusRankClass.UpPoint - instance43.Player.RankMatchScore;
                num17 = versusRankClass.UpPoint - versusRankClass.DownPoint;
              }
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSlider, (UnityEngine.Object) null))
                return;
              this.mSlider.set_value((float) (1.0 - (double) num16 / (double) num17));
              return;
            case GameParameter.ParameterTypes.UNIT_RARITY:
              VersusRankRankingRewardParam dataOfClass75 = DataSource.FindDataOfClass<VersusRankRankingRewardParam>(((Component) this).get_gameObject(), (VersusRankRankingRewardParam) null);
              if (dataOfClass75 == null)
              {
                ((Component) ((Component) this).get_transform()).get_gameObject().SetActive(false);
                return;
              }
              if (dataOfClass75.RankBegin != dataOfClass75.RankEnd)
                return;
              ((Component) ((Component) this).get_transform()).get_gameObject().SetActive(false);
              return;
            case GameParameter.ParameterTypes.PARTY_LEADERSKILLNAME:
              GameManager instance44 = MonoSingleton<GameManager>.Instance;
              List<VersusRankMissionParam> versusRankMissionList = instance44.GetVersusRankMissionList(instance44.RankMatchScheduleId);
              bool flag15 = false;
              using (List<RankMatchMissionState>.Enumerator enumerator = instance44.Player.RankMatchMissionState.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  RankMatchMissionState rmms = enumerator.Current;
                  VersusRankMissionParam rankMissionParam = versusRankMissionList.Find((Predicate<VersusRankMissionParam>) (mission => mission.IName == rmms.IName));
                  if (!rmms.IsRewarded && rankMissionParam != null && rankMissionParam.IVal <= rmms.Progress)
                  {
                    flag15 = true;
                    break;
                  }
                }
              }
              ((Component) this).get_gameObject().SetActive(flag15);
              return;
            case GameParameter.ParameterTypes.PARTY_LEADERSKILLDESC:
              GameManager instance45 = MonoSingleton<GameManager>.Instance;
              if (instance45.Player.mRankMatchSeasonResult == null)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              this.SetTextValue(instance45.Player.mRankMatchSeasonResult.Rank);
              return;
            case GameParameter.ParameterTypes.UNIT_DEF:
              GameManager instance46 = MonoSingleton<GameManager>.Instance;
              if (instance46.Player.mRankMatchSeasonResult == null)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              if (instance46.Player.mRankMatchSeasonResult.Rank > 3)
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              Image component19 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component19, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet13 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet13, (UnityEngine.Object) null))
                return;
              component19.set_sprite(spriteSheet13.GetSprite("ranking_" + (object) instance46.Player.mRankMatchSeasonResult.Rank));
              ((Behaviour) component19).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.UNIT_MND:
              GameManager instance47 = MonoSingleton<GameManager>.Instance;
              Image component20 = (Image) ((Component) this).GetComponent<Image>();
              if (instance47.Player.mRankMatchSeasonResult == null || UnityEngine.Object.op_Equality((UnityEngine.Object) component20, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet14 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet14, (UnityEngine.Object) null))
                return;
              component20.set_sprite(spriteSheet14.GetSprite("class_" + (object) instance47.Player.mRankMatchSeasonResult.Class));
              ((Behaviour) component20).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.UNIT_SPEED:
              GameManager instance48 = MonoSingleton<GameManager>.Instance;
              if (instance48.Player.mRankMatchSeasonResult == null)
                return;
              this.SetTextValue(instance48.Player.mRankMatchSeasonResult.Score);
              return;
            case GameParameter.ParameterTypes.UNIT_LUCK:
              this.InstanceType = 1;
              JSON_MyPhotonPlayerParam versusPlayerParam1 = this.GetVersusPlayerParam();
              if (versusPlayerParam1 != null)
              {
                this.SetTextValue(versusPlayerParam1.rankmatch_score.ToString());
                return;
              }
              this.ResetToDefault();
              return;
            case GameParameter.ParameterTypes.UNIT_JOBNAME:
              GameManager instance49 = MonoSingleton<GameManager>.Instance;
              if (instance49.Player.mRankMatchSeasonResult == null)
                return;
              Image component21 = (Image) ((Component) this).GetComponent<Image>();
              if (UnityEngine.Object.op_Equality((UnityEngine.Object) component21, (UnityEngine.Object) null))
              {
                ((Component) this).get_gameObject().SetActive(false);
                return;
              }
              SpriteSheet spriteSheet15 = AssetManager.Load<SpriteSheet>("pvp/rankmatch_class");
              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet15, (UnityEngine.Object) null))
                return;
              int num18 = (int) instance49.Player.mRankMatchSeasonResult.Class;
              component21.set_sprite(spriteSheet15.GetSprite("classname_result_" + (object) num18));
              ((Behaviour) component21).set_enabled(true);
              return;
            case GameParameter.ParameterTypes.UNIT_JOBRANK:
              GameManager instance50 = MonoSingleton<GameManager>.Instance;
              if (instance50.Player.mRankMatchSeasonResult == null)
                return;
              VersusRankParam versusRankParam1 = instance50.GetVersusRankParam(instance50.Player.mRankMatchSeasonResult.ScheduleId);
              if (versusRankParam1 == null)
                return;
              LocalizedText.Get("sys.RANK_MATCH_MATCHING_SEASONTIME");
              this.SetTextValue(string.Format(LocalizedText.Get("sys.RANK_MATCH_MATCHING_SEASONTIME"), (object) versusRankParam1.BeginAt.Month, (object) versusRankParam1.BeginAt.Day, (object) versusRankParam1.BeginAt.Hour, (object) versusRankParam1.BeginAt.Minute, (object) versusRankParam1.EndAt.Month, (object) versusRankParam1.EndAt.Day, (object) versusRankParam1.EndAt.Hour, (object) versusRankParam1.EndAt.Minute));
              return;
            case GameParameter.ParameterTypes.UNIT_ELEMENT:
              GameManager instance51 = MonoSingleton<GameManager>.Instance;
              VersusRankParam versusRankParam2 = instance51.GetVersusRankParam(instance51.RankMatchScheduleId);
              if (versusRankParam2 == null)
                return;
              this.SetTextValue(versusRankParam2.Name);
              return;
            default:
              switch (parameterType - 1000)
              {
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                  SceneBattle instance52 = SceneBattle.Instance;
                  if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance52, (UnityEngine.Object) null) && instance52.Battle != null)
                  {
                    if (instance52.Battle.GetQuestResult() == BattleCore.QuestResult.Win)
                    {
                      this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_REWARD_WIN"));
                      return;
                    }
                    this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_REWARD_JOIN"));
                    return;
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                  ((Component) this).get_gameObject().SetActive(MonoSingleton<GameManager>.Instance.Player.VersusSeazonGiftReceipt);
                  return;
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                  instance1 = MonoSingleton<GameManager>.Instance;
                  SceneBattle instance53 = SceneBattle.Instance;
                  if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance53, (UnityEngine.Object) null) && instance53.CurrentQuest != null)
                  {
                    BattleCore battle = instance53.Battle;
                    if (battle != null)
                    {
                      BattleCore.Record questRecord = battle.GetQuestRecord();
                      if (questRecord != null)
                      {
                        this.SetTextValue((int) questRecord.pvpcoin);
                        return;
                      }
                    }
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                  this.SetTextValue(string.Format(LocalizedText.Get("sys.MULTI_VERSUS_REMAIN_COIN"), (object) MonoSingleton<GameManager>.Instance.VersusCoinRemainCnt));
                  return;
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                  GameManager instance54 = MonoSingleton<GameManager>.Instance;
                  if (this.Index == 0)
                  {
                    ((Component) this).get_gameObject().SetActive(instance54.VersusTowerMatchBegin);
                    return;
                  }
                  if (this.Index != 1)
                    return;
                  ((Component) this).get_gameObject().SetActive(!instance54.VersusTowerMatchBegin);
                  return;
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                  if (MonoSingleton<GameManager>.Instance.IsVSCpuBattle)
                  {
                    this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_CPU"));
                    return;
                  }
                  switch (GlobalVars.SelectedMultiPlayVersusType)
                  {
                    case VERSUS_TYPE.Free:
                      this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_FREE"));
                      return;
                    case VERSUS_TYPE.Tower:
                      this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_TOWER"));
                      return;
                    case VERSUS_TYPE.Friend:
                      this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_FRIEND"));
                      return;
                    case VERSUS_TYPE.RankMatch:
                      this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_RANKMATCH"));
                      return;
                    default:
                      return;
                  }
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                  PlayerData player6 = MonoSingleton<GameManager>.Instance.Player;
                  SceneBattle instance55 = SceneBattle.Instance;
                  if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance55, (UnityEngine.Object) null) && player6 != null)
                  {
                    BattleCore battle = instance55.Battle;
                    if (battle != null)
                    {
                      BattleCore.Record questRecord = battle.GetQuestRecord();
                      if (questRecord != null)
                      {
                        ((Component) this).get_gameObject().SetActive(questRecord.result == BattleCore.QuestResult.Win && player6.VersusTowerWinBonus > 0);
                        return;
                      }
                    }
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_COIN:
                  SceneBattle bs4 = SceneBattle.Instance;
                  if (UnityEngine.Object.op_Equality((UnityEngine.Object) bs4, (UnityEngine.Object) null) || bs4.Battle == null || bs4.Battle.CurrentUnit == null)
                  {
                    this.ResetToDefault();
                    return;
                  }
                  MyPhoton instance56 = PunMonoSingleton<MyPhoton>.Instance;
                  JSON_MyPhotonPlayerParam param = instance56.GetMyPlayersStarted()?.Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerIndex == bs4.Battle.CurrentUnit.OwnerPlayerIndex));
                  List<MyPhoton.MyPlayer> roomPlayerList4 = instance56.GetRoomPlayerList();
                  MyPhoton.MyPlayer myPlayer3 = roomPlayerList4 == null || param == null ? (MyPhoton.MyPlayer) null : roomPlayerList4.Find((Predicate<MyPhoton.MyPlayer>) (p => p.playerID == param.playerID));
                  if (MonoSingleton<GameManager>.Instance.AudienceMode)
                  {
                    ((Component) this).get_gameObject().SetActive(true);
                    return;
                  }
                  if (this.Index == 0)
                  {
                    ((Component) this).get_gameObject().SetActive(myPlayer3 == null);
                    return;
                  }
                  if (this.Index == 1)
                  {
                    ((Component) this).get_gameObject().SetActive(myPlayer3 != null);
                    return;
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINATIME:
                  SceneBattle instance57 = SceneBattle.Instance;
                  if (UnityEngine.Object.op_Equality((UnityEngine.Object) instance57, (UnityEngine.Object) null) || instance57.Battle == null || instance57.Battle.CurrentUnit == null)
                  {
                    this.ResetToDefault();
                    return;
                  }
                  BattleCore.QuestResult questResult = instance57.CheckAudienceResult();
                  if (questResult == BattleCore.QuestResult.Pending)
                  {
                    this.ResetToDefault();
                    return;
                  }
                  if (this.Index == 0)
                  {
                    ((Component) this).get_gameObject().SetActive(questResult == BattleCore.QuestResult.Win);
                    return;
                  }
                  if (this.Index != 1)
                    return;
                  ((Component) this).get_gameObject().SetActive(questResult == BattleCore.QuestResult.Lose);
                  return;
                case GameParameter.ParameterTypes.QUEST_NAME:
                  ((Component) this).get_gameObject().SetActive(MonoSingleton<GameManager>.Instance.AudienceMode);
                  return;
                case GameParameter.ParameterTypes.QUEST_STAMINA:
                  MyPhoton.MyRoom audienceRoom = MonoSingleton<GameManager>.Instance.AudienceRoom;
                  if (audienceRoom != null)
                  {
                    if (audienceRoom.name.IndexOf("_free") != -1)
                    {
                      this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_ADUIENCE_FREE"));
                      return;
                    }
                    if (audienceRoom.name.IndexOf("_tower") != -1)
                    {
                      this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_ADUIENCE_TOWER"));
                      return;
                    }
                    if (audienceRoom.name.IndexOf("_friend") == -1)
                      return;
                    this.SetTextValue(LocalizedText.Get("sys.MULTI_VERSUS_ADUIENCE_FRIEND"));
                    return;
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.QUEST_STATE:
                  this.SetTextValue(string.Format(LocalizedText.Get("sys.MULTI_VERSUS_TOWER_NOW_FLOOR"), (object) GameUtility.HalfNum2FullNum(MonoSingleton<GameManager>.Instance.Player.VersusTowerFloor.ToString())));
                  return;
                case GameParameter.ParameterTypes.QUEST_OBJECTIVE:
                  ((Component) this).get_gameObject().SetActive(GlobalVars.SelectedMultiPlayVersusType == VERSUS_TYPE.Tower);
                  return;
                case GameParameter.ParameterTypes.QUEST_BONUSOBJECTIVE:
                  GameManager instance58 = MonoSingleton<GameManager>.Instance;
                  if (instance58.AudienceRoom != null)
                  {
                    int audienceMax = instance58.AudienceRoom.audienceMax;
                    JSON_MyPhotonRoomParam roomParam3 = instance58.AudienceManager.GetRoomParam();
                    if (roomParam3 != null)
                    {
                      this.SetTextValue(string.Format(LocalizedText.Get("sys.MULTI_VERSUS_AUDIENCE_NUM"), (object) GameUtility.HalfNum2FullNum(roomParam3.audienceNum.ToString()), (object) GameUtility.HalfNum2FullNum(audienceMax.ToString())));
                      return;
                    }
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.ITEM_ICON:
                  if (GlobalVars.VersusCpu != null)
                  {
                    ImageArray component6 = (ImageArray) ((Component) this).get_gameObject().GetComponent<ImageArray>();
                    if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component6, (UnityEngine.Object) null))
                    {
                      component6.ImageIndex = GlobalVars.VersusCpu.Get().Deck - 1;
                      return;
                    }
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.QUEST_DESCRIPTION:
                  VersusCpuData dataOfClass76 = DataSource.FindDataOfClass<VersusCpuData>(((Component) this).get_gameObject(), (VersusCpuData) null);
                  if (dataOfClass76 != null)
                  {
                    this.SetTextValue(dataOfClass76.Name);
                    return;
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.SUPPORTER_NAME:
                  VersusCpuData dataOfClass77 = DataSource.FindDataOfClass<VersusCpuData>(((Component) this).get_gameObject(), (VersusCpuData) null);
                  if (dataOfClass77 != null)
                  {
                    this.SetTextValue(dataOfClass77.Lv);
                    return;
                  }
                  this.ResetToDefault();
                  return;
                case GameParameter.ParameterTypes.SUPPORTER_LEVEL:
                  VersusCpuData dataOfClass78 = DataSource.FindDataOfClass<VersusCpuData>(((Component) this).get_gameObject(), (VersusCpuData) null);
                  if (dataOfClass78 != null)
                  {
                    this.SetTextValue(dataOfClass78.Score);
                    return;
                  }
                  this.ResetToDefault();
                  return;
                default:
                  switch (parameterType - 1700)
                  {
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                      QuestParam questParam26;
                      if ((questParam26 = this.GetQuestParam()) != null && questParam26.bonusObjective != null)
                      {
                        int compCount;
                        int maxCount;
                        this.GetQuestObjectiveCount(questParam26, out compCount, out maxCount);
                        if (compCount > 0)
                        {
                          if (compCount > 0 && compCount < maxCount)
                          {
                            this.SetImageIndex(0);
                            return;
                          }
                          this.SetImageIndex(1);
                          return;
                        }
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                      QuestParam questParam27;
                      if ((questParam27 = this.GetQuestParam()) != null && questParam27.bonusObjective != null)
                      {
                        int compCount;
                        int maxCount;
                        this.GetQuestObjectiveCount(questParam27, out compCount, out maxCount);
                        this.SetTextValue(compCount);
                        if (compCount < maxCount)
                          return;
                        ((Graphic) this.mText).set_color(new Color(1f, 1f, 0.0f));
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                      QuestParam questParam28;
                      if ((questParam28 = this.GetQuestParam()) != null && questParam28.bonusObjective != null)
                      {
                        int compCount;
                        int maxCount;
                        this.GetQuestObjectiveCount(questParam28, out compCount, out maxCount);
                        this.SetTextValue(maxCount);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                      QuestParam questParamAuto4 = this.GetQuestParamAuto();
                      if (questParamAuto4 != null && 0 <= this.Index && (questParamAuto4.bonusObjective != null && this.Index < questParamAuto4.bonusObjective.Length))
                      {
                        this.SetTextValue(questParamAuto4.bonusObjective[this.Index].itemNum);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                      QuestParam questParamAuto5 = this.GetQuestParamAuto();
                      if (questParamAuto5 != null && 0 <= this.Index && (questParamAuto5.bonusObjective != null && this.Index < questParamAuto5.bonusObjective.Length))
                      {
                        if (questParamAuto5.bonusObjective[this.Index].IsProgressMission())
                        {
                          ((Component) this).get_gameObject().SetActive(true);
                          return;
                        }
                        ((Component) this).get_gameObject().SetActive(false);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                      QuestParam questParamAuto6 = this.GetQuestParamAuto();
                      if (questParamAuto6 != null && 0 <= this.Index && (questParamAuto6.bonusObjective != null && this.Index < questParamAuto6.bonusObjective.Length))
                      {
                        if (questParamAuto6.bonusObjective[this.Index].IsProgressMission())
                        {
                          if (questParamAuto6.mission_values != null && this.Index < questParamAuto6.mission_values.Length)
                          {
                            int missions_val = Mathf.Max(questParamAuto6.GetMissionValue(this.Index), 0);
                            bool isAchievable = questParamAuto6.bonusObjective[this.Index].CheckHomeMissionValueAchievable(missions_val);
                            this.SetTextValue(GameUtility.ComposeQuestMissionProgressText(questParamAuto6.bonusObjective[this.Index], missions_val, isAchievable));
                            return;
                          }
                          this.SetTextValue("-");
                          return;
                        }
                        ((Component) this).get_gameObject().SetActive(false);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                      QuestParam questParamAuto7 = this.GetQuestParamAuto();
                      if (questParamAuto7 != null && 0 <= this.Index && (questParamAuto7.bonusObjective != null && this.Index < questParamAuto7.bonusObjective.Length))
                      {
                        int result = 0;
                        if (questParamAuto7.bonusObjective[this.Index].IsProgressMission())
                        {
                          string questMissionTextId = GameUtility.GetQuestMissionTextID(questParamAuto7.bonusObjective[this.Index]);
                          if (int.TryParse(questParamAuto7.bonusObjective[this.Index].TypeParam, out result))
                          {
                            this.SetTextValue(LocalizedText.Get(questMissionTextId + "_PROGRESS_TARGET", new object[1]
                            {
                              (object) result
                            }));
                            return;
                          }
                          this.SetTextValue(LocalizedText.Get(questMissionTextId + "_PROGRESS_TARGET"));
                          return;
                        }
                        ((Component) this).get_gameObject().SetActive(false);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_COIN:
                      QuestParam questParam29;
                      if ((questParam29 = this.GetQuestParam()) != null && questParam29.HasMission())
                      {
                        int index1 = !questParam29.IsMissionCompleteALL() || questParam29.state != QuestStates.Cleared ? 0 : 1;
                        this.SetAnimatorInt("state", index1);
                        this.SetImageIndex(index1);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINATIME:
                      QuestParam questParam30;
                      if ((questParam30 = this.GetQuestParam()) != null && questParam30.bonusObjective != null)
                      {
                        int compCount;
                        int maxCount;
                        this.GetQuestObjectiveCount(questParam30, out compCount, out maxCount);
                        if (questParam30.IsMissionCompleteALL() && questParam30.state == QuestStates.Cleared)
                          ++compCount;
                        int num3 = maxCount + 1;
                        this.SetTextValue(compCount);
                        if (compCount < num3)
                          return;
                        ((Graphic) this.mText).set_color(new Color(1f, 1f, 0.0f));
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.QUEST_NAME:
                      QuestParam questParam31;
                      if ((questParam31 = this.GetQuestParam()) != null && questParam31.bonusObjective != null)
                      {
                        int compCount;
                        int maxCount;
                        this.GetQuestObjectiveCount(questParam31, out compCount, out maxCount);
                        this.SetTextValue(maxCount + 1);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    case GameParameter.ParameterTypes.QUEST_STAMINA:
                      QuestParam questParamAuto8 = this.GetQuestParamAuto();
                      if (questParamAuto8 != null && 0 <= this.Index && (questParamAuto8.bonusObjective != null && this.Index < questParamAuto8.bonusObjective.Length))
                      {
                        if (questParamAuto8.bonusObjective[this.Index].IsProgressMission())
                        {
                          if (questParamAuto8.mission_values != null && this.Index < questParamAuto8.mission_values.Length)
                          {
                            int missions_val = Mathf.Max(questParamAuto8.GetMissionValue(this.Index), 0);
                            bool isAchievable = questParamAuto8.bonusObjective[this.Index].CheckHomeMissionValueAchievable(missions_val);
                            Color color2 = Color.get_white();
                            if (!questParamAuto8.IsMissionClear(this.Index))
                              color2 = !isAchievable ? Color.get_red() : Color.get_green();
                            this.SetTextValue(GameUtility.ComposeQuestMissionProgressText(questParamAuto8.bonusObjective[this.Index], missions_val, isAchievable));
                            this.SetTextColor(color2);
                            return;
                          }
                          this.SetTextValue("-");
                          return;
                        }
                        ((Component) this).get_gameObject().SetActive(false);
                        return;
                      }
                      this.ResetToDefault();
                      return;
                    default:
                      switch (parameterType - 1600)
                      {
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                          MultiPlayAPIRoom dataOfClass79 = DataSource.FindDataOfClass<MultiPlayAPIRoom>(((Component) this).get_gameObject(), (MultiPlayAPIRoom) null);
                          if (dataOfClass79 != null)
                          {
                            if (dataOfClass79.limit == 0)
                            {
                              this.SetTextValue("-");
                              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null))
                                return;
                              ((Graphic) this.mText).set_color(new Color(0.0f, 0.0f, 0.0f));
                              return;
                            }
                            if (dataOfClass79.unitlv == 0)
                            {
                              this.SetTextValue(LocalizedText.Get("sys.MULTI_JOINLIMIT_NONE"));
                              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null))
                                return;
                              ((Graphic) this.mText).set_color(new Color(0.0f, 0.0f, 0.0f));
                              return;
                            }
                            this.SetTextValue(dataOfClass79.unitlv.ToString() + LocalizedText.Get("sys.MULTI_JOINLIMIT_OVER"));
                            if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null))
                              return;
                            GameManager instance5 = MonoSingleton<GameManager>.Instance;
                            PlayerData player4 = instance5.Player;
                            PartyData party = player4.Partys[2];
                            QuestParam quest = instance5.FindQuest(dataOfClass79.quest.iname);
                            bool flag11 = true;
                            if (party != null && quest != null)
                            {
                              for (int index1 = 0; index1 < (int) quest.unitNum; ++index1)
                              {
                                long unitUniqueId = party.GetUnitUniqueID(0);
                                if (unitUniqueId <= 0L)
                                {
                                  flag11 = false;
                                  break;
                                }
                                UnitData unitDataByUniqueId = player4.FindUnitDataByUniqueID(unitUniqueId);
                                if (unitDataByUniqueId != null)
                                  flag11 &= unitDataByUniqueId.CalcLevel() >= dataOfClass79.unitlv;
                              }
                            }
                            if (flag11)
                            {
                              ((Graphic) this.mText).set_color(new Color(0.0f, 0.0f, 0.0f));
                              return;
                            }
                            ((Graphic) this.mText).set_color(new Color(1f, 0.0f, 0.0f));
                            return;
                          }
                          this.ResetToDefault();
                          return;
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                          MultiPlayAPIRoom dataOfClass80 = DataSource.FindDataOfClass<MultiPlayAPIRoom>(((Component) this).get_gameObject(), (MultiPlayAPIRoom) null);
                          if (dataOfClass80 != null)
                          {
                            if (dataOfClass80.limit == 0)
                            {
                              this.SetTextValue("-");
                              return;
                            }
                            if (dataOfClass80.clear == 0)
                            {
                              this.SetTextValue(LocalizedText.Get("sys.MULTI_JOINLIMIT_NONE"));
                              return;
                            }
                            this.SetTextValue(LocalizedText.Get("sys.MULTI_JOIN_LIMIT_ONLY_CLEAR"));
                            return;
                          }
                          this.ResetToDefault();
                          return;
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                          GameManager instance59 = MonoSingleton<GameManager>.Instance;
                          SceneBattle bs5 = SceneBattle.Instance;
                          if (UnityEngine.Object.op_Equality((UnityEngine.Object) bs5, (UnityEngine.Object) null) || bs5.Battle == null || bs5.Battle.CurrentUnit == null)
                          {
                            this.ResetToDefault();
                            return;
                          }
                          if (instance59.AudienceMode)
                          {
                            AudienceStartParam startedParam = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam();
                            if (bs5.Battle.CurrentUnit.OwnerPlayerIndex <= 0 || bs5.Battle.CurrentUnit.OwnerPlayerIndex > 2)
                            {
                              this.ResetToDefault();
                              return;
                            }
                            JSON_MyPhotonPlayerParam player4 = startedParam.players[bs5.Battle.CurrentUnit.OwnerPlayerIndex - 1];
                            if (player4 == null)
                              return;
                            string str3 = player4.playerName;
                            if (this.Index == 1)
                              str3 = player4.playerName + LocalizedText.Get("sys.MULTI_BATTLE_THINKING");
                            else if (this.Index == 2)
                              str3 = player4.playerName + LocalizedText.Get("sys.MULTI_BATTLE_NOWTURN");
                            this.SetTextValue(str3);
                            return;
                          }
                          if (instance59.IsVSCpuBattle)
                          {
                            string name = instance59.Player.Name;
                            bool flag11 = bs5.Battle.CurrentUnit.OwnerPlayerIndex != 1;
                            if (flag11)
                            {
                              VersusCpuData versusCpu = (VersusCpuData) GlobalVars.VersusCpu;
                              if (versusCpu != null)
                                name = versusCpu.Name;
                            }
                            if (this.Index == 1)
                              name += LocalizedText.Get("sys.MULTI_BATTLE_THINKING");
                            else if (this.Index == 2)
                            {
                              if (flag11)
                                name += LocalizedText.Get("sys.MULTI_VS_CPU");
                              name += LocalizedText.Get("sys.MULTI_BATTLE_NOWTURN");
                            }
                            this.SetTextValue(name);
                            return;
                          }
                          MyPhoton instance60 = PunMonoSingleton<MyPhoton>.Instance;
                          JSON_MyPhotonPlayerParam photonPlayerParam4 = instance60.GetMyPlayersStarted()?.Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerIndex == bs5.Battle.CurrentUnit.OwnerPlayerIndex));
                          List<MyPhoton.MyPlayer> roomPlayerList5 = instance60.GetRoomPlayerList();
                          MyPhoton.MyPlayer myPlayer4 = photonPlayerParam4 != null ? instance60.FindPlayer(roomPlayerList5, photonPlayerParam4.playerID, photonPlayerParam4.playerIndex) : (MyPhoton.MyPlayer) null;
                          if (photonPlayerParam4 == null)
                          {
                            this.ResetToDefault();
                            return;
                          }
                          string str9 = photonPlayerParam4.playerName;
                          if (this.Index == 0)
                          {
                            if (myPlayer4 == null)
                              ((Graphic) this.mText).set_color(new Color(0.5f, 0.5f, 0.5f));
                            else
                              ((Graphic) this.mText).set_color(new Color(1f, 1f, 1f));
                          }
                          else if (this.Index == 1)
                            str9 = photonPlayerParam4.playerName + LocalizedText.Get("sys.MULTI_BATTLE_THINKING");
                          else if (this.Index == 2)
                            str9 = photonPlayerParam4.playerName + LocalizedText.Get("sys.MULTI_BATTLE_NOWTURN");
                          this.SetTextValue(str9);
                          return;
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                          JSON_MyPhotonRoomParam roomParam6 = this.GetRoomParam();
                          JSON_MyPhotonPlayerParam roomPlayerParam10 = this.GetRoomPlayerParam();
                          if (roomPlayerParam10 == null || roomPlayerParam10.units == null || roomParam6 == null)
                          {
                            this.ResetToDefault();
                            return;
                          }
                          int num19 = 0;
                          int unitSlotNum2 = roomParam6.GetUnitSlotNum(roomPlayerParam10.playerIndex);
                          for (int index1 = 0; index1 < roomPlayerParam10.units.Length; ++index1)
                          {
                            if (roomPlayerParam10.units[index1].slotID < unitSlotNum2 && roomPlayerParam10.units[index1].unit != null)
                              num19 = num19 + (int) roomPlayerParam10.units[index1].unit.Status.param.atk + (int) roomPlayerParam10.units[index1].unit.Status.param.mag;
                          }
                          this.SetTextValue(num19);
                          return;
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                          List<MyPhoton.MyPlayer> roomPlayerList6 = PunMonoSingleton<MyPhoton>.Instance.GetRoomPlayerList();
                          if (roomPlayerList6 == null)
                          {
                            ((Component) this).get_gameObject().SetActive(false);
                            return;
                          }
                          MyPhoton.MyPlayer myPlayer5 = roomPlayerList6.Find((Predicate<MyPhoton.MyPlayer>) (p => p.playerID == 1));
                          if (myPlayer5 != null)
                          {
                            bool flag11 = JSON_MyPhotonPlayerParam.Parse(myPlayer5.json).state == this.InstanceType;
                            switch (this.Index)
                            {
                              case 0:
                                ((Component) this).get_gameObject().SetActive(flag11);
                                return;
                              case 1:
                                Button component6 = (Button) ((Component) this).get_gameObject().GetComponent<Button>();
                                if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component6, (UnityEngine.Object) null))
                                {
                                  ((Selectable) component6).set_interactable(flag11);
                                  return;
                                }
                                ((Component) this).get_gameObject().SetActive(flag11);
                                return;
                              default:
                                ((Component) this).get_gameObject().SetActive(false);
                                return;
                            }
                          }
                          else
                          {
                            ((Component) this).get_gameObject().SetActive(false);
                            return;
                          }
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                          UnityEngine.UI.Text component22 = (UnityEngine.UI.Text) ((Component) this).GetComponent<UnityEngine.UI.Text>();
                          if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component22, (UnityEngine.Object) null))
                            return;
                          MyPhoton instance61 = PunMonoSingleton<MyPhoton>.Instance;
                          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance61, (UnityEngine.Object) null))
                          {
                            MyPhoton.MyRoom currentRoom3 = instance61.GetCurrentRoom();
                            if (currentRoom3 != null)
                            {
                              JSON_MyPhotonRoomParam myPhotonRoomParam3 = JSON_MyPhotonRoomParam.Parse(currentRoom3.json);
                              if (myPhotonRoomParam3 != null)
                              {
                                component22.set_text(myPhotonRoomParam3.challegedMTFloor.ToString() + "!");
                                return;
                              }
                            }
                          }
                          this.ResetToDefault();
                          return;
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                          QuestParam dataOfClass81 = DataSource.FindDataOfClass<QuestParam>(((Component) this).get_gameObject(), (QuestParam) null);
                          if (dataOfClass81 != null)
                          {
                            ((Component) this).get_gameObject().SetActive(dataOfClass81.IsMultiLeaderSkill);
                            return;
                          }
                          ((Component) this).get_gameObject().SetActive(false);
                          return;
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_COIN:
                          MultiPlayAPIRoom room8 = this.GetRoom();
                          if (room8.floor <= 0)
                          {
                            this.SetTextValue(string.Empty);
                            return;
                          }
                          this.SetTextValue(string.Format(LocalizedText.Get("sys.MULTI_TOWER_FLOOR_NAME"), (object) room8.floor));
                          return;
                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINATIME:
                          MultiPlayAPIRoom room9 = this.GetRoom();
                          Transform transform2 = ((Component) this).get_transform().Find("Clear");
                          Transform transform3 = ((Component) this).get_transform().Find("NotClear");
                          bool flag16 = room9.is_clear == 1;
                          if (UnityEngine.Object.op_Inequality((UnityEngine.Object) transform2, (UnityEngine.Object) null))
                            ((Component) transform2).get_gameObject().SetActive(flag16);
                          if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) transform3, (UnityEngine.Object) null))
                            return;
                          ((Component) transform3).get_gameObject().SetActive(!flag16);
                          return;
                        default:
                          switch (parameterType - 1200)
                          {
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                              QuestParam questParam32;
                              if ((questParam32 = this.GetQuestParam()) != null)
                              {
                                bool flag11 = questParam32.CheckDisableContinue();
                                if (this.Index == 0)
                                {
                                  ((Component) this).get_gameObject().SetActive(flag11);
                                  return;
                                }
                                ((Component) this).get_gameObject().SetActive(!flag11);
                                return;
                              }
                              ((Component) this).get_gameObject().SetActive(false);
                              return;
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                              QuestParam questParam33;
                              if ((questParam33 = this.GetQuestParam()) != null)
                              {
                                bool flag11 = questParam33.DamageUpprPl != 0 || questParam33.DamageUpprEn != 0;
                                if (this.Index == 0)
                                {
                                  ((Component) this).get_gameObject().SetActive(flag11);
                                  return;
                                }
                                ((Component) this).get_gameObject().SetActive(!flag11);
                                return;
                              }
                              ((Component) this).get_gameObject().SetActive(false);
                              return;
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                              QuestParam questParam34;
                              if ((questParam34 = this.GetQuestParam()) == null)
                                return;
                              string str10 = LocalizedText.Get("quest_info." + questParam34.iname);
                              string str11 = !(str10 == questParam34.iname) ? LocalizedText.Get("sys.PARTYEDITOR_COND_DETAIL") + str10 : string.Empty;
                              LocalizedText.UnloadTable("quest_info");
                              if (!string.IsNullOrEmpty(str11))
                                str11 += "\n";
                              List<string> addQuestInfo = questParam34.GetAddQuestInfo(true);
                              if (addQuestInfo.Count != 0)
                              {
                                string empty2 = string.Empty;
                                for (int index1 = 0; index1 < addQuestInfo.Count; ++index1)
                                {
                                  if (index1 > 0)
                                    empty2 += "\n";
                                  empty2 += addQuestInfo[index1];
                                }
                                str11 += empty2;
                              }
                              this.SetTextValue(str11);
                              return;
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                              QuestParam questParam35;
                              if ((questParam35 = this.GetQuestParam()) != null)
                              {
                                QuestCondParam questCondParam = (QuestCondParam) null;
                                if (questParam35.EntryCondition != null)
                                  questCondParam = questParam35.EntryCondition;
                                else if (questParam35.EntryConditionCh != null)
                                  questCondParam = questParam35.EntryConditionCh;
                                if (questCondParam != null && questCondParam.party_type == PartyCondType.Forced)
                                {
                                  bool flag11 = this.GetUnitData() != null;
                                  if (this.Index == 0)
                                  {
                                    ((Component) this).get_gameObject().SetActive(flag11);
                                    return;
                                  }
                                  ((Component) this).get_gameObject().SetActive(!flag11);
                                  return;
                                }
                              }
                              ((Component) this).get_gameObject().SetActive(false);
                              return;
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                              QuestParam questParam36;
                              if ((questParam36 = this.GetQuestParam()) != null)
                              {
                                QuestCondParam questCondParam = (QuestCondParam) null;
                                if (questParam36.EntryCondition != null)
                                  questCondParam = questParam36.EntryCondition;
                                else if (questParam36.EntryConditionCh != null)
                                  questCondParam = questParam36.EntryConditionCh;
                                if (questCondParam != null && questCondParam.party_type == PartyCondType.Limited)
                                {
                                  ((Component) this).get_gameObject().SetActive(true);
                                  return;
                                }
                              }
                              ((Component) this).get_gameObject().SetActive(false);
                              return;
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                              QuestParam questParam37;
                              if ((questParam37 = this.GetQuestParam()) != null)
                              {
                                if (questParam37.questParty != null)
                                {
                                  if (!((IEnumerable<PartySlotTypeUnitPair>) questParam37.questParty.GetMainSubSlots()).Any<PartySlotTypeUnitPair>((Func<PartySlotTypeUnitPair, bool>) (s => s.Type == PartySlotType.Free)))
                                  {
                                    ((Component) this).get_gameObject().SetActive(true);
                                    return;
                                  }
                                }
                                else
                                {
                                  QuestCondParam questCondParam = (QuestCondParam) null;
                                  if (questParam37.EntryCondition != null)
                                    questCondParam = questParam37.EntryCondition;
                                  else if (questParam37.EntryConditionCh != null)
                                    questCondParam = questParam37.EntryConditionCh;
                                  if (questCondParam != null && questCondParam.party_type == PartyCondType.Forced)
                                  {
                                    ((Component) this).get_gameObject().SetActive(true);
                                    return;
                                  }
                                }
                              }
                              ((Component) this).get_gameObject().SetActive(false);
                              return;
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                              QuestParam questParam38;
                              if ((questParam38 = this.GetQuestParam()) == null)
                                return;
                              Selectable component23 = (Selectable) ((Component) this).get_gameObject().GetComponent<Selectable>();
                              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component23, (UnityEngine.Object) null))
                                return;
                              bool flag17 = questParam38.state == QuestStates.Cleared;
                              if (this.Index == 0)
                              {
                                component23.set_interactable(flag17);
                                return;
                              }
                              component23.set_interactable(!flag17);
                              return;
                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_COIN:
                              QuestParam questParam39;
                              if ((questParam39 = this.GetQuestParam()) != null)
                              {
                                bool isUnitChange = questParam39.IsUnitChange;
                                if (this.Index == 0)
                                {
                                  ((Component) this).get_gameObject().SetActive(isUnitChange);
                                  return;
                                }
                                ((Component) this).get_gameObject().SetActive(!isUnitChange);
                                return;
                              }
                              ((Component) this).get_gameObject().SetActive(false);
                              return;
                            default:
                              switch (parameterType - 1400)
                              {
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                  BattleCore.OrderData orderData = this.GetOrderData();
                                  if (orderData == null)
                                    return;
                                  Image component24 = (Image) ((Component) this).get_gameObject().GetComponent<Image>();
                                  if (!UnityEngine.Object.op_Implicit((UnityEngine.Object) component24))
                                    return;
                                  ((Behaviour) component24).set_enabled(orderData.IsCastSkill);
                                  return;
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                  skillParam = (SkillParam) null;
                                  SkillData skillData4;
                                  skillParam = (skillData4 = this.GetSkillData()) == null ? this.GetSkillParam() : skillData4.SkillParam;
                                  if (skillParam != null)
                                  {
                                    this.SetTextValue((int) skillParam.count);
                                    return;
                                  }
                                  this.ResetToDefault();
                                  return;
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                  skillParam = (SkillParam) null;
                                  SkillData skillData5;
                                  skillParam = (skillData5 = this.GetSkillData()) == null ? this.GetSkillParam() : skillData5.SkillParam;
                                  int index4 = 0;
                                  if (skillParam != null)
                                    index4 = (int) skillParam.element_type;
                                  if (index4 != 0)
                                  {
                                    this.SetImageIndex(index4);
                                    ((Component) this).get_gameObject().SetActive(true);
                                    return;
                                  }
                                  this.ResetToDefault();
                                  ((Component) this).get_gameObject().SetActive(false);
                                  return;
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                                  skillParam = (SkillParam) null;
                                  SkillData skillData6;
                                  skillParam = (skillData6 = this.GetSkillData()) == null ? this.GetSkillParam() : skillData6.SkillParam;
                                  int index5 = 0;
                                  if (skillParam != null)
                                    index5 = (int) skillParam.attack_detail;
                                  if (index5 != 0)
                                  {
                                    this.SetImageIndex(index5);
                                    ((Component) this).get_gameObject().SetActive(true);
                                    return;
                                  }
                                  this.ResetToDefault();
                                  ((Component) this).get_gameObject().SetActive(false);
                                  return;
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                                  skillParam = (SkillParam) null;
                                  SkillData skillData7;
                                  skillParam = (skillData7 = this.GetSkillData()) == null ? this.GetSkillParam() : skillData7.SkillParam;
                                  int index6 = 0;
                                  if (skillParam != null)
                                    index6 = (int) skillParam.attack_type;
                                  if (index6 != 0)
                                  {
                                    this.SetImageIndex(index6);
                                    ((Component) this).get_gameObject().SetActive(true);
                                    return;
                                  }
                                  this.ResetToDefault();
                                  ((Component) this).get_gameObject().SetActive(false);
                                  return;
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                                  ((Component) this).get_gameObject().SetActive(WeatherData.CurrentWeatherData != null);
                                  return;
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                                  WeatherParam weatherParam1 = this.GetWeatherParam();
                                  if (weatherParam1 != null)
                                  {
                                    this.SetTextValue(weatherParam1.Name);
                                    return;
                                  }
                                  this.ResetToDefault();
                                  return;
                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_COIN:
                                  WeatherParam weatherParam2 = this.GetWeatherParam();
                                  if (weatherParam2 != null && !string.IsNullOrEmpty(weatherParam2.Icon))
                                  {
                                    GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = AssetPath.WeatherIcon(weatherParam2.Icon);
                                    return;
                                  }
                                  this.ResetToDefault();
                                  return;
                                default:
                                  switch (parameterType - 1800)
                                  {
                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                      skillParam = (SkillParam) null;
                                      SkillData skillData8;
                                      skillParam = (skillData8 = this.GetSkillData()) == null ? this.GetSkillParam() : skillData8.SkillParam;
                                      if (skillParam != null)
                                      {
                                        this.SetTextValue(skillParam.MapEffectDesc);
                                        return;
                                      }
                                      this.ResetToDefault();
                                      return;
                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                      MapEffectParam mapEffectParam = this.GetMapEffectParam();
                                      if (mapEffectParam != null)
                                      {
                                        this.SetTextValue(mapEffectParam.Name);
                                        return;
                                      }
                                      this.ResetToDefault();
                                      return;
                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                      JobParam jobParam4 = this.GetJobParam();
                                      if (jobParam4 != null)
                                      {
                                        this.SetTextValue(jobParam4.DescCharacteristic);
                                        return;
                                      }
                                      this.ResetToDefault();
                                      return;
                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                                      JobParam jobParam5 = this.GetJobParam();
                                      if (jobParam5 != null)
                                      {
                                        ((Component) this).get_gameObject().SetActive(!string.IsNullOrEmpty(jobParam5.DescCharacteristic));
                                        return;
                                      }
                                      ((Component) this).get_gameObject().SetActive(false);
                                      return;
                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                                      JobParam jobParam6 = this.GetJobParam();
                                      if (jobParam6 != null)
                                      {
                                        this.SetTextValue(jobParam6.DescOther);
                                        return;
                                      }
                                      this.ResetToDefault();
                                      return;
                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                                      JobParam jobParam7 = this.GetJobParam();
                                      if (jobParam7 != null)
                                      {
                                        ((Component) this).get_gameObject().SetActive(!string.IsNullOrEmpty(jobParam7.DescOther));
                                        return;
                                      }
                                      ((Component) this).get_gameObject().SetActive(false);
                                      return;
                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                                      goto label_326;
                                    default:
                                      switch (parameterType - 2100)
                                      {
                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                          UnitParam unitParam12;
                                          if ((unitParam12 = this.GetUnitParam()) == null)
                                            return;
                                          UnitUnlockTimeParam unitUnlockTimeParam = MonoSingleton<GameManager>.Instance.MasterParam.GetUnitUnlockTimeParam(unitParam12.unlock_time);
                                          if (unitUnlockTimeParam == null)
                                            return;
                                          string format4 = "yyyy/MM/dd HH:mm";
                                          this.SetTextValue(unitUnlockTimeParam.end_at.ToString(format4));
                                          return;
                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                          JSON_MyPhotonPlayerParam versusPlayerParam2 = this.GetVersusPlayerParam();
                                          if (versusPlayerParam2 != null)
                                          {
                                            this.SetTextValue(versusPlayerParam2.totalStatus.ToString());
                                            return;
                                          }
                                          this.ResetToDefault();
                                          return;
                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                          MyPhoton instance62 = PunMonoSingleton<MyPhoton>.Instance;
                                          UnitData unitData59 = (UnitData) null;
                                          if (MonoSingleton<GameManager>.Instance.AudienceMode)
                                          {
                                            JSON_MyPhotonPlayerParam player4 = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam().players[this.InstanceType];
                                            if (player4 != null)
                                            {
                                              player4.SetupUnits();
                                              if (player4.units.Length > 0)
                                                unitData59 = player4.units[0].unit;
                                            }
                                          }
                                          else
                                          {
                                            MyPhoton.MyPlayer player = instance62.GetMyPlayer();
                                            if (this.InstanceType == 0)
                                            {
                                              JSON_MyPhotonPlayerParam photonPlayerParam5 = JSON_MyPhotonPlayerParam.Parse(player.json);
                                              if (photonPlayerParam5.units != null && photonPlayerParam5.units.Length > 0 && photonPlayerParam5.units[0].unit != null)
                                                unitData59 = photonPlayerParam5.units[0].unit;
                                            }
                                            else
                                            {
                                              JSON_MyPhotonPlayerParam photonPlayerParam5 = instance62.GetMyPlayersStarted().Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerID != player.playerID));
                                              if (photonPlayerParam5.units != null && photonPlayerParam5.units.Length > 0 && photonPlayerParam5.units[0].unit != null)
                                                unitData59 = photonPlayerParam5.units[0].unit;
                                            }
                                          }
                                          if (unitData59 != null && !string.IsNullOrEmpty(unitData59.UnitParam.image))
                                          {
                                            IconLoader iconLoader = GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject());
                                            string str3 = AssetPath.UnitSkinImage(unitData59.UnitParam, unitData59.GetSelectedSkin(-1), unitData59.CurrentJob.JobID);
                                            if (!string.IsNullOrEmpty(str3))
                                            {
                                              iconLoader.ResourcePath = str3;
                                              return;
                                            }
                                          }
                                          this.ResetToDefault();
                                          return;
                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                                          UnitData data = (UnitData) null;
                                          if (MonoSingleton<GameManager>.Instance.AudienceMode)
                                          {
                                            AudienceStartParam startedParam = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam();
                                            int index1;
                                            int index2;
                                            if (this.InstanceType < 5)
                                            {
                                              index1 = 0;
                                              index2 = this.InstanceType;
                                            }
                                            else
                                            {
                                              index1 = 1;
                                              index2 = this.InstanceType - 5;
                                            }
                                            JSON_MyPhotonPlayerParam player4 = startedParam.players[index1];
                                            if (player4 != null)
                                            {
                                              player4.SetupUnits();
                                              if (player4.units.Length > index2)
                                                data = player4.units[index2].unit;
                                            }
                                          }
                                          else
                                          {
                                            MyPhoton instance5 = PunMonoSingleton<MyPhoton>.Instance;
                                            MyPhoton.MyPlayer player = instance5.GetMyPlayer();
                                            JSON_MyPhotonPlayerParam.UnitDataElem[] unitDataElemArray = (JSON_MyPhotonPlayerParam.UnitDataElem[]) null;
                                            int index1;
                                            if (this.InstanceType < 5)
                                            {
                                              index1 = this.InstanceType;
                                              JSON_MyPhotonPlayerParam photonPlayerParam5 = JSON_MyPhotonPlayerParam.Parse(player.json);
                                              if (photonPlayerParam5.units != null && photonPlayerParam5.units.Length > 0)
                                                unitDataElemArray = photonPlayerParam5.units;
                                            }
                                            else
                                            {
                                              index1 = this.InstanceType - 5;
                                              JSON_MyPhotonPlayerParam photonPlayerParam5 = instance5.GetMyPlayersStarted().Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerID != player.playerID));
                                              if (photonPlayerParam5.units != null && photonPlayerParam5.units.Length > 0)
                                                unitDataElemArray = photonPlayerParam5.units;
                                            }
                                            if (unitDataElemArray != null && index1 < unitDataElemArray.Length && unitDataElemArray[index1].unit != null)
                                              data = unitDataElemArray[index1].unit;
                                          }
                                          if (this.ParameterType == GameParameter.ParameterTypes.VERSUS_DRAFT_PARTY_IMAGE_ICON)
                                          {
                                            if (data != null)
                                            {
                                              DataSource.Bind<UnitData>(((Component) this).get_gameObject(), data);
                                              UnitIcon component6 = (UnitIcon) ((Component) this).get_gameObject().GetComponent<UnitIcon>();
                                              if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component6, (UnityEngine.Object) null))
                                                return;
                                              component6.UpdateValue();
                                              return;
                                            }
                                            ((Component) this).get_gameObject().SetActive(false);
                                            return;
                                          }
                                          if (this.ParameterType != GameParameter.ParameterTypes.VERSUS_DRAFT_PARTY_IMAGE)
                                            return;
                                          if (data != null && !string.IsNullOrEmpty(data.UnitParam.image))
                                          {
                                            IconLoader iconLoader = GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject());
                                            string str3 = AssetPath.UnitSkinImage(data.UnitParam, data.GetSelectedSkin(-1), data.CurrentJob.JobID);
                                            if (!string.IsNullOrEmpty(str3))
                                            {
                                              iconLoader.ResourcePath = str3;
                                              return;
                                            }
                                          }
                                          this.ResetToDefault();
                                          return;
                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                                          if (this.InstanceType == 0)
                                          {
                                            ((Component) this).get_gameObject().SetActive(VersusDraftList.VersusDraftTurnOwn);
                                            return;
                                          }
                                          ((Component) this).get_gameObject().SetActive(!VersusDraftList.VersusDraftTurnOwn);
                                          return;
                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                                          int num20 = 0;
                                          if (MonoSingleton<GameManager>.Instance.AudienceMode)
                                          {
                                            JSON_MyPhotonPlayerParam player4 = MonoSingleton<GameManager>.Instance.AudienceManager.GetStartedParam().players[this.InstanceType != 0 ? 1 : 0];
                                            if (player4 != null)
                                            {
                                              player4.SetupUnits();
                                              for (int index1 = 0; index1 < player4.units.Length; ++index1)
                                                num20 = num20 + (int) player4.units[index1].unit.Status.param.atk + (int) player4.units[index1].unit.Status.param.mag;
                                            }
                                          }
                                          else
                                          {
                                            MyPhoton instance5 = PunMonoSingleton<MyPhoton>.Instance;
                                            MyPhoton.MyPlayer player = instance5.GetMyPlayer();
                                            JSON_MyPhotonPlayerParam.UnitDataElem[] unitDataElemArray = (JSON_MyPhotonPlayerParam.UnitDataElem[]) null;
                                            if (this.InstanceType == 0)
                                            {
                                              JSON_MyPhotonPlayerParam photonPlayerParam5 = JSON_MyPhotonPlayerParam.Parse(player.json);
                                              if (photonPlayerParam5.units != null && photonPlayerParam5.units.Length > 0)
                                                unitDataElemArray = photonPlayerParam5.units;
                                            }
                                            else
                                            {
                                              JSON_MyPhotonPlayerParam photonPlayerParam5 = instance5.GetMyPlayersStarted().Find((Predicate<JSON_MyPhotonPlayerParam>) (p => p.playerID != player.playerID));
                                              if (photonPlayerParam5.units != null && photonPlayerParam5.units.Length > 0)
                                                unitDataElemArray = photonPlayerParam5.units;
                                            }
                                            if (unitDataElemArray != null)
                                            {
                                              for (int index1 = 0; index1 < unitDataElemArray.Length; ++index1)
                                                num20 = num20 + (int) unitDataElemArray[index1].unit.Status.param.atk + (int) unitDataElemArray[index1].unit.Status.param.mag;
                                            }
                                          }
                                          this.SetTextValue(num20);
                                          return;
                                        default:
                                          switch (parameterType - 2200)
                                          {
                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                              TobiraRecipeParam dataOfClass82 = DataSource.FindDataOfClass<TobiraRecipeParam>(((Component) this).get_gameObject(), (TobiraRecipeParam) null);
                                              if (dataOfClass82 == null)
                                                return;
                                              this.SetTextValue(dataOfClass82.Cost);
                                              return;
                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                              UnlockTobiraRecipe dataOfClass83 = DataSource.FindDataOfClass<UnlockTobiraRecipe>(((Component) this).get_gameObject(), (UnlockTobiraRecipe) null);
                                              if (dataOfClass83 == null)
                                                return;
                                              this.SetTextValue(dataOfClass83.RequiredAmount);
                                              return;
                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                              UnlockTobiraRecipe dataOfClass84 = DataSource.FindDataOfClass<UnlockTobiraRecipe>(((Component) this).get_gameObject(), (UnlockTobiraRecipe) null);
                                              if (dataOfClass84 == null)
                                                return;
                                              this.SetTextValue(dataOfClass84.Amount);
                                              return;
                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                                              TobiraEnhanceRecipe dataOfClass85 = DataSource.FindDataOfClass<TobiraEnhanceRecipe>(((Component) this).get_gameObject(), (TobiraEnhanceRecipe) null);
                                              if (dataOfClass85 == null)
                                                return;
                                              this.SetTextValue(dataOfClass85.RequiredAmount);
                                              return;
                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                                              TobiraEnhanceRecipe dataOfClass86 = DataSource.FindDataOfClass<TobiraEnhanceRecipe>(((Component) this).get_gameObject(), (TobiraEnhanceRecipe) null);
                                              if (dataOfClass86 == null)
                                                return;
                                              this.SetTextValue(dataOfClass86.Amount);
                                              return;
                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_GOLD:
                                              TobiraData dataOfClass87 = DataSource.FindDataOfClass<TobiraData>(((Component) this).get_gameObject(), (TobiraData) null);
                                              if (dataOfClass87 != null)
                                              {
                                                if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null))
                                                {
                                                  bool flag11 = dataOfClass87.ViewLv >= this.Index + 1;
                                                  this.SetImageIndex(!flag11 ? 0 : 1);
                                                  if (this.ParameterType != GameParameter.ParameterTypes.UNIT_TOBIRA_LEVEL_ICON)
                                                    return;
                                                  ((Component) this).get_gameObject().SetActive(flag11);
                                                  return;
                                                }
                                                this.SetTextValue(dataOfClass87.ViewLv);
                                                return;
                                              }
                                              this.ResetToDefault();
                                              return;
                                            default:
                                              switch (parameterType - 1100)
                                              {
                                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                                  ArtifactData artifactData1 = this.GetArtifactData();
                                                  if (artifactData1 != null)
                                                  {
                                                    ArtifactData.RarityUpResults rarityUpResults = artifactData1.CheckEnableRarityUp();
                                                    string key = (string) null;
                                                    if ((rarityUpResults & ArtifactData.RarityUpResults.RarityMaxed) != ArtifactData.RarityUpResults.Success)
                                                      key = "sys.ARTI_RARITYUP_MAX";
                                                    else if ((rarityUpResults & ArtifactData.RarityUpResults.NoLv) != ArtifactData.RarityUpResults.Success)
                                                      key = "sys.ARTI_RARITYUP_NOLV";
                                                    else if ((rarityUpResults & ArtifactData.RarityUpResults.NoGold) != ArtifactData.RarityUpResults.Success)
                                                      key = "sys.ARTI_RARITYUP_NOGOLD";
                                                    else if ((rarityUpResults & ArtifactData.RarityUpResults.NoKakera) != ArtifactData.RarityUpResults.Success)
                                                      key = "sys.ARTI_RARITYUP_NOMTRL";
                                                    if (!string.IsNullOrEmpty(key))
                                                    {
                                                      ((Component) this).get_gameObject().SetActive(true);
                                                      this.SetTextValue(LocalizedText.Get(key));
                                                      return;
                                                    }
                                                  }
                                                  ((Component) this).get_gameObject().SetActive(false);
                                                  return;
                                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                                  bool flag18 = false;
                                                  ArtifactData artifactData2 = this.GetArtifactData();
                                                  GameManager instanceDirect = MonoSingleton<GameManager>.GetInstanceDirect();
                                                  if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instanceDirect, (UnityEngine.Object) null) && artifactData2 != null)
                                                  {
                                                    ArtifactData artifactByUniqueId = instanceDirect.Player.FindArtifactByUniqueID((long) artifactData2.UniqueID);
                                                    if (artifactByUniqueId != null)
                                                      flag18 = artifactByUniqueId.IsFavorite;
                                                  }
                                                  if (this.Index == 1)
                                                    flag18 = !flag18;
                                                  ((Component) this).get_gameObject().SetActive(flag18);
                                                  return;
                                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                                  ArtifactData artifactData3 = this.GetArtifactData();
                                                  if (artifactData3 != null)
                                                  {
                                                    bool flag11 = artifactData3.CheckEnableRarityUp() == ArtifactData.RarityUpResults.Success;
                                                    if (flag11)
                                                      this.SetImageIndex((int) artifactData3.Rarity + 1);
                                                    ((Component) this).get_gameObject().SetActive(flag11);
                                                    return;
                                                  }
                                                  ((Component) this).get_gameObject().SetActive(false);
                                                  return;
                                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINAMAX:
                                                  switch (this.InstanceType)
                                                  {
                                                    case 0:
                                                    case 1:
                                                      ArtifactParam artifactParam5 = this.GetArtifactParam();
                                                      if (artifactParam5 != null && this.LoadArtifactIcon(artifactParam5))
                                                        return;
                                                      this.ResetToDefault();
                                                      return;
                                                    case 2:
                                                      ArtifactRewardData dataOfClass88 = DataSource.FindDataOfClass<ArtifactRewardData>(((Component) this).get_gameObject(), (ArtifactRewardData) null);
                                                      if (dataOfClass88 == null)
                                                        return;
                                                      ArtifactParam artifactParam6 = dataOfClass88.ArtifactParam;
                                                      if (artifactParam6 != null && this.LoadArtifactIcon(artifactParam6))
                                                        return;
                                                      this.ResetToDefault();
                                                      return;
                                                    default:
                                                      return;
                                                  }
                                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXP:
                                                  switch (this.InstanceType)
                                                  {
                                                    case 0:
                                                    case 1:
                                                      this.SetArtifactFrame(this.GetArtifactParam());
                                                      return;
                                                    case 2:
                                                      ArtifactRewardData dataOfClass89 = DataSource.FindDataOfClass<ArtifactRewardData>(((Component) this).get_gameObject(), (ArtifactRewardData) null);
                                                      if (dataOfClass89 == null)
                                                        return;
                                                      ArtifactParam artifactParam7 = dataOfClass89.ArtifactParam;
                                                      if (artifactParam7 == null)
                                                        return;
                                                      this.SetArtifactFrame(artifactParam7);
                                                      return;
                                                    default:
                                                      return;
                                                  }
                                                case GameParameter.ParameterTypes.GLOBAL_PLAYER_EXPNEXT:
                                                  switch (this.InstanceType)
                                                  {
                                                    case 1:
                                                      QuestParam questParamAuto9 = this.GetQuestParamAuto();
                                                      if (questParamAuto9 != null && 0 <= this.Index && (questParamAuto9.bonusObjective != null && this.Index < questParamAuto9.bonusObjective.Length))
                                                      {
                                                        this.SetTextValue(questParamAuto9.bonusObjective[this.Index].itemNum);
                                                        return;
                                                      }
                                                      break;
                                                    case 2:
                                                      ArtifactRewardData dataOfClass90 = DataSource.FindDataOfClass<ArtifactRewardData>(((Component) this).get_gameObject(), (ArtifactRewardData) null);
                                                      if (dataOfClass90 == null)
                                                        return;
                                                      if (dataOfClass90.ArtifactParam != null)
                                                      {
                                                        this.SetTextValue(dataOfClass90.Num);
                                                        return;
                                                      }
                                                      break;
                                                  }
                                                  this.ResetToDefault();
                                                  return;
                                                default:
                                                  switch (parameterType - 1300)
                                                  {
                                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                                      TrickParam trickParam1 = this.GetTrickParam();
                                                      if (trickParam1 != null)
                                                      {
                                                        this.SetTextValue(trickParam1.Name);
                                                        return;
                                                      }
                                                      this.ResetToDefault();
                                                      return;
                                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                                      TrickParam trickParam2 = this.GetTrickParam();
                                                      if (trickParam2 != null)
                                                      {
                                                        this.SetTextValue(trickParam2.Expr);
                                                        return;
                                                      }
                                                      this.ResetToDefault();
                                                      return;
                                                    case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                                      TrickParam trickParam3 = this.GetTrickParam();
                                                      if (trickParam3 != null)
                                                      {
                                                        GameUtility.RequireComponent<IconLoader>(((Component) this).get_gameObject()).ResourcePath = AssetPath.TrickIconUI(trickParam3.MarkerName);
                                                        return;
                                                      }
                                                      this.ResetToDefault();
                                                      return;
                                                    default:
                                                      switch (parameterType - 2000)
                                                      {
                                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                                          Image component25 = (Image) ((Component) this).GetComponent<Image>();
                                                          if (UnityEngine.Object.op_Equality((UnityEngine.Object) component25, (UnityEngine.Object) null))
                                                          {
                                                            ((Component) this).get_gameObject().SetActive(false);
                                                            return;
                                                          }
                                                          ChallengeCategoryParam dataOfClass91 = DataSource.FindDataOfClass<ChallengeCategoryParam>(((Component) this).get_gameObject(), (ChallengeCategoryParam) null);
                                                          if (dataOfClass91 != null && !string.IsNullOrEmpty(dataOfClass91.iname))
                                                          {
                                                            SpriteSheet spriteSheet2 = AssetManager.Load<SpriteSheet>("ChallengeMission/ChallengeMission_Images");
                                                            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet2, (UnityEngine.Object) null))
                                                            {
                                                              component25.set_sprite(spriteSheet2.GetSprite("help/" + dataOfClass91.iname));
                                                              return;
                                                            }
                                                          }
                                                          component25.set_sprite((Sprite) null);
                                                          return;
                                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                                          Image component26 = (Image) ((Component) this).GetComponent<Image>();
                                                          if (UnityEngine.Object.op_Equality((UnityEngine.Object) component26, (UnityEngine.Object) null))
                                                          {
                                                            ((Component) this).get_gameObject().SetActive(false);
                                                            return;
                                                          }
                                                          ChallengeCategoryParam dataOfClass92 = DataSource.FindDataOfClass<ChallengeCategoryParam>(((Component) this).get_gameObject(), (ChallengeCategoryParam) null);
                                                          if (dataOfClass92 != null && !string.IsNullOrEmpty(dataOfClass92.iname))
                                                          {
                                                            SpriteSheet spriteSheet2 = AssetManager.Load<SpriteSheet>("ChallengeMission/ChallengeMission_Images");
                                                            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet2, (UnityEngine.Object) null))
                                                            {
                                                              component26.set_sprite(spriteSheet2.GetSprite("button/" + dataOfClass92.iname));
                                                              return;
                                                            }
                                                          }
                                                          component26.set_sprite((Sprite) null);
                                                          return;
                                                        case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                                          Image component27 = (Image) ((Component) this).GetComponent<Image>();
                                                          if (UnityEngine.Object.op_Equality((UnityEngine.Object) component27, (UnityEngine.Object) null))
                                                          {
                                                            ((Component) this).get_gameObject().SetActive(false);
                                                            return;
                                                          }
                                                          TrophyParam dataOfClass93 = DataSource.FindDataOfClass<TrophyParam>(((Component) this).get_gameObject(), (TrophyParam) null);
                                                          if (dataOfClass93 != null && !string.IsNullOrEmpty(dataOfClass93.iname))
                                                          {
                                                            SpriteSheet spriteSheet2 = AssetManager.Load<SpriteSheet>("ChallengeMission/ChallengeMission_Images");
                                                            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) spriteSheet2, (UnityEngine.Object) null))
                                                            {
                                                              component27.set_sprite(spriteSheet2.GetSprite("reward/" + dataOfClass93.iname));
                                                              return;
                                                            }
                                                          }
                                                          component27.set_sprite((Sprite) null);
                                                          return;
                                                        default:
                                                          switch (parameterType - 2500)
                                                          {
                                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_NAME:
                                                              bool flag19 = false;
                                                              AbilityData abilityData4;
                                                              if ((abilityData4 = this.GetAbilityData()) != null)
                                                              {
                                                                flag19 = abilityData4.IsDerivedAbility;
                                                              }
                                                              else
                                                              {
                                                                AbilityParam abilityParam6;
                                                                if ((abilityParam6 = this.GetAbilityParam()) != null)
                                                                {
                                                                  AbilityDeriveParam dataOfClass25 = DataSource.FindDataOfClass<AbilityDeriveParam>(((Component) this).get_gameObject(), (AbilityDeriveParam) null);
                                                                  if (dataOfClass25 != null && dataOfClass25.m_DeriveParam == abilityParam6)
                                                                    flag19 = true;
                                                                }
                                                              }
                                                              ((Component) this).get_gameObject().SetActive(flag19);
                                                              return;
                                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_LEVEL:
                                                              Color color3;
                                                              // ISSUE: explicit reference operation
                                                              ((Color) @color3).\u002Ector(1f, 1f, 0.0f, 1f);
                                                              Color color4 = Color.get_white();
                                                              AbilityData abilityData5;
                                                              if ((abilityData5 = this.GetAbilityData()) != null)
                                                              {
                                                                if (abilityData5.IsDerivedAbility)
                                                                  color4 = color3;
                                                              }
                                                              else
                                                              {
                                                                AbilityParam abilityParam6;
                                                                if ((abilityParam6 = this.GetAbilityParam()) != null)
                                                                {
                                                                  AbilityDeriveParam dataOfClass25 = DataSource.FindDataOfClass<AbilityDeriveParam>(((Component) this).get_gameObject(), (AbilityDeriveParam) null);
                                                                  if (dataOfClass25 != null && dataOfClass25.m_DeriveParam == abilityParam6)
                                                                    color4 = color3;
                                                                }
                                                              }
                                                              this.SetTextColor(color4);
                                                              this.SetSyncColorOriginColor(color4);
                                                              return;
                                                            case GameParameter.ParameterTypes.GLOBAL_PLAYER_STAMINA:
                                                              string key1 = string.Empty;
                                                              ImageSpriteSheet component28 = (ImageSpriteSheet) ((Component) this).GetComponent<ImageSpriteSheet>();
                                                              if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component28, (UnityEngine.Object) null))
                                                                key1 = component28.DefaultKey;
                                                              AbilityData abilityData6;
                                                              if ((abilityData6 = this.GetAbilityData()) != null)
                                                              {
                                                                key1 = AbilityParam.TypeDetailToSpriteSheetKey(abilityData6.Param.type_detail);
                                                              }
                                                              else
                                                              {
                                                                AbilityParam abilityParam6;
                                                                if ((abilityParam6 = this.GetAbilityParam()) != null)
                                                                  key1 = AbilityParam.TypeDetailToSpriteSheetKey(abilityParam6.type_detail);
                                                              }
                                                              this.SetImageBySpriteSheet(key1);
                                                              return;
                                                            default:
                                                              if (parameterType != GameParameter.ParameterTypes.UNIT_EXTRA_PARAM_MOVE)
                                                              {
                                                                if (parameterType != GameParameter.ParameterTypes.UNIT_EXTRA_PARAM_JUMP)
                                                                {
                                                                  if (parameterType != GameParameter.ParameterTypes.FIRST_FRIEND_MAX)
                                                                  {
                                                                    if (parameterType != GameParameter.ParameterTypes.FIRST_FRIEND_COUNT)
                                                                    {
                                                                      if (parameterType != GameParameter.ParameterTypes.ITEM_RARITY)
                                                                      {
                                                                        if (parameterType != GameParameter.ParameterTypes.ITEM_FLAVORORDESC)
                                                                        {
                                                                          if (parameterType != GameParameter.ParameterTypes.GAMEOBJECT_ACTIVE)
                                                                          {
                                                                            if (parameterType != GameParameter.ParameterTypes.GAMEOBJECT_INACTIVE)
                                                                            {
                                                                              if (parameterType != GameParameter.ParameterTypes.SKILL_DERIVED_TEXTCOLOR)
                                                                              {
                                                                                if (parameterType != GameParameter.ParameterTypes.ARTIFACT_SETEFFECT_TRIGGER_ICON)
                                                                                {
                                                                                  if (parameterType != GameParameter.ParameterTypes.CONCEPTCARD_AWAKE)
                                                                                    return;
                                                                                  this.mImageArray.set_sprite((Sprite) null);
                                                                                  ConceptCardData dataOfClass25 = DataSource.FindDataOfClass<ConceptCardData>(((Component) this).get_gameObject(), (ConceptCardData) null);
                                                                                  if (dataOfClass25 == null || !UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null) || (int) dataOfClass25.AwakeCount <= 0)
                                                                                    return;
                                                                                  int index1 = (int) dataOfClass25.AwakeCount - 1;
                                                                                  if ((int) dataOfClass25.Lv >= (int) dataOfClass25.LvCap)
                                                                                    index1 = this.mImageArray.Images.Length - 1;
                                                                                  this.SetImageIndex(index1);
                                                                                  ((Component) this).get_gameObject().SetActive(true);
                                                                                  return;
                                                                                }
                                                                                SkillAbilityDeriveParam dataOfClass94 = DataSource.FindDataOfClass<SkillAbilityDeriveParam>(((Component) this).get_gameObject(), (SkillAbilityDeriveParam) null);
                                                                                if (dataOfClass94 == null)
                                                                                  return;
                                                                                string triggerArtifactIname = dataOfClass94.GetTriggerArtifactIname(this.Index);
                                                                                if (string.IsNullOrEmpty(triggerArtifactIname))
                                                                                  return;
                                                                                this.LoadArtifactIcon(MonoSingleton<GameManager>.Instance.MasterParam.GetArtifactParam(triggerArtifactIname));
                                                                                return;
                                                                              }
                                                                              Color color2;
                                                                              // ISSUE: explicit reference operation
                                                                              ((Color) @color2).\u002Ector(1f, 1f, 0.0f, 1f);
                                                                              Color color5 = Color.get_white();
                                                                              SkillData skillData9;
                                                                              if ((skillData9 = this.GetSkillData()) != null)
                                                                              {
                                                                                if (skillData9.IsDerivedSkill)
                                                                                  color5 = color2;
                                                                              }
                                                                              else if ((skillParam = this.GetSkillParam()) != null)
                                                                              {
                                                                                SkillDeriveParam dataOfClass25 = DataSource.FindDataOfClass<SkillDeriveParam>(((Component) this).get_gameObject(), (SkillDeriveParam) null);
                                                                                if (dataOfClass25 != null && dataOfClass25.m_DeriveParam == skillParam)
                                                                                  color5 = color2;
                                                                              }
                                                                              this.SetTextColor(color5);
                                                                              this.SetSyncColorOriginColor(color5);
                                                                              return;
                                                                            }
                                                                            ((Component) this).get_gameObject().SetActive(!DataSource.FindDataOfClass<bool>(((Component) this).get_gameObject(), false));
                                                                            return;
                                                                          }
                                                                          ((Component) this).get_gameObject().SetActive(DataSource.FindDataOfClass<bool>(((Component) this).get_gameObject(), true));
                                                                          return;
                                                                        }
                                                                        ItemParam itemParam6;
                                                                        if ((itemParam6 = this.GetItemParam()) != null)
                                                                        {
                                                                          if (!string.IsNullOrEmpty(itemParam6.Flavor))
                                                                          {
                                                                            this.SetTextValue(itemParam6.Flavor);
                                                                            return;
                                                                          }
                                                                          this.SetTextValue(itemParam6.Expr);
                                                                          return;
                                                                        }
                                                                        this.ResetToDefault();
                                                                        return;
                                                                      }
                                                                      ItemParam itemParam20 = this.GetItemParam();
                                                                      Image component6 = (Image) ((Component) this).GetComponent<Image>();
                                                                      if (itemParam20 != null && UnityEngine.Object.op_Inequality((UnityEngine.Object) component6, (UnityEngine.Object) null))
                                                                      {
                                                                        GameSettings instance5 = GameSettings.Instance;
                                                                        int rare = itemParam20.rare;
                                                                        if (UnityEngine.Object.op_Inequality((UnityEngine.Object) instance5, (UnityEngine.Object) null) && rare >= 0 && rare < instance5.ArtifactIcon_Rarity.Length)
                                                                        {
                                                                          component6.set_sprite(instance5.ArtifactIcon_Rarity[rare]);
                                                                          return;
                                                                        }
                                                                      }
                                                                      this.ResetToDefault();
                                                                      return;
                                                                    }
                                                                    PlayerData player4 = MonoSingleton<GameManager>.Instance.Player;
                                                                    if (player4 == null)
                                                                      return;
                                                                    this.SetTextValue(player4.FirstFriendCount);
                                                                    return;
                                                                  }
                                                                  FixParam fixParam4 = MonoSingleton<GameManager>.Instance.MasterParam.FixParam;
                                                                  if (fixParam4 == null)
                                                                    return;
                                                                  this.SetTextValue((int) fixParam4.FirstFriendMax);
                                                                  return;
                                                                }
                                                                UnitData unitData15;
                                                                if ((unitData15 = this.GetUnitData()) != null)
                                                                {
                                                                  this.SetTextValue((int) unitData15.Status.param.jmp);
                                                                  return;
                                                                }
                                                                this.ResetToDefault();
                                                                return;
                                                              }
                                                              UnitData unitData60;
                                                              if ((unitData60 = this.GetUnitData()) != null)
                                                              {
                                                                this.SetTextValue((int) unitData60.Status.param.mov);
                                                                return;
                                                              }
                                                              this.ResetToDefault();
                                                              return;
                                                          }
                                                      }
                                                  }
                                              }
                                          }
                                      }
                                  }
                              }
                          }
                      }
                  }
              }
          }
      }
    }

    public static void UpdateValuesOfType(GameParameter.ParameterTypes type)
    {
      for (int index = 0; index < GameParameter.Instances.Count; ++index)
      {
        if (GameParameter.Instances[index].ParameterType == type)
          GameParameter.Instances[index].UpdateValue();
      }
    }

    public static void UpdateAll(GameObject root)
    {
      Component[] componentsInChildren = root.GetComponentsInChildren(typeof (IGameParameter), true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (componentsInChildren[index] is GameParameter)
        {
          GameParameter gameParameter = componentsInChildren[index] as GameParameter;
          if (((Component) gameParameter).get_gameObject().get_activeInHierarchy())
          {
            gameParameter.UpdateValue();
          }
          else
          {
            Transform transform = ((Component) gameParameter).get_transform();
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) transform.get_parent(), (UnityEngine.Object) null) && ((Component) transform.get_parent()).get_gameObject().get_activeInHierarchy())
              gameParameter.UpdateValue();
          }
        }
        else
          ((IGameParameter) componentsInChildren[index]).UpdateValue();
      }
    }

    private void SetUpdateInterval(float interval)
    {
      if (!((Component) this).get_gameObject().get_activeInHierarchy())
        return;
      if ((double) interval <= 0.0)
      {
        if (this.mUpdateCoroutine == null)
          return;
        this.StopCoroutine(this.mUpdateCoroutine);
      }
      else
      {
        this.mNextUpdateTime = Time.get_time() + interval;
        if (this.mUpdateCoroutine != null)
          return;
        this.mUpdateCoroutine = this.StartCoroutine(this.UpdateTimer());
      }
    }

    [DebuggerHidden]
    private IEnumerator UpdateTimer()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameParameter.\u003CUpdateTimer\u003Ec__Iterator79()
      {
        \u003C\u003Ef__this = this
      };
    }

    private void OnDestroy()
    {
      GameParameter.Instances.Remove(this);
    }

    public void ToggleChildren(bool visible)
    {
      Transform transform = ((Component) this).get_transform();
      int childCount = transform.get_childCount();
      for (int index = 0; index < childCount; ++index)
        ((Component) transform.GetChild(index)).get_gameObject().SetActive(visible);
    }

    public void ToggleEmpty(bool visible)
    {
      if (!this.mIsEmptyGO)
        return;
      ((Component) this).get_gameObject().SetActive(visible);
    }

    public void ResetToDefault()
    {
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null))
        this.mText.set_text(this.mDefaultValue);
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSlider, (UnityEngine.Object) null))
      {
        this.mSlider.set_value((float) this.mDefaultRangeValue.x);
        this.mSlider.set_maxValue((float) this.mDefaultRangeValue.y);
      }
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mInputField, (UnityEngine.Object) null))
        this.mInputField.set_text(this.mDefaultValue);
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImage, (UnityEngine.Object) null))
      {
        this.mImage.set_texture(this.mDefaultImage);
      }
      else
      {
        if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null))
          return;
        this.mImageArray.set_sprite(this.mDefaultSprite);
      }
    }

    private void SetTextValue(string value)
    {
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null))
        this.mText.set_text(value);
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mInputField, (UnityEngine.Object) null))
        return;
      this.mInputField.SetText(value);
    }

    private void SetTextValue(int value)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null) && !UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mInputField, (UnityEngine.Object) null))
        return;
      this.SetTextValue(value.ToString());
    }

    private void SetTextColor(Color color)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null))
        return;
      ((Graphic) this.mText).set_color(color);
    }

    private void SetSyncColorOriginColor(Color color)
    {
      SyncColor component = (SyncColor) ((Component) this).GetComponent<SyncColor>();
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null))
        return;
      component.ForceOriginColorChange(color);
    }

    private void SetSliderValue(int value, int maxValue)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSlider, (UnityEngine.Object) null))
        return;
      this.mSlider.set_maxValue((float) maxValue);
      this.mSlider.set_minValue(0.0f);
      this.mSlider.set_value((float) value);
    }

    private void SetImageIndex(int index)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null))
        return;
      this.mImageArray.ImageIndex = index;
    }

    private void SetAnimatorInt(string name, int value)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mAnimator, (UnityEngine.Object) null))
        return;
      this.mAnimator.SetInteger(name, value);
    }

    private void SetAnimatorBool(string name, bool value)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mAnimator, (UnityEngine.Object) null))
        return;
      this.mAnimator.SetBool(name, value);
    }

    private int GetImageLength()
    {
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null))
        return this.mImageArray.Images.Length;
      return 0;
    }

    private void SetImageBySpriteSheet(string key)
    {
      ImageSpriteSheet component = (ImageSpriteSheet) ((Component) this).GetComponent<ImageSpriteSheet>();
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null))
        return;
      component.SetSprite(key);
    }

    private void Awake()
    {
      GameParameter.Instances.Add(this);
      this.mText = (UnityEngine.UI.Text) ((Component) this).GetComponent<UnityEngine.UI.Text>();
      this.mSlider = (Slider) ((Component) this).GetComponent<Slider>();
      this.mInputField = (InputField) ((Component) this).GetComponent<InputField>();
      this.mAnimator = (Animator) ((Component) this).GetComponent<Animator>();
      this.mImage = (RawImage) ((Component) this).GetComponent<RawImage>();
      this.mImageArray = (ImageArray) ((Component) this).GetComponent<ImageArray>();
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mText, (UnityEngine.Object) null))
        this.mDefaultValue = this.mText.get_text();
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSlider, (UnityEngine.Object) null))
      {
        this.mDefaultRangeValue.x = (__Null) (double) this.mSlider.get_value();
        this.mDefaultRangeValue.y = (__Null) (double) this.mSlider.get_maxValue();
      }
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mInputField, (UnityEngine.Object) null))
        this.mDefaultValue = this.mInputField.get_text();
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImage, (UnityEngine.Object) null))
        this.mDefaultImage = this.mImage.get_texture();
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mImageArray, (UnityEngine.Object) null))
        this.mDefaultSprite = this.mImageArray.get_sprite();
      else
        this.mIsEmptyGO = true;
    }

    private void Start()
    {
      this.mStarted = true;
      this.UpdateValue();
    }

    private void OnEnable()
    {
      if (this.mUpdateCoroutine != null)
      {
        this.StopCoroutine(this.mUpdateCoroutine);
        this.mUpdateCoroutine = (Coroutine) null;
      }
      if (!this.mStarted)
        return;
      this.UpdateValue();
    }

    public void UpdateValue()
    {
      if (!((Component) this).get_gameObject().get_activeInHierarchy())
      {
        if (typeof (GameParameter.ParameterTypes).GetField(this.ParameterType.ToString()).GetCustomAttributes(typeof (GameParameter.AlwaysUpdate), true).Length <= 0)
          return;
        this.InternalUpdateValue();
      }
      else
        this.InternalUpdateValue();
    }

    protected virtual void BatchUpdate()
    {
    }

    private static int AbilityTypeDetailToImageIndex(EAbilitySlot type, EAbilityTypeDetail typeDetail)
    {
      switch (typeDetail)
      {
        case EAbilityTypeDetail.Job_Unique:
        case EAbilityTypeDetail.Job_Basic:
          return 0;
        case EAbilityTypeDetail.Job_Support:
          return 1;
        case EAbilityTypeDetail.Job_Reaction:
          return 2;
        default:
          return 3;
      }
    }

    public enum QuestInstanceTypes
    {
      Any,
      Playing,
      Selected,
    }

    public enum ArenaPlayerInstanceTypes
    {
      Any,
      Enemy,
    }

    public enum UnitInstanceTypes
    {
      Any,
      OBSOLETE_MainTarget,
      OBSOLETE_SubTarget,
      CurrentTurn,
      ArenaPlayerUnit0,
      ArenaPlayerUnit1,
      ArenaPlayerUnit2,
      EnemyArenaPlayerUnit0,
      EnemyArenaPlayerUnit1,
      EnemyArenaPlayerUnit2,
      PartyUnit0,
      PartyUnit1,
      PartyUnit2,
      VersusUnit,
      MultiUnit,
      MultiTowerUnit,
      VersusCpuUnit0,
      VersusCpuUnit1,
      VersusCpuUnit2,
      RankMatchUnit,
      FriendPartyUnit,
      VersusDraftPlayerUnit0,
      VersusDraftPlayerUnit1,
      VersusDraftPlayerUnit2,
      VersusDraftPlayerUnit3,
      VersusDraftPlayerUnit4,
      VersusDraftPlayerUnit5,
      VersusDraftEnemyUnit0,
      VersusDraftEnemyUnit1,
      VersusDraftEnemyUnit2,
      VersusDraftEnemyUnit3,
      VersusDraftEnemyUnit4,
      VersusDraftEnemyUnit5,
    }

    public enum PartyAttackTypes
    {
      Normal,
      Quest,
    }

    public enum TargetInstanceTypes
    {
      Main,
      Sub,
    }

    public enum ItemInstanceTypes
    {
      Any,
      Inventory,
      QuestReward,
      Equipment,
      EnhanceMaterial,
      EnhanceEquipData,
      SellItem,
      ConsumeItem,
    }

    public enum ArtifactInstanceTypes
    {
      Any,
      QuestReward,
      Trophy,
    }

    public enum TrophyBadgeInstanceTypes
    {
      Any,
      Normal,
      Daily,
      NormalStory,
      NormalEvent,
      NormalTraining,
      NormalOther,
    }

    public enum TrophyConditionInstances
    {
      List,
      Index,
    }

    public enum VersusPlayerInstanceType
    {
      Player,
      Enemy,
    }

    public enum RankMatchPlayer
    {
      Current,
      Before,
    }

    public enum VersusDraftSlot
    {
      PlayerLeader,
      PlayerSlot2,
      PlayerSlot3,
      PlayerSlot4,
      PlayerSlot5,
      EnemyLeader,
      EnemySlot2,
      EnemySlot3,
      EnemySlot4,
      EnemySlot5,
    }

    public enum ParameterTypes
    {
      [GameParameter.ParameterDesc("プレイヤーの名前")] GLOBAL_PLAYER_NAME = 0,
      [GameParameter.ParameterDesc("プレイヤーのレベル")] GLOBAL_PLAYER_LEVEL = 1,
      [GameParameter.ParameterDesc("プレイヤーの現在のスタミナ")] GLOBAL_PLAYER_STAMINA = 2,
      [GameParameter.ParameterDesc("プレイヤーの最大スタミナ")] GLOBAL_PLAYER_STAMINAMAX = 3,
      [GameParameter.ParameterDesc("プレイヤーの経験値")] GLOBAL_PLAYER_EXP = 4,
      [GameParameter.ParameterDesc("プレイヤーが次のレベルまでに必要な経験値")] GLOBAL_PLAYER_EXPNEXT = 5,
      [GameParameter.ParameterDesc("プレイヤーの所持ゴールド")] GLOBAL_PLAYER_GOLD = 6,
      [GameParameter.ParameterDesc("プレイヤーの所持コイン")] GLOBAL_PLAYER_COIN = 7,
      [GameParameter.ParameterDesc("プレイヤーのスタミナが次に回復するまでの時間")] GLOBAL_PLAYER_STAMINATIME = 8,
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエスト名")] QUEST_NAME = 9,
      [GameParameter.ParameterDesc("クエストに必要なスタミナ"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_STAMINA = 10, // 0x0000000A
      [GameParameter.EnumParameterDesc("クエストのクリア状態にあわせてAnimatorのstateという名前のInt値、ImageArrayのインデックスを切り替えます。", typeof (QuestStates)), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_STATE = 11, // 0x0000000B
      [GameParameter.ParameterDesc("クエスト勝利条件"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_OBJECTIVE = 12, // 0x0000000C
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエストボーナス条件。インデックスでボーナス条件の番号を指定してください。"), GameParameter.UsesIndex] QUEST_BONUSOBJECTIVE = 13, // 0x0000000D
      [GameParameter.ParameterDesc("アイテムアイコン"), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes))] ITEM_ICON = 14, // 0x0000000E
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエストの説明")] QUEST_DESCRIPTION = 15, // 0x0000000F
      [GameParameter.ParameterDesc("フレンドの名前")] SUPPORTER_NAME = 16, // 0x00000010
      [GameParameter.ParameterDesc("フレンドのレベル")] SUPPORTER_LEVEL = 17, // 0x00000011
      [GameParameter.ParameterDesc("フレンドのユニットレベル")] SUPPORTER_UNITLEVEL = 18, // 0x00000012
      [GameParameter.ParameterDesc("フレンドのリーダースキル名")] SUPPORTER_LEADERSKILLNAME = 19, // 0x00000013
      [GameParameter.ParameterDesc("フレンドの攻撃力")] SUPPORTER_ATK = 20, // 0x00000014
      [GameParameter.ParameterDesc("フレンドのHP")] SUPPORTER_HP = 21, // 0x00000015
      [GameParameter.ParameterDesc("フレンドの魔法攻撃力")] SUPPORTER_MAGIC = 22, // 0x00000016
      [GameParameter.EnumParameterDesc("フレンドのレアリティにあわせてAnimatorのrareという名前のInt値を切り替えます。", typeof (ERarity))] SUPPORTER_RARITY = 23, // 0x00000017
      [GameParameter.EnumParameterDesc("フレンドの属性にあわせてAnimatorのelementという名前のInt値を切り替えます。", typeof (EElement))] SUPPORTER_ELEMENT = 24, // 0x00000018
      [GameParameter.ParameterDesc("フレンドのアイコン")] SUPPORTER_ICON = 25, // 0x00000019
      [GameParameter.ParameterDesc("フレンドのリーダースキルの説明")] SUPPORTER_LEADERSKILLDESC = 26, // 0x0000001A
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエストの副題")] QUEST_SUBTITLE = 27, // 0x0000001B
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのレベル")] UNIT_LEVEL = 28, // 0x0000001C
      [GameParameter.ParameterDesc("ユニットのHP"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_HP = 29, // 0x0000001D
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの最大HP")] UNIT_HPMAX = 30, // 0x0000001E
      [GameParameter.ParameterDesc("ユニットの攻撃力"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_ATK = 31, // 0x0000001F
      [GameParameter.ParameterDesc("ユニットの魔力"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_MAG = 32, // 0x00000020
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのアイコン")] UNIT_ICON = 33, // 0x00000021
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの名前")] UNIT_NAME = 34, // 0x00000022
      [GameParameter.EnumParameterDesc("ユニットのレアリティに応じてAnimatorにrareというint値を設定します。ImageArrayの場合レアリティに応じた番号のイメージを使用します。StarGaugeの場合現在のレアリティと最大のレアリティがそれぞれ現在値と最大値になります。", typeof (ERarity)), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_RARITY = 35, // 0x00000023
      [GameParameter.ParameterDesc("パーティのリーダースキルの名前")] PARTY_LEADERSKILLNAME = 36, // 0x00000024
      [GameParameter.ParameterDesc("パーティのリーダースキルの説明")] PARTY_LEADERSKILLDESC = 37, // 0x00000025
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの防御力")] UNIT_DEF = 38, // 0x00000026
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの魔法防御力")] UNIT_MND = 39, // 0x00000027
      [GameParameter.ParameterDesc("ユニットの素早さ"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_SPEED = 40, // 0x00000028
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの運")] UNIT_LUCK = 41, // 0x00000029
      [GameParameter.ParameterDesc("ユニットジョブ名"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOBNAME = 42, // 0x0000002A
      [GameParameter.ParameterDesc("ユニットジョブランク"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOBRANK = 43, // 0x0000002B
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.EnumParameterDesc("ユニット属性にあわせてAnimatorのelementという名前のInt値、もしくはImageArrayを切り替えます。", typeof (EElement))] UNIT_ELEMENT = 44, // 0x0000002C
      [GameParameter.InstanceTypes(typeof (GameParameter.PartyAttackTypes)), GameParameter.ParameterDesc("パーティの総攻撃力")] PARTY_TOTALATK = 45, // 0x0000002D
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("インベントリーにセットされたアイテムのアイコン\n*スロット番号をIndexで指定")] INVENTORY_ITEMICON = 46, // 0x0000002E
      [GameParameter.ParameterDesc("インベントリーにセットされたアイテムの名前*スロット番号をIndexで指定"), GameParameter.UsesIndex] INVENTORY_ITEMNAME = 47, // 0x0000002F
      [GameParameter.ParameterDesc("アイテムの名前"), GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.UsesIndex] ITEM_NAME = 48, // 0x00000030
      [GameParameter.ParameterDesc("アイテムの説明"), GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.UsesIndex] ITEM_DESC = 49, // 0x00000031
      [GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.ParameterDesc("アイテムの売却価格")] ITEM_SELLPRICE = 50, // 0x00000032
      [GameParameter.ParameterDesc("アイテムの購入価格"), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes))] ITEM_BUYPRICE = 51, // 0x00000033
      [GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.ParameterDesc("アイテムの所持個数"), GameParameter.UsesIndex] ITEM_AMOUNT = 52, // 0x00000034
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("インベントリーにセットされたアイテムの所持数*スロット番号をIndexで指定")] INVENTORY_ITEMAMOUNT = 53, // 0x00000035
      [GameParameter.ParameterDesc("所持ユニット数")] PLAYER_NUMUNITS = 54, // 0x00000036
      [GameParameter.ParameterDesc("所持可能の最大ユニット数")] PLAYER_MAXUNITS = 55, // 0x00000037
      [GameParameter.ParameterDesc("選択しているグリッドの高さ")] GRID_HEIGHT = 56, // 0x00000038
      [GameParameter.ParameterDesc("スキルの名前")] SKILL_NAME = 57, // 0x00000039
      [GameParameter.ParameterDesc("スキルのアイコン")] SKILL_ICON = 58, // 0x0000003A
      [GameParameter.ParameterDesc("スキルの説明")] SKILL_DESCRIPTION = 59, // 0x0000003B
      [GameParameter.ParameterDesc("スキルの消費ジュエル")] SKILL_MP = 60, // 0x0000003C
      [GameParameter.ParameterDesc("クエストで入手したゴールド")] BATTLE_GOLD = 61, // 0x0000003D
      [GameParameter.ParameterDesc("クエストで入手した宝箱の個数")] BATTLE_TREASURE = 62, // 0x0000003E
      [GameParameter.ParameterDesc("ユニットのMP"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_MP = 63, // 0x0000003F
      [GameParameter.ParameterDesc("ユニットの最大MP"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_MPMAX = 64, // 0x00000040
      [GameParameter.ParameterDesc("ターゲットの出力ポイント"), GameParameter.InstanceTypes(typeof (GameParameter.TargetInstanceTypes))] TARGET_OUTPUTPOINT = 65, // 0x00000041
      [GameParameter.ParameterDesc("ターゲットのクリティカル率"), GameParameter.InstanceTypes(typeof (GameParameter.TargetInstanceTypes))] TARGET_CRITICALRATE = 66, // 0x00000042
      [GameParameter.EnumParameterDesc("ターゲットの行動の種類にあわせてAnimatorにstate(Int)を設定する。", typeof (SceneBattle.TargetActionTypes)), GameParameter.InstanceTypes(typeof (GameParameter.TargetInstanceTypes))] TARGET_ACTIONTYPE = 67, // 0x00000043
      [GameParameter.ParameterDesc("アビリティのアイコン")] ABILITY_ICON = 68, // 0x00000044
      [GameParameter.ParameterDesc("アビリティの名前")] ABILITY_NAME = 69, // 0x00000045
      [GameParameter.ParameterDesc("クエストで入手可能な欠片のアイコン"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_KAKERA_ICON = 70, // 0x00000046
      [GameParameter.ParameterDesc("ユニットの経験値"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_EXP = 71, // 0x00000047
      [GameParameter.ParameterDesc("ユニットのレベルアップに必要な経験値の合計"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_EXPMAX = 72, // 0x00000048
      [GameParameter.ParameterDesc("ユニットのレベルアップに必要な経験値の残り"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_EXPTOGO = 73, // 0x00000049
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの覚醒に必要な欠片の所持数")] UNIT_KAKERA_NUM = 74, // 0x0000004A
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの覚醒に必要な欠片の数")] UNIT_KAKERA_MAX = 75, // 0x0000004B
      [GameParameter.ParameterDesc("装備品の経験値 (未実装)")] EQUIPMENT_EXP = 76, // 0x0000004C
      [GameParameter.ParameterDesc("装備品の強化に必要な経験値 (未実装)")] EQUIPMENT_EXPMAX = 77, // 0x0000004D
      [GameParameter.ParameterDesc("装備品のランク。Animatorであればrankというint値にランクを設定する。ImageArrayであればランクに対応したイメージを使用する。")] EQUIPMENT_RANK = 78, // 0x0000004E
      [GameParameter.ParameterDesc("アビリティのレベル")] ABILITY_RANK = 79, // 0x0000004F
      [GameParameter.ParameterDesc("アビリティの経験値")] OBSOLETE_ABILITY = 80, // 0x00000050
      [GameParameter.ParameterDesc("アビリティの最大経験値")] ABILITY_NEXTGOLD = 81, // 0x00000051
      [GameParameter.EnumParameterDesc("アビリティの種類にあわせて、Animatorのtype、ImageArrayを切り替えます。", typeof (EAbilitySlot))] ABILITY_SLOT = 82, // 0x00000052
      [GameParameter.ParameterDesc("ユニットのIndexで指定したジョブのアイコン"), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOB_JOBICON = 83, // 0x00000053
      [GameParameter.ParameterDesc("ユニットのIndexで指定したジョブのランク"), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOB_RANK = 84, // 0x00000054
      [GameParameter.ParameterDesc("ユニットのIndexで指定したジョブの名前"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.UsesIndex] UNIT_JOB_NAME = 85, // 0x00000055
      [GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのIndexで指定したジョブの最大ランク")] UNIT_JOB_RANKMAX = 86, // 0x00000056
      [GameParameter.ParameterDesc("装備アイテムのHP。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_HP = 87, // 0x00000057
      [GameParameter.ParameterDesc("装備アイテムのAP。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_AP = 88, // 0x00000058
      [GameParameter.ParameterDesc("装備アイテムの初期AP。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_IAP = 89, // 0x00000059
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの攻撃力。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。")] EQUIPMENT_ATK = 90, // 0x0000005A
      [GameParameter.ParameterDesc("装備アイテムの防御力。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_DEF = 91, // 0x0000005B
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの魔法攻撃力。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。")] EQUIPMENT_MAG = 92, // 0x0000005C
      [GameParameter.ParameterDesc("装備アイテムの魔法防御力。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_MND = 93, // 0x0000005D
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの回復力。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。")] EQUIPMENT_REC = 94, // 0x0000005E
      [GameParameter.ParameterDesc("装備アイテムの速度。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_SPD = 95, // 0x0000005F
      [GameParameter.ParameterDesc("装備アイテムのクリティカル。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_CRI = 96, // 0x00000060
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの運。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。")] EQUIPMENT_LUK = 97, // 0x00000061
      [GameParameter.ParameterDesc("装備アイテムの移動量。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_MOV = 98, // 0x00000062
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの移動高低差。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。")] EQUIPMENT_JMP = 99, // 0x00000063
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの射程。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。")] EQUIPMENT_RANGE = 100, // 0x00000064
      [GameParameter.ParameterDesc("装備アイテムの範囲。空のゲームオブジェクトの場合は値が0の時自身を非表示にします。"), GameParameter.AlwaysUpdate] EQUIPMENT_SCOPE = 101, // 0x00000065
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの影響可能な高低差範囲。値が0の時、子供を非表示にし、LayoutElementを無効にします。")] EQUIPMENT_EFFECTHEIGHT = 102, // 0x00000066
      [GameParameter.ParameterDesc("装備アイテムの名前")] EQUIPMENT_NAME = 103, // 0x00000067
      [GameParameter.ParameterDesc("装備アイテムのアイコン")] EQUIPMENT_ICON = 104, // 0x00000068
      [GameParameter.ParameterDesc("アビリティ強化に使用できるポイントの残り")] OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_NUM = 105, // 0x00000069
      [GameParameter.ParameterDesc("アビリティを強化できる回数")] OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_RANKUPCOUNT = 106, // 0x0000006A
      [GameParameter.ParameterDesc("アビリティを強化できる回数の最大値")] OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_RANKUPCOUNTMAX = 107, // 0x0000006B
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("アビリティを強化できる回数のクールダウン時間。\nクールダウン時間が無い場合は子供を非表示にします。")] OBSOLETE_GLOBAL_PLAYER_ABILITYPOINT_COOLDOWNTIME = 108, // 0x0000006C
      [GameParameter.ParameterDesc("装備アイテムの所持数")] EQUIPMENT_AMOUNT = 109, // 0x0000006D
      [GameParameter.ParameterDesc("装備アイテムを装備するために必要なレベル")] EQUIPMENT_REQLV = 110, // 0x0000006E
      [GameParameter.ParameterDesc("進化素材の所持個数。スライダー対応")] JOBEVOITEM_AMOUNT = 111, // 0x0000006F
      [GameParameter.ParameterDesc("進化素材の必要個数")] JOBEVOITEM_REQAMOUNT = 112, // 0x00000070
      [GameParameter.ParameterDesc("進化素材のアイコン")] JOBEVOITEM_ICON = 113, // 0x00000071
      [GameParameter.ParameterDesc("進化素材の名前")] JOBEVOITEM_NAME = 114, // 0x00000072
      [GameParameter.ParameterDesc("ユニットの現在のジョブを進化させるのに必要なゴールド")] UNIT_EVOCOST = 115, // 0x00000073
      [GameParameter.ParameterDesc("ユニットのクリティカル値"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_CRIT = 116, // 0x00000074
      [GameParameter.ParameterDesc("ユニットの回復力"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_REGEN = 117, // 0x00000075
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットが持つリーダースキルの名前")] UNIT_LEADERSKILLNAME = 118, // 0x00000076
      [GameParameter.ParameterDesc("ユニットが持つリーダースキルの説明"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_LEADERSKILLDESC = 119, // 0x00000077
      [GameParameter.ParameterDesc("アイテムの効果値")] ITEM_VALUE = 120, // 0x00000078
      [GameParameter.ParameterDesc("ユニットのレベルの最大値")] UNIT_LEVELMAX = 121, // 0x00000079
      [GameParameter.ParameterDesc("ユニットのIndexで指定したジョブの解放状態にあわせてAnimatorにBoolパラメーターunlockedを設定します。解放済みであればunlockedがオンになります。"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.UsesIndex] UNIT_JOB_UNLOCKSTATE = 122, // 0x0000007A
      [GameParameter.ParameterDesc("ユニットの現在のジョブのランク"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOBRANKMAX = 123, // 0x0000007B
      [GameParameter.ParameterDesc("アビリティの解放条件")] ABILITY_UNLOCKINFO = 124, // 0x0000007C
      [GameParameter.ParameterDesc("アビリティの説明")] ABILITY_DESC = 125, // 0x0000007D
      [GameParameter.ParameterDesc("アイテムの種類にあわせたフレームをImageに設定します。フレームの設定はGameSettings.ItemIconsを参照します。"), GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.UsesIndex] ITEM_FRAME = 126, // 0x0000007E
      [GameParameter.ParameterDesc("インベントリーにセットされたアイテムのフレーム。Item/Frameと同じです。"), GameParameter.UsesIndex] INVENTORY_FRAME = 127, // 0x0000007F
      [GameParameter.ParameterDesc("アイテム作成素材の所持個数")] RECIPEITEM_AMOUNT = 128, // 0x00000080
      [GameParameter.ParameterDesc("アイテム作成素材の必要個数")] RECIPEITEM_REQAMOUNT = 129, // 0x00000081
      [GameParameter.ParameterDesc("アイテム作成素材のアイコン")] RECIPEITEM_ICON = 130, // 0x00000082
      [GameParameter.ParameterDesc("アイテム作成素材の名前")] RECIPEITEM_NAME = 131, // 0x00000083
      [GameParameter.ParameterDesc("アイテム作成コスト")] RECIPEITEM_CREATE_COST = 132, // 0x00000084
      [GameParameter.ParameterDesc("作成アイテム名")] RECIPEITEM_CREATE_ITEM_NAME = 133, // 0x00000085
      [GameParameter.ParameterDesc("アイテム作成素材のフレーム")] RECIPEITEM_FRAME = 134, // 0x00000086
      [GameParameter.ParameterDesc("ユニットのキャライメージ (中サイズ) GameSettings.Unit_PortraitMedium で命名規則を決めれます。"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_PORTRAIT_MEDIUM = 135, // 0x00000087
      [GameParameter.ParameterDesc("クエストで入手した補正値等も含めたゴールドの合計")] QUESTRESULT_GOLD = 136, // 0x00000088
      [GameParameter.ParameterDesc("クエストでプレイヤーが得た経験値")] QUESTRESULT_PLAYEREXP = 137, // 0x00000089
      [GameParameter.ParameterDesc("クエストでパーティが得た経験値")] QUESTRESULT_PARTYEXP = 138, // 0x0000008A
      [GameParameter.ParameterDesc("クエストの評価結果にあわせてAnimatorのrate(int)、ImageArrayを切り替えます。※使用してない")] QUESTRESULT_RATE = 139, // 0x0000008B
      [GameParameter.ParameterDesc("プレイヤーのレベルアップ前のレベル")] PLAYERLEVELUP_LEVEL = 140, // 0x0000008C
      [GameParameter.ParameterDesc("プレイヤーのレベルアップ後のレベル")] PLAYERLEVELUP_LEVELNEXT = 141, // 0x0000008D
      [GameParameter.ParameterDesc("プレイヤーのレベルアップ後のスタミナ")] PLAYERLEVELUP_STAMINA = 142, // 0x0000008E
      [GameParameter.ParameterDesc("プレイヤーのレベルアップ前の最大スタミナ")] PLAYERLEVELUP_STAMINAMAX = 143, // 0x0000008F
      [GameParameter.ParameterDesc("プレイヤーのレベルアップ後の最大スタミナ")] PLAYERLEVELUP_STAMINAMAXNEXT = 144, // 0x00000090
      [GameParameter.ParameterDesc("プレイヤーのレベルアップ前の最大フレンドスロット数")] PLAYERLEVELUP_FRIENDNUM = 145, // 0x00000091
      [GameParameter.ParameterDesc("プレイヤーのレベルアップ後の最大フレンドスロット数")] PLAYERLEVELUP_FRIENDNUMNEXT = 146, // 0x00000092
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("アンロックされた物の説明。インデックスで行数を指定してください。")] PLAYERLEVELUP_UNLOCKINFO = 147, // 0x00000093
      [GameParameter.EnumParameterDesc("プレイ中クエストのボーナス条件の達成状態にあわせてAnimatorのstate(int)、ImageArrayを切り替えます。インデックスでボーナス条件の番号を指定してください。", typeof (QuestBonusObjectiveState)), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_BONUSOBJECTIVE_STATE = 148, // 0x00000094
      OBSOLETE_QUEST_BONUSOBJECTIVE_ITEMICON = 149, // 0x00000095
      OBSOLETE_QUEST_BONUSOBJECTIVE_ITEMAMOUNT = 150, // 0x00000096
      [GameParameter.EnumParameterDesc("ユニットの陣営にあわせてImageArrayのインデックス、Animatorのindex(int)を切り替えます。", typeof (EUnitSide))] UNIT_SIDE = 151, // 0x00000097
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのジョブのアイコン")] UNIT_JOBICON = 152, // 0x00000098
      [GameParameter.ParameterDesc("ユニットの現ジョブのアイコン。GameSettingsのJobIcon2のパスを使用する。"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOBICON2 = 153, // 0x00000099
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("ユニットのIndexで指定したジョブのアイコン。GameSettingsのJobIcon2のパスを使用する。"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOB_JOBICON2 = 154, // 0x0000009A
      [GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.UsesIndex, GameParameter.ParameterDesc("アイテムの作成コスト")] ITEM_CREATECOST = 155, // 0x0000009B
      [GameParameter.ParameterDesc("プレイヤーの現在の洞窟用スタミナ")] GLOBAL_PLAYER_CAVESTAMINA = 156, // 0x0000009C
      [GameParameter.ParameterDesc("プレイヤーの最大の洞窟用スタミナ")] GLOBAL_PLAYER_CAVESTAMINAMAX = 157, // 0x0000009D
      [GameParameter.ParameterDesc("プレイヤーの洞窟用スタミナが次に回復するまでの時間")] GLOBAL_PLAYER_CAVESTAMINATIME = 158, // 0x0000009E
      [GameParameter.ParameterDesc("アイテムの種類ごとの所持上限"), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes))] ITEM_AMOUNTMAX = 159, // 0x0000009F
      [GameParameter.ParameterDesc("所持しているアイテムの種類")] PLAYER_NUMITEMS = 160, // 0x000000A0
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエストが通常クエストかエリートクエストかどうかでImageArrayのインデックスを切り替えます。0=通常、1=エリート、2=エクストラ")] QUEST_DIFFICULTY = 161, // 0x000000A1
      [GameParameter.ParameterDesc("ユニットの現在位置の高さ"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_HEIGHT = 162, // 0x000000A2
      [GameParameter.ParameterDesc("装備アイテムの種類にあわせたフレームをImageに設定します。フレームの設定はGameSettings.ItemIconsを参照します。"), GameParameter.UsesIndex] EQUIPMENT_FRAME = 163, // 0x000000A3
      [GameParameter.ParameterDesc("クエストリストで使用するチャプター(章)の名前")] QUESTLIST_CHAPTERNAME = 164, // 0x000000A4
      [GameParameter.ParameterDesc("クエストリストで使用するセクション(部)の名前")] QUESTLIST_SECTIONNAME = 165, // 0x000000A5
      [GameParameter.ParameterDesc("メールの文面")] MAIL_MESSAGE = 166, // 0x000000A6
      [GameParameter.ParameterDesc("マルチクエストが通常マップかイベントマップかどうかでImageArrayのインデックスを切り替えます。0=通常、1=イベント"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_MULTI_TYPE = 167, // 0x000000A7
      [GameParameter.ParameterDesc("マルチプレイヤーの名前")] MULTI_PLAYER_NAME = 168, // 0x000000A8
      [GameParameter.ParameterDesc("マルチプレイヤーのレベル")] MULTI_PLAYER_LEVEL = 169, // 0x000000A9
      [GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (JSON_MyPhotonPlayerParam.EState)), GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("マルチプレイヤーの状態( Index: 0 == 否定 / 1 == 完全一致")] MULTI_PLAYER_STATE = 170, // 0x000000AA
      [GameParameter.AlwaysUpdate, GameParameter.UsesIndex, GameParameter.ParameterDesc("マルチプレイヤーのユニットアイコン")] MULTI_PLAYER_UNIT_ICON = 171, // 0x000000AB
      [GameParameter.ParameterDesc("メールで付与されるアイテムの文字列表現")] MAIL_GIFT_STRING = 172, // 0x000000AC
      [GameParameter.ParameterDesc("メールの有効期限")] MAIL_GIFT_LIMIT = 173, // 0x000000AD
      [GameParameter.ParameterDesc("メールを既読にした日時")] MAIL_GIFT_GETAT = 174, // 0x000000AE
      [GameParameter.ParameterDesc("マルチ部屋リストのコメント")] MULTI_ROOM_LIST_COMMENT = 175, // 0x000000AF
      [GameParameter.ParameterDesc("マルチ部屋リストの部屋主名")] MULTI_ROOM_LIST_OWNER_NAME = 176, // 0x000000B0
      [GameParameter.ParameterDesc("マルチ部屋リストの部屋主レベル")] MULTI_ROOM_LIST_OWNER_LV = 177, // 0x000000B1
      [GameParameter.ParameterDesc("マルチ部屋リストのクエスト名")] MULTI_ROOM_LIST_QUEST_NAME = 178, // 0x000000B2
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("マルチ部屋リストの鍵アイコン. 0:鍵あり 1:鍵なし"), GameParameter.AlwaysUpdate] MULTI_ROOM_LIST_LOCKED_ICON = 179, // 0x000000B3
      [GameParameter.ParameterDesc("マルチ部屋リストの参加人数")] MULTI_ROOM_LIST_PLAYER_NUM = 180, // 0x000000B4
      [GameParameter.ParameterDesc("プレイヤーのフレンドコード")] GLOBAL_PLAYER_FRIENDCODE = 181, // 0x000000B5
      [GameParameter.ParameterDesc("フレンドのフレンドコード")] FRIEND_FRIENDCODE = 182, // 0x000000B6
      [GameParameter.ParameterDesc("フレンドの名前")] FRIEND_NAME = 183, // 0x000000B7
      [GameParameter.ParameterDesc("フレンドのレベル")] FRIEND_LEVEL = 184, // 0x000000B8
      [GameParameter.ParameterDesc("フレンドの最終ログイン")] FRIEND_LASTLOGIN = 185, // 0x000000B9
      [GameParameter.ParameterDesc("所持可能の最大アイテム数")] PLAYER_MAXITEMS = 186, // 0x000000BA
      [GameParameter.ParameterDesc("売却アイテムの選択数分の価格")] SHOP_ITEM_SELLPRICE = 187, // 0x000000BB
      [GameParameter.ParameterDesc("売却アイテムの数")] SHOP_ITEM_SELLNUM = 188, // 0x000000BC
      [GameParameter.ParameterDesc("売却アイテムの選択インデックス")] SHOP_ITEM_SELLINDEX = 189, // 0x000000BD
      [GameParameter.ParameterDesc("売却アイテムの選択数")] SHOP_ITEM_SELLSELECTCOUNT = 190, // 0x000000BE
      [GameParameter.ParameterDesc("ショップ総売却価格")] SHOP_SELLPRICETOTAL = 191, // 0x000000BF
      [GameParameter.ParameterDesc("ショップアイテムのインベントリ設定状態で表示状態を切り替え"), GameParameter.AlwaysUpdate] SHOP_ITEM_STATE_INVENTORY = 192, // 0x000000C0
      [GameParameter.ParameterDesc("ショップアイテムの設置数を取得")] SHOP_ITEM_BUYAMOUNT = 193, // 0x000000C1
      [GameParameter.ParameterDesc("ショップアイテムの購入総額を取得")] SHOP_ITEM_BUYPRICE = 194, // 0x000000C2
      [GameParameter.ParameterDesc("ショップアイテムの売却済み状態で表示状態を切り替え"), GameParameter.AlwaysUpdate] SHOP_ITEM_STATE_SOLDOUT = 195, // 0x000000C3
      [GameParameter.ParameterDesc("ショップアイテムの購入通貨別のアイコン"), GameParameter.UsesIndex] SHOP_ITEM_BUYTYPEICON = 196, // 0x000000C4
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ショップアイテムの売却選択状態で表示状態を切り替え")] SHOP_ITEM_STATE_SELLSELECT = 197, // 0x000000C5
      [GameParameter.ParameterDesc("ショップアイテムのアイコン上の売却数")] SHOP_ITEM_ICONSELLNUM = 198, // 0x000000C6
      [GameParameter.ParameterDesc("装備可能ユニットが存在する場合のバッジアイコンの表示状態の切り替え"), GameParameter.AlwaysUpdate] SHOP_ITEM_STATE_ENABLEEQUIPUNIT = 199, // 0x000000C7
      [GameParameter.ParameterDesc("ショップアイテムの商品一覧の更新時間")] SHOP_ITEM_UPDATETIME = 200, // 0x000000C8
      [GameParameter.ParameterDesc("プレイヤーに来ているフレンド申請通知の数")] PLAYER_FRIENDREQUESTNUM = 201, // 0x000000C9
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのIndexで指定したジョブのクラスチェンジ先のジョブのアイコン"), GameParameter.UsesIndex] UNIT_JOB_CLASSCHANGE_JOBICON = 202, // 0x000000CA
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのIndexで指定したジョブの名前"), GameParameter.UsesIndex] UNIT_JOB_CLASSCHANGE_NAME = 203, // 0x000000CB
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのIndexで指定したジョブのクラスチェンジ先のジョブのアイコン"), GameParameter.UsesIndex] UNIT_JOB_CLASSCHANGE_JOBICON2 = 204, // 0x000000CC
      [GameParameter.ParameterDesc("ショップアイテムのアイコン上の売却数表示切り替え"), GameParameter.AlwaysUpdate] SHOP_ITEM_ICONSELLNUMSHOWED = 205, // 0x000000CD
      [GameParameter.ParameterDesc("プレイヤーのフレンド保持上限")] PLAYER_FRIENDMAX = 206, // 0x000000CE
      [GameParameter.ParameterDesc("プレイヤーの保持しているフレンドの数")] PLAYER_FRIENDNUM = 207, // 0x000000CF
      [GameParameter.ParameterDesc("ユニットの長い説明文"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_PROFILETEXT = 208, // 0x000000D0
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのイメージ画像")] UNIT_IMAGE = 209, // 0x000000D1
      [GameParameter.ParameterDesc("マルチ部屋リストの募集人数")] MULTI_ROOM_LIST_PLAYER_NUM_MAX = 210, // 0x000000D2
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("マルチプレイヤーのユニットアイコンフレーム")] MULTI_PLAYER_UNIT_ICON_FRAME = 211, // 0x000000D3
      [GameParameter.ParameterDesc("マルチプレイヤーのプレイヤーID")] MULTI_PLAYER_INDEX = 212, // 0x000000D4
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("マルチプレイヤーが部屋主のときに表示")] MULTI_PLAYER_IS_ROOM_OWNER = 213, // 0x000000D5
      [GameParameter.ParameterDesc("マルチプレイヤーがいないときに表示"), GameParameter.AlwaysUpdate] MULTI_PLAYER_IS_EMPTY = 214, // 0x000000D6
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("マルチプレイヤーがいるときに表示")] MULTI_PLAYER_IS_VALID = 215, // 0x000000D7
      [GameParameter.ParameterDesc("実績の名前")] TROPHY_NAME = 216, // 0x000000D8
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("共闘マルチのとき表示")] MULTI_ROOM_TYPE_IS_RAID = 217, // 0x000000D9
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("対戦マルチのとき表示")] MULTI_ROOM_TYPE_IS_VERSUS = 218, // 0x000000DA
      [GameParameter.ParameterDesc("マルチパーティの総攻撃力")] MULTI_PARTY_TOTALATK = 219, // 0x000000DB
      [GameParameter.ParameterDesc("現在のユニット操作プレイヤーID"), GameParameter.AlwaysUpdate] MULTI_CURRENT_PLAYER_INDEX = 220, // 0x000000DC
      [GameParameter.ParameterDesc("自キャラ行動までの残りターン"), GameParameter.AlwaysUpdate] MULTI_MY_NEXT_TURN = 221, // 0x000000DD
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("残りの入力制限時間")] MULTI_INPUT_TIME_LIMIT = 222, // 0x000000DE
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("現在のユニット操作プレイヤー名")] MULTI_CURRENT_PLAYER_NAME = 223, // 0x000000DF
      [GameParameter.ParameterDesc("鍵つき部屋を作るとき表示"), GameParameter.AlwaysUpdate] QUEST_MULTI_LOCK = 224, // 0x000000E0
      [GameParameter.ParameterDesc("現在の部屋コメント"), GameParameter.AlwaysUpdate] MULTI_CURRENT_ROOM_COMMENT = 225, // 0x000000E1
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("現在の部屋パスコード/0 == 半角 / 1 == 全角")] MULTI_CURRENT_ROOM_PASSCODE = 226, // 0x000000E2
      [GameParameter.ParameterDesc("ユニットが不参加スロット枠のとき表示"), GameParameter.UsesIndex, GameParameter.AlwaysUpdate] MULTI_CURRENT_ROOM_UNIT_SLOT_DISABLE = 227, // 0x000000E3
      [GameParameter.ParameterDesc("現在の部屋のクエスト名"), GameParameter.AlwaysUpdate] MULTI_CURRENT_ROOM_QUEST_NAME = 228, // 0x000000E4
      [GameParameter.ParameterDesc("マルチプレイのとき非表示(0)/表示(1)/NotInteractive(2)/Interactive(3)"), GameParameter.UsesIndex, GameParameter.AlwaysUpdate] QUEST_IS_MULTI = 229, // 0x000000E5
      [GameParameter.InstanceTypes(typeof (GameParameter.TrophyConditionInstances)), GameParameter.ParameterDesc("実績の条件のテキスト"), GameParameter.UsesIndex] TROPHY_CONDITION_TITLE = 230, // 0x000000E6
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("実績の条件のカウント、スライダーにもできるよ"), GameParameter.InstanceTypes(typeof (GameParameter.TrophyConditionInstances))] TROPHY_CONDITION_COUNT = 231, // 0x000000E7
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("実績の条件の必要カウント"), GameParameter.InstanceTypes(typeof (GameParameter.TrophyConditionInstances))] TROPHY_CONDITION_COUNTMAX = 232, // 0x000000E8
      [GameParameter.ParameterDesc("アイテムの素材経験値"), GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes))] ITEM_ENHANCEPOINT = 233, // 0x000000E9
      [GameParameter.ParameterDesc("装備アイテムの強化素材の選択数")] EQUIPITEM_ENHANCE_MATERIALSELECTCOUNT = 234, // 0x000000EA
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("アイテム所持数によって表示状態を変更（0個の場合非表示）"), GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes))] ITEM_SHOWED_AMOUNT = 235, // 0x000000EB
      [GameParameter.ParameterDesc("強化パラメータ名")] EQUIPITEM_PARAMETER_NAME = 236, // 0x000000EC
      [GameParameter.ParameterDesc("装備アイテムの初期値")] EQUIPITEM_PARAMETER_INITVALUE = 237, // 0x000000ED
      [GameParameter.ParameterDesc("装備アイテムの上昇値")] EQUIPITEM_PARAMETER_RANKUPVALUE = 238, // 0x000000EE
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムの上昇量に応じて表示状態を変更")] EQUIPITEM_PARAMETER_SHOWED_RANKUPVALUE = 239, // 0x000000EF
      [GameParameter.ParameterDesc("装備アイテムの強化素材の選択個数によって表示状態を変更（選択数が0の場合は非表示）"), GameParameter.AlwaysUpdate] EQUIPITEM_ENHANCE_SHOWED_MATERIALSELECTCOUNT = 240, // 0x000000F0
      [GameParameter.ParameterDesc("装備アイテムの強化素材の選択状態によって表示状態を変更（選択していない場合は非表示）"), GameParameter.AlwaysUpdate] EQUIPITEM_ENHANCE_SHOWED_MATERIALSELECT = 241, // 0x000000F1
      [GameParameter.ParameterDesc("装備アイテムの強化ゲージ")] EQUIPITEM_ENHANCE_GAUGE = 242, // 0x000000F2
      [GameParameter.ParameterDesc("装備アイテムの現在の強化ポイント")] EQUIPITEM_ENHANCE_CURRENTEXP = 243, // 0x000000F3
      [GameParameter.ParameterDesc("装備アイテムのランクアップまでの強化ポイント")] EQUIPITEM_ENHANCE_NEXTEXP = 244, // 0x000000F4
      [GameParameter.ParameterDesc("装備アイテムの強化前のランク")] EQUIPITEM_ENHANCE_RANKBEFORE = 245, // 0x000000F5
      [GameParameter.ParameterDesc("装備アイテムの強化後のランク")] EQUIPITEM_ENHANCE_RANKAFTER = 246, // 0x000000F6
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("装備アイテムのランクに応じたイメージを使用します")] EQUIPDATA_RANKBADGE = 247, // 0x000000F7
      [GameParameter.ParameterDesc("機能がアンロックされている場合のみ表示"), GameParameter.InstanceTypes(typeof (UnlockTargets))] UNLOCK_SHOWED = 248, // 0x000000F8
      [GameParameter.ParameterDesc("切断されたプレイヤーIndex")] MULTI_NOTIFY_DISCONNECTED_PLAYER_INDEX = 249, // 0x000000F9
      [GameParameter.ParameterDesc("切断されたプレイヤーが(0:部屋主じゃなかったとき表示 1:他人が部屋主になったとき表示 2:自分が部屋主になったとき表示)"), GameParameter.UsesIndex] MULTI_NOTIFY_DISCONNECTED_PLAYER_IS_ROOM_OWNER = 250, // 0x000000FA
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("行動順のプレイヤーが切断されているとき表示(0) 非表示(1)")] MULTI_CURRENT_PLAYER_IS_DISCONNECTED = 251, // 0x000000FB
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("行動順のプレイヤーが部屋主かどうか")] MULTI_CURRENT_PLAYER_IS_ROOM_OWNER = 252, // 0x000000FC
      [GameParameter.ParameterDesc("自分が部屋主のとき表示(0) 非表示(1)"), GameParameter.AlwaysUpdate, GameParameter.UsesIndex] MULTI_I_AM_ROOM_OWNER = 253, // 0x000000FD
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("部屋主のプレイヤーIndex")] MULTI_ROOM_OWNER_PLAYER_INDEX = 254, // 0x000000FE
      [GameParameter.ParameterDesc("ガチャでドロップしたものの名称")] GACHA_DROPNAME = 255, // 0x000000FF
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("達成済みデイリーミッションの有無で表示状態を切り替える"), GameParameter.InstanceTypes(typeof (GameParameter.TrophyBadgeInstanceTypes))] TROPHY_BADGE = 256, // 0x00000100
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("実績の報酬ゴールド。ゴールドが0なら自身を非表示にする。")] TROPHY_REWARDGOLD = 257, // 0x00000101
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("実績の報酬コイン。コインが0なら自身を非表示にする。")] TROPHY_REWARDCOIN = 258, // 0x00000102
      [GameParameter.ParameterDesc("実績の報酬プレイヤー経験値。経験値が0なら自身を非表示にする。"), GameParameter.AlwaysUpdate] TROPHY_REWARDEXP = 259, // 0x00000103
      [GameParameter.ParameterDesc("報酬に含まれる経験値")] REWARD_EXP = 260, // 0x00000104
      [GameParameter.ParameterDesc("報酬に含まれるコイン")] REWARD_COIN = 261, // 0x00000105
      [GameParameter.ParameterDesc("報酬に含まれるゴールド")] REWARD_GOLD = 262, // 0x00000106
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットのお気に入りロック状態")] UNIT_FAVORITE = 263, // 0x00000107
      [GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.ParameterDesc("装備アイテムの種類にあわせたフレームをImageに設定します。フレームの設定はGameSettings.ItemIconsを参照します。")] EQUIPDATA_FRAME = 264, // 0x00000108
      [GameParameter.ParameterDesc("ジョブのランクにあわせてImageArrayを切り替えます"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOBRANKFRAME = 265, // 0x00000109
      [GameParameter.ParameterDesc("ローカルプレイヤーのレベルによってキャップされたユニットの最大レベル"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_CAPPEDLEVELMAX = 266, // 0x0000010A
      [GameParameter.ParameterDesc("リビジョン番号")] APPLICATION_REVISION = 267, // 0x0000010B
      [GameParameter.ParameterDesc("ビルドID")] APPLICATION_BUILD = 268, // 0x0000010C
      [GameParameter.ParameterDesc("アセットのリビジョン番号")] APPLICATION_ASSETREVISION = 269, // 0x0000010D
      [GameParameter.ParameterDesc("プロダクト名称")] PRODUCT_NAME = 270, // 0x0000010E
      [GameParameter.ParameterDesc("プロダクト値段")] PRODUCT_PRICE = 271, // 0x0000010F
      [GameParameter.InstanceTypes(typeof (GameParameter.ArenaPlayerInstanceTypes)), GameParameter.ParameterDesc("アリーナプレイヤーの順位")] ARENAPLAYER_RANK = 272, // 0x00000110
      [GameParameter.InstanceTypes(typeof (GameParameter.ArenaPlayerInstanceTypes)), GameParameter.ParameterDesc("アリーナプレイヤーの総攻撃力")] ARENAPLAYER_TOTALATK = 273, // 0x00000111
      [GameParameter.InstanceTypes(typeof (GameParameter.ArenaPlayerInstanceTypes)), GameParameter.ParameterDesc("アリーナプレイヤーのリーダースキル")] ARENAPLAYER_LEADERSKILLNAME = 274, // 0x00000112
      [GameParameter.InstanceTypes(typeof (GameParameter.ArenaPlayerInstanceTypes)), GameParameter.ParameterDesc("アリーナプレイヤーのリーダースキルの説明")] ARENAPLAYER_LEADERSKILLDESC = 275, // 0x00000113
      [GameParameter.ParameterDesc("アリーナプレイヤーの名前"), GameParameter.InstanceTypes(typeof (GameParameter.ArenaPlayerInstanceTypes))] ARENAPLAYER_NAME = 276, // 0x00000114
      [GameParameter.ParameterDesc("プレイヤーのアリーナランク")] GLOBAL_PLAYER_ARENARANK = 277, // 0x00000115
      [GameParameter.ParameterDesc("チケット数")] QUEST_TICKET = 278, // 0x00000116
      [GameParameter.ParameterDesc("チケット使用可能な場合のみ表示"), GameParameter.AlwaysUpdate] QUEST_IS_TICKET = 279, // 0x00000117
      [GameParameter.ParameterDesc("アリーナの挑戦権")] GLOBAL_PLAYER_ARENATICKETS = 280, // 0x00000118
      [GameParameter.ParameterDesc("アリーナのクールダウンタイム")] GLOBAL_PLAYER_ARENACOOLDOWNTIME = 281, // 0x00000119
      [GameParameter.ParameterDesc("本日のマルチプレイ残り報酬獲得回数")] MULTI_REST_REWARD = 282, // 0x0000011A
      [GameParameter.AlwaysUpdate, GameParameter.UsesIndex, GameParameter.ParameterDesc("ユニットが不参加スロット枠のとき押せない")] MULTI_CURRENT_ROOM_UNIT_SLOT_DISABLE_NOT_INTERACTIVE = 283, // 0x0000011B
      [GameParameter.ParameterDesc("お客様コード")] GLOBAL_PLAYER_OKYAKUSAMACODE = 284, // 0x0000011C
      [GameParameter.ParameterDesc("機能がアンロックされている場合のみ有効"), GameParameter.InstanceTypes(typeof (UnlockTargets))] UNLOCK_ENABLED = 285, // 0x0000011D
      [GameParameter.ParameterDesc("機能がアンロックされていると表示されなくなる"), GameParameter.InstanceTypes(typeof (UnlockTargets)), GameParameter.AlwaysUpdate] UNLOCK_HIDDEN = 286, // 0x0000011E
      [GameParameter.ParameterDesc("報酬に含まれるアリーナメダル")] REWARD_ARENAMEDAL = 287, // 0x0000011F
      [GameParameter.ParameterDesc("ショップアイテムの商品一覧の更新日")] SHOP_ITEM_UPDATEDAY = 288, // 0x00000120
      [GameParameter.ParameterDesc("アリーナプレイヤーのレベル"), GameParameter.InstanceTypes(typeof (GameParameter.ArenaPlayerInstanceTypes))] ARENAPLAYER_LEVEL = 289, // 0x00000121
      [GameParameter.ParameterDesc("プレイヤーのVIPランク")] GLOBAL_PLAYER_VIPRANK = 290, // 0x00000122
      [GameParameter.ParameterDesc("ユニットの装備品を更新"), GameParameter.UsesIndex] UNIT_EQUIPSLOT_UPDATE = 291, // 0x00000123
      [GameParameter.ParameterDesc("ユニットパラメータ指定の初期状態でのアイコン表示")] UNITPARAM_ICON = 292, // 0x00000124
      [GameParameter.ParameterDesc("ユニットパラメータ指定の初期状態でのレアリティ")] UNITPARAM_RARITY = 293, // 0x00000125
      [GameParameter.ParameterDesc("ユニットパラメータ指定の初期状態でのジョブアイコン")] UNITPARAM_JOBICON = 294, // 0x00000126
      [GameParameter.ParameterDesc("ユニットパラメータ指定の初期状態での欠片所持数")] UNITPARAM_PIECE_AMOUNT = 295, // 0x00000127
      [GameParameter.ParameterDesc("ユニットパラメータ指定の初期状態での欠片必要数")] UNITPARAM_PIECE_NEED = 296, // 0x00000128
      [GameParameter.ParameterDesc("ユニットパラメータ指定の初期状態での欠片ゲージ")] UNITPARAM_PIECE_GAUGE = 297, // 0x00000129
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットパラメータ指定でアンロック可能か確認")] UNITPARAM_IS_UNLOCKED = 298, // 0x0000012A
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエストで入手可能な欠片のフレーム")] QUEST_KAKERA_FRAME = 299, // 0x0000012B
      [GameParameter.ParameterDesc("ユニットの連携値"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_COMBINATION = 300, // 0x0000012C
      [GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのジョブ変更可能か確認")] UNIT_STATE_JOBCHANGED = 301, // 0x0000012D
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ショップの主要通貨の表示状態")] SHOP_STATE_MAINCOSTFRAME = 302, // 0x0000012E
      [GameParameter.ParameterDesc("ショップの主要通貨アイコン")] SHOP_MAINCOST_ICON = 303, // 0x0000012F
      [GameParameter.ParameterDesc("ショップの主要通貨の所持量")] SHOP_MAINCOST_AMOUNT = 304, // 0x00000130
      [GameParameter.ParameterDesc("対象ユニットの成長バッジ"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.AlwaysUpdate] UNIT_BADGE_GROWUP = 305, // 0x00000131
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("対象ユニットの解放バッジ")] UNITPARAM_BADGE_UNLOCK = 306, // 0x00000132
      [GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.ParameterDesc("アイテムで装備可能なユニットが存在する場合に表示状態を変更するバッジ"), GameParameter.AlwaysUpdate] ITEM_BADGE_EQUIPUNIT = 307, // 0x00000133
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットのバッジ表示状態を変更")] BADGE_UNIT = 308, // 0x00000134
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニット強化のバッジ表示状態を変更")] BADGE_UNITENHANCED = 309, // 0x00000135
      [GameParameter.ParameterDesc("ユニット開放のバッジ表示状態を変更"), GameParameter.AlwaysUpdate] BADGE_UNITUNLOCKED = 310, // 0x00000136
      [GameParameter.ParameterDesc("ガチャのバッジ表示状態を変更"), GameParameter.AlwaysUpdate] BADGE_GACHA = 311, // 0x00000137
      [GameParameter.ParameterDesc("ゴールドガチャのバッジ表示状態を変更"), GameParameter.AlwaysUpdate] BADGE_GOLDGACHA = 312, // 0x00000138
      [GameParameter.ParameterDesc("レアガチャのバッジ表示状態を変更"), GameParameter.AlwaysUpdate] BADGE_RAREGACHA = 313, // 0x00000139
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("アリーナのバッジ表示状態を変更")] BADGE_ARENA = 314, // 0x0000013A
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("マルチプレイのバッジ表示状態を変更")] BADGE_MULTIPLAY = 315, // 0x0000013B
      [GameParameter.ParameterDesc("デイリーミッションのバッジ表示状態を変更"), GameParameter.AlwaysUpdate] BADGE_DAILYMISSION = 316, // 0x0000013C
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("フレンドのバッジ表示状態を変更")] BADGE_FRIEND = 317, // 0x0000013D
      [GameParameter.ParameterDesc("ギフトのバッジ表示状態を変更"), GameParameter.AlwaysUpdate] BADGE_GIFTBOX = 318, // 0x0000013E
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ショートカットメニューのバッジ表示状態を変更")] BADGE_SHORTCUTMENU = 319, // 0x0000013F
      [GameParameter.ParameterDesc("現VIPランクにおけるVIPポイント。スライダー対応")] GLOBAL_PLAYER_VIPPOINT = 320, // 0x00000140
      [GameParameter.ParameterDesc("現VIPランクにおける最大VIPポイント")] GLOBAL_PLAYER_VIPPOINTMAX = 321, // 0x00000141
      [GameParameter.ParameterDesc("プレイヤーの所持コイン (固有無償幻晶石)")] GLOBAL_PLAYER_COINFREE = 322, // 0x00000142
      [GameParameter.ParameterDesc("プレイヤーの所持コイン (固有有償幻晶石)")] GLOBAL_PLAYER_COINPAID = 323, // 0x00000143
      [GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットが編成中のパーティメンバーか？")] UNIT_STATE_PARTYMEMBER = 324, // 0x00000144
      [GameParameter.ParameterDesc("ログインボーナスの日付")] LOGINBONUS_DAYNUM = 325, // 0x00000145
      None = 326, // 0x00000146
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットがレベルソート中か？")] UNIT_STATE_LVSORT = 327, // 0x00000147
      [GameParameter.ParameterDesc("ユニットがパラメータソート中か？"), GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_STATE_PARAMSORT = 328, // 0x00000148
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットのソート対象パラメータの値")] UNIT_SORTTYPE_VALUE = 329, // 0x00000149
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("スキルの修得条件の表示有無")] SKILL_STATE_CONDITION = 330, // 0x0000014A
      [GameParameter.ParameterDesc("スキルの修得条件")] SKILL_CONDITION = 331, // 0x0000014B
      [GameParameter.ParameterDesc("アビリティの修得条件")] ABILITY_CONDITION = 332, // 0x0000014C
      [GameParameter.ParameterDesc("ガチャのコスト")] GACHA_COST = 333, // 0x0000014D
      [GameParameter.ParameterDesc("ガチャの回数")] GACHA_NUM = 334, // 0x0000014E
      [GameParameter.ParameterDesc("無料通常ガチャの残り回数")] GACHA_GOLD_RESTNUM = 335, // 0x0000014F
      [GameParameter.ParameterDesc("無料通常ガチャの残り回数の表示状態変更")] GACHA_GOLD_STATE_RESTNUM = 336, // 0x00000150
      [GameParameter.ParameterDesc("無料通常ガチャのインターバル時間表示"), GameParameter.AlwaysUpdate] GACHA_GOLD_TIMER = 337, // 0x00000151
      [GameParameter.ParameterDesc("無料通常ガチャの状態によって表示状態変更"), GameParameter.AlwaysUpdate] GACHA_GOLD_STATE_TIMER = 338, // 0x00000152
      [GameParameter.ParameterDesc("無料通常ガチャのボタン状態変更"), GameParameter.AlwaysUpdate] GACHA_GOLD_STATE_INTERACTIVE = 339, // 0x00000153
      [GameParameter.ParameterDesc("無料レアガチャのインターバル時間表示"), GameParameter.AlwaysUpdate] GACHA_COIN_TIMER = 340, // 0x00000154
      [GameParameter.ParameterDesc("無料レアガチャの状態によって表示状態変更"), GameParameter.AlwaysUpdate] GACHA_COIN_STATE_TIMER = 341, // 0x00000155
      [GameParameter.ParameterDesc("無料レアガチャのボタン状態変更"), GameParameter.AlwaysUpdate] GACHA_COIN_STATE_INTERACTIVE = 342, // 0x00000156
      [GameParameter.ParameterDesc("ユニットのイメージ画像2"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_IMAGE2 = 343, // 0x00000157
      [GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.ParameterDesc("アイテムのフレーバーテキスト")] ITEM_FLAVOR = 344, // 0x00000158
      [GameParameter.AlwaysUpdate, GameParameter.UsesIndex, GameParameter.ParameterDesc("ユニット覚醒レベル"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_AWAKE = 345, // 0x00000159
      [GameParameter.ParameterDesc("ユニットがフレンドか？"), GameParameter.AlwaysUpdate] SUPPORTER_ISFRIEND = 346, // 0x0000015A
      [GameParameter.ParameterDesc("ユニット覚醒レベル")] SUPPORTER_COST = 347, // 0x0000015B
      [GameParameter.ParameterDesc("サムネイル化されたジョブのアイコンをImageコンポーネントに設定します"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] Unit_ThumbnailedJobIcon = 348, // 0x0000015C
      [GameParameter.ParameterDesc("マルチプレイヤーのユニットジョブランクフレーム"), GameParameter.UsesIndex, GameParameter.AlwaysUpdate] MULTI_PLAYER_UNIT_JOBRANKFRAME = 349, // 0x0000015D
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("マルチプレイヤーのユニットジョブランク")] MULTI_PLAYER_UNIT_JOBRANK = 350, // 0x0000015E
      [GameParameter.ParameterDesc("マルチプレイヤーのユニットジョブアイコン"), GameParameter.UsesIndex, GameParameter.AlwaysUpdate] MULTI_PLAYER_UNIT_JOBICON = 351, // 0x0000015F
      [GameParameter.AlwaysUpdate, GameParameter.UsesIndex, GameParameter.ParameterDesc("マルチプレイヤーのユニットレア度")] MULTI_PLAYER_UNIT_RARITY = 352, // 0x00000160
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("マルチプレイヤーのユニット属性"), GameParameter.AlwaysUpdate] MULTI_PLAYER_UNIT_ELEMENT = 353, // 0x00000161
      [GameParameter.ParameterDesc("マルチプレイヤーのユニットレベル"), GameParameter.UsesIndex, GameParameter.AlwaysUpdate] MULTI_PLAYER_UNIT_LEVEL = 354, // 0x00000162
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("実績の報酬スタミナ。スタミナが0なら自身を非表示にする。")] TROPHY_REWARDSTAMINA = 355, // 0x00000163
      [GameParameter.ParameterDesc("ジョブアイコン")] JOB_JOBICON = 356, // 0x00000164
      [GameParameter.ParameterDesc("ジョブ名")] JOB_NAME = 357, // 0x00000165
      [GameParameter.ParameterDesc("クエストでプレイヤーが得たマルチコイン")] QUESTRESULT_MULTICOIN = 358, // 0x00000166
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("本日のマルチプレイ残り報酬獲得回数が0のとき表示(0)/非表示(1)/受け取れたとき表示(2)/受け取れなかったとき表示(3)/今回が最後のうけとりのとき表示(4)")] MULTI_REST_REWARD_IS_ZERO = 359, // 0x00000167
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットが参加スロット枠のとき表示")] MULTI_CURRENT_ROOM_UNIT_SLOT_ENABLE = 360, // 0x00000168
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットが割り当てられているスロット枠のとき表示")] MULTI_CURRENT_ROOM_UNIT_SLOT_VALID = 361, // 0x00000169
      [GameParameter.ParameterDesc("報酬に含まれるスタミナ")] REWARD_STAMINA = 362, // 0x0000016A
      [GameParameter.ParameterDesc("当日クエストに挑戦した回数")] QUEST_CHALLENGE_NUM = 363, // 0x0000016B
      [GameParameter.ParameterDesc("当日クエストに挑戦できる限度")] QUEST_CHALLENGE_MAX = 364, // 0x0000016C
      [GameParameter.ParameterDesc("クエストの挑戦回数をリセットした数")] QUEST_RESET_NUM = 365, // 0x0000016D
      [GameParameter.ParameterDesc("クエストの挑戦回数をリセットできる限度")] QUEST_RESET_MAX = 366, // 0x0000016E
      [GameParameter.ParameterDesc("ジョブアイコン2")] JOB_JOBICON2 = 367, // 0x0000016F
      [GameParameter.ParameterDesc("ユニットの国"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] OBSOLETE_UNIT_PROFILE_COUNTRY = 368, // 0x00000170
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの身長")] OBSOLETE_UNIT_PROFILE_HEIGHT = 369, // 0x00000171
      [GameParameter.ParameterDesc("ユニットの体重"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] OBSOLETE_UNIT_PROFILE_WEIGHT = 370, // 0x00000172
      [GameParameter.ParameterDesc("ユニットの誕生日"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] OBSOLETE_UNIT_PROFILE_BIRTHDAY = 371, // 0x00000173
      [GameParameter.ParameterDesc("ユニットの星座"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] OBSOLETE_UNIT_PROFILE_ZODIAC = 372, // 0x00000174
      [GameParameter.ParameterDesc("ユニットの血液型"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] OBSOLETE_UNIT_PROFILE_BLOOD = 373, // 0x00000175
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの好きなもの")] OBSOLETE_UNIT_PROFILE_FAVORITE = 374, // 0x00000176
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの趣味")] OBSOLETE_UNIT_PROFILE_HOBBY = 375, // 0x00000177
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【毒】")] UNIT_STATE_CONDITION_POISON = 376, // 0x00000178
      [GameParameter.ParameterDesc("ユニットの状態異常【麻痺】"), GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_STATE_CONDITION_PARALYSED = 377, // 0x00000179
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【スタン】"), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_STUN = 378, // 0x0000017A
      [GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【睡眠】")] UNIT_STATE_CONDITION_SLEEP = 379, // 0x0000017B
      [GameParameter.ParameterDesc("ユニットの状態異常【魅了】"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_CHARM = 380, // 0x0000017C
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットの状態異常【石化】"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_STATE_CONDITION_STONE = 381, // 0x0000017D
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【暗闇】"), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_BLINDNESS = 382, // 0x0000017E
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【沈黙】"), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_DISABLESKILL = 383, // 0x0000017F
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【移動禁止】"), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_DISABLEMOVE = 384, // 0x00000180
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【攻撃禁止】"), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_DISABLEATTACK = 385, // 0x00000181
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの状態異常【ゾンビ化・狂乱】"), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_ZOMBIE = 386, // 0x00000182
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットの状態異常【死の宣告】")] UNIT_STATE_CONDITION_DEATHSENTENCE = 387, // 0x00000183
      [GameParameter.ParameterDesc("ユニットの状態異常【狂化】"), GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_STATE_CONDITION_BERSERK = 388, // 0x00000184
      [GameParameter.ParameterDesc("ユニットの状態異常【ノックバック無効】"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.AlwaysUpdate] UNIT_STATE_CONDITION_DISABLEKNOCKBACK = 389, // 0x00000185
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニットの状態異常【バフ無効】")] UNIT_STATE_CONDITION_DISABLEBUFF = 390, // 0x00000186
      [GameParameter.ParameterDesc("ユニットの状態異常【デバフ無効】"), GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_STATE_CONDITION_DISABLEDEBUFF = 391, // 0x00000187
      [GameParameter.ParameterDesc("ターン表示のユニット陣営フレーム"), GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] TURN_UNIT_SIDE_FRAME = 392, // 0x00000188
      [GameParameter.ParameterDesc("JobSetの開放条件")] JOBSET_UNLOCKCONDITION = 393, // 0x00000189
      [GameParameter.ParameterDesc("マルチで自キャラ生存数が0のとき表示(0)/非表示(1)"), GameParameter.UsesIndex] MULTI_REST_MY_UNIT_IS_ZERO = 394, // 0x0000018A
      [GameParameter.AlwaysUpdate, GameParameter.UsesIndex, GameParameter.ParameterDesc("マルチ部屋画面で対象プレイヤーが自分のとき0:表示/1:非表示/2:ImageArrayのインデックス切り替え(0=自分 1=他人)/3:チーム編成ボタン/4:情報をみるボタン/5:チーム編成ボタン(マルチ塔)/6:プレイヤーがまだリザルト画面に存在するか")] MULTI_PLAYER_IS_ME = 395, // 0x0000018B
      [GameParameter.ParameterDesc("クエストリストで使用するセクション(部)の説明")] QUESTLIST_SECTIONEXPR = 396, // 0x0000018C
      [GameParameter.AlwaysUpdate, GameParameter.UsesIndex, GameParameter.ParameterDesc("マルチプレイの部屋に鍵がかかっているとき表示(0)/非表示(1)/部屋主かつ鍵ありで表示(2)/部屋主かつ鍵なしで表示(3)")] MULTI_CURRENT_ROOM_IS_LOCKED = 397, // 0x0000018D
      [GameParameter.ParameterDesc("メールの受け取り日時")] MAIL_GIFT_RECEIVE = 398, // 0x0000018E
      [GameParameter.ParameterDesc("クエストのタイムリミット")] QUEST_TIMELIMIT = 399, // 0x0000018F
      [GameParameter.ParameterDesc("ユニットの現在のチャージタイム")] UNIT_CHARGETIME = 400, // 0x00000190
      [GameParameter.ParameterDesc("ユニットのチャージタイム")] UNIT_CHARGETIMEMAX = 401, // 0x00000191
      [GameParameter.ParameterDesc("ギミックオブジェクトの説明文")] GIMMICK_DESCRIPTION = 402, // 0x00000192
      [GameParameter.ParameterDesc("プロダクトDesc 0:そのまま 1:前半 2:後半"), GameParameter.UsesIndex] PRODUCT_DESC = 403, // 0x00000193
      [GameParameter.ParameterDesc("プロダクト個数 (x10)")] PRODUCT_NUMX = 404, // 0x00000194
      [GameParameter.ParameterDesc("ユニットの器用さ (ver1.1以降で表示されます)"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_DEX = 405, // 0x00000195
      [GameParameter.InstanceTypes(typeof (GameParameter.ArtifactInstanceTypes)), GameParameter.UsesIndex, GameParameter.ParameterDesc("アーティファクトの名前")] ARTIFACT_NAME = 406, // 0x00000196
      [GameParameter.ParameterDesc("アーティファクトのフレーバーテキスト")] ARTIFACT_DESC = 407, // 0x00000197
      [GameParameter.ParameterDesc("アーティファクトのボーナス条件")] ARTIFACT_SPEC = 408, // 0x00000198
      [GameParameter.ParameterDesc("アーティファクトのレアリティ")] ARTIFACT_RARITY = 409, // 0x00000199
      [GameParameter.ParameterDesc("アーティファクトのレアリティ (最大)")] ARTIFACT_RARITYCAP = 410, // 0x0000019A
      [GameParameter.ParameterDesc("アーティファクトの所持数")] ARTIFACT_NUM = 411, // 0x0000019B
      [GameParameter.ParameterDesc("アーティファクトの売却額")] ARTIFACT_SELLPRICE = 412, // 0x0000019C
      [GameParameter.ParameterDesc("アプリのバンドルバージョン")] APPLICATION_VERSION = 413, // 0x0000019D
      [GameParameter.ParameterDesc("フレンドユニットの最大レベル")] SUPPORTER_UNITCAPPEDLEVELMAX = 414, // 0x0000019E
      [GameParameter.ParameterDesc("欠片ポイント")] GLOBAL_PLAYER_PIECEPOINT = 415, // 0x0000019F
      [GameParameter.ParameterDesc("ショップ欠片ポイント総売却価格")] SHOP_KAKERA_SELLPRICETOTAL = 416, // 0x000001A0
      [GameParameter.InstanceTypes(typeof (GameParameter.ItemInstanceTypes)), GameParameter.UsesIndex, GameParameter.ParameterDesc("魂の欠片の売却価格")] ITEM_KAKERA_PRICE = 417, // 0x000001A1
      [GameParameter.ParameterDesc("魂の欠片変換の選択数分の価格")] SHOP_ITEM_KAKERA_SELLPRICE = 418, // 0x000001A2
      [GameParameter.ParameterDesc("報酬に含まれるマルチコイン")] REWARD_MULTICOIN = 419, // 0x000001A3
      [GameParameter.ParameterDesc("報酬に含まれる欠片ポイント")] REWARD_KAKERACOIN = 420, // 0x000001A4
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("クエスト出撃条件 (0)改行表記、指定文字なし/(1)一行表記、指定文字付/(2)改行表記、指定文字付")] QUEST_UNIT_ENTRYCONDITION = 421, // 0x000001A5
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("クエスト出撃条件が設定されている場合に表示(0)/非表示(1)")] OBSOLETE_QUEST_IS_UNIT_ENTRYCONDITION = 422, // 0x000001A6
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("クエストにユニットが出撃可能な場合に表示(0)/非表示(1)"), GameParameter.AlwaysUpdate] QUEST_IS_UNIT_ENABLEENTRYCONDITION = 423, // 0x000001A7
      [GameParameter.ParameterDesc("キャラクタークエスト：エピソード解放条件")] QUEST_CHARACTER_MAINUNITCONDITION = 424, // 0x000001A8
      [GameParameter.ParameterDesc("キャラクタークエスト：話数")] QUEST_CHARACTER_EPISODENUM = 425, // 0x000001A9
      [GameParameter.ParameterDesc("キャラクタークエスト：エピソード名")] QUEST_CHARACTER_EPISODENAME = 426, // 0x000001AA
      [GameParameter.ParameterDesc("限定ショップアイテムの残り購入可能数を取得")] SHOP_LIMITED_ITEM_BUYAMOUNT = 427, // 0x000001AB
      [GameParameter.ParameterDesc("ユニットのIndexで指定したジョブがマスターしている場合に表示。Indexが-1の場合は選択中のジョブで判定。JobDataが直接設定されている場合はバインドされたJobDataで判定"), GameParameter.UsesIndex, GameParameter.AlwaysUpdate] UNIT_IS_JOBMASTER = 428, // 0x000001AC
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニット覚醒レベル"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_NEXTAWAKE = 429, // 0x000001AD
      [GameParameter.ParameterDesc("操作時間が延長された際に表示する秒数")] MULTIPLAY_ADD_INPUTTIME = 430, // 0x000001AE
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("ユニットの限界突破最大値に達している場合にIndex:0で非表示。Index:1で表示。"), GameParameter.AlwaysUpdate] UNIT_IS_AWAKEMAX = 431, // 0x000001AF
      [GameParameter.ParameterDesc("コンフィグでオートプレイ選択時場合にIndex:0で表示。Index:1で非表示。"), GameParameter.AlwaysUpdate, GameParameter.UsesIndex] CONFIG_IS_AUTOPLAY = 432, // 0x000001B0
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("フレンドがお気に入りなら表示(0)/非表示(1)"), GameParameter.UsesIndex] FRIEND_ISFAVORITE = 433, // 0x000001B1
      [GameParameter.ParameterDesc("キャラクタークエスト出撃条件(0)改行表記/(1)一行表記"), GameParameter.UsesIndex] QUEST_CHARACTER_ENTRYCONDITION = 434, // 0x000001B2
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("キャラクタークエスト出撃条件が設定されている場合に表示(0)/非表示(1)"), GameParameter.UsesIndex] QUEST_CHARACTER_IS_ENTRYCONDITION = 435, // 0x000001B3
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("キャラクタークエスト出撃条件のタイトル表示")] QUEST_CHARACTER_CONDITIONATTENTION = 436, // 0x000001B4
      [GameParameter.ParameterDesc("復帰したプレイヤーINDEX")] MULTIPLAY_RESUME_PLAYER_INDEX = 437, // 0x000001B5
      [GameParameter.ParameterDesc("復帰したプレイヤーがホストか？")] MULTIPLAY_RESUME_PLAYER_IS_HOST = 438, // 0x000001B6
      [GameParameter.ParameterDesc("復帰したが、他プレイヤーがいない")] MULTIPLAY_RESUME_BUT_NOT_PLAYER = 439, // 0x000001B7
      [GameParameter.ParameterDesc("ショップごとに保持数を表示するイベントコイン")] EVENTCOIN_SHOPTYPEICON = 440, // 0x000001B8
      [GameParameter.ParameterDesc("イベントコイン一覧のアイテム名")] EVENTCOIN_ITEMNAME = 441, // 0x000001B9
      [GameParameter.ParameterDesc("イベントコイン一覧の説明")] EVENTCOIN_MESSAGE = 442, // 0x000001BA
      [GameParameter.ParameterDesc("イベントコイン一覧の所持数")] EVENTCOIN_POSSESSION = 443, // 0x000001BB
      [GameParameter.ParameterDesc("ショップアイテムのイベントコイン別アイコン")] EVENTCOIN_PRICEICON = 444, // 0x000001BC
      [GameParameter.ParameterDesc("イベントショップアイテムの残り購入可能数を取得")] SHOP_EVENT_ITEM_BUYAMOUNT = 445, // 0x000001BD
      [GameParameter.ParameterDesc("イベント終了までの時間")] TROPHY_REMAININGTIME = 446, // 0x000001BE
      [GameParameter.ParameterDesc("お客様コードのみを表示"), GameParameter.UsesIndex] GLOBAL_PLAYER_OKYAKUSAMACODE2 = 447, // 0x000001BF
      [GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType)), GameParameter.ParameterDesc("対戦相手のユニット")] VERSUS_UNIT_IMAGE = 448, // 0x000001C0
      [GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType)), GameParameter.ParameterDesc("対戦相手の名前")] VERSUS_PLAYER_NAME = 449, // 0x000001C1
      [GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType)), GameParameter.ParameterDesc("対戦相手のレベル")] VERSUS_PLAYER_LEVEL = 450, // 0x000001C2
      [GameParameter.ParameterDesc("対戦相手の総合攻撃力"), GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType))] VERSUS_PLAYER_TOTALATK = 451, // 0x000001C3
      [GameParameter.ParameterDesc("対戦結果"), GameParameter.InstanceTypes(typeof (BattleCore.QuestResult))] VERSUS_RESULT = 452, // 0x000001C4
      [GameParameter.ParameterDesc("対戦ランク表示")] VERSUS_RANK = 453, // 0x000001C5
      [GameParameter.ParameterDesc("現在のランクポイントを表示")] VERSUS_RANK_POINT = 454, // 0x000001C6
      [GameParameter.ParameterDesc("ランクアップまでのポイントを表示")] VERSUS_RANK_NEXT_POINT = 455, // 0x000001C7
      [GameParameter.ParameterDesc("現在のランクのアイコン")] VERSUS_RANK_ICON = 456, // 0x000001C8
      [GameParameter.ParameterDesc("現在のランクのインデックス")] VERSUS_RANK_ICON_INDEX = 457, // 0x000001C9
      [GameParameter.ParameterDesc("部屋内プレイヤーのランクのアイコン")] VERSUS_ROOMPLAYER_RANK_ICON = 458, // 0x000001CA
      [GameParameter.ParameterDesc("部屋内プレイヤーのランクのインデックス")] VERSUS_ROOMPLAYER_RANK_ICON_INDEX = 459, // 0x000001CB
      [GameParameter.ParameterDesc("対戦マップのサムネイル")] VERSUS_MAP_THUMNAIL = 460, // 0x000001CC
      [GameParameter.ParameterDesc("マップ選択中のサムネイル")] VERSUS_MAP_THUMNAIL2 = 461, // 0x000001CD
      [GameParameter.ParameterDesc("マップ選択中のマップ名")] VERSUS_MAP_NAME = 462, // 0x000001CE
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("マップが選択されていれば表示")] VERSUS_MAP_SELECT = 463, // 0x000001CF
      [GameParameter.ParameterDesc("所有アリーナコイン")] SHOP_ARENA_COIN = 464, // 0x000001D0
      [GameParameter.ParameterDesc("所有マルチコイン")] SHOP_MULTI_COIN = 465, // 0x000001D1
      [GameParameter.ParameterDesc("プレイヤーの所持コイン (共通無償幻晶石)")] GLOBAL_PLAYER_COINCOM = 466, // 0x000001D2
      [GameParameter.ParameterDesc("プレイヤーの所持コイン (無償幻晶石　共通＆固有)")] GLOBAL_PLAYER_FREECOINSET = 467, // 0x000001D3
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("ユニット覚醒レベル2"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.UsesIndex] UNIT_AWAKE2 = 468, // 0x000001D4
      VS_ST = 999, // 0x000003E7
      [GameParameter.ParameterDesc("対戦の報酬タイプ")] VS_TOWER_REWARD_NAME = 1000, // 0x000003E8
      [GameParameter.ParameterDesc("シーズン報酬受け取り可能か？")] VS_TOWER_SEASON_RECEIPT = 1001, // 0x000003E9
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("PvPコインの枚数")] VS_COIN = 1002, // 0x000003EA
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("フリーマッチでのpvpコイン受け取り可能枚数")] VS_REMAIN_COIN = 1003, // 0x000003EB
      [GameParameter.ParameterDesc("タワーマッチ開催中か"), GameParameter.UsesIndex, GameParameter.AlwaysUpdate] VS_OPEN_TOWERMATCH = 1004, // 0x000003EC
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("対戦の種類を表示")] VS_QUEST_CATEGORY = 1005, // 0x000003ED
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("タワーマッチで連勝した際のボーナス表示")] VS_TOWER_WINBONUS = 1006, // 0x000003EE
      [GameParameter.AlwaysUpdate, GameParameter.UsesIndex, GameParameter.ParameterDesc("観戦時の表示・非表示対応")] VS_AUDIENCE_DISPLAY = 1007, // 0x000003EF
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("Index == 0:1P勝利 / Index == 1:2P勝利 時に表示する"), GameParameter.AlwaysUpdate] VS_AUDIENCE_WIN_PLAYER = 1008, // 0x000003F0
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("観戦時のときだけ表示する")] VS_AUDIENCE_ONLY_DISPLAY = 1009, // 0x000003F1
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("観戦している部屋の種類")] VS_AUDIENCE_TYPE = 1010, // 0x000003F2
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("現在の階層")] VS_TOWER_FLOOR = 1011, // 0x000003F3
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("タワーマッチのときだけ表示する")] VS_TOWER_ONLY_DISPLAY = 1012, // 0x000003F4
      [GameParameter.ParameterDesc("観戦者数"), GameParameter.AlwaysUpdate] VS_AUDIENCE_NUM = 1013, // 0x000003F5
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("CPU対戦の難易度")] VS_CPU_DIFFICULTY = 1014, // 0x000003F6
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("CPU対戦の名前")] VS_CPU_NAME = 1015, // 0x000003F7
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("CPU対戦のレベル")] VS_CPU_LV = 1016, // 0x000003F8
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("CPU対戦の攻撃力")] VS_CPU_TOTALATK = 1017, // 0x000003F9
      VS_ED = 1098, // 0x0000044A
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("武具コンディション")] ARTIFACT_ST = 1099, // 0x0000044B
      [GameParameter.ParameterDesc("武具コンディション"), GameParameter.AlwaysUpdate] ARTIFACT_EVOLUTION_COND = 1100, // 0x0000044C
      [GameParameter.ParameterDesc("武具がお気に入りなら表示(0)/非表示(1)"), GameParameter.AlwaysUpdate, GameParameter.UsesIndex] ARTIFACT_ISFAVORITE = 1101, // 0x0000044D
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("武具の進化後の★の数")] ARTIFACT_EVOLUTION_RARITY = 1102, // 0x0000044E
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("武具アイコン"), GameParameter.InstanceTypes(typeof (GameParameter.ArtifactInstanceTypes))] ARTIFACT_ICON = 1103, // 0x0000044F
      [GameParameter.InstanceTypes(typeof (GameParameter.ArtifactInstanceTypes)), GameParameter.UsesIndex, GameParameter.ParameterDesc("武具の種類にあわせたフレームをImageに設定します。")] ARTIFACT_FRAME = 1104, // 0x00000450
      [GameParameter.ParameterDesc("武具の個数"), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.ArtifactInstanceTypes))] ARTIFACT_AMOUNT = 1105, // 0x00000451
      ARTIFACT_ED = 1198, // 0x000004AE
      QUEST_UI_ST = 1199, // 0x000004AF
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("クエストコンティニュー不可が設定されている場合に表示(0)/非表示(1)"), GameParameter.AlwaysUpdate] QUEST_IS_MAP_NO_CONTINUE = 1200, // 0x000004B0
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("クエストダメージ制限が設定されている場合に表示(0)/非表示(1)"), GameParameter.UsesIndex] QUEST_IS_MAP_DAMAGE_LIMIT = 1201, // 0x000004B1
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("クエスト情報の詳細テキスト Loc/japanese/quest_info.txt参照"), GameParameter.UsesIndex] QUEST_UI_DETAIL_INFO = 1202, // 0x000004B2
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("クエスト出撃条件でチームが固定されていて、かつユニットが設定されている場合に表示(0)/非表示(1)"), GameParameter.UsesIndex] QUEST_IS_UNIT_ENTRYCONDITION_FORCE_AVAILABLEUNIT = 1203, // 0x000004B3
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("クエスト出撃条件で出撃ユニットが制限されていた場合に表示")] QUEST_IS_UNIT_ENTRYCONDITION_LIMIT = 1204, // 0x000004B4
      [GameParameter.ParameterDesc("クエスト出撃条件で出撃ユニットが固定されていた場合に表示"), GameParameter.AlwaysUpdate] QUEST_IS_UNIT_ENTRYCONDITION_FORCE = 1205, // 0x000004B5
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("クエストをクリアしていた場合にinteractable=true(0)/false(1)")] QUEST_IS_CLEARD_INTERACTABLE = 1206, // 0x000004B6
      [GameParameter.UsesIndex, GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("クエストユニット交代が許可されている場合に表示(0)/非表示(1)")] QUEST_IS_UNIT_CHANGE = 1207, // 0x000004B7
      QUEST_IS_ED = 1298, // 0x00000512
      TRICK_ST = 1299, // 0x00000513
      [GameParameter.ParameterDesc("特殊パネルの名称")] TRICK_NAME = 1300, // 0x00000514
      [GameParameter.ParameterDesc("特殊パネルの説明")] TRICK_DESC = 1301, // 0x00000515
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("特殊パネルUIアイコン")] TRICK_UI_ICON = 1302, // 0x00000516
      TRICK_ED = 1398, // 0x00000576
      BATTLE_UI_ST = 1399, // 0x00000577
      [GameParameter.ParameterDesc("オーダーユニットが詠唱中か？")] BATTLE_UI_IMG_IS_CAST_ORDER = 1400, // 0x00000578
      [GameParameter.ParameterDesc("スキルの使用回数")] SKILL_USE_COUNT = 1401, // 0x00000579
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.EnumParameterDesc("スキル属性にあわせてImageArrayを切り替えます。(属性がない場合は非表示)", typeof (EElement))] SKILL_ELEMENT = 1402, // 0x0000057A
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.EnumParameterDesc("スキル攻撃詳細区分にあわせてImageArrayを切り替えます。(攻撃詳細区分がない場合は非表示)", typeof (AttackDetailTypes))] SKILL_ATTACK_DETAIL = 1403, // 0x0000057B
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.EnumParameterDesc("スキル攻撃タイプにあわせてImageArrayを切り替えます。(攻撃タイプがない場合は非表示)", typeof (AttackTypes))] SKILL_ATTACK_TYPE = 1404, // 0x0000057C
      [GameParameter.ParameterDesc("天候が発動していれば表示(0)/非表示(1)")] BATTLE_UI_WTH_IS_ENABLE = 1405, // 0x0000057D
      [GameParameter.ParameterDesc("天候名を表示")] BATTLE_UI_WTH_NAME = 1406, // 0x0000057E
      [GameParameter.ParameterDesc("天候アイコンを表示")] BATTLE_UI_WTH_ICON = 1407, // 0x0000057F
      BATTLE_UI_ED = 1498, // 0x000005DA
      UNIT_EXTRA_PARAM_ST = 1499, // 0x000005DB
      [GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes)), GameParameter.ParameterDesc("ユニットの移動力")] UNIT_EXTRA_PARAM_MOVE = 1500, // 0x000005DC
      [GameParameter.ParameterDesc("ユニットのジャンプ力"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_EXTRA_PARAM_JUMP = 1501, // 0x000005DD
      UNIT_EXTRA_PARAM_ED = 1598, // 0x0000063E
      MULTI_UI_ST = 1599, // 0x0000063F
      [GameParameter.ParameterDesc("マルチのユニットレベル制限")] MULTI_UI_ROOM_LIMIT_UNITLV = 1600, // 0x00000640
      [GameParameter.ParameterDesc("クリアの有無")] MULTI_UI_ROOM_LIMIT_CLEAR = 1601, // 0x00000641
      [GameParameter.AlwaysUpdate, GameParameter.ParameterDesc("現在の操作プレイヤー名を表示 [0]:切断時グレー表示 [1]:思考中追加 [2]～のターン追加"), GameParameter.UsesIndex] MULTI_UI_CURRENT_PLAYER_NAME = 1602, // 0x00000642
      [GameParameter.ParameterDesc("マルチタワーでの総攻撃力")] MULTI_TOWER_UI_PARTY_TOTALATK = 1603, // 0x00000643
      [GameParameter.ParameterDesc("オーナーの状態"), GameParameter.AlwaysUpdate, GameParameter.InstanceTypes(typeof (JSON_MyPhotonPlayerParam.EState)), GameParameter.UsesIndex] MULTI_OWNER_STATE = 1604, // 0x00000644
      [GameParameter.ParameterDesc("マルチタワー階層")] MULTI_TOWER_FLOOR = 1605, // 0x00000645
      [GameParameter.ParameterDesc("リーダースキル有効なクエストか？")] MULTI_QUEST_IS_LEADERSKILL = 1606, // 0x00000646
      [GameParameter.ParameterDesc("マルチ部屋リストの階層")] MULTI_TOWER_ROOM_LIST_FLOOR = 1607, // 0x00000647
      [GameParameter.ParameterDesc("マルチ部屋リストのクリア済みか")] MULTI_TOWER_ROOM_LIST_ISCLEAR = 1608, // 0x00000648
      MULTI_UI_ED = 1698, // 0x000006A2
      QUEST_BONUSOBJECTIVE_ST = 1699, // 0x000006A3
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.EnumParameterDesc("プレイ中クエストのボーナス条件の達成状態にあわせてImageArrayを切り替えます(未達成/達成/全達成)。インデックスでボーナス条件の番号を指定してください。", typeof (QuestBonusObjectiveState))] QUEST_BONUSOBJECTIVE_STAR = 1700, // 0x000006A4
      [GameParameter.ParameterDesc("クエストミッションの達成個数を表示します。全てのミッションを達成していた場合、テキストの色が変わります。"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_BONUSOBJECTIVE_AMOUNT = 1701, // 0x000006A5
      [GameParameter.ParameterDesc("クエストミッションの最大個数を表示します。"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_BONUSOBJECTIVE_AMOUNTMAX = 1702, // 0x000006A6
      [GameParameter.ParameterDesc("クエストミッションの報酬の数を表示します。"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.UsesIndex] QUEST_BONUSOBJECTIVE_REWARD_NUM = 1703, // 0x000006A7
      [GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエストミッションの進捗を表示するタイプのミッションならActive、表示しないタイプなら非Active")] QUEST_BONUSOBJECTIVE_PROGRESS_BADGE = 1704, // 0x000006A8
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.UsesIndex, GameParameter.ParameterDesc("クエストミッションの進捗（現在）")] QUEST_BONUSOBJECTIVE_PROGRESS_CURRENT = 1705, // 0x000006A9
      [GameParameter.ParameterDesc("クエストミッションの進捗（目標値）"), GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_BONUSOBJECTIVE_PROGRESS_TARGET = 1706, // 0x000006AA
      [GameParameter.UsesIndex, GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("塔クエストの固定ミッションの星")] QUEST_TOWER_BONUSOBJECTIVE_FIXED_STATE = 1707, // 0x000006AB
      [GameParameter.ParameterDesc("塔クエストミッションの達成個数を表示します。全てのミッションを達成していた場合、テキストの色が変わります。"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_TOWER_BONUSOBJECTIVE_AMOUNT = 1708, // 0x000006AC
      [GameParameter.ParameterDesc("塔クエストミッションの最大個数を表示します。"), GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes))] QUEST_TOWER_BONUSOBJECTIVE_AMOUNTMAX = 1709, // 0x000006AD
      [GameParameter.InstanceTypes(typeof (GameParameter.QuestInstanceTypes)), GameParameter.ParameterDesc("クエストミッションの進捗（現在）\n▼色変更あり\n　達成可能：緑\n　達成不可：赤\n　達成済み：白"), GameParameter.UsesIndex] QUEST_TOWER_BONUSOBJECTIVE_PROGRESS_COLOR = 1710, // 0x000006AE
      QUEST_BONUSOBJECTIVE_ED = 1798, // 0x00000706
      ME_UI_ST = 1799, // 0x00000707
      [GameParameter.ParameterDesc("スキルのマップ(地形)効果説明")] ME_UI_SKILL_DESC = 1800, // 0x00000708
      [GameParameter.ParameterDesc("マップ(地形)効果名")] ME_UI_NAME = 1801, // 0x00000709
      [GameParameter.ParameterDesc("ジョブ特効説明")] JOB_DESC_CH = 1802, // 0x0000070A
      [GameParameter.ParameterDesc("ジョブ特効説明が設定されていれば表示(0)/非表示(1)")] IS_JOB_DESC_CH = 1803, // 0x0000070B
      [GameParameter.ParameterDesc("ジョブその他の効果説明")] JOB_DESC_OT = 1804, // 0x0000070C
      [GameParameter.ParameterDesc("ジョブその他の効果説明が設定されていれば表示(0)/非表示(1)")] IS_JOB_DESC_OT = 1805, // 0x0000070D
      [GameParameter.ParameterDesc("ユニットの現ジョブのMediumアイコン。UNIT_JOBICON2に参照不具合があるため新設"), GameParameter.InstanceTypes(typeof (GameParameter.UnitInstanceTypes))] UNIT_JOBICON2_BUGFIX = 1806, // 0x0000070E
      ME_UI_ED = 1898, // 0x0000076A
      FIRST_FRIEND_ST = 1899, // 0x0000076B
      [GameParameter.ParameterDesc("初フレンド成立人数上限")] FIRST_FRIEND_MAX = 1900, // 0x0000076C
      [GameParameter.ParameterDesc("初フレンド成立人数")] FIRST_FRIEND_COUNT = 1901, // 0x0000076D
      FIRST_FRIEND_ED = 1998, // 0x000007CE
      CHALLENGEMISSION_ST = 1999, // 0x000007CF
      [GameParameter.ParameterDesc("カテゴリ名からヘルプ画像を表示")] CHALLENGEMISSION_IMG_HELP = 2000, // 0x000007D0
      [GameParameter.ParameterDesc("カテゴリ名からタブ画像を表示")] CHALLENGEMISSION_IMG_BUTTON = 2001, // 0x000007D1
      [GameParameter.ParameterDesc("トロフィー名から報酬画像を表示")] CHALLENGEMISSION_IMG_REWARD = 2002, // 0x000007D2
      CHALLENGEMISSION_ED = 2098, // 0x00000832
      UNITPARAM_EXTRA_ST = 2099, // 0x00000833
      [GameParameter.ParameterDesc("ユニットパラメーターの錬成可能終了日時(Y/m/d H:i)")] UNITPARAM_EXTRA_UNLOCKLIMIT = 2100, // 0x00000834
      [GameParameter.ParameterDesc("訓練対戦のの総戦闘力"), GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType))] VERSUS_COM_TOTALSTATUS = 2101, // 0x00000835
      [GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType)), GameParameter.ParameterDesc("運営指定戦のリーダーユニット画像")] VERSUS_DRAFT_IMAGE = 2102, // 0x00000836
      [GameParameter.InstanceTypes(typeof (GameParameter.VersusDraftSlot)), GameParameter.ParameterDesc("運営指定戦のパーティユニットアイコン")] VERSUS_DRAFT_PARTY_IMAGE_ICON = 2103, // 0x00000837
      [GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType)), GameParameter.ParameterDesc("運営指定戦で先攻/後攻でGameObjectのOn/Off切り替え")] VERSUS_DRAFT_TURN = 2104, // 0x00000838
      [GameParameter.ParameterDesc("運営指定戦のパーティユニット画像"), GameParameter.InstanceTypes(typeof (GameParameter.VersusDraftSlot))] VERSUS_DRAFT_PARTY_IMAGE = 2105, // 0x00000839
      [GameParameter.ParameterDesc("運営指定戦のパーティ総攻撃力"), GameParameter.InstanceTypes(typeof (GameParameter.VersusPlayerInstanceType))] VERSUS_DRAFT_PARTY_TOTALATK = 2106, // 0x0000083A
      UNITPARAM_EXTRA_ED = 2198, // 0x00000896
      UNIT_TOBIRA_ST = 2199, // 0x00000897
      [GameParameter.ParameterDesc("真理開眼の開放に必要なゼニー")] UNIT_TOBIRA_UNLOCK_COST = 2200, // 0x00000898
      [GameParameter.ParameterDesc("真理開眼の開放に必要な素材の数")] UNIT_TOBIRA_UNLOCK_REQAMOUNT = 2201, // 0x00000899
      [GameParameter.ParameterDesc("真理開眼の開放に必要な素材の所持数")] UNIT_TOBIRA_UNLOCK_AMOUNT = 2202, // 0x0000089A
      [GameParameter.ParameterDesc("真理開眼の扉の強化に必要な素材の数")] UNIT_TOBIRA_ENHANCE_REQAMOUNT = 2203, // 0x0000089B
      [GameParameter.ParameterDesc("真理開眼の扉の強化に必要な素材の所持数")] UNIT_TOBIRA_ENHANCE_AMOUNT = 2204, // 0x0000089C
      [GameParameter.UsesIndex, GameParameter.ParameterDesc("真理開眼の扉のレベルを示すアイコン表示")] UNIT_TOBIRA_LEVEL_ICON = 2205, // 0x0000089D
      [GameParameter.ParameterDesc("真理開眼の扉のレベルを示すアイコン表示"), GameParameter.UsesIndex] UNIT_TOBIRA_LEVEL_ICON2 = 2206, // 0x0000089E
      UNIT_TOBIRA_ED = 2298, // 0x000008FA
      ITEM_ST = 2299, // 0x000008FB
      [GameParameter.ParameterDesc("アイテムのレアリティ")] ITEM_RARITY = 2300, // 0x000008FC
      [GameParameter.ParameterDesc("アイテムのフレーバーテキスト(フレーバーが無ければ説明)")] ITEM_FLAVORORDESC = 2301, // 0x000008FD
      ITEM_ED = 2398, // 0x0000095E
      GLOBAL_PLAYER_RANKMATCH_ST = 2399, // 0x0000095F
      [GameParameter.ParameterDesc("プレイヤーのランクマッチのランキング順位")] GLOBAL_PLAYER_RANKMATCH_RANK = 2400, // 0x00000960
      [GameParameter.ParameterDesc("プレイヤーのランクマッチのポイント")] GLOBAL_PLAYER_RANKMATCH_SCORE_CURRENT = 2401, // 0x00000961
      [GameParameter.ParameterDesc("プレイヤーの次のクラスまでに必要なポイント")] GLOBAL_PLAYER_RANKMATCH_SCORE_NEXT = 2402, // 0x00000962
      [GameParameter.ParameterDesc("プレイヤーのランクマッチ挑戦可能回数")] GLOBAL_PLAYER_RANKMATCH_BP = 2403, // 0x00000963
      [GameParameter.ParameterDesc("ランクマッチのクラスアイコン")] RANKMATCH_CLASS_ICON = 2404, // 0x00000964
      [GameParameter.ParameterDesc("ランクマッチのクラス名")] RANKMATCH_CLASS_NAME = 2405, // 0x00000965
      [GameParameter.ParameterDesc("ランクマッチのクラスの枠")] RANKMATCH_CLASS_FRAME = 2406, // 0x00000966
      [GameParameter.ParameterDesc("プレイヤーのランクマッチのクラスと同じなら表示")] RANKMATCH_PLAYER_ISCLASS = 2407, // 0x00000967
      [GameParameter.ParameterDesc("プレイヤーのランクマッチの順位(3位以上なら非表示になる)")] GLOBAL_PLAYER_RANKMATCH_RANK_IMAGESET = 2408, // 0x00000968
      [GameParameter.ParameterDesc("プレイヤーのランクマッチの順位アイコン(4位以下なら非表示になる)")] GLOBAL_PLAYER_RANKMATCH_RANK_ICON = 2409, // 0x00000969
      [GameParameter.ParameterDesc("プレイヤーのランクマッチパーティの総攻撃力")] PARTY_RANKMATCHTOTALATK = 2410, // 0x0000096A
      [GameParameter.ParameterDesc("ランクマッチのランキングの順位(3位以上なら非表示になる)")] RANKMATCH_RANKING_RANK_IMAGESET = 2411, // 0x0000096B
      [GameParameter.ParameterDesc("ランクマッチのランキングの順位アイコン(4位以下なら非表示になる)")] RANKMATCH_RANKING_RANK_ICON = 2412, // 0x0000096C
      [GameParameter.ParameterDesc("ランクマッチのランキングのクラスアイコン")] RANKMATCH_RANKING_CLASS = 2413, // 0x0000096D
      [GameParameter.ParameterDesc("ランクマッチのランキングのユーザー名")] RANKMATCH_RANKING_NAME = 2414, // 0x0000096E
      [GameParameter.ParameterDesc("ランクマッチのランキングのユーザーランク")] RANKMATCH_RANKING_LEVEL = 2415, // 0x0000096F
      [GameParameter.ParameterDesc("ランクマッチのランキングのスコア")] RANKMATCH_RANKING_SCORE = 2416, // 0x00000970
      [GameParameter.InstanceTypes(typeof (GameParameter.RankMatchPlayer)), GameParameter.ParameterDesc("プレイヤーのランクマッチのクラス")] GLOBAL_PLAYER_RANKMATCH_CLASS = 2417, // 0x00000971
      [GameParameter.ParameterDesc("ランクマッチのランキングの順位(3位以上なら非表示になる)")] RANKMATCH_RANKING_RANKREWARD_IMAGESET_BEGIN = 2418, // 0x00000972
      RANKMATCH_RANKING_RANKREWARD_IMAGESET_END = 2419, // 0x00000973
      [GameParameter.ParameterDesc("ランクマッチのランキングの順位アイコン(4位以下なら非表示になる)")] RANKMATCH_RANKING_RANKREWARD_ICON = 2420, // 0x00000974
      [GameParameter.ParameterDesc("ランクマッチの戦績の勝敗")] RANKMATCH_HISTORY_RESULT = 2421, // 0x00000975
      [GameParameter.ParameterDesc("ランクマッチの戦績の変動ポイント")] RANKMATCH_HISTORY_VALUE = 2422, // 0x00000976
      [GameParameter.ParameterDesc("ランクマッチの戦績のクラスアイコン")] RANKMATCH_HISTORY_CLASS = 2423, // 0x00000977
      [GameParameter.ParameterDesc("ランクマッチの戦績のユーザー名")] RANKMATCH_HISTORY_NAME = 2424, // 0x00000978
      [GameParameter.ParameterDesc("ランクマッチの戦績のユーザーランク")] RANKMATCH_HISTORY_LEVEL = 2425, // 0x00000979
      [GameParameter.ParameterDesc("ランクマッチの戦績のスコア")] RANKMATCH_HISTORY_SCORE = 2426, // 0x0000097A
      [GameParameter.ParameterDesc("ランクマッチのミッション名")] RANKMATCH_MISSION_NAME = 2427, // 0x0000097B
      [GameParameter.ParameterDesc("ランクマッチのミッションの必要回数")] RANKMATCH_MISSION_NEED = 2428, // 0x0000097C
      [GameParameter.ParameterDesc("ランクマッチのミッションの進行度")] RANKMATCH_MISSION_PROGRESS = 2429, // 0x0000097D
      [GameParameter.ParameterDesc("プレイヤーのランクマッチのクラス"), GameParameter.InstanceTypes(typeof (GameParameter.RankMatchPlayer))] GLOBAL_PLAYER_RANKMATCH_CLASSNAME = 2430, // 0x0000097E
      [GameParameter.ParameterDesc("プレイヤーのランクマッチの対戦回数")] GLOBAL_PLAYER_RANKMATCH_COUNT_TOTAL = 2431, // 0x0000097F
      [GameParameter.ParameterDesc("プレイヤーのランクマッチの対戦勝利数")] GLOBAL_PLAYER_RANKMATCH_COUNT_WIN = 2432, // 0x00000980
      [GameParameter.ParameterDesc("プレイヤーのランクマッチの対戦敗北数")] GLOBAL_PLAYER_RANKMATCH_COUNT_LOSE = 2433, // 0x00000981
      [GameParameter.ParameterDesc("プレイヤーのランクマッチのスコアゲージ")] GLOBAL_PLAYER_RANKMATCH_SCORE_GAUGE = 2434, // 0x00000982
      RANKMATCH_RANKING_RANKREWARD_IMAGESET_SPERATE = 2435, // 0x00000983
      [GameParameter.ParameterDesc("ランクマッチのミッションの報酬受取可能ならActiveに"), GameParameter.AlwaysUpdate] RANKMATCH_MISSION_COMPLETED_ACTIVE = 2436, // 0x00000984
      [GameParameter.ParameterDesc("ランクマッチのシーズン報酬ランキングの順位(3位以上なら非表示になる)")] RANKMATCH_RESULT_RANK_NUM = 2437, // 0x00000985
      [GameParameter.ParameterDesc("ランクマッチのシーズン報酬ランキングの順位アイコン(4位以下なら非表示になる)")] RANKMATCH_RESULT_RANK_ICON = 2438, // 0x00000986
      [GameParameter.ParameterDesc("ランクマッチのシーズン報酬ランキングのクラスアイコン")] RANKMATCH_RESULT_CLASS = 2439, // 0x00000987
      [GameParameter.ParameterDesc("ランクマッチのシーズン報酬ランキングのスコア")] RANKMATCH_RESULT_SCORE = 2440, // 0x00000988
      [GameParameter.ParameterDesc("対戦相手のランクマッチスコア")] VERSUS_PLAYER_RANKMATCHSCORE = 2441, // 0x00000989
      [GameParameter.ParameterDesc("ランクマッチのシーズン報酬ランキングのクラス名")] RANKMATCH_RESULT_CLASS_NAME = 2442, // 0x0000098A
      [GameParameter.ParameterDesc("ランクマッチのシーズン期間")] RANKMATCH_RESULT_PERIOD = 2443, // 0x0000098B
      [GameParameter.ParameterDesc("ランクマッチのタイトル")] RANKMATCH_TITLE = 2444, // 0x0000098C
      GLOBAL_PLAYER_RANKMATCH_ED = 2498, // 0x000009C2
      ABILITY_ST = 2499, // 0x000009C3
      [GameParameter.ParameterDesc("派生アビリティならActiveになる"), GameParameter.AlwaysUpdate] ABILITY_DERIVED_BADGE = 2500, // 0x000009C4
      [GameParameter.ParameterDesc("派生アビリティならテキストの色が変わる")] ABILITY_DERIVED_TEXTCOLOR = 2501, // 0x000009C5
      [GameParameter.ParameterDesc("アビリティの種類詳細によってタイトルが変わる（SpriteSheet）")] ABILITY_TITLE_DETAIL_TYPE = 2502, // 0x000009C6
      ABILITY_ED = 2598, // 0x00000A26
      SKILL_ST = 2599, // 0x00000A27
      [GameParameter.ParameterDesc("派生スキルならテキストの色が変わる")] SKILL_DERIVED_TEXTCOLOR = 2600, // 0x00000A28
      SKILL_ED = 2698, // 0x00000A8A
      ARTIFACT_SETEFFECT_ST = 2699, // 0x00000A8B
      [GameParameter.ParameterDesc("セット効果の発動条件のアイコン"), GameParameter.UsesIndex] ARTIFACT_SETEFFECT_TRIGGER_ICON = 2700, // 0x00000A8C
      ARTIFACT_SETEFFECT_ED = 2798, // 0x00000AEE
      CONCEPTCARD_ST = 2799, // 0x00000AEF
      [GameParameter.ParameterDesc("真理念装の限界突破回数を示すアイコン")] CONCEPTCARD_AWAKE = 2800, // 0x00000AF0
      CONCEPTCARD_ED = 2898, // 0x00000B52
      GAMEOBJECT_ST = 2899, // 0x00000B53
      [GameParameter.ParameterDesc("ゲームオブジェクトの表示/非表示")] GAMEOBJECT_ACTIVE = 2900, // 0x00000B54
      GAMEOBJECT_INACTIVE = 2901, // 0x00000B55
      GAMEOBJECT_ED = 2998, // 0x00000BB6
    }

    public class UsesIndex : Attribute
    {
    }

    public class ParameterDesc : Attribute
    {
      public ParameterDesc(string text)
      {
      }
    }

    public class AlwaysUpdate : Attribute
    {
    }

    public class EnumParameterDesc : GameParameter.ParameterDesc
    {
      public EnumParameterDesc(string text, System.Type type)
        : base(text)
      {
      }
    }

    public class InstanceTypes : Attribute
    {
      public InstanceTypes(System.Type instanceType)
      {
      }
    }
  }
}
