// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Text;
using System.Collections;
using Loom.WeaverMessages;

namespace Loom
{
    /// <summary>
    /// This is the abstract definition of an aspect weaver.
    /// </summary>
    [Serializable]
    public static class AspectWeaver
    {
        /// <summary>
        /// Internal use.
        /// </summary>
        public  interface IAspectWeaverEvent
        {
            /// <summary>
            /// Internal use.
            /// </summary>
            bool HasTarget {get; }
            /// <summary>
            /// Internal use.
            /// </summary>
            event WeaverMessageEventHandler WeaverMessages;
            /// <summary>
            /// Internal use.
            /// </summary>
            void Fire(WeaverMessageEventArgs arg);
        }

        internal static IAspectWeaverEvent EventHandler; 

        /// <summary>
        /// Use this event handler to get Loom weaver messages.
        /// </summary>
        /// <example>
        /// This example shows how to use weaver-message event handler to obtain messages from the weaver:
        /**
        <code>

            ...         
            
            /// define event handler and create a handler method
            var eventHandler = new Loom.WeaverMessages.WeaverMessageEventHandler(Weaver_WeaverMessages);

            /// register event handler
            AspectWeaver.WeaverMessages += eventHandler;
            
            /// do your stuff here ...
            
            /// unregister event handler
            AspectWeaver.WeaverMessages -= eventHandler;

            ...      
        
        /// the handler method for the event
        static void Weaver_WeaverMessages(object sender, Loom.WeaverMessages.WeaverMessageEventArgs args)
        {
            Console.WriteLine("{0}: {1}", args.Type, args.Message);
        }

          
        </code> */
        /// </example>
        /// <seealso cref="Loom.WeaverMessages.WeaverMessageEventArgs"/>
        /// <seealso cref="Loom.WeaverMessages.MessageType"/>
        public static event WeaverMessageEventHandler WeaverMessages
        {
            add
            {
                EventHandler.WeaverMessages += value;
            }
            remove
            {
                EventHandler.WeaverMessages -= value;
            }
        }


        internal static void WriteWeaverMessage(MessageType type, int code, string message, params string[] args)
        {
#if !Debug
            if (!EventHandler.HasTarget) return;
#endif
            System.Diagnostics.Debug.Assert(EventHandler!= null);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(message, (object[])args);

            System.Diagnostics.Debug.WriteLine(sb.ToString());
            EventHandler.Fire(new WeaverMessageEventArgs(type, code, sb.ToString()));

        }
    }
}