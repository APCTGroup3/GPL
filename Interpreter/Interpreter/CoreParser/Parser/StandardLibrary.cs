using System;
using System.Collections.Generic;

namespace CoreParser.Parser.StandardLibrary
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

        public static Terminal Run(string name, params Terminal[] args)
        {
            foreach (Function f in functions)
            {
                if (f.Name.Equals(name))
                {
                    return f.Execute(args);
                }
            }
            throw new Exception("No method found");
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
                return new Void();
            }));
        }





    }
}
