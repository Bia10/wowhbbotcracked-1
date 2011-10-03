namespace Styx.Bot.CustomClasses
{
	using System;
	using System.IO;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using System.Linq;
	using System.Windows.Forms;
	using System.Media;
	using Styx;
	using Styx.Combat.CombatRoutine;
	using Styx.Helpers;
	using Styx.Logic;
	using Styx.Logic.Combat;
	using Styx.Logic.Pathing;
	using Styx.WoWInternals;
	using Styx.WoWInternals.WoWObjects;
    using Styx.Plugins.PluginClass;
	using System.Net;
	using System.Runtime.InteropServices;
	using System.Reflection;

    public class Instancebuddy : HBPlugin
    {
		private static LocalPlayer Me { get { return ObjectManager.Me; } }
		private static string LFGStatus;
		private static string SubMode;
		private static WoWPlayer tank;
		private static WoWPlayer _tank;
		private static WoWUnit LastTarget;		
		private static string tankName;
		private static string emptyProfile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Instancebuddy")) + "\\empty.xml";
		private static string wawFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Instancebuddy")) + "\\warning.wav";
		private static bool ShouldLeave;
		private static bool Queued;
		private static bool DiedInInstance;
		private static bool soundPlayed;
		private static bool UseMountWasOn;
		private static bool LootWasOn;
		private static bool roleChecked;
		private static bool LoadingScreen;
		private static bool threadRestCheckIsRunning;
		private static int errorCount;
		private static int targetCount;
		private static int tickCount;
		private static Thread th;
		private Stopwatch partyTimer = new Stopwatch();																	
		
		private static void warningSound()
		{
			if (!soundPlayed)
			{
				soundPlayed = true;
				SoundPlayer simpleSound = new SoundPlayer(wawFile);
				simpleSound.Play();
			}
		}
		
        private static void Log(string format, params object[] args)
        {
            Logging.Write(Color.DarkSlateBlue, "[Instancebuddy v0.12b]:" + format, args);
        }
		
        private static void dLog(string format, params object[] args)
        {
			Logging.WriteDebug("[Instancebuddy v0.12b]:" + format, args);
        }	
		
		private static bool MeCheck
		{
			get
			{
				return (ObjectManager.IsInGame && Me != null && Me.IsValid);
			}
		}
		
		private static void safeMoveTo(WoWPoint point)
		{
			try
			{
				WoWPoint movePoint = point;
				
				if (Navigator.GeneratePath(Me.Location, movePoint).Length == 0)
				{
					Log("Can not generate navigation path to tank. Something wrong with mesh");
					errorCount++;
					if (errorCount > 50)
						warningSound();
				}
				else
				{
					errorCount = 0;
					soundPlayed = false;
					Navigator.MoveTo(movePoint);	
					Thread.Sleep(100);
				}
			}
			catch (System.Threading.ThreadAbortException) { throw; }
			catch (Exception c)
			{
				Log("This instance has not been meshed yet");
				dLog("{0}", c);
				warningSound();
			}			
		}
		
		private static void safeMoveTo(WoWUnit unit)
		{
			try
			{
				WoWPoint movePoint = unit.Location;
				
				if (Navigator.GeneratePath(Me.Location, movePoint).Length == 0)
				{
					Log("Can not generate navigation path to tank. Something wrong with mesh");
					errorCount++;
					if (errorCount > 50)
						warningSound();
				}
				else
				{
					errorCount = 0;
					soundPlayed = false;
					Navigator.MoveTo(movePoint);	
					Thread.Sleep(100);
				}
			}
			catch (System.Threading.ThreadAbortException) { throw; }
			catch (Exception c)
			{
				Log("This instance has not been meshed yet");
				dLog("{0}", c);
				warningSound();
			}			
		}
		
		private static void safeMoveTo(WoWUnit unit, float range)
		{
			try
			{
				WoWPoint movePoint = WoWMovement.CalculatePointFrom(unit.Location, range);
				
				if (Navigator.GeneratePath(Me.Location, movePoint).Length == 0)
				{
					Log("Can not generate navigation path to tank. Something wrong with mesh");
					errorCount++;
					if (errorCount > 50)
						warningSound();
				}
				else
				{
					errorCount = 0;
					soundPlayed = false;
					Navigator.MoveTo(movePoint);	
					Thread.Sleep(100);
				}
			}
			catch (System.Threading.ThreadAbortException) { throw; }
			catch (Exception c)
			{
				Log("This instance has not been meshed yet");
				dLog("{0}", c);
				warningSound();
			}			
		}		
		
		public static void crashFix()
		{
			if (!MeCheck)
			{
				Log("Loading Screen detected");
				th = new Thread(new ThreadStart(restartBot));
				th.Start();	
				Thread.Sleep(1000);
			}
		}
		
		public static void restartBot()
		{
			Styx.Logic.BehaviorTree.TreeRoot.Stop();
			
			tickCount = Environment.TickCount;
			while (!MeCheck && Environment.TickCount - tickCount < 10000)
			{
				Thread.Sleep(100);
			}
			
			Log("Successfully zoned. Restarting bot");
			Thread.Sleep(2500);
			Styx.Logic.BehaviorTree.TreeRoot.Start();
			th.Abort();
		}		
		
		private static int RandomNumber(int min, int max)
		{
			Random random = new Random();
			return random.Next(min, max);
		}		
      
        public override void Pulse()
        {
			try
			{
				crashFix();
								
				//Release spirit override				
				while (MeCheck && !Me.IsAlive && Me.IsInInstance)
				{
					DiedInInstance = true;
					List<WoWPlayer> rezzerList = Me.PartyMembers.FindAll(player => 
								(player.Class == WoWClass.Shaman ||
								 player.Class == WoWClass.Paladin ||
								 player.Class == WoWClass.Priest ||
								 player.Class == WoWClass.Druid) &&
								player.IsAlive &&
								player.Distance < 60);			
					
					if (rezzerList.Count == 0)
					{
						Log("No rezzers around. Releasing corpse");					
						Lua.DoString("RepopMe()", "lfg.lua");
						//Crash fix
						while (MeCheck)
						{
							Thread.Sleep(100);
						}
						crashFix();
						break;
					}
					
					Lua.DoString("AcceptResurrect()", "lfg.lua");
					Thread.Sleep(100);
				}
				
				//Retrieve corpse override
				while (MeCheck && Me.Auras.ContainsKey("Ghost") && DiedInInstance)
				{
					tickCount = Environment.TickCount;
					while (MeCheck)
					{
						safeMoveTo(Me.CorpsePoint);
						Thread.Sleep(100);
						if (Environment.TickCount - tickCount > 120000)
						{
							warningSound();
							Log("Corpse run failed !");
							return;
						}
						crashFix();					
					}			
				}	
				
				//Changing profile
				if (MeCheck && Me.IsInInstance && Logic.Profiles.ProfileManager.XmlLocation != emptyProfile)
				{
					Log("Entered to the dungeon. Loading emtpy profile");
					LevelbotSettings.Instance.LastUsedPath = Logic.Profiles.ProfileManager.XmlLocation;
					Logic.Profiles.ProfileManager.LoadNew(emptyProfile, false);
				}
				else if (MeCheck && !Me.IsInInstance && Logic.Profiles.ProfileManager.XmlLocation == emptyProfile)
				{
					Log("Loading previous profile");
					Logic.Profiles.ProfileManager.LoadNew(LevelbotSettings.Instance.LastUsedPath, true);
				}	
				
				//Variable checks
				if (MeCheck && !Me.IsInInstance)
				{
					if (UseMountWasOn)
						LevelbotSettings.Instance.UseMount = true;
					if (LootWasOn)
						LevelbotSettings.Instance.LootMobs = true;					

					RaFHelper.SetLeader(Me);
					LastTarget = null;
					tank = null;
					_tank = null;	
				}
				else
				{
					if (LevelbotSettings.Instance.UseMount)
						UseMountWasOn = true;
					else
						UseMountWasOn = false;
						
					if (LevelbotSettings.Instance.LootMobs)
						LootWasOn = true;
					else
						LootWasOn = false;
						
					LevelbotSettings.Instance.UseMount = false;
					LevelbotSettings.Instance.LootMobs = false;
					Queued = false;
				}
				
				if (MeCheck && Me.IsAlive)
				{
					DiedInInstance = false;
				}
				//		
				
				//Get current LFG mode
				List<string> GetLFGMode = Lua.LuaGetReturnValue("return select(1, select(1, GetLFGMode()))", "lfg.lua");
				List<string> SubModeLUA = Lua.LuaGetReturnValue("return select(2, select(1, GetLFGMode()))", "lfg.lua");
				
				if (GetLFGMode != null)
				{
					LFGStatus = GetLFGMode[0];
				}
				else
				{
					LFGStatus = null;
					Queued = false;
					roleChecked = false;
				}

				if (SubModeLUA != null)
					SubMode = SubModeLUA[0];
				else
					SubMode = null;					
		
				//Dungeon over detection
				if (MeCheck && Me.PartyMembers.Count < 4 && Me.IsInInstance && LFGStatus == "lfgparty")
				{
					if (!partyTimer.IsRunning)
						partyTimer.Start();
						
					if (partyTimer.ElapsedMilliseconds >= 60000)
					{
						ShouldLeave = true;
					}
				}
				else if (partyTimer.IsRunning)
				{
					partyTimer.Reset();
					ShouldLeave = false;
				}	
				//Not in queue
				if (MeCheck && LFGStatus == null && !Queued && !Me.Auras.ContainsKey("Dungeon Deserter") && !Me.Auras.ContainsKey("Dungeon Cooldown"))
				{
					roleChecked = false;
					if ((MeCheck && Me.IsInParty && Me.IsGroupLeader) || !Me.IsInParty)
					{
						Queued = true;
						Log("Queueing up for random dungeon");
						Lua.DoString("LFDQueueFrame_Join()");
						Thread.Sleep(2000);
						return;
					}
				}
				
				//Dungeon finished
				else if (LFGStatus == "abandonedInDungeon" || ShouldLeave)
				{
					roleChecked = false;
					Log("Dungeon run is over. Waiting 5 seconds before teleporting out.");
					Thread.Sleep(5000);		
					if (MeCheck && Me.IsInParty)
						Lua.DoString("LeaveParty()");
					else
						Lua.DoString("LFGTeleport(true)");
					
					while (MeCheck)
					{
						Thread.Sleep(100);
					}
					
					crashFix();
					return;
				}
				
				//Dungeon invite up
				else if (MeCheck && LFGStatus == "proposal" && SubMode == "unaccepted" && !Me.Combat)
				{
					roleChecked = false;
					Log("Waiting for 5 seconds before accepting dungeon invite");
					Thread.Sleep(5000);
					Log("Accepting dungeon invite");
					Lua.DoString("AcceptProposal()");
					Thread.Sleep(2000);
					
					while (MeCheck)
					{
						SubModeLUA = Lua.LuaGetReturnValue("return select(2, select(1, GetLFGMode()))", "lfg.lua");
						
						if (SubModeLUA != null)
							SubMode = SubModeLUA[0];	
							
						if (SubMode != null && SubMode == "empowered")
							break;
							
						Thread.Sleep(100);						
					}					
					crashFix();				
					return;
				}
				
				//Leader queued for lfd
				else if (MeCheck && LFGStatus == "rolecheck" && !Me.Combat && !roleChecked)
				{
					roleChecked = true;
					Log("Waiting for 5 seconds before accepting role check");
					Thread.Sleep(5000);
					Log("Role Check is in progress");
					Lua.DoString("LFDRoleCheckPopupAcceptButton:Click()");
					Thread.Sleep(2000);
					return;
				}
				
				//In dungeon and running
				else if (MeCheck && LFGStatus == "lfgparty" && Me.IsInInstance)
				{	
					roleChecked = false;
					if (MeCheck && Me.PartyMembers.Count > 0 && Me.IsGroupLeader)
					{
						Log("You are the dungeon guide. Switching it to someone else");
						Lua.DoString("PromoteToLeader('party1')");
						return;
					}
		
					// Tank Assign
					if (MeCheck && Me.IsInParty && Me.IsInInstance)
					{	
						string partyMember = Lua.LuaGetReturnValue("if select(1, UnitGroupRolesAssigned(\"party1\")) then return 1 elseif select(1, UnitGroupRolesAssigned(\"party2\")) then return 2 elseif select(1, UnitGroupRolesAssigned(\"party3\")) then return 3 elseif select(1, UnitGroupRolesAssigned(\"party4\")) then return 4 else return 0 end",
												   "lfg.lua")[0];

						switch (partyMember)
						{
							case "1":
								_tank = Me.PartyMember1;
								break;
							case "2":
								_tank = Me.PartyMember2;
								break;
							case "3":
								_tank = Me.PartyMember3;
								break;
							case "4":
								_tank = Me.PartyMember4;
								break;
							default:
								_tank = null;
								break;
						}
						
						if (tank != _tank)
						{
							if (_tank == null)
							{						
								//To make sure we are following someone after corpse run etc.
								WoWPlayer randomMember = Me.PartyMembers.Find(player => player.IsAlive && player.Distance < 100);
				
								if (randomMember != null && tank != randomMember)
								{
									tank = randomMember;
									RaFHelper.SetLeader(tank);
									soundPlayed = false;
									Log("There is no tank in party. Following random party member");
								}
								else
								{
									Log("There is nobody to follow. You need to move manually !");
									warningSound();
									return;
								}
							}
							else 
							{
								tank = _tank;
								RaFHelper.SetLeader(tank);							
								Log("Tank is set to: {0} with MaxHP: {1}", tank.Class, tank.MaxHealth);
							}
						}
					}
									
					//Target best target and execute pull sequence
					Dictionary<WoWUnit, int> targetList = new Dictionary<WoWUnit, int>();
					List<KeyValuePair<WoWUnit, int>> sortedList = new List<KeyValuePair<WoWUnit, int>>();
					
					foreach (WoWPlayer player in Me.PartyMembers)
					{
						if (player.CurrentTarget != null && !player.CurrentTarget.IsFriendly && player.CurrentTarget.IsTargetingMyPartyMember && player.CurrentTarget.HealthPercent < 100)
						{
							if (targetList.Count > 0 && targetList.ContainsKey(player.CurrentTarget))
							{
								foreach (KeyValuePair<WoWUnit, int> pair in targetList)
								{
									if (pair.Key == player.CurrentTarget)
									{
										targetCount = pair.Value + 1;
										targetList.Remove(player.CurrentTarget);
										targetList.Add(player.CurrentTarget, targetCount);
										break;
									}
								}
							}
							else
							{
								targetList.Add(player.CurrentTarget, 1);
							}
						}
					}
					
					if (targetList.Count > 0)
					{
						sortedList = new List<KeyValuePair<WoWUnit, int>>(targetList);
					}

					
					if (sortedList.Count > 1)
					{
						sortedList.Sort(delegate(KeyValuePair<WoWUnit, int> firstPair, KeyValuePair<WoWUnit, int> nextPair)
								{ return nextPair.Value.CompareTo(firstPair.Value);});
					}
	
					if (sortedList.Count > 0 && LastTarget != sortedList[0].Key)
					{
						LastTarget = sortedList[0].Key;
						LastTarget.Target();
						Thread.Sleep(100);
						

						while (MeCheck && Me.CurrentTarget != null && Me.CurrentTarget.IsAlive && (!Me.CurrentTarget.InLineOfSight || Me.CurrentTarget.Distance > 30))
						{
							safeMoveTo(Me.CurrentTarget);
						}
						
						SequenceManager.CallSequenceExecutor(Sequence.Pull);
					}
				
					// Following tank				
					if (MeCheck && tank != null && tank.IsValid && tank.IsAlive)
					{
						if (tank.Distance > 15 && !Me.Combat && !Me.IsResting)
							safeMoveTo(tank, 15.0f);
						else if (!tank.InLineOfSight && !Me.Combat && !Me.IsResting)
							safeMoveTo(tank);
						else if (Me.Combat && tank.Distance > 35 && !Me.IsCasting)
							safeMoveTo(tank, 30.0f);
					}			
				}
			}
			catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception c)
            {
                Log("An Exception occured. Check debug log for details.");
				dLog("{0}", c);
            }			
        }

        public override string Name { get { return "Instancebuddy"; } }

        public override string Author { get { return "raphus"; } }


        public override Version Version { get { return new Version(0, 1, 2); } }

        public override bool WantButton { get { return false; } }
        public override void OnButtonPress()
        {
        }
    }
}

