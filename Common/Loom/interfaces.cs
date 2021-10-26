// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;

namespace Loom
{
    /// <summary>
    /// Use this interface to retrieve all interwoven aspects of an object.
    /// </summary>
    /// <remarks>
    /// If a class becomes interwoven, the weaver implements this interface to the interwoven class. 
    /// If an object does not implement this interface then there is no aspect interwoven.
    /// </remarks>
    /// <example>
    /** This example shows how to obtain all aspects interwoven to an instance object:
        <code>
        IAspectInfo iinfo = obj as IAspectInfo;
        
        if (iinfo != null)
        {
            // object is interwoven, so get all aspects
            Aspect[] aspects = iinfo.GetAspects();
        }
        else
        {
            // there is no aspect interwoven
        }
        </code>*/
    /// </example>
    /// <seealso cref="GetAspects()"/>
    /// <seealso cref="IAspectInfo.GetAspects()"/>
    public interface IAspectInfo
    {
        /// <summary>
        /// Returns all aspects in a given interwoven instance.
        /// </summary>
        /// <returns>an array of aspects</returns>
        /// <example>
        /** This example shows how to obtain all aspects interwoven to an instance object:
            <code>
            IAspectInfo iinfo = obj as IAspectInfo;
        
            if (iinfo != null)
            {
                // object is interwoven, so get all aspects
                Aspect[] aspects = iinfo.GetAspects();
            }
            else
            {
                // there is no aspect interwoven
            }
            </code>*/
        /// </example>
        Aspect[] GetAspects();

        /// <summary>
        /// Searches for specified aspects in an interwoven instance.
        /// </summary>
        /// <param name="aspecttype">the type of the aspect to look for</param>
        /// <returns>an array of aspects</returns>
        /// <example>
        /** This example shows how to obtain all aspects of one type interwoven to an instance object:
            <code>
         
            public class BaseAspect : Aspect
            {
                ...
            }
         
            public class Aspect1 : BaseAspect
            { 
                ...
            }
         
            public class Aspect2 : BaseAspect
            { 
                ...
            }
         
         
            public class TargetType
            {
                ...
            }
         
            ...
         
            // create an array of aspects
            Aspect[] aspects = new Aspect[2];
            aspects[0] = new Aspect1();
            aspects[1] = new Aspect2();

            // interweaves Aspect1 and Aspect2 with target class
            TargetType obj = Weaver.Create(aspects);
         
            IAspectInfo iinfo = obj as IAspectInfo;
        
            if (iinfo != null)
            {
                // object is interwoven, so get all aspects of type BaseAspect
                Aspect[] aspects1 = iinfo.GetAspects(typeof(BaseAspect));
            }
            else
            {
                // there is no aspect interwoven
            }
            </code>*/
        /// </example>
        Aspect[] GetAspects(Type aspecttype);

        /// <summary>
        /// Gets the target class <see cref="Type"/> of the current instance.
        /// </summary>
        Type TargetClass
        {
            get;
        }
    }
}
