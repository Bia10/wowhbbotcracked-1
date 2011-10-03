#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: apoc $
// $Date: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/Settings/WarriorSettings.cs $
// $LastChangedBy: apoc $
// $LastChangedDate: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $LastChangedRevision: 190 $
// $Revision: 190 $

#endregion

using System.ComponentModel;

using Styx.Helpers;

using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace Singular.Settings
{
    internal class WarriorSettings : Styx.Helpers.Settings
    {
        public WarriorSettings()
            : base(SingularSettings.SettingsPath + "_Warrior.xml")
        {
        }

        [Setting]
        [Styx.Helpers.DefaultValue(50)]
        [Category("Protection")]
        [DisplayName("Enraged Regeneration Health")]
        [Description("Enrage Regeneration will be used when your health drops below this value")]
        public int WarriorEnragedRegenerationHealth { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Protection")]
        [DisplayName("Shield Wall Health")]
        [Description("Shield Wall will be used when your health drops below this value")]
        public int WarriorProtShieldWallHealth { get; set; }
    }
}