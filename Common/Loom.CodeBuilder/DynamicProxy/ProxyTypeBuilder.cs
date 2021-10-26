// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Loom.WeaverMessages;
using Loom.Common;
using Loom.AspectModel;
using Loom.Runtime;
using Loom.CodeBuilder;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.CodeBuilder.DynamicProxy.Declarations;
using Loom.JoinPoints.Implementation;
using Loom.AspectProperties;
using Loom.CodeBuilder.DynamicProxy.JoinPoints;

namespace Loom.CodeBuilder.DynamicProxy
{
    internal struct MethodAspectInfo
    {
        public MethodAspectInfo(AspectAttribute aspect, MethodBaseJoinPoint jp)
        {
            this.aspect = aspect;
            this.jp = jp;
        }

        public AspectAttribute aspect;
        public MethodBaseJoinPoint jp;
    }


    class ProxyMemberBuilderList : List<ProxyMemberBuilder> { }

    /// <summary>
    /// Der InterwWovenTypeBuilder ist verantwortlich für die Deklaration des verwobenen Typs.
    /// Er enthält alle Funktionen, die zum hinzufügen neuer Member in diesen Typ notwendig sind.
    /// Für die Definition kommen der <see cref="ProxyAdviceMemberBuilder"/> und die <see cref="CodeBrick"/>
    /// zum Einsatz.
    /// </summary>
    internal abstract class ProxyTypeBuilder : ProxyTypeBuilderBase, IAcceptJoinPoint
    {
        /// <summary>
        /// Cache für alle Aspektklassen
        /// </summary>
        private static Dictionary<Type, AspectClass> c_aspectclasses = new Dictionary<Type, AspectClass>();
        /// <summary>
        /// Ein Keks
        /// </summary>
        private static int c_magiccookie=0;

        // Verwebung
        private ProxyTypeActivatorBuilder classobjectbuilder;
        protected ProxyTypeActivatorBuilder ClassObjectBuilder
        {
            get { return classobjectbuilder; }
        }
        /// <summary>
        /// wenn eine Methode verwoben wurde, wird diese Variable auf false gesetzt. false: es muss ein neuer Proxy erzeugt werden (CreateProxy) 
        /// </summary>
        private bool iscreated;
        /// <summary>
        /// der Typ wurde verwoben
        /// </summary>
        private bool isinterwoven;

        // Base info
        protected Type aspecttype;
        protected Type basetype;
        protected Type targetclass;


        // JoinPoints
        class InterfaceDictionary : Dictionary<Type, ProxyMemberBuilder[]> { }

        private ProxyMemberBuilder[] definedVTableBuilder;
        private ProxyMemberBuilder[] definedCtorBuilder;
        private ProxyMemberBuilder[][] definedInterfaceBuilder;
        private ProxyMemberBuilder[][] introducedInterfaceBuilder;
        private InterfaceJoinPointCollection[] introducedInterfaces;
        private List<ProxyJPInitializerBuilder> definedInitializerBuilder;
        protected JoinPointCollection joinpointcollection;
        // Eigenschaften des aktuell implementierten Typs
        private bool abstracttype;

        /// <summary>
        /// Konstruktor für Typen, für die es noch kein Klassenobjekt gibt
        /// wird verwendet, um statisch verwobenen Proxies zu erzeugen
        /// </summary>
        /// <param name="targetmodule"></param>
        /// <param name="targetclass"></param>
        /// <param name="classobjectbuilder"></param>
        public ProxyTypeBuilder(ModuleBuilder targetmodule, Type targetclass, ProxyTypeActivatorBuilder classobjectbuilder)
            :
            base(targetmodule)
        {
            c_magiccookie++;

            this.classobjectbuilder = classobjectbuilder;
            this.basetype = targetclass;
            this.targetclass = targetclass;
            this.joinpointcollection = JoinPointCollection.GetJoinPoints(targetclass);
            this.iscreated = true;
            this.isinterwoven = false;

            classobjectbuilder.SetProxyTypeBuilder(this);
        }

        /// <summary>
        /// Konstruktor für Typen, bei denen es ein Klassenobjekt für die Basisklasse gibt
        /// wird verwendet um dynamisch verwobene Typen zu erzeugen
        /// </summary>
        public ProxyTypeBuilder(ModuleBuilder targetmodule, ClassObject parentclassobject, ProxyTypeActivatorBuilder classobjectbuilder)
            :
            base(targetmodule)
        {
            c_magiccookie++;

            this.classobjectbuilder = classobjectbuilder;
            this.basetype = parentclassobject.Type;
            this.targetclass = parentclassobject.TargetClass;
            this.joinpointcollection = JoinPointCollection.GetJoinPoints(basetype);
            this.iscreated = true;
            this.isinterwoven = false;

            classobjectbuilder.SetProxyTypeBuilder(this);
        }

        /// <summary>
        /// fügt einen Codebrick für einen Initializer hinzu.
        /// Diese werden dann vom Constructorbuilder verarbeitet
        /// </summary>
        /// <param name="codeBrick"></param>
        internal void AddInitializer(ProxyJPInitializerBuilder codeBrick)
        {
            if (definedInitializerBuilder == null) definedInitializerBuilder = new List<ProxyJPInitializerBuilder>();
            definedInitializerBuilder.Add(codeBrick);
        }

        /// <summary>
        /// Die aktuellen JoinPoints
        /// </summary>
        internal JoinPointCollection CurrentJoinPoints
        {
            get { return joinpointcollection; }
        }

        /// <summary>
        /// der Aspekttyp
        /// </summary>
        public override Type AspectType
        {
            get { return aspecttype; }
        }

        /// <summary>
        /// Basisklasse des neuen Typs
        /// </summary>
        public override Type BaseType
        {
            get { return basetype; }
        }

        /// <summary>
        /// Zielklasse
        /// </summary>
        public override Type TargetClass
        {
            get { return targetclass; }
        }

        /// <summary>
        /// Zeigt an, dass der gerade gebaute Typ abstract ist.
        /// </summary>
        public bool IsAbstract
        {
            get { return abstracttype; }
        }

        /// <summary>
        /// MethodBilder für die Initializer
        /// </summary>
        public IList<ProxyJPInitializerBuilder> DefinedInitializerBuilder
        {
            get { return definedInitializerBuilder; }
        }

        /// <summary>
        /// 1. Phase im Verwebungsprozess:
        /// Verwebt eine Aspektclasse auf dem kompletten Typen
        /// Hier wird noch kein Code erzeugt
        /// </summary>
        /// <param name="aspect"></param>
        public override bool Interweave(Aspect aspect)
        {
            IJoinPointEnumerator jpenum = joinpointcollection.GetDynamicAspect(aspect);
            // Aspectslot definieren
            var proxyjpcol = joinpointcollection as ProxyJoinPointCollection;
            int aspextindex = proxyjpcol == null ? 0 : proxyjpcol.AspectIndex;
            declarations.Aspects = new AspectField[] { new DynamicAspectField(aspextindex) };

            this.aspecttype = aspect.GetType();
            InterweaveJoinPoints(jpenum);

            if (!iscreated)
            {
                CreateProxy(null);
                JoinPointCollection.RegisterJoinPoints(joinpointcollection);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 1. Phase im Verwebungsprozess
        /// Verwebt alle auf dem Typen definierten Aspekte
        /// </summary>
        /// <returns></returns>
        protected bool Interweave(IEnumerator<IJoinPointEnumerator> aspectenum)
        {
            // Der Enumerator enthält für jeden auf TargetClass definierten Aspekt ein IJoinPointEnumerator-Objekt
            // Auf diesem kann ermittelt werden, welche JoinPoints betroffen sind und auch die Aspektinstanzen

            this.abstracttype = true; // solange noch nicht alle Aspekte verwoben sind, werden Abstrakte Typen gebaut

            bool bnext = aspectenum.MoveNext();

            if (!bnext)
            {
                return false;
            }

            int aspectarraypos = 0;
            var aspects = new List<AspectField>();
            bool ismainproxy = false;


            do
            {
                this.iscreated = true;

                IJoinPointEnumerator jpenum = aspectenum.Current;
                var ai=(ProxyAspectCoverageInfo)jpenum.Aspects.First();
                var ac=(ProxyAspectClass)ai.AspectClass;

                if (ac.IsVirtual)
                {
                    ac.MagicCookie=c_magiccookie;
                    foreach (ProxyAspectCoverageInfo pai in jpenum.Aspects)
                    {
                        pai.CheckEmptyConstructor();
                    }
                }

                // Nachfolgende Aspekte untersuchen und bereits bearbeitete virtuelle skippen
                ProxyAspectClass cmpac;
                do
                {
                    bnext = aspectenum.MoveNext();
                    if (!bnext)
                    {
                        break;
                    }

                    var cmpasp = aspectenum.Current.Aspects;
                    cmpac = (ProxyAspectClass)cmpasp.First().AspectClass;
                } while (cmpac.IsVirtual && cmpac.MagicCookie == c_magiccookie);

                if (!bnext)
                {
                    // Das ist der letzte statische Aspekt: jetzt werden die Typen konkret
                    // Ein Proxy muss jetzt in jedem Fall gebaut werden, auch wenn sich herausstellt, dass es keinen Match gibt
                    this.abstracttype = false;
                    ismainproxy = true;
                }

                this.aspecttype=ac.AspectType;

                AspectField[] aspectfields=null;
                if(ac.IsVirtual)
                {
                    aspectfields=new AspectField[] { ai.CreateAspectField() };
                    foreach (ProxyAspectCoverageInfo pai in jpenum.Aspects)
                    {
                        pai.CheckEmptyConstructor();
                    }
                }
                else
                {
                    aspectfields=jpenum.Aspects.Select(pai =>((ProxyAspectCoverageInfo)pai).CreateAspectField()).ToArray();
                }
                aspects.AddRange(aspectfields);
                foreach (var af in aspectfields)
                {
                    af.ArrayPosition = aspectarraypos++;
                }

                declarations.Aspects = aspectfields;

                // Jetzt alle JoinPoints verweben
                InterweaveJoinPoints(jpenum);


                // Und den Proxy erstellen
                if (!this.iscreated || !this.abstracttype)
                {
                    this.isinterwoven = true;
                    CreateProxy(ismainproxy ? aspects: null);
                }
            }
            while (bnext);

            // Wenn erfolgreich verwoben wurde, müssen die neuen JoinPoints abgespeichert werden
            // Diese wurden durch CreateProxy automatisch aktualisiert
            if (this.isinterwoven)
            {

                JoinPointCollection.RegisterJoinPoints(joinpointcollection);
                return true;
            }

            return false;
        }

        private static AspectField GetClassAspectField<T>(ref Dictionary<AspectClass, AspectField> classdict, AspectCoverageInfo ai) where T:AspectField,new()
        {
            AspectField af;
            if (classdict == null)
            {
                classdict = new Dictionary<AspectClass, AspectField>();
                classdict.Add(ai.AspectClass, af = new T());
            }
            else if (!classdict.TryGetValue(ai.AspectClass, out af))
            {
                classdict.Add(ai.AspectClass, af = new T());
            }
            return af;
        }

        /// <summary>
        /// Für jeden Aspekt wird diese Methode einmal aufgerufen. 
        /// Sie initialisiert die Strukturen für die verwebung und baut dann einen Proxy
        /// </summary>
        /// <param name="enumerator"></param>
        public void InterweaveJoinPoints(IJoinPointEnumerator enumerator)
        {
            AspectWeaver.WriteWeaverMessage(MessageType.Info, 501, Resources.Info.INF_0501, targetclass.FullName, aspecttype.FullName);

            // Slots anlegen
            definedInitializerBuilder = null;
            definedVTableBuilder = null;  // Vtable werden erst angelegt, wenn sie wirklich gebraucht werden
            definedCtorBuilder = new ProxyMemberBuilder[joinpointcollection.CtorJoinPoints.Length]; ;
            definedInterfaceBuilder = new ProxyMemberBuilder[joinpointcollection.InterfaceJoinPoints.Length][];
            introducedInterfaceBuilder = null;
            introducedInterfaces = null;

            // JoinPoints durchgehen
            enumerator.EnumJoinPoints(this, joinpointcollection);
        }

        /// <summary>
        /// Passt den Aspektindex in Falle von Per.AspectClass, Per.Class, PerInstance an
        /// </summary>
        /// <param name="aspectindex"></param>
        /// <returns></returns>
        private int AdjustAspectIndex(int aspectindex)
        {
            return (declarations.Aspects.Length == 1) ? 0: aspectindex;
        }

        #region IAcceptJoinPoint Members

        /// <summary>
        /// Hier werden für die verwobenen virtuellen Methoden die MethodBuilder erzeugt
        /// </summary>
        /// <param name="aspectindex"></param>
        /// <param name="jpindex"></param>
        /// <param name="jp"></param>
        /// <param name="matches"></param>
        public void AcceptVTableJoinPoint(int aspectindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches)
        {
            aspectindex = AdjustAspectIndex(aspectindex);

            if (definedVTableBuilder == null)
            {
                definedVTableBuilder = new ProxyMemberBuilder[joinpointcollection.VTableJoinPoints.Length];
            }

            System.Diagnostics.Debug.Assert(definedVTableBuilder[jpindex] == null);
            iscreated = false;

            ProxyAdviceMethodBuilder pamb = new ProxyAdviceMethodBuilder(this, jp, aspectindex);
            pamb.Interweave(matches);
            definedVTableBuilder[jpindex] = pamb;
        }

       
        /// <summary>
        /// er wird ein Methodbuilder für den Konstruktor erzeugt, dieser
        ///  wird dann vom TypeActivatorBuilder noch einmal in einen MB für das Klassenobjekt gekapselt
        /// </summary>
        /// <param name="aspectindex"></param>
        /// <param name="jpindex"></param>
        /// <param name="jp"></param>
        /// <param name="matches"></param>
        public void AcceptCtorJoinPoint(int aspectindex, int jpindex, ConstructorJoinPoint jp, AspectMemberCollection matches)
        {
            aspectindex = AdjustAspectIndex(aspectindex);

            System.Diagnostics.Debug.Assert(definedCtorBuilder[jpindex] == null);
            if (matches != null) iscreated = false;
            ProxyMemberBuilder pmb = classobjectbuilder.DefineConstructor(new ProxyConstructorBuilder(this, jp, false, aspectindex), matches, aspectindex);
            definedCtorBuilder[jpindex] = pmb;
        }

        /// <summary>
        /// Hier wird ein MEthodBuilder für eine Interfacemethode erzeugt
        /// </summary>
        /// <param name="aspectindex"></param>
        /// <param name="ifcindex"></param>
        /// <param name="jpindex"></param>
        /// <param name="jp"></param>
        /// <param name="matches"></param>
        public void AcceptInterfaceJoinPoint(int aspectindex, int ifcindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches)
        {
            aspectindex = AdjustAspectIndex(aspectindex);

            if (definedInterfaceBuilder[ifcindex] == null) // wurde das interface schon angefasst?
            {
                definedInterfaceBuilder[ifcindex] = new ProxyMemberBuilder[joinpointcollection.InterfaceJoinPoints[ifcindex].JoinPoints.Length];
            }
            iscreated = false;
            ProxyAdviceMethodBuilder pamb = new ProxyAdviceMethodBuilder(this, jp, aspectindex);
            System.Diagnostics.Debug.Assert(definedInterfaceBuilder[ifcindex][jpindex] == null);
            System.Diagnostics.Debug.Assert(joinpointcollection.InterfaceJoinPoints[ifcindex].JoinPoints[jpindex] == jp);
            pamb.Interweave(matches);
            definedInterfaceBuilder[ifcindex][jpindex] = pamb;
        }

        /// <summary>
        /// Hier werden die neu hinzukommenden Interfaces angemeldet
        /// </summary>
        /// <param name="ifcol"></param>
        public void AcceptInterface(ICollection<InterfaceJoinPointCollection> ifcol)
        {
            introducedInterfaceBuilder = new ProxyMemberBuilder[ifcol.Count][];
            introducedInterfaces = new InterfaceJoinPointCollection[ifcol.Count];

            int iPos = 0;
            foreach (InterfaceJoinPointCollection ifjpcol in ifcol)
            {
                introducedInterfaces[iPos] = ifjpcol;
                introducedInterfaceBuilder[iPos] = new ProxyMemberBuilder[ifjpcol.JoinPoints.Length];
                iPos++;
            }
            iscreated = false;
        }

        /// <summary>
        /// Hier wird ein Methodbuilder für eine Introduction erzeugt
        /// </summary>
        /// <param name="aspectindex"></param>
        /// <param name="ifcindex"></param>
        /// <param name="jpindex"></param>
        /// <param name="jp"></param>
        /// <param name="matches"></param>
        public void AcceptIntroductionJoinPoint(int aspectindex, int ifcindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches)
        {
            aspectindex = AdjustAspectIndex(aspectindex);
            ProxyIntroducedMethodBuilder pamb = new ProxyIntroducedMethodBuilder(this, jp, aspectindex);
            System.Diagnostics.Debug.Assert(introducedInterfaceBuilder[ifcindex][jpindex] == null);
            pamb.Interweave(matches);
            introducedInterfaceBuilder[ifcindex][jpindex] = pamb;
        }

        #endregion



        /// <summary>
        /// 2. Phase im Verwebungsprozess:
        /// Nachdem festgelegt wurde, welche Methoden wie in die JoinPoints einzuweben sind, 
        /// wird nun der konkrete Code generiert und eine neue JoinpointCollection für den
        /// erzeugten Proxy angelegt
        /// </summary>
        /// <param name="typename"></param>
        /// <returns>a new typebuilder</returns>
        private void CreateProxy(ICollection<AspectField> aspects)
        {
            if (targetclass.IsSealed)
            {
                throw new NotSupportedException();
            }

            string typename = aspects!=null ? WeavingCodeNames.GetClassName(targetclass) : WeavingCodeNames.GetGenericClassName(TargetClass);

            // Proxytyp anlegen und Basisstrukturen definieren

            TypeAttributes typeattr = TypeAttributes.Public;
            if (basetype.IsSerializable)
            {
                typeattr |= TypeAttributes.Serializable;
            }
            if (abstracttype)
            {
                typeattr |= TypeAttributes.Abstract;
            }

            typebuilder = TargetModule.DefineType(typename, typeattr, basetype);
            if (basetype.IsGenericTypeDefinition)
            {
                var gen = basetype.GetGenericArguments();
                string[] genp = gen.Select(t => t.Name).ToArray();
                typebuilder.DefineGenericParameters(genp);
            }
            // Dataslots definieren
            List<Declaration> statics=null;
            foreach (Declaration decl in declarations)
            {
                decl.DefineMember(this);
                if(decl.NeedStaticInitialize)
                {
                    if(statics==null)
                    {
                        statics=new List<Declaration>();
                    }
                    statics.Add(decl);
                }
            }
            if (statics != null)
            {
                var cctor = typebuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, Common.ReflectionObjects.c_param_);
                ILGenerator ilgen = cctor.GetILGenerator();
                foreach(Declaration decl in statics)
                {
                    decl.EmitStaticInitialization(ilgen);
                }
                ilgen.Emit(OpCodes.Ret);
            }

            //IAspectInfo 
            ProxyJoinPointCollection proxyjpc = joinpointcollection as ProxyJoinPointCollection;
            MethodInfo miImplGetAspects;
            int aspectindex;
            if (proxyjpc != null)
            {
                miImplGetAspects = proxyjpc.ImplOfGetAspectsMethod;
                aspectindex = proxyjpc.AspectIndex + declarations.Aspects.Length;
            }
            else
            {
                miImplGetAspects = null;
                aspectindex = declarations.Aspects.Length;
            }


            MethodInfo miGetAspectsMethodImpl = AspectInfoImpl.AddIDynamicAspectInfoInterface(
                typebuilder,
                declarations.Aspects,
                targetclass,
                proxyjpc !=null ? proxyjpc.ImplOfGetAspectsMethod:null);

            // Classenobjekt anlegen
            if (aspects!=null)
            {
                classobjectbuilder.DefineType(typename + WeavingCodeNames.c_classObjectExtension, false, aspects);
            }
            else
            {
                classobjectbuilder.DefineType(WeavingCodeNames.GetClassObjectName(BaseType), abstracttype, null);
            }

            // jetzt werden die Methoden generiert und die neuen Joinpoints für den Typ erstellt
            int iPos;
            // Construktoren Rendern
            // Der Proxy implementiert für jeden Joinpint auch einen ctor
            ConstructorJoinPoint[] ctorjoinpoints = new ConstructorJoinPoint[joinpointcollection.CtorJoinPoints.Length];
            System.Diagnostics.Debug.Assert(definedCtorBuilder.Length == joinpointcollection.CtorJoinPoints.Length);
            for (iPos = 0; iPos < ctorjoinpoints.Length; iPos++)
            {
                System.Diagnostics.Debug.Assert(definedCtorBuilder[iPos] != null);
                System.Diagnostics.Debug.Assert(definedCtorBuilder[iPos].JoinPoint == joinpointcollection.CtorJoinPoints[iPos]);
                ProxyMemberBuilder pmb = definedCtorBuilder[iPos];
                pmb.Emit();
                ctorjoinpoints[iPos] = (ConstructorJoinPoint)pmb.BindJoinPointToType();
            }

            // virtuelle Methoden
            MethodInfoJoinPoint[] vtablejoinpoints;
            if (definedVTableBuilder == null) // nichts verwoben
            {
                vtablejoinpoints = joinpointcollection.VTableJoinPoints;
            }
            else
            {
                vtablejoinpoints = new MethodInfoJoinPoint[joinpointcollection.VTableJoinPoints.Length];
                System.Diagnostics.Debug.Assert(definedVTableBuilder.Length == joinpointcollection.VTableJoinPoints.Length);
                for (iPos = 0; iPos < vtablejoinpoints.Length; iPos++)
                {
                    ProxyMemberBuilder pmb = definedVTableBuilder[iPos];
                    if (pmb != null)
                    {
                        System.Diagnostics.Debug.Assert(pmb.JoinPoint == joinpointcollection.VTableJoinPoints[iPos]);
                        pmb.Emit();
                        vtablejoinpoints[iPos] = (MethodInfoJoinPoint)pmb.BindJoinPointToType();
                    }
                    else
                    {
                        vtablejoinpoints[iPos] = joinpointcollection.VTableJoinPoints[iPos];
                    }
                }
            }

            // Interfacemethoden
            int ifccount = joinpointcollection.InterfaceJoinPoints.Length;
            System.Diagnostics.Debug.Assert(ifccount == definedInterfaceBuilder.Length);
            if (introducedInterfaceBuilder != null) ifccount += introducedInterfaceBuilder.Length;
            InterfaceJoinPoints[] interfacejoinpoints = new InterfaceJoinPoints[ifccount];
            for (iPos = 0; iPos < joinpointcollection.InterfaceJoinPoints.Length; iPos++)
            {
                ProxyMemberBuilder[] pmbarray = definedInterfaceBuilder[iPos];
                if (pmbarray == null)
                {
                    interfacejoinpoints[iPos] = joinpointcollection.InterfaceJoinPoints[iPos];
                }
                else
                {
                    typebuilder.AddInterfaceImplementation(joinpointcollection.InterfaceJoinPoints[iPos].Interface.Interface);

                    // Dieses Flag muss nicht mehr gesetzt werden, wenn einmal verwoben wurde
                    interfacejoinpoints[iPos].bContainsVirtualMethods = false;
                    interfacejoinpoints[iPos].Interface = joinpointcollection.InterfaceJoinPoints[iPos].Interface;

                    MethodInfoJoinPoint[] newifcjp = new MethodInfoJoinPoint[pmbarray.Length];
                    System.Diagnostics.Debug.Assert(pmbarray.Length == joinpointcollection.InterfaceJoinPoints[iPos].JoinPoints.Length);

                    for (int jppos = 0; jppos < newifcjp.Length; jppos++)
                    {
                        ProxyMemberBuilder pmb = pmbarray[jppos];
                        System.Diagnostics.Debug.Assert(pmb != null);
                        //System.Diagnostics.Debug.Assert(joinpointcollection.InterfaceJoinPoints[iPos].JoinPoints[jppos] == pmb.JoinPoint);

                        pmb.Emit();
                        newifcjp[jppos] = (MethodInfoJoinPoint)pmb.BindJoinPointToType();
                    }

                    interfacejoinpoints[iPos].JoinPoints = newifcjp;
                }
            }
            // Die Introductions
            int intrpos = 0;
            for (; iPos < ifccount; iPos++, intrpos++)
            {
                System.Diagnostics.Debug.Assert(intrpos < introducedInterfaceBuilder.Length);
                typebuilder.AddInterfaceImplementation(introducedInterfaces[intrpos].Interface);
                ProxyMemberBuilder[] pmbarray = introducedInterfaceBuilder[intrpos];
                System.Diagnostics.Debug.Assert(introducedInterfaces[intrpos].JoinPoints.Length == pmbarray.Length);

                interfacejoinpoints[iPos].bContainsVirtualMethods = false;
                interfacejoinpoints[iPos].Interface = introducedInterfaces[intrpos];

                MethodInfoJoinPoint[] newifcjp = new MethodInfoJoinPoint[pmbarray.Length];
                for (int jppos = 0; jppos < pmbarray.Length; jppos++)
                {
                    ProxyMemberBuilder pmb = pmbarray[jppos];
                    System.Diagnostics.Debug.Assert(pmb != null);
                    System.Diagnostics.Debug.Assert(pmb.JoinPoint == introducedInterfaces[intrpos].JoinPoints[jppos]);
                    pmb.Emit();
                    newifcjp[jppos] = (MethodInfoJoinPoint)pmb.BindJoinPointToType();
                }
                interfacejoinpoints[iPos].JoinPoints = newifcjp;
            }


            // Typ bauen
            basetype = typebuilder.CreateType();
            foreach (Declaration decl in declarations)
            {
                decl.CreateType();
            }
            if (this.globalaspectcontainer != null && aspects != null)
            {
                this.globalaspectcontainer.CreateType();
            }

            // JoinPoint's aktualisieren
            joinpointcollection = new ProxyJoinPointCollection(joinpointcollection, basetype, ctorjoinpoints, vtablejoinpoints, interfacejoinpoints, miGetAspectsMethodImpl, aspectindex);

            // Das Klassenobjekt Bauen
            classobjectbuilder.CreateClassObject();

            iscreated = true;
        }

        /// <summary>
        /// Baut den Typ zusammen und setzt statische Variablen
        /// </summary>
        /// <returns>Objekt welches den typ und die Generierung selbigen beschreibt</returns>
        public ClassObject CreateType()
        {
            ClassObject cobj = this.classobjectbuilder.CreateInstance();
           
            return cobj;
        }

    }
}
