﻿// Decompiled with JetBrains decompiler
// Type: SRPG.EventAction_MoveCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85BFDF7F-5712-4D45-9CD6-3465C703DFDF
// Assembly location: S:\Desktop\Assembly-CSharp.dll

using UnityEngine;

namespace SRPG
{
  [EventActionInfo("カメラ/移動", "カメラを移動します。", 4482628, 4500036)]
  public class EventAction_MoveCamera : EventAction
  {
    [HideInInspector]
    public float StartCameraDistanceScale = 1f;
    [HideInInspector]
    public float CameraDistanceScale = 1f;
    [HideInInspector]
    public float MoveTime = -1f;
    private const float SmoothingMargin = 0.5f;
    public EventAction_MoveCamera.CameraMoveModes MoveMode;
    [HideInInspector]
    public string ActorID;
    [HideInInspector]
    public Vector3 StartTargetPosition;
    [HideInInspector]
    public float StartRotation;
    [HideInInspector]
    public float StartCameraDistance;
    [HideInInspector]
    public EventAction_MoveCamera.CameraMoveSpeeds Speed;
    [HideInInspector]
    public Vector3 TargetPosition;
    [HideInInspector]
    public float TargetRotation;
    private float mCurrentTime;
    private float mEndTime;
    [HideInInspector]
    public float CameraDistance;
    [HideInInspector]
    public bool Async;
    [HideInInspector]
    public bool SnapToGround;
    [HideInInspector]
    public EventAction_MoveCamera.CameraFadeModes FadeMode;
    [HideInInspector]
    public float FadeTime;

    public override void OnActivate()
    {
      GameSettings instance = GameSettings.Instance;
      if ((double) this.MoveTime <= 0.0 && this.Speed == EventAction_MoveCamera.CameraMoveSpeeds.Immediate)
      {
        EventAction_MoveCamera actionMoveCamera = this;
        actionMoveCamera.TargetPosition = Vector3.op_Addition(actionMoveCamera.TargetPosition, Vector3.op_Multiply(Vector3.get_up(), instance.GameCamera_UnitHeightOffset));
        this.UpdateCameraPosition(1f);
        this.ActivateNext();
      }
      else
      {
        TargetCamera targetCamera = TargetCamera.Get((Component) Camera.get_main());
        targetCamera.Reset();
        if (this.MoveMode == EventAction_MoveCamera.CameraMoveModes.InterpolateToPoint)
        {
          this.StartTargetPosition = targetCamera.TargetPosition;
          this.StartRotation = targetCamera.Yaw;
          this.StartCameraDistanceScale = targetCamera.CameraDistance / GameSettings.Instance.GameCamera_DefaultDistance;
        }
        else if (this.MoveMode == EventAction_MoveCamera.CameraMoveModes.SpecificActor)
        {
          this.StartRotation = targetCamera.Yaw;
          this.StartTargetPosition = targetCamera.TargetPosition;
          this.StartCameraDistanceScale = targetCamera.CameraDistance / GameSettings.Instance.GameCamera_DefaultDistance;
          GameObject actor = EventAction.FindActor(this.ActorID);
          if (Object.op_Equality((Object) actor, (Object) null))
          {
            this.ActivateNext();
            return;
          }
          this.TargetPosition = actor.get_transform().get_position();
          if ((double) this.TargetRotation < 0.0)
            this.TargetRotation = this.StartRotation;
        }
        else
        {
          if (this.SnapToGround)
            this.StartTargetPosition = GameUtility.RaycastGround(this.StartTargetPosition);
          EventAction_MoveCamera actionMoveCamera = this;
          actionMoveCamera.StartTargetPosition = Vector3.op_Addition(actionMoveCamera.StartTargetPosition, Vector3.op_Multiply(Vector3.get_up(), instance.GameCamera_UnitHeightOffset));
        }
        if (this.SnapToGround)
          this.TargetPosition = GameUtility.RaycastGround(this.TargetPosition);
        EventAction_MoveCamera actionMoveCamera1 = this;
        actionMoveCamera1.TargetPosition = Vector3.op_Addition(actionMoveCamera1.TargetPosition, Vector3.op_Multiply(Vector3.get_up(), instance.GameCamera_UnitHeightOffset));
        float num1 = 1f;
        switch (this.Speed)
        {
          case EventAction_MoveCamera.CameraMoveSpeeds.Normal:
            num1 = 2f;
            break;
          case EventAction_MoveCamera.CameraMoveSpeeds.Fast:
            num1 = 8f;
            break;
          case EventAction_MoveCamera.CameraMoveSpeeds.Slow:
            num1 = 0.5f;
            break;
        }
        this.mCurrentTime = 0.0f;
        if ((double) this.MoveTime <= 0.0)
        {
          Vector3 vector3 = Vector3.op_Subtraction(this.TargetPosition, this.StartTargetPosition);
          // ISSUE: explicit reference operation
          float num2 = ((Vector3) @vector3).get_magnitude();
          if ((double) num2 <= 0.0)
          {
            if ((double) this.StartCameraDistanceScale == (double) this.CameraDistanceScale)
            {
              this.UpdateCameraPosition(1f);
              this.ActivateNext();
              return;
            }
            num2 = 1f;
          }
          this.mEndTime = num2 / num1;
        }
        else
          this.mEndTime = this.MoveTime;
        if (!this.Async)
          return;
        this.ActivateNext(true);
      }
    }

    public override void Update()
    {
      this.mCurrentTime += Time.get_deltaTime();
      float t = (double) this.mEndTime <= 0.0 ? 1f : Mathf.Clamp01(this.mCurrentTime / this.mEndTime);
      if ((this.FadeMode == EventAction_MoveCamera.CameraFadeModes.FadeIn || this.FadeMode == EventAction_MoveCamera.CameraFadeModes.FadeInOut) && (double) this.mCurrentTime < (double) this.FadeTime)
        FadeController.Instance.FadeTo(new Color(0.0f, 0.0f, 0.0f, (float) (1.0 - (double) this.mCurrentTime / (double) this.FadeTime)), 0.0f, 0);
      if ((this.FadeMode == EventAction_MoveCamera.CameraFadeModes.FadeOut || this.FadeMode == EventAction_MoveCamera.CameraFadeModes.FadeInOut) && (double) this.mCurrentTime >= (double) this.mEndTime - (double) this.FadeTime)
        FadeController.Instance.FadeTo(new Color(0.0f, 0.0f, 0.0f, (this.mCurrentTime - (this.mEndTime - this.FadeTime)) / this.FadeTime), 0.0f, 0);
      this.UpdateCameraPosition(t);
      if ((double) t < 1.0)
        return;
      if (!this.Async)
        this.ActivateNext();
      else
        this.enabled = false;
    }

    private void UpdateCameraPosition(float t)
    {
      TargetCamera targetCamera = TargetCamera.Get((Component) Camera.get_main());
      GameSettings instance = GameSettings.Instance;
      targetCamera.Pitch = -45f;
      if ((double) t >= 1.0)
      {
        targetCamera.SetPositionYaw(this.TargetPosition, this.TargetRotation);
        targetCamera.CameraDistance = instance.GameCamera_DefaultDistance * this.CameraDistanceScale;
      }
      else
      {
        targetCamera.SetPositionYaw(Vector3.Lerp(this.StartTargetPosition, this.TargetPosition, t), Mathf.Lerp(this.StartRotation, this.TargetRotation, t));
        targetCamera.CameraDistance = instance.GameCamera_DefaultDistance * Mathf.Lerp(this.StartCameraDistanceScale, this.CameraDistanceScale, t);
      }
      Camera.get_main().set_fieldOfView(instance.GameCamera_TacticsSceneFOV);
    }

    public override void GoToEndState()
    {
      GameSettings instance = GameSettings.Instance;
      if (this.Speed == EventAction_MoveCamera.CameraMoveSpeeds.Immediate)
      {
        EventAction_MoveCamera actionMoveCamera = this;
        actionMoveCamera.TargetPosition = Vector3.op_Addition(actionMoveCamera.TargetPosition, Vector3.op_Multiply(Vector3.get_up(), instance.GameCamera_UnitHeightOffset));
        this.UpdateCameraPosition(1f);
      }
      else
      {
        TargetCamera targetCamera = TargetCamera.Get((Component) Camera.get_main());
        targetCamera.Reset();
        if (this.MoveMode == EventAction_MoveCamera.CameraMoveModes.InterpolateToPoint)
        {
          this.StartTargetPosition = targetCamera.TargetPosition;
          this.StartRotation = targetCamera.Yaw;
          this.StartCameraDistanceScale = targetCamera.CameraDistance / instance.GameCamera_DefaultDistance;
        }
        else if (this.MoveMode == EventAction_MoveCamera.CameraMoveModes.SpecificActor)
        {
          this.StartRotation = targetCamera.Yaw;
          this.StartTargetPosition = targetCamera.TargetPosition;
          this.StartCameraDistanceScale = targetCamera.CameraDistance / instance.GameCamera_DefaultDistance;
          GameObject actor = EventAction.FindActor(this.ActorID);
          if (Object.op_Equality((Object) actor, (Object) null))
            return;
          this.TargetPosition = actor.get_transform().get_position();
          if ((double) this.TargetRotation < 0.0)
            this.TargetRotation = this.StartRotation;
        }
        else
        {
          if (this.SnapToGround)
            this.StartTargetPosition = GameUtility.RaycastGround(this.StartTargetPosition);
          EventAction_MoveCamera actionMoveCamera = this;
          actionMoveCamera.StartTargetPosition = Vector3.op_Addition(actionMoveCamera.StartTargetPosition, Vector3.op_Multiply(Vector3.get_up(), instance.GameCamera_UnitHeightOffset));
        }
        if (this.SnapToGround)
          this.TargetPosition = GameUtility.RaycastGround(this.TargetPosition);
        EventAction_MoveCamera actionMoveCamera1 = this;
        actionMoveCamera1.TargetPosition = Vector3.op_Addition(actionMoveCamera1.TargetPosition, Vector3.op_Multiply(Vector3.get_up(), instance.GameCamera_UnitHeightOffset));
        this.UpdateCameraPosition(2f);
      }
    }

    public enum CameraMoveModes
    {
      InterpolateToPoint,
      StartEnd,
      SpecificActor,
    }

    public enum CameraMoveSpeeds
    {
      Immediate,
      Normal,
      Fast,
      Slow,
    }

    public enum CameraFadeModes
    {
      KeepAsIs,
      FadeIn,
      FadeOut,
      FadeInOut,
    }
  }
}
