# Example
The simple example is a .Net Console Application. The application greets the user with its name.
At first, the application asks the user for his or her name and reads the name from the console. After this, the application instantiates a new object of type Base. As next, the Hello() method of the object is called with the read name. 

## The Application:
{{
 1  using System;
 2
 3  namespace Tutorial
 4  {
 5        public class Test
 6        {
 7              [STAThread](STAThread)
 8              static void Main(string[]() args)
 9              {
10                     string name;
11
12                     Console.Write("Your name: ");
13                     name = Console.ReadLine();
14                     Base b = new Base();
15                     b.Hello(name);
16                     Console.ReadLine();
17              }
18        }
19  }
}}

The Base class implements only a {{Hello()}} method, which displays a string with the read name on the console. If you do not call the Base class through an interface, you have to declare the target methods which should become interwoven as _virtual_. 

## The Base Class:
{{
 1  using System;
 2
 3  namespace Tutorial
 4  {
 5        public class Base
 6        {
 7               public virtual void Hello(string name)
 8               {
 9                      Console.WriteLine("Hello {0}!", name);
10               }
11        }
12  }
}}

The result is displayed on the console: 

{{ Your name: world }}
{{ Hello world! }}

# The Example Aspect
All aspect classes which become interwoven have to be derived from {{Loom.Aspect}} or {{Loom.AspectAttribute}}.

An aspect class contains aspect methods, in which the aspect code is implemented. An aspect attribute marks a method inside an aspect class as an aspect method. It defines how an aspect method will become interwoven with a target class method, using {{Loom.JoinPoints.Advice}}.

Additional pointcut attributes define, which method in the target class will become interwoven and which won't become interwoven with the aspect methods.

The aspect methods will become interwoven with a target method, if they fulfill the _Rules for Interweaving_. 

The example aspect implements tracing and is named {{TraceAspect}}. {{TraceAspect}} is derived from {{Loom.Aspect}}. 

The aspect method attribute {{Loom.JoinPoints.Call}} interweaves the aspect method in a call of a target class method. In TraceAspect the aspect method {{Trace()}} will become interwoven around the {{Base.Hello()}} method. Which means, that the target method can be called from inside the aspect. To do that, the aspect context must be declared explicitly in the aspect method parameter list using {{Loom.JoinPoints.JPContext}} attribute.

The pointcut attribute {{Loom.JoinPoints.IncludeAll}} defines, that all methods of the Base class including the {{Hello}} method will become interwoven. 

Inside the {{Trace()}} method the actual state of the program execution is displayed on the console. 

The interwoven target class method {{Hello()}} is called out of the aspect method {{Trace()}} via {{Invoke()}}. 

## The tracing aspect:
{{
 1  using System;
 2  using Loom;
 3  using Loom.JoinPoints;
 4
 5  namespace Tutorial
 6  {
 7        public class TraceAspect : Loom.Aspect
 8        {
 9               [Loom.JoinPoints.IncludeAll](Loom.JoinPoints.IncludeAll)
10               [Loom.Call(Advice.Around)](Loom.Call(Advice.Around))
11               public T Trace<T>([JPContext](JPContext)Context ctx, params object[]() args)
12               {
13                 Console.WriteLine("{0}.{1} called",ctx.TargetClass, ctx.CurrentMethod.Name);
14                 return (T)ctx.Invoke(args);
15               }
16        }
17  }
}}

# Interweaving of Aspect and Target Class
The dynamic aspect weaver Rapier-Loom.Net interweaves one or more aspect classes with a .Net target class on instantiation of the target class. Therefore the target class methods have to be either virtual or have to be defined via an interface. The method of the class Base is defined as virtual in this tutorial example.
For interweaving the TraceAspect have to be instantiated. 

{{ TraceAspect ta = new TraceAspect(); }}

Because Rapier-Loom.Net interweaves aspect and target class at instantiation of the target class, the target class need to be instantiated via the {{Loom.Weaver.Create<T>()}} factory method instead of the new-operator. You can additionally pass possible constructor parameters and aspect objects. 

{{ Base b = Loom.Weaver.Create<Base>(ta); }}

After this, the method which will becomes interwoven, have to be called. 

## The application extended with the aspect:
{{
 1  using System;
 2  using Loom;
 3  using Loom.JoinPoints;
 4
 5  namespace Tutorial
 6  {
 7       public class Test
 8       {
 9               [STAThread](STAThread)
10               static void Main(string[]() args)
11               {
12                      Console.Write("Your name: ");
13                      name = Console.ReadLine();
14                      TraceAspect ta = new TraceAspect();
15                      Base b = Loom.Weaver.Create<Base>(ta);
16                      B.Hello(name);
17                      Console.ReadLine();
18                }
19        }
20  }
}}

The result is the following: 

{{ Your name: world }}
{{ Base.Hello called}}
{{ Hello world! }}

## Using Aspects as Attributes
Alternatively you can use your aspect also as class or method attribute. It will then affect the annotated class and the annotated method respectively. Change your aspect declaration and derive it from {{Loom.AspectAttribute}}. You will not have to create your aspect explicitly then. It will be created when you create the instance of the {{Base}} class with the {{Create}} function:

{{
public class TraceAspect : Loom.AspectAttribute
{
...
}

[TraceAspect](TraceAspect)
public class Base
{
...
}

...

Base b = Loom.Weaver.Create<Base>();

}}

