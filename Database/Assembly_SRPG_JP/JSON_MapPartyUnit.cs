﻿// Decompiled with JetBrains decompiler
// Type: SRPG.JSON_MapPartyUnit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

namespace SRPG
{
  public class JSON_MapPartyUnit
  {
    public string name;
    public string ai;
    public int x;
    public int y;
    public int dir;
    public int wait_e;
    public int wait_m;
    public int wait_exit;
    public int ct_calc;
    public int ct_val;
    public int fvoff;
    public int ai_type;
    public int ai_x;
    public int ai_y;
    public int ai_len;
    public string parent;
    public JSON_EventTrigger trg;
    public UnitEntryTrigger[] entries;
    public int entries_and;
  }
}
