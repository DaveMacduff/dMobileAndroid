// <copyright file="SysTimer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>
// <remarks>
// How to use:
// See ActivityHdr, ActivityItem, ActivityDetail forms for examples.
// In a form constructor place:
//  private SysTimer Timer { get; set; }
//  this.Timer = new SysTimer(this.TimerTickCallback, 1000);
//
// Create a call back method:
//  private void TimerTickCallback()
//  Stop the timer in its call back method.
//  this.Timer.Stop();
//
// Start/Stop the timer for example in a Focused / Unfocused event method.
//  this.Timer.Start();
//  this.Timer.Stop();
//
// In a keypress event method, call the start method to invoke the call back method when appropriate.
//  Timer was started in the focused event handler.
//  Timer was stopped in the unfocused event handler.
//  Timer was stopped in the timer tick call back event.
//  Call timer start when the text changes to invoke the call back method passed to the timer when appropriate.
//  If the timer is already running then the call back will be invoked and the timer start ignored.
// </remarks>

namespace dMobile.Bedrock
{
    using System;
    using Xamarin.Forms;

    /// <summary>
    ///     System timer class.
    /// </summary>
    public sealed class SysTimer
    {
        /// <summary>
        ///    Flag indicating whether the timer is enabled. Toggle the flag to start/stop the timer.
        /// </summary>
        private bool isEnabled;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SysTimer"/> class.
        /// </summary>
        /// <param name="callback"> Callback action.</param>
        /// <param name="millisecondsInterval"> Milliseconds interval.</param>
        public SysTimer(Action callback, int millisecondsInterval = 1000)
        {
            this.Callback = callback;
            this.MillisecondsInterval = millisecondsInterval;
            this.IsEnabled = false;
            this.IsRunning = false;
        }

        /// <summary>
        ///    Gets a value indicating whether the timer is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///    Gets or sets a value indicating whether the timer is enabled.
        /// </summary>
        /// <remarks>Assigning a value to this property will start/stop the timer.</remarks>
        public bool IsEnabled
        {
            get => this.isEnabled;

            set
            {
                this.isEnabled = value;
                if (this.isEnabled)
                {
                    if (!this.IsRunning)
                    {
                        this.Start();
                    }
                }
                else
                {
                    if (this.IsRunning)
                    {
                        this.Stop();
                    }
                }
            }
        }

        /// <summary>
        ///    Gets the callback.
        /// </summary>
        public Action Callback { get; }

        /// <summary>
        ///   Gets the milliseconds period.
        /// </summary>
        public int MillisecondsInterval { get; }

        /// <summary>
        ///    Start the timer.
        /// </summary>
        public void Start()
        {
            // Do not use the IsEnabled property here, it will cause a stack overflow. Rather use the isEnabled field.
            this.isEnabled = true;
            if (this.IsRunning)
            {
                // Timer is already running, invoke the callback.
                // This will happen if the timer is enabled while it is running.
                this.Callback.Invoke();
            }
            else
            {
                // Start the timer if it is not running.
                Device.StartTimer(TimeSpan.FromMilliseconds(this.MillisecondsInterval), () =>
                {
                    this.IsRunning = this.isEnabled;

                    // Check if the timer is still running, Stop() will set it to false.
                    if (this.IsRunning)
                    {
                        this.Callback.Invoke();
                    }

                    // Return true to repeat counting, false to stop timer.
                    return this.isEnabled;
                });
            }
        }

        /// <summary>
        ///   Stop the timer.
        /// </summary>
        public void Stop()
        {
            // Do not use the IsEnabled property here, it will cause a stack overflow. Rather use the isEnabled field.
            this.isEnabled = false;
            this.IsRunning = false;
        }
    }
}
