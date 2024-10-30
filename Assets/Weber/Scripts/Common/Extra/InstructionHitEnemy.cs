using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using Weber.Scripts.Legend.Unit;

namespace Weber.Scripts.Common.Extra
{
    public class InstructionHitEnemy : Instruction
    {
        protected override Task Run(Args args)
        {
            var self = args.Self.Get<CharacterUnit>();
            var target = args.Target.Get<CharacterUnit>();
            target.OnMeleeHit(self);
            return DefaultResult;
        }
    }
}