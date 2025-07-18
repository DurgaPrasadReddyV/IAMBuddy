# Find all Dockerfiles recursively
$dockerfiles = Get-ChildItem -Recurse -Filter Dockerfile

foreach ($file in $dockerfiles) {
    $projectDir = $file.DirectoryName
    $imageName = "iambuddy-" + $file.Directory.Name.ToLower()
    Write-Host "Building image $imageName from $($file.FullName)"
    
    # Use the solution root directory as build context instead of project directory
    $solutionRoot = (Get-Item $projectDir).Parent.FullName
    docker build -f $file.FullName -t $imageName $solutionRoot
}