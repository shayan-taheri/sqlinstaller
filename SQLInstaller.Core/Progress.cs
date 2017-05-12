//-----------------------------------------------------------------------
// <copyright file="Progress.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
	/// <summary>
	/// Progress class.
	/// </summary>
	public sealed class Progress
	{
        /// <summary>
        /// The status type of the message.
        /// </summary>
		private StatusMessage status;

        /// <summary>
        /// The progress message.
        /// </summary>
		private string message;

        /// <summary>
        /// A percent complete.
        /// </summary>
		private int percent;

        /// <summary>
        /// Initializes a new instance of the Progress class.
        /// </summary>
        /// <param name="status">The status type.</param>
		public Progress(StatusMessage status)
		{
			this.status = status;
		}

        /// <summary>
        /// Initializes a new instance of the Progress class.
        /// </summary>
        /// <param name="status">The status type.</param>
        /// <param name="percent">The percent complete.</param>
        public Progress(StatusMessage status, int percent)
			: this(status)
		{
			this.percent = percent;
		}

        /// <summary>
        /// Initializes a new instance of the Progress class.
        /// </summary>
        /// <param name="status">The status type.</param>
        /// <param name="percent">The percent complete.</param>
        /// <param name="message">The status message.</param>
        public Progress(StatusMessage status, int percent, string message)
			: this(status, percent)
		{
			this.message = message;
		}

        /// <summary>
        /// Gets the type of status.
        /// </summary>
		public StatusMessage Status
		{
			get { return this.status; }
		}

        /// <summary>
        /// Gets the status message.
        /// </summary>
		public string Message
		{
			get { return this.message; }
		}

        /// <summary>
        /// Gets the status percent.
        /// </summary>
		public int Percent
		{
			get { return this.percent; }
		}
	}
}