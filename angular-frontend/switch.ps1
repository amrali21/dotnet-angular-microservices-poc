$nginxRoot = "C:\nginx-1.27.3\"
$nginxConfig = "$nginxRoot\conf\nginx.conf"

$rawFile = Get-Content   $nginxConfig

#iterate over the file to replace green and blue servers 
for($i=0; $i -le $rawFile.Length; $i++) {
    if($rawFile[$i] -match "mysites/blue")
    {
        $rawFile[$i] = $rawFile[$i] -replace "blue","green" 
    }elseif($rawFile[$i] -match "mysites/green")
    {
        $rawFile[$i] = $rawFile[$i] -replace "green","blue" 
    }
}
$rawFile | Set-Content $nginxConfig


#gracefully re-load nginx server
$currentLocation = Get-Location
Set-Location $nginxRoot
.\nginx -s reload

Set-Location $currentLocation

Write-Host "switched live and test"