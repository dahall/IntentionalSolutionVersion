using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
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

			var menuCommandID = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(Execute, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		/// <summary>Gets the instance of the command.</summary>
		public static SetVerCmd Instance { get; private set; }

		public DTE Dte { get; }

		/// <summary>Gets the service provider from the owner package.</summary>
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

		/// <summary>Initializes the singleton instance of the command.</summary>
		/// <param name="package">Owner package, not null.</param>
		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in SetVerCmd's constructor requires the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
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
}