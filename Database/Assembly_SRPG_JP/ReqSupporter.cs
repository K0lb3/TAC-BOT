﻿// Decompiled with JetBrains decompiler
// Type: SRPG.ReqSupporter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

namespace SRPG
{
  public class ReqSupporter : WebAPI
  {
    public ReqSupporter(Network.ResponseCallback response)
    {
      this.name = "btl/com/supportlist";
      this.body = WebAPI.GetRequestString((string) null);
      this.callback = response;
    }
  }
}
