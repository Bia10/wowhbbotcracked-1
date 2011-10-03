using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx.Database;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles.Quest;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Styx.Logic.BehaviorTree;
using Action = TreeSharp.Action;

namespace Styx.Bot.Quest_Behaviors
{
    public class q13947 : CustomForcedBehavior
    {
        

        

        bool success = true;

        public q13947(Dictionary<string, string> args)
            : base(args)
        {
			MobId       = GetAttributeAsMobId("MobId", true, new [] { "NpcId" }) ?? 0;
			MobId2       = GetAttributeAsMobId("MobId2", true, new [] { "NpcId2" }) ?? 0;
			QuestId     = GetAttributeAsQuestId("QuestId", false, null) ?? 0;
	    QuestRequirementComplete = GetAttributeAsEnum<QuestCompleteRequirement>("QuestCompleteRequirement", false, null) ?? QuestCompleteRequirement.NotComplete;
            QuestRequirementInLog    = GetAttributeAsEnum<QuestInLogRequirement>("QuestInLogRequirement", false, null) ?? QuestInLogRequirement.InLog;
        }

        public WoWPoint Location { get; private set; }
        public int MobId { get; set; }
        public int MobId2 { get; set; }
        public int QuestId { get; set; }
	public QuestCompleteRequirement QuestRequirementComplete { get; private set; }
        public QuestInLogRequirement    QuestRequirementInLog { get; private set; }
        public static LocalPlayer me = ObjectManager.Me;
		
		
        #region Overrides of CustomForcedBehavior
	
        public List<WoWUnit> mob1List
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == MobId && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
		public List<WoWUnit> mob2List
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                                    .Where(u => (u.Entry == MobId2 && !u.Dead))
                                    .OrderBy(u => u.Distance).ToList();
            }
        }
        
		static public bool InVehicle { get { return Lua.GetReturnVal<int>("if IsPossessBarVisible() or UnitInVehicle('player') then return 1 else return 0 end", 0) == 1; } }
		//static public bool Obj1Done { get { return Lua.GetReturnVal<int>("a,b,c=GetQuestLogLeaderBoard(1,GetQuestLogIndexByID(13947));if c==1 then return 1 else return 0 end", 0) == 1; } }
		//static public bool Obj2Done { get { return Lua.GetReturnVal<int>("a,b,c=GetQuestLogLeaderBoard(2,GetQuestLogIndexByID(13947));if c==1 then return 1 else return 0 end", 0) == 1; } }
		
        private Composite _root;
        protected override Composite CreateBehavior()
        {
            return _root ?? (_root =
                new PrioritySelector(
					
                    new Decorator(ret => (!InVehicle),
                        new Sequence(
                            new Action(ret => TreeRoot.StatusText = "Finished!"),
			 new Action(ret => Lua.DoString("RunMacroText('/click VehicleMenuBarActionButton2','0')")),
                            new WaitContinue(120,
                            new Action(delegate
                            {
                                _isDone = true;
                                return RunStatus.Success;
                                }))
                            )),
			
					
							
			new Decorator(ret => InVehicle && mob1List[0].Location.Distance(me.Location) <= 30,
                        new Sequence(
                            new Action(ret => TreeRoot.StatusText = "Bombing - " + mob1List[0].Name),
							new Action(ret => mob1List[0].Target()),
							new Action(ret => Lua.DoString("RunMacroText('/click VehicleMenuBarActionButton1','0')")),
							new Action(ret => LegacySpellManager.ClickRemoteLocation(mob1List[0].Location)),
										
                            new Action(ret => Thread.Sleep(2000))
                        )
					),
					
			new Decorator(ret => InVehicle && mob2List.Count > 0 && mob2List[0].Location.Distance(me.Location) <= 30,
                        new Sequence(
                            new Action(ret => TreeRoot.StatusText = "Bombing - " + mob2List[0].Name),
							new Action(ret => mob2List[0].Target()),
							new Action(ret => Lua.DoString("RunMacroText('/click VehicleMenuBarActionButton1','0')")),
							new Action(ret => LegacySpellManager.ClickRemoteLocation(mob2List[0].Location)),
			    new Action(ret => Thread.Sleep(2000))
                        )
					)
                )
			);
        }

        

        
        

        private bool _isDone;
        public override bool IsDone
        {
            get { return _isDone; }
        }

        #endregion
    }
}

