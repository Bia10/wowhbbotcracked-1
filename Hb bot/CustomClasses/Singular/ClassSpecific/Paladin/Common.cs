#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-04-11 22:21:17 +0200 (må, 11 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Paladin/Common.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-04-11 22:21:17 +0200 (må, 11 apr 2011) $
// $LastChangedRevision: 268 $
// $Revision: 268 $

#endregion

using System.Collections.Generic;
using System.Linq;

using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Logic.Combat;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        private bool CurrentTargetIsUndeadOrDemon { get { return Me.CurrentTarget.CreatureType == WoWCreatureType.Undead || Me.CurrentTarget.CreatureType == WoWCreatureType.Demon; } }

        [Class(WoWClass.Paladin)]
        [Behavior(BehaviorType.PreCombatBuffs)]
        [Spec(TalentSpec.RetributionPaladin)]
        [Spec(TalentSpec.HolyPaladin)]
        [Spec(TalentSpec.ProtectionPaladin)]
        [Spec(TalentSpec.Lowbie)]
        [Context(WoWContext.All)]
        public Composite CreatePaladinPreCombatBuffs()
        {
            return
                new PrioritySelector(
                // This won't run, but it's here for changes in the future. We NEVER run this method if we're mounted.
                    CreateSpellBuffOnSelf("Crusader Aura", ret => Me.Mounted),
                    CreatePaladinBlessBehavior(),
                    new Decorator(
                        ret => TalentManager.CurrentSpec == TalentSpec.HolyPaladin,
                        new PrioritySelector(
                            CreateSpellBuffOnSelf("Concentration Aura"),
                            CreateSpellBuffOnSelf("Seal of Insight"),
                            CreateSpellBuffOnSelf("Seal of Righteousness", ret => !SpellManager.HasSpell("Seal of Insight"))
                        )),
                    new Decorator(
                        ret => TalentManager.CurrentSpec != TalentSpec.HolyPaladin,
                        new PrioritySelector(
                            CreateSpellBuffOnSelf("Righteous Fury", ret => TalentManager.CurrentSpec == TalentSpec.ProtectionPaladin),
                            CreateSpellBuffOnSelf("Devotion Aura",
                                ret => TalentManager.CurrentSpec == TalentSpec.ProtectionPaladin ||
                                       TalentManager.CurrentSpec == TalentSpec.Lowbie),
                            CreateSpellBuffOnSelf("Retribution Aura", ret => TalentManager.CurrentSpec == TalentSpec.RetributionPaladin),
                            CreateSpellBuffOnSelf("Seal of Truth"),
                            CreateSpellBuffOnSelf("Seal of Righteousness", ret => !SpellManager.HasSpell("Seal of Truth"))
                        ))
                    );
        }

        public Composite CreatePaladinBlessBehavior()
        {
            return
                new PrioritySelector(
                    CreateSpellBuffOnSelf("Blessing of Might",
                        ret =>
                        {
                            //Ok, this is a bit complicated but it works :p /raphus
                            var players = StyxWoW.Me.IsInParty
                                              ? StyxWoW.Me.PartyMembers.Concat(new List<WoWPlayer> { StyxWoW.Me })
                                              : StyxWoW.Me.IsInRaid
                                                    ? StyxWoW.Me.RaidMembers.Concat(new List<WoWPlayer> { StyxWoW.Me })
                                                    : new List<WoWPlayer> { StyxWoW.Me };

                            var result = players.Any(
                                p => p.DistanceSqr < 40 * 40 &&
                                     (!p.HasAura("Blessing of Might") || p.Auras["Blessing of Might"].CreatorGuid != StyxWoW.Me.Guid) &&
                                     ((p.HasAura("Blessing of Kings") && p.Auras["Blessing of Kings"].CreatorGuid != StyxWoW.Me.Guid) ||
                                     p.HasAura("Mark of the Wild") || p.HasAura("Embrace of the Shale Spider")));

                            return result;
                        }),
                    CreateSpellBuffOnSelf("Blessing of Kings",
                        ret =>
                        {
                            var players = StyxWoW.Me.IsInParty
                                              ? StyxWoW.Me.PartyMembers.Union(new List<WoWPlayer> { StyxWoW.Me })
                                              : StyxWoW.Me.IsInRaid
                                                    ? StyxWoW.Me.RaidMembers.Union(new List<WoWPlayer> { StyxWoW.Me })
                                                    : new List<WoWPlayer> { StyxWoW.Me };

                            var result = players.Any(
                                p => p.DistanceSqr < 40 * 40 &&
                                     (!p.HasAura("Blessing of Kings") || p.Auras["Blessing of Kings"].CreatorGuid != StyxWoW.Me.Guid) &&
                                     !p.HasAura("Mark of the Wild") &&
                                     !p.HasAura("Embrace of the Shale Spider"));

                            return result;
                        })
                    );
        }
    }
}