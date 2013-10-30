﻿// ****************************************************************************
// <copyright file="RelayCommandGeneric.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2013
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>22.4.2009</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.galasoft.ch</web>
// <license>
// See license.txt in this project or http://www.galasoft.ch/license_MIT.txt
// </license>
// ****************************************************************************
// <credits>This class was developed by Josh Smith (http://joshsmithonwpf.wordpress.com) and
// slightly modified with his permission.</credits>
// ****************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using GalaSoft.MvvmLight.Helpers;

#if NETFX_CORE || PORTABLE
using System.Reflection;
using GalaSoft.MvvmLight.Internal;

#endif

////using GalaSoft.Utilities.Attributes;

namespace GalaSoft.MvvmLight.Command
{
    /// <summary>
    /// A generic command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'. This class allows you to accept command parameters in the
    /// Execute and CanExecute callback methods.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    //// [ClassInfo(typeof(RelayCommand)]
    public class RelayCommand<T> : ICommand
    {
        private readonly WeakAction<T> _execute;

        private readonly WeakFunc<T, bool> _canExecute;
#if PORTABLE
        private static readonly ICommandManagerHelper _commandManager = PlatformAdapter.Resolve<ICommandManagerHelper>(false);
#endif
        private EventHandler _canExecuteChangedEvent;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that 
        /// can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = new WeakAction<T>(execute);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<T,bool>(canExecute);
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#else
#if NETFX_CORE
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#elif PORTABLE
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    if (_commandManager != null)
                        _commandManager.RequerySuggested += value;
                    else
                        _canExecuteChangedEvent += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    if (_commandManager != null)
                        _commandManager.RequerySuggested -= value;
                    else
                        _canExecuteChangedEvent -= value;
                }
            }
        }
#else
#if XAMARIN
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#else
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
#endif
#endif
#endif

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance", 
            "CA1822:MarkMembersAsStatic",
            Justification = "The this keyword is used in the Silverlight version")]
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged()
        {
#if SILVERLIGHT
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#else
#if NETFX_CORE
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif PORTABLE
            if (_commandManager != null)
            {
                _commandManager.InvalidateRequerySuggested();
            }
            else
            {
                var handler = _canExecuteChangedEvent;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
#else
#if XAMARIN
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#else
            CommandManager.InvalidateRequerySuggested();
#endif
#endif
#endif
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data 
        /// to be passed, this object can be set to a null reference</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            bool matches;
            var val = GetParameterForMethodFromValuePassedIn(parameter, out matches);
            
            if(matches)
                return CanExecute(val);

            return false;
        }


        private bool CanExecute(T parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            if (_canExecute.IsStatic || _canExecute.IsAlive)
            {
                return _canExecute.Execute(parameter);
            }

            return false;
        }

        private static T GetParameterForMethodFromValuePassedIn(object parameter, out bool typeMatches)
        {
            var val = parameter;

#if !NETFX_CORE

            var underlying = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (parameter != null
                && parameter.GetType() != typeof(T) && parameter.GetType() != underlying)
            {

                if (parameter is IConvertible)
                {
                    // internally, even the NetCore 4.5 version  will use IConvertible
                    val = Convert.ChangeType(parameter, underlying, null);
                }
            }
#endif
            
            if (val == null)
            {
                typeMatches = true;
                return default(T);
            }


            // check to see if it's of the same type
            typeMatches = val is T;

            if(typeMatches)
                return (T)val;

            // Don't throw, just return default
            return default(T); 
        }


        /// <summary>
        /// Defines the method to be called when the command is invoked. 
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data 
        /// to be passed, this object can be set to a null reference</param>
        public virtual void Execute(object parameter)
        {
            bool matches;
            var val = GetParameterForMethodFromValuePassedIn(parameter, out matches);
            
            if (!matches)
                throw new InvalidCastException("Cannot convert parameter to " + typeof(T));

            if (matches && CanExecute(val)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute(val);
            }
            
        }
    }
}