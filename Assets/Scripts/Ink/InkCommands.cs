using InkTools.InkCommandDef;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InkTools {
    namespace InkCommandDef {
        /// <summary>
        /// Command that can be executed from Ink.
        /// Can be executed with `$NameOfCommandClass #arg1 #arg2 #arg3`
        /// </summary>
        public abstract class InkCommand {
            /// <summary>
            /// Consturct a command.
            /// </summary>
            /// <param name="args">Arguments passed in by ink.</param>
            /// <param name="error">The error that an improperly constructed command returns.</param>
            public InkCommand(List<string> args, out string error) {
                error = null;
            }

            /// <summary>
            /// How to use this. When the constructor has an error, this is returned as well.
            /// Should be in the format: $commandName #&lt;requiredArg:type&gt; #&lt;requiredArg2&gt; #[optionalArg:type]
            /// People reading the docs: read this!
            /// </summary>
            public abstract string usage {
                get;
            }

            public static string CommandWrite(string name, List<string> args) {
                string output = "$" + name;
                foreach (var arg in args) {
                    output += " #" + args;
                }
                return output;
            }

            /// <summary>
            /// Called once on start, and then every frame after as long as it returns true.
            /// </summary>
            /// <returns>If we need to still update next frame.</returns>
            public virtual bool Update() { return false; }
        }

        static class CommandHelper {
            public static bool Vector3FromString(string str, out Vector3 vector) {
                vector = Vector3.zero;
                var split = str.Replace("(", "").Replace(")", "").Split(",");

                if (split.Length != 3) {
                    return false;
                }
                float x, y, z = 0f;

                x = float.Parse(split[0]);
                y = float.Parse(split[1]);
                z = float.Parse(split[2]);
                vector = new Vector3(x, y, z);
                return true;
            }
        }

        public class moveTo : InkCommand {
            Vector3 positionToUse;
            GameObject toMove;

            public override string usage => "$moveTo #<objectToMove:string> #<objectTarget:string|Vector3>";

            public moveTo(List<string> args, out string error) : base(args, out error) {
                if (args.Count != 2) {
                    error = "Requires two args: objectToMove and objectTarget.";
                    return;
                }
                string objectToMove = args[0];
                string target = args[1];

                toMove = GameObject.Find(objectToMove);
                if (toMove != null) {
                    if (CommandHelper.Vector3FromString(target, out Vector3 vec)) {
                        positionToUse = vec;
                    } else {
                        GameObject targetObject = GameObject.Find(target);
                        if (targetObject != null) {
                            positionToUse = targetObject.transform.position;
                        } else {
                            error = "objectTarget " + target + " could not be found.";
                            return;
                        }
                    }

                } else {
                    error = "Could not find " + objectToMove;
                    return;
                }
            }

            public override bool Update() {
                toMove.transform.position = Vector3.Lerp(toMove.transform.position, positionToUse, Time.deltaTime);
                if (Vector3.Distance(toMove.transform.position, positionToUse) <= 0.5f) {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Stop the player from moving.
        /// </summary>
        public class enablePlayerMove : InkCommand {
            public override string usage => "$enablePlayerMove #<enabled:bool>";

            public enablePlayerMove(List<string> args, out string error) : base(args, out error) { 
                if (args.Count != 1) {
                    error = "Requires one arg of enabled:bool. Whether or not to allow the player to move.";
                    return;
                }

                if (bool.TryParse(args[0], out bool enabled)) {
                    var character = GameObject.FindGameObjectWithTag("Player").GetComponent<characterController>();
                    character.moveEnabled = enabled;
                } else {
                    error = "enabled arg is not a bool.";
                }
            }
            public override bool Update() {
                return base.Update();
            }
        }
    }

    /// <summary>
    /// For running commands created from Ink.
    /// </summary>
    public class InkCommands {
        protected Dictionary<string, ConstructorInfo> commands = new Dictionary<string, ConstructorInfo>();
        List<InkCommand> commandUpdates = new List<InkCommand>();

        //static Type[] commandTypes;
        public InkCommands() {
            commands.Clear();
            var fullCommands = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(InkCommand));
            // Would use these, but the out variable for error makes it really difficult.
            // So we make the assumption there is only one constructor.
            /*if (commandTypes == null) {
                commandTypes = new[] { typeof(List<string>), typeof(System.String&) };
            }*/
            foreach (var command in fullCommands) {
                commands.Add(command.Name, command.GetConstructors()[0]);

            }
        }

        static protected Regex commandRegex;
        public void Evaluate(string commandString, List<string> tags) {
            const string commandMatch = @"^\$(?<commandName>[\w]+)";
            if (commandRegex == null) {
                commandRegex = new Regex(commandMatch);
            }

            Match match = commandRegex.Match(commandString);
            string commandName = match.Groups["commandName"].Value;
            if (commands.TryGetValue(commandName, out ConstructorInfo commandConstructor)) {
                string error = null;
                InkCommand command = (InkCommand)commandConstructor.Invoke(new object[] { tags, error });
                if (error == null && command.Update()) {
                    commandUpdates.Add(command);
                } else {
                    Debug.LogError("Error calling " + InkCommand.CommandWrite(commandName, tags) + ": " + error);
                }
            }
        }

        public void Update() {
            for (int i = commandUpdates.Count - 1; i >= 0; i--) {
                if (!commandUpdates[i].Update()) {
                    commandUpdates.RemoveAt(i);
                }
            }
        }
    }
}