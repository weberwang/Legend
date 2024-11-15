using System;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using Pathfinding;
using UnityEngine;
using Weber.Scripts.Legend.Game;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Common.Extra
{
    [Title("A* Pathfinding")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Category("A* Pathfinding")]
    [Description(
        "Moves the Character using A* Pathfinding. "
    )]
    [Serializable]
    public class UnitDriverPathfinding : UnitDriverTopDown
    {
        enum TrackType
        {
            //追随
            Follow,

            //拦截
            Intercept,
        }

        [SerializeField] private bool m_CanMove = true;
        [SerializeField] private TrackType m_TrackType = TrackType.Follow;
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();
        [SerializeField] private float m_RandomRaduis = 1;
        [SerializeField] private float m_NearDistance = 1;
        [SerializeField] private float _stopDistance = 0.5f;
        [SerializeField] private float m_CheckPathInterval = 0.1f;

        public bool CanMove
        {
            get => m_CanMove;
            set
            {
                m_CanMove = value;
                _AIPath.canMove = m_CanMove;
            }
        }

        private AIPath _AIPath;

        private float _lastCheckTime = 0;

        private GameObject _targetObject;

        private float _offsetRadius;

        public override void OnStartup(Character character)
        {
            base.OnStartup(character);
            _AIPath = character.Require<AIPath>();
            _targetObject = m_Target.Get(Character.gameObject);
            _offsetRadius = m_RandomRaduis;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Character.IsDead)
            {
                _AIPath.canMove = false;
            }
            else
            {
                switch (Game.Instance.GameState)
                {
                    case GameState.Over:
                        _AIPath.canMove = false;
                        break;
                    case GameState.Playing:
                        _AIPath.canMove = !Game.Instance.Paused;
                        break;
                }
            }
        }

        protected override void UpdateTranslation(IUnitMotion motion)
        {
            if (_targetObject is null) return;
            AIMove();
            if (Vector3.Distance(Transform.position, _targetObject.transform.position) < m_NearDistance)
            {
                _offsetRadius = 0;
                if (Vector3.Distance(Transform.position, _targetObject.transform.position) <= _stopDistance)
                {
                    _AIPath.canMove = false;
                }
            }
            else
            {
                _offsetRadius = m_RandomRaduis;
            }
        }

        private void AIMove()
        {
            if (!m_CanMove || m_Target is null) return;
            _lastCheckTime -= Time.deltaTime;
            if (_lastCheckTime > 0) return;
            switch (m_TrackType)
            {
                case TrackType.Follow:
                    FollowTarget();
                    break;
                case TrackType.Intercept:
                    InterceptTarget();
                    break;
            }

            _lastCheckTime = m_CheckPathInterval;
        }

        private void FollowTarget()
        {
            _AIPath.maxSpeed = Character.Motion.LinearSpeed;
            var randomPos = Random.insideUnitCircle * _offsetRadius;
            _AIPath.destination = _targetObject.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
        }

        private void InterceptTarget()
        {
            if (_targetObject is null) return;

            Vector3 targetPosition = _targetObject.transform.position;
            Vector3 targetVelocity = _targetObject.GetComponent<Character>().Motion.MoveDirection;
            Vector3 characterPosition = this.Character.transform.position;
            float characterSpeed = this.Character.Motion.LinearSpeed;

            // 预测目标位置
            Vector3 interceptPoint = PredictInterceptPoint(characterPosition, characterSpeed, targetPosition, targetVelocity);
            var randomPos = Random.insideUnitCircle * _offsetRadius;
            _AIPath.maxSpeed = characterSpeed;
            _AIPath.destination = interceptPoint + new Vector3(randomPos.x, 0, randomPos.y);
        }

        private Vector3 PredictInterceptPoint(Vector3 characterPosition, float characterSpeed, Vector3 targetPosition, Vector3 targetVelocity)
        {
            Vector3 directionToTarget = targetPosition - characterPosition;
            float distanceToTarget = directionToTarget.magnitude;
            float targetSpeed = targetVelocity.magnitude;

            float interceptTime = distanceToTarget / (characterSpeed + targetSpeed);
            Vector3 interceptPoint = targetPosition + targetVelocity * interceptTime;

            return interceptPoint;
        }
    }
}