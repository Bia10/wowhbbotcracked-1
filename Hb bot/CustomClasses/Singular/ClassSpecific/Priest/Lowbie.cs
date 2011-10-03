#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: apoc $
// $Date: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Priest/Lowbie.cs $
// $LastChangedBy: apoc $
// $LastChangedDate: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $LastChangedRevision: 190 $
// $Revision: 190 $

#endregion

using System.Linq;

using Styx.Combat.CombatRoutine;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.Priest)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Combat)]
        [Behavior(BehaviorType.Pull)]
        [Context(WoWContext.All)]
        public Composite CreateLowbiePriestCombat()
        {
            return new PrioritySelector(
                CreateDiscHealOnlyBehavior(),
                CreateEnsureTarget(),
                CreateMoveToAndFace(28f, ret => Me.CurrentTarget),
                CreateWaitForCast(),
                CreateSpellBuffOnSelf("Arcane Torrent", ret => NearbyUnfriendlyUnits.Any(u => u.IsCasting && u.DistanceSqr < 8 * 8)),
                CreateSpellBuff("Shadow Word: Pain"),
                CreateSpellCast("Mind Blast"),
                CreateSpellCast("Smite"),
                CreateFireRangedWeapon()
                );
        }
    }
}