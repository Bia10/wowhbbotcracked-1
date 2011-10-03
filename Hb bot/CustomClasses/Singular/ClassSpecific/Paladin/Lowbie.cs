#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-03-26 12:33:59 +0100 (lö, 26 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Paladin/Lowbie.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-03-26 12:33:59 +0100 (lö, 26 mar 2011) $
// $LastChangedRevision: 230 $
// $Revision: 230 $

#endregion

using Styx.Combat.CombatRoutine;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.Paladin)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Combat)]
        [Context(WoWContext.All)]
        public Composite CreateLowbiePaladinCombat()
        {
            return
                new PrioritySelector(
                    CreateEnsureTarget(),
                    CreateAutoAttack(true),
                    CreateMoveToAndFace(5f, ret => Me.CurrentTarget),
                    CreateSpellCast("Crusader Strike"),
                    CreateSpellCast("Judgement")
                    );
        }

        [Class(WoWClass.Paladin)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Pull)]
        [Context(WoWContext.All)]
        public Composite CreateLowbiePaladinPull()
        {
            return
                new PrioritySelector(
                    CreateFaceUnit(),
                    CreateAutoAttack(true),
                    CreateSpellCast("Judgement"),
                    CreateMoveToAndFace(5f, ret => Me.CurrentTarget)
                    );
        }

        [Class(WoWClass.Paladin)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Heal)]
        [Context(WoWContext.All)]
        public Composite CreateLowbiePaladinHeal()
        {
            return
                new PrioritySelector(
                    CreateSpellCastOnSelf("Word of Glory", ret => Me.HealthPercent < 50),
                    CreateSpellCastOnSelf("Holy Light", ret => Me.HealthPercent < 40)
                    );
        }

        [Class(WoWClass.Paladin)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.PreCombatBuffs)]
        [Context(WoWContext.All)]
        public Composite CreateLowbiePaladinPreCombatBuffs()
        {
            return
                new PrioritySelector(
                    CreateSpellBuffOnSelf("Seal of Righteousness"),
                    CreateSpellBuffOnSelf("Devotion Aura")
                    );
        }

        [Class(WoWClass.Paladin)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.CombatBuffs)]
        [Context(WoWContext.All)]
        public Composite CreateLowbiePaladinCombatBuffs()
        {
            return
                new PrioritySelector(
                    CreateSpellBuffOnSelf("Seal of Righteousness"),
                    CreateSpellBuffOnSelf("Devotion Aura")
                    );
        }
    }
}