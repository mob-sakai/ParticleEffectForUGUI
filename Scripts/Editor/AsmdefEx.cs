using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;

namespace Coffee.AsmdefEx
{
    internal static class ReflectionExtensions
    {
        const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        static object Inst(this object self)
        {
            return (self is Type) ? null : self;
        }

        static Type Type(this object self)
        {
            return (self as Type) ?? self.GetType();
        }

        public static object New(this Type self, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetConstructor(types)
                .Invoke(args);
        }

        public static object Call(this object self, string methodName, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetMethod(methodName, types)
                .Invoke(self.Inst(), args);
        }

        public static object Call(this object self, Type[] genericTypes, string methodName, params object[] args)
        {
            return self.Type().GetMethod(methodName, FLAGS)
                .MakeGenericMethod(genericTypes)
                .Invoke(self.Inst(), args);
        }

        public static object Get(this object self, string memberName, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetMember(memberName, FLAGS)[0];
            return mi is PropertyInfo
                ? (mi as PropertyInfo).GetValue(self.Inst(), new object[0])
                : (mi as FieldInfo).GetValue(self.Inst());
        }

        public static void Set(this object self, string memberName, object value, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetMember(memberName, FLAGS)[0];
            if (mi is PropertyInfo)
                (mi as PropertyInfo).SetValue(self.Inst(), value, new object[0]);
            else
                (mi as FieldInfo).SetValue(self.Inst(), value);
        }
    }

    internal class Settings
    {
        public bool IgnoreAccessChecks;
        public string ModifySymbols = "";

        public bool SholdChangeCompilerProcess { get { return IgnoreAccessChecks || !string.IsNullOrEmpty(ModifySymbols); } }
        public bool SholdModifyDefines { get { return IgnoreAccessChecks || !string.IsNullOrEmpty(ModifySymbols); } }

        public static Settings GetAtPath(string path)
        {
            var setting = new Settings();
            if (string.IsNullOrEmpty(path))
                return setting;

            // If input path is directory, find asmdef file.
            if (Directory.Exists(path))
                path = Directory.GetFiles(path, "*.asmdef")
                    .Select(x => x.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, ""))
                    .FirstOrDefault();

            // Not find asmdef file.
            if (string.IsNullOrEmpty(path) || !File.Exists(path) || !File.Exists(path + ".meta"))
                return setting;

            try
            {
                var json = AssetImporter.GetAtPath(path).userData;
                GetSettingssFromJson(json, out setting.IgnoreAccessChecks, out setting.ModifySymbols);
            }
            catch { }
            return setting;
        }

        public string ToJson()
        {
            return string.Format("{{\"IgnoreAccessChecks\":{0},\"ModifySymbols\":\"{1}\"}}", IgnoreAccessChecks ? "true" : "false", ModifySymbols ?? "");
        }

        public static Settings CreateFromJson(string json = "")
        {
            var setting = new Settings();
            GetSettingssFromJson(json, out setting.IgnoreAccessChecks, out setting.ModifySymbols);
            return setting;
        }

        static void GetSettingssFromJson(string json, out bool IgnoreAccessChecks, out string modifySymbols)
        {
            IgnoreAccessChecks = false;
            modifySymbols = "";
            if (string.IsNullOrEmpty(json))
                return;

            IgnoreAccessChecks = Regex.Match(json, "\"IgnoreAccessChecks\":\\s*(true|false)").Groups[1].Value == "true";
            modifySymbols = Regex.Match(json, "\"ModifySymbols\":\\s*\"([^\"]*)\"").Groups[1].Value;
        }
    }

    internal static class OpenSesameCompiler
    {
        static bool s_IsInstallFailed;
        static string s_InstallPath;
        const string k_Version = "3.4.0";
        const string k_LogHeader = "<color=#c34062><b>[OpenSesameCompiler]</b></color> ";

        static void Log(string format, params object[] args)
        {
            if (Core.LogEnabled)
                UnityEngine.Debug.LogFormat(k_LogHeader + format, args);
        }

        public static string GetInstalledPath()
        {
            if (s_IsInstallFailed)
                return "";

            if (!string.IsNullOrEmpty(s_InstallPath))
                return s_InstallPath;

            try
            {
                s_InstallPath = Install(k_Version);
            }
            catch (Exception ex)
            {
                s_IsInstallFailed = true;
                UnityEngine.Debug.LogException(new Exception(k_LogHeader + ex.Message, ex.InnerException));
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            return s_InstallPath;
        }

        static string Install(string version)
        {
            string packageId = "OpenSesameCompiler." + version;
            string url = "https://globalcdn.nuget.org/packages/" + packageId.ToLower() + ".nupkg";
            string dowloadPath = Path.GetTempFileName() + ".nuget";
            string installPath = ("Library/" + packageId).Replace('/', Path.DirectorySeparatorChar);
            string cscToolExe = (installPath + "/tools/csc.exe").Replace('/', Path.DirectorySeparatorChar);

            // OpenSesame compiler is already installed.
            if (File.Exists(cscToolExe))
            {
                Log("{0} is already installed at {1}", packageId, cscToolExe);
                return cscToolExe;
            }

            if (Directory.Exists(installPath))
                Directory.Delete(installPath, true);

            // Download csc from nuget.
            UnityEngine.Debug.LogFormat(k_LogHeader + "Download {0} from nuget: {1}", packageId, url);
            EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Download {0} from nuget", packageId), 0.2f);
            try
            {
                using (var client = new WebClient())
                    client.DownloadFile(url, dowloadPath);
            }
            catch
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback += OnServerCertificateValidation;
                    client.DownloadFile(url, dowloadPath);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidation;
            }

            // Extract zip.
            string args = string.Format("x {0} -o{1}", dowloadPath, installPath);
            string exe = Path.Combine(EditorApplication.applicationContentsPath,
                Application.platform == RuntimePlatform.WindowsEditor ? "Tools\\7z.exe" : "Tools/7za");
            UnityEngine.Debug.LogFormat(k_LogHeader + "Extract {0} to {1} with 7z command: {2} {3}", dowloadPath, installPath, exe, args);
            try
            {
                EditorUtility.DisplayProgressBar("Open Sesame Installer", string.Format("Extract {0}", dowloadPath), 0.4f);
            }
            finally
            {
                Process.Start(exe, args).WaitForExit();
            }

            if (File.Exists(cscToolExe))
                return cscToolExe;

            throw new FileNotFoundException("Open Sesame compiler is not found at " + cscToolExe);
        }

        private static bool OnServerCertificateValidation(object _, X509Certificate __, X509Chain ___, SslPolicyErrors ____)
        {
            return true;
        }
    }

    [InitializeOnLoad]
    internal static class Core
    {
        public static bool LogEnabled { get; private set; }
        public static string k_LogHeader = "<b><color=#9a4089>[AsmdefEx]</color></b> ";

        static void Log(string format, params object[] args)
        {
            if (LogEnabled)
                LogEx(format, args);
        }

        public static void LogEx(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(k_LogHeader + format, args);
        }

        public static void Error(Exception e)
        {
            UnityEngine.Debug.LogException(new Exception(k_LogHeader + e.Message, e.InnerException));
        }

        public static object GetScriptAssembly(string assemblyName)
        {
            Type tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
            Type tCSharpLanguage = Type.GetType("UnityEditor.Scripting.Compilers.CSharpLanguage, UnityEditor");
            return tEditorCompilationInterface.Call(new[] { tCSharpLanguage }, "GetScriptAssemblyForLanguage", assemblyName);
        }

        public static string[] ModifyDefines(IEnumerable<string> defines, bool ignoreAccessChecks, string modifySymbols)
        {
            var symbols = modifySymbols.Split(';', ',');
            var add = symbols.Where(x => 0 < x.Length && !x.StartsWith("!"));
            var remove = symbols.Where(x => 1 < x.Length && x.StartsWith("!")).Select(x => x.Substring(1));
            return defines
                .Union(add ?? Enumerable.Empty<string>())
                .Except(remove ?? Enumerable.Empty<string>())
                .Union(ignoreAccessChecks ? new[] { "IGNORE_ACCESS_CHECKS" } : Enumerable.Empty<string>())
                .Distinct()
                .ToArray();
        }

        public static void ModifyFiles(IEnumerable<string> files, bool ignoreAccessChecks)
        {
            const string s_If = "#if IGNORE_ACCESS_CHECKS // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.";
            const string s_EndIf = "#endif // [ASMDEFEX] DO NOT REMOVE THIS LINE MANUALLY.";

            // Add #if and #endif to all source files.
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                Log("ModifyFiles: {0} {1} {2}", file, ignoreAccessChecks, text.Contains(s_If));
                if (text.Contains(s_If) == ignoreAccessChecks)
                    continue;

                var m = Regex.Match(text, "[\r\n]+");
                if (!m.Success)
                    continue;

                var nl = m.Value;
                if (ignoreAccessChecks)
                {
                    text = s_If + nl + text + nl + s_EndIf;
                }
                else
                {
                    text = text.Replace(s_If + nl, "");
                    text = text.Replace(nl + s_EndIf, "");
                }
                Log("ModifyFiles: Write {0} {1} {2}", file, ignoreAccessChecks, text.Contains(s_If));
                File.WriteAllText(file, text);
            }
        }

        public static void ChangeCompilerProcess(object compiler, Settings setting)
        {
            Type tProgram = Type.GetType("UnityEditor.Utils.Program, UnityEditor");
            Type tScriptCompilerBase = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor");
            FieldInfo fiProcess = tScriptCompilerBase.GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);

            Log("Kill previous compiler process");
            var psi = compiler.Get("process", fiProcess).Call("GetProcessStartInfo") as ProcessStartInfo;
            compiler.Call("Dispose");

            // Convert response file for Mono to .Net.
            //   - Add preferreduilang option (en-US)
            //   - Change language version to 'latest'
            //   - Change 'debug' to 'debug:portable'
            //   - Change compiler switch prefix '-' to '/'
            string responseFile = Regex.Replace(psi.Arguments, "^.*@(.+)$", "$1");
            bool isMono = compiler.GetType().Name == "MonoCSharpCompiler";
            var text = File.ReadAllText(responseFile);
            text = Regex.Replace(text, "[\r\n]+", "\n");
            text = Regex.Replace(text, "^-", "/", RegexOptions.Multiline);

            // Modify scripting define symbols.
            if (setting.SholdModifyDefines)
            {
                Log("Modify scripting define symbols: {0}", responseFile);
                var defines = Regex.Matches(text, "^/define:(.*)$", RegexOptions.Multiline)
                        .Cast<Match>()
                        .Select(x => x.Groups[1].Value);

                text = Regex.Replace(text, "[\r\n]+/define:[^\r\n]+", "");
                foreach (var d in ModifyDefines(defines, setting.IgnoreAccessChecks, setting.ModifySymbols))
                {
                    text += "\n/define:" + d;
                }
            }

            // Add/remove '#if IGNORE_ACCESS_CHECKS' and '#endif' preprocessor.
            var files = Regex.Matches(text, "^\"(.*)\"$", RegexOptions.Multiline)
                    .Cast<Match>()
                    .Select(x => x.Groups[1].Value)
                    .Where(x => Path.GetExtension(x) == ".cs")
                    .Where(x => Path.GetFileName(x) != "AsmdefEx.cs");
            ModifyFiles(files, setting.IgnoreAccessChecks);

            // To access to non-publics in other assemblies, use OpenSesameCompiler instead of default csc.
            if (setting.IgnoreAccessChecks)
            {
                text = Regex.Replace(text, "^/langversion:\\d+$", "/langversion:latest", RegexOptions.Multiline);
                text = Regex.Replace(text, "^/debug$", "/debug:portable", RegexOptions.Multiline);
                text += "\n/preferreduilang:en-US";

                // Change exe file path.
                var cscToolExe = OpenSesameCompiler.GetInstalledPath();
                Log("Change csc tool exe to {0}", cscToolExe);
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    psi.FileName = Path.GetFullPath(cscToolExe);
                    psi.Arguments = "/shared /noconfig @" + responseFile;
                }
                else
                {
                    psi.FileName = Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/bin/mono");
                    psi.Arguments = cscToolExe + " /noconfig @" + responseFile;
                }
            }
            // Revert prefix symbols for mono compiler
            else if (isMono)
            {
                text = Regex.Replace(text, "^/", "-", RegexOptions.Multiline);
            }

            text = Regex.Replace(text, "\n", System.Environment.NewLine);
            File.WriteAllText(responseFile, text);

            Log("Restart compiler process: {0} {1}", psi.FileName, psi.Arguments);
            var program = tProgram.New(psi);
            program.Call("Start");
            compiler.Set("process", program, fiProcess);
        }

        static void OnAssemblyCompilationStarted(string name)
        {
            try
            {
                string assemblyName = Path.GetFileNameWithoutExtension(name);
                string assemblyFilename = assemblyName + ".dll";

                if (assemblyName != typeof(Core).Assembly.GetName().Name)
                    return;

                Type tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
                var compilerTasks = tEditorCompilationInterface.Get("Instance").Get("compilationTask").Get("compilerTasks") as IDictionary;
                var scriptAssembly = compilerTasks.Keys.Cast<object>().FirstOrDefault(x => (x.Get("Filename") as string) == assemblyFilename);

                // Should change compiler process for the assembly?
                var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
                var setting = Settings.GetAtPath(asmdefPath);
                if (!setting.SholdChangeCompilerProcess)
                    return;

                // Create new compiler to recompile.
                Log("Assembly compilation started: <b>{0} should be recompiled.</b>", assemblyName);
                Core.ChangeCompilerProcess(compilerTasks[scriptAssembly], setting);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(new Exception(k_LogHeader + e.Message, e.InnerException));
            }
        }

        static Core()
        {
            var assemblyName = typeof(Core).Assembly.GetName().Name;
            if (assemblyName == "Coffee.AsmdefEx")
                return;

            k_LogHeader = string.Format("<b><color=#9a4089>[AsmdefEx ({0})]</color></b> ", assemblyName);
            LogEnabled = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Split(';', ',')
                .Any(x => x == "ASMDEF_EX_LOG");

            Log("Start watching assembly '{0}' compilation.", typeof(Core).Assembly.GetName().Name);
            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
        }
    }

#if !IGNORE_ACCESS_CHECKS
    [InitializeOnLoad]
    internal class RecompileRequest
    {
        static RecompileRequest()
        {
            var assemblyName = typeof(RecompileRequest).Assembly.GetName().Name;
            if (assemblyName == "Coffee.AsmdefEx")
                return;

            var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            if (Core.LogEnabled)
                UnityEngine.Debug.LogFormat("<b>Request to recompile: {0} ({1})</b>", assemblyName, asmdefPath);
            AssetDatabase.ImportAsset(asmdefPath);
        }
    }
#endif
}