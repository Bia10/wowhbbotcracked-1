using System.Collections.Generic;
using System.Threading;
using Styx.Logic.Pathing;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;


namespace Styx.Bot.Quest_Behaviors
{
    public class UseAoeCircle : CustomForcedBehavior
    {
        #region Overrides of CustomForcedBehavior


        readonly bool _success = true;

        public UseAoeCircle(Dictionary<string, string> args)
            : base(args)
        {
			ItemId      = GetAttributeAsItemId("ItemId", true, null) ?? 0;
			FirePoint    = GetXYZAttributeAsWoWPoint("", true, null) ?? WoWPoint.Empty;
        }

        public WoWPoint FirePoint { get; private set; }
        public int ItemId { get; set; }

        private int _counter;
        private Composite _root;
        protected override Composite CreateBehavior()
        {
            return _root ?? (_root =
                new Decorator(ret => _counter == 0,
                    new Action(delegate
                        {
							Lua.DoString("UseItemByName(\"" + ItemId + "\")");
							Thread.Sleep(300);
                            Styx.Logic.Combat.LegacySpellManager.ClickRemoteLocation(FirePoint);
                            Thread.Sleep(300);
							_counter++;
                        }))
                    );
        }

        public override bool IsDone { get  { return _counter >= 1; } }

        #endregion
    }
}

