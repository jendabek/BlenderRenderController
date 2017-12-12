using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController
{
    public static class Extentions
    {
        public static void InvokeAction(this Control @this, Action action)
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action);
            }
            else
            {
                action();
            }
        }
        public static void InvokeAction<T>(this Control @this, Action<T> action, T param)
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, param);
            }
            else
            {
                action(param);
            }

        }
        public static void InvokeAction<TArgs>(this Control @this, Action<object, TArgs> action, object obj, TArgs e)
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, obj, e);
            }
            else
            {
                action(obj, e);
            }
        }


        /// <summary>
        /// Safely raises any EventHandler event asynchronously.
        /// </summary>
        /// <param name="sender">The object raising the event (usually this).</param>
        /// <param name="args">The TArgs for this event.</param>
        public static void Raise<TArgs>(this MulticastDelegate thisEvent, object sender, TArgs args)
        {
            var localMCD = thisEvent;
            AsyncCallback callback = ar => ((EventHandler<TArgs>)ar.AsyncState).EndInvoke(ar);

            foreach (Delegate d in localMCD.GetInvocationList())
            {
                if (d is EventHandler<TArgs> uiMethod)
                {
                    if (d.Target is ISynchronizeInvoke target)
                    {
                        target.BeginInvoke(uiMethod, new object[] { sender, args });
                    }
                    else
                    {
                        uiMethod.BeginInvoke(sender, args, callback, uiMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Safely raises any EventHandler event asynchronously.
        /// </summary>
        /// <param name="sender">The object raising the event (usually this).</param>
        /// <param name="args">The EventArgs for this event.</param>
        public static void Raise(this MulticastDelegate thisEvent, object sender, EventArgs args)
        {
            var localMCD = thisEvent;
            AsyncCallback callback = ar => ((EventHandler)ar.AsyncState).EndInvoke(ar);

            foreach (Delegate d in localMCD.GetInvocationList())
            {
                if (d is EventHandler uiMethod)
                {
                    if (d.Target is ISynchronizeInvoke target)
                    {
                        target.BeginInvoke(uiMethod, new object[] { sender, args });
                    }
                    else
                    {
                        uiMethod.BeginInvoke(sender, args, callback, uiMethod);
                    }
                }
            }
        }

    }



}
