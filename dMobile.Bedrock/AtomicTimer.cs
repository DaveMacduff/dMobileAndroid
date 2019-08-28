namespace dMobile.Bedrock
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class used to time code execution duration.
    /// </summary>
    /// <example>
    /// AtomicTimer tmr = New clsAtomicTimer()
    /// tmr.Start()
    /// long dur = tmr.Stop()
    /// </example>
    public class AtomicTimer
    {
        /// <summary>
        /// Contains frequency interval for time check.
        /// </summary>
        private long frequencyInterval = 0;

        /// <summary>
        /// Stores the starting log time.
        /// </summary>
        private long startTime = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicTimer" /> class.
        /// </summary>
        /// <remarks></remarks>
        public AtomicTimer()
        {
            if (QueryPerformanceFrequency(ref frequencyInterval) == 0)
            {
                throw new ApplicationException();
            }

            // Convert to ms.
            this.FrequencyInterval = Convert.ToInt64(this.FrequencyInterval / 1000);
        }

        /// <summary>
        /// Gets or sets the frequency interval for time check.
        /// </summary>
        private long FrequencyInterval
        {
            get { return this.frequencyInterval; }
            set { this.frequencyInterval = value; }
        }

        /// <summary>
        /// Gets or sets the starting log time.
        /// </summary>
        private long StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="perfCounter"></param>
        /// <returns></returns>
        [DllImport("coredll.dll", EntryPoint = "QueryPerformanceCounter")]
        public static extern int QueryPerformanceCounter(ref long perfCounter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        [DllImport("coredll.dll", EntryPoint = "QueryPerformanceFrequency")]
        public static extern int QueryPerformanceFrequency(ref long frequency);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public void Start()
        {
            if (QueryPerformanceCounter(ref startTime) == 0)
            {
                throw new ApplicationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public long Stop()
        {
            long stopTime = 0;
            if (QueryPerformanceCounter(ref stopTime) == 0)
            {
                throw new ApplicationException();
            }

            return Convert.ToInt64((stopTime - this.StartTime) / this.FrequencyInterval);
        }
    }
}
