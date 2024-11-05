using System;
using System.Collections.Generic;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GPUInstancer.CrowdAnimations;
using UnityEngine;
using Weber.Scripts.Legend.Game;
using Weber.Scripts.Legend.Unit;

namespace Weber.Scripts.Common.Extra
{
    [Title("Crowd")]
    [Image(typeof(IconAnimator), ColorTheme.Type.Green)]
    [Category("Crowd")]
    [Description("Crowd animation system for characters")]
    [Serializable]
    public class UnitAnimimGPU : TUnitAnimim
    {
        private AnimationClip _run;
        // private const float SMOOTH_PIVOT = 0.01f;
        // private const float SMOOTH_GROUNDED = 0.2f;
        // private const float SMOOTH_STAND = 0.1f;

        // STATIC PROPERTIES: ---------------------------------------------------------------------

        // private static readonly int K_SPEED_Z = Animator.StringToHash("Speed-Z");
        // private static readonly int K_SPEED_X = Animator.StringToHash("Speed-X");
        // // private static readonly int K_SPEED_Y = Animator.StringToHash("Speed-Y");
        //
        // private static readonly int K_SURFACE_SPEED = Animator.StringToHash("Movement");
        // private static readonly int K_PIVOT_SPEED = Animator.StringToHash("Pivot");
        //
        // // private static readonly int K_GROUNDED = Animator.StringToHash("Grounded");
        // private static readonly int K_STAND = Animator.StringToHash("Stand");
        //
        // // MEMBERS: -------------------------------------------------------------------------------
        //
        // protected Dictionary<int, AnimFloat> m_SmoothParameters;
        // protected Dictionary<int, AnimFloat> m_IndependentParameters;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        // public override void OnStartup(Character character)
        // {
        //     base.OnStartup(character);
        //
        //     this.m_SmoothParameters = new Dictionary<int, AnimFloat>
        //     {
        //         { K_SPEED_Z, new AnimFloat(0f, this.m_SmoothTime) },
        //         { K_SPEED_X, new AnimFloat(0f, this.m_SmoothTime) },
        //         // { K_SPEED_Y, new AnimFloat(0f, this.m_SmoothTime) },
        //         { K_SURFACE_SPEED, new AnimFloat(0f, this.m_SmoothTime) },
        //     };
        //
        //     this.m_IndependentParameters = new Dictionary<int, AnimFloat>
        //     {
        //         { K_PIVOT_SPEED, new AnimFloat(0f, SMOOTH_PIVOT) },
        //         // { K_GROUNDED, new AnimFloat(1f, SMOOTH_GROUNDED) },
        //         { K_STAND, new AnimFloat(1f, SMOOTH_STAND) },
        //     };
        // }

        private GPUICrowdPrefab _crowdInstance;
        private CharacterUnit _characterUnit;

        public override void OnStartup(Character character)
        {
            base.OnStartup(character);
            _characterUnit = character.Get<CharacterUnit>();
            _crowdInstance = character.GetComponentInChildren<GPUICrowdPrefab>();
        }

        public override void OnUpdate()
        {
            // base.OnUpdate();
            if (Character.IsDead) return;
            if (_crowdInstance is null)
            {
                _crowdInstance = Character.GetComponentInChildren<GPUICrowdPrefab>();
                return;
            }

            if (_run == null)
            {
                _run = _characterUnit.CharacterData.run;
                // _run
            }

            // if (_crowdInstance.animatorRef is null) return;
            // if (!_crowdInstance.animatorRef.gameObject.activeInHierarchy) return;
            //
            // _crowdInstance.animatorRef.updateMode = this.Character.Time.UpdateTime == TimeMode.UpdateMode.GameTime
            //     ? AnimatorUpdateMode.Normal
            //     : AnimatorUpdateMode.UnscaledTime;
            //
            // IUnitMotion motion = this.Character.Motion;
            // IUnitDriver driver = this.Character.Driver;
            // IUnitFacing facing = this.Character.Facing;
            // Vector3 movementDirection = motion.LinearSpeed > 0.1f
            //     ? driver.LocalMoveDirection / motion.LinearSpeed
            //     : Vector3.zero;
            //
            // float movementMagnitude = Vector3.Scale(movementDirection, Vector3Plane.NormalUp).magnitude;
            // float pivot = facing.PivotSpeed;
            //
            // // foreach (KeyValuePair<int, AnimFloat> parameter in this.m_SmoothParameters)
            // // {
            // //     parameter.Value.Smooth = this.m_SmoothTime;
            // // }
            //
            // float deltaTime = this.Character.Time.DeltaTime;

            // Update anim parameters:
            // this.m_SmoothParameters[K_SPEED_Z].UpdateWithDelta(movementDirection.z, deltaTime);
            // this.m_SmoothParameters[K_SPEED_X].UpdateWithDelta(movementDirection.x, deltaTime);
            // this.m_SmoothParameters[K_SPEED_Y].UpdateWithDelta(movementDirection.y, deltaTime);
            // this.m_SmoothParameters[K_SURFACE_SPEED].UpdateWithDelta(movementMagnitude, deltaTime);

            // this.m_IndependentParameters[K_PIVOT_SPEED].UpdateWithDelta(pivot, deltaTime);
            // this.m_IndependentParameters[K_GROUNDED].UpdateWithDelta(driver.IsGrounded, deltaTime);
            // this.m_IndependentParameters[K_STAND].UpdateWithDelta(motion.StandLevel.Current, deltaTime);

            // Update animator parameters:
            // this.m_Animator.SetFloat(K_SPEED_Z, this.m_SmoothParameters[K_SPEED_Z].Current);
            // this.m_Animator.SetFloat(K_SPEED_X, this.m_SmoothParameters[K_SPEED_X].Current);
            // this.m_Animator.SetFloat(K_SPEED_Y, this.m_SmoothParameters[K_SPEED_Y].Current);
            // this.m_Animator.SetFloat(K_SURFACE_SPEED, this.m_SmoothParameters[K_SURFACE_SPEED].Current);

            // this.m_Animator.SetFloat(K_PIVOT_SPEED, this.m_IndependentParameters[K_PIVOT_SPEED].Current);
            // this.m_Animator.SetFloat(K_GROUNDED, this.m_IndependentParameters[K_GROUNDED].Current);
            // this.m_Animator.SetFloat(K_STAND, this.m_IndependentParameters[K_STAND].Current);
            switch (Game.Instance.GameState)
            {
                case GameState.Playing:
                    if (Game.Instance.Paused)
                    {
                        GPUICrowdAPI.SetAnimationSpeed(_crowdInstance, 0);
                    }
                    else
                    {
                        GPUICrowdAPI.SetAnimationSpeed(_crowdInstance, 1);
                        if (_run != null) GPUICrowdAPI.StartAnimation(_crowdInstance, _run, transitionTime: 0.3f);
                    }

                    break;
            }
        }
    }
}