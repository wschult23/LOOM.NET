// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Authentication
{
    /// <summary>
    /// Represents a simple authenticator to verify user access.
    /// </summary>
    public class Authenticator
    {
        public static bool Authenticate(NetworkCredential Credential)
        {
            // very simple authentication mechanism
            if (Credential.UserName == "Peter" && Credential.Password == "Pan")
            {
                Console.WriteLine(String.Format("*** Authenticated as {0}.", Credential.UserName));
                return true;
            }
            else
            {
                Console.WriteLine(String.Format("*** Unable to authenticate as {0}.", Credential.UserName));
                return false;
            }
        }
    }

    /// <summary>
    /// User defined authentication exception is thrown, when authentication failed
    /// </summary>
    public class AuthenticationException : ApplicationException
    {
        public AuthenticationException(string Message)
            : base(Message)
        {
        }
    }
}
