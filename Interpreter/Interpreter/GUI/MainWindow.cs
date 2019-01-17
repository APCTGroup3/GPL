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

    TextView editor;
    bool programIsSaved = false;
    string fileName = string.Empty;
    bool useNLPLexer = false;
    ToolButton nlpLexerSwitchBtn = new ToolButton(Stock.Execute);

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

        this.nlpLexerSwitchBtn.ModifyBg(StateType.Normal, new Gdk.Color(255,0,0));
        this.nlpLexerSwitchBtn.TooltipText = "Toggle the experimental NLP lexer";
        this.nlpLexerSwitchBtn.Clicked += nlpLexerSwitchBtnClicked;

        //Add menu items to menu bar
        menu.Insert(newBtn, 0);
        menu.Insert(openBtn, 1);
        menu.Insert(saveBtn, 2);
        menu.Insert(runBtn, 3);
        menu.Insert(this.nlpLexerSwitchBtn, 4);

        //Create Horizontal pane
        //HPaned hpane = new HPaned();

        //Create TextView
        var editorWindow = new ScrolledWindow();
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

    void NewBtnClicked(object sender, EventArgs eventArgs)
    {
        if (!this.programIsSaved)
        {
            MessageDialog messageDialog = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.Close, "Make sure you've saved your work!");

            ResponseType response = (ResponseType)messageDialog.Run();

            if (response == ResponseType.Close || response == ResponseType.DeleteEvent)
            {
                messageDialog.Destroy();
            }
        }
        else
        {
            editor.Buffer.Text = "";
            this.fileName = string.Empty;
        }
    }

    void SaveBtnClicked(object sender, EventArgs eventArgs)
    {
        this.programIsSaved = true;
        if (this.fileName == string.Empty)
        {
            Gtk.FileChooserDialog fileChooserDialog = new Gtk.FileChooserDialog("Choose where to save your file",
                                                                                this,
                                                                                FileChooserAction.Save,
                                                                                "Cancel", ResponseType.Cancel,
                                                                                "Save", ResponseType.Accept);
            ResponseType response = (ResponseType)fileChooserDialog.Run();
            if (response == ResponseType.Accept)
            {
                this.fileName = fileChooserDialog.Filename;
            }
            if (response == ResponseType.Cancel)
            {
                fileChooserDialog.Destroy();
            }
            fileChooserDialog.Destroy();
        }
        System.IO.File.WriteAllText(this.fileName, editor.Buffer.Text);
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
            this.fileName = fileChooserDialog.Filename;
            System.IO.FileStream fileStream = System.IO.File.OpenRead(this.fileName);

            /*
            *   https://docs.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=netframework-4.7.2
            */

            // Get the text from the file and output it to the editor
            System.IO.StreamReader streamReader = new System.IO.StreamReader(fileStream);
            string fileText = streamReader.ReadToEnd();
            editor.Buffer.Text = fileText;


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
        Console.WriteLine("Execute the program!");

        string sourceCode = editor.Buffer.Text;

        if (sourceCode.Length > 1)
        {
            if (this.useNLPLexer) {
                // use the NLP lexer to format the code
                var lines = sourceCode.Split('\n');
                NLP_Lexer.Lexer nlpLexer = new NLP_Lexer.Lexer("7DOTRBXV6DLL22FQUJRKOMSCOEUL5XG5");
                string tempSourceCode = "";
                foreach(var line in lines)
                {
                    try {
                        tempSourceCode += nlpLexer.Tokenise(line);
                        Console.WriteLine(tempSourceCode);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                if(tempSourceCode.Length > 0)
                {
                    sourceCode = tempSourceCode;
                }
            }
            CoreParser.Lexer lexer = new CoreParser.Lexer(sourceCode);
            try
            {
                lexer.Tokenise();
                List<Token> tokens = lexer.getTokenList();
                CoreParser.Parser.Parser parser = new CoreParser.Parser.Parser();
                CoreParser.Parser.AST.Node ast = parser.Parse(tokens);

                ParserEngine.Engine.Engine engine = new Engine.Engine();
                engine.Run(ast);
            }
            catch (Exception e)
            {
                MessageDialog md = new MessageDialog(this,
                    DialogFlags.DestroyWithParent, MessageType.Error,
                    ButtonsType.Close, e.Message);
                md.Run();
                md.Destroy();
            }

            var consoleOutput = ConsoleOutput.Instance.GetOutput();
            if(consoleOutput != null){
                GUI.ConsoleWindow consoleWindow = GUI.ConsoleWindow.Instance;
                ScrolledWindow consoleWrapper = new ScrolledWindow();
                TextView console = new TextView();
                consoleWrapper.Add(console);
                consoleWindow.Add(consoleWrapper);

                foreach (var line in consoleOutput)
                {
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine(line);
                    // Write output to console window
                    console.Buffer.Text = console.Buffer.Text + "\n" + line;
                    Console.WriteLine("--------------------------------------------------");
                }
                consoleWindow.ShowAll();

            }



            //parser.PrintTree(ast);

            // Open a new window
            //GPLGUI.ParseTreeDisplay parseTreeDisplay = new GPLGUI.ParseTreeDisplay(ast);
            //parseTreeDisplay.Title = "Parse Tree";
            //parseTreeDisplay.Show();

        }
    }

    void nlpLexerSwitchBtnClicked(object sender, EventArgs eventArgs)
    {
        this.useNLPLexer = !this.useNLPLexer;
        if (this.useNLPLexer) {
            this.editor.ModifyBg(StateType.Normal, new Gdk.Color(255, 122, 122));
        } else {
            this.editor.ModifyBg(StateType.Normal, new Gdk.Color(249, 249, 249));
        }
    }


}