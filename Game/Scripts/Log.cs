using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Godot
{
	/// <summary>
	/// Handles the logging of messages and <see cref="Exception"/>s to a log file.
	/// </summary>
	public static class Log
	{
		static Log()
		{
			FilePath = DefaultFilePath;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
		}

		private const string DefaultFilePath = "user://Log.txt";
		private const string ErrorLogPath = "user://ErrorLog.txt";

		private const int MaxEntryCount = 100;

		private const int MaxFlushIntervalSeconds = 60;

		private const int MaxFlushIntervalMessages = 10;

		private static readonly Queue<Entry> Entries = new Queue<Entry>(MaxEntryCount + 1);

		private static FileAccess _file; // = new();

		private static DateTime _lastSynced;

		private static bool _hasLoggedError;

		/// <summary>
		/// The file path to which log entries are written.
		/// </summary>
		public static string FilePath
		{
			get
			{
				return _file.GetPathAbsolute();
			}
			set
			{
				// If a previous log file already exists, save its contents and close it
				if(_file != null && _file.IsOpen())
				{
					Flush(true);
					_file.Close();
				}

				_file = FileAccess.Open(value, FileAccess.ModeFlags.Write);
				//Log.file.Open(value, File.ModeFlags.Write);
			}
		}

		/// <summary>
		/// Emitted when an <see cref="Entry"/> has just been written to the log file.
		/// </summary>
		public static event Action<Entry> EntryWritten;

		/// <summary>
		/// Writes the text representation of <paramref name="entry"/> to the log file. Also writes to the console in debug mode.
		/// </summary>
		/// <param name="entry">The <see cref="Entry"/> to write.</param>
		public static void Write(Entry entry)
		{
			GodotPrint(entry);
			_file.StoreLine(entry.ToString());
			if(Entries.Count is MaxEntryCount)
			{
				Entries.Dequeue();
			}

			Entries.Enqueue(entry);
			Flush();
			EntryWritten?.Invoke(entry);
		}

		/// <summary>
		/// Writes <paramref name="message"/> to the log file, encoding it as a notification.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public static void Write(string message)
		{
			Write(new Entry(message, Entry.MessageSeverity.Notification));
		}

		/// <summary>
		/// Writes <paramref name="message"/> to the log file, encoding it as a warning.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public static void Warning(string message)
		{
			Write(new Entry(message, Entry.MessageSeverity.Warning));
		}

		/// <summary>
		/// Writes the text representation of <paramref name="exception"/> to the log file, encoding it as a warning.
		/// </summary>
		/// <param name="exception">The <see cref="Exception"/> to write.</param>
		public static void Warning(Exception exception)
		{
			Write(new Entry(exception.ToString(), Entry.MessageSeverity.Warning));
		}

		/// <summary>
		/// Writes <paramref name="message"/> to the log file, encoding it as an error.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public static void Error(string message)
		{
			Write(new Entry(message, Entry.MessageSeverity.Error));
		}

		/// <summary>
		/// Writes the text representation of <paramref name="exception"/> to the log file, encoding it as an error.
		/// </summary>
		/// <param name="exception">The <see cref="Exception"/> to write.</param>
		public static void Error(Exception exception)
		{
			Write(new Entry(exception.ToString(), Entry.MessageSeverity.Error));
		}

		public static void ResetHasLoggedError()
		{
			_hasLoggedError = false;
		}

		private static void Flush(bool force = false)
		{
			DateTime now = DateTime.Now;
			if(!force && ((now - _lastSynced).TotalSeconds < MaxFlushIntervalSeconds) && (Entries.Count < MaxFlushIntervalMessages))
			{
				return;
			}

			// If it has been 60 seconds since the last flush, or if there are 10 or more entries in the queue, flush immediately
			Entries.Clear();
			_file.Flush();
			_lastSynced = now;
		}

		private static void GodotPrint(Entry entry)
		{
			switch(entry.Severity)
			{
				case Entry.MessageSeverity.Notification:
					GD.Print(entry);
					break;
				case Entry.MessageSeverity.Warning:
					GD.PushWarning(entry.ToString());
					break;
				case Entry.MessageSeverity.Error:
					GD.PushError(entry.ToString());

					if(!_hasLoggedError)
					{
						using FileAccess errorFile = FileAccess.Open(ErrorLogPath, FileAccess.ModeFlags.Write);

						string json = GameController.Instance == null
							? JsonConvert.SerializeObject(AppController.Instance!.SaveFile.SaveData, SaveFile.JsonSerializerSettings)
							: JsonConvert.SerializeObject(GameController.Instance.SavedCampaign, SaveFile.JsonSerializerSettings);
						string fullErrorMessage = $"{json}\n\n\n{entry}";

						errorFile.StoreLine(fullErrorMessage);

						AppController.Instance.PopupManager.OpenPopupOnTop(new ErrorPopup.Request()
						{
							ErrorFilePath = errorFile.GetPathAbsolute(),
							FullErrorMessage = fullErrorMessage,
						});
					}

					_hasLoggedError = true;

					break;
			}
		}

		private static void OnUnhandledException(object source, UnhandledExceptionEventArgs arguments)
		{
			Write(new Entry(arguments.ExceptionObject.ToString(), Entry.MessageSeverity.Error));
			if(!arguments.IsTerminating)
			{
				return;
			}

			if(_file.IsOpen())
			{
				_file.Close();
			}

			_file.Dispose();
			AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
		}

		private static void OnProcessExit(object source, EventArgs arguments)
		{
			Console.WriteLine("process exit test");

			if(_file.IsOpen())
			{
				_file.Close();
			}

			_file.Dispose();
		}

		/// <summary>
		/// Represents a log entry.
		/// </summary>
		public sealed record Entry
		{
			/// <summary>
			/// Initialises a new <see cref="Entry"/> with the specified parameters.
			/// </summary>
			/// <param name="message">The message to include in the <see cref="Entry"/>.</param>
			/// <param name="severity">The <see cref="Entry"/>'s severity level.</param>
			public Entry(string message, MessageSeverity severity)
			{
				Message = message.Trim();
				Severity = severity;
				Timestamp = DateTime.Now;
			}

			/// <summary>
			/// The message of the <see cref="Entry"/>.
			/// </summary>
			public string Message
			{
				get;
			}

			/// <summary>
			/// The time when the <see cref="Entry"/> was created.
			/// </summary>
			public DateTime Timestamp
			{
				get;
			}

			/// <summary>
			/// The severity level of the <see cref="Entry"/>.
			/// </summary>
			public MessageSeverity Severity
			{
				get;
			}

			/// <summary>
			/// Returns a <see cref="String"/> that represents the <see cref="Entry"/>.
			/// </summary>
			/// <returns>A <see cref="String"/> in the format "[Severity] at Timestamp - Message".</returns>
			public override string ToString()
			{
				return $"[{Severity}] at {Timestamp.Hour:D2}:{Timestamp.Minute:D2}:{Timestamp.Second:D2}:{Timestamp.Millisecond:D3} - {Message}";
			}

			/// <summary>
			/// Represents a log entry severity.
			/// </summary>
			public enum MessageSeverity
			{
				/// <summary>
				/// Miscellaneous information.
				/// </summary>
				Notification,
				/// <summary>
				/// Minor errors that can usually be recovered from.
				/// </summary>
				Warning,
				/// <summary>
				/// Major errors that usually stop the program.
				/// </summary>
				Error,
			}
		}
	}
}
