using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Linq;

namespace scandir
{
	class Program
	{
		public static List<string> LOGS = new List<string>();
		public static void Main(string[] args)
		{
			Console.WriteLine("╔═════════╗");
			Console.WriteLine("║ scandir ║");
			Console.WriteLine("╚═════════╝");
			if (args.Length == 0)
				printHelp();
			else if (args.Length == 1){
				string[] helpers = {"/?", "help", "?"};
				string[] abouters = {"about", "/!", "!", "info"};
				if (helpers.Any(args[0].Contains))
					printHelp();
				if (abouters.Any(args[0].Contains))
					printInfo();
				
			}
			else {
				Console.Write("Scanning Directory...");
				if ( processArgs(args) )
					Console.WriteLine("DONE");
				if ( LOGS.Count > 0 ){
					Console.Write("Files Found! Writing to log...");
					StreamWriter sw = new StreamWriter(@"" + args[2]);
					sw.WriteLine("filename,fileowner,usersid,lastmodified");
					foreach (string s in LOGS)
						sw.WriteLine(s);
					sw.Close();
					Console.WriteLine("DONE");
				} else Console.WriteLine("No files found.");
			}
		}
		
		static bool processArgs(string[] args)
		{
			bool dirPass = Directory.Exists(args[1]),
				 logPass = !File.Exists(args[2]);
			
			if ( !dirPass )
				Console.WriteLine("\n[ERROR] Invalid Directory: \"" + args[1] + "\" is not a directory.");
			if ( !logPass )
				Console.WriteLine("\n[ERROR] Invalid Path: \"" + args[2] + "\" already exists.");
			if ( dirPass && logPass ){
				string[] files = Directory.GetFiles(args[1],args[0], SearchOption.AllDirectories);
				foreach ( string file in files ){
					var fs = File.GetAccessControl(file);
					string message = file + "," + //filepath
						fs.GetOwner(typeof(SecurityIdentifier)) + "," + //owner
						fs.GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) + "," + //owner sid
						File.GetLastWriteTime(file).ToString("dd/MM/yy HH:mm:ss"); //last modified time
					
					LOGS.Add(message);
				}
				return true;
         	}
			return false;				
		}
		
		static void printHelp()
		{
			Console.WriteLine("To find out more info about scandir, run \"scandir about\".\n\n");
			Console.WriteLine("Syntax: scandir <search> <dir> <output>\n\nArguments:\n");
			Console.WriteLine("<search>  A string search pattern to use when scanning a directory where * is a wildcard. For example, to scan for all exe files, use \"*.exe\" as your search filter.\n");
			Console.WriteLine("<dir>  The directory to start to scan in.\n");
			Console.WriteLine("<output>  The location to ouput a log file if results are found. Results are saved in a CSV format.\n");
		}
				
		static void printInfo()
		{
			Console.WriteLine("scandir is a utility to scan a directory for a given search pattern and return the results in a CSV file.");
			Console.WriteLine("For syntax and usage, please run \"scandir help\".");
			Console.WriteLine("scandir is an open source project, with source and license available at <https://github.com/guythundar/scandir>.");
		}
	}
}