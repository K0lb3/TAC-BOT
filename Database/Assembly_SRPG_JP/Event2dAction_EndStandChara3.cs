﻿// Decompiled with JetBrains decompiler
// Type: SRPG.Event2dAction_EndStandChara3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using UnityEngine;

namespace SRPG
{
  [EventActionInfo("New/立ち絵2/立ち絵消去2(2D)", "表示されている立ち絵を消します", 5592405, 4473992)]
  public class Event2dAction_EndStandChara3 : EventAction
  {
    public float FadeTime = 1f;
    public string CharaID;
    public bool Async;
    private float mTimer;

    public override void OnActivate()
    {
      if (string.IsNullOrEmpty(this.CharaID))
      {
        for (int index = EventStandCharaController2.Instances.Count - 1; index >= 0; --index)
          EventStandCharaController2.Instances[index].Close(0.3f);
      }
      else
      {
        EventStandCharaController2 instances = EventStandCharaController2.FindInstances(this.CharaID);
        if (Object.op_Inequality((Object) instances, (Object) null))
          instances.Close(this.FadeTime);
      }
      this.mTimer = this.FadeTime;
      if (!this.Async)
        return;
      this.ActivateNext(true);
    }

    public override void Update()
    {
      this.mTimer -= Time.get_deltaTime();
      if ((double) this.mTimer > 0.0)
        return;
      if (!this.Async)
        this.ActivateNext();
      else
        this.enabled = false;
    }
  }
}
