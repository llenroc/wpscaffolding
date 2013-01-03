[T4Scaffolding.Scaffolder(Description = "Adds an ViewModel to a Windows Phone Project")][CmdletBinding()]
param(     
	
	#[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ViewModelType,
	[string]$ModelType,
	[string]$ViewModelName,   
	[string]$ViewModelNamespace,   
	[string]$primaryKey,
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

# Ensure you've referenced System.Data.Linq
#(Get-Project $Project).Object.References.Add("System.Data.Linq") | Out-Null

#possible ViewModel types - available ViewModel templates.
#Example: ListViewModel.cs.t4 corresponds to List ViewModelType
$viewModelTypesArray = @("List", "CreateOrEdit", "Details")

#check if ViewModelType is valid
if(!$ViewModelType)
{
	throw "ViewModelType is not specified!" 
}
else
{
	#ViewModeltype binding to an existing ViewModel CreateOrEdit
	if(($ViewModelType -eq "Edit") -or ($ViewModelType -eq "Create") -or ($ViewModelType -eq "New"))
	{
		$ViewModelType = "CreateOrEdit"
	}
	if(!($viewModelTypesArray -contains $ViewModelType))
	{
		Write-Host "ViewModelType $ViewModelType is not valid ViewModelType. Possible ViewModelTypes are :\n"
		ForEach($strViewModelType in $viewModelTypesArray)
		{
			Write-Host $strViewModelType
		}
		
	}
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
		Scaffold Repository -ModelType $foundModelType.FullName -DataContextType $DataContextType -Project $Project -CodeLanguage $CodeLanguage -Force:$Force -BlockUi
	} else {
		$dataContextScaffolderResult = Scaffold DataContext -ModelType $foundModelType.FullName -DataContextType $DataContextType -Project $Project -CodeLanguage $CodeLanguage -BlockUi
		$foundDataContextType = $dataContextScaffolderResult.DataContextType
		if (!$foundDataContextType) { return }
	}
}
#if (!$foundDataContextType) { $foundDataContextType = Get-ProjectType $DataContextType -Project $Project }
#if (!$foundDataContextType) { return }

if (!$primaryKey) {$primaryKey = Get-PrimaryKey $foundModelType.FullName -Project $Project -ErrorIfNotFound}
if (!$primaryKey) { return }

$modelTypePluralized = Get-PluralizedWord $foundModelType.Name

if(!$ViewModelName)
{	
	if($ViewModelType -eq "List")
	{
		$ViewModelName = $modelTypePluralized + $ViewModelType + "ViewModel" 
	}
	else
	{
		$ViewModelName = $ModelType + $ViewModelType + "ViewModel" 
	}
}

$outputPath = Join-Path ViewModels $ViewModelName

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



if(!$ViewModelNamespace){ $ViewModelNamespace = $defaultNamespace+".ViewModels"}

Write-Host "Scaffolding $ViewModelName..."
$templateSufix = if($Repository) { "WithRepository" } else { "WithContext" }
$templateName = $ViewModelType + "ViewModel" + $templateSufix

Write-Host "Rendering $templateName"
Add-ProjectItemViaTemplate $outputPath -Template $templateName -Model @{
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
} -SuccessMessage "Added ViewModel {0}" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

#if (!$NoChildItems) {
#	$controllerNameWithoutSuffix = [System.Text.RegularExpressions.Regex]::Replace($ViewModelName, "Controller$", "", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
#	if ($ViewScaffolder) {
#		Scaffold Views -ViewScaffolder $ViewScaffolder -Controller $controllerNameWithoutSuffix -ModelType $foundModelType.FullName -Area $Area -Project $Project -CodeLanguage $CodeLanguage -Force:$overwriteFilesExceptController
#	}
#}