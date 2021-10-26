// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Text;
using Loom.JoinPoints;
using System.Runtime.Serialization;

namespace Loom
{
    /// <summary>
    /// The exception that is thrown when the aspect wever can't create an instance.
    /// </summary>
    [Serializable]
    public class AspectWeaverException : Exception, ISerializable
    {
        private int errorcode;

        /// <summary>
        /// Represents the error code of the thrown exception.
        /// </summary>
        public int ErrorCode
        {
            get { return errorcode; }
        }

        /// <summary>
        /// Internal use only!!
        /// </summary>
        public AspectWeaverException(SerializationInfo info, StreamingContext context):
            base(info,context)
        {
            this.errorcode = info.GetInt32("errorcode");
        }
        /// <summary>
        /// Internal use only!!
        /// </summary>
        public AspectWeaverException(int errorcode, string msg, params string[] args)
            : base(BuildMessage(msg, args))
        {
            this.errorcode = errorcode;
            AspectWeaver.WriteWeaverMessage(Loom.WeaverMessages.MessageType.Error, errorcode, msg, args);
        }
        /// <summary>
        /// Internal use only!!
        /// </summary>
        public AspectWeaverException(int errorcode, string msg, Exception inner, params string[] args)
            : base(BuildMessage(msg, args), inner)
        {
            this.errorcode = errorcode;
            AspectWeaver.WriteWeaverMessage(Loom.WeaverMessages.MessageType.Error, errorcode, msg, args);
        }

        static internal string BuildMessage(string msg, object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, (object[])args);
            return sb.ToString();
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("errorcode",this.errorcode);
        }
    }

    /// <summary>
    /// The exception that is thrown by methods invoked through the aspect context.
    /// </summary>
    public class MethodInvocationException : Exception
    {
        internal MethodInvocationException(string msg)
            :
            base(msg)
        {
        }

        internal MethodInvocationException(string msg, Exception inner)
            :
            base(msg, inner)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when there is an attempt to dynamically access a method or a constructor that does not exist.
    /// </summary>
    public class MissingTargetMethodException : MissingMethodException
    {
        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="ctx">Internal use.</param>
        /// <param name="args">Internal use</param>
        public MissingTargetMethodException(Context ctx, object[] args)
            :
            base(GetErrorMessage(ctx, args))
        {
            base.ClassName = ctx.TargetClass.FullName;
            base.MemberName = ctx.CurrentMethod.ToString();
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="args">Internal use</param>
        public MissingTargetMethodException(object[] args)
            :
            base(GetErrorMessage(null, args))
        {
        }

        private static string GetErrorMessage(Context ctx, object[] args)
        {
            StringBuilder sb = new StringBuilder();
            if(ctx!=null)
            {
                sb.Append(ctx.TargetClass.FullName);
                sb.Append(".");
                sb.Append(ctx.CurrentMethod.ToString());
                sb.Append("(");
            }
            bool bFirst = true;
            foreach (object obj in args)
            {
                if (!bFirst)
                {
                    sb.Append(", ");
                }
                else bFirst = false;
                if (obj == null)
                {
                    sb.Append("*");
                }
                else
                {
                    sb.Append(obj.GetType().FullName);
                }
            }
            sb.Append(")");
            return string.Format(Resources.Errors.ERR_9002, sb);
        }
    }

    /// <summary>
    /// Internal use
    /// </summary>
    public class InvalidInvocationArgsException : System.ArgumentException
    {
        /// <summary>
        /// Internal use.
        /// </summary>
        public InvalidInvocationArgsException()
            : base(Resources.Errors.ERR_9003)
        {
        }
    }
}
