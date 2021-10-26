// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Loom.AspectProperties
{
    /// <summary>
    /// Defines the object creation behavior of the <see cref="O:Loom.Weaver.Create"/> and <see cref="O:Loom.Weaver.CreateInstance"/> method.
    /// </summary>
    /// <seealso cref="CreateAspectAttribute"/>
    public enum Per
    {
        /// <summary>
        /// There is only one instance of the aspect that will be applied to all instances of interwoven target classes (The aspect is a singleton). 
        /// </summary>
        AspectClass,
        /// <summary>
        /// Each interwoven target class will have its own instance of an aspect that will be applied when you create an instance of the target class.
        /// </summary>
        Class,
        /// <summary>
        /// Each instance of a target class will get one instance of each aspect that is interwoven with the target class, no matter how many annotations of the aspect are covering the target class.
        /// </summary>
        Instance,
        /// <summary>
        /// Each annotation creates exactly one instance of the annotating aspect that will be applied to each instance of covered target classes.
        /// </summary>
        Annotation,
        /// <summary>
        /// Each annotation creates exactly one instance of the annotating aspect per target class that will be applied to each instance of covered target classes.
        /// </summary>
        ClassAndAnnotation,
        /// <summary>
        ///  Each annotation creates a new instance of the annotating aspect whenever you create an instance of the target class. This is the default behavior.
        /// </summary>
        InstanceAndAnnotation
    }

    /// <summary>
    /// Use this attribute together with a <see cref="Per"/> enumeration to control the instance creation of your aspects.
    /// </summary>
    /// <remarks>
    /// <para>You can only annotate aspects derived from <see cref="AspectAttribute"/> with this attribute.</para>
    /// <para>
    /// <b>Rapier-LOOM.NET</b>: This attribute has no effect on dynamically interwoven aspects (aspects passed as parameter in <see cref="O:Weaver.Create"/> or 
    /// <see cref="O:Weaver.CreateInstance"/>.
    /// </para>
    /// <para>
    /// The usage of <see cref="Loom.AspectProperties.Per.AspectClass"/>, <see cref="Loom.AspectProperties.Per.Class"/>
    /// and <see cref="Loom.AspectProperties.Per.Instance"/> requires a parameterless aspect annotation. This means that
    /// you couldn't pass constructor parameters or named parameters to the aspect. 
    /// </para>
    /// </remarks>
    /// <seealso cref="Loom.AspectAttribute" />
    /// <example>
    /// This example shows how to use the CreateAspect attribute to control the instance creation of your aspects:
    /**
    <code>
        // use this attribute to indicate, that the aspect class in created only once
        [CreateAspect(Per.Class)] 
        public class Singleton : AspectAttribute
        {
            private object obj = null;

            [Create(Advice.Around)]
            [IncludeAll]
            public T foo&lt;T&gt;([JPContext] Context ctx, params object[] args)
            {
                if (obj == null) obj = ctx.Invoke(args);
                return (T)obj;
            }
        }

        ...
     
        [Singleton]     // applies the singleton methodnamepattern
        public class A
        {
            public A()
            {
                Console.WriteLine("Constructor A.");
            }
        }

        public class B
        {
            public B()
            {
                Console.WriteLine("Constructor B.");
            }
        }
     
        ...
        
        A a1 = Weaver.Create&lt;A&gt;();
        A a2 = Weaver.Create&lt;A&gt;();
        Console.WriteLine(a1==a2);

        B b1 = Weaver.Create&lt;B&gt;(); 
        B b2 = Weaver.Create&lt;B&gt;();
        Console.WriteLine(b1==b2);
     
        ...
        
        [Output]
        Constructor A.
        true
        Constructor B.
        Constructor B.
        false
 
    </code>*/
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreateAspectAttribute : Attribute
    {
        private Per type;

        internal Per creationType
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Controls the instance creation of your aspects.
        /// </summary>
        /// <param name="creationtype">specifies the type of the aspect creation</param>
        public CreateAspectAttribute(Per creationtype)
        {
            this.type = creationtype;
        }
    }
}
