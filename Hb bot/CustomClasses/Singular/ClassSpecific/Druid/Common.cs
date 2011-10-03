#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-03-22 21:32:35 +0100 (ti, 22 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Druid/Common.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-03-22 21:32:35 +0100 (ti, 22 mar 2011) $
// $LastChangedRevision: 218 $
// $Revision: 218 $

#endregion

using System.Linq;

using Styx;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals.WoWObjects;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        public ShapeshiftForm WantedDruidForm { get; set; }

        [Class(WoWClass.Druid)]
        [Behavior(BehaviorType.PreCombatBuffs)]
        [Spec(TalentSpec.BalanceDruid)]
        [Spec(TalentSpec.FeralDruid)]
        [Spec(TalentSpec.FeralTankDruid)]
        [Spec(TalentSpec.RestorationDruid)]
        [Spec(TalentSpec.Lowbie)]
        [Context(WoWContext.All)]
        public Composite CreateDruidBuffComposite()
        {
            return new PrioritySelector(
                CreateSpellCast(
                    "Mark of the Wild",
                    ret => NearbyFriendlyPlayers.Any(u => !u.Dead && !u.IsGhost && u.IsInMyPartyOrRaid && CanCastMotWOn(u)),
                    ret => Me)
                // TODO: Have it buff MotW when nearby party/raid members are missing the buff.
                );
        }

        public bool CanCastMotWOn(WoWUnit unit)
        {
            return !unit.HasAura("Mark of the Wild") &&
                   !unit.HasAura("Embrace of the Shale Spider") &&
                   !unit.HasAura("Blessing of Kings");
        }
    }
}