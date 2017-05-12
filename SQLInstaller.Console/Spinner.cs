//-----------------------------------------------------------------------
// <copyright file="Spinner.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Console
{
	using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
	using System.Timers;

	/// <summary>
	/// Spinning/flaming logo.
	/// </summary>
	internal sealed class Spinner : IDisposable
    {
        /// <summary>
        /// A value indicating whether the object has been disposed.
        /// </summary>
		private bool isDisposed;

        /// <summary>
        /// The spin counter.
        /// </summary>
		private int counter;

        /// <summary>
        /// The spin timer.
        /// </summary>
		private Timer timer;

        /// <summary>
        /// The spin text sequence.
        /// </summary>
		private string[] frame = { "|", "/", "-", "\\" };

        /// <summary>
        /// Initializes a new instance of the Spinner class.
        /// </summary>
		public Spinner()
		{
            this.timer = new Timer(Constants.MinSpinTimeout);
            this.timer.Elapsed += new ElapsedEventHandler(this.Timer_Elapsed);
            this.timer.Enabled = false;

            // Enable spinning only when run directly from a terminal window.
            Process proc = this.GetParentProcess();

            this.timer.AutoReset = proc != null
                && (string.Compare(proc.ProcessName, "cmd", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(proc.ProcessName, "powershell", StringComparison.OrdinalIgnoreCase) == 0);
		}

        /// <summary>
        /// Method to begin spinning.
        /// </summary>
        /// <param name="timeout">How often to refresh.</param>
		public void Start(double timeout)
		{
			if (this.timer.AutoReset && timeout >= Constants.MinSpinTimeout)
			{
                this.timer.Interval = timeout;
                this.timer.Enabled = true;
			}
		}

        /// <summary>
        /// Method to stop spinning.
        /// </summary>
		public void Stop()
		{
            this.timer.Enabled = false;
		}

		#region IDisposable Members

        /// <summary>
        /// Method to dispose of the object.
        /// </summary>
		public void Dispose()
		{
            if (!this.isDisposed)
			{
                this.timer.Dispose();
				GC.SuppressFinalize(this);
                this.isDisposed = true;
			}
		}

		#endregion

        /// <summary>
        /// Method for timer callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The elapsed event arguments.</param>
		public void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
            this.counter++;
            Console.Write(this.frame[this.counter % this.frame.Length]);
            Console.SetCursorPosition(Console.CursorLeft - this.frame[this.counter % this.frame.Length].Length, Console.CursorTop);
		}

        #region Generated code for P/Invoke

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessInfo processInformation, int processInformationLength, out int returnLength);

        [StructLayout(LayoutKind.Sequential)]
        internal struct ProcessInfo
        {
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved20;
            internal IntPtr Reserved21;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
        }

        /// <summary>
        /// Gets the parent process of the current process.
        /// </summary>
        /// <returns>An instance of the Process class.</returns>
        private Process GetParentProcess()
        {
            Process parent = null;

            try
            {
                Process current = Process.GetCurrentProcess();
                while (current != null && string.Compare(current.ProcessName, "explorer", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    parent = current;
                    current = this.GetParentProcess(parent.Handle);
                }
            }
            catch
            {
            }

            return parent;
        }

        /// <summary>
        /// Gets the parent process of a specified process.
        /// </summary>
        /// <param name="handle">The process handle.</param>
        /// <returns>An instance of the Process class.</returns>
        private Process GetParentProcess(IntPtr handle)
        {
            Process parent = null;
            ProcessInfo pi = new ProcessInfo();
            int returnLength;

            try
            {
                int status = NtQueryInformationProcess(handle, 0, ref pi, Marshal.SizeOf(pi), out returnLength);
                if (status == 0)
                {
                    parent = Process.GetProcessById(pi.InheritedFromUniqueProcessId.ToInt32());
                }
            }
            catch
            {
            }

            return parent;
        }

        #endregion
    }
}