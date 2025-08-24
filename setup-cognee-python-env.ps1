# Get the absolute path to the main Cognee project folder (sibling to AppHost)
$solutionRoot = $PSScriptRoot
$cogneePath = Join-Path $solutionRoot "Cognee"
$venvPath = Join-Path $cogneePath ".venv"
$pythonExe = "$env:USERPROFILE\AppData\Local\Programs\Python\Python312\python.exe"
$requirements = Join-Path $cogneePath "requirements.txt"

if (-Not (Test-Path $venvPath)) {
    & $pythonExe -m venv $venvPath
}

& "$venvPath\Scripts\python.exe" -m pip install --upgrade pip
& "$venvPath\Scripts\pip.exe" install -r $requirements
