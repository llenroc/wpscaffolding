[T4Scaffolding.Scaffolder(Description = "Adds an View to a Windows Phone Project")][CmdletBinding()]
param(     
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ViewTemplateName,
	[string]$ModelType,
	[string]$ViewName,   
	[string]$ViewNamespace,   
	[string]$ViewModelName,   
	[string]$ViewModelNamespace, 
	[string]$PrimaryKey,
	[string]$DefaultNamespace,
	[string]$Project,
	[string]$CodeLanguage,
	[string]$DataContextType,
	[switch]$Repository = $false,
	[switch]$NoChildItems = $false,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[string]$ForceMode
)

# Ensure you've referenced System.Data.Entity
#(Get-Project $Project).Object.References.Add("System.Data.Entity") | Out-Null

#possible View types - available View templates.
#Example: ListView.cs.t4 corresponds to List ViewTemplateName

#check if ViewTemplateName is valid
if(!$ViewTemplateName)
{
	throw "ViewTemplateName is not specified!" 
}

# If you haven't specified a model type
if (!$ModelType) {
	throw "ModelType is not specified!" 
} else {
	# If you have specified a model type
	$foundModelType = Get-ProjectType $ModelType -Project $Project -BlockUi
	if (!$foundModelType) { return }
}

if(!$DataContextType) { $DataContextType = [System.Text.RegularExpressions.Regex]::Replace((Get-Project $Project).Name, "[^a-zA-Z0-9]", "") + "Context" }
if (!$NoChildItems) {
	if ($Repository) {
		Scaffold Repository -ModelType $foundModelType.FullName -DataContextType $DataContextType -Area $Area -Project $Project -CodeLanguage $CodeLanguage -Force:$Force -BlockUi
	} else {
	#	$dataContextScaffolderResult = Scaffold DataContext -ModelType $foundModelType.FullName -DataContextType $DataContextType -Area $Area -Project $Project -CodeLanguage $CodeLanguage -BlockUi
	#	$foundDataContextType = $dataContextScaffolderResult.DataContextType
	#	if (!$foundDataContextType) { return }
	}
}
#if (!$foundDataContextType) { $foundDataContextType = Get-ProjectType $DataContextType -Project $Project }
#if (!$foundDataContextType) { return }

if (!$primaryKey) {$primaryKey = Get-PrimaryKey $foundModelType.FullName -Project $Project -ErrorIfNotFound}
if (!$primaryKey) { return }

$modelTypePluralized = Get-PluralizedWord $foundModelType.Name

if(!$ViewName)
{	
	if($ViewTemplateName -eq "ListView")
	{
		$ViewName = $modelTypePluralized + $ViewTemplateName
	}
	else
	{
		$ViewName = $ModelType + $ViewTemplateName 
	}
}
if(!$ViewModelName)
{
	$ViewModelName = $ViewName+"Model"
	
	#we use CreateOrEditViewModel instead of both CreateViewModel and EditViewModel
	#if you have separate CreateViewModel and EditViewModel templates then use CreateOrEditViewModel
	if(($ViewModelName -eq $ModelType+"CreateViewModel") -or ($ViewModelName -eq $ModelType+"EditViewModel") )
	{
		$ViewModelName = $ModelType+"CreateOrEditViewModel"
	}
}

$outputPath = Join-Path Views $ViewName

# Prepare all the parameter values to pass to the template, then invoke the template with those values
$repositoryName = $foundModelType.Name + "Repository"
if(!$DefaultNamespace){$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value}
$modelTypeNamespace = [T4Scaffolding.Namespaces]::GetNamespace($foundModelType.FullName)
$controllerNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))
$areaNamespace = if ($Area) { [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + ".Areas.$Area") } else { $defaultNamespace }
$dataContextNamespace = $foundDataContextType.Namespace.FullName
$repositoriesNamespace = [T4Scaffolding.Namespaces]::Normalize($areaNamespace + ".Models")
#$relatedEntities = [Array](Get-RelatedEntities $foundModelType.FullName -Project $project)
if (!$relatedEntities) { $relatedEntities = @() }

if(!$ViewNamespace){ $ViewNamespace = $defaultNamespace+".Views"}
if(!$ViewModelNamespace){ $ViewModelNamespace = $defaultNamespace+".ViewModels"}

Write-Host "Scaffolding $ViewName..."
$templateName = $ViewTemplateName
$templateNameCodeBehind = $templateName + "CodeBehind"

Write-Host "Rendering $templateName"
Add-ProjectItemViaTemplate $outputPath -Template $templateName -Model @{
	ViewName = $ViewName;
	ViewNamespace = $ViewNamespace;
	ViewModelName = $ViewModelName;
	ViewModelNamespace = $ViewModelNamespace;
	ModelType = [MarshalByRefObject]$foundModelType; 
	PrimaryKey = [string]$primaryKey; 
	DefaultNamespace = $defaultNamespace; 
	AreaNamespace = $areaNamespace; 
	DataContextNamespace = $dataContextNamespace;
	RepositoriesNamespace = $repositoriesNamespace;
	ModelTypeNamespace = $modelTypeNamespace; 
	DataContextType = [MarshalByRefObject]$foundDataContextType;
	DataContextTypeName = $DataContextType;
	Repository = $repositoryName; 
	ModelTypePluralized = [string]$modelTypePluralized; 
	RelatedEntities = $relatedEntities;
} -SuccessMessage "Added View {0}" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

Write-Host "Rendering $templateNameCodeBehind"
Add-ProjectItemViaTemplate $outputPath -Template $templateNameCodeBehind -Model @{
	ViewName = $ViewName;
	ViewNamespace = $ViewNamespace;
	ViewModelName = $ViewModelName;
	ViewModelNamespace = $ViewModelNamespace;
	ModelType = [MarshalByRefObject]$foundModelType; 
	PrimaryKey = [string]$primaryKey; 
	DefaultNamespace = $defaultNamespace; 
	AreaNamespace = $areaNamespace; 
	DataContextNamespace = $dataContextNamespace;
	RepositoriesNamespace = $repositoriesNamespace;
	ModelTypeNamespace = $modelTypeNamespace; 
	DataContextType = [MarshalByRefObject]$foundDataContextType;
	DataContextTypeName = $DataContextType;
	Repository = $repositoryName; 
	ModelTypePluralized = [string]$modelTypePluralized; 
	RelatedEntities = $relatedEntities;
} -SuccessMessage "Added View CodeBehind {0}" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

#if (!$NoChildItems) {
#	$controllerNameWithoutSuffix = [System.Text.RegularExpressions.Regex]::Replace($ViewName, "Controller$", "", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
#	if ($ViewScaffolder) {
#		Scaffold Views -ViewScaffolder $ViewScaffolder -Controller $controllerNameWithoutSuffix -ModelType $foundModelType.FullName -Area $Area -Project $Project -CodeLanguage $CodeLanguage -Force:$overwriteFilesExceptController
#	}
#}