// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Linq;
using Loom.CodeBuilder.Rapier;
using Loom.Common;
using Loom.Runtime;
using System.Configuration;
using Loom.Configuration;
using Loom.JoinPoints.Implementation;
using System.IO;
using Loom.WeaverMessages;

namespace Loom
{
    /// <summary>
    /// The Weaver class contains functions to interweave one or more aspect classes with a targettype class. 
    /// </summary>
    /// 
    public static class Weaver 
    {
        /// <summary>
        /// The internal message facility
        /// </summary>
        class WeaverMessageFacility : AspectWeaver.IAspectWeaverEvent
        {
            public event WeaverMessageEventHandler WeaverMessages;

            public void Fire(Loom.WeaverMessages.WeaverMessageEventArgs arg)
            {
                System.Diagnostics.Debug.Assert(WeaverMessages != null);
                WeaverMessages(this, arg);
            }

            public bool HasTarget
            {
                get { return WeaverMessages !=null; }
            }
        }
        static WeaverMessageFacility messages = new WeaverMessageFacility();

        /// <summary>
        /// Use this event handler to get Loom weaver messages.
        /// </summary>
        /// <example>
        /// This example shows how to use weaver-message event handler to obtain messages from the weaver:
        /**
        <code>

            ...         
            
            /// define event handler and create a handler method
            Loom.WeaverMessages.WeaverMessageEventHandler eventHandler = new Loom.WeaverMessages.WeaverMessageEventHandler(Weaver_WeaverMessages);

            /// register event handler
            Weaver.WeaverMessages += eventHandler;
            
            /// do your stuff here ...
            
            /// unregister event handler
            Weaver.WeaverMessages -= eventHandler;

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
            add { messages.WeaverMessages+=value; }
            remove { messages.WeaverMessages-=value; }
        }


        class ClassObjectDictionary : Dictionary<Type, ClassObject> { };
        // statics 

        /// <summary>
        /// 1st level cache for all static interwoven types
        /// </summary>
        private static ClassObjectDictionary c_staticWovenTypesCache = new ClassObjectDictionary();

        /// <summary>
        /// Lock for internal data structures
        /// </summary>
        private static ReaderWriterLock c_lockCaches = new ReaderWriterLock();

        /// <summary>
        /// the dynamic modules
        /// </summary>
        private static Dictionary<Assembly, ModuleBuilder> c_moduleBuilders = new Dictionary<Assembly, ModuleBuilder>();

        static Weaver()
        {
            AspectWeaver.EventHandler = messages;
            Loom.Common.ReflectionObjects.Initialize(typeof(AbstractClassException));
            Loom.JoinPoints.Implementation.JoinPointCollection.Initialize(new Loom.CodeBuilder.DynamicProxy.JoinPoints.ProxyAspectCoverageInfoFactory());

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var loomsection = config.Sections["loom"] as LoomSection;
            if (loomsection != null)
            {
                try
                {
                    c_lockCaches.AcquireWriterLock(0);
                    JoinPointCollection.ApplyConfiguration(loomsection.Config);
                }
                finally
                {
                    c_lockCaches.ReleaseWriterLock();
                }
            }
        }

#if DEBUG
        /// <summary>
        /// for test purposes to save the dynamic modules
        /// </summary>
        public static void SaveAssembly()
        {
            foreach (KeyValuePair<Assembly, ModuleBuilder> kv in c_moduleBuilders)
            {
                ((AssemblyBuilder)kv.Value.Assembly).Save(Path.GetFileName(kv.Value.FullyQualifiedName));
            }
        }

        /// <summary>
        /// for test purposes to save the dynamic modules
        /// </summary>
        public static void SaveAssembly(Assembly assembly)
        {
            ((AssemblyBuilder)c_moduleBuilders[assembly].Assembly).Save(Common.WeavingCodeNames.c_assemblyNamePrefix + assembly.FullName);
        }

        /// <summary>
        /// for test purposes to create a class object
        /// </summary>
        public static Type CreateInterwovenType(Type classtype, object[] args, Aspect[] aspectarr)
        {
            ClassObject activator = GetClassObject(classtype, aspectarr);
            return activator.Type;
        }

#endif

        /// <summary>
        /// Returns a module buider for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>the module builder</returns>
        private static ModuleBuilder GetModuleBuilder(Type type)
        {
            ModuleBuilder theBuilder = null;
            if (c_moduleBuilders.TryGetValue(type.Assembly, out theBuilder))
            {
                return theBuilder;
            }
            AssemblyName an = new AssemblyName(Common.WeavingCodeNames.c_assemblyNamePrefix + type.Assembly.FullName);
            AssemblyBuilder assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            theBuilder = assemblybuilder.DefineDynamicModule(WeavingCodeNames.c_moduleName, Common.WeavingCodeNames.c_assemblyNamePrefix + type.Assembly.FullName, true);
            c_moduleBuilders.Add(type.Assembly, theBuilder);
            return theBuilder;
        }

        /// <param name="classtype">The targettype of the targettype class.</param>
        /// <include file="doc/weaver.xml" path="doc/createinstance/*"/>
        /// <include file="doc/weaverremarks.xml" path="doc/createremarks/*"/>
        public static object CreateInstance(Type classtype)
        {
            return CreateInstance(classtype, null, (Aspect[])null);
        }

        /// <param name="classtype">The targettype of the targettype class.</param>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <include file="doc/weaver.xml" path="doc/createinstance/*"/>
        /// <include file="doc/weaverremarks.xml" path="doc/createremarks/*"/>
        public static object CreateInstance(Type classtype, object[] args)
        {
            return CreateInstance(classtype, args, (Aspect[])null);
        }


        /// <param name="classtype">The targettype of the targettype class.</param>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <param name="aspect">An aspect to interweave with the targettype class</param>
        /// <include file="doc/weaver.xml" path="doc/createinstance/*"/>
        /// <include file="doc/weaverremarks.xml" path="doc/createremarks/*"/>
        public static object CreateInstance(Type classtype, object[] args, Aspect aspect)
        {
            return CreateInstance(classtype, args, new Aspect[] { aspect });
        }

        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked. </param>
        /// <include file="doc/weaver.xml" path="doc/create/*"/>
        /// <include file="doc/weaverremarks.xml" path="doc/createremarks/*"/>
        public static T Create<T>(params object[] args) where T : class
        {
            return (T)CreateInstance(typeof(T), args, (Aspect[])null);
        }

        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <param name="aspect">An aspect to interweave with the targettype class.</param>
        /// <include file="doc/weaver.xml" path="doc/create/*"/>
        /// <include file="doc/weaverremarks.xml" path="doc/createremarks/*"/>
        public static T Create<T>(Aspect aspect, params object[] args) where T : class
        {
            return (T)CreateInstance(typeof(T), args, new Aspect[] { aspect });
        }

        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <param name="aspectarray">An array of aspects to interweave with the targettype class.</param>
        /// <include file="doc/weaver.xml" path="doc/create/*"/>
        /// <include file="doc/weaverremarks.xml" path="doc/createremarks/*"/>
        public static T Create<T>(Aspect[] aspectarray, params object[] args) where T : class
        {
            return (T)CreateInstance(typeof(T), args, aspectarray);
        }


        /// <param name="classtype">The targettype of the targettype class.</param>
        /// <param name="args">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If <paramref name="args"/> is an empty array or <value>NULL</value>, the constructor that takes no parameters (the default constructor) is invoked.</param>
        /// <param name="aspectarray">An array of aspects to interweave with the targettype class.</param>
        /// <include file="doc/weaver.xml" path="doc/createinstance/*"/>
        /// <include file="doc/weaverremarks.xml" path="doc/createremarks/*"/>
        public static object CreateInstance(Type classtype, object[] args, Aspect[] aspectarray)
        {
            if (!classtype.IsClass)
            {
                throw new AspectWeaverException(1001, Loom.Resources.CodeBuilderErrors.ERR_1001, classtype.ToString());
            }
            ClassObject activator = GetClassObject(classtype, aspectarray);
            return activator.CreateInstance(args, aspectarray);
        }

        /// <summary>
        /// Creates a class object that can create interwoven instances
        /// </summary>
        /// <param name="classtype"></param>
        /// <param name="dynamicaspects"></param>
        /// <returns></returns>
        internal static ClassObject GetClassObject(Type classtype, Aspect[] dynamicaspects)
        {
            if (!classtype.IsPublic && !classtype.IsNestedPublic)
            {
                throw new AspectWeaverException(1011, Loom.Resources.CodeBuilderErrors.ERR_1011, classtype.FullName);
            }
            if (classtype.IsSealed)
            {
                throw new AspectWeaverException(1007, Loom.Resources.CodeBuilderErrors.ERR_1007, classtype.FullName);
            }

            ClassObject activator;
            // load static parts from cache
            c_lockCaches.AcquireReaderLock(-1);
            try
            {
                activator = GetClassObject(classtype);

                // add dynamic parts
                if (dynamicaspects != null)
                {
                    activator = InterweaveAspect(activator, dynamicaspects);
                }
            }
            finally
            {
                c_lockCaches.ReleaseReaderLock();
            }

            return activator;
        }

        /// <summary>
        /// Gets the class object for the static part of <paramref name="classtype"/>
        /// </summary>
        /// <param name="classtype"></param>
        /// <returns></returns>
        private static ClassObject GetClassObject(Type classtype)
        {
            ClassObject activator;
            if (c_staticWovenTypesCache.TryGetValue(classtype, out activator))
            {
                return activator;
            }

            LockCookie lc = c_lockCaches.UpgradeToWriterLock(-1);
            try
            {
                if (!c_staticWovenTypesCache.TryGetValue(classtype, out activator))
                {
                    // never created -> interweave
                    try
                    {
                        ModuleBuilder module = GetModuleBuilder(classtype);
                        RLProxyTypeBuilder ptb = new RLProxyTypeBuilder(module, classtype);
                        ptb.Interweave();
                        activator = ptb.CreateType();
                        c_staticWovenTypesCache.Add(classtype, activator);
                    }
                    catch (Exception e)
                    {
                        activator = new ClassObjectException(classtype, e);
                        c_staticWovenTypesCache.Add(classtype, activator);
                        throw e;
                    }
                }
            }
            finally
            {
                c_lockCaches.DowngradeFromWriterLock(ref lc);
            }
            return activator;
        }

        /// <summary>
        /// Gets a ClassObject (safe version).
        /// </summary>
        /// <param name="targetclass">the target class type of introspection</param>
        /// <returns>null</returns>
        internal static ClassObject GetClassObjectSafe(Type targetclass)
        {
            c_lockCaches.AcquireReaderLock(-1);
            try
            {
                var clobj = GetClassObject(targetclass);
                // GetAspects mit True aufufen, da sonst die dynamischen Aspekte instantiiert werden w�rden
                return clobj;
            }
            finally
            {
                c_lockCaches.ReleaseReaderLock();
            }
        }


        /// <summary>
        /// Returns a list of static aspects that covering a target class.
        /// </summary>
        /// <remarks>
        /// This method returns static aspects of a target class. Static aspects are aspects with a <see cref="Loom.AspectProperties.CreateAspectAttribute"/> and not using <see cref="Loom.AspectProperties.Per.Instance"/> or <see cref="Loom.AspectProperties.Per.InstanceAndAnnotation"/>.
        /// </remarks>
        /// <param name="targetclass">the target class type of introspection</param>
        /// <param name="aspecttype">the type of the aspect to look for</param>
        /// <returns>array of aspects</returns>
        public static Aspect[] GetAspects(Type targetclass, Type aspecttype)
        {
            return GetClassObjectSafe(targetclass).GetAspects(true).Where(aspect => aspect != null && (aspect.GetType() == aspecttype || aspecttype.IsSubclassOf(aspecttype))).ToArray();
        }


        /// <summary>
        /// Returns a list of static aspects that covering a target class.
        /// </summary>
        /// <remarks>
        /// This method returns static aspects of a target class. Static aspects are aspects with a <see cref="Loom.AspectProperties.CreateAspectAttribute"/> and not using <see cref="Loom.AspectProperties.Per.Instance"/> or <see cref="Loom.AspectProperties.Per.InstanceAndAnnotation"/>.
        /// </remarks>
        /// <param name="targetclass">the target class type of introspection</param>
        /// <returns>array of aspects</returns>
        public static Aspect[] GetAspects(Type targetclass)
        {
            return GetClassObjectSafe(targetclass).GetAspects(true).Where(aspect => aspect != null).ToArray();
        }
        
        /// <summary>
        /// interweaves new aspects to a target type represeted by a class object
        /// </summary>
        /// <param name="baseclassobject">The class object of the target type</param>
        /// <param name="aspectarr">An array of aspects to be interwoven</param>
        /// <returns></returns>
        private static ClassObject InterweaveAspect(ClassObject baseclassobject, Aspect[] aspectarr)
        {
            int iPos;
            ClassObject classobject = baseclassobject;
            // lookup existing class objects (if the same combination was already interwoven)
            for (iPos = 0; iPos < aspectarr.Length; iPos++)
            {
                classobject = baseclassobject.GetClassObject(aspectarr[iPos].GetType());
                if (classobject == null) break;
                baseclassobject = classobject;
            }

            if (classobject != null) return classobject;

            // Nothing found, start weaving
            LockCookie lc = c_lockCaches.UpgradeToWriterLock(-1);
            try
            {
                // we should retry the lookup, it could be that another thread has already done our job
                for (; ; )
                {
                    classobject = baseclassobject.GetClassObject(aspectarr[iPos].GetType());

                    if (classobject == null)
                    {
                        break;
                    }

                    // handle the unlikely event of concurrent interweaving of the same type
                    iPos++;
                    if (iPos >= aspectarr.Length)
                    {
                        return classobject;
                    }
                    baseclassobject = classobject;
                }

                // Build the intervoven proxy type
                ModuleBuilder module = GetModuleBuilder(baseclassobject.TargetClass);
                RLProxyTypeBuilder ptb = new RLProxyTypeBuilder(module, baseclassobject);
                for (; iPos < aspectarr.Length; iPos++)
                {
                    Aspect aspect = aspectarr[iPos];

                    try
                    {
                        ptb.Interweave(aspect);
                        classobject = ptb.CreateType();

                        baseclassobject.AddClassObject(aspect.GetType(), classobject);

                        baseclassobject = classobject;

                    }
                    catch (AspectWeaverException e)
                    {
                        baseclassobject.AddClassObject(aspect.GetType(), new ClassObjectException(baseclassobject.Type, e));
                        throw (e);
                    }
                    catch (Exception e)
                    {
                        e = new AspectWeaverException(9000, Loom.Resources.Errors.ERR_9000, e);
                        baseclassobject.AddClassObject(aspect.GetType(), new ClassObjectException(baseclassobject.Type, e));
                        throw (e);
                    }
                }
            }
            finally
            {
                c_lockCaches.DowngradeFromWriterLock(ref lc);
            }

            return baseclassobject;
        }

        /// <include file="doc/weaver.xml" path="doc/getclassfactory/*"/>
        public static IClassFactory<T> GetClassFactory<T>() where T : class
        {
            return GetClassFactory<T>(new Aspect[0]);
        }

        /// <include file="doc/weaver.xml" path="doc/getclassfactory/*"/>
        /// <param name="dynamicaspects">An array of aspects to interweave with the targettype class.</param>
        public static IClassFactory<T> GetClassFactory<T>(Aspect[] dynamicaspects) where T : class
        {
            try
            {
                // try to obtain the lock to inspect the caches fo a valid classs object
                c_lockCaches.AcquireReaderLock(0);
                try
                {
                    ClassObject classobject = null;
                    if(c_staticWovenTypesCache.TryGetValue(typeof(T), out classobject))
                    {
                        foreach(Aspect asp in dynamicaspects)
                        {
                            classobject = classobject.GetClassObject(asp.GetType());
                            if (classobject == null) break;
                        }
                    }
                    if (classobject == null)
                    {
                        // The class is still not interwoven, we do it asyncronously
                        return new AsyncClassFactory<T>(dynamicaspects);
                    }
                    // we found it
                    return new ClassFactory<T>(classobject, dynamicaspects);
                }
                finally
                {
                    c_lockCaches.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                // the caches are locked, so we do it asyncronous 
                return new AsyncClassFactory<T>(dynamicaspects);
            }
        }

        /// <summary>
        /// Searches for specified aspects in a given interwoven instance.
        /// </summary>
        /// <param name="obj">the instance to search</param>
        /// <param name="aspecttype">the type of the aspect to look for</param>
        /// <returns>an array of aspects</returns>
        public static Aspect[] GetAspects(object obj, Type aspecttype)
        {
            IAspectInfo iaspinfo = obj as IAspectInfo;
            return iaspinfo != null ? iaspinfo.GetAspects(aspecttype) : new Aspect[0];
        }

        /// <summary>
        /// Returns all aspects in a given interwoven instance.
        /// </summary>
        /// <param name="obj">the instance to search</param>
        /// <returns>an array of aspects</returns>
        public static Aspect[] GetAspects(object obj)
        {
            return GetAspects(obj, null);
        }

        /// <summary>
        /// Retrieves an object representing the original type of an interwoven class.
        /// </summary>
        /// <param name="interwovenType">the type object of the interwoven class</param>
        /// <returns>a type object that represents the uninterwoven target class</returns>
        public static Type GetTargetClass(Type interwovenType)
        {
            return interwovenType.GetInterfaces().FirstOrDefault(t=>t==typeof(IAspectInfo)) != null ? GetTargetClass(interwovenType.BaseType) : interwovenType;
        }

        /// <summary>
        /// Retrieves an object representing the original type of an interwoven instance. If the instance isn't interwoven the result will be 
        /// the same as <see cref="Object.GetType"/>.
        /// </summary>
        /// <param name="instance">the interwoven instance</param>
        /// <returns>the type of the current instance if it wouldn't be interwoven</returns>
        public static Type GetTargetClass(object instance)
        {
            IAspectInfo ai = instance as IAspectInfo;
            return ai == null ? instance.GetType() : ai.TargetClass;
        }

        /// <summary>
        /// Gets a value indicating whether the object is interwoven or not.
        /// </summary>
        /// <param name="obj">the object to inspect</param>
        /// <returns>True, if the object is interwoven and so implements the <see cref="Loom.IAspectInfo"/> interface; <br />
        /// false, if the object is not interwoven
        /// </returns>
        /// <seealso cref="Loom.IAspectInfo"/>
        public static bool IsObjectInterwoven(object obj)
        {
            return obj is IAspectInfo;
        }


        /// <summary>
        /// Searches for aspects of the given type in this instance.
        /// </summary>
         /// <param name="obj">the instance to inspect</param>
        /// <typeparam name="ASPECTTYPE">the expected aspect type</typeparam>
        /// <returns>enumeration of aspects</returns>
        public static IEnumerable<ASPECTTYPE> Aspects<ASPECTTYPE>(this object obj) where ASPECTTYPE : Aspect
        {
            return Weaver.GetAspects(obj, typeof(ASPECTTYPE)).Cast<ASPECTTYPE>();
        }

        /// <summary>
        /// Searches for aspects covering this <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">the type to inspect</param>
        /// <typeparam name="ASPECTTYPE">the expected aspect type</typeparam>
        /// <returns>enumeration of aspects</returns>
        public static IEnumerable<ASPECTTYPE> StaticAspects<ASPECTTYPE>(this Type type) where ASPECTTYPE : Aspect
        {
            return GetClassObjectSafe(type).GetAspects(true).Where(aspect => aspect != null && (aspect.GetType() == typeof(ASPECTTYPE) || aspect.GetType().IsSubclassOf(typeof(ASPECTTYPE)))).Cast<ASPECTTYPE>();
        }
    }
}
