using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
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
		public static readonly Guid CommandSet = new("8cd00976-86c6-4ddd-9c4f-c758bc01d6e4");

		/// <summary>VS Package that provides this command, not null.</summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="SetVerCmd"/> class. Adds our command handlers for menu (commands must exist in the
		/// command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private SetVerCmd(AsyncPackage package, EnvDTE80.DTE2 dte, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
			Dte = dte;

			CommandID menuCommandID = new(CommandSet, CommandId);
			MenuCommand menuItem = new(Execute, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		/// <summary>Gets the instance of the command.</summary>
		public static SetVerCmd Instance { get; private set; }

		/// <summary>Design-time Environment.</summary>
		public EnvDTE80.DTE2 Dte { get; }

		/// <summary>Gets the service provider from the owner package.</summary>
		public IAsyncServiceProvider ServiceProvider => package;

		/// <summary>Initializes the singleton instance of the command.</summary>
		/// <param name="package">Owner package, not null.</param>
		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in SetVerCmd's constructor requires the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			EnvDTE80.DTE2 dte = await package.GetServiceAsync(typeof(DTE)) as EnvDTE80.DTE2;
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

			IDictionary<string, List<string>> files = null;
			if (Dte?.Solution is not null && (files = Dte.Solution.GetFiles()) is not null || files.Count > 0)
				new VersionDialog(Dte?.Solution?.FileName, files).ShowDialog();
			else
				EnvDTEExt.ShowMessageBox("Unable to identify any projects.");
		}
	}
}