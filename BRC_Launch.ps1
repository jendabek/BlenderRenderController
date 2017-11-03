param(
    [Parameter(Mandatory = $true)][string]$DeployPath,
    [Parameter(Mandatory = $true)][string]$FilesPath,
    [string]$Version = '0000',
    [ValidateSet("lite", "full", "unix")][string]$Format = "lite"
)


if (!(Test-Path $DeployPath))
{
    Write-Host "Error, Deploy path not found" -b Red -f White
    exit 1
}
elseif (!(Test-Path $FilesPath))
{
    Write-Host "Error, Files path not found" -b Red -f White
    exit 2
}

''
Write-Host "Version: $Version"
Write-Host "Format: $Format"
''
 
$OLDPWD = $PWD.Path

#clear deploy folder
Set-Location "$DeployPath"
#del .\*.*
del .\tmp\* -Recurse 

Write-Host "Coping files..." -NoNewline

#xcopy "$FilesPath\*" "$DeployPath\tmp\*" /Y /S /F
copy "$FilesPath\*" "$DeployPath\tmp\" -Force -Recurse

Write-Host "Done"

Write-Host "Creating base zip..." -NoNewline

# base zip
$baseZipName = "BRC_v$Version"
Compress-Archive .\tmp\* .\"$baseZipName.zip" -Force

Write-Host "Done"

# Full
if ($Format -eq 'full')
{
    Write-Host "Creating 'Full' zips..." -NoNewline

    copy ./"$baseZipName.zip" ./"$baseZipName.Full-x86.zip"
    copy .\ffmpegExes\ffmpeg-x86.exe .\ffmpeg.exe -Force
    Compress-Archive .\ffmpeg.exe ./"$baseZipName.Full-x86.zip" -Update

    copy "./$baseZipName.zip" ./"$baseZipName.Full-x64.zip"
    copy .\ffmpegExes\ffmpeg-x64.exe .\ffmpeg.exe -Force
    Compress-Archive .\ffmpeg.exe ./"$baseZipName.Full-x64.zip" -Update

    Write-Host "Done"
}
elseif ($Format -eq 'unix')
{
    Write-host "Creating 'unix' zip..." -NoNewline

    copy ./"$baseZipName.zip" ./"$baseZipName.Unix.zip"
    Compress-Archive ..\extras\utilities\BRC_unix.sh ./"$baseZipName.Unix.zip" -Update
    
    Write-Host "Done"
}


echo "Launch script finished"