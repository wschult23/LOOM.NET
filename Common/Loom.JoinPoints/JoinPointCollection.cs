// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using Loom.AspectModel;
using Loom.Configuration;
using System.Reflection.Emit;
using System.Collections;


namespace Loom.JoinPoints.Implementation
{
    /// <summary>
    /// Repräsentiert alle JoinPoints einer Klasse
    /// </summary>
    internal class JoinPointCollection : IEnumerable<MethodBaseJoinPoint>
    {
        class JoinPointDictionary : Dictionary<MethodBase, MethodInfoJoinPoint> { }
        delegate IEnumerable<AspectCoverageInfo> GetCustomAttributeAspectsDelegate(MemberInfo provider);

        /// <summary>
        /// Configuriert, ob <see cref="MethodJoinPoints"/> auch Aspekte liefern soll
        /// </summary>
        public static bool EnableJoinPointsOnNonVirtualMethods = false;

        /// <summary>
        /// Statische Caches
        /// </summary>
        static Dictionary<Type, JoinPointCollection> c_jpcollectioncache = new Dictionary<Type, JoinPointCollection>();
        static Dictionary<Type, InterfaceJoinPointCollection> c_jpifccache = new Dictionary<Type, InterfaceJoinPointCollection>();
        static Dictionary<Type, AspectClass> c_aspectclasscache = new Dictionary<Type, AspectClass>();

        /// <summary>
        /// Zuordnung der Aspekten zu den Assemblies
        /// </summary>
        internal static Dictionary<Assembly, AssemblyAspectInfo> c_assemblyInfo = new Dictionary<Assembly, AssemblyAspectInfo>();
        /// <summary>
        /// Dictionary der vorkonfigurierten Assemblies
        /// </summary>
        internal static Dictionary<AssemblyName, AssemblyConfig> c_assemblyConfig = new Dictionary<AssemblyName, AssemblyConfig>();
        /// <summary>
        /// Factorty um AspectInfo-Objecte zu erzeugen.
        /// </summary>
        internal static IJoinPointClassFactory c_aspectinfofactory;

        /// <summary>
        /// die JoinPoints
        /// </summary>
        private MethodInfoJoinPoint[] vtablejoinpoints;
        private ConstructorJoinPoint[] ctorjoinpoints;
        private InterfaceJoinPoints[] interfacejoinpoints;
        private MethodInfoJoinPoint[] methodjoinpoints;
        /// <summary>
        /// Der Typ für den die JP gültig sind
        /// </summary>
        private Type type;
        
        /// <summary>
        /// Die Collection des Basistypen
        /// </summary>
        private JoinPointCollection basecollection;
        /// <summary>
        /// ist true, sobald der Typ aus der mscorlib kommt oder es sich um eine verwobene Collection handelt
        /// </summary>
        private bool stopsearchforaspects;

        internal JoinPointCollection BaseCollection
        {
            get { return basecollection; }
        }

        /// <summary>
        /// Alle Aspekte des Typs
        /// </summary>
        private AspectCoverageInfo[] aspects;

        /// <summary>
        /// überprüft, ob der angegebene Typ überhaupt auf statische Aspekte geprft werden muss
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool HasNoAspects(Type type)
        {
            return type.Assembly.FullName.StartsWith("mscorlib,");
        }

        public static void Initialize(IJoinPointClassFactory factory)
        {
            c_aspectinfofactory = factory;
        }

        /// <summary>
        /// Ctor für Zielklassen
        /// </summary>
        /// <param name="basecollection"></param>
        /// <param name="type"></param>
        /// <param name="ctorjoinpoints"></param>
        /// <param name="classjoinpoints"></param>
        /// <param name="interfacejoinpoints"></param>
        /// <param name="aspects"></param>
        private JoinPointCollection(JoinPointCollection basecollection, Type type, ConstructorJoinPoint[] ctorjoinpoints, MethodInfoJoinPoint[] classjoinpoints, InterfaceJoinPoints[] interfacejoinpoints, AspectCoverageInfo[] aspects)
            :
            this(basecollection, type, ctorjoinpoints, classjoinpoints, interfacejoinpoints)
        {
            this.aspects = aspects;
            this.stopsearchforaspects = false;
        }

        /// <summary>
        /// Für einen generierten Proxy wird mit diesem Konstruktor eine neue Collection gebaut
        /// </summary>
        /// <param name="basecollection">Collection, von dem Typ, von dem der Proy abgeleitet wurde</param>
        /// <param name="type">der Typ des Proxys</param>
        /// <param name="ctorjoinpoints">die JoinPoints der Konstruktoren</param>
        /// <param name="vtbljoinpoints">die JoinPoints der virtuelllen Methoden</param>
        /// <param name="interfacejoinpoints">die JoinPoints der Interfaces</param>
        internal protected JoinPointCollection(JoinPointCollection basecollection, Type type, ConstructorJoinPoint[] ctorjoinpoints, MethodInfoJoinPoint[] vtbljoinpoints, InterfaceJoinPoints[] interfacejoinpoints)
        {
            System.Diagnostics.Debug.Assert(basecollection == null || basecollection.type == type.BaseType);
            this.type = type;
            this.vtablejoinpoints = vtbljoinpoints;
            this.ctorjoinpoints = ctorjoinpoints;
            this.interfacejoinpoints = interfacejoinpoints;
            this.methodjoinpoints = null; // wird nur für Recalls gebraucht und wird daher erst später gebaut
            this.basecollection = basecollection;
            this.stopsearchforaspects = true;
        }

        /// <summary>
        /// returns the metadata of an aspect
        /// </summary>
        /// <param name="aspecttype"></param>
        /// <returns></returns>
        internal static AspectClass GetAspectClass(Type aspecttype)
        {
            if (aspecttype == typeof(object) || aspecttype==typeof(AspectAttribute))
            {
                return null;
            }

            AspectClass ac;
            if (!c_aspectclasscache.TryGetValue(aspecttype, out ac))
            {
                ac = c_aspectinfofactory.CreateAspectClass(aspecttype,GetAspectClass(aspecttype.BaseType));
                c_aspectclasscache.Add(aspecttype, ac);
            }
            return ac;
        }


        /// <summary>
        /// Sucht die Aspekte auf dem Assemblypfad für einen Typen
        /// </summary>
        /// <param name="jpcolaspects">DIe Collection auf der die suche ausgeführt wird</param>
        /// <param name="jpenumerator">Die Collection, auf der die Joinpoints enumeriert werden</param>
        /// <param name="bDeclared">true: Das ist die Collection der zu verwebenden Zielklasse, false sonst</param>
        private static void EnumAssemblyAspects(List<IJoinPointEnumerator> jpenumerator, JoinPointCollection jpcolaspects, bool bDeclared)
        {
            // Die Collection bis auf die Basisklasse durchgehen
            if (jpcolaspects.stopsearchforaspects) return;
            if (jpcolaspects.basecollection != null)
            {
                EnumAssemblyAspects(jpenumerator, jpcolaspects.basecollection, false);
            }

            Type type = jpcolaspects.type;
            AssemblyAspectInfo aspectcollection = GetAssemblyAspectInfo(type);

            // Wurden die Aspekte dieser Assembly in diesem Lauf schon berücksichtigt?
            if (aspectcollection.magiccookie != AssemblyAspectInfo.c_magiccookie)
            {
                foreach (var aspect in aspectcollection.aspects)
                {
                    if (bDeclared || aspect.AspectClass.Inherited)
                    {
                        jpenumerator.Add(new JoinPointCollectionEnumerator(aspect));
                    }
                }
            }
            aspectcollection.magiccookie = AssemblyAspectInfo.c_magiccookie;
        }

        /// <summary>
        /// Registriert die Aspekte, die auf einer Assembly definiert wurden.
        /// </summary>
        /// <param name="type"></param>
        /// 
        /// <returns></returns>
        private static AssemblyAspectInfo GetAssemblyAspectInfo(Type type)
        {
            Assembly assembly = type.Assembly;
            AssemblyAspectInfo aspectcollection;

            // Gibt es schon einen Eintrag für die Assembly
            if (!c_assemblyInfo.TryGetValue(assembly, out aspectcollection))
            {
                aspectcollection = new AssemblyAspectInfo();
                var aspects = GetAssemblyCustomAttributeAspects(type);

                AssemblyConfig aconf;
                var an = assembly.GetName();
                if (!c_assemblyConfig.TryGetValue(an, out aconf))
                {
                    foreach (var kv in c_assemblyConfig)
                    {
                        if (kv.Key.Name == an.Name)
                        {
                            aconf = kv.Value;
                            break;
                        }
                    }
                }
                if (aconf!=null)
                {
                    aspectcollection.aspects = CombineConfiguredAspects(aspects, aconf.aspects, assembly).ToArray();
                    aspectcollection.typeconfiguration = aconf.types.ToDictionary(tc => GetTypeSafe(assembly, tc));
                }
                else
                {
                    aspectcollection.aspects = aspects.ToArray();
                }

                c_assemblyInfo.Add(assembly, aspectcollection);
            }
            return aspectcollection;
        }

        private static Type GetTypeSafe(Assembly assembly, TargetTypeConfig tc)
        {
            var res=assembly.GetType(tc.name);
            if (res == null)
            {
                throw new AspectWeaverException(25, Resources.Errors.ERR_0025, tc.name, assembly.FullName);
            }
            return res;
        }


        /// <summary>
        /// Sucht rekursiv die Aspekte auf dem Klassenpfad für einen Typen
        /// </summary>
        /// <param name="jpcolaspects">DIe Collection auf der die suche ausgeführt wird</param>
        /// <param name="jpenumerator"></param>
        /// <remarks>
        /// Im Gegensatz zur anderen überladung werden nur Aspekte, die auch veerbt wurden mit in die Liste aufgenommen
        /// </remarks>
        private static void EnumClassAspects(List<IJoinPointEnumerator> jpenumerator, JoinPointCollection jpcolaspects)
        {
            if (jpcolaspects.stopsearchforaspects) return;
            if (jpcolaspects.basecollection != null)
            {
                EnumClassAspects(jpenumerator, jpcolaspects.basecollection);
            }

            foreach (var aspect in jpcolaspects.aspects)
            {
                if (aspect.AspectClass.Inherited)
                {
                    jpenumerator.Add(new JoinPointCollectionEnumerator(aspect));
                }
            }
        }

        /// <summary>
        /// Sucht die Aspekte auf dem Klassenpfad für einen Typen, dabei wird berücksichtigt, dass Aspekte evtl nicht vererbt werden
        /// </summary>
        /// <param name="jpenumerator"></param>
        private void EnumClassAspects(List<IJoinPointEnumerator> jpenumerator)
        {
            if (stopsearchforaspects) return;
            if (basecollection != null)
            {
                EnumClassAspects(jpenumerator, basecollection);
            }

            foreach (var aspect in aspects)
            {
                jpenumerator.Add(new JoinPointCollectionEnumerator(aspect));
            }
        }

        /// <summary>
        /// Sucht die Aspekte auf dem Interfacepfad für einen Typen
        /// </summary>
        /// <param name="jpcolaspects">Die Collection auf der die suche ausgeführt wird</param>
        /// <param name="jpenumerator"></param>
        private static void EnumInterfaceAspects(List<IJoinPointEnumerator> jpenumerator, JoinPointCollection jpcolaspects)
        {
            for (int ifcindex = 0; ifcindex < jpcolaspects.interfacejoinpoints.Length; ifcindex++)
            {
                InterfaceJoinPoints ifcjp = jpcolaspects.interfacejoinpoints[ifcindex];
                foreach (var aspect in ifcjp.Interface.aspects)
                {
                    JoinPointInterfaceCollectionEnumerator jpcol = new JoinPointInterfaceCollectionEnumerator(aspect, ifcjp, ifcindex);
                    jpenumerator.Add(jpcol);
                }
            }
        }

        /// <summary>
        /// Sucht die Aspekte auf den Methoden
        /// </summary>
        /// <param name="jpenumerator"></param>
        /// <remarks>
        /// die Methode sucht den Joinpoint, der annotiert wurde, später wird der richtige JoinPoint über den JoinPointAspectInfo gefunden
        /// </remarks>
        private void EnumMethodAspects(List<IJoinPointEnumerator> jpenumerator)
        {
            // Aspekte nach Typen sortieren
            Dictionary<AspectClass, JoinPointAspectInfoList> aspectdict = null;

            ExtractMethodAspects(ref aspectdict, ctorjoinpoints, -1, 1);
            ExtractMethodAspects(ref aspectdict, vtablejoinpoints, -1, 1);

            for (int ifcindex = 0; ifcindex < interfacejoinpoints.Length; ifcindex++)
            {
                InterfaceJoinPoints ifjp = interfacejoinpoints[ifcindex];
                ExtractMethodAspects(ref aspectdict, ifjp.JoinPoints, ifcindex, 2);
            }

            if (aspectdict != null)
            {
                foreach (KeyValuePair<AspectClass, JoinPointAspectInfoList> kvpair in aspectdict)
                {
                    JoinPointMethodEnumerator jpenum = new JoinPointMethodEnumerator(kvpair.Key, kvpair.Value);
                    jpenumerator.Add(jpenum);
                }
            }
        }


        /// <summary>
        /// fügt alle JoinPoints, auf denen ein Aspekt definiert ist, einem Dictionary hinzu
        /// </summary>
        /// <param name="aspectdict">das Dictionary</param>
        /// <param name="jpcol">die Collection</param>
        /// <param name="ifcindex">Der Index des Interfaces, sonst -1</param>
        /// <param name="delimiter">Abgrenzug zwischen den JoinPointTypen 0=CTOR, 1=VTBL, 2=IFC</param>
        private static void ExtractMethodAspects(ref Dictionary<AspectClass, JoinPointAspectInfoList> aspectdict, MethodBaseJoinPoint[] jpcol, int ifcindex, int delimiter)
        {
            for (int jpindex = 0; jpindex < jpcol.Length; jpindex++)
            {
                MethodBaseJoinPoint jp = jpcol[jpindex];
                foreach (AspectCoverageInfo aspect in jp.Aspects)
                {
                    AddJoinPointAspectInfo(ref aspectdict, aspect, jp, ifcindex, jpindex, delimiter);
                }
            }
        }

        /// <summary>
        /// fügt ein AspektInfo zu einem Dictionary hinzu
        /// </summary>
        /// <param name="aspectdict"></param>
        /// <param name="jp"></param>
        /// <param name="ifcindex">Der Index des Interfaces, sonst -1</param>
        /// <param name="aspect"></param>
        /// <param name="jpindex"></param>
        /// <param name="delimiter">0 für Konstruktoren, 1 für VTBLJoinPoints, 2 für IFCJoinPoints</param>
        private static void AddJoinPointAspectInfo(ref Dictionary<AspectClass, JoinPointAspectInfoList> aspectdict, AspectCoverageInfo aspect, MethodBaseJoinPoint jp, int ifcindex, int jpindex, int delimiter)
        {
            AspectClass aspectclass = aspect.AspectClass;
            JoinPointAspectInfoList jplist = null;
            if (aspectdict == null)
            {
                aspectdict = new Dictionary<AspectClass, JoinPointAspectInfoList>();
                jplist = InitializeJoinPointAspectInfoList(aspectdict, aspectclass, jplist);
            }
            else if (!aspectdict.TryGetValue(aspectclass, out jplist))
            {
                jplist = InitializeJoinPointAspectInfoList(aspectdict, aspectclass, jplist);
            }

            jplist.list.Add(new JoinPointAspectInfo(aspect, ifcindex, jpindex));
            switch (delimiter)
            {
                case 0: jplist.firstIndexOfVtblJoinPointAspectInfo = jplist.list.Count; break;
                case 1: jplist.firstIndexOfIfcJoinPointAspectInfo = jplist.list.Count; break;
            }
        }

        /// <summary>
        /// Initialisiert die JoinPointAspectInfoListe
        /// </summary>
        /// <param name="aspectdict"></param>
        /// <param name="aspectclass"></param>
        /// <param name="jplist"></param>
        /// <returns></returns>
        private static JoinPointAspectInfoList InitializeJoinPointAspectInfoList(Dictionary<AspectClass, JoinPointAspectInfoList> aspectdict, AspectClass aspectclass, JoinPointAspectInfoList jplist)
        {
            jplist = new JoinPointAspectInfoList(new List<JoinPointAspectInfo>());
            aspectdict.Add(aspectclass, jplist);
            return jplist;
        }

        /// <summary>
        /// zählt alle Aspekte die mit dem Typen dieser Kollektion statisch verwoben werden sollen auf.
        /// </summary>
        public ICollection<IJoinPointEnumerator> GetAspects()
        {
            AssemblyAspectInfo.c_magiccookie++;

            // Auf dieser JP-Collection werden die Joinpoints ermittelt, während der Enumeration kann sich diese ändern
            List<IJoinPointEnumerator> jpenumerator = new List<IJoinPointEnumerator>();
            EnumAssemblyAspects(jpenumerator, this, true);
            EnumClassAspects(jpenumerator);
            EnumInterfaceAspects(jpenumerator, this);
            EnumMethodAspects(jpenumerator);

            return jpenumerator;
        }

        /// <summary>
        /// fügt einem Typen explizit eine List von Aspecten hinzu
        /// </summary>
        /// <param name="dynamicaspect"></param>
        public IJoinPointEnumerator GetDynamicAspect(Aspect dynamicaspect)
        {
            Type aspecttype = dynamicaspect.GetType();
            AspectClass ac = GetAspectClass(aspecttype);
            IJoinPointEnumerator jpcolenum = new DynamicJoinPointCollectionEnumerator(ac);
            return jpcolenum;
        }

        public Type DeclaringType
        {
            get { return type; }
        }

       

        /// <summary>
        /// Liefert die Joinpoints für alle Virtuellen Methoden dieses Typs
        /// </summary>
        public MethodInfoJoinPoint[] VTableJoinPoints
        {
            get { return vtablejoinpoints; }
        }

        /// <summary>
        /// Liefert die Joinpoints für alle Interfacemethoden dieses Typs
        /// </summary>
        public InterfaceJoinPoints[] InterfaceJoinPoints
        {
            get { return interfacejoinpoints; }
        }

        /// <summary>
        /// Liefert die Joinpoints für alle Konstruktoren dieses Typs
        /// </summary>
        public ConstructorJoinPoint[] CtorJoinPoints
        {
            get { return ctorjoinpoints; }
        }

        /// <summary>
        /// Liefert die Joinpoints für alle (public) Methoden dieses Typs
        /// </summary>
        public IEnumerable<MethodInfoJoinPoint> MethodJoinPoints
        {
            get
            {
                if (methodjoinpoints != null) return methodjoinpoints;


                Func<MemberInfo, TargetTypeConfig, AspectCoverageInfo[]> aspects;
                if(EnableJoinPointsOnNonVirtualMethods)
                {
                    aspects=(mi,cnf) => AddConfiguredMemberInfoAspects(GetCustomAttributeAspects(mi),mi,cnf).ToArray();
                }
                else
                {
                    aspects=(mi,cnf) => new AspectCoverageInfo[0];
                }

                Dictionary<MethodBase, MethodInfoJoinPoint> processedmethods = new Dictionary<MethodBase, MethodInfoJoinPoint>();
                foreach (MethodInfoJoinPoint jp in vtablejoinpoints)
                {
                    if (jp is PropertyJoinPoint) continue;
                    processedmethods.Add(((IManageJoinPoint)jp).KeyMethod, jp);
                }

                var config = GetTargetTypeConfig(type);

                for (Type curtype = type; curtype != null; curtype = curtype.BaseType)
                {
                    foreach (MethodInfo mi in curtype.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                    {
                        if (mi.IsSpecialName) continue;
                        ProxyMethodJoinPoint jp = new ProxyMethodJoinPoint(type, mi, aspects.Invoke(mi,config));
                        if (!processedmethods.ContainsKey(jp.KeyMethod))
                        {
                            processedmethods.Add(jp.KeyMethod, jp);
                        }
                    }
                }

                methodjoinpoints = new MethodInfoJoinPoint[processedmethods.Values.Count];
                processedmethods.Values.CopyTo(methodjoinpoints, 0);

                return methodjoinpoints;
            }
        }

        public Type[] GetInterfaces()
        {
            return type.GetInterfaces();
        }

        /// <summary>
        /// Liefert die JoinPointCollection für den angegebenen Typ
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static JoinPointCollection GetJoinPoints(Type type)
        {
            JoinPointCollection collection;
            if (c_jpcollectioncache.TryGetValue(type, out collection))
            {
                return collection;
            }

            // Nicht da also neu bauen
            if (type.BaseType != null)
            {
                collection = GetJoinPoints(type.BaseType);
            }
            else collection = null;

            GetCustomAttributeAspectsDelegate attraspects;
            bool checkforattributes;
            if(HasNoAspects(type))
            {
                attraspects = prov => Enumerable.Empty<AspectCoverageInfo>();
                checkforattributes=false;
            }
            else
            {
                attraspects = prov => GetCustomAttributeAspects(prov);
                checkforattributes=true;
            }

            TargetTypeConfig config = GetTargetTypeConfig(type);
            InterfaceJoinPoints[] interfacejoinpoints = GetInterfaces(type, config, attraspects);
            ConstructorJoinPoint[] ctorjoinpoints = GetConstructors(type, config, attraspects);

            JoinPointDictionary classjoinpoints = new JoinPointDictionary();
            AddProperties(type, config, classjoinpoints, attraspects);
            AddEvents(type,config,classjoinpoints, attraspects);
            AddVirtualMethods(type, config, classjoinpoints, attraspects);
            if (collection != null)
            {
                JoinCollection<MethodInfoJoinPoint>(type, collection.vtablejoinpoints, classjoinpoints);
            }

            MethodInfoJoinPoint[] classjpts = new MethodInfoJoinPoint[classjoinpoints.Values.Count];
            classjoinpoints.Values.CopyTo(classjpts, 0);

            var aspects=GetTypeAspects(type, config, checkforattributes);
            collection = new JoinPointCollection(collection, type, ctorjoinpoints, classjpts, interfacejoinpoints, aspects);

            RegisterJoinPoints(collection);

            return collection;
        }

        internal static void RegisterJoinPoints(JoinPointCollection collection)
        {
            c_jpcollectioncache.Add(collection.type, collection);
        }


        /// <summary>
        /// Liefert die Joinpoints für ein Interface in einer InterfaceJoinPointCollection
        /// </summary>
        /// <param name="ifc">Das Interface</param>
        /// <returns></returns>
        internal static InterfaceJoinPointCollection GetInterfaceJoinPoints(Type ifc)
        {
            // Bereits im Cache?
            InterfaceJoinPointCollection jpts;
            if (c_jpifccache.TryGetValue(ifc, out jpts))
            {
                return jpts;
            }

            jpts = new InterfaceJoinPointCollection();

            // Erst einmal die Properties und Events in einem Dictionary merken
            // für diese werden spezielle JoinPoint-Typen erzeugt
            // Es ist allerdings wichtig, dass sie im Joinpointarray an der richtigen 
            // Stelle erscheinen, da später das Mapping auf die Implementation
            // anhand des Layouts erfolgt (InterfaceMap)
            MethodInfo[] miarr = ifc.GetMethods();
            jpts.JoinPoints = new MethodInfoJoinPoint[miarr.Length];
            Dictionary<MethodInfo, MethodInfoJoinPoint> jptsdict = new Dictionary<MethodInfo, MethodInfoJoinPoint>();

            TargetTypeConfig config = GetTargetTypeConfig(ifc);

            GetCustomAttributeAspectsDelegate attraspects;
            bool checkforattributes;
            if(HasNoAspects(ifc))
            {
                attraspects = prov => Enumerable.Empty<AspectCoverageInfo>();
                checkforattributes=false;
            }
            else
            {
                attraspects = prov => GetCustomAttributeAspects(prov);
                checkforattributes=true;
            }

            // Aspekte suchen
            foreach (PropertyInfo pi in ifc.GetProperties())
            {
                // Aspecte auf dem Property ermitteln
                var aspects = AddConfiguredMemberInfoAspects(attraspects.Invoke(pi), pi, config);

                foreach (MethodInfo mi in pi.GetAccessors())
                {
                    aspects=AddConfiguredMemberInfoAspects(aspects.Union(attraspects.Invoke(mi)), mi, config);

                    jptsdict.Add(mi, new IntroducedPropertyJoinPoint(pi, mi, aspects.ToArray()));
                }
            }
            foreach (EventInfo ei in ifc.GetEvents())
            {
                // Aspekte auf Events
                var aspects = AddConfiguredMemberInfoAspects(attraspects.Invoke(ei), ei,config);

                foreach (MethodInfo mi in GetEventMethods(ei))
                {
                    aspects = AddConfiguredMemberInfoAspects(aspects.Union(attraspects.Invoke(mi)), mi, config);
                    jptsdict.Add(mi, new IntroducedEventJoinPoint(ei, mi, aspects.ToArray()));
                }
            }

            // Joinpoints gem. des Layouts ablegen
            for (int iPos = 0; iPos < miarr.Length; iPos++)
            {
                MethodInfo mi = miarr[iPos];
                jpts.JoinPoints[iPos] = mi.IsSpecialName ? jptsdict[mi] : new IntroducedMethodJoinPoint(mi, AddConfiguredMemberInfoAspects(attraspects.Invoke(mi),mi,config).ToArray());
            }
            // Aspekte für das Interface setzen
            jpts.aspects = GetTypeAspects(ifc, config, checkforattributes);

            // Basisinterfaceseinbauen & Cachen
            Type[] baseifcs = ifc.GetInterfaces();
            InterfaceJoinPointCollection[] basecollections = new InterfaceJoinPointCollection[baseifcs.Length];
            for (int iPos = 0; iPos < baseifcs.Length; iPos++)
            {
                basecollections[iPos] = GetInterfaceJoinPoints(baseifcs[iPos]);
            }
            jpts.BaseInterfaces = basecollections;
            jpts.Interface = ifc;

            c_jpifccache.Add(ifc, jpts);

            return jpts;
        }

        /// <summary>
        /// Liefert die ASpekte auf einem Memberinfo-Objekt inkl. konfigurierter Aspekte
        /// </summary>
        /// <param name="aspects"></param>
        /// <param name="mi"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static IEnumerable<AspectCoverageInfo> AddConfiguredMemberInfoAspects(IEnumerable<AspectCoverageInfo> aspects, MemberInfo mi, TargetTypeConfig config)
        {
            if (config != null && config.member!=null)
            {
                foreach(MemberConfig mc in config.member)
                {
                    if (mc.name == mi.Name)
                    {
                        switch (mi.MemberType)
                        {
                            case MemberTypes.Method:
                                if (mc is MethodConfig)
                                {
                                    aspects = CombineConfiguredAspects(aspects, mc.aspects, mi.DeclaringType.Assembly);
                                }
                                break;
                            case MemberTypes.Property:
                            case MemberTypes.Event:
                                if (mc is AccessorConfig)
                                {
                                    aspects = CombineConfiguredAspects(aspects, mc.aspects, mi.DeclaringType.Assembly);
                                }
                                break;
                        }
                        
                    }
                }
            }
            return aspects;
        }

              

        /// <summary>
        /// Liefert die Aspekte auf einem Typen inkl. der Konfigurierten Aspekte
        /// </summary>
        /// <param name="type"></param>
        /// <param name="config"></param>
        /// <param name="checkforattributes"></param>
        /// <returns></returns>
        private static AspectCoverageInfo[] GetTypeAspects(Type type, TargetTypeConfig config, bool checkforattributes)
        {
            IEnumerable<AspectCoverageInfo> aspects = 
                checkforattributes ? GetCustomAttributeAspects(type): Enumerable.Empty<AspectCoverageInfo>();

            if (config != null)
            {
                aspects=CombineConfiguredAspects(aspects, config.aspects, type.Assembly);
            }
            return aspects.ToArray();
        }

        /// <summary>
        /// Kombiniert eine Aspektliste mit konfigurierten Aspekten
        /// </summary>
        /// <param name="aspects"></param>
        /// <param name="aspectcnfs"></param>
        /// <param name="assemblyscope">the current assembly in scope</param>
        /// <returns></returns>
        private static IEnumerable<AspectCoverageInfo> CombineConfiguredAspects(IEnumerable<AspectCoverageInfo> aspects, AspectConfig[] aspectcnfs, Assembly assemblyscope)
        {
            if (aspectcnfs != null && aspectcnfs.Length > 0)
            {
                var cnfaspects = aspectcnfs.Select(config =>
                {
                    try
                    {
                        var type = Common.Resolver.ResolveType(config.type, assemblyscope);
                        return c_aspectinfofactory.CreateAspectCoverageInfoFromConfig(GetAspectClass(type), config);
                    }
                    catch (Exception e)
                    {
                        throw new AspectWeaverException(26, Resources.Errors.ERR_0024, e, config.type, e.Message);
                    }

                });
                return aspects != null ? aspects.Union(cnfaspects) : cnfaspects;
            }
            return aspects;
        }
        /// <summary>
        /// fügt zum bestehenden Aspektarray die Aspekte des Providers hinzu
        /// </summary>
        /// <param name="provider">Reflection-Objekt, das weitere Aspekte enthält</param>
        /// <returns></returns>
        private static IEnumerable<AspectCoverageInfo> GetCustomAttributeAspects(MemberInfo provider)
        {
            var provideraspects = provider.GetCustomAttributesData();
            return provideraspects.Where(cad=>cad.Constructor.DeclaringType.IsSubclassOf(typeof(AspectAttribute))).Select(cad=>c_aspectinfofactory.CreateAspectCoverageInfoFromCustomAttribute(GetAspectClass(cad.Constructor.DeclaringType), cad));
        }

        /// <summary>
        /// fügt zum bestehenden Aspektarray die Aspekte des Providers hinzu
        /// </summary>
        /// <param name="provider">Reflection-Objekt, das weitere Aspekte enthält</param>
        /// <returns></returns>
        private static IEnumerable<AspectCoverageInfo> GetCustomAttributeAspects(Type provider)
        {
            var provideraspects = provider.GetCustomAttributesData();
            return provideraspects.Where(cad => cad.Constructor.DeclaringType.IsSubclassOf(typeof(AspectAttribute))).Select(cad => c_aspectinfofactory.CreateAspectCoverageInfoFromCustomAttribute(GetAspectClass(cad.Constructor.DeclaringType), cad));
        }

        /// <summary>
        /// fügt zum bestehenden Aspektarray die Aspekte des Providers hinzu
        /// </summary>
        /// <param name="provider">Reflection-Objekt, das weitere Aspekte enthält</param>
        /// <returns></returns>
        private static IEnumerable<AspectCoverageInfo> GetAssemblyCustomAttributeAspects(Type provider)
        {
            var provideraspects = provider.Assembly.GetCustomAttributesData();
            return provideraspects.Where(cad => cad.Constructor.DeclaringType.IsSubclassOf(typeof(AspectAttribute))).Select(cad => c_aspectinfofactory.CreateAspectCoverageInfoFromCustomAttribute(GetAspectClass(cad.Constructor.DeclaringType), cad));
        }
        /// <summary>
        /// Gibt alle zu einem Event gehörenden Methoden zurück
        /// </summary>
        /// <param name="ei"></param>
        /// <returns></returns>
        private static IEnumerable<MethodInfo> GetEventMethods(EventInfo ei)
        {
            MethodInfo mi;
            mi = ei.GetAddMethod();
            if (mi != null) yield return mi;
            mi = ei.GetRaiseMethod();
            if (mi != null) yield return mi;
            mi = ei.GetRemoveMethod();
            if (mi != null) yield return mi;
            foreach (MethodInfo miother in ei.GetOtherMethods())
            {
                yield return mi;
            }
        }

        /// <summary>
        /// Liefert die Konfiguration eines Typs einer Assembly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static TargetTypeConfig GetTargetTypeConfig(Type type)
        {
            AssemblyAspectInfo asseminfo = GetAssemblyAspectInfo(type);
            TargetTypeConfig config = null;
            if (asseminfo.typeconfiguration != null)
            {
                asseminfo.typeconfiguration.TryGetValue(type, out config);
            }
            return config;
        }

        /// <summary>
        /// Füllt die fehlenden Methoden aus dem Basisdictionary auf
        /// </summary>
        /// <param name="type"></param>
        /// <param name="basecollection"></param>
        /// <param name="joinpoints"></param>
        public static void JoinCollection<T>(Type type, ICollection<T> basecollection, Dictionary<MethodBase, T> joinpoints) where T : MethodBaseJoinPoint
        {
            // Vereinigung der JoinPoints
            foreach (MethodBaseJoinPoint jpbase in basecollection)
            {
                T jpcurrent;
                if (!joinpoints.TryGetValue(((IManageJoinPoint)jpbase).KeyMethod, out jpcurrent))
                {
                    // JoinPoint muss von der Basisklasse übernommen werden
                    jpcurrent = (T)jpbase.Clone(type);
                    joinpoints.Add(((IManageJoinPoint)jpcurrent).KeyMethod, jpcurrent);
                }
            }
        }

        private static bool IsIfcPublic(Type ifc)
        {
            if (ifc.IsPublic) return true;
            if (ifc.IsNested && ifc.IsNestedPublic) return IsIfcPublic(ifc.DeclaringType);
            return false;
        }

        /// <summary>
        /// Erzeugt für den Typ eine InterfaceJoinPoint-Liste für jedes von ihm implementierte Interface
        /// </summary>
        /// <param name="type">Ein Klassentyp</param>
        /// <param name="checkcustomattributes"></param>
        /// <param name="config"></param>
        /// <remarks>
        /// Interfaces erlauben mehrfachvererbung, allerdings liefert eine InterfaceMap auch nur die neu hinzugekommenen Methoden und
        /// GetInterfaces grundsätzlich auch alle Basisinterfaces. In dieser Methode werden aus den reinen InterfaceJoinPoints die
        /// Joinpoints für die Implementation abgeleitet und gebunden
        /// </remarks>
        /// <returns></returns>
        private static InterfaceJoinPoints[] GetInterfaces(Type type, TargetTypeConfig config, GetCustomAttributeAspectsDelegate checkcustomattributes)
        {
            int ifPos;
            Type[] ifctypes = type.GetInterfaces();

            // öffentliche Interfaces herausfiltern
            int ifCount = 0;
            for (ifPos = 0; ifPos < ifctypes.Length; ifPos++)
            {
                Type ifctype = ifctypes[ifPos];
                if (IsIfcPublic(ifctype))
                {
                    if (ifCount != ifPos)
                    {
                        ifctypes[ifCount] = ifctype;
                    }
                    ifCount++;
                }
            }

            InterfaceJoinPoints[] ifcjpts = new InterfaceJoinPoints[ifCount];
            for (ifPos = 0; ifPos < ifCount; ifPos++)
            {
                InterfaceJoinPointCollection ifcol = GetInterfaceJoinPoints(ifctypes[ifPos]);

                // Herausfinden, welche Methoden zu Properties & Events gehören

                InterfaceMapping ifcmap = type.GetInterfaceMap(ifcol.Interface);
                System.Diagnostics.Debug.Assert(ifcmap.InterfaceMethods.Length == ifcol.JoinPoints.Length);
                MethodInfoJoinPoint[] ifcarr = new MethodInfoJoinPoint[ifcmap.InterfaceMethods.Length];

                bool bVirtual = false;
                int ifcolpos = 0;
                for (int current = 0; current < ifcmap.TargetMethods.Length; current++)
                {
                    MethodInfo mitarget = ifcmap.TargetMethods[current];
                    // Ist in dem Interface eine Methode explizit (dies wird daran erkannt, dass sie nicht final ist, weil 
                    // virtuell sind sie alle), muss das Interface als solches definiert werden
                    if (!mitarget.IsFinal) bVirtual = true;

                    // Aspekte ermitteln & Joinpoint binden:
                    // Die Reihenfolge der JoinPoints in der InterfaceJoinPointsCollection und der Methoden in
                    // der InterfaceMap sind nicht identisch. Daher muss der passende Joinpoint erst gesucht werden.
                    // Die Wahrscheinlichkeit, dass die Reihenfolge doch korrekt ist ist jedoch sehr hoch, da die
                    // Interfaces üblicherweise in der Reiohenfolge implementiert werden, in der sie auch im Interface
                    // definiert wurden. (Die Optimierung kann nur bei einem Programmierfehler in einer Endlosschleife enden)
                    while (ifcol.JoinPoints[ifcolpos].MethodInterface != ifcmap.InterfaceMethods[current])
                    {
                        ifcolpos = (ifcolpos + 1) % ifcol.JoinPoints.Length;
                    }
                    MethodInfoJoinPoint jpcurrent = ifcol.JoinPoints[ifcolpos];

                    var aspects = AddConfiguredMemberInfoAspects(jpcurrent.Aspects.Union(checkcustomattributes(mitarget)), mitarget, config);

                    // Methode Binden und speichern
                    ifcarr[current] = ((IBindInterfaceMethod)jpcurrent).BindToImplementation(mitarget, aspects.ToArray());
                }

                ifcjpts[ifPos].bContainsVirtualMethods = bVirtual;
                ifcjpts[ifPos].Interface = ifcol;
                ifcjpts[ifPos].JoinPoints = ifcarr;
            }
            return ifcjpts;
        }


        private static void AddProperties(Type type, TargetTypeConfig config, JoinPointDictionary dictionary, GetCustomAttributeAspectsDelegate checkcustomattributes)
        {
            PropertyInfo[] piarr;
            try
            {
                piarr = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            }
            catch (Exception)
            {
                piarr = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            }
            if (piarr != null)
            {
                foreach (PropertyInfo pi in piarr)
                {
                    var aspects = AddConfiguredMemberInfoAspects(checkcustomattributes(pi), pi, config);
                    foreach (MethodInfo mi in pi.GetAccessors(true))
                    {
                        if (!mi.IsVirtual || mi.IsFinal) continue;
                        if (!mi.IsPublic && !mi.IsFamily) continue;
                        aspects = AddConfiguredMemberInfoAspects(aspects.Union(checkcustomattributes(mi)), mi, config);
                        ProxyPropertyJoinPoint jp = new ProxyPropertyJoinPoint(type, pi, mi, aspects.ToArray());
                        dictionary.Add(jp.KeyMethod, jp);
                    }
                }
            }
        }

        private static void AddEvents(Type type, TargetTypeConfig config, JoinPointDictionary dictionary, GetCustomAttributeAspectsDelegate checkcustomattributes)
        {
            EventInfo[] piarr;
            try
            {
                piarr = type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            }
            catch (Exception)
            {
                piarr = type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            }
            if (piarr != null)
            {
                foreach (EventInfo ei in piarr)
                {
                    var aspects = AddConfiguredMemberInfoAspects(checkcustomattributes(ei), ei, config);
                    foreach (MethodInfo mi in GetEventMethods(ei))
                    {
                        if (!mi.IsVirtual || mi.IsFinal) continue;
                        if (!mi.IsPublic && !mi.IsFamily) continue;
                        aspects = AddConfiguredMemberInfoAspects(aspects.Union(checkcustomattributes(mi)), mi, config);
                        var jp = new ProxyEventJoinPoint(type, ei, mi, aspects.ToArray());
                        dictionary.Add(jp.KeyMethod, jp);
                    }
                }
            }
        }


        /// <summary>
        /// fügt die virtuellen Methoden eines Typs in ein Dictionary ein
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dictionary"></param>
        /// <param name="checkcustomattributes"></param>
        /// <param name="config"></param>
        private static void AddVirtualMethods(Type type, TargetTypeConfig config, JoinPointDictionary dictionary, GetCustomAttributeAspectsDelegate checkcustomattributes)
        {
            MethodInfo[] methodarr;
            try
            {
                methodarr = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch (Exception) //ReflectPermission (SecurityException)
            {
                methodarr = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            }
            foreach (MethodInfo mi in methodarr)
            {
                if (mi.IsSpecialName || !mi.IsVirtual || mi.IsFinal) continue;
                if (!mi.IsPublic && !mi.IsFamily) continue;

                var aspects = AddConfiguredMemberInfoAspects(checkcustomattributes(mi), mi, config).ToArray();

                if (mi.Name == "Finalize")
                {
                    ProxyDestroyMethodJoinPoint jp = new ProxyDestroyMethodJoinPoint(type, mi, aspects);
                    dictionary.Add(jp.KeyMethod, jp);
                }
                else
                {
                    ProxyVirtualMethodJoinPoint jp = new ProxyVirtualMethodJoinPoint(type, mi, aspects);
                    dictionary.Add(jp.KeyMethod, jp);
                }

            }
        }

        private static ConstructorJoinPoint[] GetConstructors(Type type, TargetTypeConfig config, GetCustomAttributeAspectsDelegate checkcustomattributes)
        {
            ConstructorInfo[] ctors = type.GetConstructors();
            ConstructorJoinPoint[] ctorjpt = new ConstructorJoinPoint[ctors.Length];
            for (int iPos = 0; iPos < ctors.Length; iPos++)
            {
                ConstructorInfo ctor = ctors[iPos];
                ProxyConstructorJoinPoint jp = new ProxyConstructorJoinPoint(type, ctor, AddConfiguredMemberInfoAspects(checkcustomattributes(ctor), ctor,config).ToArray());
                ctorjpt[iPos] = jp;
            }
            return ctorjpt;
        }


        #region IEnumerable<MethodBaseJoinPoint> Members

        public IEnumerator<MethodBaseJoinPoint> GetEnumerator()
        {
            foreach (MethodInfoJoinPoint jp in vtablejoinpoints)
            {
                yield return jp;
            }
            foreach (ConstructorJoinPoint jp in ctorjoinpoints)
            {
                yield return jp;
            }
            foreach (InterfaceJoinPoints ifcjp in interfacejoinpoints)
            {
                foreach (MethodInfoJoinPoint jp in ifcjp.JoinPoints)
                {
                    yield return jp;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Sucht in der JoinpointCollection nch einer Ableitung der Methode, die durch den Joinpoint Repräsentiert wird.
        /// </summary>
        /// <param name="basejp"></param>
        /// <returns></returns>
        internal MethodBaseJoinPoint GetDerivedJoinPoint(MethodBaseJoinPoint basejp)
        {
            IManageJoinPoint mjp = basejp as IManageJoinPoint;
            if (mjp == null) return basejp;

            if (basejp is ConstructorJoinPoint)
            {
                foreach (IManageJoinPoint cmjp in ctorjoinpoints)
                {
                    if (cmjp.KeyMethod == mjp.KeyMethod) return (MethodBaseJoinPoint)cmjp;
                }
            }
            foreach (IManageJoinPoint vmjp in vtablejoinpoints)
            {
                if (vmjp.KeyMethod == mjp.KeyMethod) return (MethodBaseJoinPoint)vmjp;
            }
            MethodInfoJoinPoint mijp = basejp as MethodInfoJoinPoint;
            if (mijp != null)
            {
                MethodInfo miifc = mijp.MethodInterface;
                if (miifc != null)
                {
                    foreach (InterfaceJoinPoints ifcjp in interfacejoinpoints)
                    {
                        if (ifcjp.Interface.Interface != miifc.DeclaringType) continue;
                        foreach (IManageJoinPoint imjp in ifcjp.JoinPoints)
                        {
                            if (imjp.KeyMethod == mjp.KeyMethod) return (MethodBaseJoinPoint)imjp;
                        }
                    }
                }
            }
            System.Diagnostics.Debug.Assert(false);
            return null;
        }

       
        /// <summary>
        /// Fügt konfigurierte Aspekte in das Joinpoint-Modell ein
        /// </summary>
        /// <param name="config"></param>
        internal static void ApplyConfiguration(LoomConfig config)
        {
            // Prepare Configuration Items for fast lookup
            if (config.assemblies == null)
            {
                return;
            }

            foreach (var assemcnf in config.assemblies)
            {
                c_assemblyConfig.Add(new AssemblyName(assemcnf.name), assemcnf);
            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append(" Type=\"");
            sb.Append(type.Name);
            sb.Append("\"");
            return sb.ToString();
        }
    }

}
