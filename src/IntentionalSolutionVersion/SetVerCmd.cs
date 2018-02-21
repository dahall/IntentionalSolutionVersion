using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;

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
		private readonly SetVerCmdPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="SetVerCmd"/> class. Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private SetVerCmd(Package package)
		{
			this.package = (package as SetVerCmdPackage) ?? throw new ArgumentNullException("package");

			if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
			{
				var menuCommandID = new CommandID(CommandSet, CommandId);
				var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
				commandService.AddCommand(menuItem);
				menuItem.BeforeQueryStatus += (s, e) => menuItem.Enabled = this.package.DTE.Solution.IsOpen;
			}
		}

		private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			(sender as OleMenuCommand).Enabled = package.DTE.Solution.IsOpen;
		}

		/// <summary>Gets the instance of the command.</summary>
		public static SetVerCmd Instance { get; private set; }

		/// <summary>Gets the service provider from the owner package.</summary>
		private IServiceProvider ServiceProvider => package;

		/// <summary>Initializes the singleton instance of the command.</summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package)
		{
			Instance = new SetVerCmd(package);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked. See the constructor to see how the menu item is associated
		/// with this function using OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void MenuItemCallback(object sender, EventArgs e)
		{
			new VersionDialog(package?.DTE?.Solution?.FileName, package?.DTE?.Solution?.GetFiles()).ShowDialog();
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
		public Version Version { get; set; }
		// This may be an XMLPath stmt or Line#
		public string Locator { get; set; }
		public string RegEx { get; set; }
		public string Namespace { get; set; }
		public string Project { get; set; }
		public override string ToString() => $"{Path.GetFileName(FileName)}={Version}:{LineText}";
	}

}