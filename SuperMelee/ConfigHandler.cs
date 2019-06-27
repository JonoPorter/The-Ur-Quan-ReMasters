#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using System.CodeDom.Compiler;
using System.CodeDom;
using Physics2D;
using Microsoft.CSharp;
using System.Text.RegularExpressions;

using System.Security;
using System.Security.Permissions;
using System.Security.Cryptography;
namespace ReMasters.SuperMelee
{


    class ConfigHandler
    {
        static SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
        static CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
        static List<Assembly> assemblies;
        static string[] AssemblyInfo = new string[] { 
            "using System.Security.Permissions;",
            "[assembly: EnvironmentPermission(SecurityAction.RequestRefuse, Unrestricted=true)]",
            //"[assembly: FileIOPermission(SecurityAction.RequestRefuse, Unrestricted=true)]",
            "[assembly: FileDialogPermission(SecurityAction.RequestRefuse, Unrestricted=true)]",
            "[assembly: ReflectionPermission(SecurityAction.RequestRefuse, Unrestricted=true)]",
            "[assembly: RegistryPermission(SecurityAction.RequestRefuse, Unrestricted=true)]",
            "[assembly: SecurityPermission(SecurityAction.RequestRefuse)]",
            //"[assembly: UIPermission(SecurityAction.RequestRefuse, Unrestricted=true)]"
        };
        static string dot = @"[\.(namespace )]\s*?";
        static string dotorsem = @"\s*?[\.;\{]";
        static string[] DisallowedNamespaces = new string[]{
            dot+"CodeDom"+dotorsem,
            dot+"ComponentModel"+dotorsem,
            dot+"Configuration"+dotorsem,
            dot+"Deployment"+dotorsem,
            dot+"Diagnostics"+dotorsem,
            dot+"IO"+dotorsem,
            dot+"Net"+dotorsem,
            dot+"Reflection"+dotorsem,
            dot+"Resources"+dotorsem,
            dot+"CompilerServices"+dotorsem,
            dot+"ConstrainedExecution"+dotorsem,
            dot+"Hosting"+dotorsem,
            dot+"InteropServices"+dotorsem,
            dot+"Remoting"+dotorsem,
            dot+"Versioning"+dotorsem,
            dot+"Security"+dotorsem,
            dot+"Threading"+dotorsem,
            dot+"Timers"+dotorsem,
            dot+"Web"+dotorsem,
            dot+"Xml"+dotorsem,
            "Microsoft"+dotorsem,
        };
        static string[] assemblyNames = new string[] {
            "System.dll", 
            "System.Drawing.dll",
            "System.Xml.dll",
            "AdvanceSystem.dll",
            "AdvanceMath.dll", 
            "AdvanceMath.Geometry2D.dll", 
            "Physics2D.dll", 
            "ReMasters.SuperMelee.dll"
        };
        public static List<ShipLoader> GetShips()
        {
            List<ShipLoader> returnvalues = new List<ShipLoader>();

            DirectoryInfo info = new DirectoryInfo(SuperMeleePaths.ShipDir);
            if (info.Exists)
            {
                string AssemblyInfoPath = Path.GetTempFileName();
                File.WriteAllLines(AssemblyInfoPath, AssemblyInfo);
                CompilerParameters parameters = new CompilerParameters(assemblyNames);
                foreach (DirectoryInfo dirinfo in info.GetDirectories())
                {
                    FileInfo[] fileinfos = dirinfo.GetFiles("*.cs");
                    if (fileinfos.Length > 0)
                    {
#if Release
                        try
                        {
#endif
                        string sourceFile = fileinfos[0].FullName;
                        string directory = dirinfo.FullName + Path.DirectorySeparatorChar;
                        string shipsName = dirinfo.Name.Replace(Path.DirectorySeparatorChar,'.');

                        string infoFile = directory + "CompiledInfo.txt";
                        string binFile = directory + shipsName+".dll";
                        string errorFile = directory + "CompilerErrors.txt";
                        string configFile = directory + ShipLoader.ConfigFileName;
                        /*string infoFile = Path.ChangeExtension(sourceFile, ".CompiledInfo.txt");
                        string binFile = Path.ChangeExtension(sourceFile, ".dll");
                        string errorFile = Path.ChangeExtension(sourceFile, ".Error.txt");*/



                        if (File.Exists(infoFile))
                        {
                            Console.WriteLine("Loading " + shipsName);
                            try
                            {
                                string[] CompiledInfo = File.ReadAllLines(infoFile);
                                if (CompiledInfo.Length > 0 &&
                                    File.Exists(binFile) &&
                                    CompiledInfo[0] == GetHash(sourceFile, binFile))
                                {
                                    ShipLoader loader = GetShipFromAssembly(Assembly.LoadFile(binFile));
                                    loader.ShipDirectory = dirinfo.FullName + Path.DirectorySeparatorChar;
                                    returnvalues.Add(loader);
                                    continue;
                                }
                            }
                            catch (UnauthorizedAccessException) { }
                            Console.WriteLine("Loading Failed");
                            File.Delete(infoFile);
                        }
                        if (CheckNamespaceUse(sourceFile, errorFile))
                        {
                            Console.WriteLine("Compiling " + shipsName);
                            parameters.OutputAssembly = binFile;
                            if (File.Exists(binFile))
                            {
                                File.Delete(binFile);
                            }
                            if (File.Exists(configFile))
                            {
                                File.Delete(configFile);
                            }
                            string[] paths = new string[fileinfos.Length + 1];
                            paths[0] = AssemblyInfoPath;
                            for (int pos = 0; pos < fileinfos.Length; ++pos)
                            {
                                paths[pos + 1] = fileinfos[pos].FullName;
                            }
                            CompilerResults results = provider.CompileAssemblyFromFile(parameters, paths);
                            if (results.Errors.Count > 0)
                            {
                                using (StreamWriter writer = new StreamWriter(errorFile))
                                {
                                    foreach (CompilerError error in results.Errors)
                                    {
                                        writer.WriteLine("{0} {1} Description: {2} File: {3} Line: {4} Column: {5}",
                                            (error.IsWarning) ? ("Warning") : ("Error"),
                                            error.ErrorNumber,
                                            error.ErrorText,
                                            error.FileName,
                                            error.Line,
                                            error.Column);
                                    }
                                }
                                continue;
                            }
                            ShipLoader loader = GetShipFromAssembly(results.CompiledAssembly);
                            loader.ShipDirectory = dirinfo.FullName + Path.DirectorySeparatorChar;
                            returnvalues.Add(loader);

                            File.WriteAllLines(infoFile, new string[] { GetHash(sourceFile, binFile) });
                            File.SetAttributes(infoFile, File.GetAttributes(infoFile) | FileAttributes.Hidden);
                            File.SetAttributes(binFile, File.GetAttributes(binFile) | FileAttributes.Hidden);
                            if (File.Exists(errorFile))
                            {
                                File.Delete(errorFile);
                            }
                        }
#if Release
                        }
                        catch (Exception ex)
                        {
                            ErrorBox.DisplayError(ex);
                        }
#endif
                    }
                }
                File.Delete(AssemblyInfoPath);
            }
            return returnvalues;
        }
        static ShipLoader GetShipFromAssembly(Assembly assembly)
        {
            ShipLoader rv = null;
            FileIOPermission permission = new FileIOPermission(PermissionState.Unrestricted);
            permission.AllFiles = FileIOPermissionAccess.AllAccess;
            permission.Deny();
            bool isUsed = false;
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsAbstract && type.IsPublic && (type.IsSubclassOf(typeof(ShipLoader)) || type == typeof(ShipLoader)))
                {
                    rv = (ShipLoader)(type.GetConstructor(Type.EmptyTypes).Invoke(null));
                    break;
                }
            }
            if (isUsed)
            {
                if (assemblies == null)
                {
                    assemblies = new List<Assembly>();
                    System.AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
                }
                assemblies.Add(assembly);
            }
            return rv;
        }
        static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name == args.Name)
                {
                    return assembly;
                }
            }
            return null;
        }
        static string GetHash(string SourceName, string AssemblyName)
        {
            string hash = Environment.MachineName + Environment.UserDomainName + Environment.UserName + Environment.NewLine + Environment.Version + Environment.OSVersion + Environment.CurrentDirectory + Environment.ProcessorCount + Path.GetTempPath();
            hash = BitConverter.ToString(hasher.ComputeHash(System.Text.UnicodeEncoding.Unicode.GetBytes(hash))).Replace("-", "");
            using (BufferedStream stream = new BufferedStream(File.OpenRead(SourceName)))
            {
                hash += BitConverter.ToString(hasher.ComputeHash(stream)).Replace("-", "");
            }
            using (BufferedStream stream = new BufferedStream(File.OpenRead(AssemblyName)))
            {
                hash += BitConverter.ToString(hasher.ComputeHash(stream)).Replace("-", "");
            }
            return BitConverter.ToString(hasher.ComputeHash(System.Text.UnicodeEncoding.Unicode.GetBytes(hash))).Replace("-", ""); ;
        }
        static bool CheckNamespaceUse(string filename, string errorFile)
        {
            bool returnvalue = true;
            string sourcecode = File.ReadAllText(filename);
            foreach (string DisallowedNamespace in DisallowedNamespaces)
            {
                if (Regex.IsMatch(sourcecode, DisallowedNamespace))
                {
                    returnvalue = false;
                    using (StreamWriter writer = new StreamWriter(errorFile, true))
                    {
                        writer.WriteLine("The use of the namespace System" + DisallowedNamespace.Replace(dot, ".").Replace(dotorsem, "") + " is not allowed.");
                    }
                }
            }
            return returnvalue;
        }
    }
}

