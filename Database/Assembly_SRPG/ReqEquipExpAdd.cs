﻿// Decompiled with JetBrains decompiler
// Type: SRPG.ReqEquipExpAdd
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE644F5D-682F-4D6E-964D-A0DD77A288F7
// Assembly location: C:\Users\André\Desktop\Assembly-CSharp.dll

using System.Collections.Generic;

namespace SRPG
{
  public class ReqEquipExpAdd : WebAPI
  {
    public ReqEquipExpAdd(long iid, int slot, Dictionary<string, int> usedItems, Network.ResponseCallback response)
    {
      this.name = "unit/job/equip/enforce";
      this.body = "\"iid\":" + (object) iid + ",";
      ReqEquipExpAdd reqEquipExpAdd = this;
      reqEquipExpAdd.body = reqEquipExpAdd.body + "\"id_equip\":" + (object) slot + ",";
      this.body += "\"mats\":[";
      string str = string.Empty;
      using (Dictionary<string, int>.Enumerator enumerator = usedItems.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<string, int> current = enumerator.Current;
          str += "{";
          str = str + "\"iname\":\"" + current.Key + "\",";
          str = str + "\"num\":" + (object) current.Value;
          str += "},";
        }
      }
      if (str.Length > 0)
        str = str.Substring(0, str.Length - 1);
      this.body += str;
      this.body += "]";
      this.body = WebAPI.GetRequestString(this.body);
      this.callback = response;
    }
  }
}
