#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: apoc $
// $Date: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Rogue/Lowbie.cs $
// $LastChangedBy: apoc $
// $LastChangedDate: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $LastChangedRevision: 190 $
// $Revision: 190 $

#endregion

using Styx.Combat.CombatRoutine;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.Rogue)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Combat)]
        [Context(WoWContext.All)]
        public Composite CreateLowbieRogueCombat()
        {
            return new PrioritySelector(
                CreateEnsureTarget(),
                CreateAutoAttack(true),
                CreateMoveToAndFace(),
                CreateSpellCast("Eviscerate", ret => Me.ComboPoints == 5 || Me.CurrentTarget.HealthPercent <= 40 && Me.ComboPoints >= 2),
                CreateSpellCast("Sinister Strike")
                );
        }

        [Class(WoWClass.Rogue)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Pull)]
        [Context(WoWContext.All)]
        public Composite CreateLowbieRoguePull()
        {
            return new PrioritySelector(
                CreateSpellBuffOnSelf("Stealth"),
                CreateAutoAttack(true),
                CreateMoveToAndFace()
                );
        }
    }
}