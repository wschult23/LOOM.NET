// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using Loom;
using Loom.JoinPoints;
using System.Net;

namespace Authentication
{
    /// <summary>
    /// This Authentication aspect shows, how to implement an authentication aspect to control the access of 
    /// application methods, simply by annotating the target methods with this aspect. The aspect checks 
    /// the entered username and password and caches the credentials for reuse.
    /// </summary>
    public class AuthenticationAspect : AspectAttribute
    {
        // caches the user credentials
        private NetworkCredential credential;

        [IncludeAll]            // includes all methods
        [Call(Advice.Around)]   // aspect method is called instead of the target method
        public T AuthenticateAction<T>([JPContext] Context context, params object[] parameters)
        {
            Console.WriteLine("*** Action needs authentication.");

            // check for already cached credentials
            if (credential == null)
            {
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                credential = new NetworkCredential(username, password);  // store credentials to aspect
            }
            
            if (Authenticator.Authenticate(credential))     // authenticate the user
                return (T)context.Invoke(parameters);       // invoke the original method and return the result
            else
                throw new AuthenticationException("Error: Authentication failed.");  // or throw an authentication exception
        }
        
    }
}
