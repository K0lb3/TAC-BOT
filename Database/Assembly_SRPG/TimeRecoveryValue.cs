﻿// Decompiled with JetBrains decompiler
// Type: SRPG.TimeRecoveryValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE644F5D-682F-4D6E-964D-A0DD77A288F7
// Assembly location: C:\Users\André\Desktop\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace SRPG
{
  public class TimeRecoveryValue
  {
    private float lastUpdateTime = -1f;
    public OInt val;
    public OInt valMax;
    public OInt valRecover;
    public OLong interval;
    public OLong at;

    public void Update()
    {
      if ((int) this.val >= (int) this.valMax || (double) this.lastUpdateTime == (double) Time.get_realtimeSinceStartup())
        return;
      this.lastUpdateTime = Time.get_realtimeSinceStartup();
      long num1 = Network.GetServerTime() - (long) this.at;
      long at = (long) this.at;
      long interval = (long) this.interval;
      int num2 = (int) (num1 / interval);
      this.at = (OLong) (at + (long) num2 * interval);
      this.val = (OInt) Math.Min((int) this.val + num2, (int) this.valMax);
    }

    public long GetNextRecoverySec()
    {
      if ((int) this.val >= (int) this.valMax)
        return 0;
      long num1 = Network.GetServerTime() - (long) this.at;
      int num2 = (int) (num1 / (long) this.interval);
      return (long) this.interval - (num1 - (long) this.interval * (long) num2);
    }

    public void SubValue(int subval)
    {
      this.at = (OLong) Network.GetServerTime();
      this.val = (OInt) Math.Max((int) this.val - subval, 0);
    }
  }
}
