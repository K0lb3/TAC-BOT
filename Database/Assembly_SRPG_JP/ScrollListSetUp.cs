﻿// Decompiled with JetBrains decompiler
// Type: SRPG.ScrollListSetUp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using UnityEngine;

namespace SRPG
{
  public interface ScrollListSetUp
  {
    void OnSetUpItems();

    void OnUpdateItems(int idx, GameObject obj);
  }
}