using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CommandLineParser : MonoBehaviour
{
    [SerializeField] private List<CommandLineParam> commandLineParams;
    void Awake()
    {
        var args = Environment.GetCommandLineArgs().ToList();
        args.ForEach(arg =>
        {
            if (!arg.Contains("=")) return;
            
            var splitArg = arg.Split("=");
            commandLineParams.FirstOrDefault(parameter => parameter.identifier == splitArg[0].ToLower())
                .function.Invoke(splitArg[1]);
        });
    }
    
    [Serializable]
    public struct CommandLineParam
    {
        public string identifier;
        public UnityEvent<string> function;
    }
}