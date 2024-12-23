
@@@ FULLY LOADED - WEAPON SYSTEM @@@


1. Introduction
===============

 FullyLoaded is a data-driven weapon system for Unity with features such as object pooling, shared ammo pools, weapon switching, and many ways of customising your weapons.  This can all be accomplished without needing to write code, simply create your weapon and projectile assets and modify them using the custom inspectors.

 FullyLoaded works for both 2D and 3D projects!


2. Exploring the Example Content
================================

 2.1 2D Example Scene
 --------------------

 The 2D example scene can be found in "FullyLoaded/ExampleScenes/2DExample".  Note that you will need to add a scripting define to enable 2D mode (see section 3 below).
 
 The 2D example scene has a player prefab called "ExamplePlayer2D" that can be dropped into your scene as a quick way of testing the weapons system.  There are three example scripts attached that handle movement and aiming: ExamplePlayerAiming2D, ExamplePlayerMovement2D and ExamplePlayerInput.  The player prefab supports both the legacy and the new input system.
 
 The default controls are as follows:
 - W/S/A/D to move up/down/left/right
 - Mouse to aim
 - Left/Right mouse button to fire Primary/Secondary fire
 - R to reload
 - Q/E to switch to previous/next weapon
 
 
 2.2 3D Example Scene
 --------------------
 
 The 3D example scene can be found in "FullyLoaded/ExampleScenes/3DExample".
 
 The 3D example scene has a player prefab called "ExamplePlayer" that can be dropped into your scene as a quick way of testing the weapon system.  It has three relevant scripts: ExamplePlayerMovement, ExamplePlayerCamera and ExamplePlayerInput.  Like the 2D example scene, it works with both the legacy and the new input system.
 
 The ExamplePlayer prefab will stick to the ground while moving around, to set which collision layers are considered ground you can edit the "Overlap Layers" property in the inspector of the ExamplePlayerMovement script.
 
 The default controls are as follows:
 - W/S/A/D to move forwards/backwards/left/right
 - Mouse to move the camera and aim
 - Left/Right mouse button to fire Primary/Secondary fire
 - R to reload
 - Q/E to switch to previous/next weapon
 - Spacebar to toggle between first and third-person view


3. Project Setup for 2D Games
=============================

To enable 2D mode in FullyLoaded you must add a scripting define to your project settings.  Go to the "Edit" menu and click on "Project Settings...", then click "Player Settings" on the left.  Scroll down to the "Scripting Defines" section and add a new define called "FULLY_LOADED_2D" without quotes.  The presence of this scripting define enables all 2D functionality.



4. Weapons
==========

 4.1 Creating Weapon Definition Assets
 -------------------------------------

 Each weapon is configured via a Weapon Definition asset (a scriptable-object).  To create a new weapon, right click in the Project tab and choose "Create -> Fully Loaded -> Weapon Definition Asset".  Having weapons as assets in your project means you can easily re-use them between scenes, and even between projects!


 4.2 Primary and Secondary Fire
 ------------------------------

 Each weapon can have both a primary and a secondary fire which are configured separately.  Primary and secondary fire can both be used simultaneously, they can fire different projectiles, have different clips and ammo, and can be completely independent of each other if you wish.

 Secondary fire is optional, to enable secondary fire for a weapon, click the checkbox next to "Secondary Fire Config" in the inspector.

 The following describes the different aspects of a primary/secondary fire config that can be configured


 4.3 A Note on Terminology
 -------------------------

 Because there can be so much going on when firing a weapon, FullyLoaded uses a few distinct terms to describe the different stages of firing.

 Firstly, a weapon is 'activated' by the trigger behaviour.  In the case of a semi-automatic weapon, the weapon is activated once for each pull and release of the trigger, in the case of an automatic weapon, it is activated repeatedly while the trigger is held.

 Each activation will fire one or more 'shots', there could be a single shot for each activation, or a single activation could cause a succession of shots, one after the other, if a weapon is set up for burst fire.

 Finally, each shot will spawn one or more projectiles (or perform one or more instant-hit raycasts).  Shots can be set up to spawn multiple projectiles at once, e.g. for a shotgun weapon.  This is done using the multi-shot config.

 So Activation -> FireShot(s) -> Spawn Projectile(s)


 4.4 Trigger Behaviours
 ----------------------
 Trigger behaviours tell the weapon when it should activate, usually in response to player input.  When you call WeaponUser.PullTrigger() or WeaponUser.ReleaseTrigger() that information is processed by the trigger behaviour to determine if an activation should occur.

 FullyLoaded includes three trigger behaviours, but more can easily be added.

 Automatic: The weapon activates repeatedly while the trigger is held.  You can configure the rate of fire.
 SemiAuto: The weapon activates once for each trigger pull.  You can configure the max rate of fire.
 HoldToCharge: The trigger must be held for a certain period of time and then released to activate.

 Each of these trigger behaviours has its own set of properties that can be configured.


 4.5 Ammo Config
 ---------------------
 The ammo config allows you to choose which type of ammo the weapon uses, whether it should have infinite ammo or not, how much ammo is spent per shot (or per activation), whether the weapon has a reloadable clip, and also whether or not the weapon can be reloaded.

 There are two types of reload for weapons with clips: manual reload and auto-reload, which can be enabled/disabled independently.  Manual reload occurs when WeaponUser.Reload() is called, usually as a result of player input.  Auto-reload (if enabled) will cause the weapon to reload when it is activated with an empty clip, if there is ammo available to do so.


 4.6 Burst Fire Config
 ---------------------

 Burst-fire allows a weapon to fire multiple shots in quick succession from a single activation.  For example, you might want each trigger pull to fire three shots with a small interval in between each shot.  It's worth noting that while a weapon is burst-firing, other activations will be blocked until the burst has completed (so you can't overlap bursts).

 When burst fire is enabled, you can configure the number of shots and the time between shots.
 
 The "Use Trigger Charges" tickbox allows for a variable number of shots when combined with the "HoldToCharge" trigger behaviour.  This allows you to vary the length of the burst depending upon how long the trigger was held.


 4.7 Multi-Shot Config
 ---------------------

 Multi-shot allows each shot to fire multiple projectiles at once, such as for a shotgun type weapon.  Simply enable it and enter the shot count.
 
 The "Use Trigger Charges" tickbox allows for a variable number of shots when combined with the "HoldToCharge" trigger behaviour.  This allows you to vary the number of shots depending upon how long the trigger was held.


 4.8 Shot Spread Behaviour
 -------------------------

 Shot-spread allows adding randomization to the trajectory of your shots, rather than having your weapons be laser-accurate at all times.  There are two shot spread behaviours included in FullyLoaded for 3D projects: box and cone shaped spread behaviours, though it is easy to create your own too.  For 2D projects there is a cone shaped shot spread behaviour available if desired.


 4.9 Projectile
 --------------

 Finally, select a projectile definition to use, this determines if the weapon is a projectile weapon or an instant-hit weapon.  Projectile definitions are explained in the next section.


5. Projectiles
==============

 5.1 Creating Projectile Assets
 ------------------------------

 Like weapons, projectiles are configured via an asset which can be re-used between scenes or projects.  To create a Projectile Definition Asset, right click in the Project tab and choose "Create -> Fully Loaded -> Projectile Definition Asset"


 5.2 Shot Types
 --------------
 FullyLoaded has two types of shots, 'Projectile' and 'Instant-Hit'.

 Projectile shots spawn a physical projectile into the scene which travels at a given speed, and can optionally be affected by gravity and interact with colliders.  When a projectile hits an object in the scene, it can impart damage and physical force.  Projectiles are spawned via the object pool to minimise garbage at runtime.

 Instant-hit shots happen instantaneously by performing a raycast to determine which object in the scene was hit.

 The type of shot is set in the Projectile Definition by changing the 'Type' property.


 5.3 Projectile Prefabs
 ----------------------

 Projectile Definition Assets that have their type set to 'Projectile' must be paired with a projectile prefab in order to function.  While the projectile definition has various settings to configure how the projectile behaves, the prefab is what actually gets spawned when the weapon is fired.  All projectiles are spawned via the Object Pooling System in order to minimise garbage.

 There are two projectile prefab types included in FullyLoaded and new projectile types can easily be added by extending the BaseProjectile class.  Which prefabs you should use depends if you are using FullyLoaded in 2D or 3D mode, as different colliders and rigidbodies will be used.  The two included projectile prefab types are:

 PhysicsProjectile: for projectiles that interact with physics in the world, for example a grenade that needs to bounce off walls and come to rest on the ground.  These are implemented using dynamic rigidbodies with continuous collision.

 PiercingProjectile: for projectiles that can pierce through objects/enemies.  These are implemented using kinematic rigidbodies with circle/sphere-casts to detect collisions.

 Both projectile prefabs can be found in "FullyLoaded/Prefabs/Projectiles/2D" and "FullyLoaded/Prefabs/Projectiles/3D" depending on if you're using FullyLoaded in 2D or 3D mode.


 5.4 Damage Config
 -----------------

 All Projectile Definitions have an 'Impact Damage' property, which determines how much damage is imparted onto a valid target when hit.  There is also an optional 'Impact Damage Type' which can be set (for custom damage processing).


 5.5 Projectile Config
 ---------------------

 This section allows you to set the projectile prefab, as well as things like the speed and lifetime.  Different settings will be shown depending upon the type of projectile prefab (PhysicsProjectile or PiercingProjectile).
 
 PhysicsProjectiles can have their ability to bounce off surfaces limited to some maximum number, as well as being able to set if they persist after their first impact (important for projectiles like grenades that you would like to come to rest).  To tweak how a PhysicsProjectile interacts with colliders in your scene you should modify the settings on the rigidbody of the prefab itself such as gravity and drag.  Setting a PhysicMaterial on the prefab allows you to control friction and bounciness.
 
 PiercingProjectiles can have their gravity and drag set via the projectile config, as well as being able to set how many penetrations they can do.  If you would like to limit the projectile to only being able to pierce certain colliders, you can set the "Penetrable Tag" to a tag defined in your project, and the projectile will only penetrate colliders that have that tag set.

 The 'Projectile Layer' property allows you to select the collision layer the projectile will use once spawned, make sure this is a layer that is set (in the collision matrix) to collide with any obstacles or targets necessary.  It is recommended to make sure this layer is not set to collide with itself in the collision matrix.


 5.6 Instant-Hit Config
 ----------------------

 For Instant-hit projectiles, you can select the collision layers that can be hit, as well as the max distance of the raycast that is performed (the max range of the shot).  Similarly to PiercingProjectiles, instant-hit projectiles can be set to penetrate through colliders if desired.


 5.7 Explosion Config
 --------------------

 Projectiles can be set to explode, causing area-of-effect damage.  Here you can configure the size of that damage radius, as well as the damage range and damage type.

 For instant-hit projectiles, checking the 'Explode On Impact' checkbox will cause an explosion at the impact point.

 For projectile projectiles, there are two types of explosion possible, 'Explode On Impact' and 'Explode After Fuse'.  'Explode On Impact' means the projectile will explode as soon as it hits something, while 'Explode After Fuse' means that the projectile will only explode when its lifetime expires - this allows grenade-type projectiles that bounce around before exploding.

 The 'Explosion Overlap Layers' property is used in combination with a sphere overlap test to determine which objects are in range of the explosion that could potentially take damage.  Only objects in the selected layers will be valid targets for damage.

 The 'Explosion Obstacle Layers' is used to check if an object is behind cover or not.  A raycast will be performed from the centre of the explosion to each object that is within range, using the layer mask given in the 'Explosion Obstacle Layers' property.  If the raycast hits any obstacles before it reaches the object, the object is considered behind cover and won't take explosion damage.


 5.8 Effects Config
 ------------------

 This section allows attaching effects to the projectile.  All effects are spawned via the object pool so that they can be re-used without creating runtime garbage.  The following effects (in the form of effects prefabs) can be set per projectile:

 Trail Effect: causes a trail effect to appear behind the projectile, or along the instant-hit path
 Impact Effect: spawned at the point of impact
 Explosion Effect: spawned when the projectile explodes



6. Ammo
=============

 6.1 Defining Ammo Types
 -----------------------------
 
 Ammo types are defined as Ammo Type assets, which allows ammo types to be easily shared between weapons.  To create an Ammo Type Asset, right click in the Project tab and choose "Create -> Fully Loaded -> Ammo Type Asset"



7. Using The System - Important Components and Classes
======================================================

 7.1 WeaponUser Component
 ------------------------

 The WeaponUser component encapsulates firing weapons, switching between weapons and reloading weapons, and it does so by making simple function calls.  WeaponUser is ideal for player characters that need to respond to input: when the player presses the fire button you call PullTrigger(), and when they release the fire button you call ReleaseTrigger(), that's all there is to it.  Switching between weapons and reloading are equally simple.
 
 There are some properties that need to be set up on the WeaponUser component before use, see the ExamplePlayer prefab for an example.  The most important is the 'Muzzle Point' property, which determines the point in space from which all shots originate.  The 'Aim Mode' property determines how the current aim direction is determined, see the WeaponAim section (7.3) for more details.


 7.2 ScriptedWeaponUser Component
 --------------------------------
 
 ScriptedWeaponUser is a much simpler version of WeaponUser, designed for situations where firing weapons is handled by code as opposed to player input.  ScriptedWeaponUser has no concept of ammo clips, reloading, or even trigger behaviours: to fire a shot you call Fire() and that's it.  Because there is no trigger behaviour, you will have to handle things like rate of fire in the calling code.


 7.3 WeaponAim
 -------------
 
 The WeaponAim class is owned by WeaponUser/ScriptedWeaponUser and has one job: to determine the aim direction.  There are three aiming modes that can be used, which one to use is set on the WeaponUser/ScriptedWeaponUser component.
 
 Use Muzzle Direction: In this aiming mode, shots originate from the muzzle transform position, and are fired in the muzzle transform's positive Z axis (in 3D mode), or from the muzzle transform's positive X axis if using FullyLoaded in 2D mode.  If you want to change the aim direction, rotate the muzzle transform (or one of its parent transforms).
 
 Use Target Transform: In this aiming mode, shots originate from the muzzle transform position, and are fired towards the target transform position.  The target transform can be set by calling WeaponAim.SetTargetTransform().
 
 Use Target Position: In this aiming mode, shots originate from the muzzle transform position, and are fired towards the target position.  The target position can be set by calling WeaponAim.SetTargetPosition().


 7.4 WeaponBag Component
 -----------------------
 
 The WeaponBag component is used to set which weapons a WeaponUser is able to use, and whether or not they are owned.  A weapon can only be acquired if it is in the list within WeaponBag.  To make your character own a weapon, make sure the 'Initially Owned' checkbox is ticked.
 
 The order of weapons in this list also determines their order when switching weapons.


 7.5 AmmoBelt Component
 ----------------------

 The AmmoBelt component is an optional component that allows you to set up the ammo that can be used by a WeaponUser, set the initial and max amounts, and whether the ammo is unlimited or not.  When an AmmoBelt component is present, weapons will draw ammo from the AmmoBelt when used.
 
 If no AmmoBelt component is present, all ammo will be infinite.
 
 
 7.6 DamageTarget
 ----------------
 
 In order to receive damage, a GameObject must have a DamageTarget component.  The DamageTarget component has an event that can be registered with called OnHit, which will be called whenever that object receives damage from a projectile/instant-hit or explosion.  The GameObject will also need a collider to represent the area that can be hit.  The layer that the collider is in will determine which projectiles can hit it, it must match the Projectile Definition's 'Hit Layers' or 'Projectile Layer' property for that particular projectile to damage that object.



8. Projectile Effects
=====================

 8.1 Impact Effects
 ------------------
 
 Impact effects are spawned whenever a projectile or instant-hit shot hits something, and can be set in the Projectile Definition.  Impact effects make use of the Object Pooling System to prevent garbage by re-using instances rather than destroying and re-instantiating them.
 
 There is an existing prefab at "FullyLoaded/Prefabs/Projectiles/SpawnedEffect", so a simple way of adding new effects is to create a prefab variant from this prefab, then add whatever you need to the variant (Particle Systems, etc), and override the 'Lifetime' property.  You can create your own prefab with a SpawnedEffect component added to it instead if desired.


 8.2 Projectile / Instant-Hit Trails
 ---------------------
 
 Projectile / Instant-Hit trails are spawned whenever a shot is fired, and can be set in the Projectile Definition.  Trail effects make use of the Object Pooling System to prevent garbage.
 
 There are existing prefabs in the "FullyLoaded/Prefabs/Projectiles" folder, "ProjectileTrail" and "InstantHitTrail", a simple way of creating your own versions is to create a prefab variant from one of these and then modify the variant however you desire.  The prefab must have a ProjectileTrail/InstantHitTrail component.


 8.3 Explosion Effects
 ---------------------

 Explosion effects are spawned when a projectile is set to explode, and can be set in the Projectile Definition.  Like other effects, they make use of the Object Pooling System.
 
 Explosion effects and impact effects can be used interchangeably, they are just prefabs with a SpawnedEffect component added to them.  Like with impact effects, you can create a prefab variant of the "FullyLoaded/Prefabs/Projectiles/SpawnedEffect" and customise that to easily create new explosion effects.


9. Object Pooling
=================

 9.1 What Is Object Pooling
 --------------------------
 
 Object pooling is an optimization that can be very useful for Unity games.  Ordinarily, instantiating and destroying lots of objects will create a lot of garbage - the memory allocated for those objects that is no longer needed.  In something like a weapon system where thousands of projectiles can be spawned this is important, as cleaning up that memory is slow and can impact your game's performance when it does happen.
 
 An object pool keeps track of objects that are spawned, but instead of destroying those objects when they are no longer needed, they are instead disabled and are kept in reserve.  When a new object needs to be instantiated, instead of creating a whole new instance, the pool is first checked to see if there is an unused object that can be reused.  In FullyLoaded, this is used to eliminate all garbage from spawning projectiles and effects.
  

 9.2 Automatic Object Pooling
 ----------------------------
 
 When you fire a projectile you may notice a new GameObject is spawned into the scene, called ObjectPoolManager.  This is the automatic object pooling system.  All projectiles and effects are spawned as children of this GameObject, and when they are destroyed they remain in the pool for re-use.
 
 It is also possible to add the ObjectPoolManager component to your scene if you would like more control over the object pooling behaviour.  Doing so will allow you to set limits on the pool sizes for different spawned objects, and to set what happens when the pool is full and a spawn is attempted. 



10. Other Features
=================

 10.1 Pickup System
 -----------------
 
 FullyLoaded includes a simple weapon/ammo pickup system, to create a pickup simply add a WeaponPickup or an AmmoPickup component to your GameObject, along with an appropriate collider set in trigger mode.  Make sure that the collider is in a collision layer that is set to collide with that of the WeaponUser that is picking them up.  There are settings for respawn time and whether or not the pickup is available at the start of the game or not.


 10.2 Custom Damage Types
 -----------------------
 
 In order to attach custom data to damage events you can use the DamageType system.  DamageType is a scriptable object which can be derived from to create your own damage types.  Whenever damage is dealt to a DamageTarget component, the DamageType is always passed through to the OnHit event, you can use this to do custom processing for different damage types.
 
 
 10.3 Events
 ----------
 
 Both WeaponUser and SimpleProjectile/PhysicsProjectile have an events class, WeaponEvents and ProjectileEvents respectively.  Not only are these classes visible in their respective inspector windows, where they expose UnityEvent that can be hooked up, but they also expose C# events that you can register your code with.  These can be used to trigger custom behaviour for various situations.



11. Technical Support
=====================

Online documentation can be found here: https://fellbyte.co.uk/fullyloaded.html

If you have any further questions or would like to report a bug, please email support@fellbyte.co.uk

Thanks for purchasing :)