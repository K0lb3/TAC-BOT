﻿// Decompiled with JetBrains decompiler
// Type: SRPG.JSON_WeatherParam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE644F5D-682F-4D6E-964D-A0DD77A288F7
// Assembly location: C:\Users\André\Desktop\Assembly-CSharp.dll

using System;

namespace SRPG
{
  [Serializable]
  public class JSON_WeatherParam
  {
    public string iname;
    public string name;
    public string expr;
    public string icon;
    public string effect;
    public string[] buff_ids;
    public string[] cond_ids;
  }
}
