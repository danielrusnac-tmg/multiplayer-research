using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMG.Survival.Infrastructure.PubSub
{
    /// <summary>
    /// This class implements Publish Subscribe design pattern which allows loose coupling between the applications components.<para></para>
    /// Here senders of messages, called publishers, do not program the messages to be sent directly to specific receivers, called subscribers.<para></para>
    /// Messages are published without the knowledge of what or if any subscriber of that knowledge exists. 
    /// </summary>
    public class PubSubService : IPubSubService
    {
        private readonly Dictionary<Type, Delegate> _parameterizedSubscribers = new();
        private readonly Dictionary<Type, Action> _nonParameterizedSubscribers = new();

        private readonly Dictionary<Type, int> _events = new();
        private readonly ICoroutineRunner _coroutineRunner;

        /// <summary>
        /// This delegate is used to create parameterized subscribers
        /// </summary>
        /// <param name="data">Data to be shared</param>
        /// <typeparam name="T">Event type</typeparam>
        public delegate void ParameterizedAction<in T>(T data);

        public PubSubService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        /// <summary>
        /// Registers a listener parameterless action for the given event type
        /// </summary>
        /// <param name="action">Action to register</param>
        /// <typeparam name="T">Event type</typeparam>
        public void RegisterListener<T>(Action action)
        {
            Type type = typeof(T);

            if (!_nonParameterizedSubscribers.ContainsKey(type))
            {
                _nonParameterizedSubscribers.Add(type, action);
            }
            else
            {
                _nonParameterizedSubscribers[type] += action;
            }
        }

        /// <summary>
        /// Registers a listener parameterized action for the given event type
        /// </summary>
        /// <param name="action">Action to register</param>
        /// <typeparam name="T">Event type</typeparam>
        public void RegisterListener<T>(ParameterizedAction<T> action)
        {
            Type type = typeof(T);

            if (_parameterizedSubscribers.TryGetValue(type, out var currentDelegate))
            {
                _parameterizedSubscribers[type] = Delegate.Combine(currentDelegate, action);
            }
            else
            {
                _parameterizedSubscribers.Add(type, action);
            }
        }

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
        public Coroutine RegisterCoroutineListener<T>()
        {
            if (!_events.ContainsKey(typeof(T)))
            {
                _events.Add(typeof(T), 1);
            }
            else
            {
                _events[typeof(T)]++;
            }

            return _coroutineRunner.StartCoroutine(WaitForEventCoroutine<T>());
        }

        /// <summary>
        /// Unregisters the listener parameterless action for the given event type
        /// </summary>
        /// <param name="action">Action to unregister</param>
        /// <typeparam name="T">Event type</typeparam>
        public void UnregisterListener<T>(Action action)
        {
            Type type = typeof(T);

            if (_nonParameterizedSubscribers.ContainsKey(type))
            {
                _nonParameterizedSubscribers[type] -= action;

                if (_nonParameterizedSubscribers[type] == null)
                {
                    _nonParameterizedSubscribers.Remove(type);
                }
            }
        }

        /// <summary>
        /// Unregisters the listener parameterized action for the given event type
        /// </summary>
        /// <param name="action">Action to unregister</param>
        /// <typeparam name="T">Event type</typeparam>
        public void UnregisterListener<T>(ParameterizedAction<T> action)
        {
            Type type = typeof(T);

            if (_parameterizedSubscribers.TryGetValue(type, out var currentDelegate))
            {
                _parameterizedSubscribers[type] = Delegate.Remove(currentDelegate, action);

                if (_parameterizedSubscribers[type] == null)
                {
                    _parameterizedSubscribers.Remove(type);
                }
            }
        }

        /// <summary>
        /// Publishes the specified event type
        /// </summary>
        /// <param name="eventToPublish">The event itself with possibility to add shareable data in it</param>
        /// <typeparam name="T">Event type</typeparam>
        public void Publish<T>(in T eventToPublish)
        {
            Type type = typeof(T);

            if (_parameterizedSubscribers.ContainsKey(type))
            {
                ((ParameterizedAction<T>)_parameterizedSubscribers[type])(eventToPublish);
            }

            if (_nonParameterizedSubscribers.ContainsKey(type))
            {
                _nonParameterizedSubscribers[type]();
            }
        }

        /// <summary>
        /// Publishes coroutine event type
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        public void PublishCoroutineEvent<T>()
        {
            if (!_events.ContainsKey(typeof(T)))
            {
                return;
            }

            _events[typeof(T)] = 0;
        }
        
        private IEnumerator WaitForEventCoroutine<T>()
        {
            while (!_events.ContainsKey(typeof(T)) || _events[typeof(T)] > 0)
            {
                yield return null;
            }
        }
    }
}