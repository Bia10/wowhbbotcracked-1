#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/DeathKnight/Lowbie.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $LastChangedRevision: 265 $
// $Revision: 265 $

#endregion

using Styx.Combat.CombatRoutine;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.DeathKnight)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Combat)]
        [Context(WoWContext.All)]
        public Composite CreateLowbieDeathKnightCombat()
        {
            return new PrioritySelector(
                CreateEnsureTarget(),
                CreateAutoAttack(true),
                CreateFaceUnit(),
                CreateSpellCast("Death Grip", ret => Me.CurrentTarget.Distance > 15),
                CreateSpellCast("Death Coil", false),
                CreateSpellCast("Icy Touch", false),
                CreateMoveToAndFace(5f, ret => Me.CurrentTarget),
                CreateSpellCast("Blood Strike"),
                CreateSpellCast("Plague Strike")
                );
        }
    }
}