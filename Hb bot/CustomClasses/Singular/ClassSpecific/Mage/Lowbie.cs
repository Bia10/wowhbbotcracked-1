#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Mage/Lowbie.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $LastChangedRevision: 265 $
// $Revision: 265 $

#endregion

using Styx.Combat.CombatRoutine;
using Styx.Logic.Combat;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.Mage)]
        [Spec(TalentSpec.Lowbie)]
        [Context(WoWContext.All)]
        [Behavior(BehaviorType.Combat)]
        [Behavior(BehaviorType.Pull)]
        public Composite CreateLowbieMageCombat()
        {
            return new PrioritySelector(
                CreateEnsureTarget(),
                CreateMoveToAndFace(30f, ret => Me.CurrentTarget),
                CreateAutoAttack(true),
                CreateWaitForCast(true),
                CreateMagePolymorphOnAddBehavior(),
                CreateSpellCast("Arcane Missiles", ret => Me.Auras.ContainsKey("Arcane Missiles!")),
                CreateSpellCast("Fireball", ret => !SpellManager.HasSpell("Frostbolt")),
                CreateSpellBuff("Fire Blast", ret => SpellManager.CanCast("Fire Blast") && Me.CurrentTarget.HealthPercent < 10),
                CreateSpellCast("Frostbolt"),
                CreateFireRangedWeapon()
                );
        }
    }
}