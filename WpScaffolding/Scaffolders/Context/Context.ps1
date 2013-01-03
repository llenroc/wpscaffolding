[T4Scaffolding.Scaffolder(Description = "Adds an Context to a Windows Phone Project")][CmdletBinding()]
param(     
	
	[string]$ContextName,   
	[string]$ContextNamespace,   
	[string]$DefaultNamespace,
    [string]$Project,
    [string]$CodeLanguage,
	[switch]$NoChildItems = $false,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[string]$ForceMode
)



if(!$ContextName)
{	
	throw "ContextName is required to continue"
}

$outputPath = Join-Path Models $ContextName


if(!$DefaultNamespace){$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value}

if(!$ContextNamespace){ $ContextNamespace = $defaultNamespace+".Models"}

Write-Host "Scaffolding $ContextName..."
$templateName = $ContextType;

Write-Host "Rendering $templateName"
Add-ProjectItemViaTemplate $outputPath -Template $templateName -Model @{
	ContextName = $ContextName;
	ContextNamespace = $ContextNamespace;
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
} -SuccessMessage "Added Context {0}" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

#if (!$NoChildItems) {
#	$controllerNameWithoutSuffix = [System.Text.RegularExpressions.Regex]::Replace($ContextName, "Controller$", "", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
#	if ($ViewScaffolder) {
#		Scaffold Views -ViewScaffolder $ViewScaffolder -Controller $controllerNameWithoutSuffix -ModelType $foundModelType.FullName -Area $Area -Project $Project -CodeLanguage $CodeLanguage -Force:$overwriteFilesExceptController
#	}
#}