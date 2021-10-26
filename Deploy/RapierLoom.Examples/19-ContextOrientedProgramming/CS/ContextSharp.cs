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

/// This example shows, how the Context Oriented Programming paradigm can be implemented with LOOM.NET
/// To learn more about COP, please visit http://www.swa.hpi.uni-potsdam.de/cop/
namespace Loom.ContextSharp
{
    /// <summary>
    /// Internal base class
    /// </summary>
    public class LayerPtr
    {
        internal LayerPtr()
        {
        }

        internal Layer nextlayer;
    }

    /// <summary>
    /// This ist the base class for all custom layers
    /// </summary>
    public abstract class Layer: LayerPtr
    {
        internal Stack<Context> contextstack = new Stack<Context>();

        protected Layer():base()
        {
        }

        public object Proceed()
        {
            Context context = contextstack.Peek();
            return Layered.InvokeMethod<object>(nextlayer, context);
        }
    }

    /// <summary>
    /// The class to activate a layer
    /// </summary>
    /// <typeparam name="LAYER"></typeparam>
    public class With<LAYER> : IDisposable where LAYER : Layer, new()
    {
        protected LayerPtr predecessor;
        protected LAYER layer;

        /// <summary>
        /// Activates the given layer
        /// </summary>
        public static IDisposable Do
        {
            get
            {
                return new With<LAYER>();
            }
        }

        private With()
        {
            LayerPtr currentlayerptr=Layered.currentlayer;

            /// check if the layer is already active
            while (currentlayerptr.nextlayer != null)
            {
                if (currentlayerptr.nextlayer.GetType() == typeof(LAYER))
                {
                    predecessor = currentlayerptr;
                    predecessor.nextlayer = predecessor.nextlayer.nextlayer;
                    break;
                }
                currentlayerptr = currentlayerptr.nextlayer;
            }

            /// select or create the new layer
            if (predecessor == null)
            {
                layer = new LAYER();
            }
            else
            {
                layer = (LAYER)predecessor.nextlayer;
            }
            // activate the layer
            layer.nextlayer = Layered.currentlayer.nextlayer;
            Layered.currentlayer.nextlayer = layer;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // remove the layer from the first position
            Layered.currentlayer.nextlayer = layer.nextlayer;
            // move it back to the old position
            if (predecessor != null)
            {
                layer.nextlayer = predecessor.nextlayer;
                predecessor.nextlayer = layer;
            }
            else
            {
                IDisposable disp = layer as IDisposable;
                if (disp != null) disp.Dispose();
            }
        }

        #endregion
    }

    /// <summary>
    /// The class to deactivate a layer
    /// </summary>
    /// <typeparam name="LAYER"></typeparam>
    public class Without<LAYER>:IDisposable where LAYER : Layer
    {
        protected LayerPtr predecessor;
        protected LAYER layer;

        /// <summary>
        /// Deactivates the given layer
        /// </summary>
        public static IDisposable Do
        {
            get
            {
                return new Without<LAYER>();
            }
        }

        private Without()
        {
            // find the layer and remove it from the "active" list
            for (LayerPtr layerptr = Layered.currentlayer; layerptr.nextlayer != null; layerptr = layerptr.nextlayer)
            {
                if (layerptr.nextlayer.GetType() == typeof(LAYER))
                {
                    predecessor = layerptr;
                    layer = (LAYER)layerptr.nextlayer;
                    layerptr.nextlayer = layerptr.nextlayer.nextlayer;
                    break;
                }
            }
        }


        #region IDisposable Members

        // activate it again
        void IDisposable.Dispose()
        {
            if (predecessor != null)
            {
                predecessor.nextlayer = layer;
            }
        }

        #endregion
    }

   
    /// <summary>
    /// The aspect to declare a layered class
    /// </summary>
    public class Layered : AspectAttribute
    {
        [ThreadStatic]
        internal static LayerPtr currentlayer=new LayerPtr();

        /// <summary>
        /// The method dispatch advice to control the call graph
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [IncludeAll]
        [Call(Advice.Around)]
        public T ContextDispatch<T>([JPContext] Context ctx, params object[] args)
        {
            // Tag the context, will will later need the arguments if the control flow comes back with a Layer.Proceed() call
            ctx.Tag = args;
            return InvokeMethod<T>(currentlayer.nextlayer, ctx);
        }

        /// <summary>
        /// Internal helper to Invoke the layer implementation methods
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="layer"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        internal static T InvokeMethod<T>(Layer layer, Context ctx)
        {
            // manipulate the arguments, so that the layer comes first
            object[] args = (object[])ctx.Tag;
            object[] newargs = new object[args.Length + 1];
            args.CopyTo(newargs, 1);

            // go through the list of active layers and try to invoke a implementation for these layers
            for (; ; )
            {
                // if there are no more layers, invoke the base implementation
                if (layer == null)
                {
                    return (T)ctx.Invoke(args);
                }

                // Push the Context to the internal stack, we need it for Layer.Proceed()
                layer.contextstack.Push(ctx);
                try
                {
                    // set the current layer to the first argument and call the implementation
                    newargs[0] = layer;
                    try
                    {
                        return (T)ctx.ReCall(newargs);
                    }
                    catch (MissingTargetMethodException) 
                    { 
                        // if ther is no implementation, we ignore this
                    }
                }
                finally
                {
                    layer.contextstack.Pop();
                }
                layer = layer.nextlayer;
            }
        }
    }

}
