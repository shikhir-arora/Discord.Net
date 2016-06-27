Commands
========

|updated|  

|breaking|  

|incomplete|  


The `Discord.Net.Commands`_ package provides an Attribute-based Command Parser.

.. _Discord.Net.Commands: https://www.nuget.org/packages/Discord.Net.Commands

.. warning::
	The package on NuGet is behind the version used for this documentation.

Setup
-----

To use Commands, you must setup a Commands Service and setup the Command Handler. 

.. note::
	The below snippet includes a very bare-bones Command Handler. You can extend your Command Handler as much as you like, however the below snippet is all you need to get started.

.. literalinclude:: /samples/command_handler.cs
	:language: csharp6
	:tab-width: 2

Adding Commands
---------------

To add commands that will be automatically discovered, create a new class that imports ``Discord.Commands``, and flag the class with the ``Module`` attribute, as shown below.

**All Command Handlers** must be a ``public async Task(IMessage msg)``, with other command parameters going after the ``msg`` parameter.

.. literalinclude:: /samples/command.cs
	:language: csharp6
	:tab-width: 2

Attributes
----------

``Module`` - Apply to a ``class``; marks the class to be auto-discovered by the Command Service.  

``Command(string)`` - Apply to a ``async Task (IMessage)``; marks the method as a command.  

``Description(string)`` - Use with a command or parameter; sets the description for the command or parameter, respectively.

Parameters
----------

**Required:** Add any type of parameter to the command method.

**Optional:** Add any type of optional parameter to the command method, e.g. ``string optionalParam = null``.

**Multiple:** Add any type of params array to the command method, e.g. ``params string[]``. 

**Unparsed:** Add any type of parameter to the command method, flag it with ``Unparsed``.

All parameters can also be flagged with ``Description``.