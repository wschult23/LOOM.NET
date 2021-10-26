// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;


namespace Loom.WeaverMessages
{
    /// <summary>
    /// Specifies the event type.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// This entry reports an error.
        /// </summary>
        Error,
        /// <summary>
        /// This entry reports a warning.
        /// </summary>
        Warning,
        /// <summary>
        /// This entry is informational.
        /// </summary>
        Info
    }

    /// <summary>
    /// Provides data for the <see cref="Loom.WeaverMessages.WeaverMessageEventHandler">WeaverMessage</see> event handler.
    /// </summary>
    [Serializable]
    public class WeaverMessageEventArgs : EventArgs
    {
        private MessageType evt;
        private string message;
        private int code;

        internal WeaverMessageEventArgs(MessageType evt, int code, string message)
        {
            this.evt = evt;
            this.message = message;
            this.code = code;
        }
        /// <summary>
        /// Gets the <see cref="MessageType">type</see> of the fired event.
        /// </summary>
        /// <seealso cref="MessageType"/>
        public MessageType Type { get { return evt; } }
        /// <summary>
        /// Gets the event's message.
        /// </summary>
        public string Message { get { return message; } }
        /// <summary>
        /// Gets the event's code.
        /// </summary>
        public int Code { get { return code; } }

    }


    /// <summary>
    /// Represents the method that handles the <see cref="Loom.WeaverMessages.WeaverMessageEventHandler">WeaverMessage</see> event handler.
    /// </summary>
    [Serializable]
    public delegate void WeaverMessageEventHandler(object sender, WeaverMessageEventArgs args);
}
