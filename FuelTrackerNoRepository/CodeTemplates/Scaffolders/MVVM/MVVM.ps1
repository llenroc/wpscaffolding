[T4Scaffolding.Scaffolder(Description = "Adds an MVVM classes and view to a Windows Phone Project")][CmdletBinding()]
param(     
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

# Ensure you've referenced System.Data.Linq
#(Get-Project $Project).Object.References.Add("System.Data.Linq") | Out-Null


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


# Prepare all the parameter values to pass to the template, then invoke the template with those values
$repositoryName = $foundModelType.Name + "Repository"
if(!$DefaultNamespace){$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value}
$modelTypeNamespace = [T4Scaffolding.Namespaces]::GetNamespace($foundModelType.FullName)
$areaNamespace = if ($Area) { [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + ".Areas.$Area") } else { $defaultNamespace }
$dataContextNamespace = $foundDataContextType.Namespace.FullName
$repositoriesNamespace = [T4Scaffolding.Namespaces]::Normalize($areaNamespace + ".Models")
#$relatedEntities = [Array](Get-RelatedEntities $foundModelType.FullName -Project $project)
if (!$relatedEntities) { $relatedEntities = @() }

if(!$ViewNamespace){ $ViewNamespace = $defaultNamespace+".Views"}
if(!$ViewModelNamespace){ $ViewModelNamespace = $defaultNamespace+".ViewModels"}

Write-Host "Starting MVVM Scaffolding for $ModelType"

#ViewModels
Scaffold ViewModel List -ModelType $ModelType -DataContextType $DataContextType -Force:$Force
Scaffold ViewModel Details -ModelType $ModelType -DataContextType $DataContextType -Force:$Force
Scaffold ViewModel Create -ModelType $ModelType -DataContextType $DataContextType -Force:$Force
Scaffold ViewModel Edit -ModelType $ModelType -DataContextType $DataContextType -Force:$Force

#Views
Scaffold View ListView -ModelType $ModelType -DataContextType $DataContextType -Force:$Force
Scaffold View DetailsView -ModelType $ModelType -DataContextType $DataContextType -Force:$Force
Scaffold View CreateOrEditViewControl -ModelType $ModelType -DataContextType $DataContextType -Force:$Force
Scaffold View CreateView -ModelType $ModelType -DataContextType $DataContextType -Force:$Force
Scaffold View EditView -ModelType $ModelType -DataContextType $DataContextType -Force:$Force

Write-Host "TO DO: " -ForegroundColor DarkBlue -BackgroundColor Gray
Write-Host "Put the following HyperlinkButton on the MainPage to easily navigate to $modelTypePluralized list page" -ForegroundColor DarkBlue -BackgroundColor Gray
$hyperlinkbutton  = "<HyperlinkButton Content=`""+$modelTypePluralized+" list`" NavigateUri=`"/Views/"+$modelTypePluralized+"ListView.xaml`"/>"
Write-Host $hyperlinkbutton -ForegroundColor DarkGreen -BackgroundColor Gray