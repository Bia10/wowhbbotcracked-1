#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: apoc $
// $Date: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/Settings/WarlockSettings.cs $
// $LastChangedBy: apoc $
// $LastChangedDate: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $LastChangedRevision: 190 $
// $Revision: 190 $

#endregion

using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace Singular.Settings
{
    internal class WarlockSettings : Styx.Helpers.Settings
    {
        public WarlockSettings()
            : base(SingularSettings.SettingsPath + "_Warlock.xml")
        {
        }
    }
}