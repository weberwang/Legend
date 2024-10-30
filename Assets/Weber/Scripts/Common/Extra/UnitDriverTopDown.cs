using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace Weber.Scripts.Common.Extra
{
    [Title("Top Down")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Category("Top Down")]
    [Serializable]
    public class UnitDriverTopDown : UnitDriverController
    {
        public override bool IsGrounded => true;

        public override void OnStartup(Character character)
        {
            base.OnStartup(character);

            var position = Transform.position;
            position.y = Character.Motion.Height / 2f;
            Transform.position = position;
        }

        public override void OnUpdate()
        {
            if (this.Character.IsDead) return;
            if (this.m_Controller == null) return;

            UpdateGravity(Character.Motion);
            this.UpdateTranslation(this.Character.Motion);

            Vector3 displacement = this.Transform.position - this.m_PreviousPosition;
            this.m_Velocity = this.Character.Time.DeltaTime > float.Epsilon
                ? displacement / this.Character.Time.DeltaTime
                : Vector3.zero;

            this.m_PreviousPosition = this.Transform.position;
        }

        protected virtual Vector3 UpdateMoveToDirection(IUnitMotion motion)
        {
            this.m_MoveDirection = motion.MoveDirection;
            return this.m_MoveDirection * this.Character.Time.DeltaTime;
        }

        protected virtual Vector3 UpdateMoveToPosition(IUnitMotion motion)
        {
            float distance = Vector3.Distance(this.Character.Feet, motion.MovePosition);
            float brakeRadiusHeuristic = Math.Max(motion.Height, motion.Radius * 2f);
            float velocity = motion.MoveDirection.magnitude;

            if (distance < brakeRadiusHeuristic)
            {
                velocity = Mathf.Lerp(
                    motion.LinearSpeed, Mathf.Max(motion.LinearSpeed * 0.25f, 1f),
                    1f - Mathf.Clamp01(distance / brakeRadiusHeuristic)
                );
            }

            this.m_MoveDirection = motion.MoveDirection;
            return this.m_MoveDirection.normalized * (velocity * this.Character.Time.DeltaTime);
        }

        // protected virtual void UpdateTranslation(IUnitMotion motion)
        // {
        //     Vector3 movement = Vector3.up * (this.m_VerticalSpeed * this.Character.Time.DeltaTime);
        //
        //     Vector3 kinetic = motion.MovementType switch
        //     {
        //         Character.MovementType.MoveToDirection => this.UpdateMoveToDirection(motion),
        //         Character.MovementType.MoveToPosition => this.UpdateMoveToPosition(motion),
        //         _ => Vector3.zero
        //     };
        //
        //     Vector3 rootMotion = this.Character.Animim.RootMotionDeltaPosition;
        //     Vector3 translation = Vector3.Lerp(kinetic, rootMotion, this.Character.RootMotionPosition);
        //
        //     movement += this.m_Axonometry?.ProcessTranslation(this, translation) ?? translation;
        //
        //     if (this.m_FrameSlideFromCharacter >= Time.frameCount - 1)
        //     {
        //         float deltaSpeed = motion.LinearSpeed * this.Character.Time.DeltaTime;
        //         movement += this.m_SlideFromCharacter * deltaSpeed;
        //     }
        //
        //     movement += this.m_AddTranslation.Consume();
        //
        //     if (this.m_Controller.enabled)
        //     {
        //         this.m_Controller.Move(movement);
        //     }
        // }
    }
}