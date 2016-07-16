Type Readers
============

Type Readers allow you to parse different types of arguments.

By default, the following Types are supported arguments:

+ string
+ sbyte/byte
+ ushort/short
+ uint/int
+ ulong/long
+ float, double, decimal
+ DateTime/DateTimeOffset
+ IUser/IGuildUser
+ IChannel/IGuildChannel/ITextChannel/IVoiceChannel
+ IRole
+ IMessage

Creating a Type Reader
----------------------

To create a TypeReader, create a new class that imports ``Discord`` and ``Discord.Commands``. Ensure your class inherits from the ``Discord.Commands.TypeReader`` class.

Next, satisfy the ``TypeReader`` class by adding an ``override Task<TypeReaderResult> Read(IMessage context, string input)``.

.. note::
	
	In many cases, Visual Studio can fill this in for you, with the "Implement Abstract Class" IntelliSense hint.

Inside this task, you may add whatever logic you need to parse the input string. 

Finally, return a ``TypeReaderResult``. If you were able to succesfully parse the input, return ``TypeReaderResult.FromSuccess(parsed_input)``. Otherwise, return ``TypeReaderResult.FromError``.

Installing Type Readers
-----------------------

TypeReaders are not automatically discovered by the Command Service. To install a Type Reader, invoke ``_commands.AddTypeReader(Type, TypeReader)`` (where _commands is your CommandService).

Example
-------

.. literalinclude:: /samples/typereader.cs
   :language: csharp6
   :tab-width: 2