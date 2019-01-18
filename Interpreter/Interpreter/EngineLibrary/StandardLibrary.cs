using System;
using System.Collections.Generic;

namespace EngineLibrary
{
    public delegate Terminal FunctionDelegate(params Terminal[] parameters);

    /* Defines a built-in function for the GPL language */
    public class Function
    {
        public string Name { get; } //The method name
        public Type[] ParameterTypes { get; private set; } //The arg types in the order they should be given
        private FunctionDelegate function; //The method implementation
          
         
        public Function(string name, Type[] types, FunctionDelegate func)
        {
            Name = name;
            ParameterTypes = types;
            function = func;
        }

        //Executes the function given correct arguments, else throws an exception
        public Terminal Execute(params Terminal[] args)
        {
            //Check parameters passed
            if (args.Length != ParameterTypes.Length)
            {
                throw new Exception("Error in function " + Name + ": Expected " + ParameterTypes.Length + " arguments, found " + args.Length);
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (!(args[i].GetType().Equals(ParameterTypes[i]) || args[i].GetType().IsSubclassOf(ParameterTypes[i])))
                {
                    throw new Exception("Incorrect parameter type");
                }
            }

            return function(args);
        }
    }

    public static class StandardLibrary
    {
        private static List<Function> functions = new List<Function>();

        //Static variables used in functions
        private static readonly Random rnd = new Random();

        /* Given a method name and array of parameters, executes the corresponding method if one exists, or throws an exception */
        public static Terminal Run(string name, params Terminal[] args)
        {
            foreach (Function f in functions)
            {
                if (f.Name.Equals(name))
                {
                    var valid = true;
                    //Check parameters passed
                    if (args.Length != f.ParameterTypes.Length)
                    {
                        valid = false;
                    }

                    for (int i = 0; i < args.Length; i++)
                    {
                        if (!(args[i].GetType().Equals(f.ParameterTypes[i]) || args[i].GetType().IsSubclassOf(f.ParameterTypes[i])))
                        {
                            valid = false;
                        }
                    }
                    if (valid) return f.Execute(args);
                }
            }
            throw new Exception("No method " + name + " found that has the given method signature.");
        }

        //Defines built-in functions usable by the language
        static StandardLibrary()
        {
            //Pow functions
            functions.Add(new Function("Pow", new Type[] { typeof(Number), typeof(Number) },
                                       args =>
                                       {
                                           return new Number(Math.Pow(args[0].ToDouble(), args[1].ToDouble()));
                                       }
                                      ));

            //Prints a string to the console
            functions.Add(new Function("Print", new Type[] { typeof(Terminal) },
                                      args =>
            {
                Console.WriteLine(args[0].ToStr());
                ConsoleOutput.Instance.AddNewLine(args[0].ToStr());
                return new Void();
            }));

            //Blank print function for printing newline
            functions.Add(new Function("Print", new Type[] { },
                                      args =>
                                      {
                                          Console.WriteLine();
                                          ConsoleOutput.Instance.AddNewLine("");
                                          return new Void();
                                      }));

            //Returns the length of a given array
            functions.Add(new Function("Length", new Type[] { typeof(Arr) },
                args =>
                {
                    return new Number(args[0].ToArr().Elements.Count);
                }
            ));

            //Example search function for an array 
            functions.Add(new Function("BinarySearch", new Type[] { typeof(Arr), typeof(Number) },
                args =>
                {
                    var array = args[0].ToArr();
                    var key = args[1].ToDouble();



                    var low = 0;
                    var high = array.Elements.Count;

                    while(low <= high)
                    {
                        int mid = ((high+low) / 2);
                        Console.WriteLine("High = {0}", high);
                        Console.WriteLine("Low = {0}", low);
                        Console.WriteLine("Mid = {0}", mid);
                        Console.WriteLine("array[mid] = {0}\n\n", (int)array.Get(mid).ToDouble());
                        Console.WriteLine("Key = {0}", key);

                        if ((int)array.Get(mid).ToDouble() == (int)key)
                        {
                            return new Number(mid);
                        }
                        if((int)key > (int)array.Get(mid).ToDouble())
                        {
                            low = mid+1;
                        }
                        if ((int)key < (int)array.Get(mid).ToDouble())
                        {
                            high = mid-1;
                        }
                    }
                    return new Number(-1);
                }
            ));

            //Returns an empty array of the given size;
            functions.Add(new Function("InitArray", new Type[] { typeof(Number) },
                args =>
                {
                    int i = (int)args[0].ToDouble();
                    return new Arr(i);
                }
            ));


            //Generates a random integer between two given limits
            functions.Add(new Function("Random", new Type[] { typeof(Number), typeof(Number) },
                args =>
                {
                    var arg0 = (int)args[0].ToDouble();
                    var arg1 = (int)args[1].ToDouble();

                    var min = Math.Min(arg0, arg1);
                    var max = Math.Max(arg0, arg1);

                    var res = rnd.Next(min, max);
                    return new Number(res);
                }
            ));

            //Given a number, returns it's floor int value
            functions.Add(new Function("Int", new Type[] { typeof(Number) },
                args =>
                {
                    return new Number((int)args[0].ToDouble());
                }
            ));

            //Returns the lowest Number of two given numbers
            functions.Add(new Function("Min", new Type[] { typeof(Number), typeof(Number) },
                args =>
                {
                    var n1 = args[0].ToDouble();
                    var n2 = args[1].ToDouble();
                    if (n1 <= n2)
                    {
                        return args[0];

                    }
                    else
                    {
                        return args[1];
                    }
                }

            ));

        }





    }
}
