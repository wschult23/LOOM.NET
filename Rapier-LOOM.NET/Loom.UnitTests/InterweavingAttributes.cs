// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom;
using Loom.JoinPoints;


namespace Loom.UnitTests.InterweavingAttributes
{
	public class InterweavingAttributeAspects:TestBase
	{
		public class BeforeIncludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Call(Advice.Before)]
			public void BeforeString()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAspect.BeforeString");
			}		
			[Loom.JoinPoints.Include(typeof(B))]
			[Call(Advice.Before)]
			public void BeforeTypeOf(int i)
			{
				Assert.AreEqual(11,i,"invalid value");
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAspect.BeforeTypeOf");
			}		
		}
		public class AfterIncludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Call(Advice.After)]
			public void AfterString()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAspect.AfterString");
			}
			[Loom.JoinPoints.Include(typeof(B))]
			[Call(Advice.After)]
			public void AfterTypeOf(int i)
			{
				Assert.AreEqual(11,i,"invalid value");
				RegisterCall(ref aspectTestAfter, "AfterIncludeAspect.AfterTypeOf");
			}
		}
		public class InsteadIncludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Call(Advice.Around)]
			public T InsteadString<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAspect.AroundString");
                return default(T);
			}
			[Loom.JoinPoints.Include(typeof(B))]
			[Call(Advice.Around)]
			public T InsteadTypeOf<T>(T t)
			{
				Assert.AreEqual(11, t,"invalid value");
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAspect.AroundTypeOf");
				return t;
			}
		}
		public class AfterReturningIncludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Call(Advice.AfterReturning)]
			public void AfterReturningString([JPRetVal] int returnvalue,int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAspect.AfterReturningString");
			}
			[Loom.JoinPoints.Include(typeof(F))]
			[Call(Advice.AfterReturning)]
			public void AfterReturningTypeOf([JPRetVal] double returnvalue,ref double d)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAspect.AfterReturningTypeOf");
			}
		}
		public class AfterThrowingIncludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Call(Advice.AfterThrowing)]
			public T AfterThrowingString<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAspect.AfterThrowingString");
                return default(T);
			}
			[Loom.JoinPoints.Include(typeof(B))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowingTypeOf<T>([JPException] Exception e, T t)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAspect.AfterThrowingTypeOf");
				return t;
			}
		}	
		public class BeforeIncludeAllAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAllAspect.Before");
			}			
		}
		public class AfterIncludeAllAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAllAspect.After");
			}
		}
		public class InsteadIncludeAllAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAllAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningIncludeAllAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAllAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAllAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAllAspect.AfterThrowing");
                return default(T);
            }
		}
		public class BeforeExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude("Test")]
			[Call(Advice.Before)]
			public void BeforeString()
			{
				RegisterCall(ref aspectTestBefore, "BeforeExcludeAspect.BeforeString");
			}
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude(typeof(B))]
			[Call(Advice.Before)]
			public void BeforeTypeOf(int i)
			{
				RegisterCall(ref aspectTestBefore, "BeforeExcludeAspect.BeforeTypeOf");
			}
		}
		public class AfterExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude("Test")]
			[Call(Advice.After)]
			public void AfterString()
			{
				RegisterCall(ref aspectTestAfter, "AfterExcludeAspect.AfterString");
			}
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude(typeof(B))]
			[Call(Advice.After)]
			public void AfterTypeOf(int i)
			{
				RegisterCall(ref aspectTestAfter, "AfterExcludeAspect.AfterTypeOf");
			}
		}
		public class InsteadExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude("Test")]
			[Call(Advice.Around)]
			public T InsteadString<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadExcludeAspect.AroundString");
                return default(T);
			}
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude(typeof(B))]
			[Call(Advice.Around)]
			public T InsteadTypeOf<T>(int i)
			{
				RegisterCall(ref aspectTestInstead, "InsteadExcludeAspect.AroundTypeOf");
				return (T)(object)i;
			}
		}
		public class AfterReturningExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude("Test")]
			[Call(Advice.AfterReturning)]
            public void AfterReturningString([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningExcludeAspect.AfterReturningString");
			}
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude(typeof(F))]
			[Call(Advice.AfterReturning)]
            public void AfterReturningTypeOf([JPRetVal] double returnvalue, ref double d)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningExcludeAspect.AfterReturningTypeOf");
			}
		}
		public class AfterThrowingExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude("Test")]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowingString<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingExcludeAspect.AfterThrowingString");
                return default(T);
			}
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.Exclude(typeof(B))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowingTypeOf<T>([JPException] Exception e, int i)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingExcludeAspect.AfterThrowingTypeOf");
				return (T)(object)i;
			}
		}		
		public class BeforeExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeExcludeIfAttributeDefinedAttributeAspect.Before");
			}
		}
		public class AfterExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterExcludeIfAttributeDefinedAspect.After");
			}
		}
		public class InsteadExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadExcludeIfAttributeDefinedAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningExcludeIfAttributeDefinedAspect.AfterReturning");
			}
		}
		public class AfterThrowingExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingExcludeIfAttributeDefinedAspect.AfterThrowing");
                return default(T);
            }
		}
		public class BeforeIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeIfAttributeDefinedAttribute.Before");
			}
		}
		public class AfterIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeIfAttributeDefinedAspect.After");
			}
		}
		public class InsteadIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeIfAttributeDefinedAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeIfAttributeDefinedAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(MethodTestAttribute))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeIfAttributeDefinedAspect.AfterThrowing");
                return default(T);
            }
		}

        /*
        public class BeforePreserveTypeAspectParams:TestAspectBase
		{
			[Call(Advice.Before)]
			public void Test([PreserveType()]params object[] o)
			{
				RegisterCall(ref aspectTestBefore, "BeforePreserveTypeAspectParams.Test");
			}
		}
		public class BeforePreserveTypeAspectReturn:TestAspectBase
		{
			[Call(Advice.Before)]
			[return:Loom.JoinPoints.PreserveType()]
			public object Test2()
			{
				RegisterCall(ref aspectTestBefore, "BeforePreserveTypeAspectReturn.Test2");
				object o=new object();
				return o;
			}
		}
		public class AfterPreserveTypeAspectParams:TestAspectBase
		{
			[Call(Advice.After)]
			public void Test([PreserveType]params object[] o)
			{
				RegisterCall(ref aspectTestAfter, "AfterPreserveTypeAspectParams.Test");
			}
		}
		public class AfterPreserveTypeAspectReturn:TestAspectBase
		{
			[Call(Advice.After)]
			[return:Loom.JoinPoints.PreserveType()]
			public object Test2()
			{
				RegisterCall(ref aspectTestAfter, "AfterPreserveTypeAspectReturn.Test2");
				object o=new object();
				return o;
			}
		}
		public class InsteadPreserveTypeAspectParams:TestAspectBase
		{
			[Call(Advice.Around)]
			public void Test([PreserveType()]params object[] o)
			{
				RegisterCall(ref aspectTestInstead, "InsteadPreserveTypeAspectParams.Test");
			}
		}
		public class InsteadPreserveTypeAspectReturn:TestAspectBase
		{
			[Call(Advice.Around)]
			[return:Loom.JoinPoints.PreserveType()]
			public object Test2()
			{
				RegisterCall(ref aspectTestInstead, "InsteadPreserveTypeAspectReturn.Test2");
				object o=new object();
				return o;
			}
		}
		public class AfterReturningPreserveTypeAspect:TestAspectBase
		{
			[Call(Advice.AfterReturning)]
			[return:Loom.JoinPoints.PreserveType()]
			public object Test3(object returnvalue,[PreserveType()]params object[] o)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningPreserveTypeAspect.Test3");
				return o[0];
			}
		}
		public class AfterThrowingPreserveTypeAspectParams:TestAspectBase
		{
			[Call(Advice.AfterThrowing)]
			public void Test(Exception e,[PreserveType()]params object[] o)
			{
				Assert.AreEqual(typeof(object),o[0].GetType(),"invalid parameter value");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingPreserveTypeAspectParams.Test");
			}
		}
		public class AfterThrowingPreserveTypeAspectReturn:TestAspectBase
		{
			[Call(Advice.AfterThrowing)]
			[return:Loom.JoinPoints.PreserveType()]
			public object Test2(Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingPreserveTypeAspectReturn.Test2");
				object o=new object();
				return o;
			}
		}
         * */
		public class BeforeDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeDeclaredOnlyAspect.Before");
			}
		}
		public class AfterDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterDeclaredOnlyAspect.After");
			}
		}
		public class InsteadDeclaredOnlyAspect:TestAspectBase
		{
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead,"InsteadDeclaredOnlyAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningDeclaredOnlyAspect:TestAspectBase
		{
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning,"AfterReturningDeclaredOnlyAspect.AfterReturning");
			}
		}
		public class AfterThrowingDeclaredOnlyAspect:TestAspectBase
		{
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingDeclaredOnlyAspect.AfterThrowing");
                return default(T);
			}
		}
		public class BeforeIncludeAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAndExcludeAspect.Before");
			}
		}
		public class AfterIncludeAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAndExcludeAspect.After");
			}
		}
		public class InsteadIncludeAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAndExcludeAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningIncludeAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAndExcludeAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAndExcludeAspect.AfterThrowing");
                return default(T);
			}
		}
		public class BeforeIncludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAndIncludeIfAttributeDefinedAspect.Before");
			}
		}
		public class AfterIncludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAndIncludeIfAttributeDefinedAspect.After");
			}
		}
		public class InsteadIncludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAndIncludeIfAttributeDefinedAspect.Around");
                return default(T);
			}
		}
		public class AfterReturningIncludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAndIncludeIfAttributeDefinedAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAndIncludeIfAttributeDefinedAspect.AfterThrowing");
                return default(T);
			}
		}
		public class BeforeIncludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAndExcludeIfAttributeDefinedAspect.Before");
			}
		}
		public class AfterIncludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAndExcludeIfAttributeDefinedAspect.After");
			}
		}
		public class InsteadIncludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAndExcludeIfAttributeDefinedAspect.Around");
                return default(T);
			}
		}
		public class AfterReturningIncludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAndExcludeIfAttributeDefinedAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAndExcludeIfAttributeDefinedAspect.AfterThrowing");
                return default(T);
			}
		}
		public class BeforeIncludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAndDeclaredOnlyAspect.Before");
			}
		}
		public class AfterIncludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAndDeclaredOnlyAspect.After");
			}
		}
		public class InsteadIncludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAndDeclaredOnlyAspect.Around");
                return default(T);
			}
		}
		public class AfterReturningIncludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAndDeclaredOnlyAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Include("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAndDeclaredOnlyAspect.AfterThrowing");
                return default(T);
			}
		}
		public class BeforeExcludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeExcludeAndIncludeIfAttributeDefinedAspect.Before");
			}
		}
		public class AfterExcludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterExcludeAndIncludeIfAttributeDefinedAspect.After");
			}
		}
		public class InsteadExcludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadExcludeAndIncludeIfAttributeDefinedAspect.Around");
                return default(T);
			}
		}
		public class AfterReturningExcludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningExcludeAndIncludeIfAttributeDefinedAspect.AfterReturning");
			}
		}
		public class AfterThrowingExcludeAndIncludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingExcludeAndIncludeIfAttributeDefinedAspect.AfterThrowing");
                return default(T);
			}
		}
		
		public class BeforeExcludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeExcludeAndExcludeIfAttributeDefinedAspect.Before");
			}
		}
		public class AfterExcludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterExcludeAndExcludeIfAttributeDefinedAspect.After");
			}
		}
		public class InsteadExcludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadExcludeAndExcludeIfAttributeDefinedAspect.Around");
                return default(T);
			}
		}
		public class AfterReturningExcludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningExcludeAndExcludeIfAttributeDefinedAspect.AfterReturning");
			}
		}
		public class AfterThrowingExcludeAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test")]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingExcludeAndExcludeIfAttributeDefinedAspect.AfterThrowing");
                return default(T);
            }
		}
		
		public class BeforeExcludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeExcludeAndDeclaredOnlyAspect.Before");
			}
		}
		public class AfterExcludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterExcludeAndDeclaredOnlyAspect.After");
			}
		}
		public class InsteadExcludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadExcludeAndDeclaredOnlyAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningExcludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningExcludeAndDeclaredOnlyAspect.AfterReturning");
			}
		}
		public class AfterThrowingExcludeAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.Exclude("Test11")]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingExcludeAndDeclaredOnlyAspect.AfterThrowing");
                return default(T);
            }
		}
		
		public class BeforeIncludeAllAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAllAndExcludeAspect.Before");
			}
		}
		public class AfterIncludeAllAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAllAndExcludeAspect.After");
			}
		}
		public class InsteadIncludeAllAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAllAndExcludeAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningIncludeAllAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAllAndExcludeAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAllAndExcludeAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.Exclude("Test2")]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAllAndExcludeAspect.AfterThrowing");
                return default(T);
            }
		}
		public class BeforeIncludeAllAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAllAndExcludeIfAttributeDefinedAspect.Before");
			}
		}
		public class AfterIncludeAllAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAllAndExcludeIfAttributeDefinedAspect.After");
			}
		}
		public class InsteadIncludeAllAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAllAndExcludeIfAttributeDefinedAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningIncludeAllAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAllAndExcludeIfAttributeDefinedAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAllAndExcludeIfAttributeDefinedAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAllAndExcludeIfAttributeDefinedAspect.AfterThrowing");
                return default(T);
            }
		}
		public class BeforeIncludeAllAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeAllAndDeclaredOnlyAspect.Before");
			}
		}
		public class AfterIncludeAllAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeAllAndDeclaredOnlyAspect.After");
			}
		}
		public class InsteadIncludeAllAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeAllAndDeclaredOnlyAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningIncludeAllAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeAllAndDeclaredOnlyAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeAllAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAll()]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException]Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeAllAndDeclaredOnlyAspect.AfterThrowing");
                return default(T);
			}
		}
		public class BeforeIncludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeIncludeIfAttributeDefinedAndDeclaredOnlyAspect.Before");
			}
		}
		public class AfterIncludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterIncludeIfAttributeDefinedAndDeclaredOnlyAspect.After");
			}
		}
		public class InsteadIncludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadIncludeIfAttributeDefinedAndDeclaredOnlyAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningIncludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningIncludeIfAttributeDefinedAndDeclaredOnlyAspect.AfterReturning");
			}
		}
		public class AfterThrowingIncludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.IncludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingIncludeIfAttributeDefinedAndDeclaredOnlyAspect.AfterThrowing");
                return default(T);
            }
		}
		public class BeforeExcludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeExcludeIfAttributeDefinedAndDeclaredOnlyAspect.Before");
			}
		}
		public class AfterExcludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterExcludeIfAttributeDefinedAndDeclaredOnlyAspect.After");
			}
		}
		public class InsteadExcludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.Around)]
			public T Instead<T>()
			{
				RegisterCall(ref aspectTestInstead, "InsteadExcludeIfAttributeDefinedAndDeclaredOnlyAspect.Around");
                return default(T);
            }
		}
		public class AfterReturningExcludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterReturning)]
            public void AfterReturning([JPRetVal] int returnvalue, int i)
			{
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningExcludeIfAttributeDefinedAndDeclaredOnlyAspect.AfterReturning");
			}
		}
		public class AfterThrowingExcludeIfAttributeDefinedAndDeclaredOnlyAspect:TestAspectBase
		{
			[Loom.JoinPoints.ExcludeAnnotatedAttribute(typeof(Loom.UnitTests.TestBase.MethodTestAttribute))]
            [Loom.JoinPoints.IncludeDeclaredOnly()]
			[Call(Advice.AfterThrowing)]
            public T AfterThrowing<T>([JPException] Exception e)
			{
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingExcludeIfAttributeDefinedAndDeclaredOnlyAspect.AfterThrowing");
                return default(T);
            }
		}		
	}
	[TestClass]
	public class IncludeAttribute:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void BeforeString()
		{
			BeforeIncludeAspect bia=new BeforeIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,bia);
			a.Test();
			Assert.AreEqual(1,bia.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeStringFail()
		{
			BeforeIncludeAspect bia=new BeforeIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,bia);
			a.Test2();
			Assert.AreEqual(0,bia.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeTypeOf()
		{
			BeforeIncludeAspect bia=new BeforeIncludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,bia);
			int i=11;
			Assert.AreEqual(11,b.Test(i),"invalid return value");
			Assert.AreEqual(1,bia.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,b.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeTypeOfFail()
		{
			BeforeIncludeAspect bia=new BeforeIncludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,bia);
			int i=11;
			Assert.AreEqual(11,aio.Test(i),"invalid return value");
			Assert.AreEqual(0,bia.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,aio.testCall,"object not called");
		}
		[TestMethod]
		public void AfterString()
		{
			AfterIncludeAspect aia=new AfterIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aia);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aia.aspectTestAfter,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterStringFail()
		{
			AfterIncludeAspect aia=new AfterIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aia);
			a.Test2();			
			Assert.AreEqual(1,a.testCall,"object not called");			
			Assert.AreEqual(0,aia.aspectTestAfter,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterTypeOf()
		{
			AfterIncludeAspect aia=new AfterIncludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,aia);
			int i=11;
			Assert.AreEqual(11,b.Test(i),"invalid return value");
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,aia.aspectTestAfter,"Aspect not jpinterwoven");
		}	
		[TestMethod]
		public void AfterTypeOfFail()
		{
			AfterIncludeAspect aia=new AfterIncludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,aia);
			int i=11;
			Assert.AreEqual(11,aio.Test(i),"invalid return value");
			Assert.AreEqual(1,aio.testCall,"object not called");
			Assert.AreEqual(0,aia.aspectTestAfter,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void InsteadString()
		{
			InsteadIncludeAspect iia=new InsteadIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iia);
			a.Test();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,iia.aspectTestInstead,"Aspect not jpinterwoven");
		}		
		[TestMethod]
		public void InsteadStringFail()
		{
			InsteadIncludeAspect iia=new InsteadIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iia);
			a.Test2();
			Assert.AreEqual(1,a.testCall,"object  not called");
			Assert.AreEqual(0,iia.aspectTestInstead,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void InsteadTypeOf()
		{
			InsteadIncludeAspect iia=new InsteadIncludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,iia);				
			Assert.AreEqual(11,b.Test(11),"invalid return value");
			Assert.AreEqual(0,b.testCall,"object called");
			Assert.AreEqual(1,iia.aspectTestInstead,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void InsteadTypeOfFail()
		{
			InsteadIncludeAspect iia=new InsteadIncludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,iia);
			Assert.AreEqual(11,aio.Test(11),"invalid return value");
			Assert.AreEqual(1,aio.testCall,"object not called");
			Assert.AreEqual(0,iia.aspectTestInstead,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterReturningString()
		{
			AfterReturningIncludeAspect aria=new AfterReturningIncludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,aria);
			while(11!=b.Test(11))
			{
				if(aria.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}			
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,aria.aspectTestAfterReturning,"Aspect not jpinterwoven"); 

		}
		[TestMethod]
		public void AfterReturningStringFail()
		{
			AfterReturningIncludeAspect aria=new AfterReturningIncludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,aria);
			while(11!=b.Test2(11))
			{
				if(aria.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}			
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,aria.aspectTestAfterReturning,"Aspect jpinterwoven"); 
		}
		[TestMethod]
		public void AfterReturningTypeOf()
		{
			AfterReturningIncludeAspect aria=new AfterReturningIncludeAspect();
			F f=(F)Weaver.CreateInstance(typeof(F),null,aria);
			double d=11.11;
			while(11.11!=f.Test(ref d))
			{
				if(aria.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,f.testCall,"object not called");
			Assert.AreEqual(2,aria.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterReturningTypeOfFail()
		{
			AfterReturningIncludeAspect aria=new AfterReturningIncludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,aria);
			double d=11.11;
			while(11.11!=aio.Test(ref d))
			{
				if(aria.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,aio.testCall,"object not called");
			Assert.AreEqual(0,aria.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowingString()
		{
			AfterThrowingIncludeAspect atia=new AfterThrowingIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atia);
			a.bThrowException=true;				
			try
			{
				a.Test();
			}
			catch
			{					
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atia.aspectTestAfterThrowing,"Aspect not called after throwing an exception");
			}				
		}
		[TestMethod]
		public void AfterThrowingStringFail()
		{
			AfterThrowingIncludeAspect atia=new AfterThrowingIncludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atia);
			a.bThrowException=true;				
			try
			{
				a.Test2();
			}
			catch
			{					
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(0,atia.aspectTestAfterThrowing,"Aspect called after throwing an exception");
			}				
		}
		[TestMethod]
		public void AfterThrowingTypeOf()
		{
			AfterThrowingIncludeAspect atia=new AfterThrowingIncludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,atia);
			b.bThrowException=true;
			try
			{
				b.Test(11);
			}
			catch
			{
				Assert.AreEqual(1,b.testCall,"object not called");
				Assert.AreEqual(2,atia.aspectTestAfterThrowing,"Aspect not called after throwing an exception");
			}		
		}
		[TestMethod]
		public void AfterThrowingTypeOfFail()
		{
			AfterThrowingIncludeAspect atia=new AfterThrowingIncludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,atia);
			aio.bThrowException=true;
			try
			{
				aio.Test(11);
			}
			catch
			{
				Assert.AreEqual(1,aio.testCall,"object not called");
				Assert.AreEqual(0,atia.aspectTestAfterThrowing,"Aspect called after throwing an exception");
			}
		}
	}		
	[TestClass]
	public class IncludeAllAttribute:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAllAspect biaa=new BeforeIncludeAllAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,biaa);
			a.Test();
			Assert.AreEqual(1,biaa.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			biaa.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(3,biaa.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(4,a.testCall,"object not called");			
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAllAspect aiaa=new AfterIncludeAllAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aiaa);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aiaa.aspectTestAfter,"Aspect not jpinterwoven");
			a.testCall=0;
			aiaa.aspectTestAfter=0;
			a.Test2();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(4,aiaa.aspectTestAfter,"Aspect not jpinterwoven");				
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAllAspect iiaa=new InsteadIncludeAllAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iiaa);
			a.Test();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,iiaa.aspectTestInstead,"Aspect not jpinterwoven");
			iiaa.aspectTestInstead=0;
			a.Test2();
			Assert.AreEqual(2,iiaa.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.testCall,"object called");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAllAspect ariaa=new AfterReturningIncludeAllAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ariaa);
			while(11!=b.Test(11))
			{
				if(ariaa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}			
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ariaa.aspectTestAfterReturning,"Aspect not jpinterwoven"); 
			b.testCall=0;
			ariaa.aspectTestAfterReturning=0;
			while(11!=b.Test2(11))
			{
				if(ariaa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(4,ariaa.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAllAspect atiaa=new AfterThrowingIncludeAllAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atiaa);
			a.bThrowException=true;				
			try
			{
				a.Test();
			}
			catch
			{					
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atiaa.aspectTestAfterThrowing,"Aspect not called after throwing an exception");
			}
			a.testCall=0;
			atiaa.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(4,atiaa.aspectTestAfterThrowing,"Aspect not called after throwing an exception");
			}
		}
	}		
	[TestClass]
	public class ExcludeAttribute:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void BeforeString()
		{
			BeforeExcludeAspect bea=new BeforeExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,bea);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,bea.aspectTestBefore,"Aspect jpinterwoven");		
		}
		[TestMethod]
		public void BeforeStringFail()
		{
			BeforeExcludeAspect bea=new BeforeExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,bea);
			a.Test2();
			Assert.AreEqual(1,bea.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");			
		}
		[TestMethod]
		public void BeforeTypeOf()
		{
			BeforeExcludeAspect bea=new BeforeExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,bea);
			b.Test(11);
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,bea.aspectTestBefore,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void BeforeTypeOfFail()
		{
			BeforeExcludeAspect bea=new BeforeExcludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,bea);
			Assert.AreEqual(11,aio.Test(11),"invalid return value");
			Assert.AreEqual(1,bea.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,aio.testCall,"object not called");			
		}
		[TestMethod]
		public void AfterString()
		{
			AfterExcludeAspect aea=new AfterExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aea);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,aea.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterStringFail()
		{
			AfterExcludeAspect aea=new AfterExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aea);
			a.Test2();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aea.aspectTestAfter,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterTypeOf()
		{
			AfterExcludeAspect aea=new AfterExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,aea);
			Assert.AreEqual(11,b.Test(11),"invalid return value");
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,aea.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterTypeOfFail()
		{
			AfterExcludeAspect aea=new AfterExcludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,aea);
			Assert.AreEqual(11,aio.Test(11),"invalid return value");
			Assert.AreEqual(1,aio.testCall,"object not called");
			Assert.AreEqual(2,aea.aspectTestAfter,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void InsteadString()
		{
			InsteadExcludeAspect iea=new InsteadExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iea);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,iea.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void InsteadStringFail()
		{
			InsteadExcludeAspect iea=new InsteadExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iea);
			a.Test2();
			Assert.AreEqual(1,iea.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.testCall,"object called");
		}
		[TestMethod]
		public void InsteadTypeOf()
		{
			InsteadExcludeAspect iea=new InsteadExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,iea);
			Assert.AreEqual(11,b.Test(11),"invalid return value");
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,iea.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void InsteadTypeOfFail()
		{
			InsteadExcludeAspect iea=new InsteadExcludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,iea);
			Assert.AreEqual(11,aio.Test(11),"invalid return value");
			Assert.AreEqual(0,aio.testCall,"object called");
			Assert.AreEqual(1,iea.aspectTestInstead,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterReturningString()
		{
			AfterReturningExcludeAspect area=new AfterReturningExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,area);
			while(11!=b.Test(11))
			{
				if(area.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}			
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,area.aspectTestAfterReturning,"Aspect jpinterwoven"); 			
		}
		[TestMethod]
		public void AfterReturningStringFail()
		{
			AfterReturningExcludeAspect area=new AfterReturningExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,area);
			while(11!=b.Test2(11))
			{
				if(area.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,area.aspectTestAfterReturning,"Aspect not jpinterwoven"); 	
		}
		[TestMethod]
		public void AfterReturningTypeOf()
		{
			AfterReturningExcludeAspect area=new AfterReturningExcludeAspect();
			F f=(F)Weaver.CreateInstance(typeof(F),null,area);
			double d=11.11;
			while(11.11!=f.Test(ref d))
			{
				if(area.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,f.testCall,"object not called");
			Assert.AreEqual(0,area.aspectTestAfterReturning,"Aspect jpinterwoven");
			
			
		}
		[TestMethod]
		public void AfterReturningTypeOfFail()
		{
			AfterReturningExcludeAspect area=new AfterReturningExcludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,area);
			double d=11.11;
			while(11.11!=aio.Test(ref d))
			{
				if(area.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,aio.testCall,"object not called");
			Assert.AreEqual(2,area.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowingString()
		{
			AfterThrowingExcludeAspect atea=new AfterThrowingExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atea);
			a.bThrowException=true;				
			try
			{
				a.Test();
			}
			catch
			{					
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(0,atea.aspectTestAfterThrowing,"Aspect called after throwing an exception");
			}
			
			
		}
		[TestMethod]
		public void AfterThrowingStringFail()
		{
			AfterThrowingExcludeAspect atea=new AfterThrowingExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atea);
			a.bThrowException=true;	
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atea.aspectTestAfterThrowing,"Aspect not called after throwing an exception");
			}
		}
		[TestMethod]
		public void AfterThrowingTypeOf()
		{
			AfterThrowingExcludeAspect atea=new AfterThrowingExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,atea);
			b.bThrowException=true;
			try
			{
				b.Test(11);
			}
			catch
			{
				Assert.AreEqual(1,b.testCall,"object not called");
				Assert.AreEqual(0,atea.aspectTestAfterThrowing,"Aspect called after throwing an exception");
			}
		}
		[TestMethod]
		public void AfterThrowingTypeOfFail()
		{
			AfterThrowingExcludeAspect atea=new AfterThrowingExcludeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,atea);
			try
			{
				aio.Test(11);
			}
			catch
			{
				Assert.AreEqual(1,aio.testCall,"object not called");
				Assert.AreEqual(2,atea.aspectTestAfterThrowing,"Aspect not called after throwing an exception");
			}
		}
	}	
	[TestClass]
	public class ExcludeIfAttributeDefinedAttribute:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			// 4.5.04 WS: Bitte immmer Basis-Setup aufrufen, da hier der Output vom
			// Aspektweaver auf die NUnit-Console umgeleitet wird!
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeExcludeIfAttributeDefinedAspect beiada=new BeforeExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,beiada);
			a.Test2();
			Assert.AreEqual(0,beiada.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");						
		}
		[TestMethod]
		public void BeforeFail()
		{
			BeforeExcludeIfAttributeDefinedAspect beiada=new BeforeExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,beiada);
			a.Test();
			Assert.AreEqual(1,beiada.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");						
		}
		[TestMethod]
		public void After()
		{
			AfterExcludeIfAttributeDefinedAspect aeiada=new AfterExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aeiada);
			a.Test2();
			Assert.AreEqual(0,aeiada.aspectTestAfter,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");				
		}
		[TestMethod]
		public void AfterFail()
		{
			AfterExcludeIfAttributeDefinedAspect aeiada=new AfterExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aeiada);
			a.Test();
			Assert.AreEqual(2,aeiada.aspectTestAfter,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");				
		}
		[TestMethod]
		public void Instead()
		{
			InsteadExcludeIfAttributeDefinedAspect ieiada=new InsteadExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ieiada);
			a.Test2();
			Assert.AreEqual(0,ieiada.aspectTestInstead,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
		}
		[TestMethod]
		public void InsteadFail()
		{
			InsteadExcludeIfAttributeDefinedAspect ieiada=new InsteadExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ieiada);
			a.Test();
			Assert.AreEqual(1,ieiada.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.testCall,"object called");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningExcludeIfAttributeDefinedAspect areiada=new AfterReturningExcludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,areiada);
			while(11!=b.Test2(11))
			{
				if(areiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(0,areiada.aspectTestAfterReturning,"Aspect jpinterwoven");
			Assert.AreEqual(1,b.testCall,"object not called");
		}
		[TestMethod]
		public void AfterReturningFail()
		{
			AfterReturningExcludeIfAttributeDefinedAspect areiada=new AfterReturningExcludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,areiada);
			while(11!=b.Test(11))
			{
				if(areiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(2,areiada.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(1,b.testCall,"object not called");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingExcludeIfAttributeDefinedAspect ateiada=new AfterThrowingExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ateiada);
			a.bThrowException=true;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(0,ateiada.aspectTestAfterThrowing,"Aspect jpinterwoven after throwing an exception");
			}
		}
		[TestMethod]
		public void AfterThrowingFail()
		{
			AfterThrowingExcludeIfAttributeDefinedAspect ateiada=new AfterThrowingExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ateiada);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,ateiada.aspectTestAfterThrowing,"Aspect not jpinterwoven after throwing an exception");
			}
		}
	}

	[TestClass]
	public class IncludeIfAttributeDefinedAttribute:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeIfAttributeDefinedAspect biiada=new BeforeIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,biiada);
			a.Test2();
			Assert.AreEqual(1,biiada.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeFail()
		{
			BeforeIncludeIfAttributeDefinedAspect biiada=new BeforeIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,biiada);
			a.Test();
			Assert.AreEqual(0,biiada.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeIfAttributeDefinedAspect aiiada=new AfterIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aiiada);
			a.Test2();
			Assert.AreEqual(2,aiiada.aspectTestAfter,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
		}
		[TestMethod]
		public void AfterFail()
		{
			AfterIncludeIfAttributeDefinedAspect aiiada=new AfterIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aiiada);
			a.Test();
			Assert.AreEqual(0,aiiada.aspectTestAfter,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeIfAttributeDefinedAspect iiiada=new InsteadIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iiiada);
			a.Test2();
			Assert.AreEqual(1,iiiada.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.testCall,"object called");
		}
		[TestMethod]
		public void InsteadFail()
		{
			InsteadIncludeIfAttributeDefinedAspect iiiada=new InsteadIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iiiada);
			a.Test();
			Assert.AreEqual(0,iiiada.aspectTestInstead,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object called");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeIfAttributeDefinedAspect ariiada=new AfterReturningIncludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ariiada);
			while(11!=b.Test2(11))
			{
				if(ariiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ariiada.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterReturningFail()
		{
			AfterReturningIncludeIfAttributeDefinedAspect ariiada=new AfterReturningIncludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ariiada);
			while(11!=b.Test(11))
			{
				if(ariiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,ariiada.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeIfAttributeDefinedAspect atiiada=new AfterThrowingIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atiiada);
			a.bThrowException=true;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atiiada.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
		}
		[TestMethod]
		public void AfterThrowingFail()
		{
			AfterThrowingIncludeIfAttributeDefinedAspect atiiada=new AfterThrowingIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atiiada);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(0,atiiada.aspectTestAfterThrowing,"Aspect jpinterwoven");
			}
		}
	}

    /*
	[TestClass]
	public class PreserveTypeAttribute:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void BeforeParams()
		{
			BeforePreserveTypeAspectParams bptap=new BeforePreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,bptap);
			object o=new object();
			i.Test(o);
			Assert.AreEqual(1,bptap.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,i.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeReturn()
		{
			BeforePreserveTypeAspectReturn bptar=new BeforePreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,bptar);
			Assert.AreEqual(typeof(object),i.Test2().GetType(),"invalid return value");
			Assert.AreEqual(1,bptar.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,i.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeParamsFail()
		{
			BeforePreserveTypeAspectParams bptap=new BeforePreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,bptap);
			int a=1;
			i.Test5(a);
			Assert.AreEqual(0,bptap.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,i.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeReturnFail()
		{
			BeforePreserveTypeAspectReturn bptar=new BeforePreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,bptar);
			Assert.AreEqual(typeof(int),i.Test4().GetType(),"invalid return value");
			Assert.AreEqual(0,bptar.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,i.testCall,"object not called");
		}
		[TestMethod]
		public void AfterParams()
		{
			AfterPreserveTypeAspectParams aptap=new AfterPreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,aptap);
			object o=new object();
			i.Test(o);
			Assert.AreEqual(1,i.testCall,"object not called");
			Assert.AreEqual(2,aptap.aspectTestAfter,"Aspect not jpinterwoven");
			
		}
		[TestMethod]
		public void AfterReturn()
		{
			AfterPreserveTypeAspectReturn aptar=new AfterPreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,aptar);
			Assert.AreEqual(typeof(object),i.Test2().GetType(),"invalid return value");
			Assert.AreEqual(1,i.testCall,"object not called");
			Assert.AreEqual(2,aptar.aspectTestAfter,"Aspect not jpinterwoven");
			
		}
		[TestMethod]
		public void AfterParamsFail()
		{
			AfterPreserveTypeAspectParams aptap=new AfterPreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,aptap);
			int a=1;
			i.Test5(a);
			Assert.AreEqual(1,i.testCall,"object not called");
			Assert.AreEqual(0,aptap.aspectTestAfter,"Aspect jpinterwoven");
			
		}
		[TestMethod]
		public void AfterReturnFail()
		{
			AfterPreserveTypeAspectReturn aptar=new AfterPreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,aptar);
			Assert.AreEqual(typeof(int),i.Test4().GetType(),"invalid return value");
			Assert.AreEqual(1,i.testCall,"object not called");
			Assert.AreEqual(0,aptar.aspectTestAfter,"Aspect jpinterwoven");
			
		}
		[TestMethod]
		public void InsteadParams()
		{
			InsteadPreserveTypeAspectParams iptap=new InsteadPreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,iptap);
			object o=new object();
			i.Test(o);
			Assert.AreEqual(0,i.testCall,"object called");
			Assert.AreEqual(1,iptap.aspectTestInstead,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void InsteadReturn()
		{
			InsteadPreserveTypeAspectReturn iptar=new InsteadPreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,iptar);
			Assert.AreEqual(typeof(object),i.Test2().GetType(),"invalid return value");
			Assert.AreEqual(0,i.testCall,"object not called");
			Assert.AreEqual(1,iptar.aspectTestInstead,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void InsteadParamsFail()
		{
			InsteadPreserveTypeAspectParams iptap=new InsteadPreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,iptap);
			int a=1;
			i.Test5(a);
			Assert.AreEqual(0,iptap.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(1,i.testCall,"object called");			
		}
		[TestMethod]
		public void InsteadReturnFail()
		{
			InsteadPreserveTypeAspectReturn iptar=new InsteadPreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,iptar);
			Assert.AreEqual(typeof(int),i.Test4().GetType(),"invalid return value");
			Assert.AreEqual(1,i.testCall,"object called");
			Assert.AreEqual(0,iptar.aspectTestInstead,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningPreserveTypeAspect arpta=new AfterReturningPreserveTypeAspect();
			I i=(I)Weaver.CreateInstance(typeof(I),null,arpta);
			object o=new object();
			while(o!=i.Test3(o))
			{
				if(arpta.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}			
			Assert.AreEqual(1,i.testCall,"object not called");
			Assert.AreEqual(2,arpta.aspectTestAfterReturning,"Aspect not jpinterwoven"); 
		}
		[TestMethod]
		public void AfterReturningFail()
		{
			AfterReturningPreserveTypeAspect arpta=new AfterReturningPreserveTypeAspect();
			I i=(I)Weaver.CreateInstance(typeof(I),null,arpta);
			int a=1;
			while(a.ToString()!=i.Test6(a).ToString())
			{
				if(arpta.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,i.testCall,"object not called");
			Assert.AreEqual(0,arpta.aspectTestAfterReturning,"Aspect jpinterwoven"); 
		}
		[TestMethod]
		public void AfterThrowingParams()
		{
			AfterThrowingPreserveTypeAspectParams atptap=new AfterThrowingPreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,atptap);
			object o=new object();
			i.bThrowException=true;				
			try
			{
				i.Test(o);
			}
			catch
			{					
				Assert.AreEqual(1,i.testCall,"object not called");
				Assert.AreEqual(2,atptap.aspectTestAfterThrowing,"Aspect not jpinterwoven after throwing an exception");
			}
		}
		[TestMethod]
		public void AfterThrowingReturn()
		{
			AfterThrowingPreserveTypeAspectReturn atptar=new AfterThrowingPreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,atptar);
			i.bThrowException=true;				
			try
			{
				i.Test2();
			}
			catch
			{					
				Assert.AreEqual(1,i.testCall,"object not called");
				Assert.AreEqual(2,atptar.aspectTestAfterThrowing,"Aspect not jpinterwoven after throwing an exception");
			}
		}
		[TestMethod]
		public void AfterThrowingParamsFail()
		{
			AfterThrowingPreserveTypeAspectParams atptap=new AfterThrowingPreserveTypeAspectParams();
			I i=(I)Weaver.CreateInstance(typeof(I),null,atptap);
			int a=1;
			i.bThrowException=true;				
			try
			{
				i.Test5(a);
			}
			catch
			{					
				Assert.AreEqual(1,i.testCall,"object not called");
				Assert.AreEqual(0,atptap.aspectTestAfterThrowing,"Aspect jpinterwoven after throwing an exception");
			}
		}
		[TestMethod]
		public void AfterThrowingReturnFail()
		{
			AfterThrowingPreserveTypeAspectReturn atptar=new AfterThrowingPreserveTypeAspectReturn();
			I i=(I)Weaver.CreateInstance(typeof(I),null,atptar);
			i.bThrowException=true;				
			try
			{
				i.Test4();
			}
			catch
			{					
				Assert.AreEqual(1,i.testCall,"object not called");
				Assert.AreEqual(0,atptar.aspectTestAfterThrowing,"Aspect jpinterwoven after throwing an exception");
			}
		}
	}
	*/
    [TestClass]
	public class DeclaredOnlyAttribute:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeDeclaredOnlyAspect bdoa=new BeforeDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,bdoa);
			a1.Test11();
			Assert.AreEqual(1,bdoa.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a1.testCall,"object not called");
		}
		[TestMethod]
		public void BeforeFail()
		{
			BeforeDeclaredOnlyAspect bdoa=new BeforeDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,bdoa);
			a1.Test();
			Assert.AreEqual(0,bdoa.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,a1.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterDeclaredOnlyAspect adoa=new AfterDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,adoa);
			a1.Test11();
			Assert.AreEqual(1,a1.testCall,"object not called");
			Assert.AreEqual(2,adoa.aspectTestAfter,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterFail()
		{
			AfterDeclaredOnlyAspect adoa=new AfterDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,adoa);
			a1.Test();
			Assert.AreEqual(0,adoa.aspectTestAfter,"Aspect jpinterwoven");
			Assert.AreEqual(1,a1.testCall,"object not called");
		}
		[TestMethod]
		public void Instead()
		{
			InsteadDeclaredOnlyAspect idoa=new InsteadDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,idoa);
			a1.Test11();
			Assert.AreEqual(0,a1.testCall,"object called");
			Assert.AreEqual(1,idoa.aspectTestInstead,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void InsteadFail()
		{
			InsteadDeclaredOnlyAspect idoa=new InsteadDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,idoa);
			a1.Test();
			Assert.AreEqual(0,idoa.aspectTestInstead,"Aspect jpinterwoven");
			Assert.AreEqual(1,a1.testCall,"object not called");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningDeclaredOnlyAspect ardoa=new AfterReturningDeclaredOnlyAspect();
			B1 b1=(B1)Weaver.CreateInstance(typeof(B1),null,ardoa);
			while(11!=b1.Test11(11))
			{
				if(ardoa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b1.testCall,"object not called");
			Assert.AreEqual(2,ardoa.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterReturningFail()
		{
			AfterReturningDeclaredOnlyAspect ardoa=new AfterReturningDeclaredOnlyAspect();
			B1 b1=(B1)Weaver.CreateInstance(typeof(B1),null,ardoa);
			while(11!=b1.Test(11))
			{
				if(ardoa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(0,ardoa.aspectTestAfterReturning,"Aspect jpinterwoven");
			Assert.AreEqual(1,b1.testCall,"object not called");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingDeclaredOnlyAspect atdoa=new AfterThrowingDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,atdoa);
			a1.bThrowException=true;
			try
			{
				a1.Test11();
			}
			catch
			{
				Assert.AreEqual(1,a1.testCall,"object not called");
				Assert.AreEqual(2,atdoa.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
		}
		[TestMethod]
		public void AfterThrowingFail()
		{
			AfterThrowingDeclaredOnlyAspect atdoa=new AfterThrowingDeclaredOnlyAspect();
			A1 a1=(A1)Weaver.CreateInstance(typeof(A1),null,atdoa);
			a1.bThrowException=true;
			try
			{
				a1.Test();
			}
			catch
			{					
				Assert.AreEqual(1,a1.testCall,"object not called");
				Assert.AreEqual(0,atdoa.aspectTestAfterThrowing,"Aspect jpinterwoven");
			}
		}
	}
	[TestClass]
	public class IncludeAndExcludeAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAndExcludeAspect biaea=new BeforeIncludeAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,biaea);
			a.Test();
			Assert.AreEqual(1,biaea.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			biaea.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(0,biaea.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAndExcludeAspect aiaea=new AfterIncludeAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aiaea);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aiaea.aspectTestAfter,"Aspect not jpinterwoven");
			a.testCall=0;
			aiaea.aspectTestAfter=0;
			a.Test2();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(0,aiaea.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAndExcludeAspect iiaea=new InsteadIncludeAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iiaea);
			a.Test();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,iiaea.aspectTestInstead,"Aspect not jpinterwoven");
			a.testCall=0;
			iiaea.aspectTestInstead=0;
			a.Test2();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,iiaea.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAndExcludeAspect ariaea=new AfterReturningIncludeAndExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ariaea);
			while(11!=b.Test(11))
			{
				if(ariaea.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ariaea.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ariaea.aspectTestAfterReturning=0;
			while(11!=b.Test2(11))
			{
				if(ariaea.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(0,ariaea.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAndExcludeAspect atiaea=new AfterThrowingIncludeAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atiaea);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atiaea.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
			a.testCall=0;
			atiaea.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(0,atiaea.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
		}
	}
	[TestClass]
	public class IncludeAndIncludeIfAttributeDefinedAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAndIncludeIfAttributeDefinedAspect biaiiada=new BeforeIncludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,biaiiada);
			a.Test();
			Assert.AreEqual(1,biaiiada.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			biaiiada.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(3,biaiiada.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(4,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAndIncludeIfAttributeDefinedAspect aiaiiada=new AfterIncludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aiaiiada);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aiaiiada.aspectTestAfter,"Aspect not jpinterwoven");
			a.testCall=0;
			aiaiiada.aspectTestAfter=0;
			a.Test2();			
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(4,aiaiiada.aspectTestAfter,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAndIncludeIfAttributeDefinedAspect iiaiiada=new InsteadIncludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iiaiiada);
			a.Test();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,iiaiiada.aspectTestInstead,"Aspect not jpinterwoven");
			iiaiiada.aspectTestInstead=0;
			a.Test2();			
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(2,iiaiiada.aspectTestInstead,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAndIncludeIfAttributeDefinedAspect ariaiiada=new AfterReturningIncludeAndIncludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ariaiiada);
			while(11!=b.Test(11))
			{
				if(ariaiiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ariaiiada.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ariaiiada.aspectTestAfterReturning=0;
			while(11!=b.Test2(11))
			{
				if(ariaiiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(4,ariaiiada.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAndIncludeIfAttributeDefinedAspect atiaiiada=new AfterThrowingIncludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atiaiiada);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atiaiiada.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
			a.testCall=0;
			atiaiiada.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(4,atiaiiada.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
		}
	}
	[TestClass]
	public class IncludeAndExcludeIfAttributeDefinedAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAndExcludeIfAttributeDefinedAspect biaeiada=new BeforeIncludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,biaeiada);
			a.Test();
			Assert.AreEqual(1,biaeiada.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			biaeiada.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(0,biaeiada.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAndExcludeIfAttributeDefinedAspect aiaeiada=new AfterIncludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aiaeiada);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aiaeiada.aspectTestAfter,"Aspect not jpinterwoven");			
			a.testCall=0;
			aiaeiada.aspectTestAfter=0;
			a.Test2();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(0,aiaeiada.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAndExcludeIfAttributeDefinedAspect iiaeiada=new InsteadIncludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,iiaeiada);
			a.Test();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,iiaeiada.aspectTestInstead,"Aspect not jpinterwoven");			
			iiaeiada.aspectTestInstead=0;
			a.Test2();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,iiaeiada.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAndExcludeIfAttributeDefinedAspect ariaeiada=new AfterReturningIncludeAndExcludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ariaeiada);
			while(11!=b.Test(11))
			{
				if(ariaeiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ariaeiada.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ariaeiada.aspectTestAfterReturning=0;
			while(11!=b.Test2(11))
			{
				if(ariaeiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(0,ariaeiada.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAndExcludeIfAttributeDefinedAspect atiaeiada=new AfterThrowingIncludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,atiaeiada);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atiaeiada.aspectTestAfterThrowing,"Aspect not jpinterwoven");			
			}
			a.testCall=0;
			atiaeiada.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(0,atiaeiada.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
		}
	}
	[TestClass]
	public class IncludeAndDeclaredOnly:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAndDeclaredOnlyAspect biadoa=new BeforeIncludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,biadoa);
			a.Test11();
			Assert.AreEqual(1,biadoa.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			biadoa.aspectTestBefore=0;
			a.Test();
			Assert.AreEqual(0,biadoa.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAndDeclaredOnlyAspect aiadoa=new AfterIncludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,aiadoa);
			a.Test11();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aiadoa.aspectTestAfter,"Aspect not jpinterwoven");			
			a.testCall=0;
			aiadoa.aspectTestAfter=0;
			a.Test();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(0,aiadoa.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAndDeclaredOnlyAspect iiadoa=new InsteadIncludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,iiadoa);
			a.Test11();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,iiadoa.aspectTestInstead,"Aspect not jpinterwoven");			
			iiadoa.aspectTestInstead=0;
			a.Test();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,iiadoa.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAndDeclaredOnlyAspect ariadoa=new AfterReturningIncludeAndDeclaredOnlyAspect();
			B1 b=(B1)Weaver.CreateInstance(typeof(B1),null,ariadoa);
			while(11!=b.Test11(11))
			{
				if(ariadoa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ariadoa.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ariadoa.aspectTestAfterReturning=0;
			while(11!=b.Test(11))
			{
				if(ariadoa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(0,ariadoa.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAndDeclaredOnlyAspect atiadoa=new AfterThrowingIncludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,atiadoa);
			a.bThrowException=true;
			try
			{
				a.Test11();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,atiadoa.aspectTestAfterThrowing,"Aspect not jpinterwoven");			
			}
			a.testCall=0;
			atiadoa.aspectTestAfterThrowing=0;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(0,atiadoa.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
		}
	}
	[TestClass]
	public class ExcludeAndIncludeIfAttributeDefined:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeExcludeAndIncludeIfAttributeDefinedAspect beaiiada=new BeforeExcludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,beaiiada);
			a.Test();
			Assert.AreEqual(0,beaiiada.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
			a.testCall=0;
			a.Test2();
			Assert.AreEqual(2,beaiiada.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterExcludeAndIncludeIfAttributeDefinedAspect aeaiiada=new AfterExcludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aeaiiada);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,aeaiiada.aspectTestAfter,"Aspect jpinterwoven");			
			a.testCall=0;
			a.Test2();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(3,aeaiiada.aspectTestAfter,"Aspect not jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadExcludeAndIncludeIfAttributeDefinedAspect ieaiiada=new InsteadExcludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ieaiiada);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,ieaiiada.aspectTestInstead,"Aspect jpinterwoven");			
			a.testCall=0;
			a.Test2();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(2,ieaiiada.aspectTestInstead,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningExcludeAndIncludeIfAttributeDefinedAspect areaiiada=new AfterReturningExcludeAndIncludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,areaiiada);
			while(11!=b.Test(11))
			{
				if(areaiiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,areaiiada.aspectTestAfterReturning,"Aspect jpinterwoven");
			b.testCall=0;
			while(11!=b.Test2(11))
			{
				if(areaiiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(2,b.testCall,"object not called");
			Assert.AreEqual(3,areaiiada.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}	
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingExcludeAndIncludeIfAttributeDefinedAspect ateaiiada=new AfterThrowingExcludeAndIncludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ateaiiada);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(0,ateaiiada.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
			a.testCall=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(2,a.testCall,"object called");
				Assert.AreEqual(3,ateaiiada.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
		}
	}
	[TestClass]
	public class ExcludeAndExcludeIfAttributeDefinedAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeExcludeAndExcludeIfAttributeDefinedAspect beaeiada=new BeforeExcludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,beaeiada);
			a.Test();
			Assert.AreEqual(0,beaeiada.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
			a.testCall=0;
			a.Test2();
			Assert.AreEqual(0,beaeiada.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterExcludeAndExcludeIfAttributeDefinedAspect aeaeiada=new AfterExcludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aeaeiada);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,aeaeiada.aspectTestAfter,"Aspect jpinterwoven");
			a.testCall=0;
			a.Test2();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,aeaeiada.aspectTestAfter,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void Instead()
		{
			InsteadExcludeAndExcludeIfAttributeDefinedAspect ieaeiada=new InsteadExcludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ieaeiada);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,ieaeiada.aspectTestInstead,"Aspect jpinterwoven");
			a.testCall=0;
			a.Test2();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,ieaeiada.aspectTestInstead,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningExcludeAndExcludeIfAttributeDefinedAspect areaeiada=new AfterReturningExcludeAndExcludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,areaeiada);
			while(11!=b.Test(11))
			{
				if(areaeiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,areaeiada.aspectTestAfterReturning,"Aspect jpinterwoven");
			b.testCall=0;
			while(11!=b.Test2(11))
			{
				if(areaeiada.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(2,b.testCall,"object not called");
			Assert.AreEqual(0,areaeiada.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingExcludeAndExcludeIfAttributeDefinedAspect ateaeiada=new AfterThrowingExcludeAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ateaeiada);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(0,ateaeiada.aspectTestAfterThrowing,"Aspect jpinterwoven");
			}
			a.testCall=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(2,a.testCall,"object not called");
				Assert.AreEqual(0,ateaeiada.aspectTestAfterThrowing,"Aspect jpinterwoven");
			}
		}
	}
	[TestClass]
	public class ExcludeAndDeclaredOnlyAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeExcludeAndDeclaredOnlyAspect beadoa=new BeforeExcludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,beadoa);
			a.Test11();
			Assert.AreEqual(0,beadoa.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
			a.testCall=0;
			a.Test();
			Assert.AreEqual(0,beadoa.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterExcludeAndDeclaredOnlyAspect aeadoa=new AfterExcludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,aeadoa);
			a.Test11();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(0,aeadoa.aspectTestAfter,"Aspect jpinterwoven");
			a.testCall=0;
			a.Test();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,aeadoa.aspectTestAfter,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void Instead()
		{
			InsteadExcludeAndDeclaredOnlyAspect ieadoa=new InsteadExcludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,ieadoa);
			a.Test11();
			Assert.AreEqual(0,ieadoa.aspectTestInstead,"Aspect jpinterwoven");
			Assert.AreEqual(1,a.testCall,"object not called");
			a.testCall=0;
			a.Test();
			Assert.AreEqual(0,ieadoa.aspectTestInstead,"Aspect jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningExcludeAndDeclaredOnlyAspect areadoa=new AfterReturningExcludeAndDeclaredOnlyAspect();
			B1 b=(B1)Weaver.CreateInstance(typeof(B1),null,areadoa);
			while(11!=b.Test11(11))
			{
				if(areadoa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(0,areadoa.aspectTestAfterReturning,"Aspect jpinterwoven");
			b.testCall=0;
			while(11!=b.Test(11))
			{
				if(areadoa.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(2,b.testCall,"object not called");
			Assert.AreEqual(0,areadoa.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingExcludeAndDeclaredOnlyAspect ateadoa=new AfterThrowingExcludeAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,ateadoa);
			a.bThrowException=true;
			try
			{
				a.Test11();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(0,ateadoa.aspectTestAfterThrowing,"Aspect jpinterwoven");
			}
			a.testCall=0;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(2,a.testCall,"object not called");
				Assert.AreEqual(0,ateadoa.aspectTestAfterThrowing,"Aspect jpinterwoven");
			}
		}
	}
	[TestClass]
	public class IncludeAllAndExcludeAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAllAndExcludeAspect biaaea=new BeforeIncludeAllAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,biaaea);
			a.Test();
			Assert.AreEqual(1,biaaea.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			biaaea.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(0,biaaea.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAllAndExcludeAspect aa=new AfterIncludeAllAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aa);
			a.Test();			
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aa.aspectTestAfter,"Aspect not jpinterwoven");
			a.testCall=0;
			aa.aspectTestAfter=0;
			a.Test2();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(0,aa.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAllAndExcludeAspect ia=new InsteadIncludeAllAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ia);
			a.Test();			
			Assert.AreEqual(0,a.testCall,"object not called");
			Assert.AreEqual(1,ia.aspectTestInstead,"Aspect not jpinterwoven");
			ia.aspectTestInstead=0;
			a.Test2();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,ia.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAllAndExcludeAspect ar=new AfterReturningIncludeAllAndExcludeAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ar);
			while(11!=b.Test(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ar.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ar.aspectTestAfterReturning=0;
			while(11!=b.Test(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(0,ar.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAllAndExcludeAspect at=new AfterThrowingIncludeAllAndExcludeAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,at);
			a.bThrowException=true;
			try
			{
				a.Test();			
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,at.aspectTestAfterThrowing,"Aspect not jpinterwoven");
			}
			a.testCall=0;
			at.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(0,at.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
		}
	}
	
	[TestClass]
	public class IncludeAllAndExcludeIfAttributeDefinedAttributes: InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAllAndExcludeIfAttributeDefinedAspect ba=new BeforeIncludeAllAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ba);
			a.Test();
			Assert.AreEqual(1,ba.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			ba.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(0,ba.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAllAndExcludeIfAttributeDefinedAspect aa=new AfterIncludeAllAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aa);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aa.aspectTestAfter,"Aspect not jpinterwoven");			
			a.testCall=0;
			aa.aspectTestAfter=0;
			a.Test2();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(0,aa.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAllAndExcludeIfAttributeDefinedAspect ia=new InsteadIncludeAllAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ia);
			a.Test();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,ia.aspectTestInstead,"Aspect not jpinterwoven");			
			ia.aspectTestInstead=0;
			a.Test2();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,ia.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAllAndExcludeIfAttributeDefinedAspect ar=new AfterReturningIncludeAllAndExcludeIfAttributeDefinedAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ar);
			while(11!=b.Test(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ar.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ar.aspectTestAfterReturning=0;
			while(11!=b.Test2(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(0,ar.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAllAndExcludeIfAttributeDefinedAspect at=new AfterThrowingIncludeAllAndExcludeIfAttributeDefinedAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,at);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,at.aspectTestAfterThrowing,"Aspect not jpinterwoven");			
			}
			a.testCall=0;
			at.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(0,at.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
		}
	}
	[TestClass]
	public class IncludeAllAndDeclaredOnlyAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeAllAndDeclaredOnlyAspect ba=new BeforeIncludeAllAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,ba);
			a.Test11();
			Assert.AreEqual(1,ba.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			ba.aspectTestBefore=0;
			a.Test();
			Assert.AreEqual(0,ba.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeAllAndDeclaredOnlyAspect aa=new AfterIncludeAllAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,aa);
			a.Test11();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aa.aspectTestAfter,"Aspect not jpinterwoven");			
			a.testCall=0;
			aa.aspectTestAfter=0;
			a.Test();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(0,aa.aspectTestAfter,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeAllAndDeclaredOnlyAspect ia=new InsteadIncludeAllAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,ia);
			a.Test11();
			Assert.AreEqual(0,a.testCall,"object called");
			Assert.AreEqual(1,ia.aspectTestInstead,"Aspect not jpinterwoven");			
			ia.aspectTestInstead=0;
			a.Test();
			Assert.AreEqual(2,a.testCall,"object not called");
			Assert.AreEqual(0,ia.aspectTestInstead,"Aspect jpinterwoven");			
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeAllAndDeclaredOnlyAspect ar=new AfterReturningIncludeAllAndDeclaredOnlyAspect();
			B1 b=(B1)Weaver.CreateInstance(typeof(B1),null,ar);
			while(11!=b.Test11(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ar.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ar.aspectTestAfterReturning=0;
			while(11!=b.Test(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(0,ar.aspectTestAfterReturning,"Aspect jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeAllAndDeclaredOnlyAspect at=new AfterThrowingIncludeAllAndDeclaredOnlyAspect();
			A1 a=(A1)Weaver.CreateInstance(typeof(A1),null,at);
			a.bThrowException=true;
			try
			{
				a.Test11();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,at.aspectTestAfterThrowing,"Aspect not jpinterwoven");			
			}
			a.testCall=0;
			at.aspectTestAfterThrowing=0;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(0,at.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
		}
	}
	[TestClass]
	public class IncludeIfAttributeDefinedAndDeclaredOnlyAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeIncludeIfAttributeDefinedAndDeclaredOnlyAspect ba=new BeforeIncludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ba);
            a.Test();
			Assert.AreEqual(1,ba.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			ba.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(3,ba.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(4,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterIncludeIfAttributeDefinedAndDeclaredOnlyAspect aa=new AfterIncludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aa);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aa.aspectTestAfter,"Aspect not jpinterwoven");			
			a.testCall=0;
			aa.aspectTestAfter=0;
			a.Test2();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(4,aa.aspectTestAfter,"Aspect not jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadIncludeIfAttributeDefinedAndDeclaredOnlyAspect ia=new InsteadIncludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ia);
			a.Test();
			Assert.AreEqual(1,ia.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.testCall,"object not called");
			ia.aspectTestInstead=0;
			a.Test2();
			Assert.AreEqual(2,ia.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.testCall,"object not called");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningIncludeIfAttributeDefinedAndDeclaredOnlyAspect ar=new AfterReturningIncludeIfAttributeDefinedAndDeclaredOnlyAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ar);
			while(11!=b.Test(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ar.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ar.aspectTestAfterReturning=0;
			while(11!=b.Test2(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(4,ar.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingIncludeIfAttributeDefinedAndDeclaredOnlyAspect at=new AfterThrowingIncludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,at);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,at.aspectTestAfterThrowing,"Aspect not jpinterwoven");			
			}
			a.testCall=0;
			at.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(4,at.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
		}
	}
	[TestClass]
	public class ExcludeIfAttributeDefinedAndDeclaredOnlyAttributes:InterweavingAttributeAspects
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Before()
		{
			BeforeExcludeIfAttributeDefinedAndDeclaredOnlyAspect ba=new BeforeExcludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ba);
			a.Test();
			Assert.AreEqual(1,ba.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
			a.testCall=0;
			ba.aspectTestBefore=0;
			a.Test2();
			Assert.AreEqual(0,ba.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(3,a.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterExcludeIfAttributeDefinedAndDeclaredOnlyAspect aa=new AfterExcludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,aa);
			a.Test();
			Assert.AreEqual(1,a.testCall,"object not called");
			Assert.AreEqual(2,aa.aspectTestAfter,"Aspect not jpinterwoven");			
			a.testCall=0;
			aa.aspectTestAfter=0;
			a.Test2();
			Assert.AreEqual(3,a.testCall,"object not called");
			Assert.AreEqual(0,aa.aspectTestAfter,"Aspect not jpinterwoven");			
		}
		[TestMethod]
		public void Instead()
		{
			InsteadExcludeIfAttributeDefinedAndDeclaredOnlyAspect ia=new InsteadExcludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ia);
			a.Test();
			Assert.AreEqual(1,ia.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.testCall,"object not called");
			ia.aspectTestInstead=0;
			a.Test2();
			Assert.AreEqual(0,ia.aspectTestInstead,"Aspect jpinterwoven");
			Assert.AreEqual(2,a.testCall,"object not called");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningExcludeIfAttributeDefinedAndDeclaredOnlyAspect ar=new AfterReturningExcludeIfAttributeDefinedAndDeclaredOnlyAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,ar);
			while(11!=b.Test(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(1,b.testCall,"object not called");
			Assert.AreEqual(2,ar.aspectTestAfterReturning,"Aspect not jpinterwoven");
			b.testCall=0;
			ar.aspectTestAfterReturning=0;
			while(11!=b.Test2(11))
			{
				if(ar.aspectTestAfterReturning!=0)
				{
					Assert.Fail("Aspect jpinterwoven before method returned");
					break;
				}
			}
			Assert.AreEqual(3,b.testCall,"object not called");
			Assert.AreEqual(0,ar.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingExcludeIfAttributeDefinedAndDeclaredOnlyAspect at=new AfterThrowingExcludeIfAttributeDefinedAndDeclaredOnlyAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,at);
			a.bThrowException=true;
			try
			{
				a.Test();
			}
			catch
			{
				Assert.AreEqual(1,a.testCall,"object not called");
				Assert.AreEqual(2,at.aspectTestAfterThrowing,"Aspect not jpinterwoven");			
			}
			a.testCall=0;
			at.aspectTestAfterThrowing=0;
			try
			{
				a.Test2();
			}
			catch
			{
				Assert.AreEqual(3,a.testCall,"object not called");
				Assert.AreEqual(0,at.aspectTestAfterThrowing,"Aspect jpinterwoven");			
			}
		}
	}
}