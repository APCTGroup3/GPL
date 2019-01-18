using System;
using Gtk;
using CoreParser;
using System.Collections.Generic;
using ParserEngine;
using EngineLibrary;
using Microsoft.FSharp.Core;
using NLP_Lexer;

public partial class MainWindow : Gtk.Window
{

    TextView editor; // Editor has to belong to class to allow other methods access
    bool programIsSaved = false; // Store saved state of the program
    string fileName = string.Empty; // Initialise empty file name

    //Allows checks whether user wants to use NLP or parse tree display
    bool useNLPLexer = false;
    bool useParseTreeDisplay = false;

    // Must be accessed in multiple methods to set text
    TextView infoTextView = new TextView();

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        // Make fullscreen and centered
        SetDefaultSize(Screen.Width, Screen.Height);
        SetPosition(WindowPosition.Center);

        //Create vbox to hold UI
        VBox vbox = new VBox(false, 2);

        // Create menu bar
        Toolbar menu = new Toolbar();
        menu.ToolbarStyle = ToolbarStyle.Icons;

        //Make menu items
        ToolButton newBtn = new ToolButton(Stock.New);
        newBtn.TooltipText = "Create a new program";
        newBtn.Clicked += NewBtnClicked;

        ToolButton saveBtn = new ToolButton(Stock.Save);
        saveBtn.TooltipText = "Save the program";
        saveBtn.Clicked += SaveBtnClicked;

        ToolButton openBtn = new ToolButton(Stock.Open);
        openBtn.TooltipText = "Open a program";
        openBtn.Clicked += OpenBtnClicked;

        ToolButton runBtn = new ToolButton(Stock.MediaPlay);
        runBtn.TooltipText = "Run the program";
        runBtn.Clicked += RunBtnClicked;

        ToolButton nlpLexerSwitchBtn = new ToolButton(Stock.DialogWarning); // NLP Lexer button displayed with warning as it's experimental
        nlpLexerSwitchBtn.TooltipText = "Toggle the experimental NLP lexer";
        nlpLexerSwitchBtn.Clicked += nlpLexerSwitchBtnClicked;

        ToolButton parseTreeSwitchBtn = new ToolButton(Stock.Indent);
        parseTreeSwitchBtn.TooltipText = "Toggle the Parse Tree display";
        parseTreeSwitchBtn.Clicked += ParseTreeSwitchBtnClicked;


        //Add menu items to menu bar
        menu.Insert(newBtn, 0);
        menu.Insert(openBtn, 1);
        menu.Insert(saveBtn, 2);
        menu.Insert(runBtn, 3);
        menu.Insert(new SeparatorToolItem(), 4); // Seperate NLP toggle and parse tree toggle from other buttons
        menu.Insert(nlpLexerSwitchBtn, 5);
        menu.Insert(parseTreeSwitchBtn, 5);
        menu.ShowAll();



        //Create TextView
        var editorWindow = new ScrolledWindow(); // Make TextView Scrollable
        editor = new TextView();
        int editorWidth = this.editor.SizeRequest().Width + 10;
        int editorHeight = this.editor.SizeRequest().Height + 10;
        editor.BorderWidth = 10;
        editor.ModifyBg(StateType.Normal, new Gdk.Color(249, 249, 249));
        editorWindow.Add(editor);


        // Add menu to top of screen
        vbox.PackStart(menu, false, false, 0);

        vbox.SetSizeRequest(editorWidth, editorHeight);

        // Add editor to vbox
        vbox.Add(editorWindow);

        //Add info view to bottom of window
        vbox.PackEnd(this.infoTextView, false, false, 0);
        this.infoTextView.Visible = true; // Make sure info view is visible

        // Add vbox to window
        Add(vbox);

        // Display all children
        ShowAll();
    }


    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    //New Button event
    void NewBtnClicked(object sender, EventArgs eventArgs)
    {
        //Clear everything relating to the current file
        editor.Buffer.Text = ""; 
        this.fileName = string.Empty;
    }

    void SaveBtnClicked(object sender, EventArgs eventArgs)
    {
        Console.WriteLine("Save");
        // Check for filename, if it has one just write straight away
        if (this.fileName == string.Empty)
        {
            // Open file chooser dialog with save action
            Gtk.FileChooserDialog fileChooserDialog = new Gtk.FileChooserDialog("Choose where to save your file",
                                                                                this,
                                                                                FileChooserAction.Save,
                                                                                "Cancel", ResponseType.Cancel,
                                                                                "Save", ResponseType.Accept);

            // Run the dialog by casting the return from run to a ResponseType object
            ResponseType response = (ResponseType)fileChooserDialog.Run();

            // Check the response type
            if (response == ResponseType.Accept)
            {
                // Only write the file if the user selects accept
                this.fileName = fileChooserDialog.Filename;
                System.IO.File.WriteAllText(this.fileName, editor.Buffer.Text);
                this.programIsSaved = true;
            }
            if (response == ResponseType.Cancel)
            {
                fileChooserDialog.Destroy();
            }
            fileChooserDialog.Destroy();
        }
    }

    void OpenBtnClicked(object sender, EventArgs eventArgs)
    {
        // Select and load in a source code file

        /*
        *   https://stackoverflow.com/questions/20612468/making-gtk-file-chooser-to-select-file-only
        */

        Gtk.FileChooserDialog fileChooserDialog = new Gtk.FileChooserDialog("Select your source code file",
                                                                            this,
                                                                            FileChooserAction.Open,
                                                                            "Cancel", ResponseType.Cancel,
                                                                            "Open", ResponseType.Accept
                                                                           );

        ResponseType response = (ResponseType)fileChooserDialog.Run();

        if (response == ResponseType.Accept)
        {
            // Set filename and open selected file
            this.fileName = fileChooserDialog.Filename;
            System.IO.FileStream fileStream = System.IO.File.OpenRead(this.fileName);

            /*
            *   https://docs.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=netframework-4.7.2
            */

            // Get the text from the file and output it to the editor
            System.IO.StreamReader streamReader = new System.IO.StreamReader(fileStream);
            string fileText = streamReader.ReadToEnd();
            editor.Buffer.Text = fileText;


            //Close the filestream for safety
            fileStream.Close();
        }
        if (response == ResponseType.Cancel)
        {
            fileChooserDialog.Destroy();
        }
        fileChooserDialog.Destroy();
    }

    void RunBtnClicked(object sender, EventArgs eventArgs)
    {
        //Execute the program

        string sourceCode = editor.Buffer.Text;

        if (sourceCode.Length > 1)
        {
            if (this.useNLPLexer) {
                // use the NLP lexer to format the code
                var lines = sourceCode.Split('\n');
                NLP_Lexer.Lexer nlpLexer = new NLP_Lexer.Lexer("7DOTRBXV6DLL22FQUJRKOMSCOEUL5XG5"); // Create NLP Lexer with access token for wit.ai
                string tempSourceCode = ""; // Init
                foreach(var line in lines) //Loop through lines
                {
                    try {
                        tempSourceCode += nlpLexer.Tokenise(line); // Add wit ai output to sourcecode
                        Console.WriteLine(tempSourceCode);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                // Make sure that there is code in ttempSourceCode before executing it
                if(tempSourceCode.Length > 0)
                {
                    sourceCode = tempSourceCode;
                }
            }

            // Start the exectuion

            CoreParser.Lexer lexer = new CoreParser.Lexer(sourceCode);
            try
            {
                // Tokenise
                lexer.Tokenise();
                List<Token> tokens = lexer.getTokenList();

                // Parse from returned token list
                CoreParser.Parser.Parser parser = new CoreParser.Parser.Parser();
                CoreParser.Parser.AST.Node ast = parser.Parse(tokens);

                // Execute the parse tree
                ParserEngine.Engine.Engine engine = new Engine.Engine();
                engine.Run(ast);

                // Display parse tree if user has toggled it
                if (this.useParseTreeDisplay)
                {
                    // Open a new window
                    GUI.ParseTreeDisplay parseTreeDisplay = new GUI.ParseTreeDisplay(ast);
                    parseTreeDisplay.Title = "Parse Tree";
                    parseTreeDisplay.ShowAll();
                }
            }
            // Display errors encountered during execution in alert
            catch (Exception e)
            {
                MessageDialog md = new MessageDialog(this,
                    DialogFlags.DestroyWithParent, MessageType.Error,
                    ButtonsType.Close, e.Message);
                md.Run();
                md.Destroy();
            }

            // Collect console output
            var consoleOutput = ConsoleOutput.Instance.GetOutput();
            if(consoleOutput != null){
                // As long as there is console output, get the window and display it
                GUI.ConsoleWindow consoleWindow = GUI.ConsoleWindow.Instance;
                ScrolledWindow consoleWrapper = new ScrolledWindow(); // Make Console Window scrollable
                TextView console = new TextView();
                consoleWrapper.Add(console);
                consoleWindow.Add(consoleWrapper);

                // Write the console output to the console window
                foreach (var line in consoleOutput)
                {
                    // Write output to console window
                    console.Buffer.Text = console.Buffer.Text + "\n" + line;
                }
                consoleWindow.ShowAll();

            }

        }
    }

    void nlpLexerSwitchBtnClicked(object sender, EventArgs eventArgs)
    {
        // Toggle NLP lexer and show red border
        this.useNLPLexer = !this.useNLPLexer;
        if (this.useNLPLexer) {
            this.editor.ModifyBg(StateType.Normal, new Gdk.Color(255, 122, 122));
        } else {
            this.editor.ModifyBg(StateType.Normal, new Gdk.Color(249, 249, 249));

        }

    }

    void ParseTreeSwitchBtnClicked(object sender, EventArgs e)
    {
        // Toggle parse tree display and display text indicating the current state
        this.useParseTreeDisplay = !this.useParseTreeDisplay;
        if (this.useParseTreeDisplay)
        {
            this.infoTextView.Buffer.Text = "Using Parse Tree Display";
        }
        else
        {
            this.infoTextView.Buffer.Text = "";
        };
    }

}