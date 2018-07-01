﻿// Decompiled with JetBrains decompiler
// Type: SRPG.EventAction_WaitAsyncActions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using UnityEngine;

namespace SRPG
{
  [EventActionInfo("同期", "非同期処理が完了するのを待ちます", 5592405, 4473992)]
  public class EventAction_WaitAsyncActions : EventAction
  {
    public override void OnActivate()
    {
    }

    public override void Update()
    {
      for (int index = 0; index < this.Sequence.Actions.Length && !Object.op_Equality((Object) this.Sequence.Actions[index], (Object) this); ++index)
      {
        if (this.Sequence.Actions[index].enabled)
          return;
      }
      this.ActivateNext();
    }
  }
}
