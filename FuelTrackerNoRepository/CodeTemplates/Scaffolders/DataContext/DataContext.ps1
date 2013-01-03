[T4Scaffolding.Scaffolder(Description = "Makes an SQL Ce DataContext able to persist models of a given type, creating the DataContext first if necessary")][CmdletBinding()]
param(        
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ModelType,
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$DataContextType,
	[string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders
)

# Ensure we can find the model type
$foundModelType = Get-ProjectType $ModelType -Project $Project
if (!$foundModelType) { return }

# Find the DataContext class, or create it via a template if not already present
$foundDataContextType = Get-ProjectType $DataContextType -Project $Project -AllowMultiple
if (!$foundDataContextType) {
	# Determine where the DataContext class will go
	$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
	if ($DataContextType.Contains(".")) {
		if ($DataContextType.StartsWith($defaultNamespace + ".", [System.StringComparison]::OrdinalIgnoreCase)) {
			$DataContextType = $DataContextType.Substring($defaultNamespace.Length + 1)
		}
		$outputPath = $DataContextType.Replace(".", [System.IO.Path]::DirectorySeparatorChar)
		$DataContextType = [System.IO.Path]::GetFileName($outputPath)
	} else {
		$outputPath = Join-Path Models $DataContextType
	}
	
	$dataContextNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))
	Add-ProjectItemViaTemplate $outputPath -Template DataContext -Model @{
		DefaultNamespace = $defaultNamespace; 
		DataContextNamespace = $dataContextNamespace; 
		DataContextType = $DataContextType; 
	} -SuccessMessage "Added database context '{0}'" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

	$foundDataContextType = Get-ProjectType ($dataContextNamespace + "." + $DataContextType) -Project $Project
	if (!$foundDataContextType) { throw "Created database context $DataContextType, but could not find it as a project item" }
} elseif (($foundDataContextType | Measure-Object).Count -gt 1) {
	throw "Cannot find the database context class, because more than one type is called $DataContextType. Try specifying the fully-qualified type name, including namespace."
}

# Add a new property on the DataContext class
if ($foundDataContextType) {
	$propertyName = Get-PluralizedWord $foundModelType.Name

	# If this is not a DataContext, we can't add a new property, so ensure there is already one
	# Unfortunately we have to use the awkward "PowerShellInvoke" calling mechanism otherwise
	# the PowerShell COM wrapper objects can't be passed into the .NET code.
	$isAssignableToParamTypes = New-Object System.Type[] 2
	$isAssignableToParamTypes[0] = [EnvDTE.CodeType]
	$isAssignableToParamTypes[1] = [System.String]
	$isDataContext = [T4Scaffolding.Core.EnvDTE.EnvDTEExtensions]::PowerShellInvoke([T4Scaffolding.Core.EnvDTE.EnvDTEExtensions].GetMethod("IsAssignableTo", $isAssignableToParamTypes), $null, @($foundDataContextType, "System.Data.Linq.DataContext"))
	if (!$isDataContext) {
		$existingMembers = [T4Scaffolding.Core.EnvDTE.EnvDTEExtensions]::PowerShellInvoke([T4Scaffolding.Core.EnvDTE.EnvDTEExtensions].GetMethod("VisibleMembers"), $null, $foundDataContextType)
		$hasExistingPropertyForModel = $existingMembers | ?{ ($_.Name -eq $propertyName) -and ($_.Kind -eq [EnvDTE.vsCMElement]::vsCMElementProperty) }
		if ($hasExistingPropertyForModel) {
			Write-Warning "$($foundDataContextType.Name) already contains a '$propertyName' property."
		} else {
			throw "$($foundDataContextType.FullName) is not a System.Data.Linq.DataContext class and does not contain a '$propertyName' property, so it cannot be used as the database context."
		}
	} else {
		# This *is* a DataContext, so we can freely add a new property if there isn't already one for this model
		Add-ClassMemberViaTemplate -Name $propertyName -CodeClass $foundDataContextType -Template DataContextEntityMember -Model @{
			EntityType = $foundModelType;
			EntityTypeNamePluralized = $propertyName;
		} -SuccessMessage "Added '$propertyName' to database context '$($foundDataContextType.FullName)'" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage
	}
}

return @{
	DataContextType = $foundDataContextType
}