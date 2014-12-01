# ----------------------------------------------------------------------------------
#
# Copyright Microsoft Corporation
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# ----------------------------------------------------------------------------------

$global:StorSimpleGlobalConfigMap = $null

function Read-ConfigFile($configFilePath = "")
{
	if($global:StorSimpleGlobalConfigMap) {
		return $global:StorSimpleGlobalConfigMap
	}

	if([string]::IsNullOrEmpty($configFilePath)) {
		$configFilePath = $env:STORSIMPLE_SDK_TEST_CONFIG_PATH
	}

	if([string]::IsNullOrEmpty($configFilePath)) {
		throw "Read-ConfigFile: Config file path is not specified. Please specify file path OR set the environment variable STORSIMPLE_SDK_TEST_CONFIG_PATH."
	}

	Write-Verbose "Reading configuration file $configFilePath"

	Get-Content $configFilePath |  foreach-object -begin {$configMap=@{}} -process { $k = [regex]::split($_,'='); if(($k[0].CompareTo("") -ne 0) -and ($k[0].StartsWith("[") -ne $True) -and ($k[0].StartsWith("#") -ne $True)) { $configMap.Add($k[0], $k[1]) } }
	$global:StorSimpleGlobalConfigMap = $configMap
	return $configMap
}

<#
.SYNOPSIS
#>
function Get-Configuration ($configPropertyKey)
{
	Write-Verbose "Fetching from configuration map with the key '$configPropertyKey'"
	
	$configurationMap = Read-ConfigFile

	if($configurationMap) {
		if($configurationMap.ContainsKey($configPropertyKey)) {
			$configurationValue = $configurationMap[$configPropertyKey]
			Write-Verbose "Returning configuration property with value '$configurationValue' for key '$configPropertyKey'"

			return $configurationValue;
		}
	} else {
		throw "Cannot get value for property '$configPropertyKey'. Configuration map does not contain any keys"
	}

	Write-Verbose "Could not find any property in the configuration map with the key '$configPropertyKey'"
	return $null
}
