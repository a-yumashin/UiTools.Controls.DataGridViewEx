using System;
using System.Reflection;
using static UiTools.Controls.ExtendedDataGridView.CommonStuff;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal static class DynamicEventHelper
    {
        /// <summary>
        /// Adds an event handler to a dynamic object.
        /// </summary>
        /// <param name="extObj">Object owning the event to be processed.</param>
        /// <param name="eventName">Event name.</param>
        /// <param name="delegateTypeName">Full name of the event delegate type (e.g. "Namespace.Type").</param>
        /// <param name="handlerObject">Object owning the event handler method.</param>
        /// <param name="handlerMethod">Event handler method.</param>
        public static void AddDynamicEventHandler(dynamic extObj, string eventName, string delegateTypeName,
            object handlerObject, MethodInfo handlerMethod)
        {
            try
            {
                // Load delegate type by its name
                Type delegateType = Type.GetType(delegateTypeName);
                if (delegateType == null)
                    throw new ArgumentException(string.Format("{0}: {1}", SR("Delegate type not found"), delegateTypeName));

                // Create delegate of the resolved type
                Delegate handler = Delegate.CreateDelegate(delegateType, handlerObject, handlerMethod);

                // Add handler to the event
                extObj.GetType().GetEvent(eventName)?.AddEventHandler(extObj, handler);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}: {1}", SR("Failed to add dynamic event handler"), ex.Message));
            }
        }
    }
}
