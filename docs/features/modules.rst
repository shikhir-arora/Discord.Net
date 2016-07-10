Modules
=======

|updated|  

Modules in Discord.Net mark classes that can contain groups of Commands to be used by the :doc:`/features/commands` Service.

Usage
-----

To create a module, create a class that you will place commands in. Next, ensure that your class imports ``Discord.Commands``. Finally, flag this class with the ``[Module]`` attribute.

Example:

.. code-block:: csharp6
	
	using Discord.Commands;

	[Module]
	public class InfoModule
	{
		// Place Commands Here
	}

There are two ways to load modules; both involve using the Commands Service.

Loading Modules Automatically
-----------------------------

The Commands Service can automatically discover all classes in an Assembly marked with the ``Module`` attribute, and load them. 

To automatically load Modules, run the following code. ``await _commands.LoadAssembly(Assembly.GetEntryAssembly());``

This code assumes that ``_commands`` is your Commands Service, and that the modules you want to load are in the same Assembly as the code you are running.

.. warning::
	
	If your module has a custom constructor that is not parameter-less (it takes arguments), you will need to manually load this module.

Loading Modules Manually
------------------------

To manually load a Module, create an instance of it. Then, run ``await _commands.Load(instanceOfMyModule);``

