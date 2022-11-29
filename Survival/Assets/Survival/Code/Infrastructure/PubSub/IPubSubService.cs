using System;
using UnityEngine;

namespace TMG.Survival.Infrastructure.PubSub
{
    public interface IPubSubService
    {
        /// <summary>
        /// Registers a listener parameterless action for the given event type
        /// </summary>
        /// <param name="action">Action to register</param>
        /// <typeparam name="T">Event type</typeparam>
        void RegisterListener<T>(Action action);

        /// <summary>
        /// Registers a listener parameterized action for the given event type
        /// </summary>
        /// <param name="action">Action to register</param>
        /// <typeparam name="T">Event type</typeparam>
        void RegisterListener<T>(PubSubService.ParameterizedAction<T> action);

        /// <summary>
        /// Registers a coroutine event listener
        /// </summary>
        /// <remarks>
        /// Coroutine events should be only empty structs
        /// </remarks>
        /// <example>
        /// It can be used to yield a coroutine until the specified event type is triggered
        /// <code>
        /// private IEnumerator ExampleCoroutine()
        /// {
        /// 	// Some logic
        ///
        /// 	....
        ///
        /// 	// Wait until the event is triggered
        /// 	yield PubSubManager.RegisterCoroutineListener&lt;ExampleEvent&gt;();
        ///
        /// 	....
        /// 
        /// 	// Some logic
        /// }
        /// </code>
        /// </example>
        /// <typeparam name="T">Event type</typeparam>
        /// <returns>Returns the coroutine itself</returns>
        Coroutine RegisterCoroutineListener<T>();

        /// <summary>
        /// Unregisters the listener parameterless action for the given event type
        /// </summary>
        /// <param name="action">Action to unregister</param>
        /// <typeparam name="T">Event type</typeparam>
        void UnregisterListener<T>(Action action);

        /// <summary>
        /// Unregisters the listener parameterized action for the given event type
        /// </summary>
        /// <param name="action">Action to unregister</param>
        /// <typeparam name="T">Event type</typeparam>
        void UnregisterListener<T>(PubSubService.ParameterizedAction<T> action);

        /// <summary>
        /// Publishes the specified event type
        /// </summary>
        /// <param name="eventToPublish">The event itself with possibility to add shareable data in it</param>
        /// <typeparam name="T">Event type</typeparam>
        void Publish<T>(in T eventToPublish);

        /// <summary>
        /// Publishes coroutine event type
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        void PublishCoroutineEvent<T>();
    }
}