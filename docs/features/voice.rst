Voice
=====

|outdated|  

|incomplete|  

|breaking|

.. warning::
	
	Audio in 1.0 is not complete. Most of the documentation below is untested.

Installation
------------

Before setting up the AudioService, you must first install the package `from NuGet`_ or `GitHub`_.

Add the package to your solution, and then import the namespace ``Discord.Audio``.

You will also need libsodium for voice encryption. You must acquire a compiled binary of libsodium, and place it in the directory your project runs from. A precompiled binary is also `available here`_. You may also find 64-bit/linux releases `from here`_.

Optionally, Discord.Audio also relies on the Opus Audio Library for encoding audio. Opus binaries are only necessary if you will be sending PCM data to Discord. You must acquire a compiled binary of Opus, and place it in the directory your project runs from (/ on dnx, /bin/debug on net45). A 32-bit binary is `available for your convienence`_.

.. _from NuGet: https://www.nuget.org/packages/Discord.Net.Audio/0.9.0-rc3
.. _GitHub: https://github.com/RogueException/Discord.Net/tree/master/src/Discord.Net.Audio
.. _available for your convienence: https://github.com/RogueException/Discord.Net/blob/master/src/Discord.Net.Audio/opus.dll
.. _available here: https://github.com/RogueException/Discord.Net/blob/master/src/Discord.Net.Audio/libsodium.dll
.. _from here: https://download.libsodium.org/libsodium/releases/

Setup
-----

|updated-sec|

To use audio, you must configure your DiscordSocketClient to use Audio.

.. code-block:: csharp6
	
	_client = new DiscordSocketClient(new DiscordSocketConfig() { AudioMode = AudioMode.Outgoing });

For most music bots, you will only need ``AudioMode.Outgoing``. If you need to receive audio, set your AudioMode to ``AudioMode.Incoming`` or ``AudioMode.Both``.

Joining a Channel
-----------------

|updated-sec|

Joining Voice Channels is pretty straight-forward, and is required to send Audio. This will also allow us to get an IAudioClient, which we will later use to send Audio.

.. code-block:: csharp6
	
	// Get the audio channel
	var channel = await _client.GetChannelAsync(150482537465118721) as IVoiceChannel;
	// Get the IAudioClient by calling the JoinAsync method.
	var _audio = await channel.JoinAsync();

The client will sustain a connection to this channel until it is kicked, disconnected from Discord, or told to Disconnect.

The IAudioClient
----------------

|updated-sec|

The IAudioClient is used to connect/disconnect to/from a Voice Channel, and to send audio to that Voice Channel.

.. function:: Func<Task> IAudioClient.Connected
	
	Event raised when the audio client connects

.. function:: Func<Exception, Task> IAudioClient.Disconnected
	
	Event raised when the audio client disconnects, raised with the exception that caused the disconnect.

.. function:: Func<int before, int after, Task> LatencyUpdated
	
	Raised when the Latency field is updated

.. function:: IAudioClient.DisconnectAsync();
	
	Disconnects the IAudioClient from the Voice Server.

.. function:: CreateOpusStream(int samplesPerFrame, int bufferSize = 4000)

	Returns an RTPWriteStream that allows you to send DCA (Opus Audio) to Discord.

.. function:: CreatePCMStream (int samplesPerFrame, int bufferSize = 4000)

	Returns an OpsuEncodeStream that allows you to send PCM data to Discord. Requires the Opus library.
 


Broadcasting
------------

|updated-sec|

There are multiple approaches to broadcasting audio. Discord.Net can convert your PCM data into Opus format, so the only work you need to do is converting your audio into a format that our encoder will accept. The format the OpusEncodeStream takes is 16-bit 48000Hz PCM.

Alternatively, you may send DCA audio to Discord, which is audio already encoded to the Opus format.

Broadcasting with NAudio
------------------------

`NAudio`_ is one of the easiest approaches to sending audio, although it is not multi-platform compatible. The following example will show you how to read an mp3 file, and send it to Discord.
You can `download NAudio from NuGet`_.

.. code-block:: csharp6

	using NAudio;
	using NAudio.Wave;
	using NAudio.CoreAudioApi;
	
	public void SendAudio(string filePath)
	{
		var channelCount = _client.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
		var OutFormat = new WaveFormat(48000, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
		using (var MP3Reader = new Mp3FileReader(filePath)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
		using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
		{
			resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
			int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
			byte[] buffer = new byte[blockSize];
			int byteCount;

			while((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
			{
				if (byteCount < blockSize)
				{
					// Incomplete Frame
					for (int i = byteCount; i < blockSize; i++)
						buffer[i] = 0;
				}
				_vClient.Send(buffer, 0, blockSize); // Send the buffer to Discord
			}
		}

	}

.. _NAudio: https://naudio.codeplex.com/
.. _download NAudio from NuGet: https://www.nuget.org/packages/NAudio/

Broadcasting with FFmpeg
------------------------

`FFmpeg`_ allows for a more advanced approach to sending audio, although it is multiplatform safe. The following example will show you how to stream a file to Discord.

.. code-block:: csharp6

	public void SendAudio(string pathOrUrl)
	{
		var process = Process.Start(new ProcessStartInfo { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
			FileName = "ffmpeg",
			Arguments = $"-i {pathOrUrl} " + // Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
				"-f s16le -ar 48000 -ac 2 pipe:1", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout. 
			UseShellExecute = false,
			RedirectStandardOutput = true // Capture the stdout of the process
		});
		Thread.Sleep(2000); // Sleep for a few seconds to FFmpeg can start processing data.
		
		int blockSize = 3840; // The size of bytes to read per frame; 1920 for mono
		byte[] buffer = new byte[blockSize];
		int byteCount;

		while (true) // Loop forever, so data will always be read
		{
			byteCount = process.StandardOutput.BaseStream // Access the underlying MemoryStream from the stdout of FFmpeg
				.Read(buffer, 0, blockSize); // Read stdout into the buffer

			if (byteCount == 0) // FFmpeg did not output anything
				break; // Break out of the while(true) loop, since there was nothing to read.

			_vClient.Send(buffer, 0, byteCount); // Send our data to Discord
		}
		_vClient.Wait(); // Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.
	}

.. _FFmpeg: https://ffmpeg.org/

.. note::
	
	The code-block above assumes that your client is configured to stream 2-channel audio. It also may prematurely end a song. FFmpeg can — especially when streaming from a URL — stop to buffer data from a source, and cause your output stream to read empty data. Because the snippet above does not safely track for failed attempts, or buffers, an empty buffer will cause playback to stop. This is also not 'memory-friendly'.

Multi-Server Broadcasting
-------------------------

Multi-Server Broadcasting is supported out-of-box. Just create an ``IAudioClient`` for each server you wish to broadcast to.


Receiving
---------
**Receiving is not implemented in the latest version of Discord.Net**