﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    private const int LOG_RETENTION_DAYS = 7;
    private const string LAUNCHER_LOG_PREFIX = "launcher_";
    private const string SERVER_LOG_PREFIX = "server_";

    static void Main(string[] args)
    {
        Console.Title = "Whackerlink Console";
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        string logsDir = Path.Combine(currentDir, "logs");

        InitializeLogging(logsDir);

        try
        {
            RunMainLogic(currentDir, logsDir);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRASH] {ex}");
        }
        finally
        {
            Console.WriteLine("\nPress any key to close this window...");
            Console.ReadKey();
        }
    }

    static void InitializeLogging(string logsDir)
    {
        try
        {
            Directory.CreateDirectory(logsDir);

            foreach (var file in Directory.GetFiles(logsDir, "*.txt")
                     .Where(f => File.GetCreationTime(f) < DateTime.Now.AddDays(-LOG_RETENTION_DAYS)))
            {
                File.Delete(file);
            }

            string logPath = Path.Combine(logsDir, $"{LAUNCHER_LOG_PREFIX}{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(logPath, $"[{GetTimestamp()}] Launcher started\n");

            Console.SetOut(new DualTextWriter(Console.Out, logPath));
        }
        catch
        {
            Console.WriteLine("[WARNING] Failed to initialize logging system");
        }
    }

    static void RunMainLogic(string currentDir, string logsDir)
    {
        Console.WriteLine("=== WHACKERLINK SERVER LAUNCHER ===");

        
        try
        {
            var version = Environment.Version;
            Console.WriteLine($".NET Version: {version}");
        }
        catch
        {
            Console.WriteLine("ERROR: Install .NET 8 Runtime first!\nDownload: https://dotnet.microsoft.com");
            return;
        }

        
        string exePath = Path.Combine(currentDir, "WhackerLinkServer.exe");
        string? configPath = FindConfigFile(currentDir);

        if (configPath == null || !FileExists(exePath))
        {
            return;
        }

        
        ShowLatestServerLog(logsDir);

        
        Console.WriteLine("\nStarting WhackerLinkServer...");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"-c \"{configPath}\"",
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };

        process.Start();
        Console.WriteLine($"Server started (using config: {Path.GetFileName(configPath)})");
        Console.WriteLine("\nThanks for using Whackerlink!");
        Console.WriteLine("\nThe server is now running. Close this window to terminate it.");

        process.WaitForExit();
        Console.WriteLine("\nServer Stopped. Press any key to close...");
        Console.ReadKey();
    }

    static string? FindConfigFile(string currentDir)
    {
        string configDir = Path.Combine(currentDir, "configs");
        string[] excludedFiles = { "auth_keys.yml", "rid_acl.yml" };

        try
        {
            if (!Directory.Exists(configDir))
            {
                Console.WriteLine($"\nERROR: Config directory not found: {configDir}");
                return null;
            }

            
            string primaryConfig = Path.Combine(configDir, "config.yml");
            if (File.Exists(primaryConfig))
            {
                return primaryConfig;
            }

            
            var configFiles = Directory.GetFiles(configDir, "*.yml")
                .Where(f => !excludedFiles.Contains(Path.GetFileName(f), StringComparer.OrdinalIgnoreCase))
                .OrderBy(f => Path.GetFileName(f))
                .ToList();

            if (configFiles.Count == 0)
            {
                Console.WriteLine($"\nERROR: No valid config files found in {configDir}");
                Console.WriteLine("Excluded files: " + string.Join(", ", excludedFiles));
                return null;
            }

            
            if (configFiles.Count > 1)
            {
                Console.WriteLine($"\nNOTICE: Multiple config files found, using {Path.GetFileName(configFiles[0])}");
                Console.WriteLine("Available configs: " + string.Join(", ", configFiles.Select(Path.GetFileName)));
            }

            return configFiles[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nERROR: Failed to search for config files: {ex.Message}");
            return null;
        }
    }

    static bool FileExists(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }

        Console.WriteLine($"\nERROR: File not found - {path}");
        Console.WriteLine($"Current directory: {Path.GetDirectoryName(path)}");
        return false;
    }

    static void ShowLatestServerLog(string logsDir)
    {
        try
        {
            var latestServerLog = Directory.GetFiles(logsDir, $"{SERVER_LOG_PREFIX}*.txt")
                .OrderByDescending(f => f)
                .FirstOrDefault();

            if (latestServerLog != null)
            {
                Console.WriteLine($"\nLatest server log: {Path.GetFileName(latestServerLog)}");
                Console.WriteLine($"Created: {File.GetCreationTime(latestServerLog)}");
            }
        }
        catch
        {
            Console.WriteLine("[DEBUG] No server logs found");
        }
    }

    public static string GetTimestamp() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
}

class DualTextWriter : TextWriter
{
    private readonly TextWriter _original;
    private readonly string _logPath;

    public DualTextWriter(TextWriter original, string logPath)
    {
        _original = original;
        _logPath = logPath;
    }

    public override void WriteLine(string? value)
    {
        _original.WriteLine(value);
        try
        {
            if (value != null)
            {
                File.AppendAllText(_logPath, $"[{Program.GetTimestamp()}] {value}\n");
            }
        }
        catch { /* Silently fail if logging fails */ }
    }

    public override Encoding Encoding => _original.Encoding;
}