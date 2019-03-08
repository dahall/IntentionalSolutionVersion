using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using EnvDTE;
using Task = System.Threading.Tasks.Task;

namespace IntentionalSolutionVersion
{
	/// <summary>Command handler</summary>
	internal sealed class SetVerCmd
	{
		/// <summary>Command ID.</summary>
		public const int CommandId = 0x0100;

		/// <summary>Command menu group (command set GUID).</summary>
		public static readonly Guid CommandSet = new Guid("8cd00976-86c6-4ddd-9c4f-c758bc01d6e4");

		/// <summary>VS Package that provides this command, not null.</summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="SetVerCmd"/> class. Adds our command handlers for menu (commands must exist in the
		/// command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private SetVerCmd(AsyncPackage package, DTE dte, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
			Dte = dte;

			var menuCommandId = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(Execute, menuCommandId);
			commandService.AddCommand(menuItem);
		}

		public DTE Dte { get; }

		/// <summary>Gets the instance of the command.</summary>
		public static SetVerCmd Instance { get; private set; }

		/// <summary>Gets the service provider from the owner package.</summary>
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => package;

		/// <summary>Initializes the singleton instance of the command.</summary>
		/// <param name="package">Owner package, not null.</param>
		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in SetVerCmd's constructor requires the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
			var dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
			Instance = new SetVerCmd(package, dte, commandService);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked. See the constructor to see how the menu
		/// item is associated with this function using OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void Execute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			new VersionDialog(Dte?.Solution?.FileName, Dte?.Solution?.GetFiles()).ShowDialog();
		}
	}

	internal class VerData
	{
		public VerData(string fn, Version ver, string line, string loc, string regex, string ns = null)
		{
			FileName = fn; Version = ver; LineText = line; Locator = loc; RegEx = regex; Namespace = ns;
		}

		public string FileName { get; set; }
		public string LineText { get; set; }

		// This may be an XMLPath stmt or Line#
		public string Locator { get; set; }

		public string Namespace { get; set; }
		public string Project { get; set; }
		public string RegEx { get; set; }
		public Version Version { get; set; }

		public override string ToString() => $"{Path.GetFileName(FileName)}={Version}:{LineText}";
	}
}