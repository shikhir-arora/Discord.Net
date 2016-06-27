Logging
=======

|updated|

Discord.Net will log all of its events/exceptions using an internal Logger.

Messages can be accessed through the ``IDiscordClient.Log`` Event.

Usage
-----

To handle Log Messages through Discord.Net's Logger, hook into the ``Log`` event. It is raised with a single parameter, ``LogMessage``.

The LogManager does not provide a string-based result for the message, you must put your own message format together using the data provided through LogMessageEventArgs
See the Example for a snippet of logging.

Example
-------

.. literalinclude:: /samples/logging.cs
   :language: csharp6
   :tab-width: 2
