﻿// Decompiled with JetBrains decompiler
// Type: SRPG.FlowNode_ReqUpdateSelectAward
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using GR;
using UnityEngine;

namespace SRPG
{
  [FlowNode.Pin(10, "Success", FlowNode.PinTypes.Output, 10)]
  [FlowNode.NodeType("System/ReqUpdateSelectAward", 32741)]
  [FlowNode.Pin(0, "Request", FlowNode.PinTypes.Input, 0)]
  [FlowNode.Pin(11, "Failure", FlowNode.PinTypes.Output, 11)]
  public class FlowNode_ReqUpdateSelectAward : FlowNode_Network
  {
    public override void OnActivate(int pinID)
    {
      if (pinID != 0)
        return;
      string iname = FlowNode_Variable.Get("CONFIRM_SELECT_AWARD");
      if (MonoSingleton<GameManager>.GetInstanceDirect().Player.SelectedAward != iname)
      {
        this.ExecRequest((WebAPI) new ReqUpdateSelectAward(iname, new Network.ResponseCallback(((FlowNode_Network) this).ResponseCallback)));
        ((Behaviour) this).set_enabled(true);
      }
      else
        this.Success();
    }

    private void Success()
    {
      ((Behaviour) this).set_enabled(false);
      this.ActivateOutputLinks(10);
    }

    private void Failure()
    {
      ((Behaviour) this).set_enabled(false);
    }

    public override void OnSuccess(WWWResult www)
    {
      if (Network.IsError)
      {
        Network.EErrCode errCode = Network.ErrCode;
      }
      WebAPI.JSON_BodyResponse<Json_ResSelectAward> jsonObject = JSONParser.parseJSONObject<WebAPI.JSON_BodyResponse<Json_ResSelectAward>>(www.text);
      DebugUtility.Assert(jsonObject != null, "res == null");
      Network.RemoveAPI();
      if (jsonObject == null)
      {
        this.Failure();
      }
      else
      {
        MonoSingleton<GameManager>.Instance.Player.SelectedAward = jsonObject.body.selected_award;
        this.Success();
      }
    }
  }
}
