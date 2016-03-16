# Practice-Turnbased-RPG-Battle-System
## By Thomas Deeb

Practice for a basic turn-based RPG battle system in SFML.NET.

The goal is to create a flexible battle system with the following features:  
- Turn order based on speed, with order differing depending on speed mods during battle **(DONE)**  
- Status effects (modifies stats, damage per turn, restricts actions such as magic use) **(DONE)**  
- Items (damage, healing, status) and inventory **(DONE)**  
- Magic (# of turns to cast, target multiple enemies, % chance for inflicting status effects) **(DONE)**  
- Character and enemy exclusive actions and spells, which can be easily added or moved to different characters and enemies **(DONE)**  

# Additional Notes

This turn-based RPG is written in C# and SFML.NET and is based on the Final Fantasy series. The focus was entirely on the turn-based
gameplay, so there's very little polish in the visual department.  

It was a great learning experience working on this project and I learned quite a bit from it. It was my first time ever writing
a turn-based system. Here are the areas of the system I feel could use more refinement:  

- UI and battle menus: Whatever UI is there was written quickly, and while I did polish up the battle menus later on, they could be more flexible
and easier to use.  
- Animations: I wrote sprite animation classes that support looping and a variable number of frames but never used them. They likely would fit in
regardless of the way animations were defined either through code or metadata.  
- BattleCommands: The battle menus themselves weren't bad, but passing the required information through multiple levels in a clean way is difficult. It's highly probable that I could've linked BattleCommands more directly to the menus or actions themselves so there's less code clutter. Additionally,
there could be much better support for BattleCommands with submenus, such as the Item and Magic commands, as those types were the most problematic.  
- SFML: I decided to try out this library for this project since I heard good things about it. Unfortunately it doesn't support sprite batching, which is crucial in any large game project. It has some very nice features and is a lightweight library, but chances are I won't use it again in the future.