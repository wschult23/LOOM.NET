// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Loom.DependecyInjection;
using Loom;

// *************************************************************************************
// This example shows, how dependency injection can easily be implemented with LOOM.NET
//
// *************************************************************************************

// The declaration of the service interfaces


    public interface IEngine
    {
    }

    public interface IMonocoque
    {
    }

    public interface IGearBox
    {
    }

    public interface IPowerSupply
    {
    }

    // The definition of the service implementations
    public class LithiumIonBattery : IPowerSupply
    {
    }

    public class ElectricMotor : IEngine, IDisposable
    {
        /// <summary>
        /// automatically inject the dependency to IPowerSupply
        /// </summary>
        [Inject]
        public virtual IPowerSupply PowerSupply { get; set; }

        public override string ToString()
        {
            return string.Format("{0} powered by {1}", base.ToString(), PowerSupply);
        }


        public void Dispose()
        {
            Console.WriteLine("Engine destroyed");
        }
    }

    public class CarbonFiberMonocoque : IMonocoque
    {
    }

    public class AutomaticGearbox : IGearBox
    {
    }

    public class Car : IDisposable
    {
        private IEngine engine;

        /// <summary>
        /// automatically inject the dependency to IEngine and run initialization code.
        /// </summary>
        [Inject(AutoDispose = false)]
        public virtual void SetEngine(IEngine engine)
        {
            this.engine = engine;
            Console.WriteLine("The engine is plugged in.");
        }

        /// <summary>
        /// automatically inject the dependency to IGearBox
        /// </summary>
        [Inject]
        public virtual IGearBox Gearbox { get; set; }

        /// <summary>
        /// automatically inject the dependency to IMonocoque
        /// </summary>
        [Inject]
        public virtual IMonocoque Monocoque { get; set; }

        public Car()
        {
            Console.WriteLine("The car is created. Checking components:");
            Console.WriteLine("Engine available: {0}", engine != null);
            Console.WriteLine("Gearbox available: {0}", Gearbox != null);
            Console.WriteLine("Monocoque available: {0}", Monocoque != null);
        }

        public override string ToString()
        {
            return string.Format("I'm an {0} with a {1} and a {2} that has a {3}", base.ToString(), engine, Gearbox, Monocoque);
        }

        public void Dispose()
        {
            Console.WriteLine("Car destroyed.");
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // Register the implementations to the interfaces
            Inject.RegisterClass<IEngine, ElectricMotor>();
            Inject.RegisterClass<IMonocoque, CarbonFiberMonocoque>();
            Inject.RegisterClass<IGearBox, AutomaticGearbox>();
            Inject.RegisterClass<IPowerSupply, LithiumIonBattery>();

            // Create the dependent object
            using (var car = Weaver.Create<Car>())
            {
                Console.WriteLine(car);
            }

            Console.ReadKey();
        }
    }
