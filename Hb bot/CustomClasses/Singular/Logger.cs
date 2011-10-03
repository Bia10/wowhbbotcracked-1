#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: bobby53 $
// $Date: 2011-04-13 14:07:08 +0200 (on, 13 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/Logger.cs $
// $LastChangedBy: bobby53 $
// $LastChangedDate: 2011-04-13 14:07:08 +0200 (on, 13 apr 2011) $
// $LastChangedRevision: 273 $
// $Revision: 273 $

#endregion

using System.Drawing;

using Singular.Settings;

using Styx.Helpers;

namespace Singular
{
    internal class Logger
    {
        public static void Write(string message)
        {
            Write(Color.Lime, message);
        }

        public static void Write( System.Drawing.Color clr, string message)
        {
            Logging.Write( clr, "[Singular] " + message);
        }

        public static void WriteDebug(string message)
        {
            if (!SingularSettings.Instance.EnableDebugLogging)
            {
                return;
            }

            Logging.WriteDebug(Color.Lime, "[Singular-DEBUG] " + message);
        }
    }
}