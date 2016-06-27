Terminology
===========

Preface
-------

Most terms for objects remain the same between 0.9 and 1.0. The major difference is that the ``Server`` is now called ``Guild``, to stay in line with Discord internally.

Introduction to Interfaces
--------------------------

Discord.Net 1.0 is built strictly around Interfaces. There are no methods that return a concrete object, only an interface. 

Many of the interfaces in Discord.Net are linked through inheritance. For example, ``IChannel`` represents any channel in Discord. ``IGuildChannel`` inherits from IChannel, and represents all channels belonging to a Guild. As a result, ``IChannel`` can be cast to ``IGuildChannel``, and you may find yourself doing this frequently in order to properly utilize the library.

The Inheritance Tree
--------------------

.. image:: https://i.lithi.io/kpgd.png
.. image:: https://i.lithi.io/kNrr.png
.. image:: https://i.lithi.io/gs8d.png
.. image:: https://i.lithi.io/LAJr.png