#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: apoc $
// $Date: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/Composites/ActionLogMessage.cs $
// $LastChangedBy: apoc $
// $LastChangedDate: 2011-03-18 17:36:36 +0100 (fr, 18 mar 2011) $
// $LastChangedRevision: 190 $
// $Revision: 190 $

#endregion

using TreeSharp;

namespace Singular.Composites
{
    public delegate string LogMessageRetriever();

    internal class ActionLogMessage : Action
    {
        private readonly bool _debug;

        private readonly string _message;
        private readonly LogMessageRetriever _messageGrabber;

        public ActionLogMessage(bool debug, string message)
        {
            _message = message;
            _debug = debug;
        }

        public ActionLogMessage(bool debug, LogMessageRetriever message)
        {
            _messageGrabber = message;
            _debug = debug;
        }

        protected override RunStatus Run(object context)
        {
            if (_debug)
            {
                Logger.WriteDebug(_messageGrabber != null ? _messageGrabber() : _message);
            }
            else
            {
                Logger.Write(_messageGrabber != null ? _messageGrabber() : _message);
            }

            if (Parent is Selector)
            {
                return RunStatus.Failure;
            }
            return RunStatus.Success;
        }
    }
}