using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Json2CSharp
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ConvertJson2CSharpCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("917d3b37-34f4-4298-8172-d41e5adaaaeb");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertJson2CSharpCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ConvertJson2CSharpCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ConvertJson2CSharpCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ConvertJson2CSharpCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ConvertJson2CSharpCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var application = (DTE2)Package.GetGlobalService(typeof(DTE));
            if (IsCs(application))
            {
                try
                {
                    Paste(application, Json2CSharpLib.JsonCSharpConvertion.Convert(ClipboardText));
                }
                catch (Exception)
                {
                    ShowError("Not a valid JSON");
                }
            }
            else
            {
                ShowError("Works only on C# files");
            }
        }

        private void ShowError(string errorMessage)
        {
            string message = string.Format(CultureInfo.CurrentCulture, errorMessage);
            string title = "Convert JSON to C#";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private static bool IsCs(DTE2 application)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return application.ActiveWindow.Caption.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Inserts text at current cursor location in the application
        /// </summary>
        /// <param name="application">application with activewindow</param>
        /// <param name="text">text to insert</param>
        private static void Paste(DTE2 application, string text)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            //get the text document
            var txt = (TextDocument)application.ActiveDocument.Object("TextDocument");

            //get an edit point
            EditPoint ep = txt.Selection.ActivePoint.CreateEditPoint();

            //get a start point
            EditPoint sp = txt.Selection.ActivePoint.CreateEditPoint();

            //open the undo context
            bool isOpen = application.UndoContext.IsOpen;
            if (!isOpen)
                application.UndoContext.Open("JSON to C#");

            //clear the selection
            if (!txt.Selection.IsEmpty)
                txt.Selection.Delete();

            //insert the text
            //ep.Insert(Indent(text, ep.LineCharOffset))
            ep.Insert(text);

            //smart format
            sp.SmartFormat(ep);

            //close the context
            if (!isOpen)
                application.UndoContext.Close();
        }
        private static string ClipboardText
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return string.Empty;
                //is it Unicode? Then we use that
                if (iData.GetDataPresent(DataFormats.UnicodeText))
                    return Convert.ToString(iData.GetData(DataFormats.UnicodeText));
                //otherwise ANSI
                if (iData.GetDataPresent(DataFormats.Text))
                    return Convert.ToString(iData.GetData(DataFormats.Text));
                return string.Empty;
            }
        }
    }
}
