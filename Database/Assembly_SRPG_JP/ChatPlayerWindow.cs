﻿// Decompiled with JetBrains decompiler
// Type: SRPG.ChatPlayerWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using GR;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SRPG
{
  [FlowNode.Pin(1, "Add Block", FlowNode.PinTypes.Input, 1)]
  [FlowNode.Pin(11, "Output Remove Block", FlowNode.PinTypes.Output, 11)]
  [FlowNode.Pin(10, "Output Add Block", FlowNode.PinTypes.Output, 10)]
  [FlowNode.Pin(2, "Remove Block", FlowNode.PinTypes.Input, 2)]
  [FlowNode.Pin(0, "Refresh", FlowNode.PinTypes.Input, 0)]
  public class ChatPlayerWindow : MonoBehaviour, IFlowInterface
  {
    [SerializeField]
    private Text UserName;
    [SerializeField]
    private BitmapText UserLv;
    [SerializeField]
    private Text LastLogin;
    [SerializeField]
    private GameObject Add;
    [SerializeField]
    private GameObject Remove;
    [SerializeField]
    private GameObject FriendAdd;
    [SerializeField]
    private GameObject FriendRemove;
    [SerializeField]
    private GameObject Award;
    private ChatPlayerData mPlayer;

    public ChatPlayerWindow()
    {
      base.\u002Ector();
    }

    public ChatPlayerData Player
    {
      get
      {
        return this.mPlayer;
      }
      set
      {
        if (value == null)
          this.DummyUserData();
        else
          this.mPlayer = value;
      }
    }

    public void Activated(int pinID)
    {
      switch (pinID)
      {
        case 0:
          this.Refresh();
          break;
        case 1:
          FlowNode_GameObject.ActivateOutputLinks((Component) this, 10);
          break;
        case 2:
          FlowNode_GameObject.ActivateOutputLinks((Component) this, 11);
          break;
      }
    }

    private void Awake()
    {
      if (!UnityEngine.Object.op_Inequality((UnityEngine.Object) this.Award, (UnityEngine.Object) null))
        return;
      this.Award.SetActive(false);
    }

    private void Refresh()
    {
      if (this.mPlayer == null)
        return;
      GlobalVars.SelectedFriendID = this.mPlayer.fuid;
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.UserName, (UnityEngine.Object) null))
        this.UserName.set_text(this.mPlayer.name);
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.LastLogin, (UnityEngine.Object) null))
      {
        TimeSpan timeSpan = DateTime.Now - GameUtility.UnixtimeToLocalTime(this.mPlayer.lastlogin);
        int days = timeSpan.Days;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        if (days > 0)
          this.LastLogin.set_text(LocalizedText.Get("sys.LASTLOGIN_DAY", new object[1]
          {
            (object) days.ToString()
          }));
        else if (hours > 0)
          this.LastLogin.set_text(LocalizedText.Get("sys.LASTLOGIN_HOUR", new object[1]
          {
            (object) hours.ToString()
          }));
        else
          this.LastLogin.set_text(LocalizedText.Get("sys.LASTLOGIN_MINUTE", new object[1]
          {
            (object) minutes.ToString()
          }));
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.UserLv, (UnityEngine.Object) null))
        this.UserLv.text = this.mPlayer.lv.ToString();
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.Add, (UnityEngine.Object) null) && UnityEngine.Object.op_Inequality((UnityEngine.Object) this.Remove, (UnityEngine.Object) null))
      {
        if (FlowNode_Variable.Get("IsBlackList").Contains("1"))
        {
          this.Remove.SetActive(true);
          this.Add.SetActive(false);
        }
        else
        {
          this.Remove.SetActive(false);
          this.Add.SetActive(true);
        }
      }
      if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.FriendAdd, (UnityEngine.Object) null) && UnityEngine.Object.op_Inequality((UnityEngine.Object) this.FriendRemove, (UnityEngine.Object) null))
      {
        this.FriendRemove.SetActive(this.mPlayer.is_friend != (byte) 0);
        this.FriendAdd.SetActive(this.mPlayer.is_friend == (byte) 0);
        Button component = (Button) this.FriendRemove.GetComponent<Button>();
        if (UnityEngine.Object.op_Inequality((UnityEngine.Object) component, (UnityEngine.Object) null))
          ((Selectable) component).set_interactable(!this.mPlayer.IsFavorite);
      }
      UnitData unit = this.mPlayer.unit;
      if (unit != null)
        DataSource.Bind<UnitData>(((Component) this).get_gameObject(), unit);
      if (this.mPlayer != null)
      {
        DataSource.Bind<ChatPlayerData>(((Component) this).get_gameObject(), this.mPlayer);
        if (UnityEngine.Object.op_Inequality((UnityEngine.Object) this.Award, (UnityEngine.Object) null))
          this.Award.SetActive(true);
      }
      this.FriendAdd.SetActive(!this.mPlayer.IsFriend);
      this.FriendRemove.SetActive(this.mPlayer.IsFriend);
      GameParameter.UpdateAll(((Component) this).get_gameObject());
    }

    private void DummyUserData()
    {
      this.mPlayer = new ChatPlayerData();
      this.mPlayer.exp = 10000;
      this.mPlayer.name = "TestMan";
      this.mPlayer.lv = 10;
      this.mPlayer.lastlogin = 0L;
      this.mPlayer.unit = MonoSingleton<GameManager>.Instance.Player.Units[0];
    }
  }
}
