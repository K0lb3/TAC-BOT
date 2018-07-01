﻿// Decompiled with JetBrains decompiler
// Type: SRPG.GimmickEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE644F5D-682F-4D6E-964D-A0DD77A288F7
// Assembly location: C:\Users\André\Desktop\Assembly-CSharp.dll

using System.Collections.Generic;

namespace SRPG
{
  public class GimmickEvent
  {
    public List<string> skills = new List<string>();
    public List<Unit> users = new List<Unit>();
    public List<Unit> targets = new List<Unit>();
    public List<TrickData> td_targets = new List<TrickData>();
    public GimmickEventCondition condition = new GimmickEventCondition();
    public eGimmickEventType ev_type;
    public string td_iname;
    public string td_tag;
    public int count;
    public bool IsCompleted;
    public bool IsStarter;
    public Unit starter;
  }
}
