using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAngband.Tests {
	static class UnitTest_Main {
		static bool verbose = true;
		static List<Suite> Suites = new List<Suite>();

		public static void RegisterTests() {
			//Artifact
			Randname_Test.Register();

			//Command
			Lookup_Test.Register();

			//Monster
			Monster_Test.Register();
			MonsterAttack_Test.Register();

			//Object
			ObjectAttack_Test.Register();

			//Parse
			A_Info_Test.Register();
			C_Info_Test.Register();
			E_Info_test.Register();
			F_Info_Test.Register();
			Flavor_Test.Register();
			H_Info_Test.Register();
			K_Info_Test.Register();
			Names_Test.Register();
			Owner_Test.Register();
			P_Info_Test.Register();
			Parser_Test.Register();
			ParseStore_Test.Register();
			R_Info_Test.Register();
			S_Info_Test.Register();
			V_Info_Test.Register();
			Z_Info_Test.Register();

			//Pathfind
			Pathfind_Test.Register();

			//Player
			Birth_Test.Register();
			History_Test.Register();
			Player_Test.Register();

			//Store
			Store_Test.Register();

			//Trivial
			Trivial_Test.Register();
			
			//Quark
			Quark_Test.Register();

			//Textblock
			Textblock_Test.Register();
		}

		public static void Main(String[] args) {
			RegisterTests();

			if (args[0].Equals("-v")) {
				verbose = true;
				Console.Out.Write("Initiating Test Suites...\n\n");
			}

			for(int s = 0; s < Suites.Count; s++) {
				int passed = 0;

				Suite suite = Suites[s];

				if(verbose) {
					Console.Out.Write(suite.Name + ": starting...\n");
					Console.Out.Flush();
				}

				
				try {
					suite.Setup();
				} catch (Exception ex) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Out.Write("ERROR: Setup Failed, Reason: " + ex.Message + "\n\n");
					Console.ForegroundColor = ConsoleColor.Gray;
					continue;
				}

				for(int i = 0; i < suite.Tests.Count; i++) {
					if(verbose)
						Console.Out.Write("  " + suite.Tests[i].Name + "  ");
					Console.Out.Flush();
					try {
						suite.Tests[i].Function();
						passed++;
						ShowPass();
					} catch(Success) {
						passed++;
						ShowPass();
					} catch(Failure f){
						ShowFail(f.info);
					} catch(Exception ex) {
						ShowFail(ex.Message);
						continue;
					}
					Console.Out.Flush();
				}

				try {
					suite.Teardown();
				} catch (Exception ex){
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Out.Write("ERROR: Teardown Failed, Reason: " + ex.Message + "\n\n");
					Console.ForegroundColor = ConsoleColor.Gray;
					continue;
				}

				Console.Out.Write(suite.Name + " finished: " + passed + "/" + suite.Tests.Count + " passed\n\n");
			}

			Console.Out.Write("\nAll testing complete [Press Enter]\n");
			Console.In.ReadLine();
		}

		public static void AddSuite(Suite s) {
			Suites.Add(s);
		}

		public class Success : Exception {

		}

		public class Failure : Exception {
			public string info = null;

			public Failure(string msg = null) {
				info = msg;
			}
		}

		public static bool ShowPass() {
			if(verbose) {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Out.Write("Passed\n");
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			return true;
		}

		public static bool ShowFail(string Reason = null) {
			if(verbose) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Out.Write("Failed");
				if (Reason != null){
					Console.Out.Write(" - " + Reason);
				}
				Console.Out.Write("\n");
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			return false;
		}
	}
}
