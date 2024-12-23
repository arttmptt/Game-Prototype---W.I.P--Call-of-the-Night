using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	// -------------------------------------------------------------------------------------------------

	public enum EWeaponAimMode
	{
		UseMuzzleDirection,
		UseTargetTransform,
		UseTargetPosition,
	}

	// -------------------------------------------------------------------------------------------------

	public enum EShotType
	{
		InstantHit,
		Projectile,
	}

	// -------------------------------------------------------------------------------------------------

	public enum EPhysicsProjectileExplosionType
	{
		None,
		ExplodeOnImpact,
		ExplodeAfterFuse,
	}

	// -------------------------------------------------------------------------------------------------

	public enum EShotSpreadAngle
	{
		Random,
		EvenlySpaced,
	}

	// -------------------------------------------------------------------------------------------------

	public enum EPickupSpawnMode
	{
		OnceOnly,
		RespawnTimer,
	}

	// -------------------------------------------------------------------------------------------------

	public enum EWeaponMode
	{
		Primary,
		Secondary,
	}

	// -------------------------------------------------------------------------------------------------

	public enum EReloadType
	{
		Manual,
		Automatic
	}

	// -------------------------------------------------------------------------------------------------

	public enum EReloadTarget
	{
		Both,
		PrimaryOnly,
		SecondaryOnly,
	}

	// -------------------------------------------------------------------------------------------------

	public enum EFireMode
	{
		SemiAutomatic,
		Automatic,
		Burst,
	}

	// -------------------------------------------------------------------------------------------------
}
