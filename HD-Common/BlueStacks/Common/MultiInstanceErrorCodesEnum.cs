using System;

namespace BlueStacks.Common
{
	public enum MultiInstanceErrorCodesEnum
	{
		ReachedMaxLimit = -1,
		CloneVmFailure = -2,
		RegistryCopyFailure = -3,
		CreateServiceFailure = -4,
		UnknownException = -5,
		CommandNotFound = -6,
		VmNameNotValid = -7,
		VmNotExist = -8,
		VmNotRunning = -9,
		CannotDeleteDefaultVm = -10,
		NotSupportedInLegacyAndRawMode = -11,
		VirtualBoxInitFailed = -12,
		NotSupportedInLegacyMode = -13,
		WrongValue = -14,
		DeviceCapsNotPresent = -15,
		FactoryResetUnHandledException = -16,
		ProcessAlreadyRunning = -17,
		InvalidVmType = -18,
		CannotCloneRunningVm = -19,
		ErrorInRemovingDisk = -20
	}
}


