// LibPdInstance.cs - Unity integration of libpd, supporting multiple instances.
// -----------------------------------------------------------------------------
// Copyright (c) 2018 Niall Moody
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// -----------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Unity Component for running a Pure Data patch. Uses libpd's new multiple
/// instance support, so you can run multiple LibPdInstances in your scene.
/// 
/// <para>
/// Pd patches should be stored in Assets/StreamingAssets/PdAssets and assigned
/// to LibPdInstance in the inspector (type the patch name (minus .pd) into the
/// Patch text box).
/// </para>
/// </summary>
/// 
/// <remarks>
/// This uses the basic c version of libpd over the C# bindings, as I was unable
/// to get the C# bindings working with Unity. This is likely due to my own
/// inexperience with C# (I'm primarily a C++ programmer), rather than an issue
/// with the lipbd C# bindings themselves.
/// 
/// Along those lines, I modelled parts of this class after the C# bindings, so
/// you will likely see some duplicated code.
/// 
/// Also, as it stands, this requires a small patch to z_libpd.c to allow us to
/// install our own print hook (so we can pipe print messages to Unity's
/// Console). Unfortunately, libpd requires the print hook to be set up before
/// libpd_init() is called, and will not accept any changes after that.
/// 
/// This causes major problems with Unity, as we want to set the print hook when
/// we start our game, and clear it when the game exits. However, because Unity
/// keeps native dlls active as long as the editor is running, libpd_init()
/// (being a one-time function) will effectively never get called again, and the
/// print hook will remain set to the value set when we first ran our game from
/// the editor. The result: if we try to run our game from the editor a second
/// time, we crash the entire editor.
/// 
/// If you're building libpd from source yourself, you can get around this issue
/// by adding the following line to the end of z_libpd.c: libpd_set_printhook():
/// 
/// sys_printhook = libpd_printhook;
/// 
/// </remarks>
public class LibPdInstance : MonoBehaviour {

	#region libpd imports
	//--------------------------------------------------------------------------
	// libpd functions that we need to be able to call from C#.
	[DllImport("libpd")]
	private static extern int libpd_init();

	[DllImport("libpd")]
	private static extern IntPtr libpd_new_instance();

	[DllImport("libpd")]
	private static extern void libpd_set_instance(IntPtr instance);

	[DllImport("libpd")]
	private static extern void libpd_free_instance(IntPtr instance);

	[DllImport("libpd")]
	private static extern int libpd_init_audio(int inChans, int outChans, int sampleRate);

	[DllImport("libpd")]
	private static extern IntPtr libpd_openfile([In] [MarshalAs(UnmanagedType.LPStr)] string basename,
												[In] [MarshalAs(UnmanagedType.LPStr)] string dirname);

	[DllImport("libpd")]
	private static extern void libpd_closefile(IntPtr p);

	[DllImport("libpd")]
	private static extern int libpd_process_float(int ticks,
												  [In] float[] inBuffer,
												  [Out] float[] outBuffer);

	[DllImport("libpd")]
	private static extern int libpd_blocksize();

	[DllImport("libpd")]
	private static extern int libpd_start_message(int max_length);

	[DllImport("libpd")]
	private static extern void libpd_add_float(float x);

	[DllImport("libpd")]
	private static extern void libpd_add_symbol([In] [MarshalAs(UnmanagedType.LPStr)] string sym);

	[DllImport("libpd")]
	private static extern int libpd_finish_list([In] [MarshalAs(UnmanagedType.LPStr)] string recv);

	[DllImport("libpd")]
	private static extern int libpd_finish_message([In] [MarshalAs(UnmanagedType.LPStr)] string recv,
												   [In] [MarshalAs(UnmanagedType.LPStr)] string msg);

	[DllImport("libpd")]
	private static extern int libpd_bang([In] [MarshalAs(UnmanagedType.LPStr)] string recv);

	[DllImport("libpd")]
	private static extern int libpd_float([In] [MarshalAs(UnmanagedType.LPStr)] string recv,
										  float x);

	[DllImport("libpd")]
	private static extern int libpd_symbol([In] [MarshalAs(UnmanagedType.LPStr)] string recv,
										   [In] [MarshalAs(UnmanagedType.LPStr)] string sym);

	[DllImport("libpd")]
	private static extern int libpd_exists([In] [MarshalAs(UnmanagedType.LPStr)] string obj);

	[DllImport("libpd")]
	private static extern IntPtr libpd_bind([In] [MarshalAs(UnmanagedType.LPStr)] string symbol);

	[DllImport("libpd")]
	private static extern void libpd_unbind(IntPtr binding);

	[DllImport("libpd")]
	private static extern void libpd_set_verbose(int verbose);
	#endregion
	
	#region delegate/libpd callback declarations
	//--------------------------------------------------------------------------
	// Delegates/libpd callbacks.

	//-Print hook---------------------------------------------------------------
	// We don't make the print hook publicly available (for now), so it's just a
	//single static delegate.

	// Delegate/function pointer type.
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void LibPdPrintHook([In] [MarshalAs(UnmanagedType.LPStr)] string message);

	// libpd function for setting the hook.
	[DllImport("libpd")]
	private static extern void libpd_set_printhook(LibPdPrintHook hook);

	// Instance of the print hook, kept to ensure it doesn't get garbage
	// collected.
	private static LibPdPrintHook printHook;

	//-Bang hook----------------------------------------------------------------
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void LibPdBangHook([In] [MarshalAs(UnmanagedType.LPStr)] string symbol);
	
	[DllImport("libpd")]
	private static extern void libpd_set_banghook(LibPdBangHook hook);
	
	private LibPdBangHook bangHook;

	// Public delegate for receiving bang events.
	public delegate void LibPDBang(string receiver);
	// Bang event; subscribe to this to receive bangs.
	public event LibPDBang Bang = delegate{};

	//-Float hook---------------------------------------------------------------
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void LibPdFloatHook([In] [MarshalAs(UnmanagedType.LPStr)] string symbol,
										float val);
	
	[DllImport("libpd")]
	private static extern void libpd_set_floathook(LibPdFloatHook hook);
	
	private LibPdFloatHook floatHook;

	// Public delegate for receiving float events.
	public delegate void LibPDFloat(string receiver, float val);
	// Bang event; subscribe to this to receive floats.
	public event LibPDFloat Float = delegate{};

	//-Symbol hook--------------------------------------------------------------
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void LibPdSymbolHook([In] [MarshalAs(UnmanagedType.LPStr)] string symbol,
										 [In] [MarshalAs(UnmanagedType.LPStr)] string val);
	
	[DllImport("libpd")]
	private static extern void libpd_set_symbolhook(LibPdSymbolHook hook);
	
	private LibPdSymbolHook symbolHook;

	// Public delegate for receiving symbol events.
	public delegate void LibPDSymbol(string receiver, string val);
	// Bang event; subscribe to this to receive symbols.
	public event LibPDSymbol Symbol = delegate{};
	#endregion

	#region member variables
	//--------------------------------------------------------------------------
	// The Pd patch this instance is running.
	public string patch;

	// Hacky way of making pipePrintToConsoleStatic visible in the inspector.
	public bool pipePrintToConsole = false;
	// Set to true to pipe any Pure Data *print* messages to Unity's console.
	public static bool pipePrintToConsoleStatic = false;

	// Our pointer to the Pd patch this instance is running.
	IntPtr patchPointer;

	// The Pd instance we're using.
	private IntPtr instance;
	// The number of ticks to process at a time.
	private int numTicks;

	// Any bindings we have for this patch.
	private Dictionary<string, IntPtr> bindings;

	// True if we were unable to intialise Pd's audio processing.
	private bool pdFail = false;
	// True if we were unable to open our patch.
	private bool patchFail = false;

	// Global variable used to ensure we don't initialise LibPd more than once.
	private static bool pdInitialised = false;
	#endregion
	
	#region MonoBehaviour methods
	//--------------------------------------------------------------------------
	// Initialise LibPd.
	void Awake ()
	{
		// Initialise libpd, if it's not already.
		if(!pdInitialised)
		{
			// Setup hooks.
			printHook = new LibPdPrintHook(PrintOutput);
			libpd_set_printhook(printHook);
			
			bangHook = new LibPdBangHook(BangOutput);
			libpd_set_banghook(bangHook);
			
			floatHook = new LibPdFloatHook(FloatOutput);
			libpd_set_floathook(floatHook);
			
			symbolHook = new LibPdSymbolHook(SymbolOutput);
			libpd_set_symbolhook(symbolHook);

			// Initialise libpd if possible, report any errors.
			int initErr = libpd_init();
			if(initErr != 0)
			{
				Debug.LogError("Warning; libpd_init() returned " + initErr);
				Debug.LogError("(if you're running this in the editor that probably just means this isn't the first time you've run your game, and is not a problem)");
			}
			pdInitialised = true;

			// Make sure our static pipePrintToConsole variable is set
			// correctly.
			pipePrintToConsoleStatic = pipePrintToConsole;
		}

		// Calc numTicks.
		int bufferSize;
		int noOfBuffers;

		AudioSettings.GetDSPBufferSize (out bufferSize, out noOfBuffers);
		numTicks = bufferSize/libpd_blocksize();

		// Create our instance.
		instance = libpd_new_instance();

		// Set our instance.
		libpd_set_instance(instance);

		// Initialise audio.
		int err = libpd_init_audio(2, 2, AudioSettings.outputSampleRate);
		if (err != 0)
		{
			pdFail = true;
			Debug.LogError(gameObject.name + ": Could not initialise Pure Data audio. Error = " + err);
		}
		else
		{
			if(patch == String.Empty)
			{
				Debug.LogError(gameObject.name + ": No patch was assigned to this LibPdInstance.");
				patchFail = true;
			}
			else
			{
				//Create our bindings dictionary.
				bindings = new Dictionary<string, IntPtr>();

				// Open our patch.
				patchPointer = libpd_openfile(patch + ".pd", Application.streamingAssetsPath + "/PdAssets/");
				if(patchPointer == IntPtr.Zero)
				{
					Debug.LogError(gameObject.name + ": Could not open " + Application.streamingAssetsPath + "/PdAssets/" + patch + ".pd");
					patchFail = true;
				}

				// Turn on audio processing.
				libpd_start_message(1);
				libpd_add_float(1.0f);
				libpd_finish_message("pd", "dsp");
			}
		}
	}
	
	//--------------------------------------------------------------------------
	// Close the patch file on quit.
	void OnApplicationQuit ()
	{
		if(!pdFail && !patchFail)
		{
			libpd_set_instance(instance);

			libpd_start_message(1);
			libpd_add_float(0.0f);
			libpd_finish_message("pd", "dsp");

			if(printHook != null)
			{
				printHook = null;
				libpd_set_printhook(printHook);
			}

			foreach(var ptr in bindings.Values)
				libpd_unbind(ptr);
			bindings.Clear();

			libpd_closefile(patchPointer);
		}
	}
	
	//--------------------------------------------------------------------------
	// Process audio.
	void OnAudioFilterRead (float[] data, int channels)
	{
		if(!pdFail && !patchFail)
		{
			libpd_set_instance(instance);
			libpd_process_float(numTicks, data, data);
		}
	}
	#endregion
	
	#region public methods
	//--------------------------------------------------------------------------
	// Bind to a named object in the patch.
	public void Bind (string symbol)
	{
		libpd_set_instance(instance);
		IntPtr ptr = libpd_bind(symbol);
		bindings.Add(symbol, ptr);
	}

	//--------------------------------------------------------------------------
	// Release an existing binding.
	public void UnBind (string symbol)
	{
		libpd_set_instance(instance);
		libpd_unbind(bindings[symbol]);
		bindings.Remove(symbol);
	}

	//--------------------------------------------------------------------------
	// Send a bang to the named receive object.
	public void SendBang (string receiver)
	{
		libpd_set_instance(instance);
	
		int err = libpd_bang(receiver);
	
		if(err == -1)
			Debug.Log(gameObject.name + "::SendBang(): Could not find " + receiver + " object.");
	}

	//--------------------------------------------------------------------------
	// Send a float to the named receive object.
	public void SendFloat(string receiver, float val)
	{
		libpd_set_instance(instance);
	
		int err = libpd_float(receiver, val);
	
		if(err == -1)
			Debug.Log(gameObject.name + "::SendFloat(): Could not find " + receiver + " object.");
	}

	//--------------------------------------------------------------------------
	// Send a symbol to the named receive object.
	public void SendSymbol(string receiver, string symbol)
	{
		libpd_set_instance(instance);
	
		int err = libpd_symbol(receiver, symbol);
	
		if(err == -1)
			Debug.Log(gameObject.name + "::SendSymbol(): Could not find " + receiver + " object.");
	}
	#endregion

	#region delegate definitions
	//--------------------------------------------------------------------------
	// Receive print messages.
	static void PrintOutput(string message)
	{
		if(pipePrintToConsoleStatic)
			Debug.Log("libpd: " + message);
	}

	//--------------------------------------------------------------------------
	// Receive bang messages.
	void BangOutput(string symbol)
	{
		Bang(symbol);
	}

	//--------------------------------------------------------------------------
	// Receive float messages.
	void FloatOutput(string symbol, float val)
	{
		Float(symbol, val);
	}

	//--------------------------------------------------------------------------
	// Receive symbol messages.
	void SymbolOutput(string symbol, string val)
	{
		Symbol(symbol, val);
	}
	#endregion
}
