#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: apoc $
// $Date: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/Settings/RogueSettings.cs $
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
    internal class RogueSettings : Styx.Helpers.Settings
    {
        public RogueSettings()
            : base(SingularSettings.SettingsPath + "_Rogue.xml")
        {
        }

        [Setting]
        [Styx.Helpers.DefaultValue(PoisonType.Instant)]
        [Category("Common")]
        [DisplayName("Main Hand Poison")]
        [Description("Main Hand Poison")]
        public PoisonType MHPoison { get; set; }

        [Setting]
        [DefaultValue(PoisonType.Instant)]
        [Category("Common")]
        [DisplayName("Off Hand Poison")]
        [Description("Off Hand Poison")]
        public PoisonType OHPoison { get; set; }
    }
}