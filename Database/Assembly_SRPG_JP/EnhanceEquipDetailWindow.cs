﻿// Decompiled with JetBrains decompiler
// Type: SRPG.EnhanceEquipDetailWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using GR;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SRPG
{
  [FlowNode.Pin(1, "Refresh", FlowNode.PinTypes.Input, 1)]
  [FlowNode.Pin(2, "Reset", FlowNode.PinTypes.Input, 2)]
  [FlowNode.Pin(100, "強化", FlowNode.PinTypes.Output, 100)]
  public class EnhanceEquipDetailWindow : SRPG_FixedList, IFlowInterface
  {
    public GameObject Unit;
    public List<Button> Equipments;
    public List<RawImage> EquipmentRawImages;
    public List<GameObject> EquipmentCursors;
    public Transform ParamUpLayoutParent;
    public GameObject ParamUpTemplate;
    public Transform ItemLayoutParent;
    public GameObject ItemTemplate;
    public GameObject EquipSelectParent;
    public GameObject SelectedParent;
    public GameObject EnhanceGaugeParent;
    public GameObject ExpUpTemplate;
    public Text TxtExpUpValue;
    public Button BtnJob;
    public Button BtnAdd;
    public Button BtnSub;
    public Button BtnEnhance;
    public Text TxtJob;
    public Text TxtCost;
    public Text TxtComment;
    public Text TxtDisableEnhanceOnGauge;
    private UnitData mUnit;
    private int mJobIndex;
    private GameObject mSelectedEquipItem;
    private EnhanceEquipData mEnhanceEquipData;
    private List<GameObject> mEnhanceParameters;
    private GameObject mSelectedMaterialItem;
    private List<EnhanceMaterial> mEnhanceMaterials;
    private List<EnhanceMaterial> mEnableEnhanceMaterials;
    private List<ItemData> mMaterialItems;

    public override RectTransform ListParent
    {
      get
      {
        if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ItemLayoutParent, (UnityEngine.Object) null))
          return (RectTransform) ((Component) this.ItemLayoutParent).GetComponent<RectTransform>();
        return (RectTransform) null;
      }
    }

    protected override void Start()
    {
      base.Start();
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) this.ItemTemplate, (UnityEngine.Object) null))
        return;
      List<ItemData> items = MonoSingleton<GameManager>.Instance.Player.Items;
      this.mMaterialItems = new List<ItemData>(items.Count);
      for (int index = 0; index < items.Count; ++index)
      {
        if (items[index].CheckEquipEnhanceMaterial() && !items[index].Param.is_valuables)
          this.mMaterialItems.Add(items[index]);
      }
      this.mMaterialItems.Sort((Comparison<ItemData>) ((src, dsc) =>
      {
        if (src.ItemType != dsc.ItemType)
        {
          if (src.ItemType == EItemType.ExpUpEquip)
            return -1;
          if (dsc.ItemType == EItemType.ExpUpEquip)
            return 1;
        }
        return dsc.Param.enhace_point - src.Param.enhace_point;
      }));
      this.mUnit = MonoSingleton<GameManager>.Instance.Player.FindUnitDataByUniqueID((long) GlobalVars.SelectedUnitUniqueID);
      this.mJobIndex = (int) GlobalVars.SelectedUnitJobIndex;
      this.mEnhanceEquipData = new EnhanceEquipData();
      this.mEnhanceMaterials = new List<EnhanceMaterial>(this.mMaterialItems.Count);
      this.mEnableEnhanceMaterials = new List<EnhanceMaterial>(this.mMaterialItems.Count);
      this.mEnhanceParameters = new List<GameObject>(5);
      this.mSelectedEquipItem = (GameObject) null;
      this.mSelectedMaterialItem = (GameObject) null;
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.ItemTemplate, (UnityEngine.Object) null) && this.ItemTemplate.get_activeInHierarchy())
      {
        this.ItemTemplate.get_transform().SetSiblingIndex(0);
        this.ItemTemplate.SetActive(false);
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.BtnJob, (UnityEngine.Object) null))
      {
        // ISSUE: method pointer
        ((UnityEvent) this.BtnJob.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnJobChange)));
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.BtnAdd, (UnityEngine.Object) null))
      {
        // ISSUE: method pointer
        ((UnityEvent) this.BtnAdd.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnAddMaterial)));
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.BtnSub, (UnityEngine.Object) null))
      {
        // ISSUE: method pointer
        ((UnityEvent) this.BtnSub.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnSubMaterial)));
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.BtnEnhance, (UnityEngine.Object) null))
      {
        // ISSUE: method pointer
        ((UnityEvent) this.BtnEnhance.get_onClick()).AddListener(new UnityAction((object) this, __methodptr(OnEnhance)));
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtComment, (UnityEngine.Object) null))
        this.TxtComment.set_text(LocalizedText.Get("sys.ENHANCE_EQUIPMENT_MESSAGE"));
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtDisableEnhanceOnGauge, (UnityEngine.Object) null))
        this.TxtDisableEnhanceOnGauge.set_text(LocalizedText.Get("sys.DIABLE_ENHANCE_MESSAGE"));
      for (int index = 0; index < this.Equipments.Count; ++index)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: method pointer
        ((UnityEvent) this.Equipments[index].get_onClick()).AddListener(new UnityAction((object) new EnhanceEquipDetailWindow.\u003CStart\u003Ec__AnonStorey32C()
        {
          \u003C\u003Ef__this = this,
          slot = index
        }, __methodptr(\u003C\u003Em__2F4)));
      }
      this.RefreshData();
    }

    public void Activated(int pinID)
    {
      switch (pinID)
      {
        case 1:
          this.mUnit = MonoSingleton<GameManager>.Instance.Player.FindUnitDataByUniqueID((long) GlobalVars.SelectedUnitUniqueID);
          this.mJobIndex = (int) GlobalVars.SelectedUnitJobIndex;
          this.mSelectedMaterialItem = (GameObject) null;
          this.ClearEnhancedMaterial();
          if (!this.HasStarted)
            break;
          this.RefreshData();
          break;
        case 2:
          this.mUnit = MonoSingleton<GameManager>.Instance.Player.FindUnitDataByUniqueID((long) GlobalVars.SelectedUnitUniqueID);
          this.mJobIndex = (int) GlobalVars.SelectedUnitJobIndex;
          this.mSelectedEquipItem = (GameObject) null;
          this.mSelectedMaterialItem = (GameObject) null;
          this.ClearEnhancedMaterial();
          if (!this.HasStarted)
            break;
          this.RefreshData();
          break;
      }
    }

    protected override void Update()
    {
      base.Update();
    }

    private void RefreshData()
    {
      JobData jobData = this.mUnit.GetJobData(this.mJobIndex);
      DataSource.Bind<UnitData>(this.Unit, this.mUnit);
      for (int index = 0; index < this.Equipments.Count; ++index)
      {
        EquipData equip = jobData.Equips[index];
        DataSource.Bind<EquipData>(((Component) this.Equipments[index]).get_gameObject(), equip);
        bool flag = equip != null && (equip.IsValid() && equip.IsEquiped());
        ((Selectable) this.Equipments[index]).set_interactable(flag);
        ((Component) this.EquipmentRawImages[index]).get_gameObject().SetActive(flag);
        this.EquipmentCursors[index].SetActive(UnityEngine.Object.op_Equality((UnityEngine.Object) this.mSelectedEquipItem, (UnityEngine.Object) ((Component) this.Equipments[index]).get_gameObject()));
      }
      this.mEnhanceEquipData.equip = (EquipData) null;
      this.mEnhanceEquipData.gainexp = 0;
      this.mEnhanceEquipData.is_enhanced = false;
      for (int index = 0; index < this.mEnhanceParameters.Count; ++index)
        this.mEnhanceParameters[index].SetActive(false);
      ((Selectable) this.BtnEnhance).set_interactable(false);
      EquipData equipData = !UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSelectedEquipItem, (UnityEngine.Object) null) ? (EquipData) null : DataSource.FindDataOfClass<EquipData>(this.mSelectedEquipItem, (EquipData) null);
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < this.mMaterialItems.Count; ++index)
      {
        ItemData item = this.mMaterialItems[index];
        EnhanceMaterial enhanceMaterial1 = this.mEnhanceMaterials.Find((Predicate<EnhanceMaterial>) (p => p.item == item));
        if (enhanceMaterial1 == null)
        {
          EnhanceMaterial enhanceMaterial2 = new EnhanceMaterial();
          enhanceMaterial2.item = item;
          enhanceMaterial2.num = 0;
          this.mEnhanceMaterials.Add(enhanceMaterial2);
          enhanceMaterial1 = enhanceMaterial2;
        }
        if (equipData != null)
        {
          num1 += item.Param.enhace_cost * equipData.GetEnhanceCostScale() / 100 * enhanceMaterial1.num;
          num2 += item.Param.enhace_point * enhanceMaterial1.num;
        }
      }
      bool flag1 = false;
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.EquipSelectParent, (UnityEngine.Object) null))
      {
        DataSource.Bind<EnhanceEquipData>(this.EquipSelectParent, (EnhanceEquipData) null);
        this.EquipSelectParent.get_gameObject().SetActive(false);
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtComment, (UnityEngine.Object) null))
        ((Component) this.TxtComment).get_gameObject().SetActive(false);
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSelectedEquipItem, (UnityEngine.Object) null))
      {
        this.mEnhanceEquipData.equip = equipData;
        this.mEnhanceEquipData.gainexp = num2;
        if (equipData != null)
        {
          BuffEffect buffEffect = equipData.Skill.GetBuffEffect(SkillEffectTargets.Target);
          if (buffEffect != null && buffEffect.targets != null)
          {
            for (int index = 0; index < buffEffect.targets.Count; ++index)
            {
              if (index >= this.mEnhanceParameters.Count)
              {
                this.ParamUpTemplate.SetActive(true);
                GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate<GameObject>((M0) this.ParamUpTemplate);
                gameObject.get_transform().SetParent(this.ParamUpLayoutParent, false);
                this.mEnhanceParameters.Add(gameObject);
                this.ParamUpTemplate.SetActive(false);
              }
              GameObject enhanceParameter = this.mEnhanceParameters[index];
              EquipItemParameter data = DataSource.FindDataOfClass<EquipItemParameter>(enhanceParameter, (EquipItemParameter) null) ?? new EquipItemParameter();
              data.equip = equipData;
              data.param_index = index;
              DataSource.Bind<EquipItemParameter>(enhanceParameter, data);
              enhanceParameter.SetActive(true);
            }
          }
          flag1 = true;
          if (equipData.Rank == equipData.GetRankCap())
          {
            if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtComment, (UnityEngine.Object) null))
            {
              this.TxtComment.set_text(LocalizedText.Get("sys.DIABLE_ENHANCE_RANKCAP_MESSAGE"));
              ((Component) this.TxtComment).get_gameObject().SetActive(true);
            }
            flag1 = false;
          }
        }
        if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.EquipSelectParent, (UnityEngine.Object) null))
        {
          DataSource.Bind<EnhanceEquipData>(this.EquipSelectParent, this.mEnhanceEquipData);
          this.EquipSelectParent.get_gameObject().SetActive(true);
        }
      }
      else if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtComment, (UnityEngine.Object) null))
      {
        this.TxtComment.set_text(LocalizedText.Get("sys.ENHANCE_EQUIPMENT_MESSAGE"));
        ((Component) this.TxtComment).get_gameObject().SetActive(true);
      }
      this.mEnhanceEquipData.is_enhanced = flag1;
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.SelectedParent, (UnityEngine.Object) null))
      {
        DataSource.Bind<EnhanceEquipData>(this.SelectedParent, this.mEnhanceEquipData);
        this.SelectedParent.get_gameObject().SetActive(flag1);
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtDisableEnhanceOnGauge, (UnityEngine.Object) null))
        ((Component) this.TxtDisableEnhanceOnGauge).get_gameObject().SetActive(!flag1);
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtCost, (UnityEngine.Object) null))
        this.TxtCost.set_text(num1.ToString());
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.TxtJob, (UnityEngine.Object) null))
        this.TxtJob.set_text(jobData.Name);
      if (flag1)
      {
        int num3 = equipData.CalcRankFromExp(equipData.Exp + num2);
        ((Selectable) this.BtnEnhance).set_interactable(equipData != null && equipData.Rank < num3);
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.BtnAdd, (UnityEngine.Object) null))
        ((Selectable) this.BtnAdd).set_interactable(flag1 && UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSelectedMaterialItem, (UnityEngine.Object) null));
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.BtnSub, (UnityEngine.Object) null))
        ((Selectable) this.BtnSub).set_interactable(flag1 && UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSelectedMaterialItem, (UnityEngine.Object) null));
      this.mEnableEnhanceMaterials.Clear();
      for (int index = 0; index < this.mEnhanceMaterials.Count; ++index)
      {
        if (this.mEnhanceMaterials[index].item != null && this.mEnhanceMaterials[index].item.Num != 0)
          this.mEnableEnhanceMaterials.Add(this.mEnhanceMaterials[index]);
      }
      this.SetData((object[]) this.mEnableEnhanceMaterials.ToArray(), typeof (EnhanceMaterial));
      GameParameter.UpdateAll(((Component) this).get_gameObject());
    }

    protected override GameObject CreateItem()
    {
      return (GameObject) UnityEngine.Object.Instantiate<GameObject>((M0) this.ItemTemplate);
    }

    protected override void OnUpdateItem(GameObject go, int index)
    {
    }

    private void OnJobChange()
    {
      int mJobIndex = this.mJobIndex;
      do
      {
        this.mJobIndex = ++this.mJobIndex % this.mUnit.JobCount;
      }
      while (!this.mUnit.GetJobData(this.mJobIndex).IsActivated);
      if (mJobIndex == this.mJobIndex)
        return;
      this.mSelectedEquipItem = (GameObject) null;
      this.mSelectedMaterialItem = (GameObject) null;
      this.ClearEnhancedMaterial();
      this.RefreshData();
    }

    private void OnSelectEquipment(int slot)
    {
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) this.mSelectedEquipItem, (UnityEngine.Object) ((Component) this.Equipments[slot]).get_gameObject()))
        return;
      this.ClearEnhancedMaterial();
      this.ClearMaterialSelect();
      this.mSelectedEquipItem = ((Component) this.Equipments[slot]).get_gameObject();
      GlobalVars.SelectedEquipmentSlot.Set(slot);
      this.RefreshData();
    }

    protected override void OnItemSelect(GameObject go)
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.mSelectedMaterialItem, (UnityEngine.Object) go))
        return;
      this.mSelectedMaterialItem = go;
      EnhanceMaterial dataOfClass = DataSource.FindDataOfClass<EnhanceMaterial>(this.mSelectedMaterialItem, (EnhanceMaterial) null);
      if (dataOfClass != null)
      {
        for (int index = 0; index < this.mEnhanceMaterials.Count; ++index)
          this.mEnhanceMaterials[index].selected = dataOfClass == this.mEnhanceMaterials[index];
      }
      this.RefreshData();
    }

    private void OnAddMaterial()
    {
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) this.mSelectedMaterialItem, (UnityEngine.Object) null))
        return;
      EnhanceMaterial dataOfClass = DataSource.FindDataOfClass<EnhanceMaterial>(this.mSelectedMaterialItem, (EnhanceMaterial) null);
      if (dataOfClass != null)
      {
        if (!this.CheckEquipItemEnhance())
        {
          UIUtility.NegativeSystemMessage(LocalizedText.Get("sys.FAILED_ENHANCE"), LocalizedText.Get("sys.DIABLE_ENHANCE_RANKCAP_MESSAGE"), (UIUtility.DialogResultEvent) null, (GameObject) null, false, -1);
          return;
        }
        dataOfClass.num = Math.Min(++dataOfClass.num, dataOfClass.item.Num);
        DataSource.Bind<EnhanceMaterial>(this.mSelectedMaterialItem, dataOfClass);
      }
      this.RefreshData();
    }

    private void OnSubMaterial()
    {
      if (UnityEngine.Object.op_Equality((UnityEngine.Object) this.mSelectedMaterialItem, (UnityEngine.Object) null))
        return;
      EnhanceMaterial dataOfClass = DataSource.FindDataOfClass<EnhanceMaterial>(this.mSelectedMaterialItem, (EnhanceMaterial) null);
      if (dataOfClass != null)
      {
        dataOfClass.num = Math.Max(--dataOfClass.num, 0);
        DataSource.Bind<EnhanceMaterial>(this.mSelectedMaterialItem, dataOfClass);
      }
      this.RefreshData();
    }

    private bool CheckEquipItemEnhance()
    {
      if (this.mEnhanceEquipData == null || this.mEnhanceEquipData.equip == null)
        return false;
      EquipData equip = this.mEnhanceEquipData.equip;
      int current = equip.Exp + this.mEnhanceEquipData.gainexp;
      return equip.CalcRankFromExp(current) < equip.GetRankCap();
    }

    private void OnEnhance()
    {
      bool flag = false;
      for (int index = 0; index < this.mEnhanceMaterials.Count; ++index)
      {
        EnhanceMaterial mEnhanceMaterial = this.mEnhanceMaterials[index];
        if (mEnhanceMaterial.num != 0 && (mEnhanceMaterial.item.Rarity > 1 || mEnhanceMaterial.item.ItemType == EItemType.Material))
          flag = true;
      }
      GlobalVars.SelectedUnitUniqueID.Set(this.mUnit.UniqueID);
      GlobalVars.SelectedUnitJobIndex.Set(this.mJobIndex);
      GlobalVars.SelectedEquipData = DataSource.FindDataOfClass<EquipData>(this.mSelectedEquipItem, (EquipData) null);
      GlobalVars.SelectedEnhanceMaterials = this.mEnhanceMaterials;
      if (flag)
        UIUtility.ConfirmBox(LocalizedText.Get("sys.ENHANCE_ITEM_RARITY_CAUTION"), (string) null, new UIUtility.DialogResultEvent(this.OnDecide), new UIUtility.DialogResultEvent(this.OnCancel), (GameObject) null, false, -1);
      else
        FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
    }

    private void OnDecide(GameObject go)
    {
      FlowNode_GameObject.ActivateOutputLinks((Component) this, 100);
    }

    private void OnCancel(GameObject go)
    {
      GlobalVars.SelectedEquipData = (EquipData) null;
      GlobalVars.SelectedEnhanceMaterials = (List<EnhanceMaterial>) null;
    }

    private void ClearEnhancedMaterial()
    {
      for (int index = 0; index < this.mEnhanceMaterials.Count; ++index)
      {
        this.mEnhanceMaterials[index].num = 0;
        this.mEnhanceMaterials[index].selected = false;
      }
    }

    public override void GotoPreviousPage()
    {
      this.ClearMaterialSelect();
      this.RefreshData();
      base.GotoPreviousPage();
    }

    public override void GotoNextPage()
    {
      this.ClearMaterialSelect();
      this.RefreshData();
      base.GotoNextPage();
    }

    public void ClearMaterialSelect()
    {
      this.mSelectedMaterialItem = (GameObject) null;
      for (int index = 0; index < this.mEnhanceMaterials.Count; ++index)
        this.mEnhanceMaterials[index].selected = false;
    }
  }
}
