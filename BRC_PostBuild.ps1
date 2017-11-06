param (
    [string]$TargetName,
    [string]$ConfigName
)

$OLDPWD = $PWD.Path
echo "Current path: $OLDPWD"

<#
if ($ConfigName.Length -eq 0)
{
    $ConfigName = $splitPath[$splitPath.Length - 1].TrimEnd()
}
#>

Write-Host "ConfigName is $ConfigName"

# Linux Vm Transfer folder
$vmBuildsDir = "F:\PCsVirtuais\_transfer\VS\builds"

#ILMerge path
$ilMerge = "F:\DEV\_Tools\ILMerge\ILMerge.exe"

# .NET reference paths
$net_refPath_v452 = "${env:ProgramFiles(x86)}\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2"


function Merge
{
    param (
        [ValidateLength(1,99)]
        [string[]] $Filter
        
    )

    if (!(Test-Path $ilMerge))
    {
        Write-Host "Error, IlMerge exe not found" -b Red -f White
        throw "ilMerge not found"
    }
    ''
    echo 'Running IlMerge'
    ''
    # run IlMerge
	Write-Output "$ilMerge /lib:$net_refPath_v452 /wildcards /targetplatform:v4 /target:winexe /out:$TargetName.all.exe $TargetName.exe $Filter"
    & "$ilMerge" /lib:"$net_refPath_v452" /wildcards /targetplatform:v4 /target:winexe /out:$TargetName.all.exe "$TargetName.exe" $Filter

    ''
    echo 'Cleanining old files:'
    ''

    del $Filter

    ren "$TargetName.all.pdb" "$TargetName.all.pdb.temp"
    del *.pdb
    ren "$TargetName.all.pdb.temp" "$TargetName.pdb"
    
    del "$TargetName.exe"
    ren "$TargetName.all.exe" "$TargetName.exe"

    del *.xml
    
    ''
    echo 'MERGE DONE'
}


if ($ConfigName.Contains("Unix"))
{
    del "Microsoft.WindowsAPICodePack*.dll", *.xml   
    $vmSubDir = $ConfigName.Substring(0, $ConfigName.IndexOf(' '))
    #xcopy .\* "$vmBuildsDir\$vmdSubDir\*" /Y /S /F
    Copy-Item -Path .\* -Destination $vmBuildsDir\$vmSubDir -Force -Recurse
}


if ($ConfigName.Equals("Release"))
{
    Merge -Filter @('Microsoft.WindowsAPICodePack*.dll', 'System.ValueTuple.dll')
} 

#Set-Location "$OLDPWD"

echo "PostBuild Done"