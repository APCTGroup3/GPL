using System;
using System.Collections.Generic;

namespace EngineLibrary
{
    public delegate Terminal FunctionDelegate(params Terminal[] parameters);


    public class Function
    {
        public string Name { get; }
        private Type[] parameterTypes;
        private FunctionDelegate function;

         
        public Function(string name, Type[] types, FunctionDelegate func)
        {
            Name = name;
            parameterTypes = types;
            function = func;
        }

        public Terminal Execute(params Terminal[] args)
        {
            //Check parameters passed
            if (args.Length != parameterTypes.Length)
            {
                throw new Exception("Error in function " + Name + ": Expected " + parameterTypes.Length + "arguments, found " + args.Length);
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (!(args[i].GetType().Equals(parameterTypes[i]) || args[i].GetType().IsSubclassOf(parameterTypes[i])))
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

        public static Terminal Run(string name, params Terminal[] args)
        {
            foreach (Function f in functions)
            {
                if (f.Name.Equals(name))
                {
                    return f.Execute(args);
                }
            }
            throw new Exception("No method " + name + " found");
        }


        static StandardLibrary()
        {
            //Pow functions
            functions.Add(new Function("Pow", new Type[] { typeof(Number), typeof(Number) },
                                       args =>
                                       {
                                           return new Number(Math.Pow(args[0].ToDouble(), args[1].ToDouble()));
                                       }
                                      ));

            functions.Add(new Function("Print", new Type[] { typeof(Terminal) },
                                      args =>
            {
                Console.WriteLine(args[0].ToStr());
                ConsoleOutput.Instance.AddNewLine(args[0].ToStr());
                return new Void();
            }));

            functions.Add(new Function("Length", new Type[] { typeof(Arr) },
                args =>
                {
                    return new Number(args[0].ToArr().Elements.Count);
                }
            ));

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

            functions.Add(new Function("InitArray", new Type[] { typeof(Number) },
                args =>
                {
                    int i = (int)args[0].ToDouble();
                    return new Arr(i);
                }
            ));

            functions.Add(new Function("Random", new Type[] { typeof(Number), typeof(Number) },
                args =>
                {
                    var min = (int)args[0].ToDouble();
                    var max = (int)args[1].ToDouble();

                    var res = rnd.Next(min, max);
                    return new Number(res);
                }
            ));

            functions.Add(new Function("Int", new Type[] { typeof(Number) },
                args =>
                {
                    return new Number((int)args[0].ToDouble());
                }
            ));

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
