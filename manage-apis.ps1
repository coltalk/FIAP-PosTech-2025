param(
    [ValidateSet("up", "down", "build", "restore", "restart")]
    [string]$Action = "up"
)

$ErrorActionPreference = "Stop"

$projects = @(
    @{
        Name = "Catalog API"
        Path = "C:\Visual Studio\FIap\Fase 2 - fcg-catalog-api"
        Solution = "TcCatalog.sln"
    },
    @{
        Name = "Notifications API"
        Path = "C:\Visual Studio\FIap\Fase 2 - fcg-notifications-api"
        Solution = "Notifications.sln"
    },
    @{
        Name = "Payments API"
        Path = "C:\Visual Studio\FIap\Fase 2 - fcg-payments-api"
        Solution = "Payments.sln"
    },
    @{
        Name = "Users API"
        Path = "C:\Visual Studio\FIap\Fase 2 - fcg-users-api"
        Solution = "TcUser.sln"
    }
)

function Invoke-Step {
    param(
        [string]$FilePath,
        [string[]]$ArgumentList,
        [string]$WorkingDirectory,
        [string]$Description
    )

    Write-Host ""
    Write-Host "==> $Description" -ForegroundColor Cyan

    Push-Location $WorkingDirectory
    try {
        & $FilePath @ArgumentList
        if ($LASTEXITCODE -ne 0) {
            throw "Falha ao executar: $FilePath $($ArgumentList -join ' ')"
        }
    }
    finally {
        Pop-Location
    }
}

function Restore-And-Build {
    param(
        [hashtable]$Project
    )

    Invoke-Step `
        -FilePath "dotnet" `
        -ArgumentList @("restore", $Project.Solution) `
        -WorkingDirectory $Project.Path `
        -Description "$($Project.Name): dotnet restore"

    Invoke-Step `
        -FilePath "dotnet" `
        -ArgumentList @("build", $Project.Solution) `
        -WorkingDirectory $Project.Path `
        -Description "$($Project.Name): dotnet build"
}

function Docker-Up {
    param(
        [hashtable]$Project
    )

    Invoke-Step `
        -FilePath "docker" `
        -ArgumentList @("compose", "up", "-d", "--build") `
        -WorkingDirectory $Project.Path `
        -Description "$($Project.Name): docker compose up -d --build"
}

function Docker-Down {
    param(
        [hashtable]$Project
    )

    Invoke-Step `
        -FilePath "docker" `
        -ArgumentList @("compose", "down") `
        -WorkingDirectory $Project.Path `
        -Description "$($Project.Name): docker compose down"
}

switch ($Action) {
    "restore" {
        foreach ($project in $projects) {
            Invoke-Step `
                -FilePath "dotnet" `
                -ArgumentList @("restore", $project.Solution) `
                -WorkingDirectory $project.Path `
                -Description "$($project.Name): dotnet restore"
        }
    }
    "build" {
        foreach ($project in $projects) {
            Restore-And-Build -Project $project
        }
    }
    "up" {
        foreach ($project in $projects) {
            Restore-And-Build -Project $project
            Docker-Down -Project $project
            Docker-Up -Project $project
        }

        foreach ($project in $projects) {
            Docker-Down -Project $project
        }
    }
    "down" {
        foreach ($project in $projects) {
            Docker-Down -Project $project
        }
    }
    "restart" {
        foreach ($project in $projects) {
            Docker-Down -Project $project
        }

        foreach ($project in $projects) {
            Restore-And-Build -Project $project
            Docker-Down -Project $project
            Docker-Up -Project $project
        }

        foreach ($project in $projects) {
            Docker-Down -Project $project
        }
    }
}

Write-Host ""
Write-Host "Processo concluido com sucesso." -ForegroundColor Green
